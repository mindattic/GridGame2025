using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class VFXManager : MonoBehaviour
{
    // Holds active VFX instances by unique name.
    private readonly Dictionary<string, VFXInstance> visualEffects = new Dictionary<string, VFXInstance>();

    /// <summary>
    /// Instantiates a VFX prefab at a world position, parents it to the board,
    /// registers it, and returns the VFXInstance component.
    /// </summary>
    private VFXInstance CreateInstance(VFXAsset asset, Vector3 position)
    {
        var prefab = Instantiate(asset.Prefab, position, Quaternion.identity);
        prefab.transform.SetParent(g.Board.transform, worldPositionStays: true);

        var instance = prefab.GetComponent<VFXInstance>();
        instance.name = $"VFX_{asset.Name}_{Guid.NewGuid():N}";
        visualEffects.Add(instance.name, instance);
        return instance;
    }

    /// <summary>
    /// Fire-and-forget spawn. Optionally runs a trigger routine on the instance after its own sequence.
    /// </summary>
    public void Spawn(VFXAsset asset, Vector3 worldPos, IEnumerator trigger = null)
    {
        var instance = CreateInstance(asset, worldPos);
        instance.Spawn(asset, worldPos, trigger);
    }

    /// <summary>
    /// Yieldable spawn. Optionally yields a trigger routine on the instance after its own sequence.
    /// </summary>
    public IEnumerator SpawnTrigger(VFXAsset asset, Vector3 worldPos, IEnumerator trigger = null)
    {
        var instance = CreateInstance(asset, worldPos);
        yield return instance.SpawnTrigger(asset, worldPos, trigger);
    }

    /// <summary>
    /// Destroys and unregisters a VFX instance by name.
    /// </summary>
    public void Despawn(string name)
    {
        if (!visualEffects.TryGetValue(name, out var inst) || inst == null)
            return;

        Destroy(inst.gameObject);
        visualEffects.Remove(name);
    }

    /// <summary>
    /// Destroys all VFX instances from the scene without using tags.
    /// </summary>
    public void Clear()
    {
        var instances = GameObject.FindObjectsByType<VFXInstance>(FindObjectsSortMode.None);
        foreach (var instance in instances)
        {
            Destroy(instance.gameObject);
        }
    }
}
