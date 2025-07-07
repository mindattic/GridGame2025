
using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class HealSupportSequence : SequenceEvent
    {
        private readonly ActorInstance source;
        private readonly ActorInstance target;

        public HealSupportSequence(ActorInstance source, ActorInstance target)
        {
            this.source = source;
            this.target = target;
        }

        public override IEnumerator Execute()
        {
            // 1) Portrait pops in
            //yield return new PortraitPopInSequence(source).Execute();

            // 2) Fire the heal projectile
            var healSettings = new ProjectileSettings
            {
                friendlyName = "Heal",
                source = source,
                target = target,
                path = ProjectilePath.BezierCurve,
                controlPoints = BezierCurveHelper.Gentle(source, target),
                trailKey = "GreenSparkle",
                vfxKey = "BuffLife",
                trigger = new TriggerEvent(target.Heal(10))
            };
            yield return new FireProjectileSequence(healSettings).Execute();

            // 3) Portrait pops out
            //yield return new PortraitPopOutSequence(source).Execute();
        }
    }
}
