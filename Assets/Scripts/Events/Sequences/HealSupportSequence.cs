
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class HealSupportSequence : SequenceEvent
    {
        private readonly Vector3 source;
        private readonly ActorInstance target;

        public HealSupportSequence(Vector3 source, ActorInstance target)
        {
            this.source = source;
            this.target = target;
        }

        public override IEnumerator Execute()
        {
            // 1) PortraitManager pops in
            //yield return new PortraitPopInSequence(startPosition).Execute();

            // 2) Fire the heal projectile
            var healSettings = new ProjectileSettings
            {
                friendlyName = "Heal",
                startPosition = source,
                target = target,
                path = ProjectilePath.BezierCurve,
                controlPoints = BezierCurveHelper.Gentle(source, target),
                trailKey = "GreenSparkle",
                vfxKey = "BuffLife",
                trigger = new TriggerEvent(target.HealAsync(10))
            };
            yield return new FireProjectileSequence(healSettings).Execute();

            // 3) PortraitManager pops out
            //yield return new PortraitPopOutSequence(startPosition).Execute();
        }
    }
}
