using Assets.Scripts.Actions;
using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// A container with all of the "bookend pairs" found for x certain team.
/// </summary>
public class PincerAttackParticipantCollection
{
    public List<PincerAttackParticipants> Participants = new();
    public void Clear() => Participants.Clear();
}

/// <summary>
/// Manages pincer-attack logic: finding bookend pairs, computing attacks, and enqueuing actions.
/// </summary>
public class AttackManager : MonoBehaviour
{
   //Quick Reference Properties
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected List<ActorInstance> actors => GameManager.instance.actors;

    /// <summary>
    /// Check for any pincer attacks for the player's team. If found, run them. Otherwise, next turn.
    /// You can similarly do this for the enemy if desired.
    /// </summary>
    public void Check()
    {
        // We'll get pincer participants for Team.Player
        var participants = GetAttackParticipants(Team.Player);

        // If no pairs exist, skip to next turn
        if (!participants.Participants.Any())
        {
            turnManager.NextTurn();
            return;
        }

        // Otherwise, queue them up
        StartCoroutine(EnqueueAttacks(participants));
    }

    /// <summary>
    /// Gathers all "bookend pairs" for the given team, populates 
    /// each pair's opponents and attacks, plus supporters.
    /// </summary>
    public PincerAttackParticipantCollection GetAttackParticipants(Team team)
    {
        // Drop any selected player so no piece is mid-drag
        //GameManager.instance.selectedPlayerManager.Drop();

        var participants = new PincerAttackParticipantCollection();

        // Gather all active same-team actors
        var teamActors = actors
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        var indexedTeamActors = teamActors.Select((actor, index) => (actor, index));
        foreach (var (actor1, i) in indexedTeamActors)
        {
            //Skip anything up to i + 1 so we don't double-pair
            //For item i, I only want to see the items that come after it in the list
            var remainingTeamActors = teamActors.Skip(i + 1);
            foreach (var actor2 in remainingTeamActors)
            {
                // Must share row or column
                if (!actor1.IsSameRow(actor2.location) && !actor1.IsSameColumn(actor2.location))
                    continue;

                // Sort them by row or column
                var ordered = OrderByRowOrColumn(actor1, actor2);
                var between = Geometry.GetLocationsBetween(ordered[0].location, ordered[1].location);

                // All actors in between, if any
                var inBetweenActors = actors
                    .Where(x => x.isPlaying)
                    .Where(x => between.Contains(x.location))
                    .ToList();

                // Are there enemies in between, and is it fully occupied (no empty tile)?
                bool anyEnemyInBetween = inBetweenActors.Any(x => x.team != team);
                bool onlyEnemiesInBetween = inBetweenActors.All(x => x.team != team && x.isPlaying);
                bool allNonEmpty = (between.Count == inBetweenActors.Count);

                if (anyEnemyInBetween && onlyEnemiesInBetween && allNonEmpty)
                {
                    // Build x new PincerAttackParticipants
                    var opponents = inBetweenActors
                        .Where(x => x.team != team)
                        .ToList();

                    var pair = new PincerAttackParticipants
                    {
                        attacker1 = actor1,
                        attacker2 = actor2,
                        opponents = opponents
                    };

                    // Find supporters for each attacker
                    pair.supportersOfAttacker1 = FindSupporters(actor1, team);
                    pair.supportersOfAttacker2 = FindSupporters(actor2, team);

                    // Compute the AttackResults for this pair
                    // For demonstration, we’ll assume attacker1 hits all opponents,
                    // but you can adapt to let both attacker1 and attacker2 share the hits
                    var results = new List<AttackResult>();
                    foreach (var opp in pair.opponents)
                    {
                        var chosenAttacker = actor1; // always attackerA here

                        bool isHit = Formulas.IsHit(chosenAttacker, opp);
                        bool isCritical = Formulas.IsCriticalHit(chosenAttacker, opp);
                        int damage = isHit ? Formulas.CalculateDamage(chosenAttacker, opp) : 0;

                        var attackResult = new AttackResult
                        {
                            Opponent = opp,
                            IsHit = isHit,
                            IsCriticalHit = isCritical,
                            Damage = damage
                        };
                        results.Add(attackResult);
                    }
                    pair.attacks = results;

                    // Finally, store the pair
                    participants.Participants.Add(pair);
                }
            }
        }

        return participants;
    }

    /// <summary>
    /// Actually enqueues the support and pincer-attack actions, then executes them.
    /// </summary>
    private IEnumerator EnqueueAttacks(PincerAttackParticipantCollection participants)
    {
        // 1) Set sorting to highlight attackers, opponents, supporters
        SetSortingOrder(participants);

        // 2) First, queue up support (attacker => each supporter)
        foreach (var pair in participants.Participants)
        {
            // attacker1's supporters
            foreach (var supporter in pair.supportersOfAttacker1)
            {
                // Draw line from supporter to attacker
                GameManager.instance.supportLineManager.Spawn(supporter, pair.attacker1);

                // Add an action that does support logic for (attacker1, supporter)
                actionManager.Add(new AttackSupportAction(pair.attacker1, supporter));
            }

            // attacker2's supporters
            foreach (var supporter in pair.supportersOfAttacker2)
            {
                // Draw line from supporter to attacker
                GameManager.instance.supportLineManager.Spawn(supporter, pair.attacker2);

                // Add an action that does support logic for (attacker2, supporter)
                actionManager.Add(new AttackSupportAction(pair.attacker2, supporter));
            }
        }

        // 3) Then queue up the pincer attacks
        foreach (var pair in participants.Participants)
        {
            actionManager.Add(new PincerAttackAction(pair));
        }

        // 4) Execute them, with fade in / fade out for effect
        yield return boardOverlay.FadeIn();
        yield return actionManager.Execute();
        yield return boardOverlay.FadeOut();

        // 5) Clean up
        ResetSortingOrder();
        participants.Clear();
        turnManager.NextTurn();
        yield break;
    }

 
    /// <summary>
    /// Returns [lower, higher] so we can easily iterate from min->max 
    /// in row or column order
    /// </summary>
    private List<ActorInstance> OrderByRowOrColumn(ActorInstance a, ActorInstance b)
    {
        // If same col, order by y
        if (a.location.x == b.location.x)
        {
            return a.location.y < b.location.y
                ? new List<ActorInstance> { a, b }
                : new List<ActorInstance> { b, a };
        }
        // If same row, order by x
        else
        {
            return a.location.x < b.location.x
                ? new List<ActorInstance> { a, b }
                : new List<ActorInstance> { b, a };
        }
    }

    /// <summary>
    /// Returns x list of same-team supporters who share row/column 
    /// with 'attacker' and have no blocking actor in between.
    /// </summary>
    private List<ActorInstance> FindSupporters(ActorInstance attacker, Team teamOfAttacker)
    {
        var potential = actors
            .Where(x => x.isPlaying && x.team == teamOfAttacker && x != attacker)
            .Where(x => x.IsSameRow(attacker.location) || x.IsSameColumn(attacker.location))
            .ToList();

        var results = new List<ActorInstance>();
        foreach (var p in potential)
        {
            if (!IsActorBlocked(attacker, p)) //Ensure no obstacles exist
                results.Add(p);
        }

        return results;
    }


    /// <summary>
    /// True if there's at least one actor in between 'x' and 'b', 
    /// i.e. line-of-sight is blocked.
    /// </summary>
    private bool IsActorBlocked(ActorInstance a, ActorInstance b)
    {
        if (!a.IsSameRow(b.location) && !a.IsSameColumn(b.location))
            return true; // If not aligned, support is impossible.

        var between = Geometry.GetLocationsBetween(a.location, b.location);

        return actors
            .Where(x => x.isPlaying && between.Contains(x.location))
            .Any(x => x.team != a.team || x == a || x == b); // Block if an enemy or another unit is in between.
    }



    /// <summary>
    /// Sets sorting orders for attackers, opponents, supporters so we can highlight them.
    /// </summary>
    private void SetSortingOrder(PincerAttackParticipantCollection participants)
    {
        ResetSortingOrder();

        foreach (var pair in participants.Participants)
        {
            // Mark attackers
            pair.attacker1.sortingOrder = SortingOrder.Attacker;
            pair.attacker2.sortingOrder = SortingOrder.Attacker;

            // Mark opponents
            foreach (var opp in pair.opponents)
                opp.sortingOrder = SortingOrder.Opponent;

            // Mark supporters
            foreach (var s in pair.supportersOfAttacker1)
                s.sortingOrder = SortingOrder.Supporter;
            foreach (var s in pair.supportersOfAttacker2)
                s.sortingOrder = SortingOrder.Supporter;
        }
    }

    /// <summary>
    /// Returns all currently-playing actors to their default sorting order.
    /// </summary>
    private void ResetSortingOrder()
    {
        foreach (var actor in actors.Where(x => x.isPlaying))
            actor.sortingOrder = SortingOrder.Default;
    }
}
