// Assets/Scripts/Instances/SynergyLine/SynergyLineInstance.cs
// Uses its own settings copy. No global Active. Supports live apply via a static registry.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Wispy multi-strand line between two actors.
/// Each instance keeps its own tunables. Values here are only used by this class,
/// while per-strand behavior lives in SynergyLineSegment.
/// </summary>
public class SynergyLineInstance : MonoBehaviour
{
    /// <summary>
    /// Local defaults used only by SynergyLineInstance.
    /// </summary>
    private static class S
    {
        // Group
        public static readonly int WaveformCount = 4;
        public static readonly float BaseRadius = 0.07f;
        public static readonly float BaseWidth = 0.012f;
        public static readonly float Frequency = 2.2f;

        // Noise
        public static readonly float NoiseAmplitude = 0.015f;
        public static readonly float NoiseScale = 2.5f;
        public static readonly float NoiseSpeed = 0.18f;

        // Shape: thin at 0 and 1, thickest at 0.5
        public static readonly AnimationCurve RadiusOverT = new AnimationCurve(
            new Keyframe(0.00f, g.TileSize * 0.33f, 0.0f, 0.0f),
            new Keyframe(0.50f, 1.00f, 0.0f, 0.0f),
            new Keyframe(1.00f, g.TileSize * 0.33f, 0.0f, 0.0f)
        );

        // Fade
        public static readonly float FadeInTime = 0.20f;
        public static readonly float FadeOutTime = 0.30f;

        // Sorting bias for multi-wave ordering
        public static readonly int OrderOffsetPerWave = 1;
        public static readonly int ExtraFrontBias = -2;

        // Color tint defaults passed to segments
        public static readonly float HueSpeed = 0.06f;
        public static readonly float HuePhase = 0.12f;
        public static readonly float HueRange = 0.06f;
        public static readonly float SatBase = 0.90f;
        public static readonly float SatRange = 0.08f;
        public static readonly float ValBase = 1.00f;
        public static readonly float ValPulseAmp = 0.08f;
        public static readonly float ValPulseSpeed = 0.70f;

        // Halo defaults passed to segments
        public static readonly bool UseHalo = true;
        public static readonly float GlowWidthScale = 2.6f;
        public static readonly float GlowAlpha = 0.20f;
        public static readonly float GlowPulseAmp = 0.28f;
        public static readonly float GlowPulseSpeed = 0.90f;
        public static readonly float GlowHDRBoost = 1.35f;

        // Geometry resolution for each segment line
        public static readonly int SegmentCount = 32;
    }

    // Prefab cache
    private GameObject synergyLineSegmentPrefab;

    // Runtime
    private readonly List<SynergyLineSegment> segments = new List<SynergyLineSegment>(8);
    public ActorInstance supporter;
    public ActorInstance attacker;
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
        synergyLineSegmentPrefab = PrefabLibrary.Get("SynergyLineStrandPrefab");
    }

    /// <summary>
    /// Entry point for creating the visual link between two actors.
    /// Assigns endpoints, calculates weights, configures segments, and starts the update loop.
    /// </summary>
    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        this.supporter = supporter;
        this.attacker = attacker;

        if (this.supporter == null || this.attacker == null)
        {
            Debug.LogError("SynergyLineInstance.Spawn received null supporter or attacker.");
            return;
        }

        Vector7 weights = new Vector7(
            this.supporter.Stats.Strength + this.attacker.Stats.Strength,
            this.supporter.Stats.Vitality + this.attacker.Stats.Vitality,
            this.supporter.Stats.Agility + this.attacker.Stats.Agility,
            this.supporter.Stats.Stamina + this.attacker.Stats.Stamina,
            this.supporter.Stats.Intelligence + this.attacker.Stats.Intelligence,
            this.supporter.Stats.Wisdom + this.attacker.Stats.Wisdom,
            this.supporter.Stats.Luck + this.attacker.Stats.Luck
        );

        Configure(weights);
        StartLoop();
    }

    /// <summary>
    /// Requests a graceful fade out and destruction.
    /// </summary>
    public void Despawn(float fadeSeconds = -1f)
    {
        // If you want per-call fade time, thread it through S.FadeOutTime.
        despawnRequested = true;
    }

    /// <summary>
    /// Gathers rendering components from endpoints, prepares segments, normalizes weights, and applies settings.
    /// </summary>
    public void Configure(Vector7 weights)
    {
        if (supporter == null || attacker == null)
        {
            Debug.LogError("SynergyLineInstance.Configure missing supporter or attacker. Did you call Spawn first?");
            return;
        }

        aGroup = supporter != null ? supporter.GetComponentInParent<SortingGroup>() : null;
        bGroup = attacker != null ? attacker.GetComponentInParent<SortingGroup>() : null;

        if (aGroup == null) aRenderer = supporter != null ? supporter.GetComponentInParent<SpriteRenderer>() : null;
        if (bGroup == null) bRenderer = attacker != null ? attacker.GetComponentInParent<SpriteRenderer>() : null;

        EnsureSegments(S.WaveformCount);

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

        if (wNormPerStrand == null || wNormPerStrand.Length != S.WaveformCount)
            wNormPerStrand = new float[S.WaveformCount];

        for (int i = 0; i < S.WaveformCount; i++)
            wNormPerStrand[i] = (w[i % 7] / max);

        ApplySettingsToSegments();
    }

    /// <summary>
    /// Starts the coroutine loop that drives fade, animation, and lifetime.
    /// </summary>
    private void StartLoop()
    {
        if (playing) return;
        playing = true;
        despawnRequested = false;
        loopCo = StartCoroutine(LoopRoutine());
    }

    /// <summary>
    /// Handles fade in, steady update, fade out, and cleanup.
    /// </summary>
    private IEnumerator LoopRoutine()
    {
        float t = 0f;
        while (t < S.FadeInTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / S.FadeInTime);
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
        while (t < S.FadeOutTime)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / S.FadeOutTime);
            SetFadeAll(k);
            TickAll();
            yield return null;
        }

        ClearAll();
        playing = false;
        Destroy(gameObject);
    }

    /// <summary>
    /// Applies configuration to each segment, including sorting and visual parameters.
    /// </summary>
    private void ApplySettingsToSegments()
    {
        if (supporter == null || attacker == null)
        {
            Debug.LogError("SynergyLineInstance.ApplySettingsToSegments missing supporter or attacker.");
            return;
        }

        float phaseStep = (Mathf.PI * 2f) / Mathf.Max(1, S.WaveformCount);

        string layerName;
        int baseOrder;
        ResolveSortingBelowActors(out layerName, out baseOrder);

        EnsureSegments(S.WaveformCount);

        for (int i = 0; i < S.WaveformCount; i++)
        {
            float wNorm = wNormPerStrand != null && i < wNormPerStrand.Length ? wNormPerStrand[i] : 0.5f;

            float widthAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * S.BaseWidth;
            float radiusAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * S.BaseRadius;
            float phase = phaseStep * i;

            var seg = segments[i];
            seg.Configure(
                supporter.gameObject.transform,
                attacker.gameObject.transform,
                widthAbs,
                radiusAbs,
                phase,
                S.Frequency,
                S.NoiseAmplitude,
                S.NoiseScale,
                S.NoiseSpeed,
                S.RadiusOverT,
                layerName,
                baseOrder + S.ExtraFrontBias + (i * S.OrderOffsetPerWave),
                i,
                wNorm,
                S.HueSpeed,
                S.HuePhase,
                S.HueRange,
                S.SatBase,
                S.SatRange,
                S.ValBase,
                S.ValPulseAmp,
                S.ValPulseSpeed,
                S.UseHalo,
                S.GlowWidthScale,
                S.GlowAlpha,
                S.GlowPulseAmp,
                S.GlowPulseSpeed,
                S.GlowHDRBoost,
                S.SegmentCount
            );

            seg.SetFade(0f);
            seg.Tick();
        }

        for (int i = S.WaveformCount; i < segments.Count; i++)
            segments[i].gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the same fade value across all active segments.
    /// </summary>
    private void SetFadeAll(float k)
    {
        int n = Mathf.Min(S.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].SetFade(k);
    }

    /// <summary>
    /// Per frame update that keeps endpoints pinned to z = 0 and ticks all segments.
    /// </summary>
    private void TickAll()
    {
        if (supporter != null) { var pa = supporter.position; pa.z = 0f; supporter.position = pa; }
        if (attacker != null) { var pb = attacker.position; pb.z = 0f; attacker.position = pb; }

        int n = Mathf.Min(S.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].Tick();
    }

    /// <summary>
    /// Clears all segment geometry when despawning.
    /// </summary>
    private void ClearAll()
    {
        int n = Mathf.Min(S.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].Clear();
    }

    /// <summary>
    /// Ensures the segment pool has at least the requested count.
    /// </summary>
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

    /// <summary>
    /// Computes a sorting layer and order that sits beneath both actors.
    /// </summary>
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

    /// <summary>
    /// Update the sorting layer for all active strands without changing their relative order.
    /// Allows the manager to flip between below and above layers after spawn.
    /// </summary>
    public void SetSorting(string sortingLayer)
    {
        int n = Mathf.Min(S.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++)
        {
            segments[i].SetSortingLayer(sortingLayer);
        }
    }
}
