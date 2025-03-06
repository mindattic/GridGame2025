using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    // PhaseAction for processing a single attacking PincerAttackPair.
    public class PincerAttackAction : PhaseAction
    {
        protected List<ActorInstance> actors = GameManager.instance.actors;


        private PincerAttackPair participants;

        // Constructor now takes PincerAttackPair (not ActorPair).
        public PincerAttackAction(PincerAttackPair participants)
        {
            this.participants = participants;
        }

        public override IEnumerator Execute()
        {
            // If no attacks were computed, exit early.
            if (participants.attacks == null || !participants.attacks.Any())
                yield break;

            // "Grow" both attackers, then "shrink" them:
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                participants.attacker1.action.Grow(),
                participants.attacker2.action.Grow()
            );
            yield return CoroutineHelper.WaitForAll(
                GameManager.instance,
                participants.attacker1.action.Shrink(),
                participants.attacker2.action.Shrink()
            );

            // Determine a bump direction based on the first opponent
            var firstOpponent = participants.attacks.First().Opponent;
            var bumpDirection = participants.attacker1.GetDirectionTo(firstOpponent);

            // Perform bump and execute attacks at the apex
            var trigger = new Trigger(ProcessAttackSequence());
            yield return participants.attacker1.action.Bump(bumpDirection, trigger);

            //Trigger death animations on any enemies killed in last attack sequence
            yield return ProcessDeaths();
        }

        private IEnumerator ProcessAttackSequence()
        {
            foreach (var attack in participants.attacks)
            {
                var attacker = participants.attacker1;

                //Trigger the attack asynchronously (fire and forget).
                attacker.TriggerAttack(attack);

                //Short delay to create the domino effect.
                yield return Wait.For(Interval.TenthSecond);
            }
        }

        private IEnumerator ProcessDeaths()
        {
            // Wait until all dying actor's' HP bars are fully drained
            var dyingActors = actors.Where(x => x.isDying).ToList();
            if (dyingActors.Any())
                yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));
        }

    }
}
