// --- File: Assets/Scripts/Instances/Actor/ActorActions.cs ---
using Assets.Helper;
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Instances.Actor
{
    /// <summary>
    /// ActorActions encapsulates animated actions for an actor such as shaking, dodging, bumping,
    /// growing, spinning, fading, and wiggles. All actions are coroutine based.
    /// </summary>
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

        private ActorInstance instance;

        private float wiggleFocus;
        private float wiggleAmplitude;

        /// <summary>
        /// Initializes this action module for the owning actor and prepares animation parameters.
        /// </summary>
        public void Initialize(ActorInstance parentInstance)
        {
            instance = parentInstance;

            wiggleFocus = g.TileSize * 48f;
            wiggleAmplitude = 15f;
        }

        /// <summary>
        /// Triggers a shake on the actor's thumbnail. Optional trigger routine runs after the shake completes.
        /// </summary>
        public void Shake(float intensity, float duration = 0f, IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(ShakeTrigger(intensity, duration, trigger));
        }

        /// <summary>
        /// Applies a randomized positional offset to simulate a shaking effect, then restores position.
        /// If a trigger routine is provided, it runs before restoration.
        /// </summary>
        private IEnumerator ShakeTrigger(float intensity, float duration, IEnumerator trigger = null)
        {
            var originalPosition = instance.currentTile.position;
            float elapsedTime = 0f;

            if (intensity <= 0f || duration <= 0f)
                yield break;

            while (intensity > 0f && elapsedTime < duration)
            {
                var shakeOffset = new Vector3(
                    RNG.Float(-intensity, intensity),
                    RNG.Float(-intensity, intensity),
                    0f
                );

                instance.thumbnailPosition = originalPosition + shakeOffset;

                yield return Wait.OneTick();

                if (duration > 0f)
                    elapsedTime += Interval.OneTick;
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            instance.thumbnailPosition = originalPosition;
        }

        /// <summary>
        /// Triggers the dodge action as a fire and forget. Optional trigger runs at the midpoint.
        /// </summary>
        public void Dodge(IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(DodgeTrigger(trigger));
        }

        /// <summary>
        /// Executes a two phase dodge where the actor twists forward then returns to the original state.
        /// If a trigger routine is provided, it runs after the forward twist completes.
        /// </summary>
        public IEnumerator DodgeTrigger(IEnumerator trigger = null)
        {
            var rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.9f);

            float duration = 0.125f;
            float returnDuration = 0.125f;

            var startRotation = Vector3.zero;
            var targetRotation = new Vector3(15f, 70f, 15f);

            var randomDirection = new Vector3(
               RNG.Boolean ? -1f : 1f,
               RNG.Boolean ? -1f : 1f,
               RNG.Boolean ? -1f : 1f
            );

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);

                float curveValue = rotationCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.LerpUnclamped(startRotation, targetRotation, curveValue);
                currentRotation.Scale(randomDirection);

                float scaleFactor = scaleCurve.Evaluate(progress);
                scale = g.TileScale * scaleFactor;

                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            elapsedTime = 0f;
            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / returnDuration);

                float curveValue = rotationCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.LerpUnclamped(targetRotation, startRotation, curveValue);
                currentRotation.Scale(randomDirection);

                float scaleFactor = Mathf.LerpUnclamped(0.9f, 1f, progress);
                scale = g.TileScale * scaleFactor;

                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            scale = g.TileScale;
            rotation = Geometry.Rotation(Vector3.zero);
        }

        /// <summary>
        /// Starts a bump animation toward the target. Optional trigger runs at the bump apex.
        /// </summary>
        public void Bump(ActorInstance target, IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(BumpTrigger(target, trigger));
        }

        /// <summary>
        /// BumpTrigger sequence:
        /// 1) Windup backward.
        /// 2) Lunge forward to apex and optionally run the trigger routine.
        /// 3) Return to start.
        /// </summary>
        public IEnumerator BumpTrigger(ActorInstance target, IEnumerator trigger = null)
        {
            g.SortingManager.OnBump(instance, target);

            var direction = instance.GetDirectionTo(target);

            var windupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var bumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            var windupDuration = 0.15f;
            var bumpDuration = 0.1f;
            var returnDuration = 0.3f;

            var startPosition = instance.currentTile.position;
            var windupPosition = Geometry.GetDirectionalPosition(startPosition, direction.Opposite(), g.TileSize * Increment.Percent33);
            var bumpPosition = Geometry.GetDirectionalPosition(startPosition, direction, g.TileSize * Increment.Percent33);

            float elapsedTime;

            elapsedTime = 0f;
            while (elapsedTime < windupDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / windupDuration);
                position = Vector3.Lerp(startPosition, windupPosition, windupCurve.Evaluate(progress));
                yield return Wait.OneTick();
            }

            position = windupPosition;

            elapsedTime = 0f;
            float targetRotationZ = (direction == Direction.East) ? -15f : 15f;

            while (elapsedTime < bumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / bumpDuration);
                position = Vector3.Lerp(windupPosition, bumpPosition, bumpCurve.Evaluate(progress));
                rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, targetRotationZ, progress));
                yield return Wait.OneTick();
            }

            position = bumpPosition;
            rotation = Quaternion.Euler(0f, 0f, targetRotationZ);

            if (trigger != null)
                instance.FireTrigger(trigger);

            // If you spawn VFX here, do it after reaching the apex.

            elapsedTime = 0f;
            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / returnDuration);
                position = Vector3.Lerp(bumpPosition, startPosition, returnCurve.Evaluate(progress));
                rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(targetRotationZ, 0f, progress));
                yield return Wait.OneTick();
            }

            position = startPosition;
            rotation = Quaternion.identity;
        }

        /// <summary>
        /// Triggers a growth action. Optional trigger runs after growth finishes.
        /// </summary>
        public void Grow(float maxSize = 0f, IEnumerator trigger = null)
        {
            if (!instance.isActive)
                return;

            instance.FireTrigger(GrowTrigger(maxSize, trigger));
        }

        /// <summary>
        /// Increases the actor scale up to a maximum, then optionally runs the trigger routine.
        /// </summary>
        public IEnumerator GrowTrigger(float maxSize = 0f, IEnumerator trigger = null)
        {
            float targetMax = maxSize > 0f ? maxSize : g.TileSize * 1.1f;
            float minSize = scale.x;
            float increment = g.TileSize * 0.01f;
            float size = minSize;
            scale = new Vector3(size, size, 0f);

            while (size < targetMax)
            {
                size += increment;
                size = Mathf.Clamp(size, minSize, targetMax);
                scale = new Vector3(size, size, 0f);
                yield return Wait.OneTick();
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            scale = new Vector3(targetMax, targetMax, 0f);
        }

        /// <summary>
        /// Triggers a shrink action. Optional trigger runs after shrink finishes.
        /// </summary>
        public void TriggerShrink(float minSize = 0f, IEnumerator trigger = null)
        {
            if (!instance.isActive)
                return;

            instance.FireTrigger(Shrink(minSize, trigger));
        }

        /// <summary>
        /// Decreases the actor scale down to a minimum, then optionally runs the trigger routine.
        /// </summary>
        public IEnumerator Shrink(float minSize = 0f, IEnumerator trigger = null)
        {
            float targetMin = minSize > 0f ? minSize : g.TileSize;
            float maxSize = scale.x;
            float increment = g.TileSize * 0.01f;
            float size = maxSize;
            scale = new Vector3(size, size, 0f);

            while (size > targetMin)
            {
                size -= increment;
                size = Mathf.Clamp(size, targetMin, maxSize);
                scale = new Vector3(size, size, 0f);
                yield return Wait.OneTick();
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            scale = new Vector3(targetMin, targetMin, 0f);
        }

        /// <summary>
        /// Triggers a 90 degree spin. Optional trigger runs at the 90 degree point.
        /// </summary>
        public void TriggerSpin90(IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(Spin90(trigger));
        }

        /// <summary>
        /// Rotates the actor 90 degrees around Y, optionally runs the trigger routine at 90,
        /// then rotates back to zero.
        /// </summary>
        private IEnumerator Spin90(IEnumerator trigger = null)
        {
            bool hasTriggered = false;
            float rotY = 0f;
            float spinFocus = g.TileSize * 24f;
            rotation = Geometry.Rotation(0f, rotY, 0f);

            bool isDone = false;
            while (!isDone)
            {
                rotY += !hasTriggered ? spinFocus : -spinFocus;

                if (!hasTriggered && rotY >= 90f)
                {
                    rotY = 90f;

                    if (trigger != null)
                        yield return instance.YieldRoutine(trigger);

                    hasTriggered = true;
                }

                isDone = hasTriggered && rotY <= 0f;
                if (isDone)
                    rotY = 0f;

                rotation = Geometry.Rotation(0f, rotY, 0f);
                yield return Wait.OneTick();
            }

            rotation = Geometry.Rotation(0f, 0f, 0f);
        }

        /// <summary>
        /// Triggers a 360 degree spin. Optional trigger runs after 240 degrees.
        /// </summary>
        public void Spin360(IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(Spin360Trigger(trigger));
        }

        /// <summary>
        /// Rotates the actor 360 degrees around Y. If a trigger routine is provided,
        /// it runs once after passing 240 degrees.
        /// </summary>
        private IEnumerator Spin360Trigger(IEnumerator trigger = null)
        {
            bool hasTriggered = false;
            float rotY = 0f;
            float speed = g.TileSize * 24f;
            rotation = Geometry.Rotation(0f, rotY, 0f);

            bool isDone = false;
            while (!isDone)
            {
                rotY += speed;
                rotation = Geometry.Rotation(0f, rotY, 0f);

                if (!hasTriggered && rotY >= 240f)
                {
                    if (trigger != null)
                        yield return instance.YieldRoutine(trigger);

                    hasTriggered = true;
                }

                isDone = rotY >= 360f;
                yield return Wait.OneTick();
            }

            rotation = Geometry.Rotation(0f, 0f, 0f);
        }

        /// <summary>
        /// Triggers a fade in by increasing renderer alpha. Optional trigger runs after fade completes.
        /// </summary>
        public void TriggerFadeIn(float delay = 0f, IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(FadeIn(delay, trigger));
        }

        /// <summary>
        /// Gradually increases alpha to 1. If a trigger routine is provided, it runs before finalizing.
        /// </summary>
        private IEnumerator FadeIn(float delay, IEnumerator trigger = null)
        {
            float increment = 0.05f;
            float alpha = 0f;
            render.SetAlpha(alpha);

            yield return Wait.For(delay);

            while (alpha < 1f)
            {
                alpha += increment;
                alpha = Mathf.Clamp(alpha, 0f, 1f);
                render.SetAlpha(alpha);
                yield return Wait.OneTick();
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            alpha = 1f;
            render.SetAlpha(alpha);
        }

        /// <summary>
        /// Triggers a weapon wiggle when AP is full. Optional trigger runs after wiggle stops.
        /// </summary>
        public void TriggerWeaponWiggle(IEnumerator trigger = null)
        {
            if (stats.AP < stats.MaxAP || !isActive || !isAlive)
                return;

            instance.FireTrigger(WeaponWiggle(trigger));
        }

        /// <summary>
        /// Oscillates the weapon icon while AP remains full, then optionally runs the trigger routine.
        /// </summary>
        private IEnumerator WeaponWiggle(IEnumerator trigger = null)
        {
            float start = -45f;
            float rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            while (instance.stats.AP == instance.stats.MaxAP)
            {
                rotZ = start + Mathf.Sin(Time.time * wiggleFocus) * wiggleAmplitude;
                render.weaponIcon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
                yield return Wait.OneTick();
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }

        /// <summary>
        /// Triggers a wiggle on the turn delay text with damping, then settles back to zero. Optional trigger runs after settle.
        /// </summary>
        public void TriggerTurnDelayWiggle(IEnumerator trigger = null)
        {
            if (!isActive || !isAlive)
                return;

            instance.FireTrigger(TurnDelayWiggle(trigger));
        }

        /// <summary>
        /// Oscillates the turn delay text with damping, then smoothly returns to zero. Optionally runs a trigger routine.
        /// </summary>
        private IEnumerator TurnDelayWiggle(IEnumerator trigger = null)
        {
            float timeElapsed = 0f;
            float amplitude = 10f;
            float dampingRate = 0.99f;
            float cutoff = 0.1f;
            render.turnDelayText.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            while (amplitude > cutoff)
            {
                timeElapsed += Time.deltaTime;
                float rotZ = Mathf.Sin(timeElapsed * wiggleFocus) * amplitude;
                render.turnDelayText.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
                amplitude *= dampingRate;
                yield return Wait.OneTick();
            }

            float currentZ = render.turnDelayText.transform.rotation.eulerAngles.z;
            while (Mathf.Abs(Mathf.DeltaAngle(currentZ, 0f)) > cutoff)
            {
                timeElapsed += Time.deltaTime * wiggleFocus;
                currentZ = Mathf.LerpAngle(currentZ, 0f, timeElapsed);
                render.turnDelayText.transform.rotation = Quaternion.Euler(0f, 0f, currentZ);
                yield return Wait.OneTick();
            }

            if (trigger != null)
                yield return instance.YieldRoutine(trigger);

            render.turnDelayText.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
