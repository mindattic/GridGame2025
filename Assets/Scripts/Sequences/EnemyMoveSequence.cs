// --- File: Assets/Scripts/Events/Sequences/EnemyMoveSequence.cs ---
using System.Collections;

namespace Assets.Scripts.Sequences
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

        public override IEnumerator ProcessRoutine()
        {
            // Safety: null or not in play should quietly skip.
            if (enemy == null || !enemy.IsPlaying)
                yield break;

            // Optional pacing before movement.
            yield return Wait.For(Intermission.Before.Enemy.Move);

            // Decide path and Move toward destination.
            enemy.CalculateAttackStrategy();
            yield return enemy.Move.TowardDestinationRoutine();

            // No chaining here. EnemyStartSequence enqueued the follow-up attack explicitly.
        }
    }
}
