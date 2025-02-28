using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This participants holds the aligned pairs for the attack phase.


// PhaseAction for processing support actions that occur BEFORE the attack.
public class PreAttackSupportAction : PhaseAction
{
    private PincerAttackParticipants participants;
    public PreAttackSupportAction(PincerAttackParticipants participants)
    {
        this.participants = participants;
    }
    public override IEnumerator Execute()
    {
        foreach (var supportPair in participants.SupportingPairs)
        {
            ActorInstance a = supportPair.actor1;
            ActorInstance b = supportPair.actor2;
            // They support each other if they aren’t paired as attackers.
            if (a.partner != b)
            {
                if (!a.supporters.Contains(b))
                    a.supporters.Add(b);
                if (!b.supporters.Contains(a))
                    b.supporters.Add(a);
                // Enqueue healing if a is a Cleric.
                if (a.character == Character.Cleric)
                {
                    GameManager.instance.spellManager.EnqueueHeal(a, b, castBeforeAttack: true);
                    yield return null;
                }
                if (b.character == Character.Cleric)
                {
                    GameManager.instance.spellManager.EnqueueHeal(b, a, castBeforeAttack: true);
                    yield return null;
                }
            }
            // Always spawn support visual.
            GameManager.instance.supportLineManager.Spawn(supportPair);
            yield return null;
        }
        yield break;
    }
}

// PhaseAction for processing a single attacking pair.
public class PincerAttackAction : PhaseAction
{
    private ActorPair pair;
    public PincerAttackAction(ActorPair pair)
    {
        this.pair = pair;
    }
    public override IEnumerator Execute()
    {
        if (pair.attackResults == null || !pair.attackResults.Any())
            yield break;

        GameManager.instance.attackLineManager.Spawn(pair);
        yield return CoroutineHelper.WaitForAll(GameManager.instance,
            pair.actor1.action.Grow(), pair.actor2.action.Grow());
        yield return CoroutineHelper.WaitForAll(GameManager.instance,
            pair.actor1.action.Shrink(), pair.actor2.action.Shrink());

        List<ActorInstance> dyingOpponents = new List<ActorInstance>();
        foreach (var attack in pair.attackResults)
        {
            var attacker = pair.actor1;
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

// PhaseAction for processing support actions that occur AFTER the attack.
public class PostAttackSupportAction : PhaseAction
{
    private PincerAttackParticipants participants;

    public PostAttackSupportAction(PincerAttackParticipants participants)
    {
        this.participants = participants;
    }

    public override IEnumerator Execute()
    {
        foreach (var supportPair in participants.SupportingPairs)
        {
            ActorInstance a = supportPair.actor1;
            ActorInstance b = supportPair.actor2;
            if (a.partner != b)
            {
                if (a.character == Character.Cleric)
                {
                    GameManager.instance.spellManager.EnqueueHeal(a, b, castBeforeAttack: false);
                    yield return null;
                }
                if (b.character == Character.Cleric)
                {
                    GameManager.instance.spellManager.EnqueueHeal(b, a, castBeforeAttack: false);
                    yield return null;
                }
            }
            yield return null;
        }
        yield break;
    }
}

