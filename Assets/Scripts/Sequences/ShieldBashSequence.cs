using Assets.Helpers;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Sequences
{
    /// <summary>
    /// Paladin Shield Bash sequence: move to tile adjacent to target (toward Paladin),
    /// then perform a Bump with a supplied impact routine.
    /// </summary>
    public class ShieldBashSequence : SequenceEvent
    {
        private readonly ActorInstance paladin;
        private readonly ActorInstance target;

        public ShieldBashSequence(ActorInstance paladin, ActorInstance target)
        {
            this.paladin = paladin;
            this.target = target;
        }

        public override IEnumerator ProcessRoutine()
        {
            if (paladin == null || target == null || !paladin.IsPlaying || !target.IsPlaying)
                yield break;

            // Destination: adjacent to target along the axis toward paladin
            Direction dirFromTargetToPaladin;
            if (paladin.location.x == target.location.x)
                dirFromTargetToPaladin = (paladin.location.y > target.location.y) ? Direction.North : Direction.South;
            else
                dirFromTargetToPaladin = (paladin.location.x > target.location.x) ? Direction.East : Direction.West;

            var destLoc = Geometry.GetAdjacentLocationInDirection(target.location, dirFromTargetToPaladin);
            var destTile = g.TileMap.GetTile(destLoc);
            if (destTile == null || destTile.IsOccupied)
                yield break; // safety

            // Slide via actor movement routine
            paladin.location = destLoc;
            yield return paladin.StartCoroutine(paladin.Move.MoveTowardDestinationRoutine());

            // Bump -> damage placeholder
            yield return paladin.StartCoroutine(paladin.Animation.BumpRoutine(target, ShieldBashDamageRoutine()));
        }

        private IEnumerator ShieldBashDamageRoutine()
        {
            if (target != null && target.IsPlaying)
            {
                // Impact feedback at the bump apex: shake the target, then show damage feedback
                target.Animation.Shake(intensity: 0.6f, duration: 0.15f);
                g.CombatTextManager.Spawn("ShieldBash", target.Position, "Damage");
            }
            yield return null;
        }
    }
}
