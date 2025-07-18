using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using game = GameManagerHelper;

// Manages identifying and executing pincer attacks (two attackers pinning enemies between them).
public class PincerAttackManager : MonoBehaviour
{
    // Quick-access to various managers and the actor list.
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SelectedHeroManager selectedHeroManager => GameManager.instance.selectedHeroManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;

    /// <summary>
    /// Check for any valid pincer pairs on this team; either enqueue them or advance the turn.
    /// </summary>
    public void Check(Team team)
    {
        var participants = GetParticipants(team);
        if (!participants.pair.Any())
        {
            turnManager.NextTurn();
            return;
        }

        StartCoroutine(Enqueue(participants));
    }

    /// <summary>
    /// Find all bookend pairs (sameteam actors with only enemies between them).
    /// </summary>
    public PincerAttackParticipants GetParticipants(Team team)
    {
        var participants = new PincerAttackParticipants();

        var teamActors = actors
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        // Avoid duplicate pairs by indexing
        var indexed = teamActors.Select((actor, idx) => (actor, idx));
        foreach (var (actor1, i) in indexed)
        {
            foreach (var actor2 in teamActors.Skip(i + 1))
            {
                // Must align on row or column
                if (!actor1.IsSameRow(actor2.location) && !actor1.IsSameColumn(actor2.location))
                    continue;

                var betweenLocs = Geometry.GetLocationsBetween(actor1.location, actor2.location);
                var betweenActors = actors
                    .Where(x => x.isPlaying && betweenLocs.Contains(x.location))
                    .ToList();

                bool hasEnemy = betweenActors.Any(x => x.team != team);
                bool allOpponents = betweenActors.All(x => x.isPlaying && x.team != team);
                bool noGap = betweenLocs.Count == betweenActors.Count;

                if (hasEnemy && allOpponents && noGap)
                {
                    var opponents = betweenActors.Where(x => x.team != team).ToList();
                    participants.pair.Add(new PincerAttackPair
                    {
                        attacker1 = actor1,
                        attacker2 = actor2,
                        opponents = opponents,
                        supporters1 = FindSupporters(actor1),
                        supporters2 = FindSupporters(actor2)
                    });
                }
            }
        }

        return participants;
    }

    /// <summary>
    /// Recursively build a list of AttackResults, chaining through pairs.
    /// </summary>
    private List<AttackResult> ChainAttacks(ActorInstance attacker, List<PincerAttackPair> pairs)
    {
        var attackResults = new List<AttackResult>();
        var pair = pairs.FirstOrDefault(p => p.attacker1 == attacker || p.attacker2 == attacker);
        if (pair == null) 
            return attackResults;

        foreach (var opp in pair.opponents)
        {
            bool hit = Formulas.IsHit(attacker, opp);
            bool crit = Formulas.IsCriticalHit(attacker, opp);
            int dmg = hit ? Formulas.CalculateDamage(opp, attacker) : 0;

            attackResults.Add(new AttackResult
            {
                Attacker = attacker,
                Opponent = opp,
                IsHit = hit,
                IsCriticalHit = crit,
                Damage = dmg
            });

            // Chain if this attacker is also an attacker in another pair
            var next = pairs.FirstOrDefault(q => q.attacker1 == opp || q.attacker2 == opp);
            if (next != null)
                attackResults.AddRange(ChainAttacks(opp, pairs));
        }

        return attackResults;
    }

    /// <summary>
    /// Enqueue supporter visuals and pincer attacks, then execute the sequence.
    /// </summary>
    private IEnumerator Enqueue(PincerAttackParticipants participants)
    {
        sortingManager.OnPincerAttackStart(participants);

        // Gather and fade in supporters
        var allSupporters = participants.pair
            .SelectMany(p => p.supporters1.Concat(p.supporters2))
            .Distinct()
            .ToList();
        yield return boardOverlay.FadeIn();

        // Queue support sequences
        foreach (var p in participants.pair)
        {
            foreach (var sup in p.supporters1)
            {
                supportLineManager.Spawn(sup, p.attacker1);
                sequenceManager.Add(new PincerAttackSupportSequence(p.attacker1, sup));
            }
            foreach (var sup in p.supporters2)
            {
                supportLineManager.Spawn(sup, p.attacker2);
                sequenceManager.Add(new PincerAttackSupportSequence(p.attacker2, sup));
            }
        }

        // Queue pincer attacks with closestfirst ordering baked in
        foreach (var p in participants.pair)
        {
            // Clear any old attackResults
            p.attackResults1.Clear();
            p.attackResults2.Clear();

            // Build and sort attack attackResults at creation time
            var attacks1
                = ChainAttacks(p.attacker1, participants.pair)
                .OrderBy(r => Vector2.Distance(p.attacker1.location, r.Opponent.location));
            p.attackResults1.AddRange(attacks1);

            var attacks2
                = ChainAttacks(p.attacker2, participants.pair)
                .OrderBy(r => Vector2.Distance(p.attacker2.location, r.Opponent.location));
            p.attackResults2.AddRange(attacks2);

            sequenceManager.Add(new PincerAttackSequence(p));
        }

        // Execute all queued sequences, then fade out and advance turn
        yield return sequenceManager.Execute();
        yield return boardOverlay.FadeOut();

        supportLineManager.Clear();
        participants.Clear();

        turnManager.NextTurn();
    }

    /// <summary>
    /// Find sameteam supporters aligned with the attacker and not blocked.
    /// </summary>
    public List<ActorInstance> FindSupporters(ActorInstance attacker)
    {
        var candidates = actors
            .Where(x => x.isPlaying && x.team == attacker.team && x != attacker)
            .Where(x => x.IsSameRow(attacker.location) || x.IsSameColumn(attacker.location))
            .ToList();

        var result = new List<ActorInstance>();
        foreach (var c in candidates)
            if (!IsActorBlocked(attacker, c))
                result.Add(c);

        return result;
    }

    /// <summary>
    /// Check if any actor blocks the straight line between a and b.
    /// </summary>
    private bool IsActorBlocked(ActorInstance a, ActorInstance b)
    {
        if (!a.IsSameRow(b.location) && !a.IsSameColumn(b.location))
            return true;

        var between = Geometry
            .GetLocationsBetween(a.location, b.location)
            .Where(loc => !loc.Equals(a.location) && !loc.Equals(b.location));

        return actors.Any(x => x.isPlaying && between.Contains(x.location));
    }


}
