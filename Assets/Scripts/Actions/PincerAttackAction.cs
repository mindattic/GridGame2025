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



        //Fields
        private PincerAttackPair pair;

        //Constructor
        public PincerAttackAction(PincerAttackPair pair)
        {
            this.pair = pair;
        }

        public override IEnumerator Execute()
        {
            //If no attacks were computed, exit early.
            if (pair.attacks == null || !pair.attacks.Any())
                yield break;

            //TODO: Create a consolidated model of PincerAttackPair and ActorPair
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
            var firstOpponent = pair.attacks.First().Opponent;
            var bumpDirection = pair.attacker1.GetDirectionTo(firstOpponent);

            //Perform bump and execute attacks at the apex
            var trigger = new Trigger(ProcessAttackSequence());
            yield return pair.attacker1.action.Bump(bumpDirection, trigger);

            //Trigger death animations on any opponents killed in last attack sequence
            yield return ProcessDeaths();
        }

        private IEnumerator ProcessAttackSequence()
        {
            foreach (var attack in pair.attacks)
            {
                var attacker = pair.attacker1;

                //Trigger the attack asynchronously (fire and forget).
                attacker.TriggerAttack(attack);

                //Short delay to create the domino effect.
                yield return Wait.For(Interval.TenthSecond);
            }
        }

        private IEnumerator ProcessDeaths()
        {
            //Wait until all dying actor's HP bars are fully drained
            var dyingActors = actors.Where(x => x.isDying).ToList();
            if (dyingActors.Any())
                yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));
        }

    }
}
