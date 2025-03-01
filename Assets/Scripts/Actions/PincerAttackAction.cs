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

        // Constructor now takes PincerAttackParticipants (not ActorPair).
        public PincerAttackAction(PincerAttackParticipants participants)
        {
            this.participants = participants;
        }

        public override IEnumerator Execute()
        {
            // If you track the computed AttackResults in the PincerAttackParticipants:
            if (participants.attacks == null || !participants.attacks.Any())
                yield break;

            // Example: "grow" both attackers, then "shrink" them:
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

            // Track any opponents that die so we can wait for their death animations
            List<ActorInstance> dyingOpponents = new List<ActorInstance>();

            // For each AttackResult in the participants
            foreach (var attack in participants.attacks)
            {
                // In many cases you might want to choose which attacker is hitting which Opponent;
                // for example, you might vary it between attacker1 / attacker2.
                // For simplicity, here's an example using attacker1 each time:
                var attacker = participants.attacker1;

                var direction = attacker.GetDirectionTo(attack.Opponent);
                var trigger = new Trigger(attacker.Attack(attack));  // executes the actual Attack enumerator

                // Do the "bump" animation plus Attack
                yield return attacker.action.Bump(direction, trigger);

                // If Opponent was killed, queue up the final die steps
                if (attack.Opponent.isDying)
                {
                    dyingOpponents.Add(attack.Opponent);
                    attack.Opponent.TriggerDie();
                }
            }

            // Wait until all dying opponents' HP bar is fully drained
            //TODO: Add some sort of emergency release since this has a theoretical ability to fail...
            if (dyingOpponents.Any())
                yield return new WaitUntil(() => dyingOpponents.All(x => x.healthBar.isEmpty));

            yield break;
        }
    }
}
