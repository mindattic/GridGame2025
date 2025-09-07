using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class TreeInstance : MonoBehaviour
{
    [Header("Sorting")]
    [Tooltip("Match hero's sorting layer and go behind when hero is below, in front when above.")]
    public bool followHeroSorting = true;

    [Header("Idle Sway (Grass-style)")]
    [Tooltip("Enable gentle sway when there is no collision with the hero.")]
    public bool enableIdleSway = true;
    [Tooltip("Rest X-rotation when plant is normal (degrees, negative tilts toward camera).")]
    [Range(-80f, -5f)] public float foldAngleX = -45f; // normal state
    [Tooltip("Sway amplitude in degrees around the rest angle.")]
    [Range(0f, 45f)] public float swayAmplitude = 10f;
    [Tooltip("Seconds per full sway cycle (higher = slower sway).")]
    [Range(0.2f, 10f)] public float swayPeriod = 3.5f;
    [Tooltip("Randomize starting sway phase per instance.")]
    public bool randomizeSwayPhase = true;

    private SpriteRenderer spriteRenderer;

    // Cache hero and its SpriteRenderer once for all trees
    private static OverworldHero hero;
    private static SpriteRenderer heroSR;

    // Idle sway state
    private Coroutine idleSwayRoutineRef;
    private float swayPhase;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Ensure we start at rest angle
        SetLocalEulerX(foldAngleX);
        swayPhase = randomizeSwayPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
        transform.position.SetZ(0f); // ensure on Z=0 plane
    }

    private void OnEnable()
    {
        TryCacheHero();
        SetLocalEulerX(foldAngleX);
        StartIdleSwayIfAllowed();
    }

    private void OnDisable()
    {
        StopIdleSway();
        SetLocalEulerX(foldAngleX);
    }

    private void Update()
    {
        if (!followHeroSorting) return;
        if (hero == null || heroSR == null) { TryCacheHero(); if (hero == null || heroSR == null) return; }

        var heroPos = hero.transform.position;
        var selfPos = transform.position;

        // Keep same sorting layer as hero for predictable +/-1 ordering
        if (spriteRenderer.sortingLayerID != heroSR.sortingLayerID)
            spriteRenderer.sortingLayerID = heroSR.sortingLayerID;

        // Behind hero if hero is below, in front if hero is above
        bool isHeroBelow = heroPos.y < selfPos.y;
        int desiredOrder = isHeroBelow ? (heroSR.sortingOrder - 1) : (heroSR.sortingOrder + 1);
        if (spriteRenderer.sortingOrder != desiredOrder)
            spriteRenderer.sortingOrder = desiredOrder;
    }

    private static void TryCacheHero()
    {
        if (hero == null) hero = Object.FindObjectOfType<OverworldHero>();
        if (hero != null && (heroSR == null || heroSR.gameObject != hero.gameObject))
            heroSR = hero.GetComponent<SpriteRenderer>();
    }

    // === Idle sway (no trigger required) ===
    private IEnumerator IdleSwayRoutine()
    {
        float w = (swayPeriod <= 0f) ? 0f : (Mathf.PI * 2f) / Mathf.Max(0.01f, swayPeriod);
        while (enableIdleSway && isActiveAndEnabled)
        {
            float angle = foldAngleX + Mathf.Sin((Time.time * w) + swayPhase) * swayAmplitude;
            SetLocalEulerX(angle);
            yield return null;
        }
        idleSwayRoutineRef = null;
    }

    private void StartIdleSwayIfAllowed()
    {
        if (!enableIdleSway) return;
        if (idleSwayRoutineRef != null) return;
        idleSwayRoutineRef = StartCoroutine(IdleSwayRoutine());
    }

    private void StopIdleSway()
    {
        if (idleSwayRoutineRef != null)
        {
            StopCoroutine(idleSwayRoutineRef);
            idleSwayRoutineRef = null;
        }
    }

    private void SetLocalEulerX(float x)
    {
        Vector3 e = transform.localEulerAngles;
        e.x = x;
        transform.localEulerAngles = e;
    }
}
