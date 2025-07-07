using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public class EnemySpawnSequence : SequenceEvent
    {
        #region Game Properies
        protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
        #endregion

        public override IEnumerator Execute()
        {
            var spawnableEnemies = enemies.Where(x => x.isSpawnable).ToList();
            foreach (var enemy in spawnableEnemies)
            {
                var unoccupiedLocation = Random.UnoccupiedLocation;
                if (unoccupiedLocation != null)
                    enemy.Spawn(unoccupiedLocation);
            }
            yield return Wait.UntilNextFrame();
        }
    }
}
