using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class EnemySpawnSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Spawn any enemies flagged as spawnable
            var spawnableEnemies = g.Actors.Enemies.Where(x => x.isSpawnable).ToList();
            foreach (var enemy in spawnableEnemies)
            {
                var unoccupiedLocation = Random.UnoccupiedLocation;
                if (unoccupiedLocation != null)
                    enemy.Spawn(unoccupiedLocation);
            }

            // Allow spawn visuals to apply
            yield return Wait.UntilNextFrame();

            // Chain into the enemy start-of-turn and RUN it now
            g.SequenceManager.Add(new EnemyStartSequence());
            g.SequenceManager.TriggerExecute();
        }
    }
}
