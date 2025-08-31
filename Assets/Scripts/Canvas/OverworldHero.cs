using Assets.Helper;
using Assets.Helpers;
using UnityEngine;

public enum OverworldHeroTouchMode
{
    MoveToPoint,
    DirectionalClick
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
    [SerializeField] private OverworldHeroTouchMode touchMoveMode = OverworldHeroTouchMode.MoveToPoint;
    [SerializeField] private float directionalClickMagnitude = 1f; // 0..1 strength fed into analog

    // Expose mode to other systems (e.g., OverworldManager)
    public OverworldHeroTouchMode TouchMoveMode
    {
        get => touchMoveMode;
        set => touchMoveMode = value;
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
        }
        else if (allowVirtualJoystick && directionalActive && directionalOverride.sqrMagnitude > 1e-6f)
        {
            effectiveInput = directionalOverride;
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
            return;
        }

        // 2) No analog-like input -> force Idle when not using click-to-move to a point
        if (!AllowClickToMove)
        {
            SetIdle();
            isMoving = false;
            return;
        }

        // 3) MoveToPoint path (only when mode is MoveToPoint)
        if (touchMoveMode != OverworldHeroTouchMode.MoveToPoint || !isMoving)
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

        // Arrived (snap and finish)
        if (Vector2.Distance(cur, targetPosition) <= snapThreshold)
        {
            rect.anchoredPosition = targetPosition;
            OnHeroMoved?.Invoke(targetPosition);

            isMoving = false;
            SetIdle();
            return;
        }

        // Advance towards target and animate (stepwise, with collision)
        Vector2 toTarget = targetPosition - cur;
        float dist = toTarget.magnitude;
        float maxStep = moveSpeed * Time.deltaTime;
        Vector2 stepDir = dist > 1e-6f ? (toTarget / dist) : Vector2.zero;

        // Optional speed multiplier (kept for future/slow zones)
        float moveMult = GetSpeedMultiplierLocal(cur + stepDir * (maxStep * speedSampleAheadFactor));

        // Clamp step to remaining distance so we cannot overshoot
        float stepLen = Mathf.Min(maxStep * moveMult, dist);
        Vector2 stepVec = stepDir * stepLen;

        Vector2 desiredNext = ClampToMap(cur + stepVec);
        Vector2 nextPos = ResolveCollision(cur, desiredNext);
        Vector2 delta = nextPos - cur;

        if (delta.sqrMagnitude > 1e-6f)
        {
            SetAnimation(delta);
            rect.anchoredPosition = nextPos;
            OnHeroMoved?.Invoke(nextPos);
        }
        else
        {
            // Blocked; stop moving this path
            isMoving = false;
            SetIdle();
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

        switch (touchMoveMode)
        {
            case OverworldHeroTouchMode.MoveToPoint:
                SetDestinationLocal(local);
                break;

            case OverworldHeroTouchMode.DirectionalClick:
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
        if (touchMoveMode != OverworldHeroTouchMode.DirectionalClick) return;
        if (content == null) return;
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        SetDirectionalOverride(local);
    }

    public void UpdateDirectionalFromScreen(Vector2 screenPos, RectTransform content)
    {
        if (!directionalActive) return;
        if (content == null) return;
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        SetDirectionalOverride(local);
    }

    public void EndDirectional()
    {
        directionalActive = false;
        directionalOverride = Vector2.zero;
        SetIdle();
    }

    // ----------------------------------------------------------------------
    // Animation helpers for 8-way blend tree
    // ----------------------------------------------------------------------

    /// <summary>
    /// Computes direction from a point click and latches it for hold-to-move.
    /// Also kicks the Animator to the correct moving state immediately.
    /// </summary>
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

    /// <summary>
    /// Applies direction and speed from a movement delta to the Animator.
    /// Updates last look so idle keeps facing.
    /// </summary>
    private void SetAnimation(Vector2 delta)
    {
        if (animator == null) return;

        float speed = delta.magnitude;           // canvas units moved this frame
        Vector2 dir = speed > 1e-6f ? delta.normalized : lastLook;

        lastLook = dir;                          // remember facing for idle
        lastDirection = DetermineDirection4Way(delta); // maintain legacy 4-way for save text

        ApplyAnimatorParameters(dir, speed);
    }

    /// <summary>
    /// Forces idle using the last facing direction.
    /// </summary>
    private void SetIdle()
    {
        lastDirection = MoveDirection.Idle;
        ApplyAnimatorParameters(lastLook, 0f);
    }

    /// <summary>
    /// Writes MoveX, MoveY, and Speed to the Animator in one place.
    /// </summary>
    private void ApplyAnimatorParameters(Vector2 dir, float speed)
    {
        if (animator == null) return;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
        animator.SetFloat("Speed", speed);
    }

    /// <summary>
    /// Maintains legacy facing API using 4-way directions for save/load.
    /// Also updates blend-tree direction so idle pose matches.
    /// </summary>
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

    // Converts any delta into the nearest 4-way for legacy state text.
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

    // Returns true if any of the 4 world-corners projects inside the viewport rect
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

    // Clamp a Content-local point to the Terrain rect area
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

    // Bind the map rect (for clamping) and viewport (for visibility)
    public void BindMapAndViewport(RectTransform map, RectTransform view)
    {
        mapRect = map;
        viewport = view;
        if (collisionProvider == null && mapRect != null)
            collisionProvider = mapRect.GetComponent<MapTerrain>();
    }

    // Bind the collision provider explicitly
    public void BindCollisionProvider(MapTerrain provider)
    {
        collisionProvider = provider;
    }

    // Returns a multiplier (0.05..1). Blocked is handled elsewhere; this only slows.
    private float GetSpeedMultiplierLocal(Vector2 local)
    {
        // Placeholder for slow-zone sampling if you choose to use it later
        return 1f; // constant speed
    }

    // Simple 4-way collision resolution with axis slides
    private Vector2 ResolveCollision(Vector2 current, Vector2 desired)
    {
        // If desired is walkable, go there
        if (IsWalkableLocal(desired))
            return desired;

        // Try sliding along X
        Vector2 tryX = new Vector2(desired.x, current.y);
        tryX = ClampToMap(tryX);
        if (IsWalkableLocal(tryX))
            return tryX;

        // Try sliding along Y
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
}
