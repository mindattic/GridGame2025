using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public class EnemySpawnAwait : AwaitEvent
    {
        //Quick Reference Properties
        protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;

        public EnemySpawnAwait() { }

        public override IEnumerator Execute()
        {
            var spawnableEnemies = enemies.Where(x => x.isSpawnable).ToList();
            foreach (var enemy in spawnableEnemies)
            {
                enemy.Spawn(Random.UnoccupiedLocation);
            }
            yield return Wait.UntilNextFrame();
        }
    }
}
