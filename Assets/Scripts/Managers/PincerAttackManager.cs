using Assets.Scripts.Events;
using Assets.Scripts.Models;
using Assets.Scripts.Sequences;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class PincerAttackManager : MonoBehaviour
{
    /// <summary>
    /// Entry point for resolving pincer attacks for a team.
    /// Uses default start selection when no selected hero is provided.
    /// </summary>
    public void Check(Team team)
    {
        // Backward compatibility: no explicit selected hero
        var participants = GetParticipants(team, null);

        if (!participants.pair.Any())
        {
            g.TurnManager.NextTurn();
            return;
        }

        StartCoroutine(EnqueueRoutine(participants));
    }

    /// <summary>
    /// Preferred entry point when a hero was just dropped to initiate a pincer.
    /// Always starts the chain from the pair whose attacker1 equals selectedHero.
    /// Falls back to default start if that pair does not exist.
    /// </summary>
    public void Check(Team team, ActorInstance selectedHero)
    {
        var participants = GetParticipants(team, selectedHero);

        if (!participants.pair.Any())
        {
            g.TurnManager.NextTurn();
            return;
        }

        StartCoroutine(EnqueueRoutine(participants));
    }

    /// <summary>
    /// Scans the board to discover all valid pincer pairs for the given team.
    /// A valid pair has two allied attackers aligned in row or column with only opponents between them and no gaps.
    /// Pairs are ordered to form resolution chains that start from selectedHero if provided.
    /// </summary>
    public PincerAttackParticipants GetParticipants(Team team, ActorInstance selectedHero)
    {
        var participants = new PincerAttackParticipants();

        var teamActors = g.Actors
            .All
            .Where(x => x.isPlaying && x.team == team)
            .ToList();

        var indexed = teamActors.Select((actor, idx) => (actor, idx));

        foreach (var (actor1, i) in indexed)
        {
            foreach (var actor2 in teamActors.Skip(i + 1))
            {
                // Only consider same row or same column alignments
                if (!Geometry.IsSameRow(actor1.location, actor2.location) && !Geometry.IsSameColumn(actor1.location, actor2.location))
                    continue;

                var betweenLocs = Geometry.GetLocationsBetween(actor1.location, actor2.location);

                var betweenActors = g.Actors
                    .All
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

        // Order pairs using chain rules. Prefer the pair that starts with selectedHero if provided.
        participants.pair = OrderPairsByChainsThenNearest(participants.pair, selectedHero);

        return participants;
    }

    /// <summary>
    /// Orders pairs into execution chains.
    /// Chain start:
    ///   - If preferredStartHero is provided, begin with the remaining pair whose attacker1 equals that hero.
    ///   - Otherwise begin at the top leftmost remaining pair by attacker1 position.
    /// Chaining:
    ///   - Repeatedly append the pair whose attacker1 equals the previous pair's attacker2.
    ///   - If multiple pairs share that attacker1, consume them in that attacker's directional order:
    ///       Vertical: attacker1 above goes top to bottom, attacker1 below goes bottom to top.
    ///       Horizontal: attacker1 left goes right to left, attacker1 right goes left to right.
    /// When a chain ends:
    ///   - Start a new chain at the remaining pair whose attacker1 is nearest to the last pair's attacker2.
    /// Continue until all pairs are consumed.
    /// </summary>
    private List<PincerAttackPair> OrderPairsByChainsThenNearest(List<PincerAttackPair> pairs, ActorInstance preferredStartHero)
    {
        var ordered = new List<PincerAttackPair>();
        var remaining = new HashSet<PincerAttackPair>(pairs);

        // Deterministic tie breaker for starts
        System.Func<PincerAttackPair, (int y, int x)> posKey = p => (p.attacker1.location.y, p.attacker1.location.x);

        // Build attacker1 -> ordered list of that actor's pairs, per directional sweep rules
        var byAttacker1 = pairs
            .GroupBy(p => p.attacker1)
            .ToDictionary(gp => gp.Key, gp => SortPairsForAttacker1(gp.Key, gp.ToList()));

        // Helper: grid Manhattan distance
        int Dist(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        // Pick initial start with preference for the dropped hero
        PincerAttackPair PickInitialStart()
        {
            if (preferredStartHero != null)
            {
                var prefer = remaining.FirstOrDefault(p => p.attacker1 == preferredStartHero);
                if (prefer != null) return prefer;
            }

            return remaining
                .OrderBy(posKey)
                .First();
        }

        // Pick nearest next start relative to a location when a chain ends
        PincerAttackPair PickNearestStartTo(Vector2Int from)
        {
            return remaining
                .OrderBy(p => Dist(p.attacker1.location, from))
                .ThenBy(posKey)
                .First();
        }

        while (remaining.Any())
        {
            var start = ordered.Any()
                ? PickNearestStartTo(ordered.Last().attacker2.location)
                : PickInitialStart();

            var current = start;

            // Walk the chain: attacker2 of current -> next pair's attacker1
            while (current != null)
            {
                ordered.Add(current);
                remaining.Remove(current);

                // Remove the consumed pair from its attacker's queue
                if (byAttacker1.TryGetValue(current.attacker1, out var consumedList))
                    consumedList.Remove(current);

                // Follow to next: use current.attacker2's queue, picking the first remaining pair
                PincerAttackPair next = null;

                if (byAttacker1.TryGetValue(current.attacker2, out var nextList))
                {
                    next = nextList.FirstOrDefault(remaining.Contains);
                }

                current = next;
            }
        }

        return ordered;
    }

    /// <summary>
    /// Sorts all pairs for a given attacker1 by the requested sweep rules so that
    /// when chaining into this attacker1, pairs are consumed in the correct order.
    /// </summary>
    private List<PincerAttackPair> SortPairsForAttacker1(ActorInstance attacker, List<PincerAttackPair> list)
    {
        IEnumerable<(PincerAttackPair pair, int orientPri, int primaryDist, int tieX, int tieY)> keyed = list.Select(p =>
        {
            var a = attacker.location;
            var b = (p.attacker1 == attacker ? p.attacker2.location : p.attacker1.location);

            bool vertical = a.x == b.x;
            bool horizontal = a.y == b.y;

            int dy = Mathf.Abs(a.y - b.y);
            int dx = Mathf.Abs(a.x - b.x);

            // Orientation priority keeps ordering stable if both orientations exist
            int orientPri = dy == dx ? 0 : (dy > dx ? -1 : 1); // -1 vertical first, 1 horizontal first

            // Directional key
            int primaryDist;
            if (vertical)
            {
                bool attackerAbove = a.y < b.y;
                // Above means top to bottom, below means bottom to top
                primaryDist = attackerAbove ? b.y : -b.y;
            }
            else
            {
                bool attackerLeft = a.x < b.x;
                // Left means consume right to left, right means left to right
                primaryDist = attackerLeft ? -b.x : b.x;
            }

            return (p, orientPri, primaryDist, b.x, b.y);
        });

        return keyed
            .OrderBy(k => k.orientPri)
            .ThenBy(k => k.primaryDist)
            .ThenBy(k => k.tieY)
            .ThenBy(k => k.tieX)
            .Select(k => k.pair)
            .ToList();
    }

    /// <summary>
    /// Main enqueue routine.
    /// Spawns visuals for supporters, computes attack orders per pair based on relative positions,
    /// and runs sequences. Resolves deaths once at the end, then advances the turn.
    /// </summary>
    private IEnumerator EnqueueRoutine(PincerAttackParticipants participants)
    {
        g.SortingManager.OnPincerAttack(participants);

        yield return g.BoardOverlay.FadeInRoutine();

        // Enqueue supporter visuals and sequences
        foreach (var p in participants.pair)
        {
            foreach (var supporter in p.supporters1)
            {
                g.SynergyLineManager.Spawn(supporter, p.attacker1);
                g.SequenceManager.Add(new PincerAttackSupportSequence(p.attacker1, supporter));
            }

            foreach (var supporter in p.supporters2)
            {
                g.SynergyLineManager.Spawn(supporter, p.attacker2);
                g.SequenceManager.Add(new PincerAttackSupportSequence(p.attacker2, supporter));
            }
        }

        // Compute attack orders per pair using the requested rules
        foreach (var p in participants.pair)
        {
            p.attackResults1.Clear();
            p.attackResults2.Clear();

            bool vertical = p.attacker1.location.x == p.attacker2.location.x;
            bool horizontal = p.attacker1.location.y == p.attacker2.location.y;

            if (vertical)
            {
                bool attacker1Above = p.attacker1.location.y < p.attacker2.location.y;

                var asc = p.opponents.OrderBy(o => o.location.y).ToList();
                var desc = asc.AsEnumerable().Reverse().ToList();

                var attacker1Order = attacker1Above ? asc : desc;
                var attacker2Order = attacker1Above ? desc : asc;

                p.attackResults1.AddRange(attacker1Order.Select(opp => CreateAttackResult(p.attacker1, opp)));
                p.attackResults2.AddRange(attacker2Order.Select(opp => CreateAttackResult(p.attacker2, opp)));
            }
            else if (horizontal)
            {
                bool attacker1Left = p.attacker1.location.x < p.attacker2.location.x;

                var asc = p.opponents.OrderBy(o => o.location.x).ToList();
                var desc = asc.AsEnumerable().Reverse().ToList();

                var attacker1Order = attacker1Left ? desc : asc;
                var attacker2Order = attacker1Left ? asc : desc;

                p.attackResults1.AddRange(attacker1Order.Select(opp => CreateAttackResult(p.attacker1, opp)));
                p.attackResults2.AddRange(attacker2Order.Select(opp => CreateAttackResult(p.attacker2, opp)));
            }

            g.SequenceManager.Add(new PincerAttackSequence(p));
        }

        // Run DeathSequence once after all pairs have resolved
        g.SequenceManager.Add(new DeathSequence());

        yield return g.SequenceManager.ExecuteRoutine();

        // Cleanup and advance turn
        yield return g.BoardOverlay.FadeOutRoutine();
 
        g.SynergyLineManager.Clear();
        participants.Clear();
        g.TurnManager.NextTurn();
    }

    /// <summary>
    /// Wrapper that calculates a single attack result.
    /// </summary>
    private AttackResult CreateAttackResult(ActorInstance attacker, ActorInstance opponent)
    {
        var attackResult = Formulas.CalculateAttackResult(attacker, opponent);
        return attackResult;
    }

    /// <summary>
    /// Finds same-row or same-column allies who are not blocked between this attacker and the candidate.
    /// These allies can provide support lines during the pincer.
    /// </summary>
    public List<ActorInstance> FindSupporters(ActorInstance attacker)
    {
        var candidates = g.Actors
            .All
            .Where(x => x.isPlaying && x.team == attacker.team && x != attacker)
            .Where(x => Geometry.IsSameRow(x.location, attacker.location) || Geometry.IsSameColumn(x.location, attacker.location))
            .ToList();

        var result = new List<ActorInstance>();

        foreach (var c in candidates)
            if (!IsActorBlocked(attacker, c))
                result.Add(c);

        return result;
    }

    /// <summary>
    /// Returns true if any playing actor exists between the two given actors along row or column.
    /// Different rows and columns are considered blocked for pincer support purposes.
    /// </summary>
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
