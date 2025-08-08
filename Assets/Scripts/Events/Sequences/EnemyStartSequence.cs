using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Performs any start-of-turn logic for the enemy team and schedules their actions.
    /// Guarantees the sequence queue keeps running even if there are zero ready enemies.
    /// </summary>
    public class EnemyStartSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Only run during enemy turns
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            g.InputManager.inputMode = InputMode.None;

            // Small pacing
            yield return Wait.UntilNextFrame();

            // Snapshot ready enemies once for deterministic ordering
            var ready = g.Actors.Enemies
                .Where(x => x != null && x.isPlaying && x.isReady)
                .ToList();

            if (ready.Count == 0)
            {
                // No one can act -> immediately end enemy turn and run the queue
                g.SequenceManager.Add(new EndTurnSequence());
                g.SequenceManager.TriggerExecute();
                yield break;
            }

            // Enqueue all movers; movement will chain into pre-attack/attack
            foreach (var e in ready)
                g.SequenceManager.Add(new EnemyMoveSequence(e));

            // Important: kick the queue now
            g.SequenceManager.TriggerExecute();
        }
    }
}
