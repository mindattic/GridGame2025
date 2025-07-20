using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using game = GameManagerHelper;

public class SingleAttackTrigger : TriggerEvent
{
    #region Game Properties
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    #endregion

    private AttackResult attackResult;
    private VFXAsset attackVFX;

    public SingleAttackTrigger(AttackResult attack, VFXAsset attackVFX)
    {
        this.attackResult = attack;
        this.attackVFX = attackVFX;
    }

    public override IEnumerator Run()
    {
        // Data damage trigger
        var takeDamage = new TakeDamageTriggerEvent(attackResult);

        // Spawn VFX, and chain damage trigger afterward
        vfxManager.SpawnAsync(attackVFX, attackResult.Opponent.position, takeDamage);

        yield return null;
        HasExecuted = true;
    }
}
