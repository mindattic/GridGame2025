using Assets.Scripts.Models;
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

    public bool Exists(ActorInstance actor1, ActorInstance actor2)
    {
        var key = GetKey(actor1, actor2);
        return supportLines.ContainsKey(key);
    }

    public void Spawn(ActorInstance actor1, ActorInstance actor2)
    {
        var key = GetKey(actor1, actor2);

        if (Exists(actor1, actor2))
            return;

        var prefab = Instantiate(supportLinePrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<SupportLineInstance>();
        supportLines[key] = instance;
        instance.Spawn(actor1, actor2);
    }

    public void Despawn(ActorInstance actor1, ActorInstance actor2)
    {
        var key = GetKey(actor1, actor2);
        if (supportLines.TryGetValue(key, out var instance))
        {
            instance.TriggerDespawn();
            supportLines.Remove(key);
        }
    }

    public void DespawnAll()
    {
        foreach (var instance in supportLines.Values)
        {
            instance.TriggerDespawn();
        }
        supportLines.Clear();
    }

    public void Destroy(ActorInstance actor1, ActorInstance actor2)
    {
        var key = GetKey(actor1, actor2);
        if (supportLines.TryGetValue(key, out var instance))
        {
            instance.Destroy();
            supportLines.Remove(key);
        }
    }

    private (Vector2Int, Vector2Int) GetKey(ActorInstance actor1, ActorInstance actor2)
    {
        var location1 = actor1.location;
        var location2 = actor2.location;

        return location1.x < location2.x || (location1.x == location2.x && location1.y < location2.y)
            ? (location1, location2)
            : (location2, location1);
    }

}
