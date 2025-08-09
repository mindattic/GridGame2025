using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class PincerAttackSupportSequence : SequenceEvent
    {
        private ActorInstance attacker;
        private ActorInstance supporter;

        // Single-constructor approach
        public PincerAttackSupportSequence(ActorInstance attacker, ActorInstance supporter)
        {
            this.attacker = attacker;
            this.supporter = supporter;
        }

        public override IEnumerator Execute()
        {
            // If supporter is a Cleric, heal the attacker
            if (supporter.characterName == CharacterHelper.Cleric)
                yield return new HealSupportSequence(supporter.position, attacker).Execute();
        }
    }
}
