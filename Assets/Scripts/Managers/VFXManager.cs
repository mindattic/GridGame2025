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
    /// registers it, and returns the VFXInstance component. Returns null on invalid asset.
    /// </summary>
    private VFXInstance CreateInstance(VFXAsset asset, Vector3 position, Transform parentOverride = null)
    {
        if (asset == null || asset.Prefab == null)
            return null;

        GameObject go = Instantiate(asset.Prefab, position, Quaternion.identity);

        Transform parent = parentOverride != null ? parentOverride : (g.Board != null ? g.Board.transform : null);
        if (parent != null)
            go.transform.SetParent(parent, worldPositionStays: true);

        VFXInstance instance = go.GetComponent<VFXInstance>();
        if (instance == null)
            instance = go.AddComponent<VFXInstance>();

        string key = $"VFX_{asset.Name}_{Guid.NewGuid():N}";
        instance.name = key;

        if (!collection.ContainsKey(key))
            collection.Add(key, instance);

        return instance;
    }

    /// <summary>
    /// Fire-and-forget spawn at a world position. Optionally runs a routine after its own sequence.
    /// </summary>
    public void Spawn(VFXAsset asset, Vector3 position, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, position, null);
        if (instance == null)
            return;

        instance.Spawn(asset, position, routine);
    }

    /// <summary>
    /// Spawns a VFX and yields until its lifecycle finishes (including optional routine and duration).
    /// Use this when subsequent gameplay should wait for the impact to complete.
    /// </summary>
    public IEnumerator PlayRoutine(VFXAsset asset, Vector3 position, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, position, null);
        if (instance == null)
            yield break;

        yield return instance.SpawnRoutine(asset, position, routine);
    }

    /// <summary>
    /// Spawn an instance and return it, while also providing a yieldable routine to wait for completion.
    /// </summary>
    public (VFXInstance instance, IEnumerator routine) SpawnAndWait(VFXAsset asset, Vector3 position, Transform parentOverride = null, IEnumerator routine = null)
    {
        var inst = CreateInstance(asset, position, parentOverride);
        if (inst == null)
            return (null, null);
        return (inst, inst.SpawnRoutine(asset, position, routine));
    }

    /// <summary>
    /// Spawns a VFX, parents it to the provided transform so it follows movement,
    /// starts its lifecycle, and returns the VFXInstance.
    /// </summary>
    public VFXInstance SpawnInstance(VFXAsset asset, Vector3 position, Transform parent, IEnumerator routine = null)
    {
        var instance = CreateInstance(asset, position, parent);
        if (instance == null)
            return null;

        instance.Spawn(asset, position, routine);
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
            Destroy(instance.gameObject);
    }
}
