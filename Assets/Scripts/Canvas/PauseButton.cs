using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
public sealed class PauseButton : MonoBehaviour
{
    [SerializeField] private RectTransform buttonChild; // optional, just for clarity if you need to access it

    private RectTransform rect;      // root PauseButton rect
    private Canvas rootCanvas;
    private CanvasScaler canvasScaler;
    private LayoutElement layoutElement;

    private float minHeight = 48f;
    private Vector2 margin = new Vector2(8f, 8f);

    private Rect lastSafe;
    private Vector2Int lastSize;
    private bool isParented;
    private int lastSiblingCount = -1;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        canvasScaler = rootCanvas != null ? rootCanvas.GetComponent<CanvasScaler>() : null;

        EnsureInOverlay();
        JoinPaneForEqualWidth();
        Apply(true);
    }

    private void Start()
    {
        if (!isParented)
        {
            EnsureInOverlay();
            JoinPaneForEqualWidth();
            Apply(true);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (!isParented) EnsureInOverlay();
            JoinPaneForEqualWidth();
            Apply(true);
            return;
        }
#endif
        if (!isParented) EnsureInOverlay();

        var p = rect.parent;
        if (p != null)
        {
            int sibs = CountActiveChildren(p);
            if (sibs != lastSiblingCount)
            {
                JoinPaneForEqualWidth();
            }
        }

        if (Screen.safeArea != lastSafe || lastSize.x != Screen.width || lastSize.y != Screen.height)
        {
            Apply(false);
        }
    }

    // Parent under RightPane
    private void EnsureInOverlay()
    {
        var overlay = FindFirstObjectByType<CutoutOverlay>(FindObjectsInactive.Include);
        if (overlay == null) return;

        var target = overlay.RightPane;
        if (target == null) return;

        if (rect.parent != target)
        {
            rect.SetParent(target, false);
            isParented = true;
            lastSiblingCount = -1;
        }
        else
        {
            isParented = true;
        }
    }

    // Equal width logic similar to Clock
    private void JoinPaneForEqualWidth()
    {
        if (rect == null || rect.parent == null) return;

        var pane = rect.parent as RectTransform;
        var hlg = pane.GetComponent<HorizontalLayoutGroup>();
        int sibs = CountActiveChildren(pane);
        lastSiblingCount = sibs;

        if (hlg != null)
        {
            hlg.childForceExpandWidth = true;

            if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();
            if (layoutElement == null) layoutElement = gameObject.AddComponent<LayoutElement>();

            layoutElement.minWidth = -1;
            layoutElement.preferredWidth = -1;
            layoutElement.flexibleWidth = 1;

            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return;
        }

        if (sibs <= 0) sibs = 1;
        int myIndex = ActiveSiblingIndex(rect);
        float step = 1f / sibs;
        float aMinX = myIndex * step;
        float aMaxX = aMinX + step;

        rect.anchorMin = new Vector2(aMinX, 0f);
        rect.anchorMax = new Vector2(aMaxX, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // Layout
    private void Apply(bool force)
    {
        if (rect == null) return;

        lastSafe = Screen.safeArea;
        lastSize = new Vector2Int(Screen.width, Screen.height);

        // Offset from top-right inside our equal-width cell
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-margin.x, -margin.y);

        float topInsetPx = Mathf.Max(0f, Screen.height - Screen.safeArea.yMax);
        float topInsetUnits = PixelsToCanvasUnits(topInsetPx);
        float h = Mathf.Max(minHeight, topInsetUnits);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        // Width is driven by equal-share logic, so do not set sizeDelta.x
    }

    // Helpers
    private int CountActiveChildren(Transform t)
    {
        int c = 0;
        for (int i = 0; i < t.childCount; i++)
            if (t.GetChild(i).gameObject.activeInHierarchy) c++;
        return c;
    }

    private int ActiveSiblingIndex(Transform me)
    {
        var p = me.parent;
        int idx = 0;
        for (int i = 0; i < p.childCount; i++)
        {
            var ch = p.GetChild(i);
            if (!ch.gameObject.activeInHierarchy) continue;
            if (ch == me) return idx;
            idx++;
        }
        return 0;
    }

    private float PixelsToCanvasUnits(float pixels)
    {
        float s = EffectiveCanvasScale();
        return s <= 0f ? pixels : pixels / s;
    }

    private float EffectiveCanvasScale()
    {
        if (canvasScaler != null && rootCanvas != null)
        {
            switch (canvasScaler.uiScaleMode)
            {
                case CanvasScaler.ScaleMode.ConstantPixelSize:
                    return canvasScaler.scaleFactor > 0f ? canvasScaler.scaleFactor : 1f;

                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    {
                        Vector2 refRes = canvasScaler.referenceResolution;
                        if (refRes.x <= 0f || refRes.y <= 0f)
                            return rootCanvas.scaleFactor > 0f ? rootCanvas.scaleFactor : 1f;

                        if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
                        {
                            float w = Screen.width / refRes.x;
                            float h = Screen.height / refRes.y;
                            return Mathf.Min(w, h);
                        }
                        if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
                        {
                            float w = Screen.width / refRes.x;
                            float h = Screen.height / refRes.y;
                            return Mathf.Max(w, h);
                        }

                        float lw = Mathf.Log(Screen.width / refRes.x, 2f);
                        float lh = Mathf.Log(Screen.height / refRes.y, 2f);
                        float m = Mathf.Clamp01(canvasScaler.matchWidthOrHeight);
                        return Mathf.Pow(2f, Mathf.Lerp(lw, lh, m));
                    }

                case CanvasScaler.ScaleMode.ConstantPhysicalSize:
                    {
                        float dpi = Screen.dpi;
                        if (dpi <= 0f) dpi = 96f;
                        float refDpi = canvasScaler.fallbackScreenDPI > 0f ? canvasScaler.fallbackScreenDPI : 96f;
                        return dpi / refDpi;
                    }
            }
        }
        return (rootCanvas != null && rootCanvas.scaleFactor > 0f) ? rootCanvas.scaleFactor : 1f;
    }
}
