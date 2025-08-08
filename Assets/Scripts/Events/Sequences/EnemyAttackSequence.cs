using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Executes the attack for a single enemy, then queues the next step.
    /// </summary>
    public class EnemyAttackSequence : SequenceEvent
    {
        private ActorInstance enemy;

        public EnemyAttackSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        public override IEnumerator Execute()
        {
            // Only proceed if it's the enemy's turn
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            // Ensure enemy is valid and in play
            if (enemy == null || !enemy.isPlaying)
                yield break;

            // Restore AP so the enemy can act
            //enemy.RestoreAP();

            // If no AP after restore, skip directly to next attacker or end turn
            if (!enemy.hasMaxAP)
            {
                var nextEnemy = g.Actors.Enemies.FirstOrDefault(x => x.isReady && x != enemy);
                if (nextEnemy != null)
                    g.SequenceManager.Add(new EnemyAttackSequence(nextEnemy));
                else
                    g.SequenceManager.Add(new EndTurnSequence());

                g.SequenceManager.TriggerExecute();
                yield break;
            }

            // Wait before attacking (pacing/animation)
            yield return Wait.For(Intermission.Before.Enemy.Attack);

            // Find all adjacent heroes to this enemy
            var defendingHeroes = g.Actors.Heroes
                .Where(x => x.isPlaying && x.IsAdjacentTo(enemy.location))
                .ToList();

            if (defendingHeroes.Count > 0)
            {
                foreach (var hero in defendingHeroes)
                {
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

                    var attack = new SingleAttackTrigger(attackResult);
                    yield return enemy.action.Bump(hero, attack);
                    yield return DeathHelper.Process();
                }
            }

            // Reset the action bar for this enemy after attacking
            enemy.actionBar.Reset();

            // Look for another ready enemy to attack
            var nextEnemyToAttack = g.Actors.Enemies
                .FirstOrDefault(x => x.isReady && x != enemy);

            if (nextEnemyToAttack != null)
            {
                // Queue the next attack
                g.SequenceManager.Add(new EnemyAttackSequence(nextEnemyToAttack));
            }
            else
            {
                // No more attackers → end turn
                g.SequenceManager.Add(new EndTurnSequence());
            }

            // Trigger execution so the queued actions run immediately
            g.SequenceManager.TriggerExecute();
        }
    }
}
