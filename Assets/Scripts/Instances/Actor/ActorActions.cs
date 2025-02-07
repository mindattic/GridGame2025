using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Instances.Actor
{
    public class ActorActions
    {
        //External properties
        protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
        protected ActorRenderers render => instance.render;
        protected ActorStats stats => instance.stats;
        private bool isActive => instance.isActive;
        private bool isAlive => instance.isAlive;
        protected float percent33 => Constants.percent33;
        protected float tileSize => GameManager.instance.tileSize;
        private int sortingOrder { get => instance.sortingOrder; set => instance.sortingOrder = value; }
        private Quaternion rotation { get => instance.rotation; set => instance.rotation = value; }
        private Vector3 position { get => instance.position; set => instance.position = value; }
        private Vector3 scale { get => instance.scale; set => instance.scale = value; }
        protected Vector3 tileScale => GameManager.instance.tileScale;
       
        //Fields

        private ActorInstance instance;

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;
        }

        public void TriggerShake(float intensity, float duration = 0, Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();
            trigger.AddAttribute("intensity", intensity);
            trigger.AddAttribute("duration", duration);

            instance.StartCoroutine(Shake(trigger));
        }

        private IEnumerator Shake(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            float intensity = trigger.GetAttribute("intensity", 0f);
            float duration = trigger.GetAttribute("duration", 0f);
            var originalPosition = instance.currentTile.position;
            var elapsedTime = 0f;

            if (intensity <= 0 || duration <= 0)
                yield break;

            //During:
            while (intensity > 0 && elapsedTime < duration)
            {
                //Calculate a random offset based on intensity
                var shakeOffset = new Vector3(
                    Random.Float(-intensity, intensity),
                    Random.Float(-intensity, intensity),
                    0 //Keep the z-axis stable
                );

                //Apply the offset to the thumbnailSettings position
                instance.thumbnailPosition = originalPosition + shakeOffset;

                //Wait for the next frame
                yield return Wait.OneTick();

                //TriggerFill elapsed time if duration is specified
                if (duration > 0)
                    elapsedTime += Interval.OneTick;
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //After:
            instance.thumbnailPosition = originalPosition;



        }

        public void TriggerDodge(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Dodge(trigger));
        }

        public IEnumerator Dodge(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Begin:
            var rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            var scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.9f);
            float duration = 0.125f; //Total duration for the forward twist
            float returnDuration = 0.125f; //Duration for the return to starting state
            var startRotation = Vector3.zero;
            var targetRotation = new Vector3(15f, 70f, 15f);
            var randomDirection = new Vector3(
               Random.Boolean ? -1f : 1f,
               Random.Boolean ? -1f : 1f,
               Random.Boolean ? -1f : 1f);

            float elapsedTime = 0f;

            //During (Phase 1) - Twist forward:
            while (elapsedTime < duration)
            {
                //Normalize time
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);

                //Evaluate rotation and scale using AnimationCurves
                float curveValue = rotationCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.LerpUnclamped(startRotation, targetRotation, curveValue);
                currentRotation.Scale(randomDirection); //Apply random twist direction

                float scaleFactor = scaleCurve.Evaluate(progress);
                scale = tileScale * scaleFactor;

                //Apply calculated transformations
                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            //During (Phase 2) - Initialize transition:
            elapsedTime = 0f;
            while (elapsedTime < returnDuration)
            {
                //Normalize time
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / returnDuration);

                //Reverse evaluate rotation and scale using AnimationCurves
                float curveValue = rotationCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.LerpUnclamped(targetRotation, startRotation, curveValue);
                currentRotation.Scale(randomDirection); //Apply reverse direction

                float scaleFactor = Mathf.LerpUnclamped(0.9f, 1f, progress);
                scale = tileScale * scaleFactor;

                //Apply calculated transformations
                rotation = Geometry.Rotation(currentRotation);

                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);


            //After:
            scale = tileScale;
            rotation = Geometry.Rotation(Vector3.zero);
        }

        public void TriggerBump(Direction direction, Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Bump(direction, trigger));
        }

        public IEnumerator Bump(Direction direction, Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before
            var windupCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1); //Windup easing
            var bumpCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1); //Fast movement
            var returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);   //Smooth return
            var windupDuration = 0.15f;
            var bumpDuration = 0.1f;
            var returnDuration = 0.3f;
            var startPosition = instance.currentTile.position;
            var windupPosition = Geometry.GetDirectionalPosition(startPosition, direction.Opposite(), tileSize * percent33);
            var bumpPosition = Geometry.GetDirectionalPosition(startPosition, direction, tileSize * percent33);

            //Increase sorting order to ensure this tile is on top
            sortingOrder = SortingOrder.Max;

            //Phase 1: Windup (move slightly in the opposite direction)
            float elapsedTime = 0f;
            while (elapsedTime < windupDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / windupDuration);
                float curveValue = windupCurve.Evaluate(progress);

                position = Vector3.Lerp(startPosition, windupPosition, curveValue);
                yield return Wait.OneTick();
            }

            //Phase 2: Bump (quickly move in the direction and rotate slightly)
            elapsedTime = 0f;
            float targetRotationZ = (direction == Direction.East) ? -15f : 15f; //Opposite rotation for East
            while (elapsedTime < bumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / bumpDuration);
                float curveValue = bumpCurve.Evaluate(progress);

                position = Vector3.Lerp(windupPosition, bumpPosition, curveValue);
                rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, targetRotationZ, progress));
                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //Phase 3: Return to Starting position (rotate back to zero and move back slowly)
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

            //Initialize sorting order and position
            sortingOrder = SortingOrder.Default;
            position = startPosition;
            rotation = Quaternion.identity;
        }


        public void TriggerGrow(float maxSize = 0f, Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            if (maxSize != 0f)
                trigger.AddAttribute("maxSize", maxSize);

            if (instance.isActive)
                instance.StartCoroutine(Grow(trigger));
        }

        public IEnumerator Grow(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            float maxSize = trigger.GetAttribute("maxSize", tileSize * 1.1f);
            float minSize = scale.x;
            float increment = tileSize * 0.01f;
            float size = minSize;
            scale = new Vector3(size, size, 0);

            //During:
            while (size < maxSize)
            {
                size += increment;
                size = Mathf.Clamp(size, minSize, maxSize);
                scale = new Vector3(size, size, 0);
                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //After:
            scale = new Vector3(maxSize, maxSize, 0);
        }

        public void TriggerShrink(float minSize = 0f, Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            if (minSize != 0f)
                trigger.AddAttribute("minSize", minSize);

            if (instance.isActive)
                instance.StartCoroutine(Shrink(trigger));
        }

        public IEnumerator Shrink(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            float minSize = trigger.GetAttribute("minSize", tileSize);
            float maxSize = scale.x;
            float increment = tileSize * 0.01f;
            float size = maxSize;
            scale = new Vector3(size, size, 0);

            //During:
            while (size > minSize)
            {
                size -= increment;
                size = Mathf.Clamp(size, minSize, maxSize);
                scale = new Vector3(size, size, 0);
                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //After:
            scale = new Vector3(minSize, minSize, 0);
        }

        public void TriggerSpin90(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Spin90(trigger));
        }

        private IEnumerator Spin90(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            bool isDone = false;
            var rotY = 0f;
            var spinSpeed = tileSize * 24f;
            rotation = Geometry.Rotation(0, rotY, 0);

            //During:
            while (!isDone)
            {
                rotY += !trigger.HasTriggered ? spinSpeed : -spinSpeed;

                if (!trigger.HasTriggered && rotY >= 90f)
                {
                    rotY = 90f;

                    //Trigger coroutine (if applicable):
                    yield return trigger.StartCoroutine(instance);
                }

                isDone = trigger.HasTriggered && rotY <= 0f;
                if (isDone)
                    rotY = 0f;

                rotation = Geometry.Rotation(0, rotY, 0);
                yield return Wait.OneTick();
            }

            //After:
            rotation = Geometry.Rotation(0, 0, 0);

        }

        public void TriggerSpin360(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(Spin360(trigger));
        }

        private IEnumerator Spin360(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            bool isDone = false;
            var rotY = 0f;
            var speed = tileSize * 24f;
            rotation = Geometry.Rotation(0, rotY, 0);

            //During:
            while (!isDone)
            {
                rotY += speed;
                rotation = Geometry.Rotation(0, rotY, 0);

                //Trigger event and startDelay for it to finish (if applicable)
                if (!trigger.HasTriggered && rotY >= 240f)
                {
                    //Trigger coroutine (if applicable):
                    yield return trigger.StartCoroutine(instance);
                }

                isDone = rotY >= 360f;
                yield return Wait.OneTick();
            }

            //After:
            rotation = Geometry.Rotation(0, 0, 0);
        }

        public void TriggerFadeIn(float delay = 0f, Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            if (delay != 0f)
                trigger.AddAttribute("delay", delay);

            instance.StartCoroutine(FadeIn(trigger));
        }

        private IEnumerator FadeIn(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            float delay = trigger.GetAttribute("delay", 0f);
            float increment = trigger.GetAttribute("increment", 0.05f);
            float alpha = 0;
            render.SetAlpha(alpha);

            yield return Wait.For(delay);

            //During:
            while (alpha < 1)
            {
                alpha += increment;
                alpha = Mathf.Clamp(alpha, 0, 1);
                render.SetAlpha(alpha);
                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //After:
            alpha = 1;
            render.SetAlpha(alpha);
        }

        public void TriggerWeaponWiggle(Trigger trigger = default)
        {
            if (stats.AP < stats.MaxAP || !isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(WeaponWiggle(trigger));
        }

        private IEnumerator WeaponWiggle(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            float start = -45f;
            float rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            //During:
            while (instance.stats.AP == instance.stats.MaxAP)
            {
                //Calculate rotation angle using sine wave
                rotZ = start + Mathf.Sin(Time.time * instance.wiggleSpeed) * instance.wiggleAmplitude;

                //Apply the rotation
                render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);

                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //After:
            rotZ = start;
            render.weaponIcon.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        public void TriggerTurnDelayWiggle(Trigger trigger = default)
        {
            if (!isActive || !isAlive)
                return;

            if (trigger == default)
                trigger = new Trigger();

            instance.StartCoroutine(TurnDelayWiggle(trigger));
        }

        private IEnumerator TurnDelayWiggle(Trigger trigger = default)
        {
            if (trigger == default)
                trigger = new Trigger();

            //Before:
            float timeElapsed = 0f;
            float amplitude = 10f;
            float speed = instance.wiggleSpeed; //Wiggle spinSpeed
            float dampingRate = 0.99f; //Factor to reduce amplitude each cycle (closer to 1 = slower decay)
            float cutoff = 0.1f;

            render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, 0);

            //During (Phase 1) - Rock back and forth:
            while (amplitude > cutoff)
            {
                timeElapsed += Time.deltaTime;
                float rotZ = Mathf.Sin(timeElapsed * speed) * amplitude;
                render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                amplitude *= dampingRate;

                yield return Wait.OneTick();
            }

            //During (Phase 2) - Smoothly return to zero rotation:
            float currentZ = render.turnDelayText.transform.rotation.eulerAngles.z;
            while (Mathf.Abs(Mathf.DeltaAngle(currentZ, 0f)) > cutoff)
            {
                timeElapsed += Time.deltaTime * speed;
                currentZ = Mathf.LerpAngle(currentZ, 0f, timeElapsed);
                render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, currentZ);
                yield return Wait.OneTick();
            }

            //Trigger coroutine (if applicable):
            yield return trigger.StartCoroutine(instance);

            //After
            render.turnDelayText.transform.rotation = Quaternion.Euler(0, 0, 0);

        }



    }
}
