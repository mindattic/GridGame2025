using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PincerAttackManager : MonoBehaviour
{
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SelectedHeroManager selectedHeroManager => GameManager.instance.selectedHeroManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;

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

    public PincerAttackParticipants GetParticipants(Team team)
    {
        var participants = new PincerAttackParticipants();

        var teamActors = actors
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        var indexed = teamActors.Select((actor, idx) => (actor, idx));
        foreach (var (actor1, i) in indexed)
        {
            foreach (var actor2 in teamActors.Skip(i + 1))
            {
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

        // Reorder the pairs using the "snake" chain order
        participants.pair = OrderPairsSnake(participants.pair);

        return participants;
    }

    /// <summary>
    /// Orders pairs: start with the top-leftmost, follow chain where attacker2==attacker1 of next pair, etc.
    /// </summary>
    private List<PincerAttackPair> OrderPairsSnake(List<PincerAttackPair> pairs)
    {
        var ordered = new List<PincerAttackPair>();
        var remaining = new HashSet<PincerAttackPair>(pairs);

        // Helper to sort by (y, x)
        System.Func<PincerAttackPair, (int y, int x)> pos = p => (p.attacker1.location.y, p.attacker1.location.x);

        while (remaining.Any())
        {
            // Start with top-leftmost among those not already ordered
            var start = remaining.OrderBy(pos).First();
            var curr = start;

            // Chain follow attacker2==attacker1, as long as possible
            while (curr != null)
            {
                ordered.Add(curr);
                remaining.Remove(curr);
                // Find the next pair where attacker1 == curr.attacker2 and it's still unprocessed
                curr = remaining.FirstOrDefault(p => p.attacker1 == ordered.Last().attacker2);
            }
        }
        return ordered;
    }

    private List<AttackResult> ChainAttacks(ActorInstance attacker, List<PincerAttackPair> pairs)
    {
        var attackResults = new List<AttackResult>();
        var pair = pairs.FirstOrDefault(p => p.attacker1 == attacker || p.attacker2 == attacker);
        if (pair == null) return attackResults;

        foreach (var opp in pair.opponents)
        {
            bool hit = Formulas.IsHit(attacker, opp);
            bool crit = Formulas.IsCriticalHit(attacker, opp);
            int dmg = hit ? Formulas.CalculateDamage(attacker, opp) : 0;

            attackResults.Add(new AttackResult
            {
                Attacker = attacker,
                Opponent = opp,
                IsHit = hit,
                IsCriticalHit = crit,
                Damage = dmg
            });

            // NO recursive chaining—each pincer acts independently!
        }
        return attackResults;
    }

    private IEnumerator Enqueue(PincerAttackParticipants participants)
    {
        sortingManager.OnPincerAttackStart(participants);

        yield return boardOverlay.FadeIn();

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

        foreach (var p in participants.pair)
        {
            p.attackResults1.Clear();
            p.attackResults2.Clear();

            // Always sort from "left/top to right/bottom" for attacker1, reverse for attacker2
            bool vertical = p.attacker1.location.x == p.attacker2.location.x;
            bool horizontal = p.attacker1.location.y == p.attacker2.location.y;

            if (vertical)
            {
                var sorted = p.opponents.OrderBy(o => o.location.y).ToList();
                var sortedRev = sorted.AsEnumerable().Reverse().ToList();

                p.attackResults1.AddRange(sorted.Select(opp => CreateAttackResult(p.attacker1, opp)));
                p.attackResults2.AddRange(sortedRev.Select(opp => CreateAttackResult(p.attacker2, opp)));
            }
            else if (horizontal)
            {
                var sorted = p.opponents.OrderBy(o => o.location.x).ToList();
                var sortedRev = sorted.AsEnumerable().Reverse().ToList();

                p.attackResults1.AddRange(sorted.Select(opp => CreateAttackResult(p.attacker1, opp)));
                p.attackResults2.AddRange(sortedRev.Select(opp => CreateAttackResult(p.attacker2, opp)));
            }

            sequenceManager.Add(new PincerAttackSequence(p));
        }

        yield return sequenceManager.Execute();
        yield return boardOverlay.FadeOut();

        supportLineManager.Clear();
        participants.Clear();
        turnManager.NextTurn();
    }

    private AttackResult CreateAttackResult(ActorInstance attacker, ActorInstance opp)
    {
        bool hit = Formulas.IsHit(attacker, opp);
        bool crit = Formulas.IsCriticalHit(attacker, opp);
        int dmg = hit ? Formulas.CalculateDamage(attacker, opp) : 0;
        return new AttackResult
        {
            Attacker = attacker,
            Opponent = opp,
            IsHit = hit,
            IsCriticalHit = crit,
            Damage = dmg
        };
    }

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
