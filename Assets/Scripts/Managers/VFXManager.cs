using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;

    //Fields
    Dictionary<string, VFXInstance> visualEffects = new Dictionary<string, VFXInstance>();

    public void SpawnAsync(VisualEffectAsset resource, Vector3 position, TriggerEvent trigger = default)
    {
        if (trigger == default)
            trigger = new TriggerEvent();

        var prefab = Instantiate(resource.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{resource.Name}_{Guid.NewGuid():N}";
        visualEffects.Add(instance.name, instance);
        StartCoroutine(instance.Spawn(resource, position, trigger));
    }

    public IEnumerator Spawn(VisualEffectAsset resource, Vector3 position, TriggerEvent trigger = default)
    {
        if (trigger == default)
            trigger = new TriggerEvent();

        var prefab = Instantiate(resource.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{resource.Name}_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        visualEffects.Add(instance.name, instance);

        yield return instance.Spawn(resource, position, trigger);
    }


    public void Despawn(string name)
    {
        Destroy(visualEffects[name].gameObject);
        visualEffects.Remove(name);
    }

    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.VFX).ToList().ForEach(x => Destroy(x));
    }

}
