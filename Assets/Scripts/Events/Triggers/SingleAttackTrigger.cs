using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;

public class SingleAttackTrigger : TriggerEvent
{
    protected VFXManager vfxManager => GameManager.instance.vfxManager;

    private AttackResult attackResult;
    private VisualEffectAsset attackVFX;

    public SingleAttackTrigger(AttackResult attack, VisualEffectAsset attackVFX)
    {
        this.attackResult = attack;
        this.attackVFX = attackVFX;
    }

    public override IEnumerator Run()
    {
        var hitOrMiss = new HitOrMissTrigger(attackResult);
        yield return vfxManager.Spawn(attackVFX, attackResult.Opponent.position, hitOrMiss);

        HasExecuted = true;
    }
}
