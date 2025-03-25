using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Manager
{
    public class ActorManager : MonoBehaviour
    {
        //Quick Reference Properties
        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
        protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;

        public void CheckEnemyAP()
        {
            var notReadyEnemies = enemies.Where(x => x.isPlaying && !x.hasMaxAP).ToList();
            notReadyEnemies.ForEach(x => x.actionBar.TriggerFill());
        }

        public void Clear()
        {
            if (actors != null && actors.Count > 0)
            {
                foreach (var actor in actors)
                {
                    Destroy(actor.gameObject);
                }
                actors.Clear();
            }
        }

    }
}