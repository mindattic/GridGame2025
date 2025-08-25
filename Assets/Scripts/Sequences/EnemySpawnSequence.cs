using Assets.Helper;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class EnemySpawnSequence : SequenceEvent
    {
        public override IEnumerator ProcessRoutine()
        {
            // Show any enemies flagged as spawnable
            var spawnableEnemies = g.Actors.Enemies.Where(x => x.IsSpawnable).ToList();
            foreach (var enemy in spawnableEnemies)
            {
                var unoccupiedLocation = RNG.UnoccupiedLocation;
                if (unoccupiedLocation != null)
                    enemy.Spawn(unoccupiedLocation);
            }

            // Allow spawn visuals to apply
            yield return Wait.None();

            // Chain into the attacker start-of-turn and RUN it now
            g.SequenceManager.Add(new EnemyStartSequence());
            g.SequenceManager.Execute();
        }
    }
}
