using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Instances.Actor
{
    // ActorActions encapsulates a collection of animated actions for an actor instance,
    // such as shaking, dodging, bumping, growing, spinning, fading in, and weapon wiggle.
    // These actions are implemented using coroutines that interpolate values over time.
    public class ActorActions
    {
        // Quick Reference Properties:
        // Provides convenient access to the selected player, rendering module, actor stats, and more.
        protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
        protected ActorRenderers render => instance.render;
        protected ActorStats stats => instance.stats;
        private bool isActive => instance.isActive;
        private bool isAlive => instance.isAlive;
        protected float percent33 => Constants.percent33;
        protected float tileSize => GameManager.instance.tileSize;
        // Shortcuts to the actor's transform properties.
        private Quaternion rotation { get => instance.rotation; set => instance.rotation = value; }
        private Vector3 position { get => instance.position; set => instance.position = value; }
        private Vector3 scale { get => instance.scale; set => instance.scale = value; }
        protected Vector3 tileScale => GameManager.instance.tileScale;

        // Fields:
        // The parent actor instance this actions module is controlling.
        private ActorInstance instance;
        // Parameters for the weapon wiggle animation.
        private float wiggleSpeed;
        private float wiggleAmplitude;

        /// <summary>
        /// Initialize sets up this actions module with its parent actor instance and calculates
        /// initial parameters for animations.
        /// </summary>
        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;

            // Determine wiggle speed and amplitude based on tile size.
            wiggleSpeed = tileSize * 48f;
            wiggleAmplitude = 15f; // Maximum deviation (in degrees) for the weapon wiggle.
        }

        /// <summary>
        /// Triggers a shake animation on the actor's thumbnail.
        /// A Trigger parameter can specify intensity and duration.
        /// </summary>
        public void TriggerShake(float intensity, float duration = 0, Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();
            // Add intensity and duration attributes to the trigger.
            trigger.AddAttribute("intensity", intensity);
            trigger.AddAttribute("duration", duration);

            // Start the Shake coroutine.
            instance.StartCoroutine(Shake(trigger));
        }

        /// <summary>
        /// Shake coroutine: Applies a randomized positional offset to the actor's thumbnail
        /// to simulate a shaking effect.
        /// </summary>
        private IEnumerator Shake(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Retrieve intensity and duration from the trigger.
            float intensity = trigger.GetAttribute("intensity", 0f);
            float duration = trigger.GetAttribute("duration", 0f);
            // Store the original position of the actor's current tile.
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
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // After shaking, restore the thumbnail's position to its original location.
            instance.thumbnailPosition = originalPosition;
        }

        /// <summary>
        /// Triggers the dodge animation.
        /// </summary>
        public void TriggerDodge(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            // Start the Dodge coroutine.
            instance.StartCoroutine(Dodge(trigger));
        }

        /// <summary>
        /// Dodge coroutine: Executes a two-phase dodge animation where the actor twists forward
        /// then returns to the original orientation and scale.
        /// </summary>
        public IEnumerator Dodge(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Define animation curves for rotation and scaling.
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
                scale = tileScale * scaleFactor;

                // Update the actor's rotation based on the computed rotation vector.
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
                scale = tileScale * scaleFactor;

                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            // Optionally run any trigger-related coroutine after dodge.
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // Reset the actor's scale and rotation.
            scale = tileScale;
            rotation = Geometry.Rotation(Vector3.zero);
        }

        /// <summary>
        /// Triggers a bump animation in the given direction.
        /// </summary>
        public void TriggerBump(Direction direction, Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Bump(direction, trigger));
        }

        /// <summary>
        /// Bump coroutine: Simulates a bump by moving the actor slightly backward (windup),
        /// then forward with a rotation, and finally returning to the original position.
        /// </summary>
        public IEnumerator Bump(Direction direction, Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Define easing curves for each phase.
            var windupCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1); // For windup phase.
            var bumpCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);    // For the quick bump.
            var returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);      // For returning to start.
            var windupDuration = 0.15f;
            var bumpDuration = 0.1f;
            var returnDuration = 0.3f;
            // Get the actor's current position.
            var startPosition = instance.currentTile.position;
            // Calculate positions for windup (backward) and bump (forward) movements.
            var windupPosition = Geometry.GetDirectionalPosition(startPosition, direction.Opposite(), tileSize * percent33);
            var bumpPosition = Geometry.GetDirectionalPosition(startPosition, direction, tileSize * percent33);

            // Increase sorting order to bring the actor to the front.
            instance.sortingOrder = SortingOrder.Max;

            // Phase 1: Windup - move slightly opposite to the bump direction.
            float elapsedTime = 0f;
            while (elapsedTime < windupDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / windupDuration);
                float curveValue = windupCurve.Evaluate(progress);

                position = Vector3.Lerp(startPosition, windupPosition, curveValue);
                yield return Wait.OneTick();
            }

            // Phase 2: Bump - quickly move in the desired direction and apply a slight rotation.
            elapsedTime = 0f;
            // Choose a rotation angle: negative for East, positive for other directions.
            float targetRotationZ = (direction == Direction.East) ? -15f : 15f;
            while (elapsedTime < bumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / bumpDuration);
                float curveValue = bumpCurve.Evaluate(progress);

                position = Vector3.Lerp(windupPosition, bumpPosition, curveValue);
                rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, targetRotationZ, progress));
                yield return Wait.OneTick();
            }

            // Optionally run any trigger coroutine.
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // Phase 3: Return - smoothly return to the starting position and reset rotation.
            elapsedTime = 0f;
            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / returnDuration);
                float curveValue = returnCurve.Evaluate(progress);

                position = Vector3.Lerp(bumpPosition, startPosition, curveValue);
                rotation = Quaternion.Euler(0, 0, Mathf.Lerp(targetRotationZ, 0, progress));
                yield return Wait.OneTick();
            }

            // Ensure final position and rotation are reset.
            position = startPosition;
            rotation = Quaternion.identity;
        }

        /// <summary>
        /// Triggers a growth animation, increasing the actor's scale up to a maximum size.
        /// </summary>
        public void TriggerGrow(float maxSize = 0f, Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            if (maxSize != 0f)
                trigger.AddAttribute("maxSize", maxSize);

            if (instance.isActive)
                instance.StartCoroutine(Grow(trigger));
        }

        /// <summary>
        /// Grow coroutine: Increases the actor's scale gradually until a specified maximum size is reached.
        /// </summary>
        public IEnumerator Grow(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: determine target max size (default is 110% of tile size) and initial scale.
            float maxSize = trigger.GetAttribute("maxSize", tileSize * 1.1f);
            float minSize = scale.x;
            float increment = tileSize * 0.01f;
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
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // After: Set the scale exactly to max size.
            scale = new Vector3(maxSize, maxSize, 0);
        }

        /// <summary>
        /// Triggers a shrink animation, decreasing the actor's scale down to a minimum size.
        /// </summary>
        public void TriggerShrink(float minSize = 0f, Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            if (minSize != 0f)
                trigger.AddAttribute("minSize", minSize);

            if (instance.isActive)
                instance.StartCoroutine(Shrink(trigger));
        }

        /// <summary>
        /// Shrink coroutine: Decreases the actor's scale gradually until a specified minimum size is reached.
        /// </summary>
        public IEnumerator Shrink(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: determine target minimum size (default is tileSize) and current scale.
            float minSize = trigger.GetAttribute("minSize", tileSize);
            float maxSize = scale.x;
            float increment = tileSize * 0.01f;
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

            // Optionally run any trigger coroutine.
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // After: Ensure the scale is set exactly to the minimum size.
            scale = new Vector3(minSize, minSize, 0);
        }

        /// <summary>
        /// Triggers a 90-degree spin animation.
        /// </summary>
        public void TriggerSpin90(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Spin90(trigger));
        }

        /// <summary>
        /// Spin90 coroutine: Rotates the actor 90 degrees around the Y-axis and then reverses the rotation.
        /// </summary>
        private IEnumerator Spin90(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: Initialize variables for rotation.
            bool isDone = false;
            var rotY = 0f;
            var spinSpeed = tileSize * 24f;
            rotation = Geometry.Rotation(0, rotY, 0);

            // During: Rotate until reaching 90 degrees, then reverse until back at 0.
            while (!isDone)
            {
                rotY += !trigger.HasTriggered ? spinSpeed : -spinSpeed;

                if (!trigger.HasTriggered && rotY >= 90f)
                {
                    rotY = 90f;
                    trigger.SetContext(instance);
                    yield return trigger.StartCoroutine();
                }

                isDone = trigger.HasTriggered && rotY <= 0f;
                if (isDone)
                    rotY = 0f;

                rotation = Geometry.Rotation(0, rotY, 0);
                yield return Wait.OneTick();
            }

            // After: Ensure rotation is reset.
            rotation = Geometry.Rotation(0, 0, 0);
        }

        /// <summary>
        /// Triggers a 360-degree spin animation.
        /// </summary>
        public void TriggerSpin360(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Spin360(trigger));
        }

        /// <summary>
        /// Spin360 coroutine: Rotates the actor 360 degrees around the Y-axis.
        /// </summary>
        private IEnumerator Spin360(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: Initialize rotation variables.
            bool isDone = false;
            var rotY = 0f;
            var speed = tileSize * 24f;
            rotation = Geometry.Rotation(0, rotY, 0);

            // During: Increment rotation until a full 360-degree spin is completed.
            while (!isDone)
            {
                rotY += speed;
                rotation = Geometry.Rotation(0, rotY, 0);

                // Trigger the event after 240 degrees have been rotated.
                if (!trigger.HasTriggered && rotY >= 240f)
                {
                    trigger.SetContext(instance);
                    yield return trigger.StartCoroutine();
                }

                isDone = rotY >= 360f;
                yield return Wait.OneTick();
            }

            // After: Reset rotation to zero.
            rotation = Geometry.Rotation(0, 0, 0);
        }

        /// <summary>
        /// Triggers a fade-in animation by gradually increasing the actor's render alpha.
        /// </summary>
        public void TriggerFadeIn(float delay = 0f, Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            if (delay != 0f)
                trigger.AddAttribute("waitDuration", delay);

            instance.StartCoroutine(FadeIn(trigger));
        }

        /// <summary>
        /// FadeIn coroutine: Gradually increases the alpha value of the actor's render until fully opaque.
        /// </summary>
        private IEnumerator FadeIn(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: Retrieve delay and increment values from trigger attributes.
            float delay = trigger.GetAttribute("waitDuration", 0f);
            float increment = trigger.GetAttribute("increment", 0.05f);
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

            // Optionally trigger further actions.
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // After: Ensure alpha is fully set.
            alpha = 1;
            render.SetAlpha(alpha);
        }

        /// <summary>
        /// Triggers a weapon wiggle animation when the actor's action points are full.
        /// </summary>
        public void TriggerWeaponWiggle(Trigger trigger = default)
        {
            // Only trigger wiggle if AP is full.
            if (stats.AP < stats.MaxAP || !isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(WeaponWiggle(trigger));
        }

        /// <summary>
        /// WeaponWiggle coroutine: Makes the weapon icon oscillate based on a sine wave function.
        /// </summary>
        private IEnumerator WeaponWiggle(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: Set the initial rotation for the weapon icon.
            float start = -45f;
            float rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            // During: While the actor's AP remains at maximum, wiggle the weapon icon.
            while (instance.stats.AP == instance.stats.MaxAP)
            {
                // Calculate the current rotation offset using a sine function.
                rotZ = start + Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmplitude;
                render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                yield return Wait.OneTick();
            }

            // Optionally execute any trigger coroutine.
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // After: Reset the weapon icon rotation.
            rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        /// <summary>
        /// Triggers a wiggle animation on the turn delay text to indicate a delay before the actor's turn.
        /// </summary>
        public void TriggerTurnDelayWiggle(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(TurnDelayWiggle(trigger));
        }

        /// <summary>
        /// TurnDelayWiggle coroutine: Oscillates the turn delay text with damping,
        /// then smoothly returns it to its original orientation.
        /// </summary>
        private IEnumerator TurnDelayWiggle(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            // Before: Initialize variables for oscillation.
            float timeElapsed = 0f;
            float amplitude = 10f;
            float dampingRate = 0.99f; // Controls how quickly the wiggle decays.
            float cutoff = 0.1f;       // Minimum amplitude to consider as zero.
            render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Phase 1: Rock back and forth with damping.
            while (amplitude > cutoff)
            {
                timeElapsed += Time.deltaTime;
                float rotZ = Mathf.Sin(timeElapsed * wiggleSpeed) * amplitude;
                render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                amplitude *= dampingRate;
                yield return Wait.OneTick();
            }

            // Phase 2: Smoothly return to zero rotation.
            float currentZ = render.turnDelayText.transform.rotation.eulerAngles.z;
            while (Mathf.Abs(Mathf.DeltaAngle(currentZ, 0f)) > cutoff)
            {
                timeElapsed += Time.deltaTime * wiggleSpeed;
                currentZ = Mathf.LerpAngle(currentZ, 0f, timeElapsed);
                render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, currentZ);
                yield return Wait.OneTick();
            }

            // Optionally trigger any additional actions.
            trigger.SetContext(instance);
            yield return trigger.StartCoroutine();

            // After: Ensure the turn delay text rotation is reset.
            render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
