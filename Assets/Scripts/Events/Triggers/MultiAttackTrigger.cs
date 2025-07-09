using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

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
                // SingleAttackTrigger spawns VFX and applies damage via HitOrMissTrigger
                var singleAttack = new SingleAttackTrigger(attackResult, attacker.vfx.Attack);

                // Execute each attack fully before next
                yield return singleAttack.Run();

                // Wait briefly before next attack in sequence
                yield return Wait.For(Interval.QuarterSecond);
            }

            HasExecuted = true;
        }



    }
}
