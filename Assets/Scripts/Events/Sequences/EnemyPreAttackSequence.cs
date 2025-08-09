// --- File: Assets/Scripts/Events/Sequences/EnemyPreAttackSequence.cs ---
using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Performs any small anticipation before an enemy's attack.
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
            yield break;
        }
    }
}
