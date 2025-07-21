using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using g = GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Moves a single enemy on its turn.
    /// </summary>
    public class EnemyMoveSequence : SequenceEvent
    {
        private ActorInstance enemy;

        public EnemyMoveSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        public override IEnumerator Execute()
        {
            // Only proceed if it is the enemy's turn and this enemy is valid.
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            if (enemy == null || !enemy.isPlaying || !enemy.hasMaxAP)
                yield break;

            // Wait before starting move (for pacing, animation, or suspense).
            yield return Wait.For(Intermission.Before.Enemy.Move);

            // Calculate move/attack strategy and move the enemy.
            enemy.CalculateAttackStrategy();
            yield return enemy.move.TowardDestination();

            // Do NOT queue attack or change phase here.
            // Let TurnManager/SequenceManager handle attack phase after all moves complete.
        }
    }
}
