using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class GrassInstance : MonoBehaviour
{
    [Header("Flap Effect")]
    [Tooltip("Rest X-rotation when grass is normal (degrees, negative tilts toward camera).")]
    [Range(-80f, -5f)] public float foldAngleX = -45f; // normal state
    [Tooltip("Time to flatten from rest to 0 (seconds). Fast snap.")]
    [Range(0.01f, 0.3f)] public float foldInTime = 0.06f;
    [Tooltip("Time to return from 0 back to rest (seconds). Slow settle.")]
    [Range(0.01f, 0.6f)] public float foldReturnTime = 0.2f;

    [Header("Idle Sway")]
    [Tooltip("Enable gentle sway when there is no collision with the hero.")]
    public bool enableIdleSway = true;
    [Tooltip("Sway amplitude in degrees around the rest angle (e.g., 10 swings from -45 to -35 and -55).")]
    [Range(0f, 45f)] public float swayAmplitude = 10f;
    [Tooltip("Seconds per full sway cycle (higher = slower sway).")]
    [Range(0.2f, 10f)] public float swayPeriod = 3.5f;
    [Tooltip("Randomize starting sway phase per instance.")]
    public bool randomizeSwayPhase = true;

    [Header("Sorting")]
    [Tooltip("Match hero's sorting layer and go behind when hero is below, in front when above.")]
    public bool followHeroSorting = true;

    private SpriteRenderer spriteRenderer;
    private Collider2D trigger;

    // Cache hero and its SpriteRenderer once for all grass instances
    private static OverworldHero hero;
    private static SpriteRenderer heroSR;

    private Coroutine flapRoutineRef;
    private Coroutine idleSwayRoutineRef;
    private int heroInsideCount; // track nested overlaps
    private float swayPhase;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trigger = GetComponent<Collider2D>();

        // Prefer trigger behavior for pass-through
        if (trigger != null && !trigger.isTrigger)
            trigger.isTrigger = true;

        // Ensure we start at rest angle
        SetLocalEulerX(foldAngleX);

        swayPhase = randomizeSwayPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;

        transform.position.SetZ(0f); // ensure on Z=0 plane

        if (followHeroSorting) YSortUtility.Apply(spriteRenderer);
    }

    private void OnEnable()
    {
        TryCacheHero();
        // Keep rest orientation consistent on enable
        SetLocalEulerX(foldAngleX);
        heroInsideCount = 0;
        StartIdleSwayIfAllowed();

        if (followHeroSorting) YSortUtility.Apply(spriteRenderer);
    }

    private void OnDisable()
    {
        if (flapRoutineRef != null)
        {
            StopCoroutine(flapRoutineRef);
            flapRoutineRef = null;
        }
        if (idleSwayRoutineRef != null)
        {
            StopCoroutine(idleSwayRoutineRef);
            idleSwayRoutineRef = null;
        }
        heroInsideCount = 0;
        SetLocalEulerX(foldAngleX);
    }

    private void Update()
    {
        if (!followHeroSorting) return;

        YSortUtility.Apply(spriteRenderer);
    }

    private static void TryCacheHero()
    {
        if (hero == null) hero = Object.FindObjectOfType<OverworldHero>();
        if (hero != null && (heroSR == null || heroSR.gameObject != hero.gameObject))
            heroSR = hero.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the hero entering
        if (!IsHeroCollider(other)) return;

        heroInsideCount++;
        if (heroInsideCount == 1)
        {
            // Stop idle sway while intersecting
            StopIdleSway();
            // Start a snap flatten and hold at 0
            if (flapRoutineRef != null) StopCoroutine(flapRoutineRef);
            flapRoutineRef = StartCoroutine(FlattenToZeroRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsHeroCollider(other)) return;

        heroInsideCount = Mathf.Max(0, heroInsideCount - 1);
        if (heroInsideCount == 0)
        {
            // Only when the hero fully leaves, return to rest then resume idle sway
            if (flapRoutineRef != null) StopCoroutine(flapRoutineRef);
            flapRoutineRef = StartCoroutine(ReturnToRestRoutine());
        }
    }

    private bool IsHeroCollider(Component other)
    {
        if (other == null) return false;
        // Accept hero on this collider or on a parent
        var h = other.GetComponentInParent<OverworldHero>();
        if (h == null) return false;
        // Cache hero ref if not already
        if (hero == null) hero = h;
        if (heroSR == null && hero != null) heroSR = hero.GetComponent<SpriteRenderer>();
        return true;
    }

    private IEnumerator FlattenToZeroRoutine()
    {
        // Always start from rest (-45), flatten to 0 and hold
        SetLocalEulerX(foldAngleX);

        float durIn = Mathf.Max(0.01f, foldInTime);
        float t = 0f;
        while (t < durIn)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / durIn);
            // Ease-out for quick snap
            float eased = 1f - Mathf.Pow(1f - u, 2f);
            float angle = Mathf.LerpUnclamped(foldAngleX, 0f, eased);
            SetLocalEulerX(angle);
            yield return null;
        }
        SetLocalEulerX(0f); // ensure exact 0 and hold while inside
        flapRoutineRef = null;
    }

    private IEnumerator ReturnToRestRoutine()
    {
        // Return from current angle (typically 0) to rest (-45)
        float startX = GetLocalEulerXSigned();
        float endX = foldAngleX;
        float durOut = Mathf.Max(0.01f, foldReturnTime);

        float t = 0f;
        while (t < durOut)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / durOut);
            // Ease-in for gentle settle
            float eased = u * u;
            float angle = Mathf.LerpUnclamped(startX, endX, eased);
            SetLocalEulerX(angle);
            yield return null;
        }
        SetLocalEulerX(endX);
        flapRoutineRef = null;

        // Resume idle sway if still clear
        StartIdleSwayIfAllowed();
    }

    private IEnumerator IdleSwayRoutine()
    {
        // Gentle sway around the rest angle while not intersecting
        float w = (swayPeriod <= 0f) ? 0f : (Mathf.PI * 2f) / Mathf.Max(0.01f, swayPeriod);
        while (enableIdleSway && heroInsideCount == 0)
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
        if (heroInsideCount != 0) return;
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

    private float GetLocalEulerXSigned()
    {
        float x = transform.localEulerAngles.x;
        return (x > 180f) ? x - 360f : x;
    }

    private void SetLocalEulerX(float x)
    {
        Vector3 e = transform.localEulerAngles;
        e.x = x;
        transform.localEulerAngles = e;
    }
}
