// --- File: Assets/Scripts/Events/Sequences/EnemyStartSequence.cs ---
using Assets.Helper;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Builds strict per-enemy order for the enemy team:
    /// e1.move -> e1.attack -> e2.move -> e2.attack -> ... -> EndTurn
    /// </summary>
    public class EnemyStartSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Ensure we only run on the enemy turn.
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            // Disable input during AI resolution.
            g.InputManager.inputMode = InputMode.None;

            // Small pacing to let any visuals settle.
            yield return Wait.None();

            // Collect enemies that are "ready" at turn start.
            // "isReady" means isPlaying && hasMaxAP, so readiness is already established once.
            var ready = g.Actors.Enemies
                .Where(x => x.isReady)
                .ToList();

            // If none are ready, end the turn.
            if (ready.Count == 0)
            {
                g.SequenceManager.Add(new EndTurnSequence());
                g.SequenceManager.Execute();
                yield break;
            }

            // For each ready enemy, enqueue a move followed immediately by an attack for that same enemy.
            foreach (var e in ready)
            {
                g.SequenceManager.Add(new EnemyMoveSequence(e));
                g.SequenceManager.Add(new EnemyPreAttackSequence(e));
                g.SequenceManager.Add(new EnemyAttackSequence(e));
                g.SequenceManager.Add(new EnemyPostAttackSequence(e));
            }

            // After all per-enemy chains, end the enemy turn.
            g.SequenceManager.Add(new EndTurnSequence());

            // Run the global queue.
            g.SequenceManager.Execute();
        }
    }
}
