// Wispy multi-strand line between two actors.
// Each instance keeps its own tunables. Per-strand behavior lives in SynergyLineStrand.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

public class SynergyLineInstance : MonoBehaviour
{
    // Group
    [SerializeField] private int waveformCount = 4;
    [SerializeField] private float baseRadius = 0.07f;
    [SerializeField] private float baseWidth = 0.012f;

    // Fade
    [SerializeField] private float fadeInTime = 0.20f;
    [SerializeField] private float fadeOutTime = 0.30f;

    // Sorting bias for multi-wave ordering
    [SerializeField] private int orderOffsetPerWave = 1;
    [SerializeField] private int extraFrontBias = -2;

    // Geometry resolution for each strand line
    [SerializeField] private int strandSegmentCount = 32;

    // Prefab cache
    private GameObject synergyStrandPrefab;

    // Runtime
    private readonly List<SynergyLineStrand> strands = new List<SynergyLineStrand>(8);
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
        synergyStrandPrefab = PrefabLibrary.Get("SynergyStrandPrefab");
    }

    /// <summary>
    /// Entry point for creating the visual link between two actors.
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
        despawnRequested = true;
    }

    /// <summary>
    /// Gathers rendering components from endpoints, prepares strands, normalizes weights, and applies settings.
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

        EnsureStrands(waveformCount);

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

        if (wNormPerStrand == null || wNormPerStrand.Length != waveformCount)
            wNormPerStrand = new float[waveformCount];

        for (int i = 0; i < waveformCount; i++)
            wNormPerStrand[i] = (w[i % 7] / max);

        ApplySettingsToStrands();
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

    /// <summary>
    /// Applies configuration to each strand, including sorting and visual parameters.
    /// </summary>
    private void ApplySettingsToStrands()
    {
        if (supporter == null || attacker == null)
        {
            Debug.LogError("SynergyLineInstance.ApplySettingsToStrands missing supporter or attacker.");
            return;
        }

        float phaseStep = (Mathf.PI * 2f) / Mathf.Max(1, waveformCount);

        string layerName;
        int baseOrder;
        ResolveSortingBelowActors(out layerName, out baseOrder);

        EnsureStrands(waveformCount);

        for (int i = 0; i < waveformCount; i++)
        {
            float wNorm = wNormPerStrand != null && i < wNormPerStrand.Length ? wNormPerStrand[i] : 0.5f;

            float widthForStrand = Mathf.Lerp(0.75f, 1.25f, wNorm) * baseWidth;
            float radiusForStrand = Mathf.Lerp(0.75f, 1.25f, wNorm) * baseRadius;
            float phase = phaseStep * i;

            var strand = strands[i];
            strand.Configure(
                supporter.transform,
                attacker.transform,
                widthForStrand,
                radiusForStrand,
                phase,
                layerName,
                baseOrder + extraFrontBias + (i * orderOffsetPerWave),
                i,
                wNorm,
                strandSegmentCount
            );

            strand.SetFade(0f);
            strand.Tick();
        }

        for (int i = waveformCount; i < strands.Count; i++)
            strands[i].gameObject.SetActive(false);
    }

    private void SetFadeAll(float k)
    {
        int n = Mathf.Min(waveformCount, strands.Count);
        for (int i = 0; i < n; i++) strands[i].SetFade(k);
    }

    private void TickAll()
    {
        if (supporter != null) { var pa = supporter.position; pa.z = 0f; supporter.position = pa; }
        if (attacker != null) { var pb = attacker.position; pb.z = 0f; attacker.position = pb; }

        int n = Mathf.Min(waveformCount, strands.Count);
        for (int i = 0; i < n; i++) strands[i].Tick();
    }

    private void ClearAll()
    {
        int n = Mathf.Min(waveformCount, strands.Count);
        for (int i = 0; i < n; i++) strands[i].Clear();
    }

    private void EnsureStrands(int count)
    {
        if (synergyStrandPrefab == null)
        {
            var fallback = new GameObject("SynergyLineStrand_Fallback");
            fallback.SetActive(false);
            fallback.AddComponent<LineRenderer>();
            fallback.AddComponent<SynergyLineStrand>();
            synergyStrandPrefab = fallback;
        }

        while (strands.Count < count)
        {
            var instGO = Instantiate(synergyStrandPrefab, transform);
            instGO.name = "Waveform_" + strands.Count;
            instGO.SetActive(true);

            var seg = instGO.GetComponent<SynergyLineStrand>();
            if (seg == null) seg = instGO.AddComponent<SynergyLineStrand>();

            strands.Add(seg);
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

    /// <summary>
    /// Update the sorting layer for all active strands without changing their relative order.
    /// </summary>
    public void SetSorting(string sortingLayer)
    {
        int n = Mathf.Min(waveformCount, strands.Count);
        for (int i = 0; i < n; i++)
        {
            strands[i].SetSortingLayer(sortingLayer);
        }
    }
}
