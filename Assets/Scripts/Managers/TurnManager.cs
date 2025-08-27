// --- File: Assets/Scripts/Managers/TurnManager.cs ---
using Assets.Scripts.Sequences;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Central turn flow controller. The timeline is the source of truth.
    /// After each turn completes we advance the belt one block, read the new
    /// current block, and enqueue the correct side's start sequence.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public bool IsHeroTurn { get; private set; }
        public bool IsEnemyTurn => !IsHeroTurn;
        public int CurrentTurn = 0;

        /// <summary>
        /// Initialize the first turn from the timeline's current block.
        /// Defaults to hero if the timeline is not yet available.
        /// </summary>
        public void Initialize()
        {
            var enemyAtCursor = (g.Timeline != null)
                ? g.Timeline.GetCurrentEnemy()
                : null;

            IsHeroTurn = enemyAtCursor == null;
            StartTurn();
        }

        /// <summary>
        /// Finish the current turn, advance the belt one block, then start
        /// whatever block lands under the indicator. No blind toggling.
        /// </summary>
        public void NextTurn()
        {
            CurrentTurn++;

            // 1) Advance the belt exactly once.
            if (g.Timeline != null)
                g.Timeline.NextBlock();

            // 2) Decide who acts based on the new current block.
            var enemyAtCursor = (g.Timeline != null)
                ? g.Timeline.GetCurrentEnemy()
                : null;

            IsHeroTurn = enemyAtCursor == null;

            // 3) Focus the belt on the block that will act.
            if (g.Timeline != null)
            {
                if (IsHeroTurn)
                    g.Timeline.FocusOnHero();
                else
                    g.Timeline.FocusOnEnemy(enemyAtCursor);
            }

            // 4) Enqueue the correct start sequence.
            if (IsHeroTurn)
            {

                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                // Drive the exact enemy predicted by the timeline.
                // EnemyStartSequence picks by readiness; EnemyTakeTurnSequence acts this one.
                g.SequenceManager.Add(new EnemyTakeTurnSequence(enemyAtCursor));
            }
        }

        /// <summary>
        /// Start the very first block without advancing the belt.
        /// Uses the timeline's current block to pick hero or enemy.
        /// </summary>
        private void StartTurn()
        {
            if (g.Timeline != null)
            {
                var enemyAtCursor = g.Timeline.GetCurrentEnemy();
                IsHeroTurn = enemyAtCursor == null;

                if (IsHeroTurn)
                    g.Timeline.FocusOnHero();
                else
                    g.Timeline.FocusOnEnemy(enemyAtCursor);
            }

            if (IsHeroTurn)
            {
                // Do not increment CurrentTurn here. We count when a hero block begins
                // because Initialize can be called multiple times during setup.
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                var enemyAtCursor = g.Timeline?.GetCurrentEnemy();
                g.SequenceManager.Add(new EnemyTakeTurnSequence(enemyAtCursor));
            }
        }
    }
}
