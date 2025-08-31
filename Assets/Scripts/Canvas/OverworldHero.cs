using Assets.Helper;
using Assets.Helpers;
using UnityEngine;
using UnityEngine.UI;

public enum OverworldHeroInputMode
{
    ClickToMove,
    DirectionalPress
}

// OverworldHero reads inputs (fed by manager), moves within map bounds,
// and updates the Animator parameters (MoveX, MoveY, Speed) for an 8-way blend tree.
// Walkability and collision checks are delegated to MapTerrain.
public class OverworldHero : MonoBehaviour
{
    public RectTransform rect;           // The hero icon RectTransform (anchored to Content)
    public Animator animator;            // Animator that uses MoveX, MoveY, Speed
    private float moveSpeed = 128f;      // Canvas units per second

    [Header("Bindings")]
    [SerializeField] private RectTransform mapRect;   // Terrain rect for clamping movement
    [SerializeField] private RectTransform viewport;  // ScrollRect viewport; used for visibility gating
    [SerializeField] private MapTerrain collisionProvider; // Central collision provider on Terrain

    public bool AllowClickToMove { get; set; } = true;
    public event System.Action<Vector2> OnHeroMoved;  // Invoked every time hero position changes

    [Header("Tuning")]
    [SerializeField] private float snapThreshold = 0.24f; // Snap-to-target distance (for MoveToPoint)
    [SerializeField] private bool requireVisibleToMove = true;          // Only step when visible
    [SerializeField] private bool ignoreClicksWhenOffscreen = false;    // Ignore taps when hero offscreen
    [SerializeField] private bool allowVirtualJoystick = true;          // Enable joystick/analog movement
    [SerializeField] private bool idleWhileOffscreen = true;            // Idle when offscreen and movement gated

    [SerializeField, Tooltip("How far ahead (in step length units) to sample for speed/slow zones. Typical: 0.5-1.0")]
    private float speedSampleAheadFactor = 0.7f;

    [Header("Click Movement Mode")]
    [SerializeField] private OverworldHeroInputMode inputMode = OverworldHeroInputMode.ClickToMove;
    [SerializeField] private float directionalClickMagnitude = 1f; // 0..1 strength fed into analog

    // Expose mode to other systems (e.g., OverworldManager)
    public OverworldHeroInputMode TouchMoveMode
    {
        get => inputMode;
        set => inputMode = value;
    }

    [Header("Collision Probing")]
    [Tooltip("Probe radius in canvas units around hero to reduce corner clipping.")]
    [SerializeField] private float collisionProbeRadius = 0f;
    [Tooltip("Number of radial probes around the hero in addition to center (0 enables an automatic 8-ray ring).")]
    [SerializeField] private int collisionProbeRays = 0;

    // Optional slow-zone tuning (currently no effect; keep for future)
    [Header("Slow Zones (optional)")]
    [SerializeField] private bool useSlowZones = false;
    [SerializeField, Range(0f, 1f)] private float slowBandCenter = 0.5f;
    [SerializeField, Range(0f, 1f)] private float slowBandHalfWidth = 0.1f;
    [SerializeField, Range(0.05f, 1f)] private float slowMultiplier = 0.5f;

    [Header("Collision Debug (Scene view)")]
    [SerializeField] private bool debugCollisionGizmos = true;
    [SerializeField] private float debugGizmoSize = 6f;

    [Header("Destination Marker")]
    [SerializeField] private RectTransform destinationMarker;                    // Optional (auto-created)
    [SerializeField] private RectTransform destinationMarkerPrefab;   // If set, this prefab is used
    [SerializeField] private bool tintDestinationMarkerPrefab = false; // Tint all child Images to destinationMarkerColor
    [SerializeField] private bool destinationMarkerEnabled = true;
    [SerializeField] private Color destinationMarkerColor = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField, Range(4f, 64f)] private float destinationMarkerSize = 16f;
    [SerializeField, Range(1f, 6f)] private float destinationMarkerThickness = 2f;

    // Fade settings
    [SerializeField] private bool destinationMarkerFade = true;
    [SerializeField, Min(0f)] private float destinationMarkerFadeStart = 128f;  // distance where alpha  1
    [SerializeField, Min(0f)] private float destinationMarkerFadeEnd = 16f;     // distance where alpha  0
    [SerializeField, Range(0f, 1f)] private float destinationMarkerMinAlpha = 0.05f;
    [SerializeField, Min(0f)] private float destinationMarkerFadeSpeed = 8f;    // alpha units/sec

    // Runtime cache
    private Image[] _markerImages;
    private float _markerAlpha;

    private bool isMoving;                 // True while following a MoveToPoint target
    private Vector2 targetPosition;        // Destination for MoveToPoint mode

    // The project uses a MoveDirection enum elsewhere for save/load and basic facing.
    // We continue to maintain it, but animation is driven by lastLook for 8-way.
    private MoveDirection lastDirection = MoveDirection.Idle;

    // Analog input (-1..1). Set by OverworldManager each frame.
    private Vector2 analogInput;

    // Directional click override (-1..1). Latched while pressed.
    private bool directionalActive;
    private Vector2 directionalOverride;

    // 8-way facing memory for idle pose. Defaults to down.
    private Vector2 lastLook = Vector2.down;

    // Expose current facing as string for saving
    public string CurrentFacingName => lastDirection.ToString();
    public bool IsMoving => isMoving;
    private readonly Vector3[] _mapWorldCorners = new Vector3[4];

    // ---------------- Pathfinding (A*) ----------------
    [Header("ClickToMove Pathfinding")]
    [SerializeField] private bool usePathfinding = true;
    [SerializeField, Tooltip("Navigation grid cell size in canvas units (coarser = faster).")]
    private int navCellSize = 16;
    [SerializeField, Tooltip("Extra clearance from walls in canvas units to avoid hugging.")]
    private float navObstacleBuffer = 6f;
    [SerializeField, Tooltip("Max nodes the solver will expand before giving up.")]
    private int navMaxExpanded = 8000;
    [SerializeField, Tooltip("How close we must get to a waypoint before advancing.")]
    private float waypointArrive = 6f;

    private System.Collections.Generic.List<Vector2> _path; // local-space waypoints (centers)
    private int _pathIndex;

    // ----------------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------------

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (animator == null) animator = GetComponent<Animator>();

        // Auto-find Terrain for clamping if not assigned
        if (mapRect == null)
        {
            var go = GameObject.Find(GameObjectHelper.Overworld.Terrain);
            if (go != null) mapRect = go.GetComponent<RectTransform>();
        }

        // Auto-find viewport for visibility gating if not assigned
        if (viewport == null)
        {
            var vp = GameObject.Find(GameObjectHelper.Overworld.Viewport);
            if (vp != null) viewport = vp.GetComponent<RectTransform>();
        }

        // Auto-bind collision provider from the map rect if present
        if (collisionProvider == null && mapRect != null)
            collisionProvider = mapRect.GetComponent<MapTerrain>();

        // Initialize animator with default idle facing
        ApplyAnimatorParameters(lastLook, 0f);
    }

    private void Update()
    {
        if (rect == null) return;

        // Analog (joystick) has priority; otherwise use directional click override if any
        Vector2 effectiveInput = Vector2.zero;
        bool joystickActive = allowVirtualJoystick && (analogInput.sqrMagnitude > 1e-6f);
        if (joystickActive)
        {
            effectiveInput = analogInput;
            directionalActive = false; // joystick cancels directional latch
            HideDestinationMarker();
        }
        else if (allowVirtualJoystick && directionalActive && directionalOverride.sqrMagnitude > 1e-6f)
        {
            effectiveInput = directionalOverride;
            HideDestinationMarker();
        }

        // 1) Analog-like move (joystick or directional click)
        if (effectiveInput.sqrMagnitude > 1e-6f)
        {
            if (requireVisibleToMove && !IsTargetVisible(rect, viewport))
            {
                if (idleWhileOffscreen) SetIdle();
                return;
            }

            Vector2 current = rect.anchoredPosition;

            float inputMag = Mathf.Clamp01(effectiveInput.magnitude);
            Vector2 dir = inputMag > 1e-6f ? effectiveInput / inputMag : Vector2.zero;
            float baseStepLen = moveSpeed * Time.deltaTime * inputMag;

            float mult = GetSpeedMultiplierLocal(current + dir * (baseStepLen * speedSampleAheadFactor));
            Vector2 step = dir * (baseStepLen * mult);

            Vector2 desired = ClampToMap(current + step);
            Vector2 next = ResolveCollision(current, desired);
            Vector2 frameDelta = next - current;

            if (frameDelta.sqrMagnitude > 1e-6f)
            {
                SetAnimation(frameDelta);
            }
            else
            {
                SetIdle();
            }

            rect.anchoredPosition = next;
            OnHeroMoved?.Invoke(next);

            isMoving = false; // cancel click path while analog/dir is active
            _path = null; // cancel any existing path
            return;
        }

        // 2) No analog-like input -> force Idle when not using click-to-move to a point
        if (!AllowClickToMove)
        {
            SetIdle();
            isMoving = false;
            HideDestinationMarker();
            _path = null;
            return;
        }

        // 3) MoveToPoint path (only when mode is ClickToMove)
        if (inputMode != OverworldHeroInputMode.ClickToMove || !isMoving)
        {
            SetIdle();
            return;
        }

        if (requireVisibleToMove && !IsTargetVisible(rect, viewport))
        {
            if (idleWhileOffscreen) SetIdle();
            return;
        }

        Vector2 cur = rect.anchoredPosition;

        // If we have a path, walk it waypoint-by-waypoint
        if (_path != null && _pathIndex < _path.Count)
        {
            Vector2 wp = _path[_pathIndex];
            Vector2 toWp = wp - cur;
            float distWp = toWp.magnitude;

            if (distWp <= Mathf.Max(waypointArrive, snapThreshold))
            {
                // Advance to next waypoint
                _pathIndex++;
                if (_pathIndex >= _path.Count)
                {
                    // Reached final waypoint -> finish
                    rect.anchoredPosition = wp;
                    OnHeroMoved?.Invoke(wp);
                    isMoving = false;
                    SetIdle();
                    HideDestinationMarker();
                    return;
                }
                wp = _path[_pathIndex];
                toWp = wp - cur;
                distWp = toWp.magnitude;
            }

            // Step toward current waypoint, clamped to avoid overshoot
            float maxStep = moveSpeed * Time.deltaTime;
            Vector2 dir = distWp > 1e-6f ? (toWp / distWp) : Vector2.zero;
            float moveMult = GetSpeedMultiplierLocal(cur + dir * (maxStep * speedSampleAheadFactor));
            float stepLen = Mathf.Min(maxStep * moveMult, distWp);
            Vector2 desiredNext = ClampToMap(cur + dir * stepLen);
            Vector2 nextPos = ResolveCollision(cur, desiredNext);
            Vector2 delta = nextPos - cur;

            if (delta.sqrMagnitude > 1e-6f)
            {
                SetAnimation(delta);
                rect.anchoredPosition = nextPos;
                OnHeroMoved?.Invoke(nextPos);
                UpdateDestinationMarkerVisuals();
            }
            else
            {
                // Could not advance -> abort path
                _path = null;
                isMoving = false;
                SetIdle();
                HideDestinationMarker();
            }
            return;
        }

        // No path -> direct straight-line finish with overshoot clamp
        // Arrived (snap and finish)
        if (Vector2.Distance(cur, targetPosition) <= snapThreshold)
        {
            rect.anchoredPosition = targetPosition;
            OnHeroMoved?.Invoke(targetPosition);

            isMoving = false;
            SetIdle();
            HideDestinationMarker();
            return;
        }

        // Advance towards target and animate (stepwise, with collision)
        Vector2 toTarget = targetPosition - cur;
        float dist = toTarget.magnitude;
        float maxStep2 = moveSpeed * Time.deltaTime;
        Vector2 stepDir = dist > 1e-6f ? (toTarget / dist) : Vector2.zero;

        float moveMult2 = GetSpeedMultiplierLocal(cur + stepDir * (maxStep2 * speedSampleAheadFactor));
        float stepLen2 = Mathf.Min(maxStep2 * moveMult2, dist);
        Vector2 stepVec = stepDir * stepLen2;

        Vector2 desiredNext2 = ClampToMap(cur + stepVec);
        Vector2 nextPos2 = ResolveCollision(cur, desiredNext2);
        Vector2 delta2 = nextPos2 - cur;

        if (delta2.sqrMagnitude > 1e-6f)
        {
            SetAnimation(delta2);
            rect.anchoredPosition = nextPos2;
            OnHeroMoved?.Invoke(nextPos2);
            UpdateDestinationMarkerVisuals(); // fade as we approach
        }
        else
        {
            // Blocked; stop moving this path
            isMoving = false;
            SetIdle();
            HideDestinationMarker();
        }
    }

    // ----------------------------------------------------------------------
    // External input API
    // ----------------------------------------------------------------------

    // Accepts analog input from OverworldManager each frame.
    public void SetAnalogInput(Vector2 input)
    {
        analogInput = Vector2.ClampMagnitude(input, 1f);
        if (analogInput.sqrMagnitude > 1e-6f)
        {
            isMoving = false;        // cancel click-to-move while analog active
            directionalActive = false;
            HideDestinationMarker();
            _path = null;
        }
    }

    // Entry point used by OverworldManager for clicks/taps (MoveToPoint only)
    public void HandleClickScreen(Vector2 screenPos, RectTransform content)
    {
        if (content == null) return;
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        HandleClickLocal(local);
    }

    public void HandleClickLocal(Vector2 local)
    {
        if (!AllowClickToMove || rect == null || rect.parent == null) return;

        if (requireVisibleToMove && ignoreClicksWhenOffscreen && !IsTargetVisible(rect, viewport))
            return;

        switch (inputMode)
        {
            case OverworldHeroInputMode.ClickToMove:
                SetDestinationLocal(local);
                break;

            case OverworldHeroInputMode.DirectionalPress:
                HideDestinationMarker();
                SetDirectionalOverride(local);
                break;
        }
    }

    // Sets a click-to-move destination in Content-local space.
    public void SetDestinationLocal(Vector2 local)
    {
        if (!AllowClickToMove || rect == null || rect.parent == null) return;

        targetPosition = ClampToMap(local);

        Vector2 delta = targetPosition - rect.anchoredPosition;
        if (delta.sqrMagnitude > 1e-6f)
            SetAnimation(delta);

        isMoving = true;
        directionalActive = false; // ensure point-move owns motion

        // Build path (if enabled) and show final waypoint marker
        _path = null; _pathIndex = 0;
        if (usePathfinding && collisionProvider != null && mapRect != null)
        {
            if (TryComputePath(rect.anchoredPosition, targetPosition, out var path))
            {
                _path = path;
                _pathIndex = 0;
            }
        }

        if (destinationMarkerEnabled)
        {
            Vector2 finalPoint = (_path != null && _path.Count > 0) ? _path[_path.Count - 1] : PredictStop(rect.anchoredPosition, targetPosition);
            ShowDestinationMarker(finalPoint);
        }
    }

    public void SetDestinationScreen(Vector2 screenPos, RectTransform content)
    {
        if (content == null) return;
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        SetDestinationLocal(local);
    }

    // Public API for hold-to-move directional clicks.
    public void BeginDirectionalFromScreen(Vector2 screenPos, RectTransform content)
    {
        if (inputMode != OverworldHeroInputMode.DirectionalPress) return;
        if (content == null) return;
        HideDestinationMarker();
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        SetDirectionalOverride(local);
    }

    public void UpdateDirectionalFromScreen(Vector2 screenPos, RectTransform content)
    {
        if (!directionalActive) return;
        if (content == null) return;
        HideDestinationMarker();
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        SetDirectionalOverride(local);
    }

    public void EndDirectional()
    {
        directionalActive = false;
        directionalOverride = Vector2.zero;
        SetIdle();
        HideDestinationMarker();
    }

    // ----------------------------------------------------------------------
    // Animation helpers for 8-way blend tree
    // ----------------------------------------------------------------------

    private void SetDirectionalOverride(Vector2 local)
    {
        Vector2 delta = ClampToMap(local) - rect.anchoredPosition;
        if (delta.sqrMagnitude < 1e-6f)
        {
            directionalActive = false;
            return;
        }

        Vector2 dir = delta.normalized * Mathf.Clamp01(directionalClickMagnitude);
        directionalOverride = dir;
        directionalActive = true;

        // Immediate visual feedback
        SetAnimation(dir);
        isMoving = false; // use analog-like path instead
    }

    private void SetAnimation(Vector2 delta)
    {
        if (animator == null) return;

        float speed = delta.magnitude;           // canvas units moved this frame
        Vector2 dir = speed > 1e-6f ? delta.normalized : lastLook;

        lastLook = dir;                          // remember facing for idle
        lastDirection = DetermineDirection4Way(delta); // maintain legacy 4-way for save text

        ApplyAnimatorParameters(dir, speed);
    }

    private void SetIdle()
    {
        lastDirection = MoveDirection.Idle;
        ApplyAnimatorParameters(lastLook, 0f);
    }

    private void ApplyAnimatorParameters(Vector2 dir, float speed)
    {
        if (animator == null) return;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
        animator.SetFloat("Speed", speed);
    }

    public void SetFacing(string facingName)
    {
        if (string.IsNullOrEmpty(facingName)) return;

        MoveDirection dir;
        if (!System.Enum.TryParse(facingName, true, out dir))
            dir = MoveDirection.Idle;

        lastDirection = dir;

        Vector2 look = lastLook;
        switch (dir)
        {
            case MoveDirection.Up: look = Vector2.up; break;
            case MoveDirection.Right: look = Vector2.right; break;
            case MoveDirection.Down: look = Vector2.down; break;
            case MoveDirection.Left: look = Vector2.left; break;
            case MoveDirection.Idle: default: break;
        }

        lastLook = look;
        ApplyAnimatorParameters(lastLook, 0f);
    }

    private static MoveDirection DetermineDirection4Way(Vector2 delta)
    {
        if (delta.sqrMagnitude < 1e-6f) return MoveDirection.Idle;

        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            return delta.x >= 0 ? MoveDirection.Right : MoveDirection.Left;
        else
            return delta.y >= 0 ? MoveDirection.Up : MoveDirection.Down;
    }

    // ----------------------------------------------------------------------
    // Visibility and clamping
    // ----------------------------------------------------------------------

    public static bool IsTargetVisible(RectTransform target, RectTransform view)
    {
        if (target == null || view == null) return true; // fail-open
        var corners = new Vector3[4];
        target.GetWorldCorners(corners);

        for (int i = 0; i < 4; i++)
        {
            var sp = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(view, sp, null, out var lp))
            {
                if (view.rect.Contains(lp)) return true;
            }
        }
        return false;
    }

    private Vector2 ClampToMap(Vector2 local)
    {
        if (mapRect == null || mapRect.parent != rect.parent) return local;

        var size = mapRect.rect.size;
        var pos = mapRect.anchoredPosition; // top-left in parent space

        float minX = pos.x;
        float maxX = pos.x + size.x;
        float maxY = pos.y;
        float minY = pos.y - size.y;

        float clampedX = Mathf.Clamp(local.x, minX, maxX);
        float clampedY = Mathf.Clamp(local.y, minY, maxY);
        return new Vector2(clampedX, clampedY);
    }

    // ----------------------------------------------------------------------
    // Collision and speed sampling
    // ----------------------------------------------------------------------

    public void BindMapAndViewport(RectTransform map, RectTransform view)
    {
        mapRect = map;
        viewport = view;
        if (collisionProvider == null && mapRect != null)
            collisionProvider = mapRect.GetComponent<MapTerrain>();
    }

    public void BindCollisionProvider(MapTerrain provider)
    {
        collisionProvider = provider;
    }

    private float GetSpeedMultiplierLocal(Vector2 local)
    {
        // Placeholder for slow-zone sampling if you choose to use it later
        return 1f; // constant speed
    }

    // Simple normal-based slide + axis fallback
    private Vector2 ResolveCollision(Vector2 current, Vector2 desired)
    {
        // If desired is walkable, go there
        if (IsWalkableLocal(desired))
            return desired;

        Vector2 step = desired - current;
        if (step.sqrMagnitude <= 1e-10f)
            return current;

        // Try sliding along the obstacle using a contact normal
        if (collisionProvider != null)
        {
            // Compute desired sampling center (hero visual center)
            Vector2 desiredCenter = GetRectCenterLocal(desired);

            // Reuse hero probe settings to get a decent normal
            float nRadius = Mathf.Max(2f, collisionProbeRadius > 0f ? collisionProbeRadius * 0.5f : 6f);
            int nRays = Mathf.Max(8, collisionProbeRays > 0 ? collisionProbeRays : 8);

            Vector2 n = collisionProvider.EstimateObstacleNormal(desiredCenter, nRadius, nRays);
            if (n.sqrMagnitude > 1e-6f)
            {
                // Project step onto tangent (remove normal component)
                Vector2 slide = step - Vector2.Dot(step, n) * n;
                if (slide.sqrMagnitude > 1e-6f)
                {
                    Vector2 candidate = ClampToMap(current + slide);
                    if (IsWalkableLocal(candidate))
                        return candidate;
                }
            }
        }

        // Fallback: axis-aligned slides (robust on corners)
        Vector2 tryX = new Vector2(desired.x, current.y);
        tryX = ClampToMap(tryX);
        if (IsWalkableLocal(tryX))
            return tryX;

        Vector2 tryY = new Vector2(current.x, desired.y);
        tryY = ClampToMap(tryY);
        if (IsWalkableLocal(tryY))
            return tryY;

        // Blocked; stay put
        return current;
    }

    // Returns true if the given Content-local point is walkable according to MapTerrain
    private bool IsWalkableLocal(Vector2 local)
    {
        // Offset the sampling point from the RectTransform pivot to its visual center
        Vector2 centerLocal = GetRectCenterLocal(local);

        int rays = collisionProbeRays > 0 ? collisionProbeRays : 8;
        float radius = collisionProbeRadius > 0f
            ? collisionProbeRadius
            : (rect != null ? Mathf.Min(rect.rect.width, rect.rect.height) * 0.35f : 8f);

        if (collisionProvider == null)
            return true; // fail-open if provider missing

        // Probe at center + ring
        if (!collisionProvider.IsWalkableLocal(centerLocal)) return false;

        if (radius > 0f && rays > 0)
        {
            float step = 360f / rays;
            for (int i = 0; i < rays; i++)
            {
                float ang = step * i * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
                if (!collisionProvider.IsWalkableLocal(centerLocal + offset))
                    return false;
            }
        }
        return true;
    }

    // Compute the rect's visual center in parent (Content) local space for a given anchored position
    private Vector2 GetRectCenterLocal(Vector2 anchored)
    {
        if (rect == null) return anchored;
        Vector2 size = rect.rect.size;
        Vector2 pivot = rect.pivot;
        // From pivot position to center: (0.5 - pivot) * size
        Vector2 pivotToCenter = new Vector2((0.5f - pivot.x) * size.x, (0.5f - pivot.y) * size.y);
        return anchored + pivotToCenter;
    }

    // Predict a final stop position using the same collision logic; fixed step for determinism
    private Vector2 PredictStop(Vector2 start, Vector2 target)
    {
        Vector2 cur = start;
        const int maxIters = 256;
        float stepLen = Mathf.Max(1f, moveSpeed / 60f); // ~60Hz equivalent

        for (int i = 0; i < maxIters; i++)
        {
            Vector2 toTarget = target - cur;
            float dist = toTarget.magnitude;
            if (dist <= snapThreshold) return target;

            Vector2 dir = dist > 1e-6f ? (toTarget / dist) : Vector2.zero;
            float len = Mathf.Min(stepLen, dist);
            Vector2 desired = ClampToMap(cur + dir * len);
            Vector2 next = ResolveCollision(cur, desired);
            if ((next - cur).sqrMagnitude <= 1e-6f)
                return cur; // blocked before target
            cur = next;
        }
        return cur;
    }

    // Destination marker helpers
    private void EnsureDestinationMarker()
    {
        if (!destinationMarkerEnabled || rect == null || rect.parent == null) return;

        // Prefab path
        if (destinationMarkerPrefab != null)
        {
            // Instantiate once (avoid using the asset reference as instance)
            if (destinationMarker == null || destinationMarker == destinationMarkerPrefab)
            {
                var inst = Instantiate(destinationMarkerPrefab, rect.parent);
                inst.name = "DestinationMarker";
                destinationMarker = inst;
                // Ensure sensible UI anchors/pivot in Content space
                destinationMarker.anchorMin = new Vector2(0f, 1f);
                destinationMarker.anchorMax = new Vector2(0f, 1f);
                destinationMarker.pivot = new Vector2(0.5f, 0.5f);
                destinationMarker.gameObject.SetActive(false);
            }

            CacheMarkerImages();
            return;
        }

        // Procedural crosshair path (existing behavior)
        if (destinationMarker == null)
        {
            var go = new GameObject("DestinationMarker", typeof(RectTransform));
            destinationMarker = go.GetComponent<RectTransform>();
            destinationMarker.SetParent(rect.parent, false);
            destinationMarker.anchorMin = new Vector2(0f, 1f);
            destinationMarker.anchorMax = new Vector2(0f, 1f);
            destinationMarker.pivot = new Vector2(0.5f, 0.5f);
            destinationMarker.sizeDelta = new Vector2(destinationMarkerSize, destinationMarkerSize);
            BuildMarkerGraphic(destinationMarker);
            destinationMarker.gameObject.SetActive(false);
        }
        else
        {
            // Rebuild in case size changed
            BuildMarkerGraphic(destinationMarker);
        }

        CacheMarkerImages();
    }

    private void BuildMarkerGraphic(RectTransform root)
    {
        // Crosshair made of two Image quads using the default UI sprite
        CreateLine("H", root, new Vector2(destinationMarkerSize, destinationMarkerThickness), 0f);
        CreateLine("V", root, new Vector2(destinationMarkerThickness, destinationMarkerSize), 0f);
    }

    private void CreateLine(string name, RectTransform parent, Vector2 size, float zRot)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.localRotation = Quaternion.Euler(0f, 0f, zRot);

        var img = go.GetComponent<Image>();
        img.color = destinationMarkerColor;
    }

    private void ShowDestinationMarker(Vector2 local)
    {
        if (!destinationMarkerEnabled) return;
        EnsureDestinationMarker();
        if (destinationMarker == null) return;

        // Keep prefab size as-authored; only size procedural marker
        if (destinationMarkerPrefab == null)
            destinationMarker.sizeDelta = new Vector2(destinationMarkerSize, destinationMarkerSize);

        // Place at the visual center corresponding to the given anchored (pivot) position
        Vector2 centerLocal = GetRectCenterLocal(local);
        destinationMarker.anchoredPosition = centerLocal;
        destinationMarker.gameObject.SetActive(true);

        // Tint prefab variant if assigned
        if (destinationMarkerPrefab != null && tintDestinationMarkerPrefab)
        {
            var images = destinationMarkerPrefab.GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                img.color = destinationMarkerColor;
            }
        }

        // Update colors on children
        for (int i = 0; i < destinationMarker.childCount; i++)
        {
            var img = destinationMarker.GetChild(i).GetComponent<Image>();
            if (img != null) img.color = destinationMarkerColor;
        }
    }

    private void HideDestinationMarker()
    {
        if (destinationMarker != null)
            destinationMarker.gameObject.SetActive(false);
        _markerAlpha = 0f;
    }

    // ----------------------------------------------------------------------
    // Debug gizmos
    // ----------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        if (!debugCollisionGizmos || rect == null || rect.parent == null) return;
        DrawCollisionDebugGizmos();
    }

    private void DrawCollisionDebugGizmos()
    {
        var parent = (RectTransform)rect.parent;
        // Sample at the visual center, not the pivot
        Vector2 centerLocal = GetRectCenterLocal(rect.anchoredPosition);

        // Draw center sample
        Gizmos.color = SampleColor(centerLocal);
        Vector3 centerWorld = parent.TransformPoint(centerLocal);
        Gizmos.DrawSphere(centerWorld, debugGizmoSize);

        // Ring samples
        int rays = collisionProbeRays > 0 ? collisionProbeRays : 8;
        float radius = collisionProbeRadius > 0f
            ? collisionProbeRadius
            : (rect != null ? Mathf.Min(rect.rect.width, rect.rect.height) * 0.35f : 8f);

        if (rays <= 0 || radius <= 0f) return;

        float step = 360f / rays;
        for (int i = 0; i < rays; i++)
        {
            float ang = step * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
            Vector2 pLocal = centerLocal + offset;

            Gizmos.color = SampleColor(pLocal);
            Vector3 pWorld = parent.TransformPoint(pLocal);
            Gizmos.DrawSphere(pWorld, debugGizmoSize);
        }
    }

    private Color SampleColor(Vector2 local)
    {
        if (collisionProvider != null && collisionProvider.TrySamplePixelLocal(local, out Color32 px))
            return MapTerrain.IsBlockedColor(px) ? Color.red : Color.green;

        // Provider missing or sample failed
        return new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    // Compute alpha target from remaining distance and tween to it
    private void UpdateDestinationMarkerVisuals()
    {
        if (!destinationMarkerEnabled || destinationMarker == null || !destinationMarker.gameObject.activeSelf)
            return;

        if (!destinationMarkerFade)
            return;

        // Use hero's visual center for distance, not the pivot
        Vector2 heroCenter = GetRectCenterLocal(rect.anchoredPosition);
        float dist = Vector2.Distance(heroCenter, destinationMarker.anchoredPosition);

        // 1 at far (fadeStart), 0 at near (fadeEnd)
        float t = Mathf.InverseLerp(destinationMarkerFadeEnd, destinationMarkerFadeStart, dist);
        float desired = Mathf.Lerp(destinationMarkerMinAlpha, destinationMarkerColor.a, t);

        // Smooth alpha change
        _markerAlpha = Mathf.MoveTowards(_markerAlpha, desired, destinationMarkerFadeSpeed * Time.deltaTime);
        SetMarkerAlpha(_markerAlpha);
    }

    private void CacheMarkerImages()
    {
        if (destinationMarker == null) return;
        _markerImages = destinationMarker.GetComponentsInChildren<Image>(true);
    }

    private void SetMarkerAlpha(float a)
    {
        if (_markerImages == null || _markerImages.Length == 0)
            CacheMarkerImages();

        if (_markerImages == null) return;

        // Apply alpha while preserving RGB
        for (int i = 0; i < _markerImages.Length; i++)
        {
            var img = _markerImages[i];
            if (img == null) continue;
            var c = img.color;
            c.a = a;
            img.color = c;
        }
    }

    // ----------------- A* Pathfinding helpers -----------------

    private bool TryComputePath(Vector2 startLocal, Vector2 goalLocal, out System.Collections.Generic.List<Vector2> path)
    {
        path = null;
        if (mapRect == null || collisionProvider == null || navCellSize <= 0) return false;

        // Map bounds
        var size = mapRect.rect.size;
        var pos = mapRect.anchoredPosition; // top-left
        float minX = pos.x;
        float maxX = pos.x + size.x;
        float maxY = pos.y;
        float minY = pos.y - size.y;

        int cols = Mathf.Max(1, Mathf.CeilToInt((maxX - minX) / navCellSize));
        int rows = Mathf.Max(1, Mathf.CeilToInt((maxY - minY) / navCellSize));

        // Local <-> Grid converters
        System.Func<Vector2, (int gx, int gy)> L2G = (Vector2 p) =>
        {
            int gx = Mathf.Clamp((int)Mathf.Floor((p.x - minX) / navCellSize), 0, cols - 1);
            int gy = Mathf.Clamp((int)Mathf.Floor((maxY - p.y) / navCellSize), 0, rows - 1);
            return (gx, gy);
        };
        System.Func<int, int, Vector2> G2L = (int gx, int gy) =>
        {
            float x = minX + (gx + 0.5f) * navCellSize;
            float y = maxY - (gy + 0.5f) * navCellSize;
            return new Vector2(x, y);
        };

        // Clearance radius (hero body + buffer)
        float heroRadius = collisionProbeRadius > 0f ? collisionProbeRadius : (rect != null ? Mathf.Min(rect.rect.width, rect.rect.height) * 0.35f : 8f);
        float clearance = heroRadius + Mathf.Max(0f, navObstacleBuffer);
        int rays = Mathf.Max(8, collisionProbeRays > 0 ? collisionProbeRays : 8);

        // Cell walkability cache
        var walkCache = new System.Collections.Generic.Dictionary<int, bool>(1024);
        System.Func<int, int, bool> CellWalkable = (int gx, int gy) =>
        {
            if (gx < 0 || gy < 0 || gx >= cols || gy >= rows) return false;
            int key = (gy * 32749) ^ gx; // simple hash
            if (walkCache.TryGetValue(key, out bool ok)) return ok;
            Vector2 sample = G2L(gx, gy);
            bool w = collisionProvider.IsWalkableLocal(sample, clearance, rays);
            walkCache[key] = w;
            return w;
        };

        // A* node structure
        var open = new System.Collections.Generic.List<(int gx, int gy, float f, float g)>();
        var came = new System.Collections.Generic.Dictionary<int, (int px, int py)>();
        var gScore = new System.Collections.Generic.Dictionary<int, float>();
        var closed = new System.Collections.Generic.HashSet<int>();

        (int sx, int sy) = L2G(startLocal);
        (int tx, int ty) = L2G(goalLocal);

        if (!CellWalkable(tx, ty))
        {
            // If target cell is blocked by clearance, try nearby cells in a small ring
            bool foundAlt = false;
            int maxRing = 3;
            for (int r = 1; r <= maxRing && !foundAlt; r++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        int nx = tx + dx, ny = ty + dy;
                        if (CellWalkable(nx, ny)) { tx = nx; ty = ny; foundAlt = true; break; }
                    }
                    if (foundAlt) break;
                }
            }
            if (!foundAlt) return false;
        }

        int startKey = (sy * 32749) ^ sx;
        int goalKey = (ty * 32749) ^ tx;

        System.Func<int, int, float> Heuristic = (int gx, int gy) =>
        {
            // Octile distance
            float dx = Mathf.Abs(gx - tx);
            float dy = Mathf.Abs(gy - ty);
            float h = (dx + dy) + (1.41421356f - 2f) * Mathf.Min(dx, dy);
            return h * navCellSize;
        };

        void OpenPush(int gx, int gy, float g)
        {
            float f = g + Heuristic(gx, gy);
            open.Add((gx, gy, f, g));
        }

        OpenPush(sx, sy, 0f);
        gScore[startKey] = 0f;

        int expanded = 0;
        int[] n8x = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] n8y = { -1, -1, -1, 0, 0, 1, 1, 1 };
        float[] nCost = { 1.41421356f, 1f, 1.41421356f, 1f, 1f, 1.41421356f, 1f, 1.41421356f };

        while (open.Count > 0)
        {
            // pop lowest f (linear scan; small lists)
            int bestI = 0; float bestF = open[0].f;
            for (int i = 1; i < open.Count; i++) if (open[i].f < bestF) { bestF = open[i].f; bestI = i; }
            var node = open[bestI]; open.RemoveAt(bestI);
            int key = (node.gy * 32749) ^ node.gx;
            if (closed.Contains(key)) continue;
            closed.Add(key);

            expanded++;
            if (expanded > navMaxExpanded) break;

            if (node.gx == tx && node.gy == ty)
            {
                // Reconstruct
                var rev = new System.Collections.Generic.List<Vector2>(64);
                int cx = node.gx, cy = node.gy; int ckey = (cy * 32749) ^ cx;
                rev.Add(G2L(cx, cy));
                while (came.TryGetValue(ckey, out var p))
                {
                    cx = p.px; cy = p.py; ckey = (cy * 32749) ^ cx;
                    rev.Add(G2L(cx, cy));
                }
                rev.Reverse();

                // Basic string-pull smoothing (line-of-sight)
                var smoothed = StringPull(rev, clearance, rays);
                path = smoothed;
                return path != null && path.Count > 0;
            }

            // Explore neighbors (prevent cutting corners through blocked adjacents)
            for (int ni = 0; ni < 8; ni++)
            {
                int nx = node.gx + n8x[ni];
                int ny = node.gy + n8y[ni];
                if (!CellWalkable(nx, ny)) continue;

                // For diagonals, ensure both orthogonal neighbors are walkable
                bool diagonal = (n8x[ni] != 0 && n8y[ni] != 0);
                if (diagonal)
                {
                    if (!CellWalkable(node.gx, ny) || !CellWalkable(nx, node.gy)) continue;
                }

                int nkey = (ny * 32749) ^ nx;
                float tg = node.g + nCost[ni] * navCellSize;
                if (gScore.TryGetValue(nkey, out float oldG) && oldG <= tg) continue;

                gScore[nkey] = tg;
                came[nkey] = (node.gx, node.gy);
                OpenPush(nx, ny, tg);
            }
        }

        return false;
    }

    private System.Collections.Generic.List<Vector2> StringPull(System.Collections.Generic.List<Vector2> pts, float clearance, int rays)
    {
        if (pts == null || pts.Count == 0) return pts;
        var result = new System.Collections.Generic.List<Vector2>();
        Vector2 a = pts[0];
        result.Add(a);
        int i = 0;
        while (i < pts.Count - 1)
        {
            int j = pts.Count - 1;
            // Find furthest visible from a
            for (; j > i + 1; j--)
            {
                if (SegmentClear(a, pts[j], clearance, rays))
                    break;
            }
            a = pts[j];
            result.Add(a);
            i = j;
        }
        return result;
    }

    private bool SegmentClear(Vector2 a, Vector2 b, float clearance, int rays)
    {
        // Sample along the segment at ~half cell length
        float len = (b - a).magnitude;
        if (len <= 1e-4f) return true;
        int steps = Mathf.Max(1, Mathf.CeilToInt(len / Mathf.Max(1f, navCellSize * 0.5f)));
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector2 p = Vector2.Lerp(a, b, t);
            if (!collisionProvider.IsWalkableLocal(p, clearance, rays))
                return false;
        }
        return true;
    }
}
