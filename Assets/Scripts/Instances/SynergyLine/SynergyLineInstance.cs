// Assets/Scripts/Instances/SynergyLineInstance.cs
// Uses its own settings copy. No global Active. Supports live apply via a static registry.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Wispy multi-strand line between two actors.
/// Each instance owns a SynergyLineSettings copy. Use ApplyToAll to live-tune while playing.
/// </summary>
public class SynergyLineInstance : MonoBehaviour
{
    // Registry for live tuning
    private static readonly List<SynergyLineInstance> Live = new List<SynergyLineInstance>(64);

    // Prefab cache
    private GameObject synergyLineSegmentPrefab;

    // Settings
    private SynergyLineSettings settings = SynergyLineSettings.Defaults();

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

    // Cached per-strand weights for reconfigure
    private float[] wNormPerStrand;

    private void Awake()
    {
        synergyLineSegmentPrefab = PrefabRepo.Get("SynergyLineSegmentPrefab");
    }

    private void OnEnable()
    {
        if (!Live.Contains(this)) Live.Add(this);
    }

    private void OnDisable()
    {
        Live.Remove(this);
    }

    /// <summary>
    /// Broadcast helper. Debug UI can call this to hot-apply during play.
    /// </summary>
    public static void ApplyToAll(SynergyLineSettings newSettings)
    {
        for (int i = 0; i < Live.Count; i++)
        {
            if (Live[i] != null) Live[i].ApplySettings(newSettings);
        }
    }

    /// <summary>
    /// Replace internal settings and reconfigure strands.
    /// </summary>
    public void ApplySettings(SynergyLineSettings newSettings)
    {
        if (newSettings == null) return;
        settings.CopyFrom(newSettings);

        if (segments.Count > 0)
        {
            ApplySettingsToSegments();
        }
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

    public void Despawn(float fadeSeconds = -1f)
    {
        //if (fadeSeconds >= 0f) 
        //    settings.FadeOutTime = fadeSeconds;
        despawnRequested = true;
    }

    public void Configure(Transform start, Transform end, Vector7 weights)
    {
        a = start;
        b = end;

        aGroup = a != null ? a.GetComponentInParent<SortingGroup>() : null;
        bGroup = b != null ? b.GetComponentInParent<SortingGroup>() : null;

        if (aGroup == null) aRenderer = a != null ? a.GetComponentInParent<SpriteRenderer>() : null;
        if (bGroup == null) bRenderer = b != null ? b.GetComponentInParent<SpriteRenderer>() : null;

        EnsureSegments(settings.WaveformCount);

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

        if (wNormPerStrand == null || wNormPerStrand.Length != settings.WaveformCount)
            wNormPerStrand = new float[settings.WaveformCount];

        for (int i = 0; i < settings.WaveformCount; i++)
            wNormPerStrand[i] = (w[i % 7] / max);

        ApplySettingsToSegments();
    }

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
        while (t < settings.FadeInTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / settings.FadeInTime);
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
        while (t < settings.FadeOutTime)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / settings.FadeOutTime);
            SetFadeAll(k);
            TickAll();
            yield return null;
        }

        ClearAll();
        playing = false;
        Destroy(gameObject);
    }

    private void ApplySettingsToSegments()
    {
        float phaseStep = (Mathf.PI * 2f) / Mathf.Max(1, settings.WaveformCount);

        string layerName;
        int baseOrder;
        ResolveSortingBelowActors(out layerName, out baseOrder);

        EnsureSegments(settings.WaveformCount);

        for (int i = 0; i < settings.WaveformCount; i++)
        {
            float wNorm = wNormPerStrand != null && i < wNormPerStrand.Length ? wNormPerStrand[i] : 0.5f;

            float widthAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * settings.BaseWidth;
            float radiusAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * settings.BaseRadius;
            float phase = phaseStep * i;

            var seg = segments[i];
            seg.Configure(
                a, b,
                widthAbs,
                radiusAbs,
                phase,
                settings.Frequency,
                settings.NoiseAmplitude,
                settings.NoiseScale,
                settings.NoiseSpeed,
                settings.RadiusOverT,
                layerName,
                baseOrder + settings.ExtraFrontBias + (i * settings.OrderOffsetPerWave),
                i,
                wNorm,
                settings.HueSpeed,
                settings.HuePhase,
                settings.HueRange,
                settings.SatBase,
                settings.SatRange,
                settings.ValBase,
                settings.ValPulseAmp,
                settings.ValPulseSpeed,
                settings.UseHalo,
                settings.GlowWidthScale,
                settings.GlowAlpha,
                settings.GlowPulseAmp,
                settings.GlowPulseSpeed,
                settings.GlowHDRBoost,
                settings.SegmentCount
            );

            seg.SetFade(0f);
            seg.Tick();
        }

        for (int i = settings.WaveformCount; i < segments.Count; i++)
            segments[i].gameObject.SetActive(false);
    }

    private void SetFadeAll(float k)
    {
        int n = Mathf.Min(settings.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].SetFade(k);
    }

    private void TickAll()
    {
        if (a != null) { var pa = a.position; pa.z = 0f; a.position = pa; }
        if (b != null) { var pb = b.position; pb.z = 0f; b.position = pb; }

        int n = Mathf.Min(settings.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].Tick();
    }

    private void ClearAll()
    {
        int n = Mathf.Min(settings.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].Clear();
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
