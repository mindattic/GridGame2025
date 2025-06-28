using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;

public class ProcessAttackAsync : AsyncEvent
{
    ActorInstance attacker;
    ActorInstance opponent;

    public ProcessAttackAsync(ActorInstance attacker, ActorInstance opponent)
    {
        this.attacker = attacker;
        this.opponent = opponent;
    }

    public override IEnumerator Execute(MonoBehaviour context)
    {
        // Example logic — replace with your actual formula calls
        var isHit = Formulas.IsHit(attacker, opponent);
        var isCriticalHit = Formulas.IsCriticalHit(attacker, opponent);
        var damage = Formulas.CalculateDamage(opponent, attacker);

        var result = new AttackResult()
        {
            Opponent = opponent,
            IsHit = isHit,
            IsCriticalHit = isCriticalHit,
            Damage = damage
        };

        attacker.TriggerAttack(result);

        yield return DeathHelper.Process();

        HasExecuted = true;
    }
}
