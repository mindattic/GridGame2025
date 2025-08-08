using Assets.Scripts.Events;
using Assets.Scripts.Models;
using Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

/// <summary>
/// Handles the turn-based phase system for both hero and enemy teams.
/// Phases are advanced by player input for heroes and by sequence completion for enemies.
/// </summary>
public class TurnManager : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // State
    // ------------------------------------------------------------------------

    // Tracks the turn number. Increments when a new hero turn begins.
    public int currentTurn = 0;

    // Tracks which team is currently active.
    public Team currentTeam = Team.Hero;

    // Tracks the current phase of the active team's turn.
    public TurnPhase currentPhase = TurnPhase.Start;

    // Convenience flags for the active team checks.
    public bool isHeroTurn => currentTeam == Team.Hero;
    public bool isEnemyTurn => currentTeam == Team.Enemy;

    // ------------------------------------------------------------------------
    // Events
    // ------------------------------------------------------------------------

    // Raised when the current phase changes.
    public event Action<TurnPhase> OnTurnPhaseChanged;

    // Subscribes to internal and external events when enabled.
    private void OnEnable()
    {
        OnTurnPhaseChanged += HandlePhaseChanged;

        if (g.SequenceManager != null)
            g.SequenceManager.OnSequenceComplete += OnSequenceComplete;
    }

    // Unsubscribes from events when disabled.
    private void OnDisable()
    {
        OnTurnPhaseChanged -= HandlePhaseChanged;

        if (g.SequenceManager != null)
            g.SequenceManager.OnSequenceComplete -= OnSequenceComplete;
    }

    // ------------------------------------------------------------------------
    // Lifecycle
    // ------------------------------------------------------------------------

    /// <summary>
    /// Initializes the turn system and starts with the Hero Start phase.
    /// </summary>
    public void Initialize()
    {
        currentTurn = 0;
        currentTeam = Team.Hero;

        SetPhase(TurnPhase.Start);
    }

    /// <summary>
    /// Advances to the next team's turn and resets transient visuals.
    /// </summary>
    public void NextTurn()
    {
        // Alternate teams.
        currentTeam = isHeroTurn ? Team.Enemy : Team.Hero;

        // Clear support lines or any other per-turn overlays.
        g.SupportLineManager.Clear();

        // Begin the next turn at Start.
        SetPhase(TurnPhase.Start);
    }

    /// <summary>
    /// Sets the new phase and fires the phase changed event.
    /// </summary>
    public void SetPhase(TurnPhase nextPhase)
    {
        currentPhase = nextPhase;
        OnTurnPhaseChanged?.Invoke(currentPhase);
    }

    // ------------------------------------------------------------------------
    // Phase router
    // ------------------------------------------------------------------------

    /// <summary>
    /// Responds to phase changes and triggers phase-specific logic.
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

    // ------------------------------------------------------------------------
    // Start phase
    // ------------------------------------------------------------------------

    /// <summary>
    /// Handles logic for the Start phase for both teams.
    /// </summary>
    private void OnStartPhase()
    {
        if (isHeroTurn)
        {
            // Increment the visible turn counter at the start of the hero turn.
            currentTurn++;

            // Refill the player timer and apply any visual cues.
            g.TimerBar2D.Refill();
            GameManager.instance.heroManager.TriggerGlow();

            // Wait for player input to move to the next phase.
            return;
        }

        if (isEnemyTurn)
        {
            // Lock the timer and spawn any pending enemies.
            g.TimerBar2D.Lock();
            g.SequenceManager.Add(new EnemySpawnSequence());

            // If no enemies are ready, end the enemy turn immediately.
            bool anyReadyEnemies = g.Actors.Enemies.Any(x => x.isPlaying && x.hasMaxAP);
            if (!anyReadyEnemies)
            {
                // Execute any spawn visuals then flip to the next turn.
                g.SequenceManager.TriggerExecute();
                NextTurn();
                return;
            }

            // Run any start-of-turn enemy visuals or prep.
            g.SequenceManager.Add(new EnemyStartSequence());
            g.SequenceManager.TriggerExecute();

            // Do not set phase here. OnSequenceComplete will advance to Move.
            return;
        }
    }

    // ------------------------------------------------------------------------
    // Move phase
    // ------------------------------------------------------------------------

    /// <summary>
    /// Handles the Move phase for both teams.
    /// </summary>
    private void OnMovePhase()
    {
        if (isHeroTurn)
        {
            // Player can drag and drop to move. UI calls PlayerEndMovePhase when done.
            return;
        }

        if (isEnemyTurn)
        {
            // Enqueue move sequences for each ready enemy.
            int added = 0;

            foreach (var enemy in g.Actors.Enemies.Where(x => x.isReady))
            {
                g.SequenceManager.Add(new EnemyMoveSequence(enemy));
                added++;
            }

            // If at least one move was queued, execute and wait for completion callback.
            if (added > 0)
            {
                g.SequenceManager.TriggerExecute();

                // Do not set phase here. OnSequenceComplete will advance to PreAttack.
                return;
            }

            // If no moves were queued, advance to PreAttack to prevent a stall.
            SetPhase(TurnPhase.PreAttack);
            return;
        }
    }

    // ------------------------------------------------------------------------
    // PreAttack phase
    // ------------------------------------------------------------------------

    /// <summary>
    /// Handles the PreAttack phase for both teams.
    /// </summary>
    private void OnPreAttackPhase()
    {
        if (isHeroTurn)
        {
            // Wait for player to select and confirm attacks.
            // UI calls PlayerEndPreAttackPhase to proceed.
            return;
        }

        if (isEnemyTurn)
        {
            // Enemy AI can preselect targets if needed.
            // Immediately advance to Attack for automated flow.
            SetPhase(TurnPhase.Attack);
            return;
        }
    }

    // ------------------------------------------------------------------------
    // Attack phase
    // ------------------------------------------------------------------------

    /// <summary>
    /// Handles the Attack phase for both teams.
    /// </summary>
    private void OnAttackPhase()
    {
        if (isHeroTurn)
        {
            // Wait for player attack resolution.
            // UI calls PlayerEndAttackPhase when complete.
            return;
        }

        if (isEnemyTurn)
        {
            // Collect enemies that are ready to attack.
            var attackers = g.Actors.Enemies
                .Where(x => x.isPlaying && x.hasMaxAP)
                .ToList();

            // If no attackers are ready, skip to PostAttack to prevent a stall.
            if (attackers.Count == 0)
            {
                SetPhase(TurnPhase.PostAttack);
                return;
            }

            // Enqueue an attack for each ready enemy.
            attackers.ForEach(x => g.SequenceManager.Add(new EnemyAttackSequence(x)));

            // Execute attacks. OnSequenceComplete will advance to PostAttack.
            g.SequenceManager.TriggerExecute();
            return;
        }
    }

    // ------------------------------------------------------------------------
    // PostAttack phase
    // ------------------------------------------------------------------------

    /// <summary>
    /// Handles the PostAttack phase for both teams.
    /// </summary>
    private void OnPostAttackPhase()
    {
        if (isHeroTurn)
        {
            // Perform any cleanup. UI calls PlayerEndPostAttackPhase to proceed.
            return;
        }

        if (isEnemyTurn)
        {
            // Enemy cleanup can be placed here if needed, then end the turn.
            SetPhase(TurnPhase.End);
            return;
        }
    }

    // ------------------------------------------------------------------------
    // End phase
    // ------------------------------------------------------------------------

    /// <summary>
    /// Finalizes the current turn and advances to the next team.
    /// </summary>
    private void OnEndPhase()
    {
        // Immediately begin the next team's Start phase.
        NextTurn();
    }

    // ------------------------------------------------------------------------
    // Sequence completion
    // ------------------------------------------------------------------------

    /// <summary>
    /// Handles the completion of a batch of queued sequences.
    /// Controls enemy phase progression between Start, Move, Attack, and PostAttack.
    /// </summary>
    private void OnSequenceComplete()
    {
        if (!isEnemyTurn)
            return;

        // Advance enemy phases when a queued batch ends.
        if (currentPhase == TurnPhase.Start)
        {
            SetPhase(TurnPhase.Move);
            return;
        }

        if (currentPhase == TurnPhase.Move)
        {
            SetPhase(TurnPhase.PreAttack);
            return;
        }

        if (currentPhase == TurnPhase.Attack)
        {
            SetPhase(TurnPhase.PostAttack);
            return;
        }
    }

    // ------------------------------------------------------------------------
    // Hero UI helpers
    // ------------------------------------------------------------------------

    /// <summary>
    /// Called by UI when the player finishes movement.
    /// </summary>
    public void PlayerEndMovePhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.Move)
            SetPhase(TurnPhase.PreAttack);
    }

    /// <summary>
    /// Called by UI when the player confirms attacks.
    /// </summary>
    public void PlayerEndPreAttackPhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.PreAttack)
            SetPhase(TurnPhase.Attack);
    }

    /// <summary>
    /// Called by UI when the player's attack resolution finishes.
    /// </summary>
    public void PlayerEndAttackPhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.Attack)
            SetPhase(TurnPhase.PostAttack);
    }

    /// <summary>
    /// Called by UI when post attack cleanup is complete.
    /// </summary>
    public void PlayerEndPostAttackPhase()
    {
        if (isHeroTurn && currentPhase == TurnPhase.PostAttack)
            SetPhase(TurnPhase.End);
    }
}
