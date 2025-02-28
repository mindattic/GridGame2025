using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This context holds the aligned pairs for the attack phase.


// PhaseAction for processing support actions that occur BEFORE the attack.
public class PreAttackSupportAction : PhaseAction
{
    private PincerAttackContext context;
    public PreAttackSupportAction(PincerAttackContext ctx)
    {
        context = ctx;
    }
    public override IEnumerator Execute()
    {
        foreach (var supportPair in context.SupportingPairs)
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
public class AttackPairAction : PhaseAction
{
    private ActorPair pair;
    public AttackPairAction(ActorPair pair)
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
    private PincerAttackContext context;
    public PostAttackSupportAction(PincerAttackContext ctx)
    {
        context = ctx;
    }
    public override IEnumerator Execute()
    {
        foreach (var supportPair in context.SupportingPairs)
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

// Composite PhaseAction that enqueues the three sub-actions.
public class PincerAttackAction : PhaseAction
{
    private PincerAttackContext context = new PincerAttackContext();

    // Cached game manager components.
    private BoardOverlay BoardOverlay => GameManager.instance.boardOverlay;
    private TurnManager TurnManager => GameManager.instance.turnManager;
    private ActionManager ActionManager => GameManager.instance.actionManager;

    public override IEnumerator Execute()
    {
        // Build the attack context.
        ClearContext();
        ComputeContext();
        if (!context.AttackingPairs.Any())
        {
            TurnManager.NextTurn();
            yield break;
        }


        // Enqueue sub-actions.
        ActionManager.Add(new PreAttackSupportAction(context));
        foreach (var pair in context.AttackingPairs)
            ActionManager.Add(new AttackPairAction(pair));
        ActionManager.Add(new PostAttackSupportAction(context));

        BoardOverlay.TriggerFadeIn();
        yield return ActionManager.Execute();
        BoardOverlay.TriggerFadeOut();
        TurnManager.ResetSortingOrder();
        ClearContext();
        TurnManager.NextTurn();
        yield break;
    }

    // Computes aligned pairs from the player team and sets up attacking pairs.
    private void ComputeContext()
    {
        var teamMembers = GameManager.instance.players.Where(x => x.isPlaying);
        // Fill context.AlignedPairs using existing logic.
        foreach (var actor1 in teamMembers)
        {
            foreach (var actor2 in teamMembers)
            {
                if (actor1 == actor2)
                    continue;
                if (!(actor1.isPlaying && actor2.isPlaying))
                    continue;
                if (!(actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location)))
                    continue;
                // Avoid duplicates.
                if (context.AlignedPairs.Any(pair => pair.ContainsActorPair(actor1, actor2)))
                    continue;
                var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
                var pair = new ActorPair(actor1, actor2, axis);
                context.AlignedPairs.Add(pair);
            }
        }
        // For each pair that qualifies as an attacking pair, assign roles.
        foreach (var pair in context.AlignedPairs.Where(p => p.isAttacker))
        {
            pair.actor1.partner = pair.actor2;
            pair.attackResults = CalculateAttackResults(pair);
            pair.actor1.opponents = pair.attackResults.Select(a => a.Opponent).Distinct().ToList();
        }
    }

    private List<AttackResult> CalculateAttackResults(ActorPair pair)
    {
        return pair.opponents.Select(opponent =>
        {
            bool isHit = Formulas.IsHit(pair.actor1, opponent);
            bool isCriticalHit = Formulas.IsCriticalHit(pair.actor1, opponent);
            int damage = isHit ? Formulas.CalculateDamage(pair.actor1, opponent) : 0;
            return new AttackResult
            {
                Pair = pair,
                Opponent = opponent,
                IsHit = isHit,
                IsCriticalHit = isCriticalHit,
                Damage = damage,
            };
        }).ToList();
    }

    private void ClearContext()
    {
        foreach (var pair in context.AlignedPairs)
        {
            pair.actor1.partner = null;
            pair.actor1.opponents.Clear();
            pair.actor1.supporters.Clear();
            pair.actor2.partner = null;
            pair.actor2.opponents.Clear();
            pair.actor2.supporters.Clear();
        }
        context.AlignedPairs.Clear();
    }

}

