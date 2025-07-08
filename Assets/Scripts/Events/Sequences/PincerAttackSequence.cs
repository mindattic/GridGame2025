using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Events
{
    //SequenceEvent for processing a single attacking PincerAttackPair
    public class PincerAttackSequence : SequenceEvent
    {
        #region Game Properies

        protected List<ActorInstance> actors = GameManager.instance.actors;
        protected PortraitManager portraitManager = GameManager.instance.portraitManager;
        protected SortingManager sortingManager = GameManager.instance.sortingManager;
        protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
        #endregion

        //Fields
        private PincerAttackPair pair;

        //Constructor
        public PincerAttackSequence(PincerAttackPair pair)
        {
            this.pair = pair;
        }

        public override IEnumerator Execute()
        {
            // If either wave has no attack results, exit early
            if (pair.results1?.Any() != true || pair.results2?.Any() != true) 
                yield break;

            var actorPair = new ActorPair(pair.attacker1, pair.attacker2);
            yield return portraitManager.SpawnPair(actorPair);

            // "Grow" both attackers, then "shrink" them:
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

            // Determine direction to first opponent
            var firstOpponent = pair.results1.First().Opponent;
            var direction1 = pair.attacker1.GetDirectionTo(firstOpponent);
            var direction2 = pair.attacker2.GetDirectionTo(firstOpponent);

            // Attacker 1: Bump and trigger attackResult
            var trigger1 = new MultiAttackTrigger(pair.attacker1, pair.results1);
            pair.attacker1.animate.TriggerBump(direction1, trigger1);

            //yield return Wait.For(Interval.QuarterSecond); // Optional pause

            // Attacker 2: Bump and trigger attackResult
            var trigger2 = new MultiAttackTrigger(pair.attacker2, pair.results2);
            pair.attacker2.animate.TriggerBump(direction2, trigger2);


            //if (isDying)
            //    TriggerDie();

            // Trigger death animations after both attacks
            yield return DeathHelper.Process();




        }


    }
}
