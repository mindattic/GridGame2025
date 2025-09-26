using Assets.Helper;
using Assets.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;

public class TimerBar2D : MonoBehaviour
{
    private RectTransform rootRect;
    private Image back;
    private Image fill;
    private Image front; 
    private RectTransform fillRect;
    private TextMeshProUGUI countdownLabel; // new

    [Header("Duration")]
    [Tooltip("Total time in seconds for a full fill to drain to zero.")]
    [SerializeField] private float maxDuration = 6f;

    [Tooltip("If true, the bar shrinks symmetrically into the center. If false, it drains from right to left.")]
    [SerializeField] private bool isCentered = false;

    // Use 97% of canvas width.
    private const float CanvasPercent = 0.97f;

    private float timeRemaining;
    private float maxWidth;
    private Coroutine runningCoroutine;

    private void Awake()
    {
        // Cache own rect transform
        rootRect = GetComponent<RectTransform>();

        // Resolve child images
        back = GameObjectHelper.Game.TimerBar.Back;
        fill = GameObjectHelper.Game.TimerBar.Fill;
        countdownLabel = GameObjectHelper.Game.TimerBar.CountdownLabel;
        front = GameObjectHelper.Game.TimerBar.Front;

        // Cache fill rect
        fillRect = fill.GetComponent<RectTransform>();

        if (isCentered)
        {
            // Drain toward the center by keeping the fill centered
            fillRect.anchorMin = new Vector2(0.5f, 0.5f);
            fillRect.anchorMax = new Vector2(0.5f, 0.5f);
            fillRect.pivot = new Vector2(0.5f, 0.5f);
        }
        else
        {
            // Drain from right to left by anchoring the left edge
            fillRect.anchorMin = new Vector2(0f, 0.5f);
            fillRect.anchorMax = new Vector2(0f, 0.5f);
            fillRect.pivot = new Vector2(0f, 0.5f);
        }
        fillRect.anchoredPosition = Vector2.zero;

        // Initialize timer value
        timeRemaining = maxDuration;
        UpdateCountdownLabel();
    }

    //private void OnDestroy()
    //{
    //    if (g.InputManager != null)
    //        g.InputManager.OnInputModeChanged -= HandleModeChanged;
    //}

    /// <summary>
    /// Prepares layout, subscribes to mode changes, and positions the bar.
    /// </summary>
    public void Initialize()
    {
        SetLayout();
        UpdateFill();
        UpdateCountdownLabel();

        // Subscribe to mode changes
        g.InputManager.OnInputModeChanged += HandleModeChanged;

        // Apply initial state
        HandleModeChanged(g.InputManager.InputMode);

        // Position the bar at the top edge of the board in screen space.
        if (rootRect != null && g.Board != null && g.Board.screenEdges != null)
        {
            rootRect.pivot = new Vector2(0.5f, 0f); // bottom center
            rootRect.position = g.Board.screenEdges.Top + new Vector3(0, 100);
        }
    }

    /// <summary>
    /// Starts the countdown from the current remaining time.
    /// </summary>
    public void Play()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        runningCoroutine = StartCoroutine(CountdownRoutine());
    }

    /// <summary>
    /// Pauses the countdown without resetting time.
    /// </summary>
    public void Pause()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    /// <summary>
    /// Refills to full and resets the timer to maxDuration. Also tints layers back to white.
    /// </summary>
    public void Refill()
    {
        back.color = ColorHelper.Solid.White;
        if (fill != null) fill.color = ColorHelper.Solid.White;
        if (front != null) front.color = ColorHelper.Solid.White;

        timeRemaining = maxDuration;
        UpdateFill();
        UpdateCountdownLabel();
    }

    /// <summary>
    /// Tints layers red to indicate the timer is locked.
    /// </summary>
    public void Lock()
    {
        back.color = ColorHelper.Translucent.Red;
        if (fill != null) fill.color = ColorHelper.Translucent.Red;
        if (front != null) front.color = ColorHelper.Translucent.Red;
    }

    /// <summary>
    /// Sets the countdown duration in seconds. Does not start playback.
    /// Clamps current remaining time to the new duration and updates the fill.
    /// </summary>
    public void SetDuration(float seconds)
    {
        maxDuration = Mathf.Max(0.01f, seconds);
        if (timeRemaining > maxDuration)
            timeRemaining = maxDuration;

        UpdateFill();
        UpdateCountdownLabel();
    }

    /// <summary>
    /// Resets the remaining time to the current duration and updates the fill.
    /// Does not change colors and does not start playback.
    /// </summary>
    public void ResetToFull()
    {
        timeRemaining = maxDuration;
        UpdateFill();
        UpdateCountdownLabel();
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
            case InputMode.AnyTarget:
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

            // Update UI width and label
            UpdateFill();
            UpdateCountdownLabel();

            // Wait one frame
            yield return Wait.None();
        }

        // Time expired, perform drop and block input until release
        g.SelectedHeroManager.Drop();
        g.InputManager.RequireTouchRelease();

        // Clear handle
        runningCoroutine = null;
    }

    /// <summary>
    /// Applies the current fill width to the fill based on timeRemaining.
    /// </summary>
    private void UpdateFill()
    {
        if (fillRect == null) return;

        float t = Mathf.Approximately(maxDuration, 0f) ? 0f : Mathf.Clamp01(timeRemaining / maxDuration);
        float width = maxWidth * t;

        Vector2 size = fillRect.sizeDelta;
        size.x = width;
        fillRect.sizeDelta = size;
    }

    /// <summary>
    /// Updates the countdown label text, if present.
    /// </summary>
    private void UpdateCountdownLabel()
    {
        if (countdownLabel == null) return;

        // Show whole seconds with one decimal place, minimum 0.0
        float display = Mathf.Max(0f, timeRemaining);
        countdownLabel.text = display.ToString("0.0");
    }

    /// <summary>
    /// Computes sizes from the canvas and applies them to background, fill, and front.
    /// </summary>
    private void SetLayout()
    {
        float targetWidth = Mathf.Max(0f, c.CanvasRect.rect.width * CanvasPercent);
        float targetHeight = rootRect.rect.height;

        if (rootRect != null)
        {
            Vector2 selfSize = rootRect.sizeDelta;
            selfSize.x = targetWidth;
            rootRect.sizeDelta = selfSize;
            rootRect.anchoredPosition = UnitConversionHelper.World.ToCanvas(rootRect, g.Board.screenEdges.Top);
        }

        SetSize(back.GetComponent<RectTransform>(), targetWidth, targetHeight);
        var frontRect = front != null ? front.GetComponent<RectTransform>() : null;
        if (frontRect != null) SetSize(frontRect, targetWidth, targetHeight);
        SetSize(fillRect, -1f, targetHeight);

        // Keep the fill aligned after resizing regardless of mode
        fillRect.anchoredPosition = Vector2.zero;

        maxWidth = targetWidth;
        UpdateFill();
    }

    /// <summary>
    /// Sets width and height on a RectTransform using sizeDelta.
    /// Pass a negative width to keep the current width unchanged.
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
