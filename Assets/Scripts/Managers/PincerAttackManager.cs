using Assets.Scripts.Actions;
using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PincerAttackManager : MonoBehaviour
{
    // Quick Reference Properties
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected List<ActorInstance> actors => GameManager.instance.actors;

    /// <summary>
    /// Check for any pincer attacks for the player's team. If found, run them. Otherwise, next turn.
    /// </summary>
    public void Check()
    {
        // We'll get pincer collection for Team.Player
        var participants = GetParticipants(Team.Player);

        // If no pairs exist, skip to next turn
        if (!participants.participants.Any())
        {
            turnManager.NextTurn();
            return;
        }

        // Otherwise, queue them up
        StartCoroutine(EnqueueAttacks(participants));
    }

    /// <summary>
    /// Gathers all "bookend pairs" for the given team, populating 
    /// each p's opponents and attacks, plus supporters.
    /// </summary>
    public PincerAttackParticipantCollection GetParticipants(Team team)
    {
        var collection = new PincerAttackParticipantCollection();

        // Gather all active same-team actors
        var teamActors = actors
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        var indexedTeamActors = teamActors.Select((actor, index) => (actor, index));
        foreach (var (actor1, i) in indexedTeamActors)
        {
            // Skip anything up to i + 1 so we don't double-up.
            var remainingTeamActors = teamActors.Skip(i + 1);
            foreach (var actor2 in remainingTeamActors)
            {
                // Must share row or column
                if (!actor1.IsSameRow(actor2.location) && !actor1.IsSameColumn(actor2.location))
                    continue;

                // Order actors by row or column
                //var ordered = OrderByRowOrColumn(actor1, actor2);
                //var locationsBetweenAttackers = Geometry.GetLocationsBetween(ordered[0].location, ordered[1].location);
                var locationsBetweenAttackers = Geometry.GetLocationsBetween(actor1.location, actor2.location);

                // All actors in locationsBetweenAttackers, if any
                var actorsBetweenAttackers = actors
                    .Where(x => x.isPlaying)
                    .Where(x => locationsBetweenAttackers.Contains(x.location))
                    .ToList();

                // Check that all locationsBetweenAttackers positions are occupied by enemies only
                bool hasEnemyBetweenAttackers = actorsBetweenAttackers.Any(x => x.team != team);
                bool onlyOpponentsBetweenAttackers = actorsBetweenAttackers.All(x => x.isPlaying && x.team != team);
                bool hasNoGapBetweenAttackers = (locationsBetweenAttackers.Count == actorsBetweenAttackers.Count);

                if (hasEnemyBetweenAttackers && onlyOpponentsBetweenAttackers && hasNoGapBetweenAttackers)
                {
                    var attacker1 = actor1;
                    var attacker2 = actor2;
                    var opponents = actorsBetweenAttackers.Where(x => x.isPlaying && x.team != team).ToList();

                    var p = new PincerAttackParticipants
                    {
                        attacker1 = attacker1,
                        attacker2 = attacker2,
                        opponents = opponents,
                        supporters1 = FindSupporters(attacker1),
                        supporters2 = FindSupporters(attacker2)
                    };

                    collection.participants.Add(p);
                }
            }
        }

        return collection;
    }

    /// <summary>
    /// Recursively chain attacks starting from the given attacker.
    /// For the current attacker, sort opponents by distance and process each attack.
    /// If an opponent is also an attacker1 in another p, chain its attack sequence.
    /// </summary>
    private List<AttackResult> ChainAttacksRecursively(ActorInstance attacker, List<PincerAttackParticipants> participants)
    {
        var attacks = new List<AttackResult>();

        // Find the pincer p where this actor is attacker1
        var p = participants.FirstOrDefault(p => p.attacker1 == attacker);
        if (p == null)
            return attacks;

        // Sort opponents by distance from the current attacker
        var sortedOpponents = p.opponents
            .OrderBy(x => Vector2.Distance(attacker.location, x.location))
            .ToList();

        foreach (var opponent in sortedOpponents)
        {
            // Compute the attack result for this opponent
            bool isHit = Formulas.IsHit(attacker, opponent);
            bool isCritical = Formulas.IsCriticalHit(attacker, opponent);
            int damage = isHit ? Formulas.CalculateDamage(attacker, opponent) : 0;

            attacks.Add(new AttackResult
            {
                Opponent = opponent,
                IsHit = isHit,
                IsCriticalHit = isCritical,
                Damage = damage
            });

            // Check if this opponent is also an attacker1 in another p.
            // If yes, use that opponent as the new attacker context.
            var subsequentParticipants = participants.FirstOrDefault(p => p.attacker1 == opponent);
            if (subsequentParticipants != null)
            {
                attacks.AddRange(ChainAttacksRecursively(opponent, participants));
            }
        }

        return attacks;
    }

    /// <summary>
    /// Enqueues support and pincer attack actions, then executes them.
    /// </summary>
    private IEnumerator EnqueueAttacks(PincerAttackParticipantCollection collection)
    {
        // 1) Assign sorting orders to highlight attackers, opponents, supporters
        SetSortingOrder(collection);

        // 2) Queue up support actions for both attackers
        foreach (var participants in collection.participants)
        {
            // Attacker1's supporters
            foreach (var supporter in participants.supporters1)
            {
                supportLineManager.Spawn(supporter, participants.attacker1);
                actionManager.Add(new AttackSupportAction(participants.attacker1, supporter));
            }

            // Attacker2's supporters
            foreach (var supporter in participants.supporters2)
            {
                supportLineManager.Spawn(supporter, participants.attacker2);
                actionManager.Add(new AttackSupportAction(participants.attacker2, supporter));
            }
        }

        // 3) Process pincer attacks using the chained attack logic
        foreach (var participants in collection.participants)
        {
            // Here we compute the full chain of attacks starting from attacker1.
            participants.attacks = ChainAttacksRecursively(participants.attacker1, collection.participants);
            actionManager.Add(new PincerAttackAction(participants));
        }

        // 4) Execute the queued actions, with fade in / fade out effects.
        yield return boardOverlay.FadeIn();
        yield return actionManager.Execute();
        yield return boardOverlay.FadeOut();

        // 5) Clean up: reset sorting, Clear p, and go to the next turn.
        ResetSortingOrder();
        collection.Clear();
        turnManager.NextTurn();
    }

    /// <summary>
    /// Returns a list with the two actors ordered by row or column (lowest first).
    /// </summary>
    private List<ActorInstance> OrderByRowOrColumn(ActorInstance a, ActorInstance b)
    {
        if (a.location.x == b.location.x)
        {
            return a.location.y < b.location.y
                ? new List<ActorInstance> { a, b }
                : new List<ActorInstance> { b, a };
        }
        else
        {
            return a.location.x < b.location.x
                ? new List<ActorInstance> { a, b }
                : new List<ActorInstance> { b, a };
        }
    }

    /// <summary>
    /// Returns a list of same-team supporters who share a row or column with 'attacker' and have an unobstructed path.
    /// </summary>
    private List<ActorInstance> FindSupporters(ActorInstance attacker)
    {
        var potential = actors
            .Where(x => x.isPlaying && x.team == attacker.team && x != attacker)
            .Where(x => x.IsSameRow(attacker.location) || x.IsSameColumn(attacker.location))
            .ToList();

        var results = new List<ActorInstance>();
        foreach (var p in potential)
        {
            if (!IsActorBlocked(attacker, p))
                results.Add(p);
        }
        return results;
    }

    /// <summary>
    /// Determines if there's at least one actor locationsBetweenAttackers 'a' and 'b' that blocks the support.
    /// </summary>
    private bool IsActorBlocked(ActorInstance a, ActorInstance b)
    {
        if (!a.IsSameRow(b.location) && !a.IsSameColumn(b.location))
            return true;

        var between = Geometry.GetLocationsBetween(a.location, b.location);
        return actors
            .Where(x => x.isPlaying && between.Contains(x.location))
            .Any(x => x.team != a.team || x == a || x == b);
    }

    /// <summary>
    /// Sets sorting orders for attackers, opponents, and supporters for highlighting purposes.
    /// </summary>
    private void SetSortingOrder(PincerAttackParticipantCollection participants)
    {
        ResetSortingOrder();

        foreach (var pair in participants.participants)
        {
            pair.attacker1.sortingOrder = SortingOrder.Attacker;
            pair.attacker2.sortingOrder = SortingOrder.Attacker;

            foreach (var opp in pair.opponents)
                opp.sortingOrder = SortingOrder.Opponent;

            foreach (var s in pair.supporters1)
                s.sortingOrder = SortingOrder.Supporter;
            foreach (var s in pair.supporters2)
                s.sortingOrder = SortingOrder.Supporter;
        }
    }

    /// <summary>
    /// Resets all currently-playing actors to their default sorting order.
    /// </summary>
    private void ResetSortingOrder()
    {
        foreach (var actor in actors.Where(x => x.isPlaying))
            actor.sortingOrder = SortingOrder.Default;
    }
}
