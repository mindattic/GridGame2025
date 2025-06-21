using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class PincerAttackSupportEvent : GameEvent
    {
        private ActorInstance attacker;
        private ActorInstance supporter;

        // Single-constructor approach
        public PincerAttackSupportEvent(ActorInstance attacker, ActorInstance supporter)
        {
            this.attacker = attacker;
            this.supporter = supporter;
        }

        public override IEnumerator Execute()
        {
            // If supporter is a Cleric, heal the attacker
            if (supporter.characterName == CharacterHelper.Cleric)
            {
                GameManager.instance.projectileManager.EnqueueHeal(supporter, attacker);
                yield return null;
            }

            yield break;
        }
    }
}
