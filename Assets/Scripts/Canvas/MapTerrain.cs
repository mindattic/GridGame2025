using UnityEngine;
using UnityEngine.UI;

// MapTerrain is the single source of truth for collision sampling and gizmo visualization
// for the Terrain RawImage. It reads the displayed texture (and uvRect) and exposes
// helpers to sample walkability in the same space used by the hero (Content-local).
[ExecuteAlways]
public sealed class MapTerrain : MonoBehaviour
{
    [Header("Target (auto if omitted)")]
    [SerializeField] private RawImage terrainImage;          // RawImage that holds the collision mask
    [SerializeField] private RectTransform terrainRect;      // The Terrain RectTransform

    [Header("Sampling")]
    [Tooltip("Sample stride in source texture pixels (bigger = fewer points) for gizmos only.")]
    [SerializeField, Range(2, 256)] private int sampleStepPixels = 24;
    [Tooltip("Draw dots only for blocked pixels (pure black) to keep scene clear.")]
    [SerializeField] private bool drawOnlyBlocked = true;

    [Header("Gizmo Appearance")]
    [SerializeField] private Color blockedColor = new Color(1f, 0f, 0f, 0.9f);
    [SerializeField] private Color walkableColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField, Range(1f, 100f)] private float pointRadius = 8f;

    private Texture2D _tex;
    private Color32[] _pixels;
    private int _w, _h;

    // Change tracking for hot-reloading
    private Texture _lastTexture;
    private Rect _lastUvRect;
    private int _lastW, _lastH;

    private readonly Vector3[] _corners = new Vector3[4];

    private void OnEnable() { TryBind(true); }
    private void OnValidate() { TryBind(true); }
    private void Update() { TryBind(false); } // ExecuteAlways -> runs in edit and play

    // Draw a sampled grid of points showing blocked/walkable areas directly from the mask
    private void OnDrawGizmos()
    {
        // Ensure we’re sampling the current texture/uv before drawing
        TryBind(false);

        if (terrainRect == null || terrainImage == null || _tex == null || _pixels == null) return;

        // Rect corners in world space (Canvas-aware)
        terrainRect.GetWorldCorners(_corners);
        // Local axes from top-left
        Vector3 tl = _corners[1];
        Vector3 tr = _corners[2];
        Vector3 bl = _corners[0];

        Vector3 rightVec = tr - tl;
        Vector3 downVec = bl - tl;

        float width = rightVec.magnitude;
        float height = downVec.magnitude;
        if (width <= 1e-6f || height <= 1e-6f) return;

        Vector3 rightN = rightVec / width;
        Vector3 downN = downVec / height;

        // Respect RawImage.uvRect (sprite atlas or cropped)
        Rect uv = terrainImage.uvRect; // 0..1 space sub-rect

        // Choose sampling stride in UV from pixel stride
        int stepX = Mathf.Max(2, sampleStepPixels);
        int stepY = Mathf.Max(2, sampleStepPixels);

        int startX = Mathf.RoundToInt(uv.xMin * (_w - 1));
        int endX = Mathf.RoundToInt(uv.xMax * (_w - 1));
        int startY = Mathf.RoundToInt(uv.yMin * (_h - 1));
        int endY = Mathf.RoundToInt(uv.yMax * (_h - 1));

        for (int y = startY; y <= endY; y += stepY)
        {
            // Texture V runs bottom->top, but our Rect "down" runs top->bottom.
            // Flip only when RawImage.uvRect.height is positive to avoid double-flip.
            float vTex01 = Mathf.InverseLerp(uv.yMin, uv.yMax, (y + 0.5f) / (_h - 1));
            float vDown01 = (uv.height >= 0f) ? (1f - vTex01) : vTex01;

            for (int x = startX; x <= endX; x += stepX)
            {
                float u01 = Mathf.InverseLerp(uv.xMin, uv.xMax, (x + 0.5f) / (_w - 1));

                // Fetch pixel (clamped)
                int xi = Mathf.Clamp(x, 0, _w - 1);
                int yi = Mathf.Clamp(y, 0, _h - 1);
                Color32 px = _pixels[yi * _w + xi];

                bool blocked = IsBlockedColor(px);
                if (drawOnlyBlocked && !blocked) continue;

                // Convert (u,v) -> world point on the Terrain rect:
                Vector3 world = tl + (rightN * (u01 * width)) + (downN * (vDown01 * height));

                Gizmos.color = blocked ? blockedColor : walkableColor;
                Gizmos.DrawSphere(world, pointRadius);
            }
        }
    }

    // Forces a refresh (safe to call after OverworldManager assigns new sprites)
    public void ForceRefresh()
    {
        _lastTexture = null; // invalidate
        TryBind(true);
    }

    // Public API: check walkability at a Content-local point (same space as terrainRect.parent)
    public bool IsWalkableLocal(Vector2 local)
    {
        if (TrySamplePixelLocal(local, out var px))
            return !IsBlockedColor(px);
        return true; // fail-open if we cannot sample
    }

    // Public API: walkability with radial probes
    public bool IsWalkableLocal(Vector2 local, float probeRadius, int probeRays)
    {
        if (!IsWalkableLocal(local)) return false;

        if (probeRadius > 0f && probeRays > 0)
        {
            float step = 360f / probeRays;
            for (int i = 0; i < probeRays; i++)
            {
                float ang = step * i * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * probeRadius;
                if (!IsWalkableLocal(local + offset))
                    return false;
            }
        }
        return true;
    }

    // Public API: sample raw mask pixel at a Content-local point
    public bool TrySamplePixelLocal(Vector2 local, out Color32 color)
    {
        color = default;
        if (_tex == null || _pixels == null || _w <= 0 || _h <= 0 || terrainRect == null) return false;

        Vector2 uv = LocalToMaskUV(local);
        if (uv.x < 0f || uv.x > 1f || uv.y < 0f || uv.y > 1f)
            return false;

        int x = Mathf.Clamp(Mathf.RoundToInt(uv.x * (_w - 1)), 0, _w - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(uv.y * (_h - 1)), 0, _h - 1);
        int idx = y * _w + x;
        color = _pixels[idx];
        return true;
    }

    // A pixel is blocked when exactly pure-black in RGB
    public static bool IsBlockedColor(Color32 c)
    {
        return c.r == 0 && c.g == 0 && c.b == 0;
    }

    // Convert Content-local point to mask UV (respects uvRect and rotated/scaled rects)
    private Vector2 LocalToMaskUV(Vector2 local)
    {
        if (terrainRect == null) return Vector2.negativeInfinity;

        // Corners in world, then to parent local
        terrainRect.GetWorldCorners(_corners);
        var parent = terrainRect.parent as RectTransform;
        if (parent == null) return Vector2.negativeInfinity;

        Vector2 bl = parent.InverseTransformPoint(_corners[0]); // bottom-left
        Vector2 tl = parent.InverseTransformPoint(_corners[1]); // top-left
        Vector2 tr = parent.InverseTransformPoint(_corners[2]); // top-right

        Vector2 rightVec = tr - tl; float width = rightVec.magnitude;
        Vector2 downVec = bl - tl;  float height = downVec.magnitude;
        if (width <= 1e-6f || height <= 1e-6f) return Vector2.negativeInfinity;

        Vector2 rightN = rightVec / width;
        Vector2 downN = downVec / height;

        // Project from TL
        Vector2 toP = local - tl;
        float u01 = Mathf.Clamp01(Vector2.Dot(toP, rightN) / width);
        float v01 = Mathf.Clamp01(Vector2.Dot(toP, downN) / height); // top->0 .. bottom->1

        Rect uv = terrainImage != null ? terrainImage.uvRect : new Rect(0, 0, 1, 1);
        // Texture V grows up from bottom. Our v01 grows down from top.
        // Flip only when the uvRect height is positive, otherwise RawImage already flipped it.
        float vInput = (uv.height >= 0f) ? (1f - v01) : v01;

        return new Vector2(
            uv.x + u01 * uv.width,
            uv.y + vInput * uv.height
        );
    }

    // Bind to current RawImage/texture and cache pixels if readable
    private void TryBind(bool force)
    {
        if (terrainImage == null) terrainImage = GetComponent<RawImage>();
        if (terrainRect == null) terrainRect = GetComponent<RectTransform>();

        var rawTex = terrainImage != null ? terrainImage.texture : null;
        var tex2D = rawTex as Texture2D;
        Rect uv = terrainImage != null ? terrainImage.uvRect : new Rect(0, 0, 1, 1);

        bool texChanged = force || (rawTex != _lastTexture);
        bool uvChanged = force || !Approximately(_lastUvRect, uv);
        bool dimsChanged = force || (tex2D != null && (tex2D.width != _lastW || tex2D.height != _lastH));

        if (texChanged || uvChanged || dimsChanged)
        {
            _lastTexture = rawTex;
            _lastUvRect = uv;

            if (tex2D != null)
            {
                _tex = tex2D;
                _w = _tex.width;
                _h = _tex.height;
                _lastW = _w;
                _lastH = _h;

                try
                {
                    _pixels = _tex.GetPixels32(); // requires Read/Write Enabled
                }
                catch
                {
                    _pixels = null;
                }
            }
            else
            {
                _tex = null;
                _pixels = null;
                _w = _h = _lastW = _lastH = 0;
            }
        }
    }

    private static bool Approximately(Rect a, Rect b, float eps = 1e-6f)
    {
        return Mathf.Abs(a.x - b.x) <= eps &&
               Mathf.Abs(a.y - b.y) <= eps &&
               Mathf.Abs(a.width - b.width) <= eps &&
               Mathf.Abs(a.height - b.height) <= eps;
    }
}