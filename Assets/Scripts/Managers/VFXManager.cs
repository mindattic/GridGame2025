using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class VfxManager : MonoBehaviour
{
    // Holds active VFX instances by unique name.
    private readonly Dictionary<string, VFXInstance> visualEffects = new Dictionary<string, VFXInstance>();

    /// <summary>
    /// Instantiates a VFX go at a world position, parents it to the board,
    /// registers it, and returns the VFXInstance component.
    /// </summary>
    private VFXInstance CreateInstance(VFXAsset asset, Vector3 position)
    {
        var go = Instantiate(asset.Prefab, position, Quaternion.identity);
        go.transform.SetParent(g.Board.transform, worldPositionStays: true);

        var instance = go.GetComponent<VFXInstance>();
        instance.name = $"VFX_{asset.Name}_{Guid.NewGuid():N}";
        visualEffects.Add(instance.name, instance);
        return instance;
    }

    /// <summary>
    /// FireAndForget-and-forget spawn. Optionally runs a routine routine on the instance after its own sequence.
    /// </summary>
    public void Spawn(VFXAsset asset, Vector3 worldPos, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, worldPos);
        instance.Spawn(asset, worldPos, routine);
    }

    /// <summary>
    /// Yieldable spawn. Optionally yields a routine routine on the instance after its own sequence.
    /// </summary>
    public IEnumerator SpawnRoutine(VFXAsset asset, Vector3 worldPos, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, worldPos);
        yield return instance.SpawnRoutine(asset, worldPos, routine);
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
