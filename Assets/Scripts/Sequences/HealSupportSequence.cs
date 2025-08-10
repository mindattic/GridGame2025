
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

        public override IEnumerator Execute()
        {
            // 1) Portrait3DManager pops in
            //yield return new PortraitPopInSequence(startPosition).ExecuteRoutine();

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
            yield return new FireProjectileSequence(healSettings).Execute();

            // 3) Portrait3DManager pops out
            //yield return new PortraitPopOutSequence(startPosition).ExecuteRoutine();
        }
    }
}
