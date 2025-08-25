using Assets.Helper;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Instances.Actor
{
    public class ActorGlow
    {
        protected ActorRenderers render => instance.Render;
        private bool isActive => instance.IsActive;
        private bool isAlive => instance.IsAlive;
        private bool isPlayer => instance.IsHero;
        private bool isEnemy => instance.IsEnemy;
        protected AnimationCurve glowCurve => instance.glowCurve;

        //Fields

        private ActorInstance instance;
        private Vector3 initialScale;
        private float maxIntensity;
        private float speed;

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;

            initialScale = g.TileScale;
            maxIntensity = 1.5f;
            speed = 1.5f;
        }


        private bool IsGlowing =>
            instance.IsPlaying
            && (g.TurnManager.IsHeroTurn && isPlayer) || (g.TurnManager.isEnemyTurn && isEnemy);


        public void Glow()
        {
            if (instance.IsActive)
                instance.StartCoroutine(GlowRoutine());
        }

        public IEnumerator GlowRoutine()
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
