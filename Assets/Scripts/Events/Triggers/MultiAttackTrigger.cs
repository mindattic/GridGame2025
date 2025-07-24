using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.Scripts.Models;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    // Runs multiple attackResult events (like a pincer) as a single TriggerEvent
    public class MultiAttackTrigger : TriggerEvent
    {
        private List<AttackResult> attackResults;
        private ActorInstance attacker;

        public MultiAttackTrigger(ActorInstance attacker, List<AttackResult> results)
        {
            this.attacker = attacker;
            this.attackResults = results;
        }

        public override IEnumerator Run()
        {
            foreach (var attackResult in attackResults)
            {

                // Execute each attack fully before next
                yield return new SingleAttackTrigger(attackResult).Run();
                //yield return DeathHelper.Process();

                // Wait briefly before next attack in sequence
                yield return Wait.For(Interval.TenthSecond);
            }

            HasExecuted = true;
        }



    }
}
