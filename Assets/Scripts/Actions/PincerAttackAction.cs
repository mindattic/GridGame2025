using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



// PhaseAction for processing a single attacking actorPair.
public class PincerAttackAction : PhaseAction
{
    private ActorPair actorPair;
    public PincerAttackAction(ActorPair pair)
    {
        this.actorPair = pair;
    }
    public override IEnumerator Execute()
    {
        if (actorPair.attackResults == null || !actorPair.attackResults.Any())
            yield break;

        //GameManager.instance.attackLineManager.Spawn(actorPair);
        yield return CoroutineHelper.WaitForAll(GameManager.instance,
            actorPair.actor1.action.Grow(), actorPair.actor2.action.Grow());
        yield return CoroutineHelper.WaitForAll(GameManager.instance,
            actorPair.actor1.action.Shrink(), actorPair.actor2.action.Shrink());

        List<ActorInstance> dyingOpponents = new List<ActorInstance>();
        foreach (var attack in actorPair.attackResults)
        {
            var attacker = actorPair.actor1;
            var direction = attacker.GetDirectionTo(attack.Opponent);
            var trigger = new Trigger(attacker.Attack(attack));
            yield return attacker.action.Bump(direction, trigger);
            if (attack.Opponent.isDying)
            {
                dyingOpponents.Add(attack.Opponent);
                // Call static death handler.
                //yield return PincerAttackAction.HandleDeath(attack.Opponent);
                attack.Opponent.TriggerDie();
            }
        }
        if (dyingOpponents.Any())
            yield return new WaitUntil(() => dyingOpponents.All(x => x.healthBar.isEmpty));
        yield break;
    }
}
