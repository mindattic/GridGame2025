// Assets/Scripts/Instances/SynergyLineSegment.cs
using UnityEngine;

/// <summary>
/// One waveform strand with independent sine + Perlin jitter,
/// plus a breathing halo glow that mirrors the core line.
/// Also animates a tropical rainbow color and applies it to both vertex color and URP Unlit material.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class SynergyLineSegment : MonoBehaviour
{
    // Geometry
    private int segmentCount = 56;
    private float frequency = 2.2f;
    private AnimationCurve radiusOverT = AnimationCurve.EaseInOut(0, 0.2f, 1, 1);

    // Noise
    private float noiseAmplitude = 0.015f;
    private float noiseScale = 2.5f;
    private float noiseSpeed = 0.18f;

    // Core runtime
    private LineRenderer line;          // core
    private LineRenderer glow;          // halo
    private Transform a;
    private Transform b;
    private float phaseOffset;
    private float widthAbs = 0.012f;
    private float radiusAbs = 0.07f;
    private float fade = 0f;
    private float noiseSeed;
    private bool configured;

    // Rainbow animation state
    private int strandIndex;
    private float weightNorm;
    private float hueSpeed;
    private float huePhase;
    private float satMin;
    private float satMax;
    private float valBase;
    private float valPulseAmp;
    private float valPulseSpeed;

    // Glow controls
    private float glowWidthScale = 2.4f;   // halo is wider than core
    private float glowAlpha = 0.35f;       // base halo opacity
    private float glowPulseAmp = 0.25f;    // halo alpha/width pulse amount
    private float glowPulseSpeed = 0.9f;   // halo pulse speed
    private float glowHDRBoost = 1.35f;    // >1 boosts Bloom if HDR is on

    // Material color property ids
    private static int idBaseColor = -1;
    private static int idColor = -1;

    private void Awake()
    {
        // Core line
        line = GetComponent<LineRenderer>();
        if (line == null) line = gameObject.AddComponent<LineRenderer>();
        if (line.material != null) line.material = new Material(line.material);

        SetupLineRenderer(line);

        // Halo line as child
        var glowGO = new GameObject("Glow");
        glowGO.transform.SetParent(transform, false);
        glow = glowGO.AddComponent<LineRenderer>();
        glow.material = line.material != null ? new Material(line.material) : null;
        SetupLineRenderer(glow);

        noiseSeed = Random.Range(0f, 1000f);

        if (idBaseColor == -1) idBaseColor = Shader.PropertyToID("_BaseColor");
        if (idColor == -1) idColor = Shader.PropertyToID("_Color");
    }

    private void SetupLineRenderer(LineRenderer lr)
    {
        lr.useWorldSpace = true;
        lr.textureMode = LineTextureMode.Stretch;
        lr.alignment = LineAlignment.View;
        lr.numCornerVertices = 3;
        lr.numCapVertices = 3;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.positionCount = Mathf.Max(2, segmentCount);
    }

    /// <summary>
    /// Configure geometry, behavior, sorting, and rainbow controls.
    /// </summary>
    public void Configure(
        Transform start,
        Transform end,
        float widthAbsolute,
        float radiusAbsolute,
        float phase,
        float inFrequency,
        float inNoiseAmplitude,
        float inNoiseScale,
        float inNoiseSpeed,
        AnimationCurve inRadiusOverT,
        string sortingLayer,
        int sortingOrder,
        int inStrandIndex,
        float inWeightNorm,
        float inHueSpeed,
        float inHuePhase,
        float inSatMin,
        float inSatMax,
        float inValBase,
        float inValPulseAmp,
        float inValPulseSpeed
    )
    {
        a = start;
        b = end;

        widthAbs = Mathf.Max(0.0025f, widthAbsolute);
        radiusAbs = Mathf.Max(0.01f, radiusAbsolute);
        phaseOffset = phase;

        frequency = Mathf.Max(0.1f, inFrequency);
        noiseAmplitude = Mathf.Max(0f, inNoiseAmplitude);
        noiseScale = Mathf.Max(0.001f, inNoiseScale);
        noiseSpeed = Mathf.Max(0f, inNoiseSpeed);
        radiusOverT = inRadiusOverT ?? AnimationCurve.Linear(0f, 1f, 1f, 1f);

        strandIndex = inStrandIndex;
        weightNorm = Mathf.Clamp01(inWeightNorm);
        hueSpeed = inHueSpeed;
        huePhase = inHuePhase;
        satMin = inSatMin;
        satMax = inSatMax;
        valBase = inValBase;
        valPulseAmp = inValPulseAmp;
        valPulseSpeed = inValPulseSpeed;

        // Sorting for both renderers
        line.sortingLayerName = sortingLayer;
        line.sortingOrder = sortingOrder;
        glow.sortingLayerName = sortingLayer;
        glow.sortingOrder = sortingOrder - 1; // keep glow under core to avoid harsh overlap

        if (line.positionCount != segmentCount) line.positionCount = segmentCount;
        if (glow.positionCount != segmentCount) glow.positionCount = segmentCount;

        configured = true;
    }

    /// <summary>
    /// External fade, 0 to 1.
    /// </summary>
    public void SetFade(float k)
    {
        fade = Mathf.Clamp01(k);
        line.widthMultiplier = widthAbs;
        // glow width is set per-frame with pulse
    }

    public void Tick()
    {
        if (!configured || a == null || b == null) return;

        // Colors
        Color core = ComputeRainbowColor();
        core.a *= fade;

        Color halo = core;
        halo.a = Mathf.Clamp01(glowAlpha * fade);

        // Apply brightness boost to halo so Bloom can grab it
        Color haloHDR = halo * glowHDRBoost;

        ApplyColor(line, core);
        ApplyColor(glow, haloHDR);

        // Positions
        Vector3 start = a.position; start.z = 0f;
        Vector3 end = b.position; end.z = 0f;

        Vector3 dir = end - start;
        float len = dir.magnitude;
        if (len < 0.0001f)
        {
            if (line.positionCount != 2) line.positionCount = 2;
            if (glow.positionCount != 2) glow.positionCount = 2;
            line.SetPosition(0, start); line.SetPosition(1, start);
            glow.SetPosition(0, start); glow.SetPosition(1, start);
            return;
        }

        Vector3 forward = dir / len;
        Vector3 perp = new Vector3(-forward.y, forward.x, 0f);

        if (line.positionCount != segmentCount) line.positionCount = segmentCount;
        if (glow.positionCount != segmentCount) glow.positionCount = segmentCount;

        float twoPi = Mathf.PI * 2f;
        float envelope = 1f + Mathf.Sin(Time.time * 0.35f + phaseOffset * 0.7f) * 0.35f;

        // Halo pulse: width and alpha breathe softly
        float haloPulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed + strandIndex) * glowPulseAmp;
        glow.widthMultiplier = widthAbs * glowWidthScale * haloPulse;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            float radius = radiusAbs * radiusOverT.Evaluate(t) * envelope;

            float sin = Mathf.Sin(twoPi * frequency * t + phaseOffset + Time.time * 0.18f);
            float n = (Mathf.PerlinNoise(noiseSeed + t * noiseScale, Time.time * noiseSpeed) - 0.5f) * 2f;

            Vector3 basePos = Vector3.Lerp(start, end, t);
            Vector3 offset = perp * ((sin + n) * radius);

            Vector3 p = basePos + offset;
            line.SetPosition(i, p);
            glow.SetPosition(i, p);
        }
    }

    public void Clear()
    {
        if (line != null) line.positionCount = 0;
        if (glow != null) glow.positionCount = 0;
        configured = false;
    }

    private void ApplyColor(LineRenderer lr, Color c)
    {
        lr.startColor = c;
        lr.endColor = c;

        var mat = lr.material;
        if (mat != null)
        {
            if (mat.HasProperty(idBaseColor)) mat.SetColor(idBaseColor, c);
            else if (mat.HasProperty(idColor)) mat.SetColor(idColor, c);
        }
    }

    /// <summary>
    /// Tropical rainbow color animated over time with per-strand offsets.
    /// </summary>
    private Color ComputeRainbowColor()
    {
        float[] baseHues = { 0.00f, 0.08f, 0.16f, 0.33f, 0.50f, 0.62f, 0.85f };
        float t = Time.time;
        float h0 = baseHues[strandIndex % 7];

        float hue = Mathf.Repeat(h0 + t * hueSpeed + huePhase * strandIndex, 1f);
        float sat = Mathf.Lerp(satMin, satMax, 0.5f + 0.5f * weightNorm);
        float vPulse = Mathf.Sin(t * valPulseSpeed + strandIndex);
        float val = Mathf.Clamp01(valBase + valPulseAmp * vPulse);

        return Color.HSVToRGB(hue, sat, val);
    }
}
