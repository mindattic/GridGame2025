// Assets/Scripts/Instances/SynergyLineStrand.cs

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SynergyLineStrand : MonoBehaviour
{
    private LineRenderer line;
    private LineRenderer glow;

    // Pinned endpoints (world-space) captured during Configure
    private Vector3 a;
    private Vector3 b;

    // Params (unchanged from your version—trimmed for brevity)
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

    // … (tint/halo/particles fields as you already had) …

    private ParticleSystem sparkles;
    private ParticleSystemRenderer sparklesRenderer;

    private int strandCount;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line == null) line = gameObject.AddComponent<LineRenderer>();
        if (line.material != null) line.material = new Material(line.material);

        SetupLineRenderer(line);

        // Optional glow (keep if you use it)
        var glowGO = new GameObject("Glow");
        glowGO.transform.SetParent(transform, false);
        glow = glowGO.AddComponent<LineRenderer>();
        glow.material = line.material != null ? new Material(line.material) : null;
        SetupLineRenderer(glow);

        // Particles
        var sparkGO = new GameObject("Spark");
        sparkGO.transform.SetParent(transform, false);
        sparkles = sparkGO.AddComponent<ParticleSystem>();
        sparklesRenderer = sparkles.GetComponent<ParticleSystemRenderer>();

        var shader = Shader.Find("Particles/Additive");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        var mat = new Material(shader);
        mat.mainTexture = SpriteLibrary.Sprites["SynergySpark"].texture;

        sparklesRenderer.material = mat;
        sparklesRenderer.renderMode = ParticleSystemRenderMode.Billboard;

        var main = sparkles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World; // IMPORTANT: world-space particles
        main.playOnAwake = true;
        main.loop = true;
        main.maxParticles = 1024;
        main.startSpeed = 0f;
        main.startLifetime = 1f;
        main.startSize = 0.12f;
        main.prewarm = true;

        // Disable emission/shape modules (we drive particles manually)
        //sparkles.emission.enabled = false;
        //sparkles.shape.enabled = false;
    }

    private void SetupLineRenderer(LineRenderer lr)
    {
        lr.useWorldSpace = true;                     // IMPORTANT: world-space line
        lr.textureMode = LineTextureMode.Stretch;
        lr.alignment = LineAlignment.View;
        lr.numCornerVertices = 3;
        lr.numCapVertices = 3;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
    }

    public void Configure(
        Vector3 aWorld,
        Vector3 bWorld,
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
        // PIN ONCE
        a = aWorld;
        b = bWorld;

        widthAbs = Mathf.Max(0.0025f, widthAbsolute);
        radiusAbs = Mathf.Max(0.01f, radiusAbsolute);
        phaseOffset = phase;

        frequency = Mathf.Max(0.1f, inFrequency);
        noiseAmplitude = Mathf.Max(0f, inNoiseAmplitude);
        noiseScale = Mathf.Max(0.001f, inNoiseScale);
        noiseSpeed = Mathf.Max(0f, inNoiseSpeed);
        radiusOverT = inRadiusOverT ?? AnimationCurve.Linear(0f, 1f, 1f, 1f);

        // … (copy your tint/halo params as-is) …

        strandCount = Mathf.Max(2, inSegmentCount);
        line.positionCount = strandCount;
        glow.positionCount = strandCount;

        line.sortingLayerName = sortingLayer;
        line.sortingOrder = sortingOrder;
        glow.sortingLayerName = sortingLayer;
        glow.sortingOrder = sortingOrder - 1;

        if (sparklesRenderer != null)
        {
            sparklesRenderer.sortingLayerName = sortingLayer;
            sparklesRenderer.sortingOrder = sortingOrder + 2;
        }

        configured = true;
    }

    public void SetFade(float k)
    {
        fade = Mathf.Clamp01(k);
        line.widthMultiplier = widthAbs;
    }

    public void Tick()
    {
        if (!configured) return;

        // Use the pinned world-space anchors
        Vector3 start = a; start.z = 0f;
        Vector3 end = b; end.z = 0f;

        Vector3 dir = end - start;
        float len = dir.magnitude;

        if (len < 0.0001f)
        {
            if (line.positionCount != 2) line.positionCount = 2;
            if (glow.positionCount != 2) glow.positionCount = 2;
            line.SetPosition(0, start); line.SetPosition(1, start);
            glow.SetPosition(0, start); glow.SetPosition(1, start);
            sparkles.Clear();
            return;
        }

        Vector3 forward = dir / len;
        Vector3 perp = new Vector3(-forward.y, forward.x, 0f);

        if (line.positionCount != strandCount) line.positionCount = strandCount;
        if (glow.positionCount != strandCount) glow.positionCount = strandCount;

        float twoPi = Mathf.PI * 2f;
        float envelope = 1f; // keep envelope stable (no actor-driven jiggle)

        for (int i = 0; i < strandCount; i++)
        {
            float t = i / (float)(strandCount - 1);
            Vector3 p = EvaluatePathPoint(start, end, perp, envelope, twoPi, t);
            line.SetPosition(i, p);
            glow.SetPosition(i, p);
        }

        // (Optional) update particles in world space
        // UpdateSparks(start, end, perp, envelope, twoPi);
    }

    public void Clear()
    {
        if (line != null) line.positionCount = 0;
        if (glow != null) glow.positionCount = 0;
        if (sparkles != null) sparkles.Clear();
        configured = false;
    }

    public void SetSorting(string sortingLayer, int sortingOrder)
    {
        if (line != null)
        {
            line.sortingLayerName = sortingLayer;
            line.sortingOrder = sortingOrder;
        }
        if (glow != null)
        {
            glow.sortingLayerName = sortingLayer;
            glow.sortingOrder = sortingOrder - 1;
        }
        if (sparklesRenderer != null)
        {
            sparklesRenderer.sortingLayerName = sortingLayer;
            sparklesRenderer.sortingOrder = sortingOrder + 2;
        }
    }

    private Vector3 EvaluatePathPoint(Vector3 start, Vector3 end, Vector3 perp, float envelope, float twoPi, float t)
    {
        float r = radiusAbs * radiusOverT.Evaluate(t) * envelope;
        float sin = Mathf.Sin(twoPi * frequency * t + phaseOffset);
        // keep noise time-independent of actor motion
        float n = (Mathf.PerlinNoise(t * noiseScale, 0f) - 0.5f) * 2f * noiseAmplitude;

        Vector3 basePos = Vector3.Lerp(start, end, t);
        Vector3 offset = perp * ((sin + n) * r);
        return basePos + offset;
    }
}
