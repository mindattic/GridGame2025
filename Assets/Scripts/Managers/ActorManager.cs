using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

namespace Game.Manager
{
    public class ActorManager : MonoBehaviour
    {
        public float snapTheshold;

        private void Awake()
        {
            snapTheshold = g.TileSize * 0.125f * 1.01f;
        }

        public void CheckEnemyAP()
        {
            var notReadyEnemies = g.Actors.Enemies.Where(x => x.isPlaying && !x.hasMaxAP).ToList();
            notReadyEnemies.ForEach(x => x.actionBar.TriggerFill());
        }

        public void Clear()
        {
            if (g.Actors.All != null && g.Actors.All.Count > 0)
            {
                foreach (var actor in g.Actors.All)
                {
                    Destroy(actor.gameObject);
                }
                g.Actors.All.Clear();
            }
        }

    }
}