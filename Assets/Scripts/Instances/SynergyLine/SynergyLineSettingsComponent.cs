// Assets/Scripts/Instances/SynergyLineSettingsComponent.cs
// MonoBehaviour host for SynergyLineSettings. Attach to any GameObject and tweak in the Inspector.
// While playing, edits auto respawn lines by calling DebugManager.SpawnSynergyLines().

using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Live settings host you can attach to a scene object. Supports live tweak-and-see.
/// </summary>
[DisallowMultipleComponent]
public class SynergyLineSettingsComponent : MonoBehaviour
{
    [SerializeField] private SynergyLineSettings settings = SynergyLineSettings.Defaults();

    [Tooltip("If true, changing values during Play Mode will despawn and respawn synergy lines automatically.")]
    [SerializeField] private bool autoRespawnDuringPlay = true;

    // Change tracking
    private int lastHash;

    /// <summary>
    /// Provides a snapshot of current settings for runtime code.
    /// </summary>
    public SynergyLineSettings Snapshot() => settings != null ? settings.Clone() : SynergyLineSettings.Defaults();

    /// <summary>
    /// Optional global access to the first component in the scene.
    /// </summary>
    public static SynergyLineSettings TryGetCurrent()
    {
        var comp = FindFirstObjectByType<SynergyLineSettingsComponent>();
        return comp != null ? comp.Snapshot() : SynergyLineSettings.Defaults();
    }

    /// <summary>
    /// Reset this component's settings to defaults.
    /// </summary>
    [ContextMenu("Reset To Defaults")]
    public void ResetToDefaults()
    {
        settings = SynergyLineSettings.Defaults();
        lastHash = settings.ComputeHash();
#if UNITY_EDITOR
        if (Application.isPlaying && autoRespawnDuringPlay) RespawnNow();
#endif
    }

    /// <summary>
    /// Force a despawn + respawn using DebugManager.SpawnSynergyLines().
    /// </summary>
    [ContextMenu("Respawn Now")]
    public void RespawnNow()
    {
        // Despawn any existing line instances
        var lines = FindObjectsByType<SynergyLineInstance>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < lines.Length; i++)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(lines[i].gameObject);
            else
#endif
                Destroy(lines[i].gameObject);
        }

        // Call DebugManager.SpawnSynergyLines() if available
        try
        {
            g.DebugManager.SpawnSynergyLines();
        }
        catch
        {
            var t = System.Type.GetType("DebugManager");
            if (t != null)
            {
                var m = t.GetMethod("SpawnSynergyLines", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                var instProp = t.GetProperty("instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                object inst = m != null && m.IsStatic ? null : (instProp != null ? instProp.GetValue(null) : null);
                if (m != null) m.Invoke(inst, null);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Called when values change in the Inspector.
    /// During Play Mode, triggers respawn if values changed.
    /// </summary>
    private void OnValidate()
    {
        if (settings == null) settings = SynergyLineSettings.Defaults();
        int h = settings.ComputeHash();
        if (h != lastHash)
        {
            lastHash = h;
            if (Application.isPlaying && autoRespawnDuringPlay)
            {
                // Delay one frame to avoid Validate-while-spawning conflicts
                StartCoroutine(RespawnNextFrame());
            }
        }
    }

    private System.Collections.IEnumerator RespawnNextFrame()
    {
        yield return null;
        RespawnNow();
    }
#endif
}
