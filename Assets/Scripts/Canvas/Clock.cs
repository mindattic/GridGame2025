using UnityEngine;
using TMPro;

/// <summary>
/// Shows the current time and pins the label to the upper-left,
/// offsetting by the device's LEFT safe-area inset (converted to canvas units)
/// so it never hides under the rounded corner or camera cutout.
/// Place this on the "Clock" object under TopOverlay.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class Clock : MonoBehaviour
{
    [Header("Clock Settings")]
    [SerializeField] private string timeFormat = "h:mm tt";
    [SerializeField] private int fontSize = 24;
    [SerializeField] private Color fontColor = Color.white;
    [SerializeField] private Vector2 padding = new Vector2(100f, -32f); // extra space inside the safe edge
    [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Left;
    [SerializeField] private bool respectHorizontalSafeInset = true;  // shift by left safe inset

    private TMP_Text clockText;
    private RectTransform rect;
    private Canvas rootCanvas;
    private float nextClockTickTime;

    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;
    private float lastScale = -1f;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        clockText = GetComponent<TMP_Text>();
        if (clockText == null) clockText = gameObject.AddComponent<TextMeshProUGUI>();

        rootCanvas = GetComponentInParent<Canvas>();

        // Configure text
        clockText.raycastTarget = false;
        clockText.textWrappingMode = TextWrappingModes.NoWrap;
        clockText.fontSize = fontSize;
        clockText.color = fontColor;
        clockText.alignment = alignment;

        AnchorAndPosition(force: true);
        UpdateClock(true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            AnchorAndPosition(force: true);
            UpdateClock(true);
            return;
        }
#endif
        // Re-layout when screen, safe area, or scale factor changes
        float scale = GetCanvasScale();
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenSize.x ||
            Screen.height != lastScreenSize.y ||
            !Mathf.Approximately(scale, lastScale))
        {
            AnchorAndPosition(force: true);
        }

        if (Application.isPlaying && Time.unscaledTime >= nextClockTickTime)
        {
            UpdateClock(false);
            nextClockTickTime = Mathf.Floor(Time.unscaledTime) + 1f;
        }
    }

    /// <summary>
    /// Anchors to the parent's top-left and applies left safe inset in canvas units.
    /// </summary>
    private void AnchorAndPosition(bool force)
    {
        if (rect == null) return;

        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        lastScale = GetCanvasScale();

        // Top-left anchor
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);

        // Convert left inset pixels to canvas units
        float leftInsetUnits = 0f;
        if (respectHorizontalSafeInset)
        {
            float leftInsetPixels = Mathf.Max(0f, Screen.safeArea.xMin);
            leftInsetUnits = PixelsToCanvasUnits(leftInsetPixels);
        }

        // Position inside the safe edge with extra padding
        rect.anchoredPosition = new Vector2(leftInsetUnits + padding.x, padding.y);

        // Reasonable height so text is not clipped
        rect.sizeDelta = new Vector2(280f, fontSize + 8f);
    }

    /// <summary>
    /// Updates the time string.
    /// </summary>
    private void UpdateClock(bool force)
    {
        if (clockText == null) return;

        string fmt = string.IsNullOrEmpty(timeFormat) ? "h:mm tt" : timeFormat;
        string now = System.DateTime.Now.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);

        if (force || !string.Equals(clockText.text, now))
            clockText.text = now;
    }

    private float GetCanvasScale()
    {
        if (rootCanvas == null) return 1f;
        return rootCanvas.scaleFactor <= 0f ? 1f : rootCanvas.scaleFactor;
    }

    private float PixelsToCanvasUnits(float pixels)
    {
        float scale = GetCanvasScale();
        return pixels / (scale <= 0f ? 1f : scale);
    }
}
