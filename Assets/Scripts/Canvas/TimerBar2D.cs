using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameManagerHelper;

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

    // Always use 96% of canvas width for both width and vertical offset, per request.
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

        // Drain to the left: pin the left edge, let the right edge retract left as width shrinks
        barRect.anchorMin = new Vector2(0f, 0.5f);
        barRect.anchorMax = new Vector2(0f, 0.5f);
        barRect.pivot = new Vector2(0f, 0.5f);
        barRect.anchoredPosition = new Vector2(0f, 0f);

        // Initialize timer value
        timeRemaining = maxDuration;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (g.InputManager != null)
        {
            g.InputManager.OnInputModeChanged -= HandleModeChanged;
        }
    }

    /// <summary>
    /// Subscribes to input mode changes and applies the initial visibility state.
    /// </summary>
    public void Initialize()
    {
        // Compute initial layout and apply fill
        SetLayout();
        UpdateFill();

        // Subscribe to mode changes
        g.InputManager.OnInputModeChanged += HandleModeChanged;

        // Apply initial state
        HandleModeChanged(g.InputManager.inputMode);
    }

    /// <summary>
    /// Starts the countdown from the current remaining time.
    /// </summary>
    public void Play()
    {
        // Stop existing countdown if running
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }

        // Start new countdown
        countdown = StartCoroutine(Countdown());
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
    /// Refills the fill to full and resets the timer to maxDuration.
    /// </summary>
    public void Refill()
    {
        // Reset colors to white
        back.color = ColorHelper.Solid.White;
        fill.color = ColorHelper.Solid.White;
        front.color = ColorHelper.Solid.White;

        // Reset time and apply full width
        timeRemaining = maxDuration;
        UpdateFill();
    }

    /// <summary>
    /// Visually locks the timer by tinting all layers red.
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
            case InputMode.Player:
                gameObject.SetActive(true);
                break;

            case InputMode.AbilityTarget:
                gameObject.SetActive(false);
                break;

            default:
                // No change for other modes
                break;
        }
    }

    /// <summary>
    /// Main countdown loop that decrements timeRemaining and updates the fill each frame.
    /// </summary>
    private IEnumerator Countdown()
    {
        while (timeRemaining > 0f)
        {
            // Respect infinite timer debug mode
            if (g.DebugManager.isTimerInfinite)
            {
                yield return Wait.UntilNextFrame();
                continue;
            }

            // Decrement timer and clamp
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;

            // Update UI width
            UpdateFill();

            // Wait one frame
            yield return Wait.UntilNextFrame();
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
    /// Also offsets the entire timer vertically by 96 percent of the canvas width, in canvas space.
    /// </summary>
    private void SetLayout()
    {
        // Compute 96 percent of canvas width for sizing
        var canvasRect = g.Canvas2D.GetComponent<RectTransform>();
        float targetWidth = Mathf.Max(0f, canvasRect.rect.width * CanvasPercent);

        // Keep current height from root
        float targetHeight = rootRect.rect.height;

        // Apply to this container so parent layout understands our width
        if (rootRect != null)
        {
            Vector2 selfSize = rootRect.sizeDelta;
            selfSize.x = targetWidth;
            rootRect.sizeDelta = selfSize;

            // Vertical offset currently static
            Vector2 pos = rootRect.anchoredPosition;
            pos.y = 745;
            rootRect.anchoredPosition = pos;
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
    /// Sets width and height on a RectTransform using sizeDelta, preserving axis if zero or negative.
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
