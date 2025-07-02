using System.Collections.Generic;
using UnityEngine;

public class SupportLineManager : MonoBehaviour
{
    //Fields
    [SerializeField] public GameObject supportLinePrefab;
    public Dictionary<(ActorInstance, ActorInstance), SupportLineInstance> supportLines = new Dictionary<(ActorInstance, ActorInstance), SupportLineInstance>();


    public void Awake()
    {
        supportLinePrefab = PrefabRepo.Prefabs["SupportLinePrefab"];
    }

    public bool Exists(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);
        return supportLines.ContainsKey(key);
    }

    public SupportLineInstance Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);

        if (Exists(supporter, attacker))
            return null;

        var prefab = Instantiate(supportLinePrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<SupportLineInstance>();
        supportLines.Add(key, instance);
        instance.Spawn(supporter, attacker);
        return instance;
    }

    public void Despawn(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);
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
            instance.TriggerDespawn();
        }
        supportLines.Clear();
    }

    public void Destroy(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);
        if (supportLines.TryGetValue(key, out var instance))
        {
            instance.Destroy();
            supportLines.Remove(key);
        }
    }

    public (ActorInstance, ActorInstance) GetKey(ActorInstance supporter, ActorInstance attacker)
    {
        return (supporter, attacker);
    }


}
