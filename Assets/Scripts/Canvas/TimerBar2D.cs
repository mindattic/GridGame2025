using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;

public class TimerBar2D : MonoBehaviour
{
    private Image back;
    private Image fill;
    private Image front;
    private RectTransform rootRect;
    private RectTransform barRect;

    [Header("Duration")]
    [Tooltip("Total time in seconds for a full fill to drain to zero.")]
    [SerializeField] private float maxDuration = 6f;

    // Use 97% of canvas width.
    private const float CanvasPercent = 0.97f;

    private float timeRemaining;
    private float maxWidth;
    private Coroutine countdown;

    private void Awake()
    {
        // Cache own rect transform
        rootRect = GetComponent<RectTransform>();

        // Resolve child images
        back = transform.Find("Back").GetComponent<Image>();
        fill = transform.Find("Fill").GetComponent<Image>();
        front = transform.Find("Front").GetComponent<Image>();

        // Cache fill rect
        barRect = fill.GetComponent<RectTransform>();

        // DrainRoutine to the left: pin the left edge, let the right edge retract left as width shrinks
        barRect.anchorMin = new Vector2(0f, 0.5f);
        barRect.anchorMax = new Vector2(0f, 0.5f);
        barRect.pivot = new Vector2(0f, 0.5f);
        barRect.anchoredPosition = new Vector2(0f, 0f);

        // Initialize timer value
        timeRemaining = maxDuration;
    }

    private void OnDestroy()
    {
        // If you subscribe to events, unsubscribe here to avoid leaks.
        // Example:
        // if (g.InputManager != null)
        //     g.InputManager.OnInputModeChanged -= HandleModeChanged;
    }

    /// <summary>
    /// Prepares layout, subscribes to mode changes, and positions the bar.
    /// </summary>
    public void Initialize()
    {
        // Compute initial layout and apply fill
        SetLayout();
        UpdateFill();

        // Subscribe to mode changes
        g.InputManager.OnInputModeChanged += HandleModeChanged;

        // Apply initial state
        HandleModeChanged(g.InputManager.InputMode);

        // Position the bar at the top edge of the board in screen space.
        // Canvas is Screen Space - Overlay, so screen point maps directly to rectTransform.position.
        // Pivot at bottom center so the bar grows downward from the top edge.
        if (rootRect != null && g.Board != null && g.Board.screenEdges != null)
        {
            rootRect.pivot = new Vector2(0.5f, 0f);              // bottom center
            rootRect.position = g.Board.screenEdges.Top + new Vector3(0, 100);          // place at board top midpoint
        }
    }

    /// <summary>
    /// Starts the countdown from the current remaining time.
    /// </summary>
    public void Play()
    {
        // DespawnRoutine existing countdown if running
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }

        // ProcessRoutine new countdown
        countdown = StartCoroutine(CountdownRoutine());
    }

    /// <summary>
    /// Pauses the countdown without resetting time.
    /// </summary>
    public void Pause()
    {
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
    }

    /// <summary>
    /// Refills to full and resets the timer to maxDuration.
    /// </summary>
    public void Refill()
    {
        // Reset colors
        back.color = ColorHelper.Solid.White;
        fill.color = ColorHelper.Solid.White;
        front.color = ColorHelper.Solid.White;

        // Reset time and apply full width
        timeRemaining = maxDuration;
        UpdateFill();
    }

    /// <summary>
    /// Tints layers red to indicate the timer is locked.
    /// </summary>
    public void Lock()
    {
        back.color = ColorHelper.Translucent.Red;
        fill.color = ColorHelper.Translucent.Red;
        front.color = ColorHelper.Translucent.Red;
    }

    // =================================================================================================
    // Internal Logic
    // =================================================================================================

    /// <summary>
    /// Shows or hides the timer based on input mode.
    /// </summary>
    private void HandleModeChanged(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.PlayerTurn:
                gameObject.SetActive(true);
                break;

            case InputMode.AbilityTarget:
                gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Main countdown loop that decrements timeRemaining and updates the fill each frame.
    /// </summary>
    private IEnumerator CountdownRoutine()
    {
        while (timeRemaining > 0f)
        {
            // Respect infinite timer debug mode
            if (g.DebugManager.isTimerInfinite)
            {
                yield return Wait.None();
                continue;
            }

            // Decrement timer and clamp
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;

            // Update UI width
            UpdateFill();

            // Wait one frame
            yield return Wait.None();
        }

        // Time expired, perform drop
        g.SelectedHeroManager.Drop();

        // Clear handle
        countdown = null;
    }

    /// <summary>
    /// Applies the current fill width to the fill based on timeRemaining.
    /// </summary>
    private void UpdateFill()
    {
        if (barRect == null) return;

        // Compute normalized remaining time
        float t = Mathf.Approximately(maxDuration, 0f) ? 0f : Mathf.Clamp01(timeRemaining / maxDuration);

        // Compute width and apply to the fill rect
        float width = maxWidth * t;

        Vector2 size = barRect.sizeDelta;
        size.x = width;
        barRect.sizeDelta = size;
    }

    /// <summary>
    /// Computes sizes from the canvas and applies them to background, fill, and front.
    /// </summary>
    private void SetLayout()
    {
        // Compute 97 percent of canvas width for sizing
        float targetWidth = Mathf.Max(0f, c.CanvasRect.rect.width * CanvasPercent);

        // Keep current height from root
        float targetHeight = rootRect.rect.height;

        // Apply to this container so parent layout understands our width
        if (rootRect != null)
        {
            Vector2 selfSize = rootRect.sizeDelta;
            selfSize.x = targetWidth;
            rootRect.sizeDelta = selfSize;
            rootRect.anchoredPosition = ScreenHelper.GetScreenPosition(rootRect, g.Board.screenEdges.Top);
        }

        // Apply to background and front overlays
        SetSize(back.GetComponent<RectTransform>(), targetWidth, targetHeight);
        SetSize(front.GetComponent<RectTransform>(), targetWidth, targetHeight);

        // Full width for the fill when timeRemaining is max
        maxWidth = targetWidth;

        // Ensure current fill size matches current timeRemaining
        UpdateFill();
    }


    /// <summary>
    /// Sets width and height on a RectTransform using sizeDelta.
    /// </summary>
    private static void SetSize(RectTransform rt, float width, float height)
    {
        if (rt == null) return;

        Vector2 sz = rt.sizeDelta;
        if (width >= 0f) sz.x = width;
        if (height > 0f) sz.y = height;
        rt.sizeDelta = sz;
    }
}
