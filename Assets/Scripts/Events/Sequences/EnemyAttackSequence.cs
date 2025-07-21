using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using game = GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Executes the attack for a single enemy in the attack phase.
    /// </summary>
    public class EnemyAttackSequence : SequenceEvent
    {
        #region Game Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
        #endregion

        private ActorInstance enemy;

        public EnemyAttackSequence(ActorInstance enemy)
        {
            this.enemy = enemy;
        }

        public override IEnumerator Execute()
        {
            // Only proceed if it's the enemy's turn and attack phase.
            if (!turnManager.isEnemyTurn || turnManager.currentPhase != TurnPhase.Attack)
                yield break;

            // Only attack if this enemy is valid and has max AP.
            if (enemy == null || !enemy.isPlaying || !enemy.hasMaxAP)
                yield break;

            yield return Wait.For(Intermission.Before.Enemy.Attack);

            // Find all adjacent heroes to this enemy
            var defendingHeroes = heroes
                .Where(x => x.isPlaying && x.IsAdjacentTo(enemy.location))
                .ToList();

            if (defendingHeroes.Count < 1)
                yield break;

            foreach (var hero in defendingHeroes)
            {
                var direction = enemy.GetDirectionTo(hero);


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
                yield return enemy.animate.Bump(direction, attack);
                yield return DeathHelper.Process();

            }

            // Reset the action bar for this enemy after attacking.
            enemy.actionBar.Reset();

        }
    }
}
