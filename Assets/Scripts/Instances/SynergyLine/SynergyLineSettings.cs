// Assets/Scripts/Instances/SynergyLineSettings.cs
// Static settings class for synergy lines and segments.

using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Centralized tunables for synergy lines and segments.
/// Thin at both ends and thick in the middle. Transparency tuned for overlap.
/// Includes halo randomization ranges so segments do not pulse in lockstep.
/// </summary>
public static class SynergyLineSettings
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

    // Sorting
    public static readonly int OrderOffsetPerWave = 1;
    public static readonly int ExtraFrontBias = -2;

    // Color tint
    public static readonly float HueSpeed = 0.06f;
    public static readonly float HuePhase = 0.12f;
    public static readonly float HueRange = 0.06f;
    public static readonly float SatBase = 0.90f;
    public static readonly float SatRange = 0.08f;
    public static readonly float ValBase = 1.00f;
    public static readonly float ValPulseAmp = 0.08f;
    public static readonly float ValPulseSpeed = 0.70f;

    // Core alpha
    public static readonly float CoreAlpha = 0.55f;

    // Halo base
    public static readonly bool UseHalo = true;
    public static readonly float GlowWidthScale = 2.6f;
    public static readonly float GlowAlpha = 0.20f;
    public static readonly float GlowPulseAmp = 0.28f;
    public static readonly float GlowPulseSpeed = 0.90f;
    public static readonly float GlowHDRBoost = 1.35f;

    // Halo randomization
    // Set HaloRandomize to true to enable per-segment variation.
    public static readonly bool HaloRandomize = true;
    public static readonly Vector2 HaloWidthScaleRange = new Vector2(2.2f, 3.1f);
    public static readonly Vector2 HaloAlphaRange = new Vector2(0.14f, 0.26f);
    public static readonly Vector2 HaloPulseAmpRange = new Vector2(0.22f, 0.36f);
    // Multiplier applied to base pulse speed
    public static readonly Vector2 HaloPulseSpeedMultRange = new Vector2(0.75f, 1.25f);
    public static readonly Vector2 HaloHDRBoostRange = new Vector2(1.10f, 1.60f);
    // Extra phase offset so pulses start at different points
    public static readonly Vector2 HaloPhaseOffsetRange = new Vector2(0.0f, 6.283185f); // 0 to 2*pi

    // Geometry
    public static readonly int SegmentCount = 32;

    // Rev behavior for wiggles
    public static readonly float RevChancePerSecond = 0.12f;
    public static readonly float RevPeakMultiplier = 2.2f;
    public static readonly float RevAccelTime = 0.20f;
    public static readonly float RevDecelTime = 0.60f;
    public static readonly float RevCooldownMin = 0.60f;
    public static readonly float RevCooldownMax = 1.60f;

    public static float minT = 0.01f;
    public static float maxT = 0.08f;

    public static float minBaseSpeed = 0.2f;
    public static float maxBaseSpeed = 0.6f;
    public static float sparkleSpeedMulR = 1.0f; // Keep as multiplier

    public static float revActiveSpeedMul = 1.2f;

    public static float minSize = 0.10f;
    public static float maxSize = 0.14f;

    public static float minLifetime = 0.40f;
    public static float maxLifetime = 2f;

    public static float minOffsetJitter = -1f;
    public static float maxOffsetJitter = 1f;

}
