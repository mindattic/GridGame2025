// Waveform strand with sine + Perlin jitter, halo, alpha control, rev bursts,
// halo desync, and sparkles that travel along the line and despawn at the end.
// All random values use RNG instead of UnityEngine.Random.

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class SynergyLineSegment : MonoBehaviour
{
    // Renderers
    private LineRenderer line;
    private LineRenderer glow;

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

    // Tropical tint state
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

    // Base green in HSV
    private static readonly Color BaseGreenRGB = new Color(0.25f, 1.00f, 0.25f);
    private static float BaseHue = 0.42f;
    private static float BaseSat = 0.9f;
    private static float BaseVal = 1.0f;
    private static bool BaseHSVInit = false;

    // Halo controls (inputs)
    private bool useHalo;
    private float glowWidthScale;
    private float glowAlpha;
    private float glowPulseAmp;
    private float glowPulseSpeed;
    private float glowHDRBoost;

    // Halo randomized instance values
    private float glowWidthScaleR;
    private float glowAlphaR;
    private float glowPulseAmpR;
    private float glowPulseSpeedR;
    private float glowHDRBoostR;
    private float glowPhaseOffsetR;

    // Derived halo rate
    private float glowAlphaPulseSpeed;

    // Geometry
    private int segmentCount;

    // Shader property ids
    private static int idBaseColor = -1;
    private static int idColor = -1;

    // Rev state
    private float pathTime;
    private bool revActive;
    private float revElapsed;
    private float revCooldown;

    // Sparkles that move along the path
    private ParticleSystem sparkles;
    private ParticleSystemRenderer sparklesRenderer;

    // Sparkle runtime data
    private struct Sparkle
    {
        public float t;
        public float speed;
        public float size;
        public float age;
        public float lifetime;
        public float offsetJitter;
    }

    private readonly List<Sparkle> activeSparkles = new List<Sparkle>(64);
    private ParticleSystem.Particle[] particleBuffer = new ParticleSystem.Particle[64];
    private float sparkleSpawnAccum;
    private float sparkleRateR;
    private float sparkleSpeedMulR;

    /// <summary>
    /// Sets up renderers, shader ids, HSV base, rev cooldown, and sparkle system.
    /// Initializes all random seeds and per instance parameters using RNG.
    /// </summary>
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

        // Sparkles system: manual SetParticles with additive material and runtime texture from TextureRepo
        var sparkleGO = new GameObject("Sparkles");
        sparkleGO.transform.SetParent(transform, false);
        sparkles = sparkleGO.AddComponent<ParticleSystem>();
        sparklesRenderer = sparkles.GetComponent<ParticleSystemRenderer>();

        var shader = Shader.Find("Particles/Additive");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        var mat = new Material(shader);

        mat.mainTexture = SpriteLibrary.Sprites["SynergySpark"].texture;

        sparklesRenderer.material = mat;
        sparklesRenderer.renderMode = ParticleSystemRenderMode.Billboard;

        var main = sparkles.main;
        main.playOnAwake = true;
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 1024;
        main.startSpeed = 0f;
        main.startLifetime = 1f;
        main.startSize = 0.12f;

        var emission = sparkles.emission;
        emission.enabled = false;

        var shape = sparkles.shape;
        shape.enabled = false;

        // Fade and size over lifetime so sparkles feel like glints
        var col = sparkles.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.0f, 0f),
                new GradientAlphaKey(0.9f, 0.35f),
                new GradientAlphaKey(0.0f, 1f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var sizeOL = sparkles.sizeOverLifetime;
        sizeOL.enabled = true;
        sizeOL.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0.00f, 0.2f),
                new Keyframe(0.35f, 1.0f),
                new Keyframe(1.00f, 0.0f)
            )
        );

        sparkles.Play(true);

        noiseSeed = RNG.Float(0f, 1000f);
        pathTime = RNG.Float(0f, 1000f);

        if (idBaseColor == -1) idBaseColor = Shader.PropertyToID("_BaseColor");
        if (idColor == -1) idColor = Shader.PropertyToID("_Color");

        if (!BaseHSVInit)
        {
            Color.RGBToHSV(BaseGreenRGB, out BaseHue, out BaseSat, out BaseVal);
            BaseHSVInit = true;
        }

        revCooldown = RNG.Float(SynergyLineSettings.RevCooldownMin, SynergyLineSettings.RevCooldownMax);

        sparkleRateR = RNG.Float(10f, 16f);
        sparkleSpeedMulR = RNG.Float(0.85f, 1.35f);
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
    /// Configure geometry, color params, sorting, halo, and resolution. Randomizes halo per segment if enabled.
    /// Also assigns sparkle renderer sorting to sit above the core line.
    /// Uses RNG for all per instance randomization.
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

        if (SynergyLineSettings.HaloRandomize)
        {
            glowWidthScaleR = RNG.Float(SynergyLineSettings.HaloWidthScaleRange.x, SynergyLineSettings.HaloWidthScaleRange.y);
            glowAlphaR = RNG.Float(SynergyLineSettings.HaloAlphaRange.x, SynergyLineSettings.HaloAlphaRange.y);
            glowPulseAmpR = RNG.Float(SynergyLineSettings.HaloPulseAmpRange.x, SynergyLineSettings.HaloPulseAmpRange.y);
            float speedMult = RNG.Float(SynergyLineSettings.HaloPulseSpeedMultRange.x, SynergyLineSettings.HaloPulseSpeedMultRange.y);
            glowPulseSpeedR = glowPulseSpeed * speedMult;
            glowHDRBoostR = RNG.Float(SynergyLineSettings.HaloHDRBoostRange.x, SynergyLineSettings.HaloHDRBoostRange.y);
            glowPhaseOffsetR = RNG.Float(SynergyLineSettings.HaloPhaseOffsetRange.x, SynergyLineSettings.HaloPhaseOffsetRange.y);
        }
        else
        {
            glowWidthScaleR = glowWidthScale;
            glowAlphaR = glowAlpha;
            glowPulseAmpR = glowPulseAmp;
            glowPulseSpeedR = glowPulseSpeed;
            glowHDRBoostR = glowHDRBoost;
            glowPhaseOffsetR = 0f;
        }

        glowAlphaPulseSpeed = glowPulseSpeedR * 1.3f;

        segmentCount = Mathf.Max(2, inSegmentCount);
        line.positionCount = segmentCount;
        glow.positionCount = segmentCount;

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

    /// <summary>
    /// External fade and pins the core width.
    /// </summary>
    public void SetFade(float k)
    {
        fade = Mathf.Clamp01(k);
        line.widthMultiplier = widthAbs;
    }

    /// <summary>
    /// Per frame update of color, halo, geometry, rev motion, and sparkles.
    /// </summary>
    public void Tick()
    {
        if (!configured || a == null || b == null) return;

        Color greenTint = ComputeTintedGreen();

        Color core = greenTint;
        core.a *= fade * SynergyLineSettings.CoreAlpha;

        Color haloC = greenTint;
        float alphaPulse = 0.65f + 0.35f * Mathf.Sin(Time.time * glowAlphaPulseSpeed + strandIndex + glowPhaseOffsetR);
        haloC.a = Mathf.Clamp01(glowAlphaR * alphaPulse * Mathf.Pow(fade, 0.9f));
        Color haloHDR = haloC * glowHDRBoostR;

        ApplyColor(line, core);
        if (useHalo) ApplyColor(glow, haloHDR);

        var smain = sparkles.main;
        var sparkleTint = greenTint * 1.35f;
        sparkleTint.a = 0.9f;
        smain.startColor = new ParticleSystem.MinMaxGradient(sparkleTint);

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

            activeSparkles.Clear();
            sparkles.Clear();
            return;
        }

        Vector3 forward = dir / len;
        Vector3 perp = new Vector3(-forward.y, forward.x, 0f);

        if (line.positionCount != segmentCount) line.positionCount = segmentCount;
        if (glow.positionCount != segmentCount) glow.positionCount = segmentCount;

        float twoPi = Mathf.PI * 2f;

        float envelope = 1f + Mathf.Sin(Time.time * 0.35f + phaseOffset * 0.7f) * 0.35f;

        if (useHalo)
        {
            float haloWidthPulse = 1f + Mathf.Sin(Time.time * glowPulseSpeedR + strandIndex + glowPhaseOffsetR) * glowPulseAmpR;
            glow.widthMultiplier = widthAbs * glowWidthScaleR * haloWidthPulse;
        }

        float timeWarp = UpdateRevAndGetTimeWarp();
        pathTime += Time.deltaTime * timeWarp;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 p = EvaluatePathPoint(start, end, perp, envelope, twoPi, t);
            line.SetPosition(i, p);
            glow.SetPosition(i, p);
        }

        float spawnRate = sparkleRateR * Mathf.Clamp01(fade);
        sparkleSpawnAccum += spawnRate * Time.deltaTime;
        while (sparkleSpawnAccum >= 1f)
        {
            sparkleSpawnAccum -= 1f;
            SpawnSparkle();
        }

        UpdateSparkles(start, end, perp, envelope, twoPi);
    }

    /// <summary>
    /// Clear renderers when despawning.
    /// </summary>
    public void Clear()
    {
        if (line != null) line.positionCount = 0;
        if (glow != null) glow.positionCount = 0;
        activeSparkles.Clear();
        if (sparkles != null) sparkles.Clear();
        configured = false;
    }

    /// <summary>
    /// Apply a color to the LineRenderer and URP material if present.
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
    /// Tropical tint color over time.
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

    /// <summary>
    /// Time warp for rev burst. Starts bursts using RNG and resets cooldown with RNG.
    /// </summary>
    private float UpdateRevAndGetTimeWarp()
    {
        if (!revActive)
        {
            revCooldown -= Time.deltaTime;
            if (revCooldown <= 0f)
            {
                if (RNG.Percent < SynergyLineSettings.RevChancePerSecond * Time.deltaTime)
                {
                    revActive = true;
                    revElapsed = 0f;
                }
            }
        }

        if (!revActive) return 1f;

        revElapsed += Time.deltaTime;

        float acc = Mathf.Max(0.0001f, SynergyLineSettings.RevAccelTime);
        float dec = Mathf.Max(0.0001f, SynergyLineSettings.RevDecelTime);
        float total = acc + dec;

        float k;
        if (revElapsed <= acc)
        {
            float u = Mathf.Clamp01(revElapsed / acc);
            k = Mathf.SmoothStep(0f, 1f, u);
        }
        else if (revElapsed <= total)
        {
            float u = Mathf.Clamp01((revElapsed - acc) / dec);
            k = 1f - Mathf.SmoothStep(0f, 1f, u);
        }
        else
        {
            revActive = false;
            revCooldown = RNG.Float(SynergyLineSettings.RevCooldownMin, SynergyLineSettings.RevCooldownMax);
            return 1f;
        }

        float peak = Mathf.Max(1f, SynergyLineSettings.RevPeakMultiplier);
        return 1f + (peak - 1f) * k;
    }

    /// <summary>
    /// Evaluate a single path point at parameter t.
    /// </summary>
    private Vector3 EvaluatePathPoint(Vector3 start, Vector3 end, Vector3 perp, float envelope, float twoPi, float t)
    {
        float radius = radiusAbs * radiusOverT.Evaluate(t) * envelope;

        float sin = Mathf.Sin(twoPi * frequency * t + phaseOffset + pathTime * 0.18f);
        float n = (Mathf.PerlinNoise(noiseSeed + t * noiseScale, pathTime * noiseSpeed) - 0.5f) * 2f;

        Vector3 basePos = Vector3.Lerp(start, end, t);
        Vector3 offset = perp * ((sin + n) * radius);
        return basePos + offset;
    }

    /// <summary>
    /// Create a new sparkle near the start. All random draws use RNG.
    /// Lifetime scales with remaining t and speed so the sparkle can reach the end.
    /// </summary>
    private void SpawnSparkle()
    {
        Sparkle s;
        s.t = RNG.Float(SynergyLineSettings.minT, SynergyLineSettings.maxT);

        float baseSpeed = RNG.Float(SynergyLineSettings.minBaseSpeed, SynergyLineSettings.maxBaseSpeed)
                          * SynergyLineSettings.sparkleSpeedMulR;
        s.speed = baseSpeed * (revActive ? SynergyLineSettings.revActiveSpeedMul : 1.0f);

        s.size = RNG.Float(SynergyLineSettings.minSize, SynergyLineSettings.maxSize);

        float travelT = 1f - s.t;
        float timeNeeded = travelT / Mathf.Max(0.001f, s.speed);
        float padding = timeNeeded * RNG.Float(0.10f, 0.35f);
        s.lifetime = Mathf.Clamp(timeNeeded + padding,
                                 SynergyLineSettings.minLifetime,
                                 SynergyLineSettings.maxLifetime);

        s.age = 0f;
        s.offsetJitter = RNG.Float(SynergyLineSettings.minOffsetJitter, SynergyLineSettings.maxOffsetJitter);

        activeSparkles.Add(s);
    }

    /// <summary>
    /// Advance sparkles along the curve and cull finished ones. Writes positions to the particle system.
    /// </summary>
    private void UpdateSparkles(Vector3 start, Vector3 end, Vector3 perp, float envelope, float twoPi)
    {
        if (activeSparkles.Count == 0)
        {
            sparkles.Clear();
            return;
        }

        if (particleBuffer.Length < activeSparkles.Count)
            particleBuffer = new ParticleSystem.Particle[Mathf.NextPowerOfTwo(activeSparkles.Count)];

        int alive = 0;
        float dt = Time.deltaTime;
        var tint = sparkles.main.startColor.color;

        for (int i = 0; i < activeSparkles.Count; i++)
        {
            Sparkle s = activeSparkles[i];
            s.age += dt;
            s.t += s.speed * dt;

            if (s.t >= 1f || s.age >= s.lifetime)
                continue;

            Vector3 p = EvaluatePathPoint(start, end, perp, envelope, twoPi, s.t);

            float radiusAtT = radiusAbs * radiusOverT.Evaluate(s.t) * envelope;
            p += perp * (s.offsetJitter * 0.12f * radiusAtT);

            ParticleSystem.Particle pp = new ParticleSystem.Particle();
            pp.position = p;
            pp.remainingLifetime = Mathf.Max(0.01f, s.lifetime - s.age);
            pp.startLifetime = s.lifetime;
            pp.startSize = s.size;
            pp.startColor = tint;
            pp.velocity = Vector3.zero;
            pp.rotation3D = Vector3.zero;

            particleBuffer[alive] = pp;
            alive++;

            activeSparkles[i] = s;
        }

        if (alive < activeSparkles.Count)
        {
            int write = 0;
            for (int read = 0; read < activeSparkles.Count; read++)
            {
                var s = activeSparkles[read];
                if (s.t < 1f && s.age < s.lifetime)
                    activeSparkles[write++] = s;
            }
            if (write < activeSparkles.Count)
                activeSparkles.RemoveRange(write, activeSparkles.Count - write);
        }

        sparkles.SetParticles(particleBuffer, alive);
        if (!sparkles.isPlaying) sparkles.Play(true);
    }

    /// <summary>
    /// Change only the sorting layer, preserving per-strand relative order.
    /// Used by the manager to flip between below and above actor layers.
    /// </summary>
    public void SetSortingLayer(string sortingLayer)
    {
        if (line != null) line.sortingLayerName = sortingLayer;
        if (glow != null) glow.sortingLayerName = sortingLayer;
        if (sparklesRenderer != null) sparklesRenderer.sortingLayerName = sortingLayer;
    }
}
