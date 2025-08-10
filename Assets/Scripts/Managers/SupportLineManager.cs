using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages lifecycle of SupportLineInstance objects keyed by (supporter, attacker).
/// Provides spawn, despawn, destroy, and bulk clear operations.
/// </summary>
public class SupportLineManager : MonoBehaviour
{
    // ------------------------------------------------------------
    // Fields
    // ------------------------------------------------------------

    [SerializeField] public GameObject supportLinePrefab; // Prefab reference resolved in Awake
    public Dictionary<(ActorInstance, ActorInstance), SupportLineInstance> supportLines =
        new Dictionary<(ActorInstance, ActorInstance), SupportLineInstance>();

    // ------------------------------------------------------------
    // Unity lifecycle
    // ------------------------------------------------------------

    /// <summary>
    /// Resolve the support line prefab from the central PrefabRepo.
    /// </summary>
    public void Awake()
    {
        supportLinePrefab = PrefabRepo.Prefabs["SupportLinePrefab"];
    }

    // ------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------

    /// <summary>
    /// Check if a support line already exists for the given pair.
    /// </summary>
    public bool Exists(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);
        return supportLines.ContainsKey(key);
    }

    /// <summary>
    /// Create and register a new support line instance for the given pair.
    /// Returns null if one already exists.
    /// </summary>
    public SupportLineInstance Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);

        if (Exists(supporter, attacker))
            return null;

        var prefab = Instantiate(supportLinePrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<SupportLineInstance>();

        supportLines.Add(key, instance);
        instance.Spawn(supporter, attacker);

        return instance;
    }

    /// <summary>
    /// Request a graceful despawn for the given pair and remove from registry.
    /// </summary>
    public void Despawn(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);

        if (supportLines.TryGetValue(key, out var instance))
        {
            instance.Despawn();
            supportLines.Remove(key);
        }
    }

    /// <summary>
    /// DespawnRoutine all active support lines and clear the registry.
    /// </summary>
    public void Clear()
    {
        foreach (var instance in supportLines.Values)
        {
            instance.Despawn();
        }

        supportLines.Clear();
    }

    /// <summary>
    /// Immediately destroy the instance for the given pair and remove from registry.
    /// </summary>
    public void Destroy(ActorInstance supporter, ActorInstance attacker)
    {
        var key = GetKey(supporter, attacker);

        if (supportLines.TryGetValue(key, out var instance))
        {
            instance.Destroy();
            supportLines.Remove(key);
        }
    }

    /// <summary>
    /// Build a tuple key for the pair.
    /// </summary>
    public (ActorInstance, ActorInstance) GetKey(ActorInstance supporter, ActorInstance attacker)
    {
        return (supporter, attacker);
    }
}
