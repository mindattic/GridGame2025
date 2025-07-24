using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{

    public class SingleAttackTrigger : TriggerEvent
    {
        private readonly AttackResult attackResult;

        public SingleAttackTrigger(AttackResult attackResult)
        {
            this.attackResult = attackResult;
        }

        public override IEnumerator Run()
        {
            var damage = attackResult.Damage;
            attackResult.Opponent.TakeDamageAsync(damage);

            yield return null;
            HasExecuted = true;
        }
    }
}
