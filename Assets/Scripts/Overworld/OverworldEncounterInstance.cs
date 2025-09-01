using System;
using UnityEngine;
using Assets.Helpers;

/// <summary>
/// A roaming overworld encounter that wanders and gradually becomes interested in the hero
/// as they approach. When within trigger radius it starts a battle.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[DisallowMultipleComponent]
public sealed class OverworldEncounterInstance : MonoBehaviour
{
    // Wired by manager
    private OverworldEncounterManager manager;
    private OverworldHero hero;
    private SpriteRenderer terrainSR;
    private MapTerrain terrain;
    private Camera worldCamera;

    // Movement tuning
    [SerializeField] private float topSpeed = 2.0f;           // world units/sec (most are slower than hero ~2.5)
    [SerializeField] private float acceleration = 4.0f;       // units/sec^2
    [SerializeField] private float turnResponsiveness = 6.0f; // higher = snappier steering
    [SerializeField] private float wanderChangeInterval = 2.0f; // seconds between wander direction changes

    // Perception
    [SerializeField] private float visionRadius = 6.0f;          // start paying attention
    [SerializeField] private float pursuitRadius = 3.0f;         // close enough to strongly bias toward hero
    [SerializeField] private float triggerRadius = 0.6f;         // collide/trigger to start encounter

    // Obstacle avoidance
    [SerializeField] private float probeRadius = 0.18f;          // pass to terrain.IsWalkableLocal
    [SerializeField] private int probeRays = 8;

    // Runtime
    private Vector2 velocity;
    private Vector2 desiredDir;  // current steering goal
    private float wanderTimer;

    public void Setup(OverworldEncounterManager mgr, OverworldHero h, SpriteRenderer map, MapTerrain mapTerrain, Camera cam, float speedOverride = -1f)
    {
        manager = mgr;
        hero = h;
        terrainSR = map;
        terrain = mapTerrain;
        worldCamera = cam;
        if (speedOverride > 0f) topSpeed = speedOverride;

        // Ensure sorting so encounters are visible above terrain
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 90;
        }

        // Collider + rigidbody for trigger-based battles
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = triggerRadius;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Start with a random wander direction
        desiredDir = UnityEngine.Random.insideUnitCircle.normalized;
        wanderTimer = UnityEngine.Random.Range(0f, wanderChangeInterval);
    }

    private void Update()
    {
        if (hero == null || terrainSR == null || terrain == null) return;

        // 1) Wander steering target changes occasionally
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            wanderTimer = wanderChangeInterval + UnityEngine.Random.Range(-0.75f, 0.75f);
            var rand = (Vector2)UnityEngine.Random.insideUnitCircle;
            desiredDir = Vector2.Lerp(desiredDir, rand.normalized, 0.5f).normalized;
        }

        // 2) Attraction toward hero grows as distance shrinks
        Vector2 toHero = (Vector2)(hero.transform.position - transform.position);
        float d = toHero.magnitude;
        Vector2 heroBias = Vector2.zero;
        if (d < visionRadius)
        {
            // Attention ramps from 0 at visionRadius to 1 at pursuitRadius
            float t = Mathf.InverseLerp(visionRadius, pursuitRadius, Mathf.Max(pursuitRadius, d));
            float interest = 1f - t; // 0..1
            interest = interest * interest; // ease-in
            heroBias = toHero.normalized * interest;
        }

        // 3) Combine steering: wander plus hero bias
        Vector2 steerDir = (desiredDir + heroBias).normalized;

        // 4) Obstacle avoidance using MapTerrain normal
        var normal = terrain.EstimateObstacleNormal(transform.position);
        if (normal != Vector2.zero)
        {
            // If moving toward a wall, slide along it by removing component into the wall
            steerDir = (steerDir - Vector2.Dot(steerDir, normal) * normal).normalized;
        }

        // 5) Accelerate toward steering direction
        Vector2 targetVel = steerDir * topSpeed;
        velocity = Vector2.Lerp(velocity, targetVel, Mathf.Clamp01(Time.deltaTime * turnResponsiveness));

        // 6) Integrate and clamp to map bounds, ensure walkable
        Vector2 next = (Vector2)transform.position + velocity * Time.deltaTime;
        next = ClampToMap(next);
        if (!terrain.IsWalkableLocal(next, probeRadius, probeRays))
        {
            // Try a small slide along tangent if blocked
            var avoid = terrain.EstimateObstacleNormal(next);
            if (avoid != Vector2.zero)
            {
                var slide = (velocity - Vector2.Dot(velocity, avoid) * avoid);
                next = (Vector2)transform.position + slide * Time.deltaTime;
            }
        }

        transform.position = new Vector3(next.x, next.y, transform.position.z);

        // Optional: face direction (not animating yet)
        // transform.right = new Vector3(velocity.x, velocity.y, 0f);
    }

    private Vector2 ClampToMap(Vector2 p)
    {
        if (terrainSR == null) return p;
        var b = terrainSR.bounds;
        p.x = Mathf.Clamp(p.x, b.min.x, b.max.x);
        p.y = Mathf.Clamp(p.y, b.min.y, b.max.y);
        return p;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hero == null) return;
        if (other == null) return;
        if (other.GetComponent<OverworldHero>() == null) return;
        manager?.HandleEncounterTriggered(this, hero);
    }
}
