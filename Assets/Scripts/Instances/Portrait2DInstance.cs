using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Manages a 2D portrait sliding across the UI canvas with a custom curved path on the main axis.
/// </summary>
public class Portrait2DInstance : MonoBehaviour
{
    public RectTransform rectTransform { get; private set; }
    public Image image { get; private set; }

    public Direction direction;
    private float fallbackDuration = 1f;
    private AnimationCurve slideCurve;
    Vector2 destination;
    public ActorInstance actor;
    private bool isBeingDestroyed = false;

    // Optional fixed lane positions (canvas-local, relative to parent rect center)
    // If set, vertical slides will use fixedX and horizontal slides will use fixedY.
    public float? fixedX = null;
    public float? fixedY = null;

    public Transform parent
    {
        get => rectTransform.parent;
        set => rectTransform.SetParent(value, false);
    }

    public Vector3 scale
    {
        get => rectTransform.localScale;
        set => rectTransform.localScale = value;
    }

    public Sprite sprite
    {
        get => image.sprite;
        set => image.sprite = value;
    }

    private float distance; // kept for backward-compat minor offset calc

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0.5f, 0.5f);
        image = GetComponent<Image>();
        distance = c.CanvasRect.rect.height;

        slideCurve = new AnimationCurve(
            new Keyframe(0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f),
            new Keyframe(0.8f, 0.05202637f, 0.0f, 0.0f, 0.33333334f, 0.70263505f),
            new Keyframe(1.2f, -0.05f, 0.0f, 0.0f, 0.33333334f, 0.33322528f),
            new Keyframe(1.993103f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f)
        )
        {
            preWrapMode = WrapMode.ClampForever,
            postWrapMode = WrapMode.ClampForever
        };


    }

    private void OnDestroy() => isBeingDestroyed = true;



    /// <summary>
    /// Slides the portrait from off-screen start to off-screen end using slideCurve:
    /// covers full screen span vertically or horizontally, with overshoot and retreat.
    /// Start/end are positioned just outside the canvas, based on the portrait size, so it appears immediately.
    /// </summary>
    public IEnumerator SlideInRoutine()
    {
        // Canvas and portrait sizes
        Rect canvas = c.CanvasRect.rect;
        float halfCanvasW = canvas.width * 0.5f;
        float halfCanvasH = canvas.height * 0.5f;
        float halfPortraitW = rectTransform.rect.width * rectTransform.localScale.x * 0.5f;
        float halfPortraitH = rectTransform.rect.height * rectTransform.localScale.y * 0.5f;
        const float padding = 2f; // small epsilon to ensure fully offscreen

        // Offscreen coordinates along each axis (centered coordinate system)
        float offscreenRightX = halfCanvasW + halfPortraitW + padding;
        float offscreenLeftX = -offscreenRightX;
        float offscreenTopY = halfCanvasH + halfPortraitH + padding;
        float offscreenBottomY = -offscreenTopY;

        //Generate random offset (used only if a fixed lane is not provided)
        float crossSpan = (direction == Direction.East || direction == Direction.West) ? canvas.height : canvas.width; // cross-axis size
        float offsetAmount = RNG.Float(0f, crossSpan * Increment.Percent10);
        float offset = RNG.Int(1, 2) == 1 ? offsetAmount : -offsetAmount;
        bool isVertical = direction == Direction.North || direction == Direction.South;

        // Determine lane base using fixedX/fixedY if provided
        float laneX = isVertical ? (fixedX ?? offset) : 0f;
        float laneY = !isVertical ? (fixedY ?? offset) : 0f;

        // Determine destination (end point) so the curve goes from -dest -> +dest
        switch (direction)
        {
            case Direction.East:
                destination = new Vector2(offscreenRightX, laneY);
                break;
            case Direction.West:
                destination = new Vector2(offscreenLeftX, laneY);
                break;
            case Direction.North:
                destination = new Vector2(laneX, offscreenTopY);
                break;
            case Direction.South:
                destination = new Vector2(laneX, offscreenBottomY);
                break;
        }

        // Initialize position at the curve start (v at t=0)
        float startV = slideCurve.Evaluate(0f); // expected -1
        Vector2 startPos;
        if (direction == Direction.East || direction == Direction.West)
        {
            startPos = new Vector2(destination.x * startV, destination.y);
        }
        else
        {
            startPos = new Vector2(destination.x, destination.y * startV);
        }
        rectTransform.anchoredPosition = startPos;

        float startTime = Time.time;
        float curveLength = slideCurve.keys[slideCurve.length - 1].time;
        float elapsedTime = 0f;

        while (elapsedTime < curveLength)
        {
            elapsedTime = Time.time - startTime;
            float v = slideCurve.Evaluate(elapsedTime);

            Vector2 pos;
            if (direction == Direction.East || direction == Direction.West)
            {
                // X changes, Y stays on its fixed/random lane
                pos = new Vector2(destination.x * v, destination.y);
            }
            else
            {
                // Y changes, X stays on its fixed/random lane
                pos = new Vector2(destination.x, destination.y * v);
            }

            rectTransform.anchoredPosition = pos;
            yield return Wait.None();
        }

        Despawn();
    }


    private void Despawn()
    {
        if (isBeingDestroyed) return;
        isBeingDestroyed = true;
        Destroy(gameObject);
    }
}
