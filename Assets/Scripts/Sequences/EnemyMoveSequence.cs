// --- File: Assets/Scripts/Events/Sequences/EnemyMoveSequence.cs ---
using Assets.Helper;
using System.Collections;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Moves one attacker toward its target.
    /// Does not end the turn or schedule other enemies.
    /// </summary>
    public class EnemyMoveSequence : SequenceEvent
    {
        private readonly ActorInstance enemy;

        public EnemyMoveSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        public override IEnumerator Execute()
        {
            // Safety: null or not in play should quietly skip.
            if (enemy == null || !enemy.isPlaying)
                yield break;

            // Optional pacing before movement.
            yield return Wait.For(Intermission.Before.Enemy.Move);

            // Decide path and move toward destination.
            enemy.CalculateAttackStrategy();
            yield return enemy.move.TowardDestination();

            // No chaining here. EnemyStartSequence enqueued the follow-up attack explicitly.
        }
    }
}
