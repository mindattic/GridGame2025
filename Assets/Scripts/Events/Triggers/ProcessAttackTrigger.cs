using Assets.Scripts.Models;
using System.Collections;

public class ProcessAttackTrigger : TriggerEvent
{
    private ActorInstance attacker;
    private ActorInstance opponent;

    public ProcessAttackTrigger(ActorInstance attacker, ActorInstance opponent)
    {
        this.attacker = attacker;
        this.opponent = opponent;
    }

    public override IEnumerator Run()
    {
        var isHit = Formulas.IsHit(attacker, opponent);
        var isCriticalHit = Formulas.IsCriticalHit(attacker, opponent);
        var damage = Formulas.CalculateDamage(opponent, attacker);

        var result = new AttackResult
        {
            Attacker = attacker,
            Opponent = opponent,
            IsHit = isHit,
            IsCriticalHit = isCriticalHit,
            Damage = damage
        };

        var attack = new SingleAttackTrigger(result, attacker.vfx.Attack);
        yield return attack.Execute(attacker);

        yield return DeathHelper.Process();
    }

}
