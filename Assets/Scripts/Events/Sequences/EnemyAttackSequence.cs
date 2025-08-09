// --- File: Assets/Scripts/Events/Sequences/EnemyAttackSequence.cs ---
using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Executes the attack for a single enemy and then finishes.
    /// Assumes this enemy was "ready" when its chain began.
    /// </summary>
    public class EnemyAttackSequence : SequenceEvent
    {
        private readonly ActorInstance enemy;

        public EnemyAttackSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        public override IEnumerator Execute()
        {
            // Safety: null or not in play should quietly skip.
            if (enemy == null || !enemy.isPlaying)
                yield break;

            // Pacing before the attack animation.
            yield return Wait.For(Intermission.Before.Enemy.Attack);

            // Gather adjacent defenders at attack time.
            var defendingHeroes = g.Actors.Heroes
                .Where(x => x.isPlaying && Geometry.IsAdjacentTo(x.location, enemy.location))
                .ToList();

            // Resolve one-by-one if anyone is adjacent.
            if (defendingHeroes.Count > 0)
            {
                foreach (var hero in defendingHeroes)
                {
                    // Calculate combat results.
                    var isHit = Formulas.IsHit(enemy, hero);
                    var isCriticalHit = Formulas.IsCriticalHit(enemy, hero);
                    var damage = Formulas.CalculateDamage(enemy, hero);

                    var attackResult = new AttackResult
                    {
                        Attacker = enemy,
                        Opponent = hero,
                        IsHit = isHit,
                        IsCriticalHit = isCriticalHit,
                        Damage = damage
                    };

                    // Run bump + damage sequence.
                    var attack = new SingleAttackTrigger(attackResult);
                    yield return enemy.action.Bump(hero, attack);

                    // Process deaths after each strike.
                    yield return DeathHelper.Process();
                }
            }

            // Reset this enemy's action bar after its attack to mark completion.
            enemy.actionBar.Reset();

            // Do not enqueue anything else here. Global queue continues.
        }
    }
}
