using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameManagerHelper;

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
        distance = g.Canvas2D.GetComponent<RectTransform>().rect.height;
    }

    private void OnDestroy() => isBeingDestroyed = true;



    /// <summary>
    /// Slides the portrait from off-screen start to off-screen end using slideCurve:
    /// covers full screen span vertically or horizontally, with overshoot and retreat.
    /// </summary>
    public IEnumerator SlideIn()
    {
        //Generate random offset
        float offsetAmount = Random.Float(0f, distance * Increment.Percent10);
        float offset = Random.Int(1, 2) == 1 ? offsetAmount : -offsetAmount;
        bool isVertical = direction == Direction.North || direction == Direction.South;

        // Determine origin
        rectTransform.anchoredPosition = new Vector2(isVertical ? offset : 0, !isVertical ? offset : 0);

        // Determine destination
        switch (direction)
        {
            case Direction.East:
                destination = new Vector2(distance, offset);
                break;
            case Direction.West:
                destination = new Vector2(-distance, offset);
                break;
            case Direction.North:
                destination = new Vector2(offset, distance);
                break;
            case Direction.South:
                destination = new Vector2(offset, -distance);
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
                pos = new Vector2(destination.x * v, destination.y); // X changes, Y stays offset
            }
            else
            {
                pos = new Vector2(destination.x, destination.y * v); // Y changes, X stays offset
            }

            rectTransform.anchoredPosition = pos;
            yield return null;
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
