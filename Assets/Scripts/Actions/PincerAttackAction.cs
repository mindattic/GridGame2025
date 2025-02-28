using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class PincerAttackAction : PhaseAction
//{
//    // Cached game manager components.
//    private BoardOverlay BoardOverlay => GameManager.instance.boardOverlay;
//    private TurnManager TurnManager => GameManager.instance.turnManager;
//    private ActionManager ActionManager => GameManager.instance.actionManager;
//    private SpellManager SpellManager => GameManager.instance.spellManager;
//    private SupportLineManager SupportLineManager => GameManager.instance.supportLineManager;
//    private List<ActorInstance> Actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
//    private IQueryable<ActorInstance> Enemies => GameManager.instance.enemies;
//    private IQueryable<ActorInstance> Players => GameManager.instance.players;

//    // Participants now only holds the list of aligned pairs.
//    private PincerAttackParticipants participants = new PincerAttackParticipants();

//    public override IEnumerator Execute()
//    {
//        participants.Clear();

//        // Use the player team for combat.
//        var teamMembers = Players.Where(x => x.isPlaying);
//        if (!AssignParticipants(teamMembers))
//            yield break; // No valid aligned pairs.

//        // Queue pre-attack support actions.
//        ProcessPreAttackSupport();
//        yield return ActionManager.Execute();

//        SetupCombat();

//        // Process attack actions.
//        yield return ProcessAttacks();
//        yield return ActionManager.Execute();

//        // Queue post-attack support actions.
//        ProcessPostAttackSupport();
//        yield return ActionManager.Execute();

//        yield return BoardOverlay.FadeOut();
//        TurnManager.ResetSortingOrder();
//        ClearCombatParticipants();

//        // Manually trigger turn change.
//        TurnManager.NextTurn();
//        yield break;
//    }

//    private bool AssignParticipants(IQueryable<ActorInstance> teamMembers)
//    {
//        CreateAlignedPairs(teamMembers);
//        if (!participants.alignedPairs.Any())
//            return false;

//        ProcessPairs();
//        // Return true if any aligned pair qualifies as an attacking pair.
//        return participants.alignedPairs.Any(pair => pair.isAttacker);
//    }

//    // Create aligned pairs from team members (actors in the same row or column).
//    private void CreateAlignedPairs(IQueryable<ActorInstance> teamMembers)
//    {
//        foreach (var actor1 in teamMembers)
//        {
//            foreach (var actor2 in teamMembers)
//            {
//                if (actor1 == actor2)
//                    continue;
//                if (!(actor1.isPlaying && actor2.isPlaying))
//                    continue;
//                if (!(actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location)))
//                    continue;
//                if (participants.IsAlignedPair(actor1, actor2))
//                    continue;

//                var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
//                var pair = new ActorPair(actor1, actor2, axis);
//                participants.alignedPairs.Add(pair);
//            }
//        }
//    }

//    /// <summary>
//    /// Process each aligned pair.
//    /// For pairs that qualify as attacking pairs, designate actor1 as the primary attacker,
//    /// assign actor2 as its partner, compute attack results, and assign opponents.
//    /// </summary>
//    private void ProcessPairs()
//    {
//        foreach (var pair in participants.alignedPairs)
//        {
//            if (pair.isAttacker)
//            {
//                pair.actor1.partner = pair.actor2;
//                pair.attackResults = CalculateAttackResults(pair);
//                pair.actor1.opponents = pair.attackResults
//                    .Select(a => a.Opponent)
//                    .Distinct()
//                    .ToList();
//            }
//        }
//    }

//    // Calculates attack results for the given pair.
//    private List<AttackResult> CalculateAttackResults(ActorPair pair)
//    {
//        return pair.opponents.Select(opponent =>
//        {
//            bool isHit = Formulas.IsHit(pair.actor1, opponent);
//            bool isCriticalHit = Formulas.IsCriticalHit(pair.actor1, opponent);
//            int damage = isHit ? Formulas.CalculateDamage(pair.actor1, opponent) : 0;
//            return new AttackResult
//            {
//                Pair = pair,
//                Opponent = opponent,
//                IsHit = isHit,
//                IsCriticalHit = isCriticalHit,
//                Damage = damage,
//            };
//        }).ToList();
//    }

//    private IEnumerator ProcessAttacks()
//    {
//        // Derive attacking pairs from aligned pairs.
//        var attackingPairs = participants.alignedPairs.Where(pair => pair.isAttacker).ToList();
//        foreach (var attackPair in attackingPairs)
//        {
//            GameManager.instance.attackLineManager.Spawn(attackPair);

//            if (attackPair.attackResults == null || !attackPair.attackResults.Any())
//                continue;

//            // Animate the attack: actors grow then shrink.
//            yield return CoroutineHelper.WaitForAll(GameManager.instance,
//                attackPair.actor1.action.Grow(),
//                attackPair.actor2.action.Grow());
//            yield return CoroutineHelper.WaitForAll(GameManager.instance,
//                attackPair.actor1.action.Shrink(),
//                attackPair.actor2.action.Shrink());

//            List<ActorInstance> dyingOpponents = new List<ActorInstance>();

//            // Execute each attack from the pair, with actor1 as the designated attacker.
//            foreach (var attack in attackPair.attackResults)
//            {
//                var attacker = attackPair.actor1;
//                var direction = attacker.GetDirectionTo(attack.Opponent);
//                var trigger = new Trigger(attacker.Attack(attack));
//                yield return attacker.action.Bump(direction, trigger);

//                if (attack.Opponent.isDying)
//                {
//                    dyingOpponents.Add(attack.Opponent);
//                    yield return HandleDeath(attack.Opponent);
//                }
//            }

//            if (dyingOpponents.Any())
//                yield return new WaitUntil(() => dyingOpponents.All(x => x.healthBar.isEmpty));
//        }
//    }

//    // Process support actions that occur BEFORE the attack.
//    private void ProcessPreAttackSupport()
//    {
//        var supportingPairs = participants.alignedPairs.Where(pair => pair.isSupporter).ToList();
//        foreach (var supportPair in supportingPairs)
//        {
//            ActorInstance a = supportPair.actor1;
//            ActorInstance b = supportPair.actor2;

//            // They support each other if they aren’t attacking as a pair.
//            if (a.partner != b)
//            {
//                if (!a.supporters.Contains(b))
//                    a.supporters.Add(b);
//                if (!b.supporters.Contains(a))
//                    b.supporters.Add(a);

//                // Enqueue healing: if a is a Cleric, heal b; if b is a Cleric, heal a.
//                if (a.character == Character.Cleric)
//                    SpellManager.EnqueueHeal(a, b, castBeforeAttack: true);
//                if (b.character == Character.Cleric)
//                    SpellManager.EnqueueHeal(b, a, castBeforeAttack: true);
//            }

//            // Always spawn the support visual.
//            SupportLineManager.Spawn(supportPair);
//        }
//    }

//    // Process support actions that occur AFTER the attack.
//    private void ProcessPostAttackSupport()
//    {
//        var supportingPairs = participants.alignedPairs.Where(pair => pair.isSupporter).ToList();
//        foreach (var supportPair in supportingPairs)
//        {
//            ActorInstance a = supportPair.actor1;
//            ActorInstance b = supportPair.actor2;

//            if (a.partner != b)
//            {
//                if (a.character == Character.Cleric)
//                    SpellManager.EnqueueHeal(a, b, castBeforeAttack: false);
//                if (b.character == Character.Cleric)
//                    SpellManager.EnqueueHeal(b, a, castBeforeAttack: false);
//            }
//        }
//    }

//    private IEnumerator HandleDeath(ActorInstance target)
//    {
//        target.TriggerDie();
//        yield return Wait.UntilNextFrame();
//    }

//    private void SetupCombat()
//    {
//        UpdateSortingOrder();
//        BoardOverlay.TriggerFadeIn();
//    }

//    private void CleanupCombat()
//    {
//        BoardOverlay.TriggerFadeOut();
//        TurnManager.ResetSortingOrder();
//        ClearCombatParticipants();
//    }

//    private void UpdateSortingOrder()
//    {
//        foreach (var actor in Actors.Where(x => x.isPlaying))
//            actor.sortingOrder = SortingOrder.Default;

//        // Derive attacking pairs from aligned pairs.
//        var attackingPairs = participants.alignedPairs.Where(pair => pair.isAttacker).ToList();
//        foreach (var pair in attackingPairs)
//        {
//            pair.actor1.sortingOrder = SortingOrder.Attacker;
//            pair.actor2.sortingOrder = SortingOrder.Attacker;
//            foreach (var opponent in pair.opponents)
//                opponent.sortingOrder = SortingOrder.Target;
//        }

//        var supportingPairs = participants.alignedPairs.Where(pair => pair.isSupporter).ToList();
//        foreach (var pair in supportingPairs)
//        {
//            if (pair.actor1.partner == null)
//                pair.actor1.sortingOrder = SortingOrder.Supporter;
//            if (pair.actor2.partner == null)
//                pair.actor2.sortingOrder = SortingOrder.Supporter;
//        }
//    }

//    private void ClearCombatParticipants()
//    {
//        foreach (var actor in participants.Get())
//        {
//            actor.partner = null;
//            actor.opponents.Clear();
//            actor.supporters.Clear();
//            actor.SetDefault();
//        }
//    }
//}
