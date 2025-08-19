using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class VfxManager : MonoBehaviour
{
    // Holds active VFX instances by unique name.
    private readonly Dictionary<string, VFXInstance> collection = new Dictionary<string, VFXInstance>();

    /// <summary>
    /// Instantiates a VFX at a world position, parents it to the board or an override,
    /// registers it, and returns the VFXInstance component.
    /// </summary>
    private VFXInstance CreateInstance(VFXAsset asset, Vector3 position, Transform parentOverride = null)
    {
        var go = Instantiate(asset.Prefab, position, Quaternion.identity);

        var parent = parentOverride != null ? parentOverride : g.Board.transform;
        go.transform.SetParent(parent, worldPositionStays: true);

        var instance = go.GetComponent<VFXInstance>();
        instance.name = $"VFX_{asset.Name}_{Guid.NewGuid():N}";
        collection.Add(instance.name, instance);
        return instance;
    }

    /// <summary>
    /// Fire-and-forget spawn. Optionally runs a routine after its own sequence.
    /// </summary>
    public void Spawn(VFXAsset asset, Vector3 worldPos, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, worldPos, null);
        instance.Spawn(asset, worldPos, routine);
    }

    /// <summary>
    /// Overload that supports an explicit parent for following effects.
    /// </summary>
    public void Spawn(VFXAsset asset, Vector3 worldPos, Transform parentOverride, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, worldPos, parentOverride);
        instance.Spawn(asset, worldPos, routine);
    }

    /// <summary>
    /// Yieldable spawn. Optionally yields a routine after its own sequence.
    /// </summary>
    public IEnumerator SpawnRoutine(VFXAsset asset, Vector3 worldPos, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, worldPos, null);
        yield return instance.SpawnRoutine(asset, worldPos, routine);
    }

    /// <summary>
    /// Yieldable spawn with explicit parent for following effects.
    /// </summary>
    public IEnumerator SpawnRoutine(VFXAsset asset, Vector3 worldPos, Transform parentOverride, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, worldPos, parentOverride);
        yield return instance.SpawnRoutine(asset, worldPos, routine);
    }


    // Spawns a VFX prefab, parents it to the provided transform (so it follows movement),
    // registers it in the collection, and starts its lifecycle coroutine.
    // If no parent is provided, it falls back to an "Effects" root if present, otherwise leaves unparented.
    public VFXInstance SpawnReturnInstance(VFXAsset asset, Vector3 position, Transform parent, IEnumerator routine = null)
    {
        if (asset == null || asset.Prefab == null)
            return null;

        // Instantiate first so world position is correct before parenting
        GameObject go = Instantiate(asset.Prefab, position, Quaternion.identity);

        go.transform.SetParent(parent, worldPositionStays: true);

        // Get instance and register with a unique key
        VFXInstance instance = go.GetComponent<VFXInstance>();
        if (instance == null)
            instance = go.AddComponent<VFXInstance>();

        string key = $"Vfx_{asset.Name}_{Guid.NewGuid():N}";
        instance.name = key;

        // Begin the effect at the requested world position
        StartCoroutine(instance.SpawnRoutine(asset, position, routine));

        return instance;
    }





    /// <summary>
    /// Destroys and unregisters a VFX instance by name.
    /// </summary>
    public void Despawn(string name)
    {
        if (!collection.TryGetValue(name, out var inst) || inst == null)
            return;

        Destroy(inst.gameObject);
        collection.Remove(name);
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
