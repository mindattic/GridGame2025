// --- File: Assets/Scripts/Managers/TurnManager.cs ---
using Assets.Scripts.Events;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Controls which side is active and execute each side's start sequence when turns change.
    /// Keeps turn flow centralized so sequencing cannot stall due to missing or misplaced calls.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public bool IsHeroTurn { get; private set; }
        public bool isEnemyTurn => !IsHeroTurn;
        public int currentTurn = 0;

        /// <summary>
        /// Set initial state and kick off the first side's start sequence.
        /// Heroes begin first and currentTurn remains zero until their next activation.
        /// </summary>
        public void Initialize()
        {
            IsHeroTurn = true;
            StartTurn();
        }

        /// <summary>
        /// Flip the active side, increment hero turn counter when heroes become active,
        /// then enqueue that side's start sequence.
        /// </summary>
        public void NextTurn()
        {
            IsHeroTurn = !IsHeroTurn;
            if (IsHeroTurn)
            {
                currentTurn++;

                // On the moment the hero turn begins:
                g.Timeline.StartHeroTurnWindow();
            }
              

            StartTurn();
        }

        /// <summary>
        /// EnqueueRoutine the appropriate start sequence for the active side.
        /// SequenceManager executes items in order, and ProcessRoutine is safe to call repeatedly.
        /// </summary>
        private void StartTurn()
        {
            // EnqueueRoutine the correct start sequence for the active side
            g.SequenceManager.Add(IsHeroTurn ? new HeroStartSequence() : new EnemyStartSequence());

         
            // Ensure execution is running
            g.SequenceManager.Execute();
        }
    }
}
