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
        //Quick Reference Properties

        protected List<ActorInstance> actors = GameManager.instance.actors;
        protected PortraitManager portraitManager = GameManager.instance.portraitManager;
        protected SortingManager sortingManager = GameManager.instance.sortingManager;
        protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;

        //Fields
        private PincerAttackPair pair;

        //Constructor
        public PincerAttackSequence(PincerAttackPair pair)
        {
            this.pair = pair;
        }

        public override IEnumerator Execute()
        {
            // If no results were computed, exit early.
            if (pair.results == null || !pair.results.Any())
                yield break;

            // Optional: show portraits
            // sequenceManager.Add(new PortraitPopInEvent(pair.attacker1));
            // sequenceManager.Add(new PortraitPopInEvent(pair.attacker2));

            var actorPair = new ActorPair(pair.attacker1, pair.attacker2);
            yield return portraitManager.SpawnPair(actorPair);

            // "Grow" both attackers, then "shrink" them:
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.action.Grow(),
                pair.attacker2.action.Grow()
            );
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                pair.attacker1.action.Shrink(),
                pair.attacker2.action.Shrink()
            );

            // Determine direction to first opponent
            var firstOpponent = pair.results.First().Opponent;
            var direction1 = pair.attacker1.GetDirectionTo(firstOpponent);
            var direction2 = pair.attacker2.GetDirectionTo(firstOpponent);

            // Split results by attacker
            var attacker1Results = pair.results
                .Where(r => r.Attacker == pair.attacker1)
                .ToList();

            var attacker2Results = pair.results
                .Where(r => r.Attacker == pair.attacker2)
                .ToList();

            // Attacker 1: Bump and trigger attackResult
            var trigger1 = new MultiAttackTrigger(pair.attacker1, attacker1Results);
            yield return pair.attacker1.action.Bump(direction1, trigger1);

            yield return Wait.For(Interval.QuarterSecond); // Optional pause

            // Attacker 2: Bump and trigger attackResult
            var trigger2 = new MultiAttackTrigger(pair.attacker2, attacker2Results);
            yield return pair.attacker2.action.Bump(direction2, trigger2);

            // Trigger death animations after both attacks
            yield return DeathHelper.Process();
        }


    }
}
