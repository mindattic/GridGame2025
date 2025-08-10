using Assets.Scripts.Events;
using Assets.Scripts.Models;
using Assets.Scripts.Sequences;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
public class PincerAttackManager : MonoBehaviour
{
    public void Check(Team team)
    {
        var participants = GetParticipants(team);
        if (!participants.pair.Any())
        {
            g.TurnManager.NextTurn();
            return;
        }
        StartCoroutine(Enqueue(participants));
    }

    public PincerAttackParticipants GetParticipants(Team team)
    {
        var participants = new PincerAttackParticipants();

        var teamActors = g.Actors.All
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        var indexed = teamActors.Select((actor, idx) => (actor, idx));
        foreach (var (actor1, i) in indexed)
        {
            foreach (var actor2 in teamActors.Skip(i + 1))
            {
                if (!Geometry.IsSameRow(actor1.location, actor2.location) && !Geometry.IsSameColumn(actor1.location, actor2.location))
                    continue;

                var betweenLocs = Geometry.GetLocationsBetween(actor1.location, actor2.location);
                var betweenActors = g.Actors.All
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
            // Bounce with top-leftmost among those not already ordered
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

        foreach (var opponent in pair.opponents)
        {
            var attackResult = Formulas.CalculateAttackResult(attacker, opponent);
            attackResults.Add(attackResult);
        }
        return attackResults;
    }

    private IEnumerator Enqueue(PincerAttackParticipants participants)
    {
        g.SortingManager.OnPincerAttack(participants);

        yield return g.BoardOverlay.FadeIn();

        foreach (var p in participants.pair)
        {
            foreach (var supporter in p.supporters1)
            {
                g.SupportLineManager.Spawn(supporter, p.attacker1);
                g.SequenceManager.Add(new PincerAttackSupportSequence(p.attacker1, supporter));
            }
            foreach (var supporter in p.supporters2)
            {
                g.SupportLineManager.Spawn(supporter, p.attacker2);
                g.SequenceManager.Add(new PincerAttackSupportSequence(p.attacker2, supporter));
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

            g.SequenceManager.Add(new PincerAttackSequence(p));
        }

        // Run DeathSequence once after all pairs have resolved
        g.SequenceManager.Add(new DeathSequence());

        yield return g.SequenceManager.ExecuteTrigger();

        //TODO: Put this in a HeroPostAttackSequence...
        yield return g.BoardOverlay.FadeOut();
        g.SupportLineManager.Clear();
        participants.Clear();
        g.TurnManager.NextTurn();
    }

    private AttackResult CreateAttackResult(ActorInstance attacker, ActorInstance opponent)
    {
        var attackResult = Formulas.CalculateAttackResult(attacker, opponent);
        return attackResult;
    }

    public List<ActorInstance> FindSupporters(ActorInstance attacker)
    {
        var candidates = g.Actors.All
            .Where(x => x.isPlaying && x.team == attacker.team && x != attacker)
            .Where(x => Geometry.IsSameRow(x.location, attacker.location) || Geometry.IsSameColumn(x.location, attacker.location))
            .ToList();

        var result = new List<ActorInstance>();
        foreach (var c in candidates)
            if (!IsActorBlocked(attacker, c))
                result.Add(c);

        return result;
    }

    private bool IsActorBlocked(ActorInstance a, ActorInstance b)
    {
        if (!Geometry.IsSameRow(a.location, b.location) && !Geometry.IsSameColumn(a.location, b.location))
            return true;

        var between = Geometry
            .GetLocationsBetween(a.location, b.location)
            .Where(loc => !loc.Equals(a.location) && !loc.Equals(b.location));

        return g.Actors.All.Any(x => x.isPlaying && between.Contains(x.location));
    }
}
