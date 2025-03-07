
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class EnemyAttackAction : PhaseAction
    {
        //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
        protected IQueryable<ActorInstance> players => GameManager.instance.players;

        //Constructor
        public EnemyAttackAction() { }

        public override IEnumerator Execute()
        {
            if (!turnManager.isEnemyTurn || turnManager.currentPhase != TurnPhase.Attack)
                yield break;

            var attackingEnemies = enemies.ToList().Where(x => x.isPlaying && x.hasMaxAP).ToList();
            if (attackingEnemies.Count < 1)
                yield break;

            yield return Wait.For(Intermission.Before.Enemy.Attack);

            foreach (var enemy in attackingEnemies)
            {
                var defendingPlayers = players.ToList().Where(x => x.isPlaying && x.IsAdjacentTo(enemy.location)).ToList();
                if (defendingPlayers.Count < 1)
                    continue;

                foreach (var player in defendingPlayers)
                {
                    var direction = enemy.GetDirectionTo(player);
                    var trigger = new Trigger(ProcessAttack(enemy, player));
                    yield return enemy.action.Bump(direction, trigger);
                }

                enemy.actionBar.Reset();
            }

            turnManager.NextTurn();
        }

        private IEnumerator ProcessAttack(ActorInstance attacker, ActorInstance opponent)
        {
            attacker.sortingOrder = SortingOrder.Attacker;
            opponent.sortingOrder = SortingOrder.Opponent;

            var isHit = Formulas.IsHit(attacker, opponent);
            var isCriticalHit = Formulas.IsCriticalHit(attacker, opponent);
            var damage = Formulas.CalculateDamage(opponent, attacker);
            var attack = new AttackResult()
            {
                Opponent = attacker,
                IsHit = isHit,
                IsCriticalHit = isCriticalHit,
                Damage = damage
            };
            yield return opponent.Attack(attack);

            //Trigger death animations on any opponents killed in last attack
            yield return ProcessDeaths();
        }

        private IEnumerator ProcessDeaths()
        {
            //Wait until all dying actor's HP bars are fully drained
            var dyingActors = actors.Where(x => x.isDying).ToList();
            if (dyingActors.Any())
                yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));
        }

    }
}