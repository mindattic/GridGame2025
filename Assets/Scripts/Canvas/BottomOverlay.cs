using UnityEngine;

[DisallowMultipleComponent]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public sealed class BottomOverlaySimple : MonoBehaviour
{
    [SerializeField] private float minHeightRefPx = 100f; // minimum height in reference pixels

    private RectTransform rect;
    private Canvas rootCanvas;

    private Rect lastSafe;
    private Vector2Int lastSize;
    private float lastScale = -1f;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        Apply();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) { Apply(); return; }
#endif
        float s = GetScale();
        if (Screen.safeArea != lastSafe ||
            Screen.width != lastSize.x ||
            Screen.height != lastSize.y ||
            !Mathf.Approximately(s, lastScale))
        {
            Apply();
        }
    }

    /// <summary>
    /// Top of the rect sits at safeArea.yMin. Full width. Height >= minHeightRefPx.
    /// </summary>
    private void Apply()
    {
        if (rect == null) return;

        lastSafe = Screen.safeArea;
        lastSize = new Vector2Int(Screen.width, Screen.height);
        lastScale = GetScale();

        // Stretch along bottom, pivot at top
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 1f);

        // Full width
        rect.offsetMin = new Vector2(0f, rect.offsetMin.y);
        rect.offsetMax = new Vector2(0f, rect.offsetMax.y);

        // Convert bottom safe inset (pixels) to canvas units
        float safeBottomUnits = PixelsToUnits(Mathf.Max(0f, Screen.safeArea.yMin));

        // Height is current rect height or at least the minimum
        float currentH = rect.rect.height > 0f ? rect.rect.height : 0f;
        float minH = RefPxToUnits(minHeightRefPx);
        float finalH = Mathf.Max(currentH, minH);

        // Size first, then place top at safe bottom
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalH);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, safeBottomUnits);
    }

    // ---- helpers ----

    private float GetScale()
    {
        if (rootCanvas == null) return 1f;
        return rootCanvas.scaleFactor <= 0f ? 1f : rootCanvas.scaleFactor;
    }

    private float PixelsToUnits(float pixels)
    {
        float s = GetScale();
        return pixels / (s <= 0f ? 1f : s);
    }

    private float RefPxToUnits(float refPx)
    {
        float s = GetScale();
        return refPx / (s <= 0f ? 1f : s);
    }
}
