using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttackAction : TurnAction
{
    // A local instance for tracking combat participants.
    private CombatParticipants participants = new CombatParticipants();

    public override IEnumerator Execute()
    {
        participants.Clear();

        // Use the player team for combat.
        IQueryable<ActorInstance> teamMembers = GameManager.instance.players;

        if (!AssignParticipants(teamMembers))
        {
            // No valid aligned pairs; simply exit.
            yield break;
        }

        SetupCombatState();

        foreach (var pair in participants.attackingPairs)
        {
            // Spawn an attack line for visual effect.
            GameManager.instance.attackLineManager.Spawn(pair);
            yield return ResolveAttack(pair);
        }

        CleanupCombatState();

        yield break;
    }

    // Returns true if there is at least one valid attacking pair.
    private bool AssignParticipants(IQueryable<ActorInstance> teamMembers)
    {
        AssignAlignedPairs(teamMembers);
        if (!participants.alignedPairs.Any())
            return false;

        AssignAttackingPairs();
        return participants.attackingPairs.Any();
    }

    // Returns true if the two actors are aligned and both active.
    private bool AreActorsAligned(ActorInstance actor1, ActorInstance actor2)
    {
        return actor1 != null && actor2 != null && actor1 != actor2 &&
               actor1.isActive && actor1.isAlive &&
               actor2.isActive && actor2.isAlive &&
               (actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location));
    }

    private void AssignAlignedPairs(IQueryable<ActorInstance> teamMembers)
    {
        foreach (var actor1 in teamMembers)
        {
            foreach (var actor2 in teamMembers)
            {
                if (AreActorsAligned(actor1, actor2) && !participants.HasAlignedPair(actor1, actor2))
                {
                    var pair = CreateAlignedPair(actor1, actor2);
                    participants.alignedPairs.Add(pair);
                }
            }
        }
    }

    private ActorPair CreateAlignedPair(ActorInstance actor1, ActorInstance actor2)
    {
        var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
        return new ActorPair(actor1, actor2, axis);
    }

    private void AssignAttackingPairs()
    {
        foreach (var pair in participants.alignedPairs)
        {
            // If there are opponents between the pair (and no conflicting allies), then add it.
            if (pair.hasOpponentsBetween && !participants.HasAttackingPair(pair))
            {
                participants.attackingPairs.Add(pair);
                // Precompute attack results for this pair.
                pair.attackResults = CalculateAttackResults(pair);
            }
        }
    }

    private List<AttackResult> CalculateAttackResults(ActorPair pair)
    {
        return pair.opponents.Select(opponent =>
        {
            var isHit = Formulas.IsHit(pair.actor1, opponent);
            var isCriticalHit = Formulas.IsCriticalHit(pair.actor1, opponent);
            var damage = isHit ? Formulas.CalculateDamage(pair.actor1, opponent) : 0;
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

    private IEnumerator ResolveAttack(ActorPair pair)
    {
        if (pair.attackResults == null || pair.attackResults.Count == 0)
            yield break;

        yield return GrowAndShrink(pair.actor1, pair.actor2);

        foreach (var attack in pair.attackResults)
        {
            yield return PerformAttack(pair.actor1, attack);
        }
    }

    private IEnumerator GrowAndShrink(ActorInstance actor1, ActorInstance actor2)
    {
        // Wait for both actors to "grow" (attack animation start)
        yield return CoroutineHelper.WaitForAll(GameManager.instance, actor1.action.Grow(), actor2.action.Grow());
        // Then wait for both to "shrink" (attack animation end)
        yield return CoroutineHelper.WaitForAll(GameManager.instance, actor1.action.Shrink(), actor2.action.Shrink());
    }

    private IEnumerator PerformAttack(ActorInstance attacker, AttackResult attack)
    {
        var direction = attacker.GetDirectionTo(attack.Opponent);
        var trigger = new Trigger(attacker.Attack(attack));
        yield return attacker.action.Bump(direction, trigger);

        // If the opponent is dying, handle death.
        if (attack.Opponent.isDying)
        {
            yield return HandleDeath(attack.Opponent);
        }
    }

    private IEnumerator HandleDeath(ActorInstance target)
    {
        target.TriggerDie();
        yield return null;
    }

    private void SetupCombatState()
    {
        UpdateSortingOrder();
        GameManager.instance.boardOverlay.TriggerFadeIn();
    }

    private void UpdateSortingOrder()
    {
        foreach (var actor in GameManager.instance.actors.Where(x => x.isActive && x.isAlive))
        {
            actor.sortingOrder = SortingOrder.Default;
        }

        foreach (var pair in participants.attackingPairs)
        {
            pair.actor1.sortingOrder = SortingOrder.Attacker;
            foreach (var opponent in pair.opponents)
            {
                opponent.sortingOrder = SortingOrder.Target;
            }
        }
    }

    private void CleanupCombatState()
    {
        GameManager.instance.boardOverlay.TriggerFadeOut();
        GameManager.instance.turnManager.ResetSortingOrder();
        ClearCombatParticipants();
    }

    private void ClearCombatParticipants()
    {
        foreach (var actor in participants.Get())
        {
            actor.attackingPairCount = 0;
            actor.supportingPairCount = 0;
            actor.SetDefault();
        }
    }
}
