using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Manager
{
    public class ActorManager : MonoBehaviour
    {
        //External properties
        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected IQueryable<ActorInstance> players => GameManager.instance.players;
        protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;

        public void CheckEnemyAP()
        {
            var notReadyEnemies = enemies.Where(x => x.isPlaying && !x.hasMaxAP).ToList();
            notReadyEnemies.ForEach(x => x.actionBar.TriggerFill());
        }

        public void DisabledAt(Vector2Int location)
        {
            var actor = actors.Where(x => x.location == location).FirstOrDefault();
            actor?.gameObject.SetActive(false);
        }
        public void Clear()
        {
            GameObject.FindGameObjectsWithTag(Tag.Actor).ToList().ForEach(x => Destroy(x));
            actors.Clear();
        }

    }
}