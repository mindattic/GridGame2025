using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class VFXManager : MonoBehaviour
{
    //Fields
    readonly Dictionary<string, VFXInstance> visualEffects = new Dictionary<string, VFXInstance>();

    private VFXInstance CreateInstance(VFXAsset asset, Vector3 position)
    {
        var prefab = Instantiate(asset.Prefab, position, Quaternion.identity);
        prefab.transform.SetParent(g.Board.transform, worldPositionStays: true);

        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{asset.Name}_{Guid.NewGuid():N}";
        visualEffects.Add(instance.name, instance);
        return instance;
    }

    // fire-and-forget
    public void SpawnAsync(VFXAsset asset, Vector3 worldPos, TriggerEvent trigger = null)
    {
        var instance = CreateInstance(asset, worldPos);
        instance.SpawnAsync(asset, worldPos, trigger);
    }

    // coroutine you can yield
    public IEnumerator Spawn(VFXAsset asset, Vector3 worldPos, TriggerEvent trigger = null)
    {
        var instance = CreateInstance(asset, worldPos);
        yield return instance.Spawn(asset, worldPos, trigger);
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
