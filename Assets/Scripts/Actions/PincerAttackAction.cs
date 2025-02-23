using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PincerAttackAction : TurnAction
{
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected SpellManager spellManager => GameManager.instance.spellManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    private PincerAttackParticipants participants = new PincerAttackParticipants();

    public override IEnumerator Execute()
    {
        participants.Clear();

        // Use the player team for combat.
        IQueryable<ActorInstance> teamMembers = players.Where(x => x.isPlaying);

        if (!AssignParticipants(teamMembers))
        {
            // No valid aligned pairs; simply exit.
            yield break;
        }

        //Setup combat 
        UpdateSortingOrder();
        boardOverlay.TriggerFadeIn();

        foreach (var pair in participants.attackingPairs)
        {
            // Spawn an attack line for visual effect.
            GameManager.instance.attackLineManager.Spawn(pair);
            yield return ResolveAttack(pair);
        }

        //Cleanup combat
        boardOverlay.TriggerFadeOut();
        turnManager.ResetSortingOrder();
        ClearCombatParticipants();

        yield break;
    }

    // Returns true if there is at least one valid attacking pair.
    private bool AssignParticipants(IQueryable<ActorInstance> teamMembers)
    {
        AssignAlignedPairs(teamMembers);
        if (!participants.alignedPairs.Any())
            return false;

        AssignAttackingPairs();
        AssignSupportingPairs();

        var hasAttackers = participants.attackingPairs.Any();
        return hasAttackers;
    }

    // Returns true if the two actors are aligned and both active.
    private bool AreActorsAligned(ActorInstance actor1, ActorInstance actor2)
    {
        return actor1 != null && actor2 != null && actor1 != actor2 &&
               actor1.isPlaying && actor2.isPlaying &&
               (actor1.IsSameColumn(actor2.location) || actor1.IsSameRow(actor2.location));
    }


    private void AssignAlignedPairs(IQueryable<ActorInstance> teamMembers)
    {
        foreach (var actor1 in teamMembers)
        {
            foreach (var actor2 in teamMembers)
            {
                if (AreActorsAligned(actor1, actor2) && !participants.HasAlignedPair(actor1, actor2))
                {
                    var pair = CreateAlignedPair(actor1, actor2);
                    participants.alignedPairs.Add(pair);
                }
            }
        }
    }

    private ActorPair CreateAlignedPair(ActorInstance actor1, ActorInstance actor2)
    {
        var axis = actor1.IsSameColumn(actor2.location) ? Axis.Vertical : Axis.Horizontal;
        return new ActorPair(actor1, actor2, axis);
    }

    private void AssignAttackingPairs()
    {
        foreach (var pair in participants.alignedPairs)
        {
            //Ensure there are NO gaps or allies between attackers
            if (!pair.hasOpponentsBetween || pair.hasGapsBetween || pair.hasAlliesBetween) continue;

            participants.attackingPairs.Add(pair);
            pair.attackResults = CalculateAttackResults(pair); //Precompute attack results only if valid

            if (pair.attackResults.Any())
            {
                foreach (var attack in pair.attackResults)
                {
                    //Debug.Log($"Pincer attack! {pair.actor1.name} and {pair.actor2.name} attacking {attack.Opponent.name}");
                    actionManager.AddAction(new AttackAction(attack));
                }
            }

        }
    }


    private void AssignSupportingPairs()
    {
        foreach (var pair in participants.alignedPairs)
        {
            // Ensure the pair is valid for support: aligned, has no allies or enemies between, but can have gaps
            if (!pair.hasOpponentsBetween && !pair.hasAlliesBetween)
            {
                participants.supportingPairs.Add(pair);
            }
        }

        // If there are supporting pairs, spawn support visuals
        if (participants.supportingPairs.Any())
        {
            foreach (var pair in participants.supportingPairs)
            {
                supportLineManager.Spawn(pair);
                if (pair.actor1.character == Character.Cleric)
                    spellManager.CastHeal(pair.actor1, pair.actor2);
                else if (pair.actor2.character == Character.Cleric)
                    spellManager.CastHeal(pair.actor2, pair.actor1);
            }
        }
    }


    private bool IsBetween(ActorInstance attacker1, ActorInstance attacker2, ActorInstance opponent)
    {
        int minX = Mathf.Min(attacker1.location.x, attacker2.location.x);
        int maxX = Mathf.Max(attacker1.location.x, attacker2.location.x);
        int minY = Mathf.Min(attacker1.location.y, attacker2.location.y);
        int maxY = Mathf.Max(attacker1.location.y, attacker2.location.y);

        //Check if opponent is within the bounds formed by attacker1 and attacker2
        return (opponent.location.x > minX && opponent.location.x < maxX) ||
               (opponent.location.y > minY && opponent.location.y < maxY);
    }


    private List<AttackResult> CalculateAttackResults(ActorPair pair)
    {
        //TODO: Somehow combine actor1 and actor 2 attacks?...
        return pair.opponents.Select(opponent =>
        {
            var isHit = Formulas.IsHit(pair.actor1, opponent);
            var isCriticalHit = Formulas.IsCriticalHit(pair.actor1, opponent);
            var damage = isHit ? Formulas.CalculateDamage(pair.actor1, opponent) : 0;
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

    private IEnumerator ResolveAttack(ActorPair pair)
    {
        if (pair.attackResults == null || pair.attackResults.Count == 0)
            yield break;

        //Grow and shrink
        yield return CoroutineHelper.WaitForAll(GameManager.instance, pair.actor1.action.Grow(), pair.actor2.action.Grow());
        yield return CoroutineHelper.WaitForAll(GameManager.instance, pair.actor1.action.Shrink(), pair.actor2.action.Shrink());

        List<ActorInstance> dyingOpponents = new List<ActorInstance>();

        foreach (var attack in pair.attackResults)
        {
            //yield return PerformAttack(pair.actor1, attack);
            var attacker = attack.Pair.actor1; //TODO: Somehow combine actor1 and actor2?...
            var direction = attacker.GetDirectionTo(attack.Opponent);
            var trigger = new Trigger(attacker.Attack(attack));
            yield return attacker.action.Bump(direction, trigger);

            // If the opponent is dying, handle death
            if (attack.Opponent.isDying)
            {
                dyingOpponents.Add(attack.Opponent);
                yield return HandleDeath(attack.Opponent);
            }
        }

        //Wait until all deaths have completed before moving to next pair
        if (dyingOpponents.Any())
            yield return new WaitUntil(() => dyingOpponents.All(x => x.healthBar.isEmpty));
    }

    private IEnumerator HandleDeath(ActorInstance target)
    {
        target.TriggerDie();
        yield return Wait.UntilNextFrame();
    }


    private void UpdateSortingOrder()
    {
        foreach (var actor in actors.Where(x => x.isPlaying))
        {
            actor.sortingOrder = SortingOrder.Default;
        }

        foreach (var pair in participants.attackingPairs)
        {
            pair.actor1.sortingOrder = SortingOrder.Attacker;
            pair.actor2.sortingOrder = SortingOrder.Attacker;

            foreach (var opponent in pair.opponents)
            {
                opponent.sortingOrder = SortingOrder.Target;
            }
        }

        foreach (var actor in participants.supportingPairs)
        {
            actor.sortingOrder = SortingOrder.Supporter;
        }

    }



    private void ClearCombatParticipants()
    {
        foreach (var actor in participants.Get())
        {
            actor.attackingPairCount = 0;
            actor.supportingPairCount = 0;
            actor.SetDefault();
        }
    }
}
