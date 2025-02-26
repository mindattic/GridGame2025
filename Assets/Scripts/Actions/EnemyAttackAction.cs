
using Assets.Scripts.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class EnemyAttackAction : PhaseAction
    {
        private TurnManager turnManager => GameManager.instance.turnManager;
        private IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
        private IQueryable<ActorInstance> players => GameManager.instance.players;


        public EnemyAttackAction() { }

        public override IEnumerator Execute()
        {
            if (!turnManager.isEnemyTurn || turnManager.currentPhase != TurnPhase.Attack) yield break;

            var readyEnemies = enemies.ToList().Where(x => x.isPlaying && x.hasMaxAP).ToList();
            if (readyEnemies.Count < 1) yield break;

            yield return Wait.For(Intermission.Before.Enemy.Attack);

            foreach (var enemy in readyEnemies)
            {
                var defendingPlayers = players.ToList().Where(x => x.isPlaying && x.IsAdjacentTo(enemy.location)).ToList();
                if (defendingPlayers.Count < 1) continue;

                foreach (var player in defendingPlayers)
                {
                    IEnumerator Attack()
                    {
                        var isHit = Formulas.IsHit(enemy, player);
                        var isCriticalHit = false;
                        var damage = Formulas.CalculateDamage(enemy, player);
                        var attack = new AttackResult()
                        {
                            Opponent = player,
                            IsHit = isHit,
                            IsCriticalHit = isCriticalHit,
                            Damage = damage
                        };
                        yield return enemy.Attack(attack);

                        if (player.isDying)
                        {
                            yield return HandlePlayerDeath(player);
                        }
                    }

                    var direction = enemy.GetDirectionTo(player);
                    var trigger = new Trigger(Attack());
                    yield return enemy.action.Bump(direction, trigger);
                }

                enemy.actionBar.Reset();
            }

            turnManager.NextTurn();
        }

        private IEnumerator HandlePlayerDeath(ActorInstance player)
        {
            player.TriggerDie();
            yield return null;
        }
    }
}