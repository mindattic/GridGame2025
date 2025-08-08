// --- File: Assets/Scripts/Managers/TurnManager.cs ---
using Assets.Scripts.Events;
using Assets.Scripts.Models;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Owns turn state and is the single place that enqueues the correct "start of side" sequence.
    /// This prevents stalls caused by missing or misplaced sequencing when turns flip.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        // True when it is the hero side's turn
        public bool isHeroTurn { get; private set; }

        // Convenience flag for the enemy side
        public bool isEnemyTurn => !isHeroTurn;

        // 1-based turn counter for hero turns; increments when hero turn begins
        public int currentTurn = 0;

        // --------------------------------------------------------------------
        // Initialization
        // --------------------------------------------------------------------

        /// <summary>
        /// Sets up initial state and kicks off the first side's start sequence.
        /// </summary>
        public void Initialize()
        {
            // Start on hero side by design
            isHeroTurn = true;

            // Make sure the correct side start sequence runs immediately
            EnterCurrentSide();
        }

        // --------------------------------------------------------------------
        // Turn flow
        // --------------------------------------------------------------------

        /// <summary>
        /// Flips the active side and then enqueues that side's start sequence.
        /// </summary>
        public void NextTurn()
        {
            // Swap active side
            isHeroTurn = !isHeroTurn;

            // If we just landed on hero side, this is a new numbered turn
            if (isHeroTurn)
                currentTurn++;

            // Always enqueue the correct "start of side" sequence
            EnterCurrentSide();
        }

        /// <summary>
        /// Called by UI when the player clicks End Turn.
        /// Enqueues the EndTurnSequence which should perform any cleanup before NextTurn.
        /// </summary>
        public void EndHeroTurn()
        {
            // Add end of hero turn sequence to the queue and start executing
            g.SequenceManager.Add(new EndTurnSequence());
            g.SequenceManager.TriggerExecute();
        }

        // --------------------------------------------------------------------
        // Helpers
        // --------------------------------------------------------------------

        /// <summary>
        /// Enqueues the appropriate start sequence for whichever side is active.
        /// This centralizes the responsibility so it cannot be forgotten elsewhere.
        /// </summary>
        private void EnterCurrentSide()
        {
            // Safety: if something else was mid-run, we simply enqueue to the same queue.
            // The SequenceManager will drain items in order.
            if (isHeroTurn)
            {
                Debug.Log($"[TurnManager] Enter hero turn. Turn={currentTurn + 1}");
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                Debug.Log("[TurnManager] Enter enemy turn.");
                g.SequenceManager.Add(new EnemyStartSequence());
            }

            // Ensure execution is running (safe no-op if already executing)
            g.SequenceManager.TriggerExecute();
        }
    }
}
