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

    [SerializeField] public Direction direction;
    [SerializeField] private float fallbackDuration = 1f;
    [SerializeField] private AnimationCurve slideCurve;
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

    private float distance;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0.5f, 0.5f);
        image = GetComponent<Image>();
        distance = c.CanvasRect.rect.height;
    }

    private void OnDestroy() => isBeingDestroyed = true;



    /// <summary>
    /// Slides the portrait from off-screen start to off-screen end using slideCurve:
    /// covers full screen span vertically or horizontally, with overshoot and retreat.
    /// </summary>
    public IEnumerator SlideInRoutine()
    {
        //Generate random offset (used only if a fixed lane is not provided)
        float offsetAmount = RNG.Float(0f, distance * Increment.Percent10);
        float offset = RNG.Int(1, 2) == 1 ? offsetAmount : -offsetAmount;
        bool isVertical = direction == Direction.North || direction == Direction.South;

        // Determine lane base using fixedX/fixedY if provided
        float laneX = isVertical ? (fixedX ?? offset) : 0f;
        float laneY = !isVertical ? (fixedY ?? offset) : 0f;

        // Determine origin (start near center along main axis 0, with lane on cross axis)
        rectTransform.anchoredPosition = new Vector2(laneX, laneY);

        // Determine destination
        switch (direction)
        {
            case Direction.East:
                destination = new Vector2(distance, laneY);
                break;
            case Direction.West:
                destination = new Vector2(-distance, laneY);
                break;
            case Direction.North:
                destination = new Vector2(laneX, distance);
                break;
            case Direction.South:
                destination = new Vector2(laneX, -distance);
                break;
        }

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
