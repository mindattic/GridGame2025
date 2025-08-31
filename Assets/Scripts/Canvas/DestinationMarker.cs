using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Helper;
using UnityEngine;

namespace Assets.Scripts.Canvas
{
    public class DestinationMarker : MonoBehaviour
    {
        // Config
        [SerializeField] private float arriveDistance = 0.1f;   // destroy when hero is this close
        [SerializeField] private bool destroyAtZero = true;      // remove when fully faded
        [SerializeField] private bool snapToWalkable = true;     // try to adjust to walkable center on spawn

        private Transform hero;               // world-space hero transform
        private SpriteRenderer sr;            // marker sprite
        private float initialDistance = 1f;   // distance at spawn used to normalize alpha

        private void Awake()
        {
            // Ensure parent under Map (world-space)
            var map = GameObject.Find(GameObjectHelper.Overworld.Map.Root);
            if (map != null && transform.parent != map.transform)
                transform.SetParent(map.transform, true);

            sr = GetComponent<SpriteRenderer>();
            if (hero == null)
            {
                var heroGo = GameObject.Find(GameObjectHelper.Overworld.Map.Hero);
                if (heroGo != null) hero = heroGo.transform;
            }

            if (snapToWalkable)
            {
                var terrainGo = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain);
                var provider = terrainGo != null ? terrainGo.GetComponent<MapTerrain>() : null;
                if (provider != null)
                {
                    // If not walkable, nudge slightly toward hero until walkable or small steps exhausted
                    var p = new Vector2(transform.position.x, transform.position.y);
                    if (!provider.IsWalkableLocal(p))
                    {
                        var target = hero != null ? (Vector2)hero.position : p;
                        var dir = (target - p).normalized;
                        const int iters = 16; const float step = 0.05f;
                        for (int i = 1; i <= iters; i++)
                        {
                            var test = p + dir * (step * i);
                            if (provider.IsWalkableLocal(test)) { transform.position = new Vector3(test.x, test.y, transform.position.z); break; }
                        }
                    }
                }
            }
        }

        private void Start()
        {
            if (hero != null)
            {
                initialDistance = Vector2.Distance(new Vector2(hero.position.x, hero.position.y), new Vector2(transform.position.x, transform.position.y));
                if (initialDistance < 0.001f) initialDistance = 1f; // avoid div by zero when spawning on hero
            }
        }

        private void Update()
        {
            if (hero == null)
            {
                var heroGo = GameObject.Find(GameObjectHelper.Overworld.Map.Hero);
                if (heroGo != null) hero = heroGo.transform;
            }

            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (sr == null || hero == null) return;

            float d = Vector2.Distance(new Vector2(hero.position.x, hero.position.y), new Vector2(transform.position.x, transform.position.y));

            // Fade alpha as hero approaches (1 at spawn distance, 0 when reached)
            float a = Mathf.Clamp01(d / initialDistance);
            var c = sr.color;
            c.a = a;
            sr.color = c;

            if (destroyAtZero && d <= arriveDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
