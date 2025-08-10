
using Assets.Helper;
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class HealAbilitySequence : SequenceEvent
    {
        private readonly Vector3 startPosition;
        private readonly ActorInstance target;

        public HealAbilitySequence(Vector3 startPosition, ActorInstance targetActor)
        {
            this.startPosition = startPosition;
            this.target = targetActor;
        }

        public override IEnumerator ProcessRoutine()
        {
            g.InputManager.inputMode = InputMode.None;
            g.Card.BouncePortrait();

            // FireAndForget the heal projectile
            var healSettings = new ProjectileSettings
            {
                friendlyName = "Heal",
                startPosition = startPosition,
                target = target,
                path = ProjectilePath.BezierCurve,
                controlPoints = BezierCurveHelper.Gentle(startPosition, target),
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
