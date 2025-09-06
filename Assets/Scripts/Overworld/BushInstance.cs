using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BushInstance : MonoBehaviour
{
    [Header("Rustle Effect")]
    [Range(0.1f, 1f)] public float squashY = 0.8f;
    [Range(0.01f, 0.5f)] public float squashInTime = 0.06f;
    [Range(0.01f, 0.5f)] public float squashOutTime = 0.12f;

    [Header("Shake")]
    [Tooltip("Horizontal shake amplitude in world units.")]
    [Range(0.0f, 0.5f)] public float shakeAmplitude = 0.02f;
    [Tooltip("How many back-and-forth cycles during the rustle.")]
    [Range(1, 12)] public int shakeCycles = 3;

    [Tooltip("How much vertical distance around the bush pivot still counts as a pass-through.")]
    [Range(0.0f, 1.0f)] public float crossYProximity = 0.25f; // fraction of bush height

    private SpriteRenderer spriteRenderer;

    // Cache hero and its SpriteRenderer once for all bushes
    private static OverworldHero hero;
    private static SpriteRenderer heroSR;

    private bool? heroWasBelow; // null until first sample
    private Coroutine rustleRoutineRef;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        TryCacheHero();
        // Initialize side to avoid false-positive first-frame rustle
        if (hero != null)
            heroWasBelow = hero.transform.position.y < transform.position.y;
    }

    private void Update()
    {
        var heroPos = hero.transform.position;
        var bushPos = transform.position;

        bool isHeroBelow = heroPos.y < bushPos.y;

        // Ensure we’re on the same sorting layer as the hero so +/-1 behaves predictably.
        if (spriteRenderer.sortingLayerID != heroSR.sortingLayerID)
            spriteRenderer.sortingLayerID = heroSR.sortingLayerID;

        // Compute desired order relative to hero
        int desiredOrder = isHeroBelow ? (heroSR.sortingOrder - 1) : (heroSR.sortingOrder + 1);
        if (spriteRenderer.sortingOrder != desiredOrder)
            spriteRenderer.sortingOrder = desiredOrder;

        // Detect passing through the bush: side flip + horizontal overlap + near pivot Y
        if (heroWasBelow.HasValue && heroWasBelow.Value != isHeroBelow && IsOverlappingHorizontally(heroPos) && IsNearPivotY(heroPos))
        {
            // Trigger rustle
            if (rustleRoutineRef != null) StopCoroutine(rustleRoutineRef);
            rustleRoutineRef = StartCoroutine(RustleRoutine());
        }

        heroWasBelow = isHeroBelow;
    }

    private static void TryCacheHero()
    {
        if (hero == null) hero = Object.FindObjectOfType<OverworldHero>();
        if (hero != null && (heroSR == null || heroSR.gameObject != hero.gameObject))
            heroSR = hero.GetComponent<SpriteRenderer>();
    }

    private bool IsOverlappingHorizontally(Vector3 heroPos)
    {
        // Consider overlap within the bush bounds in X
        var b = spriteRenderer.bounds;
        return heroPos.x >= b.min.x && heroPos.x <= b.max.x;
    }

    private bool IsNearPivotY(Vector3 heroPos)
    {
        // Near the bush pivot Y within a fraction of the bush height
        var b = spriteRenderer.bounds;
        float tolerance = Mathf.Clamp01(crossYProximity) * (b.size.y > 0f ? b.size.y : 0.2f);
        return Mathf.Abs(heroPos.y - transform.position.y) <= Mathf.Max(0.02f, tolerance);
    }

    private IEnumerator RustleRoutine()
    {
        // Cache start transforms
        Vector3 startScale = transform.localScale;
        Vector3 startPos = transform.localPosition;

        // Targets
        float minY = Mathf.Clamp(squashY, 0.1f, 1f);
        Vector3 squashed = new Vector3(startScale.x * (1f + (1f - minY) * 0.15f), // slight widen on X for volume conservation
                                       startScale.y * minY,
                                       startScale.z);

        float durIn = Mathf.Max(0.01f, squashInTime);
        float durOut = Mathf.Max(0.01f, squashOutTime);
        float totalDur = durIn + durOut;

        // Randomize initial shake phase for variation
        float randomPhase = Random.Range(0f, Mathf.PI * 2f);

        float t = 0f;
        while (t < totalDur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / totalDur);

            // Squash envelope: quick impact then recover
            // Map u to a 0..1..0 bell curve for squash influence
            float squashCurve = Mathf.Sin(u * Mathf.PI); // 0->1->0
            float squashBlend;
            if (t <= durIn)
            {
                float pin = Mathf.Clamp01(t / durIn);
                float easeOut = 1f - Mathf.Pow(1f - pin, 2f);
                squashBlend = easeOut; // towards squashed
            }
            else
            {
                float pout = Mathf.Clamp01((t - durIn) / durOut);
                float easeIn = pout * pout;
                squashBlend = 1f - easeIn; // back to start
            }
            transform.localScale = Vector3.LerpUnclamped(startScale, squashed, squashBlend);

            // Shake: decaying horizontal sine, centered on startPos
            // Amplitude decays with (1-u) so it settles by the end.
            float amp = shakeAmplitude * (1f - u);
            float cycles = Mathf.Max(1, shakeCycles);
            float xOffset = Mathf.Sin((u * cycles * Mathf.PI * 2f) + randomPhase) * amp;
            transform.localPosition = new Vector3(startPos.x + xOffset, startPos.y, startPos.z);

            yield return null;
        }

        // Restore
        transform.localScale = startScale;
        transform.localPosition = startPos;
        rustleRoutineRef = null;
    }
}
