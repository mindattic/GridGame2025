using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Moves a single enemy on its turn, then queues its pre-attack step.
    /// Never leaves the sequence queue idle.
    /// </summary>
    public class EnemyMoveSequence : SequenceEvent
    {
        private readonly ActorInstance enemy;

        public EnemyMoveSequence(ActorInstance enemy) => this.enemy = enemy;

        public override IEnumerator Execute()
        {
            // Must be enemy turn
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            // Validate actor
            if (enemy == null || !enemy.isPlaying)
            {
                // Hand off to turn resolution so the queue doesn’t stall
                g.SequenceManager.Add(new EndTurnSequence());
                g.SequenceManager.TriggerExecute();
                yield break;
            }

            // If not ready, skip this unit cleanly
            if (!enemy.isReady)
            {
                g.SequenceManager.Add(new EndTurnSequence());
                g.SequenceManager.TriggerExecute();
                yield break;
            }

            // If this unit has no AP to move, still continue the flow into pre-attack
            if (!enemy.hasMaxAP)
            {
                g.SequenceManager.Add(new EnemyPreAttackSequence(enemy));
                g.SequenceManager.TriggerExecute();
                yield break;
            }

            // Pacing before move
            yield return Wait.For(Intermission.Before.Enemy.Move);

            // Plan and move
            enemy.CalculateAttackStrategy();
            yield return enemy.move.TowardDestination();

            // Chain into pre-attack and keep the queue running
            g.SequenceManager.Add(new EnemyPreAttackSequence(enemy));
            g.SequenceManager.TriggerExecute();
        }
    }
}
