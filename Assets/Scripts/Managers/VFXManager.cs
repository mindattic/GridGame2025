using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using game = GameManagerHelper;

public class VFXManager : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;

    //Fields
    Dictionary<string, VFXInstance> visualEffects = new Dictionary<string, VFXInstance>();

    public void SpawnAsync(VFXAsset resource, Vector3 position, TriggerEvent trigger = null)
    {
        var prefab = Instantiate(resource.Prefab, position, Quaternion.identity);
        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{resource.Name}_{Guid.NewGuid():N}";
        visualEffects.Add(instance.name, instance);
        instance.SpawnAsync(resource, position, trigger);
    }

    public IEnumerator Spawn(VFXAsset resource, Vector3 position, TriggerEvent trigger = null)
    {
        var prefab = Instantiate(resource.Prefab, position, Quaternion.identity);
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
