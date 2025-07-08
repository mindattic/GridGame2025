using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;

    //Fields
    Dictionary<string, TrailInstance> trailEffects = new Dictionary<string, TrailInstance>();

    public void SpawnAsync(TrailEffectAsset resource, Vector3 position, TriggerEvent trigger = default)
    {
        if (trigger == default)
            trigger = new TriggerEvent();

        var prefab = Instantiate(resource.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<TrailInstance>();
        instance.name = $"Trail_{resource.Name}_{Guid.NewGuid():N}";
        trailEffects.Add(instance.name, instance);
        StartCoroutine(instance.Spawn(resource, position, trigger));
    }

    public IEnumerator Spawn(TrailEffectAsset resource, Vector3 position, TriggerEvent trigger = default)
    {
        if (trigger == default)
            trigger = new TriggerEvent();

        var prefab = Instantiate(resource.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<TrailInstance>();
        instance.name = $"Trail_{resource.Name}_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        trailEffects.Add(instance.name, instance);

        yield return instance.Spawn(resource, position, trigger);
    }


    public void Despawn(string name)
    {
        Destroy(trailEffects[name].gameObject);
        trailEffects.Remove(name);
    }

    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.Trail).ToList().ForEach(x => Destroy(x));
    }

}
