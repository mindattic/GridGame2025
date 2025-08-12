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
    private readonly Dictionary<string, SynergyLineInstance> activeLines = new Dictionary<string, SynergyLineInstance>();

    private void Awake()
    {
        synergyLinePrefab = PrefabRepo.Get("SynergyLinePrefab");
    }

    /// <summary>
    /// Spawns a synergy line between the two actors if one does not already exist.
    /// Order-independent: (supporter, attacker) is treated the same as (attacker, supporter).
    /// </summary>
    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        if (supporter == null || attacker == null || synergyLinePrefab == null)
            return;

        string key = GenerateKey(supporter, attacker);

        // Prevent duplicate spawn in either order
        if (activeLines.ContainsKey(key))
            return;

        var lineObj = Instantiate(synergyLinePrefab, transform);
        lineObj.name = key;

        var line = lineObj.GetComponent<SynergyLineInstance>();
        if (line == null)
        {
            Debug.LogError("SynergyLinePrefab is missing SynergyLineInstance.");
            Destroy(lineObj);
            return;
        }

        line.Spawn(supporter, attacker);
        activeLines[key] = line;
    }

    /// <summary>
    /// Removes the synergy line between the two actors if it exists.
    /// Order-independent removal.
    /// </summary>
    public void Remove(ActorInstance a, ActorInstance b)
    {
        string key = GenerateKey(a, b);
        if (activeLines.TryGetValue(key, out var line))
        {
            if (line != null)
                Destroy(line.gameObject);

            activeLines.Remove(key);
        }
    }

    /// <summary>
    /// Checks if a synergy line already exists between the two actors in any order.
    /// </summary>
    public bool Exists(ActorInstance a, ActorInstance b)
    {
        string key = GenerateKey(a, b);
        return activeLines.ContainsKey(key);
    }

    /// <summary>
    /// Clears all active synergy lines managed by this instance.
    /// </summary>
    public void ClearAll()
    {
        foreach (var kv in activeLines)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        activeLines.Clear();
    }

    /// <summary>
    /// Builds an order-independent key from two actors based on reference identity.
    /// Ensures (A,B) and (B,A) produce the same key.
    /// </summary>
    private static string GenerateKey(ActorInstance a, ActorInstance b)
    {
        int ha = a != null ? RuntimeHelpers.GetHashCode(a) : 0;
        int hb = b != null ? RuntimeHelpers.GetHashCode(b) : 0;

        // Order-independent by sorting the two values
        int first = ha <= hb ? ha : hb;
        int second = ha <= hb ? hb : ha;

        return $"Synergy_{first}_{second}";
    }
}
