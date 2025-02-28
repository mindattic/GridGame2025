using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class PincerAttackContext
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
    }
}
