using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Handles any logic or animations that occur before an enemy attacks,
    /// then reliably hands off to either the enemy's attack, another ready enemy,
    /// or ends the turn if no enemies are ready.
    /// </summary>
    public class EnemyPreAttackSequence : SequenceEvent
    {
        private readonly ActorInstance enemy;

        public EnemyPreAttackSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        public override IEnumerator Execute()
        {
            // Only run during enemy turns
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            // If this enemy is invalid or not in play, immediately hand off to next or end turn
            if (enemy == null || !enemy.isPlaying)
            {
                EnqueueNextReadyOrEnd(ignore: enemy);
                yield break;
            }

            // Optional: pre-attack anticipation
            yield return Wait.UntilNextFrame();

            // If this enemy cannot act right now, hand off to another ready enemy or end turn
            // Do NOT silently exit, or the queue can stall on the enemy turn.
            if (!enemy.isReady || !enemy.hasMaxAP)
            {
                EnqueueNextReadyOrEnd(ignore: enemy);
                yield break;
            }

            // Otherwise proceed to attack for this enemy and keep the queue running
            g.SequenceManager.Add(new EnemyAttackSequence(enemy));
            g.SequenceManager.TriggerExecute();
        }

        /// <summary>
        /// Finds another ready enemy to continue the turn. If none are ready, ends the turn.
        /// Always triggers the sequence manager so the queue keeps advancing.
        /// </summary>
        private static void EnqueueNextReadyOrEnd(ActorInstance ignore = null)
        {
            var next = g.Actors.Enemies
                .FirstOrDefault(x => x != null && x != ignore && x.isPlaying && x.isReady && x.hasMaxAP);

            if (next != null)
                g.SequenceManager.Add(new EnemyMoveSequence(next));
            else
                g.SequenceManager.Add(new EndTurnSequence());

            g.SequenceManager.TriggerExecute();
        }
    }
}
