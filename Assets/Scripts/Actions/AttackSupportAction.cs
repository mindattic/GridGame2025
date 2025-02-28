using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Actions
{
    public class AttackSupportAction : PhaseAction
    {
        private ActorPair attackingPair;

        public AttackSupportAction(ActorPair attackingPair)
        {
            this.attackingPair = attackingPair;
        }

        public override IEnumerator Execute()
        {
            foreach (var supporter in attackingPair.actor1.supporters.Concat(attackingPair.actor2.supporters).Distinct())
            {
                foreach (var supportedActor in supporter.supporting)
                {
                    // Draw support line from supporter to the attacker they are supporting
                    GameManager.instance.supportLineManager.Spawn(supporter, supportedActor);

                    // If supporter is a Cleric, heal the specific attacker they are supporting
                    if (supporter.character == Character.Cleric)
                    {
                        GameManager.instance.spellManager.EnqueueHeal(supporter, supportedActor, castBeforeAttack: true);
                        yield return null;
                    }
                }
            }

            yield break;
        }
    }


}
