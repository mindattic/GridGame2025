// --- File: Assets/Scripts/Events/Sequences/EnemyAttackSequence.cs ---
using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Executes the attack for a single attacker and then finishes.
    /// Assumes this attacker was ready when its chain began.
    /// </summary>
    public class EnemyAttackSequence : SequenceEvent
    {
        private readonly ActorInstance attacker;

        public EnemyAttackSequence(ActorInstance enemy)
        {
            this.attacker = enemy;
        }

        /// <summary>
        /// Plays the attacker attack flow:
        /// 1) Waits pre-attack intermission.
        /// 2) Finds adjacent defenders.
        /// 3) For each defender, computes attack result and bumps with damage.
        /// 4) Processes deaths and then resets the action bar.
        /// </summary>
        public override IEnumerator Execute()
        {
            // Safety: null or not in play should quietly skip.
            if (attacker == null || !attacker.isPlaying)
                yield break;

            // Pacing before the attack animation.
            yield return Wait.For(Intermission.Before.Enemy.Attack);

            // Gather adjacent defenders at attack time.
            var defendingHeroes = g.Actors.Heroes
                .Where(x => x.isPlaying && Geometry.IsAdjacentTo(x.location, attacker.location))
                .ToList();

            // Resolve one-by-one if anyone is adjacent.
            if (defendingHeroes.Count > 0)
            {
                foreach (var opponent in defendingHeroes)
                {
                    var attackResult = Formulas.CalculateAttackResult(attacker, opponent);

                    if (attackResult == null
                        || attackResult.Opponent == null
                        || attackResult.Opponent.isDying
                        || attackResult.Opponent.isDead)
                        continue;

                    // Run bump plus damage sequence using routine-based trigger.
                    var attackRoutine = AttackHelper.SingleAttackTrigger(attackResult);
                    yield return attacker.action.BumpTrigger(opponent, attackRoutine);

                    // Process deaths after each strike.
                    yield return DeathHelper.Process();
                }
            }

            // Reset this attacker's action bar after its attack to mark completion.
            attacker.actionBar.Reset();

            // Do not enqueue anything else here. Global queue continues.
        }
    }
}
