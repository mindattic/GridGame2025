using Assets.Helper;
using UnityEngine;

// MapTerrain is the single source of truth for collision sampling and gizmo visualization
// for a world-space SpriteRenderer map. It reads the sprite's texture (atlas aware via textureRect)
// and exposes helpers to sample walkability in world space (the same space used by the hero).
[ExecuteAlways]
public sealed class MapTerrain : MonoBehaviour
{
    // Target (auto)
    private SpriteRenderer terrainSprite;   // SpriteRenderer for world-space mode

    // Gizmo sampling (debug only)
    private int sampleStepPixels = 24;                       // Sample stride in source texture pixels (gizmos only)
    private bool drawOnlyBlocked = true;                     // Draw only blocked pixels to keep scene clear

    // Gizmo appearance
    private Color blockedColor = new Color(1f, 0f, 0f, 0.9f);
    private Color walkableColor = new Color(0f, 1f, 0f, 0.25f);
    private float worldPointRadius = 0.02f; // world-space gizmo radius

    private Texture2D _tex;
    private Color32[] _pixels;
    private int _w, _h;

    // Change tracking for hot-reloading
    private Texture _lastTexture;
    private int _lastW, _lastH;

    private void OnEnable() { TryBind(true); }
    private void OnValidate() { TryBind(true); }
    private void Update() { TryBind(false); } // ExecuteAlways -> runs in edit and play

    // Draw a sampled grid of points showing blocked/walkable areas directly from the mask
    private void OnDrawGizmos()
    {
        TryBind(false); // ensure current texture cached
        if (_tex == null || _pixels == null) return;
        if (terrainSprite == null || terrainSprite.sprite == null) return;
        DrawSpriteGizmos();
    }

    private void DrawSpriteGizmos()
    {
        var sr = terrainSprite;
        var sp = sr.sprite;
        Rect tr = sp.textureRect; // pixel rect in atlas/texture
        float ppu = sp.pixelsPerUnit > 0f ? sp.pixelsPerUnit : 100f;
        Vector2 pivotPx = sp.pivot; // pivot in pixels within sprite rect

        int stepX = Mathf.Max(2, sampleStepPixels);
        int stepY = Mathf.Max(2, sampleStepPixels);

        int xMin = Mathf.RoundToInt(tr.xMin);
        int xMax = Mathf.RoundToInt(tr.xMax);
        int yMin = Mathf.RoundToInt(tr.yMin);
        int yMax = Mathf.RoundToInt(tr.yMax);

        for (int y = yMin; y <= yMax; y += stepY)
        {
            for (int x = xMin; x <= xMax; x += stepX)
            {
                int xi = Mathf.Clamp(x, 0, _w - 1);
                int yi = Mathf.Clamp(y, 0, _h - 1);
                Color32 px = _pixels[yi * _w + xi];
                bool blocked = IsBlockedColor(px);
                if (drawOnlyBlocked && !blocked) continue;

                // Sample at pixel center (+0.5)
                float xCenter = Mathf.Clamp(x + 0.5f, tr.xMin, tr.xMax);
                float yCenter = Mathf.Clamp(y + 0.5f, tr.yMin, tr.yMax);

                // Convert pixel -> local units relative to pivot (Unity Y-up)
                float lx = (xCenter - tr.xMin - pivotPx.x) / ppu;
                float ly = (yCenter - tr.yMin - pivotPx.y) / ppu;

                // Account for SpriteRenderer flips (flipX/flipY are not in Transform)
                if (sr.flipX) lx = -lx;
                if (sr.flipY) ly = -ly;

                Vector3 local = new Vector3(lx, ly, 0f);
                Vector3 world = sr.transform.TransformPoint(local);

                Gizmos.color = blocked ? blockedColor : walkableColor;
                Gizmos.DrawSphere(world, Mathf.Max(0.001f, worldPointRadius));
            }
        }
    }

    // Forces a refresh (safe to call after OverworldManager assigns new sprites)
    public void ForceRefresh()
    {
        _lastTexture = null; // invalidate
        TryBind(true);
    }

    // Public API: check walkability at a world-space point
    public bool IsWalkableLocal(Vector2 p)
    {
        if (TrySamplePixelLocal(p, out var px))
            return !IsBlockedColor(px);
        return true; // fail-open if we cannot sample (e.g., texture not readable)
    }

    // Public API: walkability with radial probes
    public bool IsWalkableLocal(Vector2 p, float probeRadius, int probeRays)
    {
        if (!IsWalkableLocal(p)) return false;

        if (probeRadius > 0f && probeRays > 0)
        {
            float step = 360f / probeRays;
            for (int i = 0; i < probeRays; i++)
            {
                float ang = step * i * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * probeRadius;
                if (!IsWalkableLocal(p + offset))
                    return false;
            }
        }
        return true;
    }

    // Public API: sample raw mask pixel at a world-space point
    public bool TrySamplePixelLocal(Vector2 p, out Color32 color)
    {
        color = default;
        if (_tex == null || _pixels == null || _w <= 0 || _h <= 0) return false;
        if (terrainSprite == null || terrainSprite.sprite == null) return false;

        var sr = terrainSprite;
        var sp = sr.sprite;
        Rect tr = sp.textureRect; // pixel rect in atlas
        float ppu = Mathf.Max(1f, sp.pixelsPerUnit);
        Vector2 pivotPx = sp.pivot;

        // Transform world -> local (units, pivot at origin)
        Vector3 local = sr.transform.InverseTransformPoint(new Vector3(p.x, p.y, sr.transform.position.z));

        // Account for SpriteRenderer flips
        if (sr.flipX) local.x = -local.x;
        if (sr.flipY) local.y = -local.y;

        // Convert local units -> pixel coordinates relative to texture
        float px = tr.xMin + pivotPx.x + (local.x * ppu);
        float py = tr.yMin + pivotPx.y + (local.y * ppu);

        int xi = Mathf.RoundToInt(px);
        int yi = Mathf.RoundToInt(py);
        if (xi < 0 || yi < 0 || xi >= _w || yi >= _h) return false;
        color = _pixels[yi * _w + xi];
        return true;
    }

    // A pixel is blocked when exactly pure-black in RGB
    public static bool IsBlockedColor(Color32 c)
    {
        return c.r == 0 && c.g == 0 && c.b == 0;
    }

    // Bind to current target and cache pixels if readable
    private void TryBind(bool force)
    {
        if (terrainSprite == null)
        {
            var terrainGo = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain);
            terrainSprite = terrainGo != null ? terrainGo.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        }

        Texture rawTex = null;
        if (terrainSprite != null && terrainSprite.sprite != null)
        {
            rawTex = terrainSprite.sprite.texture;
        }

        var tex2D = rawTex as Texture2D;

        bool texChanged = force || (rawTex != _lastTexture);
        bool dimsChanged = force || (tex2D != null && (tex2D.width != _lastW || tex2D.height != _lastH));

        if (texChanged || dimsChanged)
        {
            _lastTexture = rawTex;

            if (tex2D != null)
            {
                _tex = tex2D;
                _w = _tex.width;
                _h = _tex.height;
                _lastW = _w;
                _lastH = _h;

                try
                {
                    _pixels = _tex.GetPixels32(); // requires Read/Write Enabled in import settings
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

    // Estimate an outward normal (away from blocked area) near a given point.
    // Returns Vector2.zero if no blocked samples are nearby.
    public Vector2 EstimateObstacleNormal(Vector2 p, float sampleRadius = 0.2f, int rays = 8)
    {
        if (sampleRadius <= 0f || rays <= 0) return Vector2.zero;

        // Accumulate directions toward blocked samples
        Vector2 towardBlocked = Vector2.zero;
        float step = 360f / rays;

        for (int i = 0; i < rays; i++)
        {
            float ang = step * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
            Vector2 s = p + dir * sampleRadius;

            if (!IsWalkableLocal(s))
            {
                towardBlocked += dir; // points toward blocked region
            }
        }

        if (towardBlocked.sqrMagnitude < 1e-6f)
            return Vector2.zero;

        // Outward normal is away from blocked
        return (-towardBlocked).normalized;
    }
}