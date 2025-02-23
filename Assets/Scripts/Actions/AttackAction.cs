using System.Collections;
using UnityEngine;
using Action = Assets.Scripts.Models.PhaseAction;

namespace Assets.Scripts.Models
{
    public class AttackAction : PhaseAction
    {
        private AttackResult attackResult;

        public AttackAction(AttackResult attackResult)
        {
   
            this.attackResult = attackResult;
  
        }

        public override IEnumerator Execute()
        {
            //Pass the full AttackResult to TakeDamage
            attackResult.Opponent.TakeDamage(attackResult);

            yield return null;
        }
    }



}
