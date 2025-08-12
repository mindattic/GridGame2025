// Assets/Scripts/Instances/SynergyLineSegment.cs
using UnityEngine;

/// <summary>
/// One waveform strand with independent sine + Perlin jitter.
/// Base color is green, gently tinted toward tropical hues over time.
/// Adds a breathing halo that keeps pulsing even at low fade so the strand feels gaseous.
/// Applies color to both vertex color and URP Unlit material.
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

    // Renderers
    private LineRenderer line;      // core
    private LineRenderer glow;      // halo

    // Endpoints
    private Transform a;
    private Transform b;

    // Strand parameters
    private float phaseOffset;
    private float widthAbs = 0.012f;
    private float radiusAbs = 0.07f;
    private float fade = 0f;
    private float noiseSeed;
    private bool configured;

    // Tropical green tint state
    private int strandIndex;
    private float weightNorm;
    private float hueSpeed;     // how fast the tint shifts
    private float huePhase;     // per-strand offset
    private float hueRange;     // allowed deviation from base green hue
    private float satBase;      // base saturation for green
    private float satRange;     // extra saturation from weight
    private float valBase;      // base brightness
    private float valPulseAmp;  // brightness pulse amount
    private float valPulseSpeed;// brightness pulse speed

    // Base green in HSV (computed once)
    private static readonly Color BaseGreenRGB = new Color(0.25f, 1.00f, 0.25f); // brighter green
    private static float BaseHue = 0.42f;   // fallback
    private static float BaseSat = 0.9f;    // fallback
    private static float BaseVal = 1.0f;    // fallback
    private static bool BaseHSVInit = false;

    // Halo controls
    private float glowWidthScale = 2.6f;
    private float glowAlpha = 0.32f;
    private float glowPulseAmp = 0.28f;
    private float glowPulseSpeed = 0.9f;
    private float glowHDRBoost = 1.35f;

    // Material color property ids
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
    /// Initializes a LineRenderer with sane defaults.
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
        lr.positionCount = Mathf.Max(2, segmentCount);
    }

    /// <summary>
    /// Configure geometry, behavior, sorting, and tropical tint controls.
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
        hueRange = Mathf.Clamp01(inHueRange);
        satBase = Mathf.Clamp01(inSatBase);
        satRange = Mathf.Max(0f, inSatRange);
        valBase = Mathf.Clamp01(inValBase);
        valPulseAmp = Mathf.Max(0f, inValPulseAmp);
        valPulseSpeed = Mathf.Max(0f, inValPulseSpeed);

        line.sortingLayerName = sortingLayer;
        line.sortingOrder = sortingOrder;
        glow.sortingLayerName = sortingLayer;
        glow.sortingOrder = sortingOrder - 1;

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
        // glow width is pulsed per-frame
    }

    /// <summary>
    /// Recompute color and geometry each frame. Halo breathes even at low fade.
    /// </summary>
    public void Tick()
    {
        if (!configured || a == null || b == null) return;

        // Colors
        Color greenTint = ComputeTintedGreen();
        Color core = greenTint; core.a *= fade;

        Color halo = greenTint;
        float alphaPulse = 0.65f + 0.35f * Mathf.Sin(Time.time * glowPulseSpeed + strandIndex);
        halo.a = Mathf.Clamp01(glowAlpha * alphaPulse * Mathf.Pow(fade, 0.9f));

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

        // Slow envelope to stretch and shrink the strand
        float envelope = 1f + Mathf.Sin(Time.time * 0.35f + phaseOffset * 0.7f) * 0.35f;

        // Halo pulse width keeps breathing regardless of fade
        float haloWidthPulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed + strandIndex) * glowPulseAmp;
        glow.widthMultiplier = widthAbs * glowWidthScale * haloWidthPulse;

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
    /// Computes a green-first color that is gently pushed toward tropical hues.
    /// The hue stays near base green but shifts within hueRange, with saturation and value pulsing.
    /// </summary>
    private Color ComputeTintedGreen()
    {
        float t = Time.time;

        // Move hue around base green within a small tropical window
        float hueOffset = Mathf.Sin(t * hueSpeed + strandIndex * huePhase) * hueRange;
        float h = Mathf.Repeat(BaseHue + hueOffset, 1f);

        // Increase saturation slightly with stat weight so stronger strands pop a bit more
        float s = Mathf.Clamp01(satBase + satRange * weightNorm);

        // Gentle brightness pulse for life
        float vPulse = Mathf.Sin(t * valPulseSpeed + strandIndex * 0.4f);
        float v = Mathf.Clamp01(valBase + valPulseAmp * vPulse);

        return Color.HSVToRGB(h, s, v);
    }
}
