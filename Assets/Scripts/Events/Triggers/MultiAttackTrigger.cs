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
                var attack = new SingleAttackTrigger(attackResult, attacker.vfx.Attack);

                // start this attack running in the background
                attack.ExecuteAsync(attacker);

                // then wait a quarter-second before launching the next one
                yield return Wait.For(Interval.QuarterSecond);
            }
        }


    }
}
