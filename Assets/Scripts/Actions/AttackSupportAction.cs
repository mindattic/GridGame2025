using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Actions
{
    public class AttackSupportAction : PhaseAction
    {
        private ActorInstance attacker;
        private ActorInstance supporter;

        // Single-constructor approach
        public AttackSupportAction(ActorInstance attacker, ActorInstance supporter)
        {
            this.attacker = attacker;
            this.supporter = supporter;
        }

        public override IEnumerator Execute()
        {
            // Draw line from supporter to attacker
            GameManager.instance.supportLineManager.Spawn(supporter, attacker);

            // If supporter is a Cleric, heal the attacker
            if (supporter.character == Character.Cleric)
            {
                GameManager.instance.spellManager.EnqueueHeal(supporter, attacker, castBeforeAttack: true);
                yield return null;
            }

            yield break;
        }
    }
}
