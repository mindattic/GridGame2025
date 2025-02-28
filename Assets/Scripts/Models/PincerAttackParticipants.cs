using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models
{
    public class PincerAttackParticipants
    {
        public List<ActorPair> AlignedPairs = new List<ActorPair>();

        public List<ActorPair> AttackingPairs
        {
            get { return AlignedPairs.Where(pair => pair.isAttacker).ToList(); }
        }

        public List<ActorPair> SupportingPairs
        {
            get { return AlignedPairs.Where(pair => pair.isSupporter).ToList(); }
        }


        public void Clear()
        {
            AlignedPairs.Clear();
        }

    }
}
