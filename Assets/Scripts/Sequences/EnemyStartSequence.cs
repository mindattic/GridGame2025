// --- File: Assets/Scripts/Events/EnemyStartSequence.cs ---
using Assets.Helper;
using Assets.Scripts.Sequences;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Starts the enemy turn using the Timeline as source of truth.
    /// Centers on the acting enemy from the current timeline block,
    /// runs its move/attack chain, then queues turn end.
    /// </summary>
    public class EnemyStartSequence : SequenceEvent
    {
        public override IEnumerator ProcessRoutine()
        {
            if (!g.TurnManager.IsEnemyTurn)
                yield break;

            g.InputManager.InputMode = InputMode.None;

            // Ask the Timeline which enemy is acting on the current block.
            var actingEnemy = g.Timeline.GetActingEnemyForCurrentBlock();
            if (actingEnemy == null || !actingEnemy.IsPlaying)
            {
                // No enemy on this block (or dead): end turn cleanly.
                g.SequenceManager.Add(new EndTurnSequence());
                g.SequenceManager.Execute();
                yield break;
            }

            // Snap focus to the acting enemy's block for clarity.
            g.Timeline.FocusOnEnemyTurnNow(actingEnemy);

            // Execute the enemy's behavior.
            g.SequenceManager.Add(new EnemyMoveSequence(actingEnemy));
            g.SequenceManager.Add(new EnemyPreAttackSequence(actingEnemy));
            g.SequenceManager.Add(new EnemyAttackSequence(actingEnemy));
            g.SequenceManager.Add(new EnemyPostAttackSequence(actingEnemy));

            // Cleanup and advance one block.
            g.SequenceManager.Add(new DeathSequence());
            g.SequenceManager.Add(new EndTurnSequence());

            g.SequenceManager.Execute();
        }
    }
}
