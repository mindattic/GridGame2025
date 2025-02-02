using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Instances.Actor
{
    public class ActorGlow
    {
        //External properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected Vector3 tileScale => GameManager.instance.tileScale;
        protected ActorRenderers render => instance.render;
        private bool isActive => instance.isActive;
        private bool isAlive => instance.isAlive;
        private bool isPlayer => instance.isPlayer;
        private bool isEnemy => instance.isEnemy;
        protected AnimationCurve glowCurve => instance.glowCurve;
    
        //Fields

        private ActorInstance instance;
        private Vector3 initialScale;
        private float maxIntensity;
        private float speed;

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;

            initialScale = tileScale;
            maxIntensity = 1.5f;
            speed = 1.5f;
        }


        private bool IsGlowing => isActive && isAlive && turnManager.isStartPhase && (turnManager.isPlayerTurn && isPlayer) || (turnManager.isEnemyTurn && isEnemy);


        public void TriggerGlow()
        {
            if (isActive && isAlive)
                instance.StartCoroutine(Glow());
        }

        public IEnumerator Glow()
        {
            //Before:
            Vector3 scale = initialScale;
            render.SetGlowScale(scale);

            //During (Phase 1) - Warm Up:
            float warmupDuration = 1.0f; //Duration in seconds
            float elapsedWarmup = 0f;
            while (elapsedWarmup < warmupDuration)
            {
                elapsedWarmup += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedWarmup / warmupDuration);
                float intensity = Mathf.Lerp(1.0f, maxIntensity, progress);
                float curveValue = glowCurve.Evaluate(Time.time * speed % glowCurve.length);
                scale = new Vector3(
                    intensity + curveValue,
                    intensity + curveValue,
                    1.0f);
                render.SetGlowScale(scale);

                yield return Wait.OneTick();
            }

            //Ensure the scale ends exactly at maxIntensity:
            scale = new Vector3(
                maxIntensity,
                maxIntensity,
                1.0f);
            render.SetGlowScale(scale);

            //During (Phase 2) - Glowing:
            while (IsGlowing)
            {
                float curveValue = glowCurve.Evaluate(Time.time * speed % glowCurve.length);
                scale = new Vector3(
                    maxIntensity + curveValue,
                    maxIntensity + curveValue,
                    1.0f);
                render.SetGlowScale(scale);

                yield return Wait.OneTick();
            }

            //During (Phase 3) - Cooldown:
            float cooldownDuration = 1.0f; //Duration in seconds
            float elapsedCooldown = 0f;
            while (elapsedCooldown < cooldownDuration)
            {
                elapsedCooldown += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedCooldown / cooldownDuration);
                float intensity = Mathf.Lerp(maxIntensity, 1.0f, progress);
                float curveValue = glowCurve.Evaluate(Time.time * speed % glowCurve.length);
                scale = new Vector3(
                    intensity + curveValue,
                    intensity + curveValue,
                    1.0f);
                render.SetGlowScale(scale);

                yield return Wait.OneTick();
            }

            //After:
            scale = initialScale;
            render.SetGlowScale(scale);
        }




    }
}
