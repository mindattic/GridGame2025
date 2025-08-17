// Handles spark particles that travel along a Synergy line path.
// The strand supplies path samplers so sparks line up exactly.

using System;
using System.Collections.Generic;
using UnityEngine;

public class SynergySpark
{
    /// <summary>
    /// Tunables scoped only to the spark system.
    /// </summary>
    private static class T
    {
        public static float minT = 0.01f;
        public static float maxT = 0.08f;

        public static float minBaseSpeed = 0.2f;
        public static float maxBaseSpeed = 0.6f;

        public static float minSize = 0.10f;
        public static float maxSize = 0.16f;

        public static float minLifetime = 0.40f;
        public static float maxLifetime = 2.0f;

        public static float minOffsetJitter = -1f;
        public static float maxOffsetJitter = 1f;

        // Multiplier knobs
        public static float speedMulR_defaultMin = 0.85f;
        public static float speedMulR_defaultMax = 1.35f;
        public static float revActiveSpeedMul = 1.2f;

        // Spawn rate randomization
        public static float spawnRateMin = 10f;
        public static float spawnRateMax = 16f;

        // Default sprite key
        public static string textureKey = "SynergySpark";
    }

    // Particle system objects
    private ParticleSystem sparks;
    private ParticleSystemRenderer sparksRenderer;

    /// <summary>
    /// Spark instance as a reference type for in-place edits.
    /// </summary>
    public class SynergyLineSpark
    {
        public float t;
        public float speed;
        public float size;
        public float age;
        public float lifetime;
        public float offsetJitter;
    }

    private readonly List<SynergyLineSpark> active = new List<SynergyLineSpark>(64);
    private ParticleSystem.Particle[] particleBuffer = new ParticleSystem.Particle[64];
    private float spawnAccum;
    private float spawnRateR;
    private float speedMulR;

    /// <summary>
    /// Create child particle system under parent and set defaults.
    /// </summary>
    public void Init(Transform parent, string spriteKeyOverride = null)
    {
        var sparkGO = new GameObject("Sparks");
        sparkGO.transform.SetParent(parent, false);

        sparks = sparkGO.AddComponent<ParticleSystem>();
        sparksRenderer = sparkGO.GetComponent<ParticleSystemRenderer>();

        var shader = Shader.Find("Particles/Additive");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        var mat = new Material(shader);

        string key = string.IsNullOrEmpty(spriteKeyOverride) ? T.textureKey : spriteKeyOverride;
        if (SpriteLibrary.Sprites != null && SpriteLibrary.Sprites.ContainsKey(key))
        {
            mat.mainTexture = SpriteLibrary.Sprites[key].texture;
        }

        sparksRenderer.material = mat;
        sparksRenderer.renderMode = ParticleSystemRenderMode.Billboard;

        var main = sparks.main;
        main.playOnAwake = true;
        main.loop = true;
        main.prewarm = true; // prewarm particle system
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 1024;
        main.startSpeed = 0f;
        main.startLifetime = 1f;
        main.startSize = 0.12f;

        var emission = sparks.emission;
        emission.enabled = false;

        var shape = sparks.shape;
        shape.enabled = false;

        var col = sparks.colorOverLifetime;
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

        var sizeOL = sparks.sizeOverLifetime;
        sizeOL.enabled = true;
        sizeOL.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0.00f, 0.2f),
                new Keyframe(0.35f, 1.0f),
                new Keyframe(1.00f, 0.0f)
            )
        );

        spawnRateR = RNG.Float(T.spawnRateMin, T.spawnRateMax);
        speedMulR = RNG.Float(T.speedMulR_defaultMin, T.speedMulR_defaultMax);

        sparks.Play(true);
    }

    /// <summary>
    /// Set renderer sorting layer and order.
    /// </summary>
    public void SetSorting(string sortingLayer, int order)
    {
        if (sparksRenderer == null) return;
        sparksRenderer.sortingLayerName = sortingLayer;
        sparksRenderer.sortingOrder = order;
    }

    /// <summary>
    /// Set the base tint used for sparks.
    /// </summary>
    public void SetTint(Color tint)
    {
        if (sparks == null) return;
        var main = sparks.main;
        main.startColor = new ParticleSystem.MinMaxGradient(tint);
    }

    /// <summary>
    /// Advance simulation, spawn new sparks, and write particle buffer.
    /// The strand provides the exact path and radius samplers so sparks align perfectly.
    /// </summary>
    public void Tick(
        float fade,
        bool revActive,
        Func<float, Vector3> samplePos,
        Func<float, float> radiusAtT,
        float dt)
    {
        if (sparks == null) return;

        float spawnRate = spawnRateR * Mathf.Clamp01(fade);
        spawnAccum += spawnRate * dt;
        while (spawnAccum >= 1f)
        {
            spawnAccum -= 1f;
            Spawn(revActive);
        }

        UpdateActive(samplePos, radiusAtT, dt);

        if (!sparks.isPlaying) sparks.Play(true);
    }

    /// <summary>
    /// Immediately simulate N steps so sparks appear warmed up.
    /// </summary>
    public void Prewarm(
        float seconds,
        int steps,
        bool revActive,
        Func<float, Vector3> samplePos,
        Func<float, float> radiusAtT)
    {
        if (steps <= 0 || seconds <= 0f) return;
        float dt = seconds / steps;
        for (int i = 0; i < steps; i++)
        {
            Tick(1f, revActive, samplePos, radiusAtT, dt);
        }
    }

    /// <summary>
    /// Clear particles and active list.
    /// </summary>
    public void Clear()
    {
        active.Clear();
        if (sparks != null) sparks.Clear();
    }

    // Internal spawn of a single spark
    private void Spawn(bool revActive)
    {
        var s = new SynergyLineSpark();
        s.t = RNG.Float(T.minT, T.maxT);

        float baseSpeed = RNG.Float(T.minBaseSpeed, T.maxBaseSpeed) * speedMulR;
        s.speed = baseSpeed * (revActive ? T.revActiveSpeedMul : 1.0f);

        s.size = RNG.Float(T.minSize, T.maxSize);

        float travelT = 1f - s.t;
        float timeNeeded = travelT / Mathf.Max(0.001f, s.speed);
        float padding = timeNeeded * RNG.Float(0.10f, 0.35f);
        s.lifetime = Mathf.Clamp(timeNeeded + padding, T.minLifetime, T.maxLifetime);

        s.age = 0f;
        s.offsetJitter = RNG.Float(T.minOffsetJitter, T.maxOffsetJitter);

        active.Add(s);
    }

    // Internal update for all active sparks and particle system write
    private void UpdateActive(
        Func<float, Vector3> samplePos,
        Func<float, float> radiusAtT,
        float dt)
    {
        if (active.Count == 0)
        {
            if (sparks != null) sparks.Clear();
            return;
        }

        if (particleBuffer.Length < active.Count)
            particleBuffer = new ParticleSystem.Particle[Mathf.NextPowerOfTwo(active.Count)];

        int alive = 0;
        var tint = sparks.main.startColor.color;

        for (int i = 0; i < active.Count; i++)
        {
            SynergyLineSpark s = active[i];
            s.age += dt;
            s.t += s.speed * dt;

            if (s.t >= 1f || s.age >= s.lifetime)
                continue;

            // Sample exact strand position
            Vector3 p = samplePos(s.t);

            // Estimate tangent and perpendicular from the same path to keep jitter aligned
            float tPrev = Mathf.Max(0f, s.t - 0.01f);
            float tNext = Mathf.Min(1f, s.t + 0.01f);
            Vector3 tangent = samplePos(tNext) - samplePos(tPrev);
            if (tangent.sqrMagnitude < 1e-6f) tangent = Vector3.right;

            Vector3 perp = new Vector3(-tangent.y, tangent.x, 0f).normalized;
            float rAtT = Mathf.Max(0f, radiusAtT(s.t));
            p += perp * (s.offsetJitter * 0.12f * rAtT);

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
        }

        // Compact dead sparks
        if (alive < active.Count)
        {
            int write = 0;
            for (int read = 0; read < active.Count; read++)
            {
                var s = active[read];
                if (s.t < 1f && s.age < s.lifetime)
                    active[write++] = s;
            }
            if (write < active.Count)
                active.RemoveRange(write, active.Count - write);
        }

        sparks.SetParticles(particleBuffer, alive);
    }
}
