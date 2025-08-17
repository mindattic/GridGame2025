// Assets/Scripts/Instances/SynergyLine/SynergyLineInstance.cs
// Uses its own settings copy. No global Active. Supports live apply via a static registry.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;
using settings = SynergyLineSettings;

/// <summary>
/// Wispy multi-strand line between two actors.
/// Each instance owns a SynergyLineSettings copy. Use ApplyToAll to live-tune while playing.
/// </summary>
public class SynergyLineInstance : MonoBehaviour
{
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
        // Assign endpoints first so downstream code can safely use instance fields.
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
        // if (fadeSeconds >= 0f) settings.FadeOutTime = fadeSeconds;
        despawnRequested = true;
    }

    /// <summary>
    /// Gathers rendering components from endpoints, prepares segments, normalizes weights, and applies settings.
    /// </summary>
    public void Configure(Vector7 weights)
    {
        // Defensive guard in case Spawn was called improperly.
        if (supporter == null || attacker == null)
        {
            Debug.LogError("SynergyLineInstance.Configure missing supporter or attacker. Did you call Spawn first?");
            return;
        }

        aGroup = supporter != null ? supporter.GetComponentInParent<SortingGroup>() : null;
        bGroup = attacker != null ? attacker.GetComponentInParent<SortingGroup>() : null;

        if (aGroup == null) aRenderer = supporter != null ? supporter.GetComponentInParent<SpriteRenderer>() : null;
        if (bGroup == null) bRenderer = attacker != null ? attacker.GetComponentInParent<SpriteRenderer>() : null;

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

    /// <summary>
    /// Applies configuration to each segment, including sorting and visual parameters.
    /// </summary>
    private void ApplySettingsToSegments()
    {
        // Defensive guard to avoid null deref when assigning endpoints into segments.
        if (supporter == null || attacker == null)
        {
            Debug.LogError("SynergyLineInstance.ApplySettingsToSegments missing supporter or attacker.");
            return;
        }

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
                supporter.gameObject.transform,
                attacker.gameObject.transform,
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

    /// <summary>
    /// Sets the same fade value across all active segments.
    /// </summary>
    private void SetFadeAll(float k)
    {
        int n = Mathf.Min(settings.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].SetFade(k);
    }

    /// <summary>
    /// Per frame update that keeps endpoints pinned to z = 0 and ticks all segments.
    /// </summary>
    private void TickAll()
    {
        if (supporter != null) { var pa = supporter.position; pa.z = 0f; supporter.position = pa; }
        if (attacker != null) { var pb = attacker.position; pb.z = 0f; attacker.position = pb; }

        int n = Mathf.Min(settings.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++) segments[i].Tick();
    }

    /// <summary>
    /// Clears all segment geometry when despawning.
    /// </summary>
    private void ClearAll()
    {
        int n = Mathf.Min(settings.WaveformCount, segments.Count);
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
        int n = Mathf.Min(settings.WaveformCount, segments.Count);
        for (int i = 0; i < n; i++)
        {
            segments[i].SetSortingLayer(sortingLayer);
        }
    }
}
