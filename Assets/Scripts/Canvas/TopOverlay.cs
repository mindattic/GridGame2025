using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sizes a top overlay to exactly cover the unsafe top inset on devices with a notch.
/// Works in play and edit mode, and re-applies on resolution, orientation, or safe area change.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class TopOverlay : MonoBehaviour
{
    [Header("Overlay Target")]
    [SerializeField] private RectTransform topOverlay;

    [Header("Appearance")]
    [SerializeField] private Color overlayColor = Color.black;

    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;
    private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    private void OnEnable()
    {
        // Auto-assign if missing
        if (topOverlay == null)
            topOverlay = GetComponent<RectTransform>();

        Apply(force: true);
        ApplyOverlayColorIfAvailable();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // In edit mode, keep it responsive in Simulator
        if (!Application.isPlaying)
        {
            Apply(force: true);
            return;
        }
#endif
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenSize.x ||
            Screen.height != lastScreenSize.y ||
            Screen.orientation != lastOrientation)
        {
            Apply(force: false);
        }
    }

    /// <summary>
    /// Computes the top inset from Screen.safeArea and sizes the overlay to match it.
    /// If there is no inset, the overlay height becomes zero.
    /// </summary>
    private void Apply(bool force)
    {
        if (topOverlay == null)
            return;

        Rect safe = Screen.safeArea;
        Vector2Int size = new Vector2Int(Screen.width, Screen.height);
        ScreenOrientation orient = Screen.orientation;

        if (!force && safe == lastSafeArea && size == lastScreenSize && orient == lastOrientation)
            return;

        lastSafeArea = safe;
        lastScreenSize = size;
        lastOrientation = orient;

        // Always force anchors to top stretch
        topOverlay.anchorMin = new Vector2(0f, 1f);
        topOverlay.anchorMax = new Vector2(1f, 1f);
        topOverlay.pivot = new Vector2(0.5f, 1f);

        if (size.x <= 0 || size.y <= 0)
            return;

        float topInsetPixels = Mathf.Max(0f, size.y - safe.yMax);

        // Left and right pinned to screen edges, height equals the top inset
        topOverlay.offsetMin = new Vector2(0f, -topInsetPixels);
        topOverlay.offsetMax = new Vector2(0f, 0f);
    }

    /// <summary>
    /// Applies the configured color to the overlay Image if present.
    /// </summary>
    private void ApplyOverlayColorIfAvailable()
    {
        if (topOverlay == null)
            return;

        var img = topOverlay.GetComponent<Image>();
        if (img != null)
            img.color = overlayColor;
    }
}
