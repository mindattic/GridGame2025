using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class AttackAction : TurnAction
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
