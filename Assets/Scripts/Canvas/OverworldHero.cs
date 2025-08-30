using Assets.Helper;
using Assets.Helpers;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;

public enum OverworldHeroTouchMode
{
    MoveToPoint,
    DirectionalClick
}


public class OverworldHero : MonoBehaviour
{
    public RectTransform rect;           // The icon’s RectTransform (anchored to Content)
    public Animator animator;
    private float moveSpeed = 128f;

    [Header("Bindings")]
    [SerializeField] private RectTransform mapRect;   // Optional; if null, auto-find
    [SerializeField] private RectTransform viewport;  // ScrollRect viewport; used for visibility gating

    public bool AllowClickToMove { get; set; } = true;
    public event System.Action<Vector2> OnHeroMoved;

    [Header("Tuning")]
    [SerializeField] private float snapThreshold = 0.24f;
    [SerializeField] private bool requireVisibleToMove = true;          // Only step when visible
    [SerializeField] private bool ignoreClicksWhenOffscreen = false;    // If true, ignore clicks when offscreen
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

    [Header("Animator States (optional overrides)")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string upState = "MoveUp";
    [SerializeField] private string rightState = "MoveRight";
    [SerializeField] private string downState = "MoveDown";
    [SerializeField] private string leftState = "MoveLeft";

    // --- Collision Mask (added) ---
    [Header("Collision Mask")]
    [Tooltip("Black/white texture aligned to the map. White = walkable, Black = blocked (pure-black only).")]
    [SerializeField] private Texture2D collisionMask;
    // UV sub-rect (for atlas/cropped sprites). Defaults to full texture.
    [SerializeField, HideInInspector] private Rect collisionMaskUV = new Rect(0, 0, 1, 1);

    // Note: threshold kept for back-compat sliders only (not used for blocking).
    [Range(0f, 0.1f)]
    [SerializeField] private float blockThreshold = 0.01f;

    [Tooltip("Probe radius in canvas units around hero to reduce corner clipping.")]
    [SerializeField] private float collisionProbeRadius = 0f;
    [Tooltip("Number of radial probes around the hero in addition to center (0 enables an automatic 8-ray ring).")]
    [SerializeField] private int collisionProbeRays = 0;

    [Header("Slow Zones (optional)")]
    [SerializeField] private bool useSlowZones = false;
    [SerializeField, Range(0f, 1f)] private float slowBandCenter = 0.5f;
    [SerializeField, Range(0f, 1f)] private float slowBandHalfWidth = 0.1f;
    [SerializeField, Range(0.05f, 1f)] private float slowMultiplier = 0.5f;

    // --- Debug Visualization ---
    [Header("Collision Debug (Scene view)")]
    [SerializeField] private bool debugCollisionGizmos = true;
    [SerializeField] private float debugGizmoSize = 6f;

    private Color32[] maskPixels;
    private int maskW, maskH;

    private bool isMoving;
    private Vector2 targetPosition;
    private MoveDirection lastDirection = MoveDirection.Idle;

    // Analog input (-1..1). Set by OverworldManager each frame.
    private Vector2 analogInput;

    // Directional click override (-1..1). Latched while pressed.
    private bool directionalActive;
    private Vector2 directionalOverride;

    // Expose current facing as string for saving
    public string CurrentFacingName => lastDirection.ToString();
    public bool IsMoving => isMoving;
    private readonly Vector3[] _mapWorldCorners = new Vector3[4];

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (animator == null) animator = GetComponent<Animator>();

        // Auto-find Terrain for clamp (no legacy Map)
        if (mapRect == null)
        {
            var go = GameObject.Find(GameObjectHelper.Overworld.Terrain);
            if (go != null) mapRect = go.GetComponent<RectTransform>();
        }

        if (viewport == null)
        {
            var vp = GameObject.Find(GameObjectHelper.Overworld.Viewport);
            if (vp != null) viewport = vp.GetComponent<RectTransform>();
        }

        InitializeCollisionMask(); // keep existing collision/slows behavior
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
                SetAnimation(frameDelta);
            else
                SetIdle();

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
        float maxStep = moveSpeed * Time.deltaTime;
        Vector2 stepDir = toTarget.sqrMagnitude > 1e-6f ? toTarget.normalized : Vector2.zero;

        float moveMult = GetSpeedMultiplierLocal(cur + stepDir * (maxStep * speedSampleAheadFactor));
        Vector2 stepVec = stepDir * (maxStep * moveMult);

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
                // In hold-to-move mode, OverworldManager will call Begin/Update/End
                SetDirectionalOverride(local);
                break;
        }
    }

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

    // Public API: begin/update/end directional hold-to-move
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

    // Compute a latched direction vector from hero to click position and start moving
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

        // Play appropriate move animation immediately
        SetAnimation(dir);
        isMoving = false; // use analog-like path instead
    }

    // Freeze movement immediately (used for encounters)
    public void FreezeMovement(bool idle = true)
    {
        AllowClickToMove = false;
        allowVirtualJoystick = false;
        isMoving = false;
        analogInput = Vector2.zero;
        directionalActive = false;
        directionalOverride = Vector2.zero;
        if (idle) SetIdle();
    }

    // Teleport to an exact local position (used when restoring overworld)
    public void TeleportToLocal(Vector2 local, bool notify = true)
    {
        if (rect == null) return;
        rect.anchoredPosition = ClampToMap(local);
        if (notify) OnHeroMoved?.Invoke(rect.anchoredPosition);
    }

    public void SetFacing(string facingName)
    {
        if (string.IsNullOrEmpty(facingName)) return;
        MoveDirection dir;
        if (!System.Enum.TryParse(facingName, true, out dir))
            dir = MoveDirection.Idle;
        lastDirection = dir;
        SetAnimatorDirection(dir);
        CrossFadeIfExists(DirectionToStateName(dir));
    }

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

    private void SetAnimation(Vector2 delta)
    {
        var newDir = DetermineDirection(delta);
        if (newDir == lastDirection) return;

        lastDirection = newDir;
        SetAnimatorDirection(newDir);
        CrossFadeIfExists(DirectionToStateName(newDir));
    }

    private void SetIdle()
    {
        if (lastDirection != MoveDirection.Idle)
        {
            lastDirection = MoveDirection.Idle;
            SetAnimatorDirection(lastDirection);
            CrossFadeIfExists(idleState);
        }
    }

    private void SetAnimatorDirection(MoveDirection dir)
    {
        if (animator != null)
            animator.SetInteger("MoveDirection", (int)dir);
    }

    private static MoveDirection DetermineDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            return delta.x >= 0 ? MoveDirection.Right : MoveDirection.Left;
        else
            return delta.y >= 0 ? MoveDirection.Up : MoveDirection.Down;
    }

    private string DirectionToStateName(MoveDirection dir)
    {
        switch (dir)
        {
            case MoveDirection.Up: return upState;
            case MoveDirection.Right: return rightState;
            case MoveDirection.Down: return downState;
            case MoveDirection.Left: return leftState;
            default: return idleState;
        }
    }

    private void CrossFadeIfExists(string stateName, float fade = 0.05f)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;
        int layer = 0;
        int hash = Animator.StringToHash(stateName);
        if (animator.HasState(layer, hash))
            animator.CrossFadeInFixedTime(hash, fade, layer, 0f);
    }

    // ------------- Collision + Speed helpers ------------

    public void BindMapAndViewport(RectTransform map, RectTransform view)
    {
        mapRect = map;
        viewport = view;
    }

    private void InitializeCollisionMask()
    {
        maskPixels = null;
        maskW = maskH = 0;

        if (collisionMask == null) return;

        try
        {
            maskW = collisionMask.width;
            maskH = collisionMask.height;
            maskPixels = collisionMask.GetPixels32(); // requires Read/Write Enabled
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"OverworldHero: Could not read collision mask pixels. Ensure Read/Write Enabled. {ex.Message}");
            maskPixels = null;
            maskW = maskH = 0;
        }
    }

    public void SetCollisionMask(Texture2D mask)
    {
        collisionMask = mask;
        collisionMaskUV = new Rect(0, 0, 1, 1);
        InitializeCollisionMask();
    }

    public void SetCollisionMask(Texture2D mask, Rect uvRect01)
    {
        collisionMask = mask;
        collisionMaskUV = uvRect01;
        InitializeCollisionMask();
    }

    private bool MaskReady => collisionMask != null && maskPixels != null && maskPixels.Length == maskW * maskH && mapRect != null;

    // Robust, rotation/scale-safe mapping from Content-local point to mask UV
    private Vector2 LocalToMaskUV(Vector2 local)
    {
        if (mapRect == null) return Vector2.zero;

        // Get the map rect corners in the parent's local space (works for any pivot/anchors/scale/rotation)
        mapRect.GetWorldCorners(_mapWorldCorners);
        var parent = (RectTransform)mapRect.parent;
        Vector2 bl = parent.InverseTransformPoint(_mapWorldCorners[0]); // bottom-left
        Vector2 tl = parent.InverseTransformPoint(_mapWorldCorners[1]); // top-left
        Vector2 tr = parent.InverseTransformPoint(_mapWorldCorners[2]); // top-right

        Vector2 rightVec = tr - tl;   float width = rightVec.magnitude;
        Vector2 downVec  = bl - tl;   float height = downVec.magnitude;
        if (width <= 1e-6f || height <= 1e-6f) return Vector2.zero;

        Vector2 rightN = rightVec / width;
        Vector2 downN  = downVec  / height;

        // Project the vector from TL to the local point onto the rect's local axes
        Vector2 toP = local - tl;
        float u01 = Mathf.Clamp01(Vector2.Dot(toP, rightN) / width);
        float v01 = Mathf.Clamp01(Vector2.Dot(toP, downN)  / height); // top->0 .. bottom->1

        // Apply sub-UV in case the mask comes from an atlas/cropped sprite
        //return new Vector2(
        //    collisionMaskUV.x + u01 * collisionMaskUV.width,
        //    collisionMaskUV.y + v01 * collisionMaskUV.height
        //);

        // Apply sub-UV in case the mask comes from an atlas/cropped sprite
        // Texture V grows up from bottom. Our v01 grows down from top.
        // Flip only when the uvRect height is positive, otherwise RawImage already flipped it.
        float vInput = (collisionMaskUV.height >= 0f) ? (1f - v01) : v01;

        return new Vector2(
            collisionMaskUV.x + u01 * collisionMaskUV.width,
            collisionMaskUV.y + vInput * collisionMaskUV.height
        );

    }

    // New: sample raw pixel color at local position
    private bool TrySamplePixel(Vector2 local, out Color32 color)
    {
        color = default;
        if (!MaskReady) return false;

        Vector2 uv = LocalToMaskUV(local);
        if (uv.x < 0f || uv.x > 1f || uv.y < 0f || uv.y > 1f)
            return false;

        int x = Mathf.Clamp(Mathf.RoundToInt(uv.x * (maskW - 1)), 0, maskW - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(uv.y * (maskH - 1)), 0, maskH - 1);
        int idx = y * maskW + x;

        color = maskPixels[idx];
        return true;
    }

    // Only pure black is blocked (RGB exactly 0,0,0)
    private static bool IsBlockedColor(Color32 c)
    {
        return c.r == 0 && c.g == 0 && c.b == 0;
    }

    private bool IsWalkableLocal(Vector2 local)
    {
        if (!MaskReady) return true;

        // 1) Center probe
        if (!IsWalkablePoint(local)) return false;

        // 2) Neighborhood ring probes: use configured rays/radius; otherwise auto 8 rays with a radius based on the hero icon
        int rays = collisionProbeRays > 0 ? collisionProbeRays : 8;
        float radius = collisionProbeRadius > 0f
            ? collisionProbeRadius
            : (rect != null ? Mathf.Min(rect.rect.width, rect.rect.height) * 0.35f : 8f);

        if (radius > 0f && rays > 0)
        {
            float step = 360f / rays;
            for (int i = 0; i < rays; i++)
            {
                float ang = step * i * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
                if (!IsWalkablePoint(local + offset))
                    return false;
            }
        }

        return true;
    }

    private bool IsWalkablePoint(Vector2 local)
    {
        if (TrySamplePixel(local, out Color32 px))
            return !IsBlockedColor(px);

        // Fallback: if we couldn't sample, treat as walkable
        return true;
    }

    // Returns a multiplier (0.05..1). Blocked is handled elsewhere; this only slows.
    private float GetSpeedMultiplierLocal(Vector2 local)
    {
        return 1f; // no slowdowns; movement speed is constant
    }

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

    // ---------- Scene gizmos for visual verification ----------
    private void OnDrawGizmos()
    {
        if (!debugCollisionGizmos || rect == null || rect.parent == null) return;
        DrawCollisionDebugGizmos();
    }

    private void DrawCollisionDebugGizmos()
    {
        var parent = (RectTransform)rect.parent;
        Vector2 centerLocal = rect.anchoredPosition;

        // Draw center
        Gizmos.color = SampleColor(centerLocal);
        Vector3 centerWorld = parent.TransformPoint(centerLocal);
        Gizmos.DrawSphere(centerWorld, debugGizmoSize);

        // Ring
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
        if (!MaskReady) return new Color(0.3f, 0.3f, 0.3f, 1f); // gray
        if (!TrySamplePixel(local, out Color32 px)) return Color.gray;

        bool blocked = IsBlockedColor(px);
        return blocked ? Color.red : Color.green;
    }
}
