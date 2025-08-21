using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    public Vector2 scrollFocus = new Vector2(0f, 0f);
    private Vector2 scrollFocusMin = new Vector2(-0.02f, -0.02f);
    private Vector2 scrollFocusMax = new Vector2(0.02f, 0.02f);
    private float minSecondsBetweenChanges = 10f;
    private float maxSecondsBetweenChanges = 30f;
    [Range(0f, 10f)] private float focusLerpSpeed = 3f;
    private bool useUnscaledTime = true;

    private RawImage rawImage;
    private Rect uvRect;
    private Vector2 targetScrollFocus;
    private float nextChangeAt;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("ScrollingUITexture: No RawImage component found!");
            return;
        }

        // Start from current uvRect
        uvRect = rawImage.uvRect;

        // Randomize the UV rect position at startup
        uvRect.position = new Vector2(RNG.Float(0, 1), RNG.Float(0, 1));

        rawImage.uvRect = uvRect;

        // Initialize target focus
        targetScrollFocus = RandomFocusInRange();
        ScheduleNextChange();
    }

    void Update()
    {
        if (rawImage == null || !gameObject.activeInHierarchy)
            return;

        if (Now() >= nextChangeAt)
        {
            targetScrollFocus = RandomFocusInRange();
            ScheduleNextChange();
        }

        float dt = Dt();
        float t = 1f - Mathf.Exp(-focusLerpSpeed * dt);
        scrollFocus = Vector2.Lerp(scrollFocus, targetScrollFocus, t);

        uvRect.position += scrollFocus * dt;
        rawImage.uvRect = uvRect;
    }

    private Vector2 RandomFocusInRange()
    {
        float x = RNG.Range(scrollFocusMin.x, scrollFocusMax.x);
        float y = RNG.Range(scrollFocusMin.y, scrollFocusMax.y);
        return new Vector2(x, y);
    }

    private void ScheduleNextChange()
    {
        float wait = RNG.Range(minSecondsBetweenChanges, maxSecondsBetweenChanges);
        nextChangeAt = Now() + Mathf.Max(0f, wait);
    }

    private float Now()
    {
        return useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    private float Dt()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}
