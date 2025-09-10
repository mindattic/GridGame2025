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

        private ActorInstance instance;
        private Vector3 baseScale;
        private float maxScale;   // 1.1f target
        private float speed;
        private Coroutine glowRoutineRef;

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;
            baseScale = Vector3.one; // scale of Glow renderer is independent; treat 1 as normal
            maxScale = 1.25f;
            speed = 2.0f;
        }

        private bool IsGlowing =>
            instance.IsPlaying && ((g.TurnManager.IsHeroTurn && isPlayer) || (g.TurnManager.IsEnemyTurn && isEnemy));

        public void Glow()
        {
            if (!instance.IsActive) return;
            if (glowRoutineRef != null) instance.StopCoroutine(glowRoutineRef);
            glowRoutineRef = instance.StartCoroutine(GlowRoutine());
        }

        public IEnumerator GlowRoutine()
        {
            // Ensure starting scale at 1
            render.SetGlowScale(baseScale);

            // Warm up to 1.1
            float warm = 0.15f;
            float t = 0f;
            while (t < warm)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / warm);
                float s = Mathf.Lerp(1f, maxScale, k);
                render.SetGlowScale(new Vector3(s, s, 1f));
                yield return Wait.OneTick();
            }

            // Pulse while glowing
            while (IsGlowing)
            {
                float curve = glowCurve != null && glowCurve.length > 0 ? glowCurve.Evaluate(Time.time * speed % glowCurve.length) : Mathf.Sin(Time.time * speed) * 0.05f;
                float s = maxScale + curve * 0.05f; // subtle +/- around 1.1
                s = Mathf.Clamp(s, 1f, 1.15f);
                render.SetGlowScale(new Vector3(s, s, 1f));
                yield return Wait.OneTick();
            }

            // Cooldown back to 1.0
            float cool = 0.15f; t = 0f;
            while (t < cool)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / cool);
                float s = Mathf.Lerp(maxScale, 1f, k);
                render.SetGlowScale(new Vector3(s, s, 1f));
                yield return Wait.OneTick();
            }

            render.SetGlowScale(baseScale);
            glowRoutineRef = null;
        }
    }
}
