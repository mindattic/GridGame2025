using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if (pair.results1?.Any() != true || pair.results2?.Any() != true)
                yield break;

            // show the two attackers
            yield return portraitManager.SpawnPair(
                new ActorPair(pair.attacker1, pair.attacker2)
            );

            // grow & shrink both
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.animate.Grow(),
                pair.attacker2.animate.Grow()
            );
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.animate.Shrink(),
                pair.attacker2.animate.Shrink()
            );

            // figure out bump directions
            var firstOpponent = pair.results1.First().Opponent;
            var dir1 = pair.attacker1.GetDirectionTo(firstOpponent);
            var dir2 = pair.attacker2.GetDirectionTo(firstOpponent);

            // create triggers
            var trigger1 = new MultiAttackTrigger(pair.attacker1, pair.results1);
            var trigger2 = new MultiAttackTrigger(pair.attacker2, pair.results2);

            // fire off the visual bump + attach the trigger (returns void)
            pair.attacker1.animate.BumpAsync(dir1, trigger1);
            pair.attacker2.animate.BumpAsync(dir2, trigger2);

            // now wait for their attack sequences to actually run
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                trigger1.Run(),
                trigger2.Run()
            );

            // finally, handle any deaths
            yield return DeathHelper.Process();
        }
    }
}
