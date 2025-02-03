using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class AttackResult
    {
        public ActorPair Pair;
        public ActorInstance Opponent;
        public bool IsHit;
        public bool IsCriticalHit;
        public int Damage;
        //public AttackResultTriggers Triggers;

        //Properties
        public bool IsMiss => !IsHit;

        //public AttackResult()
        //{
        //   Triggers = new AttackResultTriggers();
        //}
    }

    //public class AttackResultTriggers
    //{
    //   public AttackResultTriggers(Trigger before = default, Trigger after = default)
    //   {
    //       Before = before;
    //       After = after;
    //   }

    //   public Trigger Before = default;
    //   public Trigger After = default;
    //}


}
