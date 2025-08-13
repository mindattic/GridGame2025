// Assets/Scripts/Instances/SynergyLineInstance.cs
// Wispy multi-instance line between two actors with fixed spawn anchors.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using settings = SynergyLineSettings;

/// <summary>
/// Wispy multi-instance line between two actors.
/// Each instance owns a SynergyLineSettings copy. Use ApplyToAll to live-tune while playing.
/// </summary>
public class SynergyLineInstance : MonoBehaviour
{
    // Prefab cache.
    private GameObject synergyLineStrandPrefab;

    // Runtime.
    private readonly List<SynergyLineStrand> strands = new List<SynergyLineStrand>(8);
    public ActorInstance supporter { get; private set; }
    public ActorInstance attacker { get; private set; }

    // Pinned world-space endpoints captured at spawn (tile centers)
    private Vector3 aWS;
    private Vector3 bWS;

    private bool playing;
    private bool despawnRequested;

    private float[] wNormPerStrand;

    private void Awake()
    {
        synergyLineStrandPrefab = PrefabLibrary.Get("SynergyLineStrandPrefab");
    }

    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        this.supporter = supporter;
        this.attacker = attacker;

        // Capture tile-centered, world-space anchors ONCE.
        aWS = supporter.currentTile.position;
        bWS = attacker.currentTile.position;

        Vector7 weights = new Vector7(
            supporter.Stats.Strength + attacker.Stats.Strength,
            supporter.Stats.Vitality + attacker.Stats.Vitality,
            supporter.Stats.Agility + attacker.Stats.Agility,
            supporter.Stats.Stamina + attacker.Stats.Stamina,
            supporter.Stats.Intelligence + attacker.Stats.Intelligence,
            supporter.Stats.Wisdom + attacker.Stats.Wisdom,
            supporter.Stats.Luck + attacker.Stats.Luck
        );

        Configure(weights);
        StartLoop();
    }

  
    public void Despawn() => despawnRequested = true;

    public void Configure(Vector7 weights)
    {
        // Compute normalized weights across strands.
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
            wNormPerStrand[i] = w[i % 7] / max;

        float phaseStep = (Mathf.PI * 2f) / Mathf.Max(1, settings.WaveformCount);

        EnsureSegments(settings.WaveformCount);

        string layer = Assets.Helpers.SortingHelper.Layer.SupportLineBelow; // SortingManager can override
        int baseOrder = 0;

        for (int i = 0; i < settings.WaveformCount; i++)
        {
            float wNorm = (i < wNormPerStrand.Length) ? wNormPerStrand[i] : 0.5f;
            float widthAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * settings.BaseWidth;
            float radiusAbs = Mathf.Lerp(0.75f, 1.25f, wNorm) * settings.BaseRadius;
            float phase = phaseStep * i;

            var s = strands[i];
            s.Configure(
                aWS, 
                bWS,
                widthAbs,
                radiusAbs,
                phase,
                settings.Frequency,
                settings.NoiseAmplitude,
                settings.NoiseScale,
                settings.NoiseSpeed,
                settings.RadiusOverT,
                sortingLayer: layer,
                sortingOrder: baseOrder + settings.ExtraFrontBias + (i * settings.OrderOffsetPerWave),
                inStrandIndex: i,
                inWeightNorm: wNorm,
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

            s.SetFade(0f);
            s.Tick();
        }

        for (int i = settings.WaveformCount; i < strands.Count; i++)
            strands[i].gameObject.SetActive(false);
    }

    private void StartLoop()
    {
        if (playing) return;
        playing = true;
        despawnRequested = false;
        StartCoroutine(LoopRoutine());
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

    private void SetFadeAll(float k)
    {
        int n = Mathf.Min(settings.WaveformCount, strands.Count);
        for (int i = 0; i < n; i++) strands[i].SetFade(k);
    }

    private void TickAll()
    {
        int n = Mathf.Min(settings.WaveformCount, strands.Count);
        for (int i = 0; i < n; i++) strands[i].Tick();
    }

    private void ClearAll()
    {
        int n = Mathf.Min(settings.WaveformCount, strands.Count);
        for (int i = 0; i < n; i++) strands[i].Clear();
    }

    private void EnsureSegments(int count)
    {
        while (strands.Count < count)
        {
            var go = Instantiate(synergyLineStrandPrefab, transform);
            go.name = "SynergyLine_" + strands.Count;
            go.SetActive(true);

            var instance = go.GetComponent<SynergyLineStrand>();
            if (instance == null) instance = go.AddComponent<SynergyLineStrand>();

            strands.Add(instance);
        }
    }

    public void SetSorting(string sortingLayer, int baseOrder = 0)
    {
        int n = Mathf.Min(settings.WaveformCount, strands.Count);
        for (int i = 0; i < n; i++)
        {
            int order = baseOrder + settings.ExtraFrontBias + (i * settings.OrderOffsetPerWave);
            strands[i].SetSorting(sortingLayer, order);
        }
    }
}
