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
    /// Determine if there are pending attacks
    /// </summary>
    public void Check()
    {
        PincerAttackParticipants participants = new PincerAttackParticipants();
        CalculateParticipants(participants);

        //If not attacks, goto next turn...
        if (!participants.AttackingPairs.Any())
            turnManager.NextTurn();

        //...otherwise queue attacks
        StartCoroutine(EnqueueAttacks(participants));
    }



    /// <summary>
    /// Calculates attack pairs from the current player team and then
    /// either queues up the attack actions or immediately advances the turn.
    /// </summary>
    private IEnumerator EnqueueAttacks(PincerAttackParticipants participants)
    {
        SetSortingOrder(participants);

        // Queue up the attack-phase actions.
        actionManager.Add(new PreAttackSupportAction(participants));
        foreach (var pair in participants.AttackingPairs)
        {
            actionManager.Add(new PincerAttackAction(pair));
        }
        actionManager.Add(new PostAttackSupportAction(participants));

        // Optionally trigger visual effects before and after the actions.
        yield return boardOverlay.FadeIn();
        yield return actionManager.Execute();
        yield return boardOverlay.FadeOut();

        ResetSortingOrder();
        participants.Clear();

        // Advance to the next turn.
        turnManager.NextTurn();
        yield break;
    }

    /// <summary>
    /// Builds the attack participants by pairing up aligned player actors.
    /// </summary>
    private void CalculateParticipants(PincerAttackParticipants participants)
    {
        participants.Clear();

        var teamMembers = players.Where(x => x.isPlaying);
        foreach (var actor1 in teamMembers)
        {
            foreach (var actor2 in teamMembers)
            {
                if (actor1 == actor2)
                    continue;
                if (!(actor1.isPlaying && actor2.isPlaying))
                    continue;
                // EnqueueAttacks if actors are aligned (same row or same column).
                if (!(actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location)))
                    continue;
                // Avoid duplicates.
                if (participants.AlignedPairs.Any(pair => pair.Is(actor1, actor2)))
                    continue;
                // Determine the axis based on their alignment.
                var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
                var pair = new ActorPair(actor1, actor2, axis);
                participants.AlignedPairs.Add(pair);
            }
        }
        // For each pair that qualifies as an attacking pair, assign roles and calculate attack results.
        foreach (var pair in participants.AlignedPairs.Where(p => p.isAttacker))
        {
            pair.actor1.partner = pair.actor2;
            pair.actor2.partner = pair.actor1; // Ensuring bidirectional partnership.

            pair.attackResults = CalculateAttackResults(pair);

            // Assign the same opponent list to both actors.
            var opponents = pair.attackResults.Select(a => a.Opponent).Distinct().ToList();
            pair.actor1.opponents = opponents;
            pair.actor2.opponents = opponents;
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
    /// Clears the participants by resetting partner, opponents, and supporters for each actor.
    /// </summary>
    private void ClearParticipants(PincerAttackParticipants participants)
    {
        foreach (var pair in participants.AlignedPairs)
        {
            pair.actor1.partner = null;
            pair.actor1.opponents.Clear();
            pair.actor1.supporters.Clear();
            pair.actor2.partner = null;
            pair.actor2.opponents.Clear();
            pair.actor2.supporters.Clear();
        }
        participants.AlignedPairs.Clear();
    }





    public void SetSortingOrder(PincerAttackParticipants participants)
    {
        ResetSortingOrder();

        foreach (var pair in participants.AttackingPairs)
        {
            var attackers = new List<ActorInstance>() { pair.actor1, pair.actor2 };
            var opponents = pair.opponents; // This includes all actors in the .opponents list.

            // Assign sorting order for attackers
            foreach (var attacker in attackers)
            {
                attacker.sortingOrder = SortingOrder.Attacker;
            }

            // Assign sorting order for opponents
            foreach (var opponent in opponents)
            {
                opponent.sortingOrder = SortingOrder.Opponent;
            }
        }

        foreach (var pair in participants.SupportingPairs)
        {
            //TODO: Set SortingOrder.Supporter to any pair.actor1 or pair2.actor2 that isn't already an attacker
            if (!pair.actor1.isAttacker)
                pair.actor1.sortingOrder = SortingOrder.Supporter;
            if (!pair.actor2.isAttacker)
                pair.actor2.sortingOrder = SortingOrder.Supporter;
        }
    }

    public void ResetSortingOrder()
    {
        foreach (var actor in actors.Where(x => x.isPlaying))
        {
            actor.sortingOrder = SortingOrder.Default;
        }
    }


}
