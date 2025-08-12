// Assets/Scripts/Instances/SynergyLineSettings.cs
// Serializable settings bag used by SynergyLineSettingsComponent and at runtime.

using UnityEngine;

/// <summary>
/// Tunables for synergy lines and segments. Designed for Inspector editing.
/// </summary>
[System.Serializable]
public class SynergyLineSettings
{
    // Group
    [SerializeField] private int waveformCount = 2;
    [SerializeField] private float baseRadius = 0.07f;
    [SerializeField] private float baseWidth = 0.012f;
    [SerializeField] private float frequency = 2.2f;

    // Noise
    [SerializeField] private float noiseAmplitude = 0.015f;
    [SerializeField] private float noiseScale = 2.5f;
    [SerializeField] private float noiseSpeed = 0.18f;

    // Shape
    [SerializeField] private AnimationCurve radiusOverT = AnimationCurve.EaseInOut(0, 0.2f, 1, 1.0f);

    // Fade
    [SerializeField] private float fadeInTime = 0.20f;
    [SerializeField] private float fadeOutTime = 0.30f;

    // Sorting
    [SerializeField] private int orderOffsetPerWave = 1;
    [SerializeField] private int extraFrontBias = -2;

    // Tropical green tint
    [SerializeField] private float hueSpeed = 0.06f;
    [SerializeField] private float huePhase = 0.12f;
    [SerializeField] private float hueRange = 0.06f;
    [SerializeField] private float satBase = 0.90f;
    [SerializeField] private float satRange = 0.08f;
    [SerializeField] private float valBase = 1.00f;
    [SerializeField] private float valPulseAmp = 0.08f;
    [SerializeField] private float valPulseSpeed = 0.70f;

    // Halo
    [SerializeField] private bool useHalo = true;
    [SerializeField] private float glowWidthScale = 2.6f;
    [SerializeField] private float glowAlpha = 0.32f;
    [SerializeField] private float glowPulseAmp = 0.28f;
    [SerializeField] private float glowPulseSpeed = 0.90f;
    [SerializeField] private float glowHDRBoost = 1.35f;

    // Geometry
    [SerializeField] private int segmentCount = 56;

    /// <summary>
    /// Factory defaults used when resetting or creating new containers.
    /// </summary>
    public static SynergyLineSettings Defaults()
    {
        return new SynergyLineSettings();
    }

    /// <summary>
    /// Deep copy so runtime code can hold an immutable snapshot.
    /// </summary>
    public SynergyLineSettings Clone()
    {
        return new SynergyLineSettings
        {
            waveformCount = waveformCount,
            baseRadius = baseRadius,
            baseWidth = baseWidth,
            frequency = frequency,

            noiseAmplitude = noiseAmplitude,
            noiseScale = noiseScale,
            noiseSpeed = noiseSpeed,

            radiusOverT = new AnimationCurve(radiusOverT.keys),

            fadeInTime = fadeInTime,
            fadeOutTime = fadeOutTime,

            orderOffsetPerWave = orderOffsetPerWave,
            extraFrontBias = extraFrontBias,

            hueSpeed = hueSpeed,
            huePhase = huePhase,
            hueRange = hueRange,
            satBase = satBase,
            satRange = satRange,
            valBase = valBase,
            valPulseAmp = valPulseAmp,
            valPulseSpeed = valPulseSpeed,

            useHalo = useHalo,
            glowWidthScale = glowWidthScale,
            glowAlpha = glowAlpha,
            glowPulseAmp = glowPulseAmp,
            glowPulseSpeed = glowPulseSpeed,
            glowHDRBoost = glowHDRBoost,

            segmentCount = segmentCount
        };
    }

    /// <summary>
    /// Replace all values from another instance.
    /// </summary>
    public void CopyFrom(SynergyLineSettings other)
    {
        waveformCount = other.waveformCount;
        baseRadius = other.baseRadius;
        baseWidth = other.baseWidth;
        frequency = other.frequency;

        noiseAmplitude = other.noiseAmplitude;
        noiseScale = other.noiseScale;
        noiseSpeed = other.noiseSpeed;

        radiusOverT = new AnimationCurve(other.radiusOverT.keys);

        fadeInTime = other.fadeInTime;
        fadeOutTime = other.fadeOutTime;

        orderOffsetPerWave = other.orderOffsetPerWave;
        extraFrontBias = other.extraFrontBias;

        hueSpeed = other.hueSpeed;
        huePhase = other.huePhase;
        hueRange = other.hueRange;
        satBase = other.satBase;
        satRange = other.satRange;
        valBase = other.valBase;
        valPulseAmp = other.valPulseAmp;
        valPulseSpeed = other.valPulseSpeed;

        useHalo = other.useHalo;
        glowWidthScale = other.glowWidthScale;
        glowAlpha = other.glowAlpha;
        glowPulseAmp = other.glowPulseAmp;
        glowPulseSpeed = other.glowPulseSpeed;
        glowHDRBoost = other.glowHDRBoost;

        segmentCount = other.segmentCount;
    }

    // Accessors for runtime code
    public int WaveformCount => waveformCount;
    public float BaseRadius => baseRadius;
    public float BaseWidth => baseWidth;
    public float Frequency => frequency;

    public float NoiseAmplitude => noiseAmplitude;
    public float NoiseScale => noiseScale;
    public float NoiseSpeed => noiseSpeed;

    public AnimationCurve RadiusOverT => radiusOverT;

    public float FadeInTime => fadeInTime;
    public float FadeOutTime => fadeOutTime;

    public int OrderOffsetPerWave => orderOffsetPerWave;
    public int ExtraFrontBias => extraFrontBias;

    public float HueSpeed => hueSpeed;
    public float HuePhase => huePhase;
    public float HueRange => hueRange;
    public float SatBase => satBase;
    public float SatRange => satRange;
    public float ValBase => valBase;
    public float ValPulseAmp => valPulseAmp;
    public float ValPulseSpeed => valPulseSpeed;

    public bool UseHalo => useHalo;
    public float GlowWidthScale => glowWidthScale;
    public float GlowAlpha => glowAlpha;
    public float GlowPulseAmp => glowPulseAmp;
    public float GlowPulseSpeed => glowPulseSpeed;
    public float GlowHDRBoost => glowHDRBoost;

    public int SegmentCount => segmentCount;

    /// <summary>
    /// Build a stable hash for change detection while playing.
    /// </summary>
    public int ComputeHash()
    {
        unchecked
        {
            int h = 17;
            h = h * 23 + waveformCount;
            h = h * 23 + baseRadius.GetHashCode();
            h = h * 23 + baseWidth.GetHashCode();
            h = h * 23 + frequency.GetHashCode();

            h = h * 23 + noiseAmplitude.GetHashCode();
            h = h * 23 + noiseScale.GetHashCode();
            h = h * 23 + noiseSpeed.GetHashCode();

            // Curve keys
            if (radiusOverT != null)
            {
                var keys = radiusOverT.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    h = h * 23 + keys[i].time.GetHashCode();
                    h = h * 23 + keys[i].value.GetHashCode();
                    h = h * 23 + keys[i].inTangent.GetHashCode();
                    h = h * 23 + keys[i].outTangent.GetHashCode();
                }
            }

            h = h * 23 + fadeInTime.GetHashCode();
            h = h * 23 + fadeOutTime.GetHashCode();

            h = h * 23 + orderOffsetPerWave;
            h = h * 23 + extraFrontBias;

            h = h * 23 + hueSpeed.GetHashCode();
            h = h * 23 + huePhase.GetHashCode();
            h = h * 23 + hueRange.GetHashCode();
            h = h * 23 + satBase.GetHashCode();
            h = h * 23 + satRange.GetHashCode();
            h = h * 23 + valBase.GetHashCode();
            h = h * 23 + valPulseAmp.GetHashCode();
            h = h * 23 + valPulseSpeed.GetHashCode();

            h = h * 23 + useHalo.GetHashCode();
            h = h * 23 + glowWidthScale.GetHashCode();
            h = h * 23 + glowAlpha.GetHashCode();
            h = h * 23 + glowPulseAmp.GetHashCode();
            h = h * 23 + glowPulseSpeed.GetHashCode();
            h = h * 23 + glowHDRBoost.GetHashCode();

            h = h * 23 + segmentCount;
            return h;
        }
    }
}
