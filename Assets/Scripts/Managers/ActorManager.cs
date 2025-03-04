using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Manager
{
    public class ActorManager : MonoBehaviour
    {
        //Quick Reference Properties
        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected IQueryable<ActorInstance> players => GameManager.instance.players;
        protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;

        public void CheckEnemyAP()
        {
            var notReadyEnemies = enemies.Where(x => x.isPlaying && !x.hasMaxAP).ToList();
            notReadyEnemies.ForEach(x => x.actionBar.TriggerFill());
        }

        public void Clear()
        {
            GameObject.FindGameObjectsWithTag(Tag.Actor).ToList().ForEach(x => Destroy(x));
            actors.Clear();
        }

    }
}