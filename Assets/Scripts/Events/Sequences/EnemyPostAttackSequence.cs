// --- File: Assets/Scripts/Events/Sequences/EnemyPostAttackSequence.cs ---
using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// EnemyPostAttackSequence
    /// Purpose:
    ///   Runs immediately after a single enemy finishes its attack.
    ///   Designed as a hook for post-attack effects or cleanup.
    ///
    /// Behavior:
    ///   1) Performs optional pacing to let visuals settle.
    ///   2) Applies any status effects that trigger after attacking.
    ///   3) Does not schedule other enemies or end the turn.
    ///
    /// Safety:
    ///   Skips quietly if the actor reference is null or no longer playing.
    /// </summary>
    public class EnemyPostAttackSequence : SequenceEvent
    {
        private readonly ActorInstance enemy; // Enemy that just attacked.

        /// <summary>
        /// Creates a new post-attack sequence for a specific enemy.
        /// </summary>
        public EnemyPostAttackSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        /// <summary>
        /// Executes any post-attack effects for the given enemy.
        /// </summary>
        public override IEnumerator Execute()
        {
            // Safety check: null or inactive enemy should skip this step.
            if (enemy == null || !enemy.isPlaying)
                yield break;

            // Optional: short pacing after the attack animation.
            yield return Wait.None();

            // Placeholder for future: apply poison, lifesteal, debuffs, or cleanup here.

            yield break;
        }
    }
}
