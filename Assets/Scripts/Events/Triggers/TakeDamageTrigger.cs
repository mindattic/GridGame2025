//using Assets.Scripts.Events;
//using Assets.Scripts.Models;
//using System.Collections;
//using g = Assets.Helpers.GameManagerHelper;
//public class TakeDamageTriggerEvent : TriggerEvent
//{
//    private AttackResult attackResult;

//    public TakeDamageTriggerEvent(AttackResult attackResult)
//    {
//        this.attackResult = attackResult;
//    }

//    public override IEnumerator Run()
//    {
//        attackResult.Opponent.TakeDamageAsync(attackResult);
//        yield return null;

//        HasExecuted = true;
//    }
//}
