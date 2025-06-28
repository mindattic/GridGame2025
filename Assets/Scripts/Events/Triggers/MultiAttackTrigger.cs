using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Events
{
    // Runs multiple attackResult events (like a pincer) as a single TriggerEvent
    public class MultiAttackTrigger : TriggerEvent
    {
        private List<AttackResult> results;
        private ActorInstance attacker;

        public MultiAttackTrigger(ActorInstance attacker, List<AttackResult> results)
        {
            this.attacker = attacker;
            this.results = results;
        }

        public override IEnumerator Run()
        {
            foreach (var result in results)
            {
                var attack = new SingleAttackTrigger(result, attacker.vfx.Attack);
                yield return attack.Execute(attacker); // Run damage + VFX

                yield return Wait.For(Interval.QuarterSecond); // Add spacing between hits
            }
        }
    }
}
