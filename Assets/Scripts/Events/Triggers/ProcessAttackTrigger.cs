//using Assets.Scripts.Events;
//using Assets.Scripts.Models;
//using System.Collections;
//using g = GameManagerHelper;
//public class ProcessAttackTrigger : TriggerEvent
//{
//    private AttackResult attackResult;
  
//    public ProcessAttackTrigger(AttackResult attackResult)
//    {
//        this.attackResult = attackResult;
 
//    }

//    public override IEnumerator Run()
//    {
//        var vfx = attackResult.Attacker.vfx.Attack;
//        yield return new SingleAttackTrigger(attackResult, vfx).Run();

//        yield return DeathHelper.Process();
//    }

//}
