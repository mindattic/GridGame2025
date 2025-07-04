using Assets.Scripts.Models;
using System.Collections;

public class HitOrMissTrigger : TriggerEvent
{
    private AttackResult attackResult;

    public HitOrMissTrigger(AttackResult attack)
    {
        this.attackResult = attack;
    }

    public override IEnumerator Run()
    {
        if (attackResult.IsHit)
        {
            // Kick off the opponent’s damage coroutine for real:
            attackResult.Opponent.TriggerTakeDamage(attackResult);
        }
        else
        {
            // AttackMiss returns an IEnumerator we *do* want to yield,
            // because it has its own dodge animate.
            yield return attackResult.Opponent.AttackMiss();
        }
    }
}
