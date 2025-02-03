using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupportLineManager : MonoBehaviour
{
    //Variables
    [SerializeField] public GameObject supportLinePrefab;
    public Dictionary<(Vector2Int, Vector2Int), SupportLineInstance> supportLines = new Dictionary<(Vector2Int, Vector2Int), SupportLineInstance>();

    public bool Exists(ActorPair pair)
    {
        var key = GetKey(pair);
        return supportLines.ContainsKey(key);
    }

    public void Spawn(ActorPair pair)
    {
        var key = GetKey(pair);

        if (Exists(pair))
            return;

        var prefab = Instantiate(supportLinePrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<SupportLineInstance>();
        supportLines[key] = instance;
        instance.Spawn(pair);
    }

    public void Despawn(ActorPair pair)
    {
        var key = GetKey(pair);
        if (supportLines.TryGetValue(key, out var instance))
        {
            instance.TriggerDespawn();
            supportLines.Remove(key);
        }
    }

    public void Clear()
    {
        foreach (var instance in supportLines.Values)
        {
            Destroy(instance.gameObject);
        }
        supportLines.Clear();
    }

    private (Vector2Int, Vector2Int) GetKey(ActorPair pair)
    {
        return (pair.actor1.location, pair.actor2.location);
    }
}
