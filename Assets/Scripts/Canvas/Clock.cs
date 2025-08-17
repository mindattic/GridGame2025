using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows the current time and pins the label to the upper-left.
/// Offsets by the device's LEFT safe-area inset converted to canvas units,
/// plus extra padding authored in reference-resolution pixels.
/// Works correctly with Canvas Scaler in any UI Scale Mode.
/// Place this on the "Clock" object under TopOverlay.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class Clock : MonoBehaviour
{

    private string timeFormat = "h:mm tt";          // Time format string used by DateTime.ToString
    private int fontSize = 24;                       // Font size applied to the TMP text
    private Color fontColor = Color.white;           // Font color for the clock
    private Vector2 padding = new Vector2(100f, -32f); // Extra space inside the safe edge in reference-resolution pixels
    private TextAlignmentOptions alignment = TextAlignmentOptions.Left; // Text alignment inside the rect
    private bool respectHorizontalSafeInset = true;  // If true, shifts by the left safe inset

    // Cached component references
    private TMP_Text clockText;       // The TextMeshPro component used to render the time
    private RectTransform rect;       // The RectTransform of this clock
    private Canvas rootCanvas;        // The first parent Canvas
    private CanvasScaler canvasScaler; // The first CanvasScaler found up the hierarchy

    // Clock tick scheduling
    private float nextClockTickTime;  // Next whole-second update when playing

    // Change detection caches
    private Rect lastSafeArea = Rect.zero;           // Last observed Screen.safeArea
    private Vector2Int lastScreenSize = Vector2Int.zero; // Last observed screen size in pixels
    private float lastEffectiveScale = -1f;          // Last computed effective UI scale used for conversions

    private void OnEnable()
    {
        // Grab core components
        rect = GetComponent<RectTransform>();
        clockText = GetComponent<TMP_Text>();

        // Ensure we have a TMP component to render text
        if (clockText == null)
        {
            clockText = gameObject.AddComponent<TextMeshProUGUI>();
        }

        // Find the nearest parent Canvas and CanvasScaler
        rootCanvas = GetComponentInParent<Canvas>();
        canvasScaler = rootCanvas != null ? rootCanvas.GetComponent<CanvasScaler>() : null;

        // Configure TMP for a simple, uncluttered label
        clockText.raycastTarget = false;
        clockText.textWrappingMode = TextWrappingModes.NoWrap;
        clockText.fontSize = fontSize;
        clockText.color = fontColor;
        clockText.alignment = alignment;

        // Initial layout and time
        AnchorAndPosition(force: true);
        UpdateClock(force: true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        // In edit mode, always refresh layout and text so Scene view reflects changes
        if (!Application.isPlaying)
        {
            AnchorAndPosition(force: true);
            UpdateClock(force: true);
            return;
        }
#endif
        // Re-layout when screen size, safe area, or effective scale changes
        float currentScale = GetEffectiveUiScale();
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenSize.x ||
            Screen.height != lastScreenSize.y ||
            !Mathf.Approximately(currentScale, lastEffectiveScale))
        {
            AnchorAndPosition(force: true);
        }

        // Update the clock text on whole-second boundaries during play
        if (Application.isPlaying && Time.unscaledTime >= nextClockTickTime)
        {
            UpdateClock(force: false);
            nextClockTickTime = Mathf.Floor(Time.unscaledTime) + 1f;
        }
    }

    /// <summary>
    /// Anchors to the parent's top-left and applies left safe inset plus reference-padding.
    /// Converts all pixel values to canvas-local units using the effective UI scale.
    /// </summary>
    private void AnchorAndPosition(bool force)
    {
        if (rect == null) return;

        // Cache the current environment observed during this layout pass
        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        lastEffectiveScale = GetEffectiveUiScale();

        // Anchor and pivot for top-left positioning
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);

        // Convert left safe inset from pixels to canvas units
        float leftInsetUnits = 0f;
        if (respectHorizontalSafeInset)
        {
            float leftInsetPixels = Mathf.Max(0f, Screen.safeArea.xMin);
            leftInsetUnits = PixelsToCanvasUnits(leftInsetPixels);
        }

        // Convert authoring padding, defined in reference-resolution pixels, into canvas units
        Vector2 refPaddingUnits = ReferencePixelsToCanvasUnits(padding);

        // Apply the final anchored position
        rect.anchoredPosition = new Vector2(leftInsetUnits + refPaddingUnits.x, refPaddingUnits.y);

        // Set a reasonable height so text is not clipped
        rect.sizeDelta = new Vector2(280f, fontSize + 8f);
    }

    /// <summary>
    /// Updates the time string displayed in the TMP label.
    /// Uses invariant culture to avoid locale issues when authoring.
    /// </summary>
    private void UpdateClock(bool force)
    {
        if (clockText == null) return;

        string fmt = string.IsNullOrEmpty(timeFormat) ? "h:mm tt" : timeFormat;
        string now = System.DateTime.Now.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);

        if (force || !string.Equals(clockText.text, now))
        {
            clockText.text = now;
        }
    }

    /// <summary>
    /// Computes the effective UI scale for converting pixels to canvas-local units.
    /// Auto-detects CanvasScaler mode and mirrors Unity's scaling behavior.
    /// Falls back to Canvas.scaleFactor if available.
    /// </summary>
    private float GetEffectiveUiScale()
    {
        // If there is a CanvasScaler, compute scale according to its mode
        if (canvasScaler != null && rootCanvas != null)
        {
            switch (canvasScaler.uiScaleMode)
            {
                case CanvasScaler.ScaleMode.ConstantPixelSize:
                    {
                        // In this mode, scaleFactor directly scales UI pixels
                        // Unity applies canvas scaleFactor = canvasScaler.scaleFactor
                        float s = canvasScaler.scaleFactor;
                        return s > 0f ? s : 1f;
                    }

                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    {
                        // Replicate Unity's width/height match calculation
                        Vector2 referenceResolution = canvasScaler.referenceResolution;
                        if (referenceResolution.x <= 0f || referenceResolution.y <= 0f)
                        {
                            // Guard against invalid config
                            return rootCanvas.scaleFactor > 0f ? rootCanvas.scaleFactor : 1f;
                        }

                        // Compute log interpolation for different screen match modes
                        float logWidth = Mathf.Log(Screen.width / referenceResolution.x, 2f);
                        float logHeight = Mathf.Log(Screen.height / referenceResolution.y, 2f);

                        float match;
                        if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
                        {
                            // Choose the smaller scale to fit entirely
                            match = 0f; // equivalent to preferring width scale if width is the limiting factor in log space
                                        // But Expand actually uses min of widthScale and heightScale. We implement it directly below.
                            float widthScale = Screen.width / referenceResolution.x;
                            float heightScale = Screen.height / referenceResolution.y;
                            float scale = Mathf.Min(widthScale, heightScale);
                            return scale > 0f ? scale : 1f;
                        }
                        else if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
                        {
                            // Choose the larger scale to ensure fill
                            float widthScale = Screen.width / referenceResolution.x;
                            float heightScale = Screen.height / referenceResolution.y;
                            float scale = Mathf.Max(widthScale, heightScale);
                            return scale > 0f ? scale : 1f;
                        }
                        else
                        {
                            // MatchWidthOrHeight blends between width and height scales in log space
                            match = Mathf.Clamp01(canvasScaler.matchWidthOrHeight);
                            float scale = Mathf.Pow(2f, Mathf.Lerp(logWidth, logHeight, match));
                            return scale > 0f ? scale : 1f;
                        }
                    }

                case CanvasScaler.ScaleMode.ConstantPhysicalSize:
                    {
                        // Convert physical units to pixels per reference unit
                        // This mirrors CanvasScaler's internal calculation approximately
                        // Get current screen DPI with fallback
                        float dpi = Screen.dpi;
                        if (dpi <= 0f) dpi = 96f;

                        // Determine number of pixels per unit based on the chosen unit
                        float unitsPerInch = 1f;
                        switch (canvasScaler.physicalUnit)
                        {
                            case CanvasScaler.Unit.Centimeters: unitsPerInch = 2.54f; break;
                            case CanvasScaler.Unit.Millimeters: unitsPerInch = 25.4f; break;
                            case CanvasScaler.Unit.Inches: unitsPerInch = 1f; break;
                            case CanvasScaler.Unit.Points: unitsPerInch = 1f / 72f; break;
                            case CanvasScaler.Unit.Picas: unitsPerInch = 1f / 6f; break;
                            default: unitsPerInch = 1f; break;
                        }

                        // Reference DPI controls the baseline for physical scaling
                        float referenceDpi = canvasScaler.fallbackScreenDPI > 0f
                            ? canvasScaler.fallbackScreenDPI
                            : 96f;

                        // Scale so that reference physical size maps to pixels
                        // Effective scale is proportional to dpi / referenceDpi
                        float scale = dpi / referenceDpi;
                        return scale > 0f ? scale : 1f;
                    }
            }
        }

        // Fallback to the Canvas scaleFactor if Scaler is missing
        if (rootCanvas != null && rootCanvas.scaleFactor > 0f)
        {
            return rootCanvas.scaleFactor;
        }

        // Final guard if no canvas is present
        return 1f;
    }

    /// <summary>
    /// Converts physical screen pixels to canvas-local units using the effective scale.
    /// Use this for Screen.safeArea insets or any raw pixel measurements.
    /// </summary>
    private float PixelsToCanvasUnits(float pixels)
    {
        float scale = GetEffectiveUiScale();
        return pixels / (scale <= 0f ? 1f : scale);
    }

    /// <summary>
    /// Converts a vector expressed in reference-resolution pixels into canvas-local units.
    /// This keeps your authored padding consistent with the CanvasScaler reference.
    /// </summary>
    private Vector2 ReferencePixelsToCanvasUnits(Vector2 refPixels)
    {
        float scale = GetEffectiveUiScale();
        float x = refPixels.x / (scale <= 0f ? 1f : scale);
        float y = refPixels.y / (scale <= 0f ? 1f : scale);
        return new Vector2(x, y);
    }
}
