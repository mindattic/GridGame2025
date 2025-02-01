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

        public bool Exists(CombatPair pair)
        {
            var key = GetKey(pair);
            return attackLines.ContainsKey(key);
        }

        public void Spawn(CombatPair pair)
        {
            var key = GetKey(pair);

            if (Exists(pair))
                return;

            var prefab = Instantiate(AttackLinePrefab, Vector2.zero, Quaternion.identity);
            var instance = prefab.GetComponent<AttackLineInstance>();
            attackLines[key] = instance;
            instance.Spawn(pair);
        }

        public void Despawn(CombatPair pair)
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

        private (Vector2Int, Vector2Int) GetKey(CombatPair pair)
        {
            return (pair.startActor.location, pair.endActor.location);
        }
    }
}
