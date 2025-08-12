// Assets/Scripts/Instances/SynergyLineInstance.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Wispy multi-strand line between two actors.
/// Combines supporter + attacker stats, spawns strands, animates until Despawn is called.
/// Colors are animated rainbow hues applied per strand and pushed to the material.
/// </summary>
public class SynergyLineInstance : MonoBehaviour
{
    // Prefab cache
    private GameObject synergyLineSegmentPrefab;

    // Group
    private int waveformCount = 7;          // one strand per stat including Luck
    private float baseRadius = 0.07f;
    private float baseWidth = 0.005f;
    private float frequency = 2.2f;

    // Noise
    private float noiseAmplitude = 0.015f;
    private float noiseScale = 2.5f;
    private float noiseSpeed = 0.18f;

    // Shape
    private AnimationCurve radiusOverT = AnimationCurve.EaseInOut(0, 0.2f, 1, 1.0f);

    // Fade
    private float fadeInTime = 0.2f;
    private float fadeOutTime = 0.3f;

    // Sorting
    private int orderOffsetPerWave = 1;
    private int extraFrontBias = -2;

    // Rainbow animation controls
    private float rainbowHueSpeed = 0.06f;       // hue change speed over time
    private float rainbowHuePhase = 0.12f;       // per-strand hue offset
    private float rainbowSatMin = 0.75f;         // saturation lower bound
    private float rainbowSatMax = 1.00f;         // saturation upper bound
    private float rainbowValueBase = 1.00f;      // brightness base
    private float rainbowValuePulseAmp = 0.08f;  // brightness pulse amount
    private float rainbowValuePulseSpeed = 0.7f; // brightness pulse speed

    // Runtime
    private readonly List<SynergyLineSegment> segments = new List<SynergyLineSegment>(8);
    private Transform a;
    private Transform b;
    private Renderer aRenderer;
    private Renderer bRenderer;
    private SortingGroup aGroup;
    private SortingGroup bGroup;

    private bool playing;
    private bool despawnRequested;
    private Coroutine loopCo;

    private void Awake()
    {
        synergyLineSegmentPrefab = PrefabRepo.Get("SynergyLineSegmentPrefab");
    }

    /// <summary>
    /// Entry point. Combines stats, configures segments, begins loop.
    /// </summary>
    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        a = supporter.transform;
        b = attacker.transform;

        Vector7 weights = new Vector7(
            supporter.stats.Strength + attacker.stats.Strength,
            supporter.stats.Vitality + attacker.stats.Vitality,
            supporter.stats.Agility + attacker.stats.Agility,
            supporter.stats.Stamina + attacker.stats.Stamina,
            supporter.stats.Intelligence + attacker.stats.Intelligence,
            supporter.stats.Wisdom + attacker.stats.Wisdom,
            supporter.stats.Luck + attacker.stats.Luck
        );

        Configure(a, b, weights);
        StartLoop();
    }

    /// <summary>
    /// Request fade out and cleanup.
    /// </summary>
    public void Despawn(float fadeSeconds = -1f)
    {
        if (fadeSeconds >= 0f) fadeOutTime = fadeSeconds;
        despawnRequested = true;
    }

    /// <summary>
    /// Configure endpoints and map weights to strands.
    /// </summary>
    public void Configure(Transform start, Transform end, Vector7 weights)
    {
        a = start;
        b = end;

        aGroup = a != null ? a.GetComponentInParent<SortingGroup>() : null;
        bGroup = b != null ? b.GetComponentInParent<SortingGroup>() : null;

        if (aGroup == null) aRenderer = a != null ? a.GetComponentInParent<SpriteRenderer>() : null;
        if (bGroup == null) bRenderer = b != null ? b.GetComponentInParent<SpriteRenderer>() : null;

        EnsureSegments(waveformCount);

        // STR, VIT, AGI, STA, INT, WIS, LCK
        float[] w = new float[7];
        w[0] = Mathf.Max(0f, weights.str);
        w[1] = Mathf.Max(0f, weights.vit);
        w[2] = Mathf.Max(0f, weights.agi);
        w[3] = Mathf.Max(0f, weights.sta);
        w[4] = Mathf.Max(0f, weights.intel);
        w[5] = Mathf.Max(0f, weights.wis);
        w[6] = Mathf.Max(0f, weights.lck);

        float max = 0.0001f;
        for (int i = 0; i < w.Length; i++) max = Mathf.Max(max, w[i]);

        float phaseStep = (Mathf.PI * 2f) / Mathf.Max(1, waveformCount);

        string layerName;
        int baseOrder;
        ResolveSortingBelowActors(out layerName, out baseOrder);

        for (int i = 0; i < waveformCount; i++)
        {
            float wNorm = w[i % 7] / max;

            float widthAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * baseWidth;
            float radiusAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * baseRadius;
            float phase = phaseStep * i;

            var seg = segments[i];
            seg.Configure(
                a, b,
                widthAbs,
                radiusAbs,
                phase,
                frequency,
                noiseAmplitude,
                noiseScale,
                noiseSpeed,
                radiusOverT,
                layerName,
                baseOrder + extraFrontBias + (i * orderOffsetPerWave),
                i,                   // strand index
                wNorm,               // normalized weight for saturation
                rainbowHueSpeed,
                rainbowHuePhase,
                rainbowSatMin,
                rainbowSatMax,
                rainbowValueBase,
                rainbowValuePulseAmp,
                rainbowValuePulseSpeed
            );

            seg.SetFade(0f);
            seg.Tick();
        }
    }

    /// <summary>
    /// Start fade in, then run until Despawn() is requested, then fade out.
    /// </summary>
    private void StartLoop()
    {
        if (playing) return;
        playing = true;
        despawnRequested = false;
        loopCo = StartCoroutine(LoopRoutine());
    }

    private IEnumerator LoopRoutine()
    {
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeInTime);
            SetFadeAll(k);
            TickAll();
            yield return null;
        }

        while (!despawnRequested)
        {
            TickAll();
            yield return null;
        }

        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / fadeOutTime);
            SetFadeAll(k);
            TickAll();
            yield return null;
        }

        ClearAll();
        playing = false;
        Destroy(gameObject);
    }

    private void SetFadeAll(float k)
    {
        for (int i = 0; i < segments.Count; i++) segments[i].SetFade(k);
    }

    private void TickAll()
    {
        if (a != null) { var pa = a.position; pa.z = 0f; a.position = pa; }
        if (b != null) { var pb = b.position; pb.z = 0f; b.position = pb; }

        for (int i = 0; i < segments.Count; i++) segments[i].Tick();
    }

    private void ClearAll()
    {
        for (int i = 0; i < segments.Count; i++) segments[i].Clear();
    }

    private void EnsureSegments(int count)
    {
        if (synergyLineSegmentPrefab == null)
        {
            var fallback = new GameObject("SynergyLineSegment_Fallback");
            fallback.SetActive(false);
            fallback.AddComponent<LineRenderer>();
            fallback.AddComponent<SynergyLineSegment>();
            synergyLineSegmentPrefab = fallback;
        }

        while (segments.Count < count)
        {
            var instGO = Instantiate(synergyLineSegmentPrefab, transform);
            instGO.name = "Waveform_" + segments.Count;
            instGO.SetActive(true);

            var seg = instGO.GetComponent<SynergyLineSegment>();
            if (seg == null) seg = instGO.AddComponent<SynergyLineSegment>();

            segments.Add(seg);
        }
    }

    private void ResolveSortingBelowActors(out string layerName, out int order)
    {
        int orderA = 0;
        int orderB = 0;

        if (aGroup != null) orderA = aGroup.sortingOrder;
        else if (aRenderer != null) orderA = aRenderer.sortingOrder;

        if (bGroup != null) orderB = bGroup.sortingOrder;
        else if (bRenderer != null) orderB = bRenderer.sortingOrder;

        layerName = Assets.Helpers.SortingHelper.Layer.SupportLineBelow;
        int underBoth = Mathf.Min(orderA, orderB) - 1;
        order = underBoth;
    }
}
