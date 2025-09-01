using System.Collections.Generic;
using UnityEngine;
using Assets.Helper;
using Assets.Helpers;
using scene = Assets.Helpers.SceneHelper;

/// <summary>
/// Spawns and updates roaming encounters on the overworld map.
/// Creates placeholder graphics that wander and gradually pursue the hero.
/// Uses 2D trigger colliders to start battles when close.
/// </summary>
[DisallowMultipleComponent]
public sealed class OverworldEncounterManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private int initialCount = 6;
    [SerializeField] private float minSpeed = 1.6f;
    [SerializeField] private float maxSpeed = 3.4f; // some faster than hero
    [SerializeField] private float minSpawnDistanceFromHero = 2.0f; // prevent instant trigger

    [Header("References")]
    [SerializeField] private OverworldHero hero; // auto-bound in Awake
    [SerializeField] private SpriteRenderer terrainSR;
    [SerializeField] private MapTerrain terrain;
    [SerializeField] private Camera worldCamera;

    private Transform mapRoot;
    private readonly List<OverworldEncounterInstance> active = new List<OverworldEncounterInstance>();
    private bool started;
    private float enableTriggersAt; // grace time before encounters can trigger battles

    private void Awake()
    {
        // Auto-bind: Map root
        var mapGo = GameObject.Find(GameObjectHelper.Overworld.Map.Root);
        mapRoot = mapGo != null ? mapGo.transform : null;

        // Auto-bind: Terrain SR + MapTerrain
        var terrainGo = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain);
        if (terrainGo != null)
        {
            terrainSR = terrainGo.GetComponent<SpriteRenderer>() ?? terrainSR;
            if (terrainSR != null)
            {
                terrain = terrainSR.GetComponent<MapTerrain>() ?? terrainSR.gameObject.AddComponent<MapTerrain>();
                terrain.ForceRefresh();
            }
        }

        // Auto-bind: Hero
        var heroGo = GameObject.Find(GameObjectHelper.Overworld.Map.Hero);
        if (heroGo != null)
            hero = heroGo.GetComponent<OverworldHero>() ?? hero;

        // Auto-bind: Camera
        worldCamera = Camera.main ?? worldCamera;

        TryStart();
    }

    // Back-compat: if someone still calls Initialize(), keep it safe and idempotent.
    public void Initialize(OverworldHero h, SpriteRenderer map, MapTerrain mapTerrain, Camera cam)
    {
        if (started) return;

        hero = h ?? hero;
        terrainSR = map ?? terrainSR;
        terrain = mapTerrain ?? terrain;
        worldCamera = cam ?? worldCamera;
        if (terrainSR != null && terrain == null)
            terrain = terrainSR.GetComponent<MapTerrain>() ?? terrainSR.gameObject.AddComponent<MapTerrain>();

        if (mapRoot == null)
        {
            var mapGo = GameObject.Find(GameObjectHelper.Overworld.Map.Root);
            mapRoot = mapGo != null ? mapGo.transform : (terrainSR != null ? terrainSR.transform.parent : null);
        }

        TryStart();
    }

    private void TryStart()
    {
        if (started) return;

        if (hero == null || terrainSR == null || terrain == null)
        {
            Debug.LogWarning("[OverworldEncounterManager] Missing references (Hero/TerrainSR/MapTerrain). Manager will idle until scene is ready.");
            return;
        }

        // Ensure hero has collider/rigidbody for 2D trigger interaction
        EnsureHeroPhysics2D(hero);

        // Spawn initial encounters spread around the map using prefab
        for (int i = 0; i < initialCount; i++)
        {
            var pos = RandomSpawnPosition();

            var prefab = PrefabLibrary.Get("OverworldEncounterInstancePrefab");
            if (prefab == null)
            {
                Debug.LogError("OverworldEncounterInstancePrefab not found in PrefabLibrary. Skipping spawn.");
                continue;
            }

            var go = Instantiate(prefab, pos, Quaternion.identity, mapRoot);

            var inst = go.GetComponent<OverworldEncounterInstance>();
            if (inst == null)
            {
                Debug.LogError("OverworldEncounterInstancePrefab is missing OverworldEncounterInstance component. Skipping spawn.");
                Destroy(go);
                continue;
            }

            float speed = Random.Range(minSpeed, maxSpeed);
            inst.Setup(this, hero, terrainSR, terrain, worldCamera, speed);
            active.Add(inst);
        }

        // Small grace period to allow scene fade-in before a trigger can transition
        enableTriggersAt = Time.time + 1.0f;

        started = true;
    }

    private static void EnsureHeroPhysics2D(OverworldHero h)
    {
        if (h == null) return;

        var hc = h.GetComponent<CircleCollider2D>();
        if (hc == null)
        {
            hc = h.gameObject.AddComponent<CircleCollider2D>();
            hc.isTrigger = false; // solid hero, triggers live on encounters
            hc.radius = 0.35f;
        }

        var rb = h.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = h.gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    private Vector3 RandomSpawnPosition()
    {
        if (terrainSR == null) return Vector3.zero;
        var b = terrainSR.bounds;

        // Try a generous number of random samples within terrain bounds
        const int maxAttempts = 500;
        for (int i = 0; i < maxAttempts; i++)
        {
            var p = new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));

            // Must be walkable (i.e., not in black areas)
            if (terrain != null && !terrain.IsWalkableLocal(p))
                continue;

            // Keep a safe distance from the hero
            if (hero != null && Vector2.Distance(p, hero.transform.position) < minSpawnDistanceFromHero)
                continue;

            return new Vector3(p.x, p.y, 0f);
        }

        // Fallback: search around the hero with random offsets until a valid spot is found
        if (hero != null)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                var offset = Random.insideUnitCircle.normalized * Mathf.Max(1f, minSpawnDistanceFromHero + Random.Range(0f, 3f));
                var near = (Vector2)hero.transform.position + offset;

                // Clamp to map bounds
                near.x = Mathf.Clamp(near.x, b.min.x, b.max.x);
                near.y = Mathf.Clamp(near.y, b.min.y, b.max.y);

                if (terrain != null && !terrain.IsWalkableLocal(near))
                    continue;

                if (Vector2.Distance(near, hero.transform.position) < minSpawnDistanceFromHero)
                    continue;

                return new Vector3(near.x, near.y, 0f);
            }
        }

        // As a last resort, return the hero's position (manager will likely skip immediate triggers via grace period)
        return hero != null ? hero.transform.position : Vector3.zero;
    }

    // Called by an instance when it touches the hero
    public void HandleEncounterTriggered(OverworldEncounterInstance instance, OverworldHero h)
    {
        if (instance == null || hero == null) return;
        if (Time.time < enableTriggersAt) return; // still in grace period

        // Persist overworld location and facing
        ProfileHelper.SaveOverworldPosition(
            new Vector2(hero.transform.position.x, hero.transform.position.y),
            ProfileHelper.Overworld.MapName,
            hero.CurrentFacingName
        );

        // Choose a random stage for test
        string stageName = RNG.Stage(ProfileHelper.Overworld.MapName);
        ProfileHelper.CurrentProfile.LatestSave.Stage.CurrentStage = stageName;

        // Optionally despawn the instance that triggered
        active.Remove(instance);
        if (instance != null) Destroy(instance.gameObject);

        scene.Change.ToGame();
    }
}
