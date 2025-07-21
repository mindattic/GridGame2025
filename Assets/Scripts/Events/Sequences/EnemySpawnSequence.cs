using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using g = GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class EnemySpawnSequence : SequenceEvent
    {
 
        public override IEnumerator Execute()
        {
            var spawnableEnemies = g.Actors.Enemies.Where(x => x.isSpawnable).ToList();
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
