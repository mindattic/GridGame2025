using Game.Instances;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Behaviors
{
    public class AttackLineManager : MonoBehaviour
    {
        //Variables
        [SerializeField] public GameObject AttackLinePrefab;
        public Dictionary<(Vector2Int, Vector2Int), AttackLineInstance> attackLines = new Dictionary<(Vector2Int, Vector2Int), AttackLineInstance>();

        public bool Exists(ActorPair actorPair)
        {
            var key = GetKey(actorPair);
            return attackLines.ContainsKey(key);
        }

        public void Spawn(ActorPair actorPair)
        {
            var key = GetKey(actorPair);

            if (Exists(actorPair))
                return;

            var prefab = Instantiate(AttackLinePrefab, Vector2.zero, Quaternion.identity);
            var instance = prefab.GetComponent<AttackLineInstance>();
            attackLines[key] = instance;
            instance.Spawn(actorPair);
        }

        public void Despawn(ActorPair pair)
        {
            var key = GetKey(pair);
            if (attackLines.TryGetValue(key, out var instance))
            {
                instance.TriggerDespawn();
                attackLines.Remove(key);
            }
        }

        public void DespawnAll()
        {
            foreach (var instance in attackLines.Values)
            {
                instance.TriggerDespawn();
            }
            attackLines.Clear();
        }

        public void Clear()
        {
            foreach (var instance in attackLines.Values)
            {
                Destroy(instance.gameObject);
            }
            attackLines.Clear();
        }

        private (Vector2Int, Vector2Int) GetKey(ActorPair actorPair)
        {
            return (actorPair.startActor.location, actorPair.endActor.location);
        }
    }
}
