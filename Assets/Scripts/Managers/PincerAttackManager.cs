// Import required namespaces from the Assets project and Unity.
// These include scripts for actions, models, utilities, and Unity's standard collections and engine.
using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// The PincerAttackManager class is responsible for managing the pincer attack mechanics.
// It coordinates identifying valid pincer attack setups, chaining the resulting results, 
// supporting the attacking units, and then executing the queued actions.
public class PincerAttackManager : MonoBehaviour
{
    // Quick reference properties to easily access various managers and lists from the GameManager singleton.
    // These properties provide shortcuts to other systems such as turn management, action handling, and board overlays.
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected EventManager eventManager => GameManager.instance.eventManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SelectedHeroManager selectedHeroManager => GameManager.instance.selectedHeroManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;


    /// <summary>
    /// Checks for any valid pincer attack opportunities for the given team.
    /// If any valid pairs (bookends) are found, the method starts a coroutine to process and execute them.
    /// Otherwise, it simply advances to the next turn.
    /// </summary>
    /// <param name="team">The team for which to check pincer results.</param>
    public void Check(Team team)
    {
        // Retrieve all valid pincer attack pair (pairs of attackers with valid enemy opponents in between)
        var participants = GetParticipants(team);

        // If no valid pairs exist, there are no pincer results to perform,
        // so we immediately move to the next turn.
        if (!participants.pair.Any())
        {
            turnManager.NextTurn();
            return;
        }

        // If one or more pairs exist, start a coroutine that will enqueue and process the results.
        StartCoroutine(Enqueue(participants));
    }

    /// <summary>
    /// Gathers all "bookend pairs" for the given team. A bookend pair is a pair of same-team actors
    /// that are aligned on the same row or column with only enemy actors (and no gaps) between them.
    /// For each valid pair, the method records the pair's opponents and any supporting actors.
    /// </summary>
    /// <param name="team">The team to gather pincer attack pair for.</param>
    /// <returns>A PincerAttackParticipants object containing all identified valid pairs.</returns>
    public PincerAttackParticipants GetParticipants(Team team)
    {
        // Create a new container for storing valid pincer attack pairs.
        var participants = new PincerAttackParticipants();

        // Filter and gather all actors that are actively playing and belong to the specified team.
        var teamActors = actors
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        // Create an indexed list to iterate through team actors without duplicating pairs.
        var indexedTeamActors = teamActors.Select((actor, index) => (actor, index));
        foreach (var (actor1, i) in indexedTeamActors)
        {
            // Skip all actors before or at the CurrentProfile index to avoid double-checking pairs.
            var remainingTeamActors = teamActors.Skip(i + 1);
            foreach (var actor2 in remainingTeamActors)
            {
                // The two actors must share either the same row or column to be considered a potential pair.
                if (!actor1.IsSameRow(actor2.location) && !actor1.IsSameColumn(actor2.location))
                    continue; // Not aligned, so skip this combination.

                // Calculate all board locations between the two potential attackers.
                var locationsBetweenAttackers = Geometry.GetLocationsBetween(actor1.location, actor2.location);

                // Find any actors that occupy the positions between the two attackers.
                var actorsBetweenAttackers = actors
                    .Where(x => x.isPlaying)
                    .Where(x => locationsBetweenAttackers.Contains(x.location))
                    .ToList();

                // Conditions for a valid pincer attack:
                // 1. There must be at least one enemy between the attackers.
                // 2. All actors between the attackers must be opponents (i.e., not on the same team).
                // 3. There should be no empty spaces between the attackers (the count of positions must match the count of actors).
                bool hasEnemyBetweenAttackers = actorsBetweenAttackers.Any(x => x.team != team);
                bool onlyOpponentsBetweenAttackers = actorsBetweenAttackers.All(x => x.isPlaying && x.team != team);
                bool hasNoGapBetweenAttackers = (locationsBetweenAttackers.Count == actorsBetweenAttackers.Count);

                if (hasEnemyBetweenAttackers && onlyOpponentsBetweenAttackers && hasNoGapBetweenAttackers)
                {
                    // At this point, we have a valid pair. Record the attackers and their respective supporting actors.
                    var attacker1 = actor1;
                    var attacker2 = actor2;
                    var opponents = actorsBetweenAttackers.Where(x => x.isPlaying && x.team != team).ToList();

                    var p = new PincerAttackPair
                    {
                        attacker1 = attacker1,
                        attacker2 = attacker2,
                        opponents = opponents,
                        // Find same-team actors that can support each attacker along the unobstructed row/column.
                        supporters1 = FindSupporters(attacker1),
                        supporters2 = FindSupporters(attacker2)
                    };

                    // Add the valid pair to the participants.
                    participants.pair.Add(p);
                }
            }
        }

        // Return all identified pincer attack pairs.
        return participants;
    }

    /// <summary>
    /// Recursively chains results starting from the specified attacker.
    /// For the CurrentProfile attacker, opponents are sorted by distance so that closer opponents are processed first.
    /// If any opponent is also found as the primary attacker (attacker1) in another valid pair, their chain is processed recursively.
    /// </summary>
    /// <param name="attacker">The starting attacker for the chain.</param>
    /// <param name="pair">List of all valid pincer attack pairs.</param>
    /// <returns>A list of AttackResult objects representing the chain of results.</returns>
    private List<AttackResult> ChainAttacks(ActorInstance attacker, List<PincerAttackPair> pair)
    {
        var attacks = new List<AttackResult>();

        // Identify the pincer attack pair where the CurrentProfile actor serves as the primary attacker.
        var p = pair.FirstOrDefault(p => p.attacker1 == attacker);
        if (p == null)
            return attacks; // No chain can be made if the actor is not found as attacker1.

        // SortingLayer the opponents by their distance from the CurrentProfile attacker to process closer enemies first.
        var sortedOpponents = p.opponents
            .OrderBy(x => Vector2.Distance(attacker.location, x.location))
            .ToList();

        // Iterate through each opponent and compute the attack result.
        foreach (var opponent in sortedOpponents)
        {
            bool isHit = Formulas.IsHit(attacker, opponent);
            bool isCritical = Formulas.IsCriticalHit(attacker, opponent);
            int damage = isHit ? Formulas.CalculateDamage(attacker, opponent) : 0;

            // Record the result of this attack.
            attacks.Add(new AttackResult
            {
                Opponent = opponent,
                IsHit = isHit,
                IsCriticalHit = isCritical,
                Damage = damage
            });

            // If this opponent is also registered as an attacker in a valid pair, chain their results recursively.
            var subsequentParticipants = pair.FirstOrDefault(p => p.attacker1 == opponent);
            if (subsequentParticipants != null)
            {
                // Append the chained results from the subsequent attacker.
                attacks.AddRange(ChainAttacks(opponent, pair));
            }
        }

        // Return the full list of chained attack results.
        return attacks;
    }

    /// <summary>
    /// Enqueues both support and pincer attack actions, then executes the queued actions with visual effects.
    /// This coroutine sets up highlighting, queues up support and attack actions, executes them,
    /// resets the board state, clears the pair, and finally advances the turn.
    /// </summary>
    /// <param name="participants">The participants of valid pincer attack pair.</param>
    private IEnumerator Enqueue(PincerAttackParticipants participants)
    {
        sortingManager.OnPincerAttackStart(participants);

        yield return boardOverlay.FadeIn();

        // --- 1. Gather all unique supporters (no duplicates)
        var allSupporters = participants.pair
            .SelectMany(pair => pair.supporters1.Concat(pair.supporters2))
            .Distinct()
            .ToList();

        // --- 2. Queue: PopInOut for all supporters
        foreach (var supporter in allSupporters)
        {

            eventManager.Add(new PortraitPopInEvent(supporter));
        }

        // --- 3. Queue: AttackSupportActions and support lines (before attack)
        foreach (var pair in participants.pair)
        {
            foreach (var supporter in pair.supporters1)
            {
                supportLineManager.Spawn(supporter, pair.attacker1);
                eventManager.Add(new PincerAttackSupportEvent(pair.attacker1, supporter));
            }
            foreach (var supporter in pair.supporters2)
            {
                supportLineManager.Spawn(supporter, pair.attacker2);
                eventManager.Add(new PincerAttackSupportEvent(pair.attacker2, supporter));
            }
        }

        // --- 4. Queue: PincerAttackActions (core attack logic)
        foreach (var pair in participants.pair)
        {
            pair.results = ChainAttacks(pair.attacker1, participants.pair);
            eventManager.Add(new PincerAttackEvent(pair));
        }

        // --- 5. Queue: PopOut for all supporters (after attack)
        foreach (var supporter in allSupporters)
        {
            eventManager.Add(new PortraitPopOutEvent(supporter));
        }

        // --- 6. Execute sequence

        yield return eventManager.Execute();
        yield return boardOverlay.FadeOut();

        participants.Clear();
        turnManager.NextTurn();
    }

    /// <summary>
    /// Returns a list containing two actors ordered by their row or column positions.
    /// The actor that comes first (lowest value) in the relevant coordinate (x for columns, y for rows)
    /// is placed first in the list.
    /// </summary>
    /// <param name="a">First actor.</param>
    /// <param name="b">Second actor.</param>
    /// <returns>A list of two actors sorted by position.</returns>
    private List<ActorInstance> OrderByRowOrColumn(ActorInstance a, ActorInstance b)
    {
        // If the actors share the same column (x-coordinate), order them by the row (y-coordinate).
        if (a.location.x == b.location.x)
        {
            return a.location.y < b.location.y
                ? new List<ActorInstance> { a, b }
                : new List<ActorInstance> { b, a };
        }
        else
        {
            // Otherwise, order them by the x-coordinate.
            return a.location.x < b.location.x
                ? new List<ActorInstance> { a, b }
                : new List<ActorInstance> { b, a };
        }
    }

    /// <summary>
    /// Finds and returns a list of same-team supporters for a given attacker.
    /// Supporters must be aligned in the same row or column with an unobstructed path from the attacker.
    /// </summary>
    /// <param name="attacker">The attacking actor for whom supporters are being found.</param>
    /// <returns>A list of supporting ActorInstances.</returns>
    private List<ActorInstance> FindSupporters(ActorInstance attacker)
    {
        // Filter potential supporters:
        // They must be playing, on the same team, not be the attacker,
        // and share either the same row or column with the attacker.
        var potential = actors
            .Where(x => x.isPlaying && x.team == attacker.team && x != attacker)
            .Where(x => x.IsSameRow(attacker.location) || x.IsSameColumn(attacker.location))
            .ToList();

        // For each potential supporter, verify that there is an unobstructed path between them and the attacker.
        var results = new List<ActorInstance>();
        foreach (var p in potential)
        {
            if (!IsActorBlocked(attacker, p))
                results.Add(p);
        }
        return results;
    }

    /// <summary>
    /// Determines whether the support between two actors (a and b) is blocked.
    /// Blocking occurs if there is any actor in between (on the path) that belongs to the enemy team,
    /// or is one of the endpoints, or if they are not aligned at all.
    /// </summary>
    /// <param name="a">The reference actor (typically the attacker).</param>
    /// <param name="b">The potential supporter.</param>
    /// <returns>True if the support line is blocked; otherwise, false.</returns>
    private bool IsActorBlocked(ActorInstance a, ActorInstance b)
    {
        // First, if they are not in the same row or column, support cannot be provided.
        if (!a.IsSameRow(b.location) && !a.IsSameColumn(b.location))
            return true;

        //Get all locations between the two actors.
        var between = Geometry.GetLocationsBetween(a.location, b.location);
        // Check if any playing actor occupies a location in between that is either an enemy or coincides with one of the endpoints.
        return actors
            .Where(x => x.isPlaying && between.Contains(x.location))
            .Any(x => x.team != a.team || x == a || x == b);
    }

    /// <summary>
    /// Resets the sorting order for all actors that are currently playing,
    /// reverting them to the default visual layer.
    /// </summary>
    private void ResetSortingOrder()
    {
        //foreach (var actor in actors.Where(x => x.isPlaying))
        //    actor.sortingOrder = SortingOrder.Default;
    }
}
