using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class TrailManager : MonoBehaviour
{
    // Holds active trail instances by unique name.
    private readonly Dictionary<string, TrailInstance> trailEffects = new Dictionary<string, TrailInstance>();

    /// <summary>
    /// Fire-and-forget spawn of a trail effect at a world position.
    /// Optionally runs a trigger routine after the trail's own sequence.
    /// </summary>
    public void Spawn(TrailEffectAsset resource, Vector3 position, IEnumerator trigger = null)
    {
        var prefab = Instantiate(resource.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<TrailInstance>();
        instance.name = $"Trail_{resource.Name}_{Guid.NewGuid():N}";
        trailEffects.Add(instance.name, instance);

        StartCoroutine(instance.Spawn(resource, position, trigger));
    }

    /// <summary>
    /// Yieldable spawn of a trail effect at a world position.
    /// Optionally yields a trigger routine after the trail's own sequence.
    /// </summary>
    public IEnumerator YieldSpawn(TrailEffectAsset resource, Vector3 position, IEnumerator trigger = null)
    {
        var prefab = Instantiate(resource.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<TrailInstance>();
        instance.name = $"Trail_{resource.Name}_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        trailEffects.Add(instance.name, instance);

        yield return instance.Spawn(resource, position, trigger);
    }

    /// <summary>
    /// Destroys and unregisters a trail instance by name.
    /// </summary>
    public void Despawn(string name)
    {
        if (!trailEffects.TryGetValue(name, out var inst) || inst == null)
            return;

        Destroy(inst.gameObject);
        trailEffects.Remove(name);
    }

    /// <summary>
    /// Destroys all trail instances from the scene without using tags.
    /// </summary>
    public void Clear()
    {
        var instances = GameObject.FindObjectsByType<TrailInstance>(FindObjectsSortMode.None);
        foreach (var instance in instances)
        {
            Destroy(instance.gameObject);
        }
    }
}
