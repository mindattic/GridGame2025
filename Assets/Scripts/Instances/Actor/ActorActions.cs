using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using static Intermission.Before;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Instances.Actor
{
    // ActorActions encapsulates a collection of animated action for an actor actors,
    // such as shaking, dodging, bumping, growing, spinning, fading in, and weapon wiggle.
    // These action are implemented using coroutines that interpolate values over time.
    public class ActorActions
    {
        protected ActorRenderers render => instance.render;
        protected ActorStats stats => instance.stats;
        private bool isActive => instance.isActive;
        private bool isAlive => instance.isAlive;
        private bool isPlaying => instance.isPlaying;
        private Quaternion rotation { get => instance.rotation; set => instance.rotation = value; }
        private Vector3 position { get => instance.position; set => instance.position = value; }
        private Vector3 scale { get => instance.scale; set => instance.scale = value; }

        // Fields:
        // The parent actor actors this action module is controlling.
        private ActorInstance instance;
        // Parameters for the weapon wiggle action.
        private float wiggleFocus;
        private float wiggleAmplitude;

        /// <summary>
        /// Assign sets up this action module with its parent actor actors and calculates
        /// initial parameters for animations.
        /// </summary>
        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;

            // Determine wiggle speed and amplitude based on tile size.
            wiggleFocus = g.TileSize * 48f;
            wiggleAmplitude = 15f; // Maximum deviation (in degrees) for the weapon wiggle.
        }

        /// <summary>
        /// Triggers a shake action on the actor's thumbnail.
        /// A TriggerEvent parameter can specify intensity and duration.
        /// </summary>
        public void ShakeAsync(float intensity, float duration = 0, TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            // Start the Shake coroutine.
            instance.StartCoroutine(Shake(trigger));
        }

        /// <summary>
        /// Shake coroutine: Applies a randomized positional offset to the actor's thumbnail
        /// to simulate a shaking effect.
        /// </summary>
        private IEnumerator Shake(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Retrieve intensity and duration from the trigger.
            float intensity = ShakeIntensity.Low;
            float duration = 1f;

            // Repositories the original position of the actor's CurrentProfile tile.
            var originalPosition = instance.currentTile.position;
            float elapsedTime = 0f;

            // Abort if intensity or duration are zero or negative.
            if (intensity <= 0 || duration <= 0)
                yield break;

            // During: Continue shaking until the elapsed time reaches the specified duration.
            while (intensity > 0 && elapsedTime < duration)
            {
                // Calculate a random offset within the range defined by intensity.
                var shakeOffset = new Vector3(
                    Random.Float(-intensity, intensity),
                    Random.Float(-intensity, intensity),
                    0 // Keep z-axis unchanged.
                );

                // Apply the random offset to the thumbnail's position.
                instance.thumbnailPosition = originalPosition + shakeOffset;

                // Wait until the next frame.
                yield return Wait.OneTick();

                // Increment elapsed time by a fixed tick interval.
                if (duration > 0)
                    elapsedTime += Interval.OneTick;
            }

            // Optionally trigger any additional behavior via the trigger.
            yield return trigger.Execute(instance);

            // After shaking, restore the thumbnail's position to its original location.
            instance.thumbnailPosition = originalPosition;
        }

        /// <summary>
        /// Triggers the dodge action.
        /// </summary>
        public void DodgeAsync(TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            // Start the Dodge coroutine.
            instance.StartCoroutine(Dodge(trigger));
        }

        /// <summary>
        /// Dodge coroutine: Executes a two-phase dodge action where the actor twists forward
        /// then returns to the original orientation and scale.
        /// </summary>
        public IEnumerator Dodge(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Define action curves for rotation and scaling.
            var rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.9f);
            // Duration for forward twist and for return.
            float duration = 0.125f; // Phase 1 duration.
            float returnDuration = 0.125f; // Phase 2 duration.
            var startRotation = Vector3.zero;
            var targetRotation = new Vector3(15f, 70f, 15f);
            // Generate a random direction multiplier for each axis (-1 or 1).
            var randomDirection = new Vector3(
               Random.Boolean ? -1f : 1f,
               Random.Boolean ? -1f : 1f,
               Random.Boolean ? -1f : 1f);

            float elapsedTime = 0f;

            // Phase 1: Twist forward.
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);

                // Evaluate the rotation curve for a smooth transition.
                float curveValue = rotationCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.LerpUnclamped(startRotation, targetRotation, curveValue);
                // Apply the random twist direction.
                currentRotation.Scale(randomDirection);

                // Evaluate the scale curve to slightly reduce the size.
                float scaleFactor = scaleCurve.Evaluate(progress);
                scale = g.TileScale * scaleFactor;

                // Save the actor's rotation based on the computed rotation vector.
                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            // Phase 2: Return to original state.
            elapsedTime = 0f;
            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / returnDuration);

                // Reverse evaluate the rotation and scale to return to starting values.
                float curveValue = rotationCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.LerpUnclamped(targetRotation, startRotation, curveValue);
                currentRotation.Scale(randomDirection);

                float scaleFactor = Mathf.LerpUnclamped(0.9f, 1f, progress);
                scale = g.TileScale * scaleFactor;

                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            // Run TriggerEvent (if applicable)
            yield return trigger.Execute(instance);

            // Reset the actor's scale and rotation.
            scale = g.TileScale;
            rotation = Geometry.Rotation(Vector3.zero);
        }
        /// <summary>
        /// Initiates a bump animation in the specified direction.
        /// </summary>
        public void BumpAsync(ActorInstance target, TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            instance.StartCoroutine(Bump(target, trigger));
        }

        /// <summary>
        /// Executes a bump animation coroutine:
        /// 1. Moves actor slightly backward (windup).
        /// 2. Moves actor forward into bump apex (triggers VfxManager precisely at apex).
        /// 3. Returns actor smoothly to original position.
        /// </summary>
        public IEnumerator Bump(ActorInstance target, TriggerEvent trigger = default)
        {
            g.SortingManager.OnBump(instance, target);

            var direction = instance.GetDirectionTo(target);

            if (trigger == default)
                trigger = new TriggerEvent();

            // Animation curves for smooth transitions.
            var windupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var bumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            var windupDuration = 0.15f;
            var bumpDuration = 0.1f;
            var returnDuration = 0.3f;

            // Define key positions for animation phases.
            var startPosition = instance.currentTile.position;
            var windupPosition = Geometry.GetDirectionalPosition(startPosition, direction.Opposite(), g.TileSize * Increment.Percent33);
            var bumpPosition = Geometry.GetDirectionalPosition(startPosition, direction, g.TileSize * Increment.Percent33);

            float elapsedTime;

            // Phase 1: Windup (move backward slightly)
            elapsedTime = 0f;
            while (elapsedTime < windupDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / windupDuration);
                position = Vector3.Lerp(startPosition, windupPosition, windupCurve.Evaluate(progress));
                yield return Wait.OneTick();
            }

            // Ensure exact windup position
            position = windupPosition;

            // Phase 2: Bump forward into apex position
            elapsedTime = 0f;
            float targetRotationZ = (direction == Direction.East) ? -15f : 15f;

            while (elapsedTime < bumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / bumpDuration);
                position = Vector3.Lerp(windupPosition, bumpPosition, bumpCurve.Evaluate(progress));
                rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, targetRotationZ, progress));
                yield return Wait.OneTick();
            }

            // Explicitly set position and rotation at apex before VfxManager.
            position = bumpPosition;
            rotation = Quaternion.Euler(0, 0, targetRotationZ);

            trigger.ExecuteAsync(instance);
            // Spawn VfxManager at precise apex position.
            //g.VfxManager.Spawn(
            //    instance.vfx.Attack,
            //    instance.currentTile.position,
            //    trigger);

            // Phase 3: Return smoothly to original position
            elapsedTime = 0f;
            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / returnDuration);
                position = Vector3.Lerp(bumpPosition, startPosition, returnCurve.Evaluate(progress));
                rotation = Quaternion.Euler(0, 0, Mathf.Lerp(targetRotationZ, 0, progress));
                yield return Wait.OneTick();
            }

            // Ensure final clean state
            position = startPosition;
            rotation = Quaternion.identity;
        }


        /// <summary>
        /// Triggers a growth action, increasing the actor's scale up to a maximum size.
        /// </summary>
        public void GrowAsync(float maxSize = 0f, TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            if (instance.isActive)
                instance.StartCoroutine(Grow(trigger));
        }

        /// <summary>
        /// Grow coroutine: Increases the actor's scale gradually until a specified maximum size is reached.
        /// </summary>
        public IEnumerator Grow(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: determine target max size (default is 110% of tile size) and initial scale.
            float maxSize = g.TileSize * 1.1f;
            float minSize = scale.x;
            float increment = g.TileSize * 0.01f;
            float size = minSize;
            scale = new Vector3(size, size, 0);

            // During: Increase size incrementally.
            while (size < maxSize)
            {
                size += increment;
                size = Mathf.Clamp(size, minSize, maxSize);
                scale = new Vector3(size, size, 0);
                yield return Wait.OneTick();
            }

            // Optionally run any trigger-related coroutine.
            yield return trigger.Execute(instance);

            // After: Assign the scale exactly to max size.
            scale = new Vector3(maxSize, maxSize, 0);
        }

        /// <summary>
        /// Triggers a shrink action, decreasing the actor's scale down to a minimum size.
        /// </summary>
        public void TriggerShrink(float minSize = 0f, TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            if (instance.isActive)
                instance.StartCoroutine(Shrink(trigger));
        }

        /// <summary>
        /// Shrink coroutine: Decreases the actor's scale gradually until a specified minimum size is reached.
        /// </summary>
        public IEnumerator Shrink(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: determine target minimum size (default is tileSize) and CurrentProfile scale.
            float minSize = g.TileSize;
            float maxSize = scale.x;
            float increment = g.TileSize * 0.01f;
            float size = maxSize;
            scale = new Vector3(size, size, 0);

            // During: Decrease size incrementally.
            while (size > minSize)
            {
                size -= increment;
                size = Mathf.Clamp(size, minSize, maxSize);
                scale = new Vector3(size, size, 0);
                yield return Wait.OneTick();
            }

            // Run TriggerEvent (if applicable)
            yield return trigger.Execute(instance);

            // After: Ensure the scale is set exactly to the minimum size.
            scale = new Vector3(minSize, minSize, 0);
        }

        /// <summary>
        /// Triggers a 90-degree spin action.
        /// </summary>
        public void TriggerSpin90(TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            instance.StartCoroutine(Spin90(trigger));
        }

        /// <summary>
        /// Spin90 coroutine: Rotates the actor 90 degrees around the Y-axis and then reverses the rotation.
        /// </summary>
        private IEnumerator Spin90(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: Assign variables for rotation.
            bool isDone = false;
            var rotY = 0f;
            var spinFocus = g.TileSize * 24f;
            rotation = Geometry.Rotation(0, rotY, 0);

            // During: Rotate until reaching 90 degrees, then reverse until back at 0.
            while (!isDone)
            {
                rotY += !trigger.HasExecuted ? spinFocus : -spinFocus;

                if (!trigger.HasExecuted && rotY >= 90f)
                {
                    rotY = 90f;
                    // Run TriggerEvent (if applicable)
                    yield return trigger.Execute(instance);
                }

                isDone = trigger.HasExecuted && rotY <= 0f;
                if (isDone)
                    rotY = 0f;

                rotation = Geometry.Rotation(0, rotY, 0);
                yield return Wait.OneTick();
            }

            // After: Ensure rotation is reset.
            rotation = Geometry.Rotation(0, 0, 0);
        }

        /// <summary>
        /// Triggers a 360-degree spin action.
        /// </summary>
        public void TriggerSpin360(TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            instance.StartCoroutine(Spin360(trigger));
        }

        /// <summary>
        /// Spin360 coroutine: Rotates the actor 360 degrees around the Y-axis.
        /// </summary>
        private IEnumerator Spin360(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: Assign rotation variables.
            bool isDone = false;
            var rotY = 0f;
            var speed = g.TileSize * 24f;
            rotation = Geometry.Rotation(0, rotY, 0);

            // During: Increment rotation until a full 360-degree spin is completed.
            while (!isDone)
            {
                rotY += speed;
                rotation = Geometry.Rotation(0, rotY, 0);

                // TriggerEvent the event after 240 degrees have been rotated.
                if (!trigger.HasExecuted && rotY >= 240f)
                {
                    // Run TriggerEvent (if applicable)
                    yield return trigger.Execute(instance);
                }

                isDone = rotY >= 360f;
                yield return Wait.OneTick();
            }

            // After: Reset rotation to zero.
            rotation = Geometry.Rotation(0, 0, 0);
        }

        /// <summary>
        /// Triggers a fade-in action by gradually increasing the actor's render alpha.
        /// </summary>
        public void TriggerFadeIn(float delay = 0f, TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            instance.StartCoroutine(FadeIn(trigger));
        }

        /// <summary>
        /// FadeIn coroutine: Gradually increases the alpha value of the actor's render until fully opaque.
        /// </summary>
        private IEnumerator FadeIn(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: Retrieve delay and increment values from trigger attributes.
            float delay = 0f;
            float increment = 0.05f;
            float alpha = 0;
            render.SetAlpha(alpha);

            // Wait for the specified delay before starting the fade-in.
            yield return Wait.For(delay);

            // During: Increment alpha until it reaches 1.
            while (alpha < 1)
            {
                alpha += increment;
                alpha = Mathf.Clamp(alpha, 0, 1);
                render.SetAlpha(alpha);
                yield return Wait.OneTick();
            }

            // Run TriggerEvent (if applicable)
            yield return trigger.Execute(instance);

            // After: Ensure alpha is fully set.
            alpha = 1;
            render.SetAlpha(alpha);
        }

        /// <summary>
        /// Triggers a weapon wiggle action when the actor's action points are full.
        /// </summary>
        public void TriggerWeaponWiggle(TriggerEvent trigger = default)
        {
            // Only trigger wiggle if AP is full.
            if (stats.AP < stats.MaxAP || !isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            instance.StartCoroutine(WeaponWiggle(trigger));
        }

        /// <summary>
        /// WeaponWiggle coroutine: Makes the weapon icon oscillate based on a sine wave function.
        /// </summary>
        private IEnumerator WeaponWiggle(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: Assign the initial rotation for the weapon icon.
            float start = -45f;
            float rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            // During: While the actor's AP remains at maximum, wiggle the weapon icon.
            while (instance.stats.AP == instance.stats.MaxAP)
            {
                // Calculate the CurrentProfile rotation offset using a sine function.
                rotZ = start + Mathf.Sin(Time.time * wiggleFocus) * wiggleAmplitude;
                render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                yield return Wait.OneTick();
            }

            // Run TriggerEvent (if applicable)
            yield return trigger.Execute(instance);

            // After: Reset the weapon icon rotation.
            rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        /// <summary>
        /// Triggers a wiggle action on the turn delay textarea to indicate a delay before the actor's turn.
        /// </summary>
        public void TriggerTurnDelayWiggle(TriggerEvent trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new TriggerEvent();

            instance.StartCoroutine(TurnDelayWiggle(trigger));
        }

        /// <summary>
        /// TurnDelayWiggle coroutine: Oscillates the turn delay textarea with damping,
        /// then smoothly returns it to its original orientation.
        /// </summary>
        private IEnumerator TurnDelayWiggle(TriggerEvent trigger = default)
        {
            if (trigger == default)
                trigger = new TriggerEvent();

            // Before: Assign variables for oscillation.
            float timeElapsed = 0f;
            float amplitude = 10f;
            float dampingRate = 0.99f; // Controls how quickly the wiggle decays.
            float cutoff = 0.1f;       // Minimum amplitude to consider as zero.
            render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Phase 1: Rock back and forth with damping.
            while (amplitude > cutoff)
            {
                timeElapsed += Time.deltaTime;
                float rotZ = Mathf.Sin(timeElapsed * wiggleFocus) * amplitude;
                render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                amplitude *= dampingRate;
                yield return Wait.OneTick();
            }

            // Phase 2: Smoothly return to zero rotation.
            float currentZ = render.turnDelayText.transform.rotation.eulerAngles.z;
            while (Mathf.Abs(Mathf.DeltaAngle(currentZ, 0f)) > cutoff)
            {
                timeElapsed += Time.deltaTime * wiggleFocus;
                currentZ = Mathf.LerpAngle(currentZ, 0f, timeElapsed);
                render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, currentZ);
                yield return Wait.OneTick();
            }

            // Run TriggerEvent (if applicable)
            yield return trigger.Execute(instance);

            // After: Ensure the turn delay textarea rotation is reset.
            render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
