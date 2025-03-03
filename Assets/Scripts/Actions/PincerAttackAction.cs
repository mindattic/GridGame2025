using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    // PhaseAction for processing a single attacking PincerAttackParticipants.
    public class PincerAttackAction : PhaseAction
    {
        private PincerAttackParticipants participants;
        private List<ActorInstance> dyingOpponents = new List<ActorInstance>();

        // Constructor now takes PincerAttackParticipants (not ActorPair).
        public PincerAttackAction(PincerAttackParticipants participants)
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

            // Perform a single bump animation for attacker1
            yield return participants.attacker1.action.Bump(bumpDirection);

 
            // Process each attack in the computed attack chain with a short delay
            foreach (var attack in participants.attacks)
            {
                // Use attacker1 for each attack; adjust if needed
                var attacker = participants.attacker1;

                // Execute the attack animation and damage calculation directly
                yield return attacker.Attack(attack);

                // Short delay to create a domino effect
                yield return Wait.For(Interval.TenthSecond);

                //If the opponent was killed, handle death animation
                if (attack.Opponent.isDying)
                {
                    dyingOpponents.Add(attack.Opponent);
                    attack.Opponent.TriggerDie();
                }
            }

            // Wait until all dying opponents' HP bars are fully drained
            if (dyingOpponents.Any())
                yield return new WaitUntil(() => dyingOpponents.All(x => x.healthBar.isEmpty));

            yield break;
        }



    }
}
