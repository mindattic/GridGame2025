using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    // SequenceEvent for processing a single attacking PincerAttackPair
    public class PincerAttackSequence : SequenceEvent
    {
        private PincerAttackPair pair;

        public PincerAttackSequence(PincerAttackPair pair)
        {
            this.pair = pair;
        }

        public override IEnumerator ProcessRoutine()
        {
            if (pair.attackResults1?.Any() != true || pair.attackResults2?.Any() != true)
                yield break;

            // Display attackers
            yield return g.Portrait2DManager.SpawnPairRoutine(
                new ActorPair(pair.attacker1, pair.attacker2)
            );

            // GrowRoutine both attackers simultaneously
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.action.GrowRoutine(),
                pair.attacker2.action.GrowRoutine()
            );

            // Shrink both attackers simultaneously
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.action.ShrinkRoutine(),
                pair.attacker2.action.ShrinkRoutine()
            );

            // Choose an adjacent target per attacker
            var opp1 = Geometry.GetClosestOpponent(pair.attacker1, pair.attackResults1);
            var opp2 = Geometry.GetClosestOpponent(pair.attacker2, pair.attackResults2);

            // Build attack routines
            var routine1 = AttackHelper.MultiAttackRoutine(pair.attackResults1);
            var routine2 = AttackHelper.MultiAttackRoutine(pair.attackResults2);

            // Run bumps toward each attacker's own adjacent opponent
            yield return pair.attacker1.action.BumpRoutine(opp1, routine1);
            yield return pair.attacker2.action.BumpRoutine(opp2, routine2);
        }

    }
}
