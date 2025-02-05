using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class EnemySpawnAction : TurnAction
    {
        protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;

        public EnemySpawnAction() { }

        public override IEnumerator Execute()
        {
            var spawnableEnemies = enemies.ToList().FindAll(x => x.isSpawnable);
            foreach (var enemy in spawnableEnemies)
            {
                enemy.Spawn(Random.UnoccupiedLocation);
            }
            yield return Wait.UntilNextFrame();
        }
    }
}
