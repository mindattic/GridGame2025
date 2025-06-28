using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class AttackResult
    {
        public ActorInstance Attacker;
        public ActorInstance Opponent;
        public bool IsHit;
        public bool IsCriticalHit;
        public int Damage;

        public bool IsMiss => !IsHit;
    }

}
