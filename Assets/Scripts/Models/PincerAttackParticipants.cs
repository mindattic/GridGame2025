using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// Each pair of attackers (e.g. A and B), plus the opponents they sandwich
    /// and any supporters for each attacker.
    /// </summary>
    public class PincerAttackParticipants
    {
        public ActorInstance attacker1;
        public ActorInstance attacker2;

        // Enemies (opponents) in between attacker1 and attacker2
        public List<ActorInstance> opponents = new();

        // Attack result data stored here, so PincerAttackAction can see it
        public List<AttackResult> attacks = new();

        // Potential same-team supporters who have clear line of sight to each attacker
        public List<ActorInstance> supportersOfAttacker1 = new();
        public List<ActorInstance> supportersOfAttacker2 = new();
    }
}
