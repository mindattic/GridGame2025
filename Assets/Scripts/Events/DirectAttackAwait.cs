using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public class DirectAttackAwait : AwaitEvent
    {
        //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
        protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;

        //Constructor
        public DirectAttackAwait() { }

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
                var defendingHeroes = heroes.ToList().Where(x => x.isPlaying && x.IsAdjacentTo(enemy.location)).ToList();
                if (defendingHeroes.Count < 1)
                    continue;

                foreach (var hero in defendingHeroes)
                {
                    var direction = enemy.GetDirectionTo(hero);

                    var processAttack = new ProcessAttackAsync(enemy, hero);
                    yield return enemy.action.Bump(direction, processAttack);


                    //enemy.action.Bump(direction, evt);
                }

                enemy.actionBar.Reset();
            }

            turnManager.NextTurn();
        }

        //private IEnumerator ProcessAttack(ActorInstance attacker, ActorInstance opponent)
        //{
        //    var isHit = Formulas.IsHit(attacker, opponent);
        //    var isCriticalHit = Formulas.IsCriticalHit(attacker, opponent);
        //    var damage = Formulas.CalculateDamage(opponent, attacker);
        //    var result = new AttackResult()
        //    {
        //        Opponent = opponent,
        //        IsHit = isHit,
        //        IsCriticalHit = isCriticalHit,
        //        Damage = damage
        //    };

        //    attacker.TriggerAttack(result);

        //    //AsyncEvent death animations on any opponents killed in last result
        //    yield return DeathHelper.Process();
        //}

        //private IEnumerator ProcessDeaths()
        //{
        //    //Wait until all dying actor's HP bars are fully drained
        //    var dyingActors = actors.Where(x => x.isDying).ToList();
        //    if (dyingActors.Any())
        //        yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));
        //}

    }
}