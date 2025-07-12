using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using game = GameManagerHelper;

namespace Assets.Scripts.Events
{
    // SequenceEvent for processing a single attacking PincerAttackPair
    public class PincerAttackSequence : SequenceEvent
    {
        #region Game Properties

        protected PortraitManager portraitManager = GameManager.instance.portraitManager;
        #endregion

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
            yield return portraitManager.SpawnPair(
                new ActorPair(pair.attacker1, pair.attacker2)
            );

            // Grow both attackers simultaneously
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.animate.Grow(),
                pair.attacker2.animate.Grow()
            );

            // Shrink both attackers simultaneously
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.animate.Shrink(),
                pair.attacker2.animate.Shrink()
            );

            // Determine bump directions
            var firstOpponent = pair.attackResults1.First().Opponent;
            var dir1 = pair.attacker1.GetDirectionTo(firstOpponent);
            var dir2 = pair.attacker2.GetDirectionTo(firstOpponent);

            // Create MultiAttackTriggers
            var trigger1 = new MultiAttackTrigger(pair.attacker1, pair.attackResults1);
            var trigger2 = new MultiAttackTrigger(pair.attacker2, pair.attackResults2);

            // Start bumps, attaching MultiAttackTriggers
            yield return pair.attacker1.animate.Bump(dir1, trigger1);
            yield return pair.attacker2.animate.Bump(dir2, trigger2);

            // Wait until both triggers complete all logic (VFX and damage)
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                trigger1.Run(),
                trigger2.Run()
            );

            // Process deaths afterward
            yield return DeathHelper.Process();
        }

    }
}
