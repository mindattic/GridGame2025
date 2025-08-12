using UnityEngine;

/// <summary>
/// Manages the spawning of SynergyLine prefabs between two ActorInstances.
/// </summary>
public class SynergyLineManager : MonoBehaviour
{
    [SerializeField] private GameObject synergyLinePrefab;

    private void Awake()
    {
        if (synergyLinePrefab == null)
        {
            // Pull from repo if not wired in the Inspector
            if (!PrefabRepo.Prefabs.TryGetValue("SynergyLinePrefab", out synergyLinePrefab) || synergyLinePrefab == null)
                Debug.LogError("SynergyLinePrefab not found in PrefabRepo or is null.");
        }
    }

    /// <summary>
    /// Spawns a synergy line between two actors.
    /// </summary>
    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        var lineObj = Instantiate(synergyLinePrefab, transform);
        var line = lineObj.GetComponent<SynergyLineInstance>();
        if (line == null)
        {
            Debug.LogError("SynergyLinePrefab is missing SynergyLineInstance.");
            Destroy(lineObj);
            return;
        }
        line.Spawn(supporter, attacker);
    }
}
