// Assets/Scripts/Instances/SynergyLineSettings.cs
// Static settings class for synergy lines and segments.

using UnityEngine;

/// <summary>
/// Centralized tunables for synergy lines and segments.
/// Thin at both ends and thick in the middle.
/// Transparency increased to reduce opacity buildup when overlapping.
/// </summary>
public static class SynergyLineSettings
{
    // Group
    public static readonly int WaveformCount = 5;
    public static readonly float BaseRadius = 0.07f;
    public static readonly float BaseWidth = 0.012f;
    public static readonly float Frequency = 2.2f;

    // Noise
    public static readonly float NoiseAmplitude = 0.015f;
    public static readonly float NoiseScale = 2.5f;
    public static readonly float NoiseSpeed = 0.18f;

    // Shape
    // Thin at t=0 and t=1, thickest at t=0.5
    public static readonly AnimationCurve RadiusOverT = new AnimationCurve(
        new Keyframe(0.00f, 0.15f, 0.0f, 0.0f),   // start thin
        new Keyframe(0.50f, 1.00f, 0.0f, 0.0f),   // middle thick
        new Keyframe(1.00f, 0.15f, 0.0f, 0.0f)    // end thin
    );

    // Fade
    public static readonly float FadeInTime = 0.20f;
    public static readonly float FadeOutTime = 0.30f;

    // Sorting
    public static readonly int OrderOffsetPerWave = 1;
    public static readonly int ExtraFrontBias = -2;

    // Tropical green tint
    public static readonly float HueSpeed = 0.06f;
    public static readonly float HuePhase = 0.12f;
    public static readonly float HueRange = 0.06f;
    public static readonly float SatBase = 0.90f;
    public static readonly float SatRange = 0.08f;
    public static readonly float ValBase = 1.00f;
    public static readonly float ValPulseAmp = 0.08f;
    public static readonly float ValPulseSpeed = 0.70f;

    // Halo
    public static readonly bool UseHalo = true;
    public static readonly float GlowWidthScale = 2.6f;
    public static readonly float GlowAlpha = 0.10f; // reduced from 0.32 for more transparency
    public static readonly float GlowPulseAmp = 0.28f;
    public static readonly float GlowPulseSpeed = 0.90f;
    public static readonly float GlowHDRBoost = 1.35f;

    // Core line opacity
    public static readonly float CoreAlpha = 0.20f; // added to allow more transparency in the main line

    // Geometry
    public static readonly int SegmentCount = 56;
}
