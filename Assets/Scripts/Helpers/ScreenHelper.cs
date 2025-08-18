using Game.Models;
using UnityEngine;
using c = Assets.Helpers.CanvasHelper;

namespace Assets.Helper
{

    /// <summary>
    /// ScreenHelper
    /// Purpose:
    ///   Centralizes safe, well-documented conversions between Screen, Viewport, World, and UI (Canvas) spaces.
    ///   All methods prefer explicit cameras and fall back to Camera.main if not provided.
    ///   Includes plane-based helpers for consistent perspective handling.
    ///
    /// Notes:
    ///   1) Screen space: pixels with origin at bottom-left. Range is [0..Screen.width, 0..Screen.height].
    ///   2) Viewport space: normalized coordinates with origin at bottom-left. Range is [0..1, 0..1].
    ///   3) World space: your scene coordinates.
    ///   4) UI space: depends on Canvas.renderMode:
    ///         - ScreenSpaceOverlay: UI lives in screen pixels.
    ///         - ScreenSpaceCamera: UI is driven by a UI camera, still pixel-aligned, but requires a camera in conversions.
    ///         - WorldSpace: UI lives directly in world coordinates.
    /// </summary>
    //public static class ScreenHelper
    //{
    //    // ------------------------------------------------------------------------
    //    // Camera utilities
    //    // ------------------------------------------------------------------------

    //    /// <summary>
    //    /// Returns a usable camera. If 'cam' is null, falls back to Camera.main.
    //    /// Throws a descriptive error if no camera is available.
    //    /// </summary>
    //    private static Camera Use(Camera cam)
    //    {
    //        cam = cam != null ? cam : Camera.main;
    //        if (cam == null)
    //            throw new System.InvalidOperationException("ScreenHelper requires a Camera. Pass one or ensure Camera.main exists.");
    //        return cam;
    //    }

    //    /// <summary>
    //    /// Returns the most appropriate camera for a Canvas.
    //    /// For ScreenSpaceOverlay, returns null (not required).
    //    /// For ScreenSpaceCamera, returns canvas.worldCamera if set, else Camera.main.
    //    /// For WorldSpace, returns canvas.worldCamera if set, else Camera.main.
    //    /// </summary>
    //    public static Camera CanvasCamera(Canvas canvas)
    //    {
    //        if (canvas == null) return null;
    //        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;

    //        if (canvas.worldCamera != null) return canvas.worldCamera;
    //        return Camera.main;
    //    }

    //    // ------------------------------------------------------------------------
    //    // Screen and viewport rectangles
    //    // ------------------------------------------------------------------------

    //    /// <summary>
    //    /// Returns the screen rectangle in pixels with origin at bottom-left.
    //    /// </summary>
    //    public static Rect ScreenPixelRect => new Rect(0f, 0f, Screen.width, Screen.height);

    //    /// <summary>
    //    /// Returns the viewport rectangle with origin at bottom-left, always (0,0,1,1).
    //    /// </summary>
    //    public static Rect ViewportUnitRect => new Rect(0f, 0f, 1f, 1f);

    //    /// <summary>
    //    /// Returns the exact camera frustum extents in world units at the camera near clip plane.
    //    /// Use WorldRectAtZ for a specific Z plane.
    //    /// </summary>
    //    public static Rect WorldRectAtNear(Camera cam = null)
    //    {
    //        cam = Use(cam);

    //        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
    //        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

    //        float minX = Mathf.Min(bl.x, tr.x);
    //        float maxX = Mathf.Max(bl.x, tr.x);
    //        float minY = Mathf.Min(bl.y, tr.y);
    //        float maxY = Mathf.Max(bl.y, tr.y);

    //        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    //    }

    //    /// <summary>
    //    /// Returns the world-space rectangle visible at a given world Z plane.
    //    /// For orthographic cameras, Z is ignored and the rect is exact.
    //    /// For perspective cameras, this projects the viewport corners onto a Plane at worldZ.
    //    /// </summary>
    //    public static Rect WorldRectAtZ(float worldZ, Camera cam = null)
    //    {
    //        cam = Use(cam);

    //        if (cam.orthographic)
    //        {
    //            // For ortho, any Z has the same XY extents. Sample at camera position Z.
    //            float z = cam.transform.position.z;
    //            Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
    //            Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

    //            float minX = Mathf.Min(bl.x, tr.x);
    //            float maxX = Mathf.Max(bl.x, tr.x);
    //            float minY = Mathf.Min(bl.y, tr.y);
    //            float maxY = Mathf.Max(bl.y, tr.y);

    //            return Rect.MinMaxRect(minX, minY, maxX, maxY);
    //        }
    //        else
    //        {
    //            // For perspective, intersect viewport corner rays with a horizontal plane at Z = worldZ.
    //            Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, worldZ));

    //            Vector2[] corners =
    //            {
    //            new Vector2(0f, 0f), new Vector2(1f, 0f),
    //            new Vector2(0f, 1f), new Vector2(1f, 1f)
    //        };

    //            float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
    //            float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;

    //            foreach (var v in corners)
    //            {
    //                Ray r = cam.ViewportPointToRay(new Vector3(v.x, v.y, 0f));
    //                if (plane.Raycast(r, out float t))
    //                {
    //                    Vector3 p = r.GetPoint(t);
    //                    minX = Mathf.Min(minX, p.x);
    //                    maxX = Mathf.Max(maxX, p.x);
    //                    minY = Mathf.Min(minY, p.y);
    //                    maxY = Mathf.Max(maxY, p.y);
    //                }
    //            }

    //            if (!float.IsFinite(minX) || !float.IsFinite(maxX) || !float.IsFinite(minY) || !float.IsFinite(maxY))
    //                return new Rect(0, 0, 0, 0);

    //            return Rect.MinMaxRect(minX, minY, maxX, maxY);
    //        }
    //    }

    //    // ------------------------------------------------------------------------
    //    // Legacy-style world rect helper retained for parity with your RectFloat usage
    //    // ------------------------------------------------------------------------

    //    /// <summary>
    //    /// Returns a RectFloat of the exact visible area of 'cam' in world units at near clip (ortho exact).
    //    /// </summary>
    //    public static RectFloat ScreenInWorldUnits(Camera cam = null)
    //    {
    //        cam = Use(cam);

    //        float z = cam.orthographic ? 0f : cam.nearClipPlane;
    //        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, z));
    //        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, z));

    //        float minX = Mathf.Min(bl.x, tr.x);
    //        float maxX = Mathf.Max(bl.x, tr.x);
    //        float minY = Mathf.Min(bl.y, tr.y);
    //        float maxY = Mathf.Max(bl.y, tr.y);

    //        return new RectFloat(minX, maxX, maxY, minY);
    //    }

    //    /// <summary>
    //    /// Symmetric approximation of the camera world rect when the camera is centered at origin.
    //    /// Accurate for centered orthographic cameras only.
    //    /// </summary>
    //    public static RectFloat CenteredScreenWorldRect
    //    {
    //        get
    //        {
    //            var cam = Use(Camera.main);

    //            if (!cam.orthographic)
    //                Debug.LogWarning("CenteredScreenWorldRect assumes an orthographic camera.");

    //            Vector2 topRight = cam.ViewportToWorldPoint(new Vector2(1f, 1f));
    //            float width = topRight.x * 2f;
    //            float height = topRight.y * 2f;
    //            return new RectFloat(0f, width, height, 0f);
    //        }

    //    }



    //    /// <summary>
    //    /// Places a UI RectTransform so its pivot aligns with a given screen-space point.
    //    /// Works for Screen Space - Overlay (uiCam = null) and Screen Space - Camera.
    //    /// </summary>
    //    public static Vector2 GetScreenPosition(RectTransform rect, Vector2 screenPoint)
    //    {
    //        Vector2 localPos;
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //            c.CanvasRect,
    //            screenPoint,
    //            null,              // Overlay canvas: must be null
    //            out localPos
    //        );

    //        return localPos;
    //    }


    //    public static class Convert
    //    {

    //        /// <summary>
    //        /// Converts a UI Transform (RectTransform) to a world position on a specified Z plane in world space.
    //        /// </summary>
    //        public static Vector3 CanvasToWorldPosition(Transform uiTransform, float worldPlaneZ = 0f)
    //        {
    //            // Convert UI object's position to screen space
    //            Vector3 screenPos = UnityEngine.RectTransformUtility.WorldToScreenPoint(null, uiTransform.position);

    //            // Distance from camera to target world Z plane
    //            float cameraZ = Camera.main.transform.position.z;
    //            float distance = Mathf.Abs(worldPlaneZ - cameraZ);

    //            // Convert to world position
    //            return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));
    //        }



    //        // -------- World <-> Screen --------

    //        /// <summary>
    //        /// World to Screen (pixels). Origin is bottom-left. Z is distance from camera.
    //        /// </summary>
    //        public static Vector3 WorldToScreenPosition(Vector3 worldPos, Camera cam = null)
    //        {
    //            cam = Use(cam);
    //            return cam.WorldToScreenPoint(worldPos);
    //        }

    //        /// <summary>
    //        /// Screen (pixels) to World at a specific distance from camera.
    //        /// For perspective, 'distanceFromCamera' is along the camera forward axis.
    //        /// For orthographic, Z becomes world Z directly.
    //        /// </summary>
    //        public static Vector3 ScreenToWorldByDistance(Vector2 screenPos, float distanceFromCamera, Camera cam = null)
    //        {
    //            cam = Use(cam);
    //            return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
    //        }

    //        /// <summary>
    //        /// Screen (pixels) to World on a plane at worldZ. Works for both orthographic and perspective.
    //        /// Perspective: casts a ray and intersects a Z-plane.
    //        /// Orthographic: returns ScreenToWorldPoint with Z = worldZ.
    //        /// </summary>
    //        public static Vector3 ScreenToWorldOnPlaneZ(Vector2 screenPos, float worldZ, Camera cam = null)
    //        {
    //            cam = Use(cam);

    //            if (cam.orthographic)
    //                return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, worldZ));

    //            Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, worldZ));
    //            Ray r = cam.ScreenPointToRay(screenPos);
    //            if (plane.Raycast(r, out float t))
    //                return r.GetPoint(t);

    //            return Vector3.zero;
    //        }

    //        /// <summary>
    //        /// Creates a world ray from a screen pixel position. Useful for 3D picks and plane intersections.
    //        /// </summary>
    //        public static Ray ScreenPointToWorldRay(Vector2 screenPos, Camera cam = null)
    //        {
    //            cam = Use(cam);
    //            return cam.ScreenPointToRay(screenPos);
    //        }

    //        // -------- World <-> Viewport --------

    //        /// <summary>
    //        /// World to Viewport. Returns normalized coordinates in [0..1] on X and Y, Z is distance.
    //        /// </summary>
    //        public static Vector3 WorldToViewportPosition(Vector3 worldPos, Camera cam = null)
    //        {
    //            cam = Use(cam);
    //            return cam.WorldToViewportPoint(worldPos);
    //        }

    //        /// <summary>
    //        /// Viewport to World at a specific distance from camera.
    //        /// </summary>
    //        public static Vector3 ViewportToWorldByDistance(Vector2 viewportPos, float distanceFromCamera, Camera cam = null)
    //        {
    //            cam = Use(cam);
    //            return cam.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, distanceFromCamera));
    //        }

    //        /// <summary>
    //        /// Viewport to World on a plane at worldZ. Perspective uses a ray-plane test.
    //        /// </summary>
    //        public static Vector3 ViewportToWorldOnPlaneZ(Vector2 viewportPos, float worldZ, Camera cam = null)
    //        {
    //            cam = Use(cam);

    //            if (cam.orthographic)
    //            {
    //                Vector3 p = cam.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 0f));
    //                p.z = worldZ;
    //                return p;
    //            }

    //            Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, worldZ));
    //            Ray r = cam.ViewportPointToRay(new Vector3(viewportPos.x, viewportPos.y, 0f));
    //            if (plane.Raycast(r, out float t))
    //                return r.GetPoint(t);

    //            return Vector3.zero;
    //        }

    //        // -------- Screen <-> Viewport --------

    //        /// <summary>
    //        /// Screen pixels to normalized Viewport.
    //        /// </summary>
    //        public static Vector2 ScreenToViewport(Vector2 screenPos)
    //        {
    //            return new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
    //        }

    //        /// <summary>
    //        /// Viewport to Screen pixels.
    //        /// </summary>
    //        public static Vector2 ViewportToScreen(Vector2 viewportPos)
    //        {
    //            return new Vector2(viewportPos.x * Screen.width, viewportPos.y * Screen.height);
    //        }

    //        // -------- UI (Canvas) conversions --------

    //        /// <summary>
    //        /// UI Transform world position to screen pixels. Works for any Canvas Render mode.
    //        /// </summary>
    //        public static Vector2 UIToScreenPoint(Transform uiTransform, Canvas canvas = null)
    //        {
    //            Camera uiCam = CanvasCamera(canvas);
    //            return WorldToScreenPoint(uiCam, uiTransform.position);
    //        }

    //        /// <summary>
    //        /// Converts a screen position (pixels) to a local position inside the given RectTransform.
    //        /// Works for both ScreenSpaceOverlay and ScreenSpaceCamera canvases.
    //        /// </summary>
    //        /// <param name="rect">The target RectTransform.</param>
    //        /// <param name="screenPos">The screen position in pixels.</param>
    //        /// <param name="uiCam">The camera rendering the UI (null for ScreenSpaceOverlay).</param>
    //        /// <param name="localPoint">Resulting local position inside the RectTransform.</param>
    //        /// <returns>True if the point is inside the rectangle, otherwise false.</returns>
    //        public static bool ScreenPointToLocalPointInRectangle(
    //            RectTransform rect,
    //            Vector2 screenPos,
    //            Camera uiCam,
    //            out Vector2 localPoint)
    //        {
    //            return UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, uiCam, out localPoint);
    //        }

    //        /// <summary>
    //        /// Screen pixels to UI local point inside 'targetRect'. Works for ScreenSpaceOverlay or ScreenSpaceCamera.
    //        /// Returns true if the point is inside 'targetRect'.
    //        /// </summary>
    //        public static bool ScreenToUILocalPoint(RectTransform targetRect, Vector2 screenPos, Canvas canvas, out Vector2 localPoint)
    //        {
    //            Camera uiCam = CanvasCamera(canvas);
    //            return ScreenPointToLocalPointInRectangle(targetRect, screenPos, uiCam, out localPoint);
    //        }

    //        /// <summary>
    //        /// World to UI local point inside 'targetRect'. Useful for placing UI markers above world objects.
    //        /// </summary>
    //        public static bool WorldToUILocalPoint(RectTransform targetRect, Vector3 worldPos, Canvas canvas, Camera worldCam, out Vector2 localPoint)
    //        {
    //            worldCam = Use(worldCam);
    //            Vector2 screen = WorldToScreenPosition(worldPos, worldCam);
    //            return ScreenToUILocalPoint(targetRect, screen, canvas, out localPoint);
    //        }

    //        /// <summary>
    //        /// UI local point (in 'sourceRect') to screen pixels.
    //        /// </summary>
    //        public static Vector2 UILocalToScreenPoint(RectTransform sourceRect, Vector2 localPoint, Canvas canvas)
    //        {
    //            Camera uiCam = CanvasCamera(canvas);
    //            Vector2 screen = LocalPointToScreenPoint(sourceRect, localPoint, uiCam);
    //            return screen;
    //        }

    //        /// <summary>
    //        /// Converts a UI element position to a world position on a target Z plane.
    //        /// For perspective, this uses the world camera for ray-plane intersection.
    //        /// For orthographic, it projects screen to world and sets Z.
    //        /// </summary>
    //        public static Vector3 UIToWorldOnPlaneZ(Transform uiTransform, float worldZ, Canvas canvas, Camera worldCam = null)
    //        {
    //            // Get screen position of UI
    //            Vector2 screen = UIToScreenPoint(uiTransform, canvas);

    //            // Project into world on the requested plane
    //            worldCam = Use(worldCam != null ? worldCam : Camera.main);
    //            return ScreenToWorldOnPlaneZ(screen, worldZ, worldCam);
    //        }

    //        /// <summary>
    //        /// For WorldSpace canvas only: UI world position is already in world coordinates.
    //        /// Provided for parity and clarity.
    //        /// </summary>
    //        public static Vector3 UIWorldSpaceToWorld(Transform uiTransform)
    //        {
    //            return uiTransform.position;
    //        }

    //        // -------- Internal helpers for UI conversions --------

    //        private static Vector2 WorldToScreenPoint(Camera cam, Vector3 worldPos)
    //        {
    //            if (cam == null) return Camera.main != null ? Camera.main.WorldToScreenPoint(worldPos) : Vector2.zero;
    //            return cam.WorldToScreenPoint(worldPos);
    //        }

    //        private static Vector2 LocalPointToScreenPoint(RectTransform rect, Vector2 localPoint, Camera uiCam)
    //        {
    //            Vector2 screen = Vector2.zero;

    //            // Transform the local point to world, then world to screen
    //            Vector3 world = rect.TransformPoint(localPoint);
    //            if (uiCam != null) screen = uiCam.WorldToScreenPoint(world);
    //            else if (Camera.main != null) screen = Camera.main.WorldToScreenPoint(world);

    //            return screen;
    //        }
    //    }

    //    // ------------------------------------------------------------------------
    //    // Canvas sizing and safe areas
    //    // ------------------------------------------------------------------------

    //    /// <summary>
    //    /// Returns the Canvas pixel rect in screen coordinates. For Overlay, this is the screen size.
    //    /// For Camera and World, this returns the RectTransform rect projected into pixels when possible.
    //    /// </summary>
    //    public static Rect CanvasPixelRect(Canvas canvas)
    //    {
    //        if (canvas == null) return ScreenPixelRect;

    //        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
    //        {
    //            return ScreenPixelRect;
    //        }

    //        // For ScreenSpaceCamera or WorldSpace, approximate using the root RectTransform
    //        RectTransform rt = canvas.GetComponent<RectTransform>();
    //        if (rt == null) return ScreenPixelRect;

    //        // Build screen-space rect by converting the four corners
    //        Vector3[] corners = new Vector3[4];
    //        rt.GetWorldCorners(corners);

    //        Camera uiCam = CanvasCamera(canvas);
    //        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
    //        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

    //        for (int i = 0; i < 4; i++)
    //        {
    //            Vector2 sp = Convert.WorldToScreenPosition(corners[i], uiCam);
    //            min = Vector2.Min(min, sp);
    //            max = Vector2.Max(max, sp);
    //        }

    //        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    //    }

    //    /// <summary>
    //    /// Convenience clamp that keeps a world position inside a board-like RectFloat.
    //    /// Preserves Z.
    //    /// </summary>
    //    public static Vector3 ClampWorldToRectFloat(Vector3 worldPos, RectFloat r)
    //    {
    //        float z = worldPos.z;
    //        worldPos.x = Mathf.Clamp(worldPos.x, r.Left, r.Right);
    //        worldPos.y = Mathf.Clamp(worldPos.y, r.Bottom, r.Top);
    //        worldPos.z = z;
    //        return worldPos;
    //    }

    //    /// <summary>
    //    /// Convenience clamp for screen pixels inside the screen bounds.
    //    /// </summary>
    //    public static Vector2 ClampScreenToBounds(Vector2 screenPos)
    //    {
    //        screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
    //        screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
    //        return screenPos;
    //    }
    //}
}