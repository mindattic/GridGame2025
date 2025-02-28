using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;


    /// <summary>
    /// Calculates attack pairs from the current player team and then
    /// either queues up the attack actions or immediately advances the turn.
    /// </summary>
    public IEnumerator CalculateAndExecute()
    {
        // Build the attack context.
        PincerAttackContext context = new PincerAttackContext();
        ComputeContext(context);

        // If there are no attacking pairs, just move to the next turn.
        if (!context.AttackingPairs.Any())
        {
            turnManager.NextTurn();
            yield break;
        }

        // Queue up the attack-phase actions.
        actionManager.Add(new PreAttackSupportAction(context));
        foreach (var pair in context.AttackingPairs)
        {
            actionManager.Add(new AttackPairAction(pair));
        }
        actionManager.Add(new PostAttackSupportAction(context));

        // Optionally trigger visual effects before and after the actions.
        boardOverlay.TriggerFadeIn();
        yield return actionManager.Execute();
        boardOverlay.TriggerFadeOut();

        // Reset board overlay ordering and clear context data.
        turnManager.ResetSortingOrder();
        ClearContext(context);

        // Advance to the next turn.
        turnManager.NextTurn();
        yield break;
    }

    /// <summary>
    /// Builds the attack context by pairing up aligned player actors.
    /// </summary>
    private void ComputeContext(PincerAttackContext context)
    {
        var teamMembers = players.Where(x => x.isPlaying);
        foreach (var actor1 in teamMembers)
        {
            foreach (var actor2 in teamMembers)
            {
                if (actor1 == actor2)
                    continue;
                if (!(actor1.isPlaying && actor2.isPlaying))
                    continue;
                // Check if actors are aligned (same row or same column).
                if (!(actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location)))
                    continue;
                // Avoid duplicates.
                if (context.AlignedPairs.Any(pair => pair.ContainsActorPair(actor1, actor2)))
                    continue;
                // Determine the axis based on their alignment.
                var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
                var pair = new ActorPair(actor1, actor2, axis);
                context.AlignedPairs.Add(pair);
            }
        }
        // For each pair that qualifies as an attacking pair, assign roles and calculate attack results.
        foreach (var pair in context.AlignedPairs.Where(p => p.isAttacker))
        {
            pair.actor1.partner = pair.actor2;
            pair.attackResults = CalculateAttackResults(pair);
            pair.actor1.opponents = pair.attackResults.Select(a => a.Opponent).Distinct().ToList();
        }
    }

    /// <summary>
    /// Computes the attack results for a given pair by determining hit, crit and damage.
    /// </summary>
    private List<AttackResult> CalculateAttackResults(ActorPair pair)
    {
        // This assumes that pair.opponents has already been populated or computed.
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

    /// <summary>
    /// Clears the context by resetting partner, opponents, and supporters for each actor.
    /// </summary>
    private void ClearContext(PincerAttackContext context)
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
