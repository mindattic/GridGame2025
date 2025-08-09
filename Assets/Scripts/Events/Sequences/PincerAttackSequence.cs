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

        public override IEnumerator Execute()
        {
            if (pair.attackResults1?.Any() != true || pair.attackResults2?.Any() != true)
                yield break;

            // Display attackers
            yield return g.Portrait2DManager.SpawnPair(
                new ActorPair(pair.attacker1, pair.attacker2)
            );

            // Grow both attackers simultaneously
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.action.Grow(),
                pair.attacker2.action.Grow()
            );

            // Shrink both attackers simultaneously
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.action.Shrink(),
                pair.attacker2.action.Shrink()
            );

            // Determine bump directions
            var firstOpponent = pair.attackResults1.First().Opponent;
  
            // Data MultiAttackTriggers
            var trigger1 = new MultiAttackTrigger(pair.attacker1, pair.attackResults1);
            var trigger2 = new MultiAttackTrigger(pair.attacker2, pair.attackResults2);

            // Bounce bumps, attaching MultiAttackTriggers
            yield return pair.attacker1.action.Bump(firstOpponent, trigger1);
            yield return pair.attacker2.action.Bump(firstOpponent, trigger2);

            //// Wait until both triggers complete all logic (VfxManager and damage)
            //yield return CoroutineHelper.WaitForAll(
            //    GameManager.instance,
            //    trigger1.Run(),
            //    trigger2.Run()
            //);

            //yield return CoroutineHelper.WaitForAll(
            //    GameManager.instance,
            //    pair.attacker1.action.Bump(dir1, trigger1),
            //    pair.attacker2.action.Bump(dir2, trigger2)
            //);

            // Process deaths afterward
            yield return DeathHelper.Process();
        }

    }
}
