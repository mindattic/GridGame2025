using Assets.Scripts.Actions;
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
        {
            turnManager.NextTurn();
            return;
        }

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

        // Queue up support actions first
        foreach (var pair in participants.AttackingPairs)
        {
            actionManager.Add(new AttackSupportAction(pair));
        }

        // Then queue up attack-phase actions
        foreach (var pair in participants.AttackingPairs)
        {
            actionManager.Add(new PincerAttackAction(pair));
        }

        //TODO: queue up post attack support actions here...

        yield return boardOverlay.FadeIn();
        yield return actionManager.Execute();
        yield return boardOverlay.FadeOut();

        ResetSortingOrder();
        participants.Clear();

        turnManager.NextTurn();
        yield break;
    }

    /// <summary>
    /// Returns true if there is no other actor (friendly or enemy) between loc1 and loc2
    /// along the same row or column. Excludes the endpoints.
    /// </summary>
    private bool IsClearPathBetween(Vector2Int loc1, Vector2Int loc2)
    {
        // Use all actors currently active in the game.
        var allActors = GameManager.instance.actors.Where(x => x.isActive && x.isAlive).ToList();

        if (loc1.x == loc2.x)
        {
            int minY = Mathf.Min(loc1.y, loc2.y);
            int maxY = Mathf.Max(loc1.y, loc2.y);
            // Exclude endpoints.
            return !allActors.Any(a => a.location.x == loc1.x && a.location.y > minY && a.location.y < maxY);
        }
        else if (loc1.y == loc2.y)
        {
            int minX = Mathf.Min(loc1.x, loc2.x);
            int maxX = Mathf.Max(loc1.x, loc2.x);
            return !allActors.Any(a => a.location.y == loc1.y && a.location.x > minX && a.location.x < maxX);
        }
        return false;
    }

    /// <summary>
    /// Builds the attack participants by pairing up aligned player actors.
    /// Then, for each attacker (actor1 and actor2), assigns supporters as those friendly actors
    /// that are in the same row or column with a clear path between them (i.e. no actor, friendly or enemy, in between).
    /// Finally, for each supporter, adds the attacker to that supporter's "supporting" list.
    /// </summary>
    private void CalculateParticipants(PincerAttackParticipants participants)
    {
        participants.Clear();
        var teamMembers = players.Where(x => x.isPlaying).ToList();

        // Create aligned pairs.
        foreach (var actor1 in teamMembers)
        {
            foreach (var actor2 in teamMembers)
            {
                if (actor1 == actor2 || !actor1.isPlaying || !actor2.isPlaying)
                    continue;

                if (!(actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location)))
                    continue;

                if (participants.AlignedPairs.Any(pair => pair.Matches(actor1, actor2)))
                    continue;

                var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
                var pair = new ActorPair(actor1, actor2, axis);
                participants.AlignedPairs.Add(pair);
            }
        }

        // Assign supporters based on a clear path (no actor between, friendly or enemy)
        foreach (var pair in participants.AlignedPairs)
        {
            // For actor1: choose friendly actors that share the same row/column and have a clear path.
            pair.actor1.supporters = teamMembers
                .Where(x => x != pair.actor1 &&
                            (pair.actor1.IsSameRow(x.location) || pair.actor1.IsSameColumn(x.location)) &&
                            IsClearPathBetween(pair.actor1.location, x.location))
                .ToList();

            foreach (var supporter in pair.actor1.supporters)
            {
                if (!supporter.supporting.Contains(pair.actor1))
                    supporter.supporting.Add(pair.actor1);
            }

            // For actor2:
            pair.actor2.supporters = teamMembers
                .Where(x => x != pair.actor2 &&
                            (pair.actor2.IsSameRow(x.location) || pair.actor2.IsSameColumn(x.location)) &&
                            IsClearPathBetween(pair.actor2.location, x.location))
                .ToList();

            foreach (var supporter in pair.actor2.supporters)
            {
                if (!supporter.supporting.Contains(pair.actor2))
                    supporter.supporting.Add(pair.actor2);
            }
        }

        // Process attack pairs: assign partners and calculate attack results.
        foreach (var pair in participants.AlignedPairs.Where(p => p.isAttacker))
        {
            pair.actor1.partner = pair.actor2;
            pair.actor2.partner = pair.actor1;

            pair.attackResults = CalculateAttackResults(pair);

            var opponents = pair.attackResults.Select(a => a.Opponent).Distinct().ToList();
            pair.actor1.opponents = opponents;
            pair.actor2.opponents = opponents;
        }
    }


    /// <summary>
    /// Computes the attack results for a given actorPair by determining hit, crit and damage.
    /// </summary>
    private List<AttackResult> CalculateAttackResults(ActorPair pair)
    {
        // This assumes that actorPair.opponents has already been populated or computed.
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
            //TODO: Set SortingOrder.Supporter to any actorPair.actor1 or pair2.actor2 that isn't already an attacker
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
