using Assets.Scripts.Events;
using Assets.Scripts.Models;
using Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

/// <summary>
/// Enum for all possible turn phases.
/// </summary>

/// <summary>
/// Handles the turn-based phase system for both hero and enemy teams.
/// Phases are advanced either by player input (for hero) or by sequences/callbacks (for enemy).
/// </summary>
public class TurnManager : MonoBehaviour
{
    // State
    public int currentTurn = 0;
    public Team currentTeam = Team.Hero;
    public TurnPhase currentPhase = TurnPhase.Start;

    // Convenience
    public bool isHeroTurn => currentTeam == Team.Hero;
    public bool isEnemyTurn => currentTeam == Team.Enemy;

    // Event for phase changes
    public event Action<TurnPhase> OnTurnPhaseChanged;

    private void OnEnable()
    {
        OnTurnPhaseChanged += HandlePhaseChanged;
        if (g.SequenceManager != null)
            g.SequenceManager.OnSequenceComplete += OnSequenceComplete;
    }

    private void OnDisable()
    {
        OnTurnPhaseChanged -= HandlePhaseChanged;
        if (g.SequenceManager != null)
            g.SequenceManager.OnSequenceComplete -= OnSequenceComplete;
    }

    /// <summary>
    /// Initializes the turn system and starts with Hero's Start phase.
    /// </summary>
    public void Initialize()
    {
        currentTurn = 0;
        currentTeam = Team.Hero;
        SetPhase(TurnPhase.Start);
    }

    /// <summary>
    /// Advances to the next team's turn and resets overlays/supportLines'.
    /// </summary>
    public void NextTurn()
    {
        currentTeam = isHeroTurn ? Team.Enemy : Team.Hero;
        g.SupportLineManager.Clear();
        //game.AttackLines.DespawnAll();
        SetPhase(TurnPhase.Start);
    }

    /// <summary>
    /// Sets the new phase and fires the event.
    /// </summary>
    public void SetPhase(TurnPhase nextPhase)
    {
        currentPhase = nextPhase;
        OnTurnPhaseChanged?.Invoke(currentPhase);
    }

    /// <summary>
    /// Responds to phase changes and triggers appropriate logic.
    /// </summary>
    private void HandlePhaseChanged(TurnPhase phase)
    {
        switch (phase)
        {
            case TurnPhase.Start: OnStartPhase(); break;
            case TurnPhase.Move: OnMovePhase(); break;
            case TurnPhase.PreAttack: OnPreAttackPhase(); break;
            case TurnPhase.Attack: OnAttackPhase(); break;
            case TurnPhase.PostAttack: OnPostAttackPhase(); break;
            case TurnPhase.End: OnEndPhase(); break;
        }
    }

    /// <summary>
    /// Handles logic for the Start phase for both teams.
    /// </summary>
    private void OnStartPhase()
    {
        if (isHeroTurn)
        {
            currentTurn++;
            g.TimerBar.Refill();
            GameManager.instance.heroManager.TriggerGlow();
            // Wait for player to trigger next phase via UI
        }
        else if (isEnemyTurn)
        {
            g.TimerBar.Lock();
            g.SequenceManager.Add(new EnemySpawnSequence());

            bool anyReadyEnemies = g.Actors.Enemies.Any(x => x.isPlaying && x.hasMaxAP);
            if (!anyReadyEnemies)
            {
                // No enemies to act; finish up and advance turn
                g.SequenceManager.TriggerExecute();
                NextTurn();
                return;
            }

            g.SequenceManager.Add(new EnemyStartSequence());
            g.SequenceManager.TriggerExecute();
            // Do NOT call SetPhase here; OnSequenceComplete will handle advance to Move
        }
    }

    /// <summary>
    /// Handles the Move phase for both teams.
    /// </summary>
    private void OnMovePhase()
    {
        if (isHeroTurn)
        {
            // Player moves; UI or input must call PlayerEndMovePhase when finished.
        }
        else if (isEnemyTurn)
        {
            // Enqueue move sequences for all ready g.Actors.Enemies.
            foreach (var enemy in g.Actors.Enemies.Where(x => x.isPlaying && x.hasMaxAP))
            {
                g.SequenceManager.Add(new EnemyMoveSequence(enemy));
            }
            g.SequenceManager.TriggerExecute();
            // Do NOT call SetPhase here; OnSequenceComplete will handle advance to PreAttack
        }
    }

    /// <summary>
    /// Handles the PreAttack phase for both teams.
    /// </summary>
    private void OnPreAttackPhase()
    {
        if (isHeroTurn)
        {
            // Wait for player to pick target/confirm attack.
            // UI/input must call PlayerEndPreAttackPhase when ready.
        }
        else if (isEnemyTurn)
        {
            // AI pre-attack logic (target selection, prep), then auto-advance:
            SetPhase(TurnPhase.Attack);
        }
    }

    /// <summary>
    /// Handles the Attack phase for both teams.
    /// </summary>
    private void OnAttackPhase()
    {
        if (isHeroTurn)
        {
            // Wait for player attack resolution/animation.
            // When done, UI/input calls PlayerEndAttackPhase.
        }
        else if (isEnemyTurn)
        {
            // Queue up one attack sequence per attacking enemy
            var attackingEnemies = g.Actors.Enemies.Where(x => x.isPlaying && x.hasMaxAP).ToList();
            attackingEnemies.ForEach(x => g.SequenceManager.Add(new EnemyAttackSequence(x)));
            g.SequenceManager.TriggerExecute();
        }
    }

    /// <summary>
    /// Handles the PostAttack phase for both teams.
    /// </summary>
    private void OnPostAttackPhase()
    {
        if (isHeroTurn)
        {
            // Wait for post-attack cleanup.
            // When done, UI/input calls PlayerEndPostAttackPhase.
        }
        else if (isEnemyTurn)
        {
            // Enemy post-attack logic.
            SetPhase(TurnPhase.End);
        }
    }

    /// <summary>
    /// Handles the End phase for both teams.
    /// </summary>
    private void OnEndPhase()
    {
        // Wrap up, then immediately move to next turn.
        NextTurn();
    }

    /// <summary>
    /// Handles the completion of any sequence. 
    /// Decides which phase to advance to for enemy turns.
    /// </summary>
    private void OnSequenceComplete()
    {
        if (isEnemyTurn)
        {
            if (currentPhase == TurnPhase.Start)
            {
                SetPhase(TurnPhase.Move);
            }
            else if (currentPhase == TurnPhase.Move)
            {
                SetPhase(TurnPhase.PreAttack);
            }
            else if (currentPhase == TurnPhase.Attack)
            {
                SetPhase(TurnPhase.PostAttack);
            }
        }
    }

    // -----------------------------------------------------------------------
    // Methods for player input/UI to manually advance phases for hero (player)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Call from UI or board when player finishes move phase.
    /// </summary>
    public void PlayerEndMovePhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.Move)
            SetPhase(TurnPhase.PreAttack);
    }

    /// <summary>
    /// Call from UI or board when player is done with pre-attack selection/confirmation.
    /// </summary>
    public void PlayerEndPreAttackPhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.PreAttack)
            SetPhase(TurnPhase.Attack);
    }

    /// <summary>
    /// Call from UI or after attack animation completes.
    /// </summary>
    public void PlayerEndAttackPhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.Attack)
            SetPhase(TurnPhase.PostAttack);
    }

    /// <summary>
    /// Call from UI or board when post-attack cleanup is finished.
    /// </summary>
    public void PlayerEndPostAttackPhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.PostAttack)
            SetPhase(TurnPhase.End);
    }
}
