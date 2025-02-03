//using Assets.Scripts.Models;
//using Assets.Scripts.Utilities;
//using Game.Behaviors;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class CombatManager : MonoBehaviour
//{
//    // External properties
//    protected TurnManager turnManager => GameManager.instance.turnManager;
//    protected AttackLineManager attackLineManager => GameManager.instance.attackLineManager;
//    protected AudioManager audioManager => GameManager.instance.audioManager;
//    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
//    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
//    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
//    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
//    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
//    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
//    protected IQueryable<ActorInstance> players => GameManager.instance.players;
//    protected ActorInstance previousSelectedPlayer => GameManager.instance.previousSelectedPlayer;

//    private CombatParticipants participants = new CombatParticipants();

//    public void Clear() => participants.Clear();

//    /// <summary>
//    /// Runs the combat phase. (This coroutine does not call turnManager.NextTurn() at its end.)
//    /// </summary>
//    public IEnumerator CombatCoroutine()
//    {
//        participants.Clear();

//        // For the purposes of combat, choose the team based on the current turn:
//        IQueryable<ActorInstance> teamMembers = turnManager.isPlayerTurn ? players : enemies;
//        if (!AssignParticipants(teamMembers))
//        {
//            // If no valid combat participants were found, simply exit.
//            yield break;
//        }

//        SetupCombatState();

//        foreach (var pair in participants.attackingPairs)
//        {
//            attackLineManager.Spawn(pair);
//            yield return ResolveAttack(pair);
//        }

//        CleanupCombatState();

//        // Do not call turnManager.NextTurn() here—
//        // the TurnManager will advance the turn once all actions for this phase have executed.
//        yield break;
//    }

//    // --- (The remainder of your existing CombatManager methods remain unchanged) ---
//    private bool AssignParticipants(IQueryable<ActorInstance> teamMembers)
//    {
//        AssignAlignedPairs(teamMembers);
//        if (!participants.alignedPairs.Any()) return false;
//        AssignAttackingPairs();
//        return participants.attackingPairs.Any();
//    }

//    private bool AreActorsAligned(ActorInstance actor1, ActorInstance actor2)
//    {
//        return actor1 != null && actor2 != null && actor1 != actor2 &&
//               actor1.isActive && actor1.isAlive &&
//               actor2.isActive && actor2.isAlive &&
//               (actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location));
//    }

//    private void AssignAlignedPairs(IQueryable<ActorInstance> teamMembers)
//    {
//        foreach (var actor1 in teamMembers)
//        {
//            foreach (var actor2 in teamMembers)
//            {
//                if (AreActorsAligned(actor1, actor2) && !participants.HasAlignedPair(actor1, actor2))
//                {
//                    var pair = CreateAlignedPair(actor1, actor2);
//                    participants.alignedPairs.Add(pair);
//                }
//            }
//        }
//    }

//    private ActorPair CreateAlignedPair(ActorInstance actor1, ActorInstance actor2)
//    {
//        var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
//        return new ActorPair(actor1, actor2, axis);
//    }

//    private void AssignAttackingPairs()
//    {
//        foreach (var pair in participants.alignedPairs)
//        {
//            if (pair.hasOpponentsBetween && !participants.HasAttackingPair(pair))
//            {
//                participants.attackingPairs.Add(pair);
//                // Precompute attack results and add each as a TurnAction
//                pair.attackResults = CalculateAttackResults(pair);
//                foreach (var result in pair.attackResults)
//                {
//                    turnManager.AddAction(TurnPhase.Attack, new AttackAction(result));
//                }
//            }
//        }
//    }

//    private List<AttackResult> CalculateAttackResults(ActorPair pair)
//    {
//        return pair.opponents.Select(opponent =>
//        {
//            var isHit = Formulas.IsHit(pair.actor1, opponent);
//            var isCriticalHit = Formulas.IsCriticalHit(pair.actor1, opponent);
//            var damage = isHit ? Formulas.CalculateDamage(pair.actor1, opponent) : 0;
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

//    private IEnumerator ResolveAttack(ActorPair pair)
//    {
//        if (pair.attackResults == null || pair.attackResults.Count == 0)
//            yield break;

//        yield return StartCoroutine(GrowAndShrink(pair.actor1, pair.actor2));

//        foreach (var attack in pair.attackResults)
//        {
//            yield return StartCoroutine(PerformAttack(pair.actor1, attack));
//        }
//    }

//    private IEnumerator GrowAndShrink(ActorInstance actor1, ActorInstance actor2)
//    {
//        yield return CoroutineHelper.WaitForAll(this, actor1.action.Grow(), actor2.action.Grow());
//        yield return CoroutineHelper.WaitForAll(this, actor1.action.Shrink(), actor2.action.Shrink());
//    }

//    private IEnumerator PerformAttack(ActorInstance attacker, AttackResult attack)
//    {
//        var direction = attacker.GetDirectionTo(attack.Opponent);
//        var trigger = new Trigger(attacker.Attack(attack));
//        yield return attacker.action.Bump(direction, trigger);

//        if (attack.Opponent.isDying)
//        {
//            yield return HandleDeath(attack.Opponent);
//        }
//    }

//    private IEnumerator HandleDeath(ActorInstance target)
//    {
//        target.TriggerDie();
//        yield return null;
//    }

//    private void SetupCombatState()
//    {
//        UpdateSortingOrder();
//        boardOverlay.TriggerFadeIn();
//    }

//    private void UpdateSortingOrder()
//    {
//        foreach (var actor in actors.Where(x => x.isActive && x.isAlive))
//        {
//            actor.sortingOrder = SortingOrder.Default;
//        }

//        foreach (var pair in participants.attackingPairs)
//        {
//            pair.actor1.sortingOrder = SortingOrder.Attacker;
//            foreach (var opponent in pair.opponents)
//            {
//                opponent.sortingOrder = SortingOrder.Target;
//            }
//        }
//    }

//    private void CleanupCombatState()
//    {
//        boardOverlay.TriggerFadeOut();
//        turnManager.ResetSortingOrder();
//        ClearCombatParticipants();
//    }

//    private void ClearCombatParticipants()
//    {
//        foreach (var actor in participants.Get())
//        {
//            actor.attackingPairCount = 0;
//            actor.supportingPairCount = 0;
//            actor.SetDefault();
//        }
//    }
//}
