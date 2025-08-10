
using Assets.Helper;
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

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

        public override IEnumerator ProcessRoutine()
        {
            // 1) Portrait3DManager pops in
            //yield return new PortraitPopInSequence(startPosition).ProcessRoutine();

            // 2) FireAndForget the heal projectile
            var healSettings = new ProjectileSettings
            {
                friendlyName = "Heal",
                startPosition = source,
                target = target,
                path = ProjectilePath.BezierCurve,
                controlPoints = BezierCurveHelper.Gentle(source, target),
                trailKey = "GreenSparkle",
                vfxKey = "BuffLife",
                routine = target.HealRoutine(10)
            };
            yield return new FireProjectileSequence(healSettings).ProcessRoutine();

            // 3) Portrait3DManager pops out
            //yield return new PortraitPopOutSequence(startPosition).ProcessRoutine();
        }
    }
}
