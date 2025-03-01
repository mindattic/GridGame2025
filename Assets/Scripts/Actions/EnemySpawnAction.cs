using System.Collections;
using System.Linq;

namespace Assets.Scripts.Models
{
    public class EnemySpawnAction : PhaseAction
    {
       //Quick Reference Properties
        protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;

        public EnemySpawnAction() { }

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
