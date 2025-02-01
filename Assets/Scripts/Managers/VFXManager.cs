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

    public void TriggerSpawn(VisualEffect vfx, Vector3 position, Trigger trigger = default)
    {
        if (trigger == default)
            trigger = new Trigger();

        var prefab = Instantiate(vfx.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{vfx.Name}Attack{Guid.NewGuid():N}";
        visualEffects.Add(instance.name, instance);
        StartCoroutine(instance.Spawn(vfx, position, trigger));
    }

    public IEnumerator Spawn(VisualEffect vfx, Vector3 position, Trigger trigger = default)
    {
        if (trigger == default)
            trigger = new Trigger();

        var prefab = Instantiate(vfx.Prefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{vfx.Name}Initialize{Guid.NewGuid():N}";
        instance.parent = board.transform;
        visualEffects.Add(instance.name, instance);

        yield return instance.Spawn(vfx, position, trigger);
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
