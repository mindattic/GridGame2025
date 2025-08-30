using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public sealed class MapTerrainInstance : MonoBehaviour
{
    [Header("Target (auto if omitted)")]
    [SerializeField] private RawImage terrainImage;          // The Terrain RawImage that holds the collision mask
    [SerializeField] private RectTransform terrainRect;      // The Terrain RectTransform

    [Header("Sampling")]
    [Tooltip("Sample stride in source texture pixels (bigger = fewer points).")]
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

    // Change tracking
    private Texture _lastTexture;
    private Rect _lastUvRect;
    private int _lastW, _lastH;

    private readonly Vector3[] _corners = new Vector3[4];

    private void OnEnable() { TryBind(true); }
    private void OnValidate() { TryBind(true); }
    private void Update() { TryBind(false); } // ExecuteAlways -> runs in edit and play
    private void OnDrawGizmos()
    {
        // Make sure we’re sampling the current texture/uv before drawing
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

                bool blocked = (px.r == 0) && (px.g == 0) && (px.b == 0);
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