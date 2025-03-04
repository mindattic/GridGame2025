using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// Each pair of attackers (e.g. A and B), plus the opponents they sandwich
    /// and any supporters for each attacker.
    /// </summary>
    public class PincerAttackPair
    {
        public ActorInstance attacker1;
        public ActorInstance attacker2;

        // Enemies (opponents) in between attacker1 and attacker2
        public List<ActorInstance> opponents = new();

        // Attack result data stored here, so PincerAttackAction can see it
        public List<AttackResult> attacks = new();

        // Potential same-team supporters who have Clear line of sight to each attacker
        public List<ActorInstance> supporters1 = new();
        public List<ActorInstance> supporters2 = new();
    }

    /// <summary>
    /// A container with all of the "bookend pairs" found for x certain team.
    /// </summary>
    public class PincerAttackParticipants
    {
        public List<PincerAttackPair> pair = new();
        public void Clear() => pair.Clear();
    }

}
