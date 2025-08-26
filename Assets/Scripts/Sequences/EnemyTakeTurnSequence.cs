// --- File: Assets/Scripts/Events/Sequences/EnemyTakeTurnSequence.cs ---
using System.Collections;
using System.Linq;
using Assets.Helper;
using Assets.Scripts.Sequences;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// Executes one specific enemy's move/attack then ends the turn.
    public sealed class EnemyTakeTurnSequence : SequenceEvent
    {
        private readonly ActorInstance enemy;
        public EnemyTakeTurnSequence(ActorInstance enemy) { this.enemy = enemy; }

        public override IEnumerator ProcessRoutine()
        {
            // If this enemy died/despawned before acting, just end turn.
            if (enemy == null || !enemy.IsPlaying)
            {
                g.SequenceManager.Add(new EndTurnSequence());
                g.SequenceManager.Execute();
                yield break;
            }

            // Optional: small pacing
            yield return Wait.None();

            // Build that enemy's chain
            g.SequenceManager.Add(new EnemyMoveSequence(enemy));
            g.SequenceManager.Add(new EnemyPreAttackSequence(enemy));
            g.SequenceManager.Add(new EnemyAttackSequence(enemy));
            g.SequenceManager.Add(new EnemyPostAttackSequence(enemy));

            // Resolve deaths once after actions
            g.SequenceManager.Add(new DeathSequence());

            // Finish the enemy turn; this will call TurnManager.NextTurn()
            g.SequenceManager.Add(new EndTurnSequence());
            g.SequenceManager.Execute();
        }
    }
}
