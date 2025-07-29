
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class HealAbilitySequence : SequenceEvent
    {
        private readonly Vector3 startPosition;
        private readonly ActorInstance targetActor;

        public HealAbilitySequence(Vector3 startPosition, ActorInstance targetActor)
        {
            this.startPosition = startPosition;
            this.targetActor = targetActor;
        }

        public override IEnumerator Execute()
        {
            g.InputManager.inputMode = InputMode.Cutscene;
            g.Card.BouncePortraitAsync();

            // Fire the heal projectile
            var healSettings = new ProjectileSettings
            {
                friendlyName = "Heal",
                startPosition = startPosition,
                target = targetActor,
                path = ProjectilePath.BezierCurve,
                controlPoints = BezierCurveHelper.Gentle(startPosition, targetActor),
                trailKey = "GreenSparkle",
                vfxKey = "BuffLife",
                trigger = new TriggerEvent(targetActor.Heal(10))
            };
            yield return new FireProjectileSequence(healSettings).Execute();

            // 3) PortraitManager pops out
            //yield return new PortraitPopOutSequence(startPosition).Execute();
        }
    }
}
