// Assets/Scripts/Instances/SynergyLineManager.cs
// Manages unique SynergyLine instances between two actors.

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Spawns and tracks SynergyLine instances between two ActorInstances.
/// Prevents duplicates, including reverse-order duplicates (A,B) vs (B,A).
/// </summary>
public class SynergyLineManager : MonoBehaviour
{
    [SerializeField] private GameObject synergyLinePrefab;

    // Active lines keyed by an order-independent pair key
    private readonly Dictionary<string, SynergyLineInstance> collection = new Dictionary<string, SynergyLineInstance>();

    private void Awake()
    {
        synergyLinePrefab = PrefabLibrary.Get("SynergyLinePrefab");
    }

    /// <summary>
    /// Spawns a synergy instance between the two actors if one does not already exist.
    /// Order-independent: (supporter, attacker) is treated the same as (attacker, supporter).
    /// </summary>
    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        string key = GenerateKey(supporter, attacker);
        if (key == null) return;
        if (collection.ContainsKey(key))
            return;

        var go = Instantiate(synergyLinePrefab, transform);
        go.name = key;
        var instance = go.GetComponent<SynergyLineInstance>();
        instance.Spawn(supporter, attacker);
        collection[key] = instance;
    }

    /// <summary>
    /// Removes the synergy instance between the two actors if it exists.
    /// Order-independent removal.
    /// </summary>
    public void Remove(ActorInstance a, ActorInstance b)
    {
        string key = GenerateKey(a, b);
        if (key == null) return;

        if (collection.TryGetValue(key, out var line))
        {
            if (line != null)
                Destroy(line.gameObject);

            collection.Remove(key);
        }
    }

    /// <summary>
    /// Checks if a synergy instance already exists between the two actors in any order.
    /// </summary>
    public bool Exists(ActorInstance a, ActorInstance b)
    {
        string key = GenerateKey(a, b);
        if (key == null) return false;

        return collection.ContainsKey(key);
    }

    /// <summary>
    /// Clears all active synergy lines managed by this instance.
    /// </summary>
    public void ClearAll()
    {
        foreach (var kv in collection)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        collection.Clear();
    }

    /// <summary>
    /// Builds an order-independent key from two actors based on reference identity.
    /// Ensures (A,B) and (B,A) produce the same key.
    /// </summary>
    private static string GenerateKey(ActorInstance a, ActorInstance b)
    {
        if (a == null || b == null) return null;

        string na = a.characterName;
        string nb = b.characterName;

        // Order independent by sorting the normalized names
        bool aFirst = string.CompareOrdinal(na, nb) <= 0;
        string first = aFirst ? na : nb;
        string second = aFirst ? nb : na;

        return $"Synergy_{first}{second}";
    }
}
