using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    //PhaseAction for processing a single attacking PincerAttackPair
    public class PincerAttackAction : PhaseAction
    {
        //Quick Reference Properties

        protected List<ActorInstance> actors = GameManager.instance.actors;
        protected PortraitManager portraitManager = GameManager.instance.portraitManager;
        protected SortingManager sortingManager = GameManager.instance.sortingManager;

        //Fields
        private PincerAttackPair pair;

        //Constructor
        public PincerAttackAction(PincerAttackPair pair)
        {
            this.pair = pair;
        }

        public override IEnumerator Execute()
        {
            //If no results were computed, exit early.
            if (pair.results == null || !pair.results.Any())
                yield break;

            var actorPair = new ActorPair(pair.attacker1, pair.attacker2);
            yield return portraitManager.Play(actorPair);

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

            //Determine a bump direction based on the first opponent
            var firstOpponent = pair.results.First().Opponent;
            var bumpDirection = pair.attacker1.GetDirectionTo(firstOpponent);

            //Perform bump and execute results at the apex
            var trigger = new Trigger(ProcessAttackSequence());
            yield return pair.attacker1.action.Bump(bumpDirection, trigger);

            //Trigger death animations on any opponents killed in last attack sequence
            yield return DeathHelper.Process();
        }

        private IEnumerator ProcessAttackSequence()
        {
            foreach (var result in pair.results)
            {
                var attacker = pair.attacker1;

                //Trigger the result asynchronously (fire and forget).
                attacker.TriggerAttack(result);

                //Short delay to create the domino effect.
                yield return Wait.For(Interval.QuarterSecond);
            }
        }

        //private IEnumerator ProcessDeaths()
        //{
        //    //Wait until all dying actor's HP bars are fully drained
        //    var dyingActors = actors.Where(x => x.isDying).ToList();
        //    if (dyingActors.Any())
        //        yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));
        //}

    }
}
