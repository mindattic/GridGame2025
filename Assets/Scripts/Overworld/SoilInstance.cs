using UnityEngine;

/// <summary>
/// Simple helper to vary grass seed and wind at runtime per instance.
/// Attach to the same GameObject as a SpriteRenderer that uses the SpriteGrassMask material.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class SoilInstance : MonoBehaviour
{
    [Header("Auto Seed")]
    [Tooltip("If true, assigns a deterministic seed from the object's position for variety.")]
    public bool autoSeed = true;

    [Tooltip("Extra seed offset for manual variation.")]
    public float seedOffset = 0f;

    [Header("Wind Jitter")]
    [Tooltip("Apply slight time scale variation for this instance.")]
    public bool windJitter = false;

    [Range(0.8f, 1.2f)]
    public float timeScale = 1f;

    MaterialPropertyBlock _props;
    SpriteRenderer _sr;

    // Shader property IDs
    static readonly int ID_Seed = Shader.PropertyToID("_Seed");

    void OnEnable()
    {
        if (_props == null) _props = new MaterialPropertyBlock();
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();
        Apply();
    }

    void Update()
    {
        if (windJitter)
        {
            // Slight drift to make instances feel unique
            timeScale = 1f + Mathf.Sin((float)Time.realtimeSinceStartup * 0.17f + transform.GetInstanceID() * 0.013f) * 0.05f;
            Shader.SetGlobalFloat("_TimeParameters", timeScale);
        }
        Apply();
    }

    void Apply()
    {
        if (_sr == null) return;

        _sr.GetPropertyBlock(_props);

        if (autoSeed)
        {
            // Stable seed from world position
            float s = Mathf.Sin(transform.position.x * 12.9898f + transform.position.y * 78.233f) * 43758.5453f;
            float seed = Mathf.Abs(s % 10000f) + seedOffset;
            _props.SetFloat(ID_Seed, seed);
        }

        _sr.SetPropertyBlock(_props);
    }
}
