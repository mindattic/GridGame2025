// Assets/Scripts/Instances/SynergyLineSegment.cs
using UnityEngine;

//
// One waveform strand with sine + Perlin jitter.
// Base color is green, gently tinted toward tropical hues over time.
// Optional breathing halo. Colors pushed to both vertex color and URP Unlit material.
//
[RequireComponent(typeof(LineRenderer))]
public class SynergyLineSegment : MonoBehaviour
{
    // Renderers
    private LineRenderer line;      // core
    private LineRenderer glow;      // halo

    // Endpoints
    private Transform a;
    private Transform b;

    // Strand parameters
    private float phaseOffset;
    private float widthAbs;
    private float radiusAbs;
    private float frequency;
    private float noiseAmplitude;
    private float noiseScale;
    private float noiseSpeed;
    private AnimationCurve radiusOverT;
    private float fade;
    private float noiseSeed;
    private bool configured;

    // Tropical green tint state
    private int strandIndex;
    private float weightNorm;
    private float hueSpeed;
    private float huePhase;
    private float hueRange;
    private float satBase;
    private float satRange;
    private float valBase;
    private float valPulseAmp;
    private float valPulseSpeed;

    // Base green in HSV (computed once)
    private static readonly Color BaseGreenRGB = new Color(0.25f, 1.00f, 0.25f); // warmer true green
    private static float BaseHue = 0.42f;
    private static float BaseSat = 0.9f;
    private static float BaseVal = 1.0f;
    private static bool BaseHSVInit = false;

    // Halo controls
    private bool useHalo;
    private float glowWidthScale;
    private float glowAlpha;
    private float glowPulseAmp;
    private float glowPulseSpeed;
    private float glowHDRBoost;

    // Geometry
    private int segmentCount;

    // Shader property ids
    private static int idBaseColor = -1;
    private static int idColor = -1;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line == null) line = gameObject.AddComponent<LineRenderer>();
        if (line.material != null) line.material = new Material(line.material);

        SetupLineRenderer(line);

        var glowGO = new GameObject("Glow");
        glowGO.transform.SetParent(transform, false);
        glow = glowGO.AddComponent<LineRenderer>();
        glow.material = line.material != null ? new Material(line.material) : null;
        SetupLineRenderer(glow);

        noiseSeed = Random.Range(0f, 1000f);

        if (idBaseColor == -1) idBaseColor = Shader.PropertyToID("_BaseColor");
        if (idColor == -1) idColor = Shader.PropertyToID("_Color");

        if (!BaseHSVInit)
        {
            Color.RGBToHSV(BaseGreenRGB, out BaseHue, out BaseSat, out BaseVal);
            BaseHSVInit = true;
        }
    }

    /// <summary>
    /// Initialize a LineRenderer with consistent defaults.
    /// </summary>
    private void SetupLineRenderer(LineRenderer lr)
    {
        lr.useWorldSpace = true;
        lr.textureMode = LineTextureMode.Stretch;
        lr.alignment = LineAlignment.View;
        lr.numCornerVertices = 3;
        lr.numCapVertices = 3;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
    }

    /// <summary>
    /// Configure geometry, behavior, sorting, color, halo, and segment resolution.
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
        float inHueRange,
        float inSatBase,
        float inSatRange,
        float inValBase,
        float inValPulseAmp,
        float inValPulseSpeed,
        bool inUseHalo,
        float inGlowWidthScale,
        float inGlowAlpha,
        float inGlowPulseAmp,
        float inGlowPulseSpeed,
        float inGlowHDRBoost,
        int inSegmentCount
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
        hueRange = Mathf.Clamp01(inHueRange);
        satBase = Mathf.Clamp01(inSatBase);
        satRange = Mathf.Max(0f, inSatRange);
        valBase = Mathf.Clamp01(inValBase);
        valPulseAmp = Mathf.Max(0f, inValPulseAmp);
        valPulseSpeed = Mathf.Max(0f, inValPulseSpeed);

        useHalo = inUseHalo;
        glowWidthScale = inGlowWidthScale;
        glowAlpha = inGlowAlpha;
        glowPulseAmp = inGlowPulseAmp;
        glowPulseSpeed = inGlowPulseSpeed;
        glowHDRBoost = inGlowHDRBoost;

        segmentCount = Mathf.Max(2, inSegmentCount);
        line.positionCount = segmentCount;
        glow.positionCount = segmentCount;

        line.sortingLayerName = sortingLayer;
        line.sortingOrder = sortingOrder;
        glow.sortingLayerName = sortingLayer;
        glow.sortingOrder = sortingOrder - 1;

        configured = true;
    }

    /// <summary>
    /// External fade, 0 to 1.
    /// </summary>
    public void SetFade(float k)
    {
        fade = Mathf.Clamp01(k);
        line.widthMultiplier = widthAbs;
        // halo width pulses per-frame
    }

    /// <summary>
    /// Per-frame update of color and geometry. Halo breathes even at low fade.
    /// </summary>
    public void Tick()
    {
        if (!configured || a == null || b == null) return;

        // Colors
        Color greenTint = ComputeTintedGreen();

        Color core = greenTint;
        core.a *= fade;

        Color halo = greenTint;
        float alphaPulse = 0.65f + 0.35f * Mathf.Sin(Time.time * glowPulseSpeed + strandIndex);
        halo.a = Mathf.Clamp01(glowAlpha * alphaPulse * Mathf.Pow(fade, 0.9f));
        Color haloHDR = halo * glowHDRBoost;

        ApplyColor(line, core);
        if (useHalo)
        {
            ApplyColor(glow, haloHDR);
        }

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

        // Slow envelope to stretch and shrink the strand
        float envelope = 1f + Mathf.Sin(Time.time * 0.35f + phaseOffset * 0.7f) * 0.35f;

        // Halo width pulse keeps breathing regardless of fade
        if (useHalo)
        {
            float haloWidthPulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed + strandIndex) * glowPulseAmp;
            glow.widthMultiplier = widthAbs * glowWidthScale * haloWidthPulse;
        }

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

    /// <summary>
    /// Clear renderers when despawning.
    /// </summary>
    public void Clear()
    {
        if (line != null) line.positionCount = 0;
        if (glow != null) glow.positionCount = 0;
        configured = false;
    }

    /// <summary>
    /// Apply a color to a LineRenderer and its material for URP.
    /// </summary>
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
    /// Computes a green-first color gently pushed toward tropical hues.
    /// </summary>
    private Color ComputeTintedGreen()
    {
        float t = Time.time;

        float hueOffset = Mathf.Sin(t * hueSpeed + strandIndex * huePhase) * hueRange;
        float h = Mathf.Repeat(BaseHue + hueOffset, 1f);

        float s = Mathf.Clamp01(satBase + satRange * weightNorm);

        float vPulse = Mathf.Sin(t * valPulseSpeed + strandIndex * 0.4f);
        float v = Mathf.Clamp01(valBase + valPulseAmp * vPulse);

        return Color.HSVToRGB(h, s, v);
    }
}
