using Assets.Helper;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;



// OverworldHero (world-space only)
// Movement types (mutually exclusive):
// - VirtualJoystick: analog stick movement only
// - ClickToMove: path or straight toward a click destination
// - DirectionalPress: hold near a point to move in that direction
// All three share the same collision and pathfinding helpers below.
[ExecuteAlways]
public class OverworldHero : MonoBehaviour
{
    // Bindings (resolved at runtime from hierarchy paths)
    public Animator animator;                 // Animator driving 8-way blend tree
    private SpriteRenderer terrainSprite;         // Map SpriteRenderer used for world bounds
    private SpriteRenderer heroSprite;        // Hero's SpriteRenderer (for probe radius inference)
    private MapTerrain collisionProvider;     // Central collision provider on Terrain
    private Camera worldCamera;               // Camera for screen->world and visibility tests

    // Movement tuning (set in code)
    private float moveSpeed = 2.5f;           // Units per second
    private float snapThreshold = 0.05f;      // Stop distance to consider goal reached
    private bool requireVisibleToMove = true; // Only move when visible in camera viewport
    private bool ignoreClicksWhenOffscreen = false;
    private bool allowVirtualJoystick = true; // Enable joystick/analog movement
    private bool idleWhileOffscreen = true;   // Idle when offscreen and movement gated

    // Sampling
    private float speedSampleAheadFactor = 0.7f; // Future-proof: speed zones, currently constant 1x


    private float probeRadiusMultiplier = 0.2f; // fraction of hero bounds to use for collision probe radius (used only if fixed radius disabled)

    // Wall-slide tuning (reduce stickiness)
    private float wallSlideUnstick = 0.02f;      // small push away from wall along normal
    private int wallSlideAttempts = 3;           // try partial tangent steps if full slide is blocked
    private float wallSlideMinFraction = 0.25f;  // smallest tangent fraction to try

    // FollowCursor speed ramp: distance at which input magnitude reaches 1
    private float followSpeedRampDistance = 6.0f;

    // Input mode
    private OverworldHeroInputMode inputMode = OverworldHeroInputMode.FollowCursor;
    public OverworldHeroInputMode InputMode
    {
        get => inputMode;
        set
        {
            if (inputMode == value) return;
            var prev = inputMode;
            inputMode = value;
            // When leaving ClickToMove, clear any path state
            if (prev == OverworldHeroInputMode.ClickToMove && inputMode != OverworldHeroInputMode.ClickToMove)
            {
                isMoving = false; _path = null; directionalActive = false;
            }
        }
    }


    public bool UsingJoystick => inputMode == OverworldHeroInputMode.VirtualJoystick;

    private float directionalClickMagnitude = 1f; // 0..1 strength fed into analog

    // External toggles
    public bool AllowClickToMove { get; set; } = true;

    // Back-compat properties for manager/save
    public OverworldHeroInputMode TouchMoveMode
    {
        get => inputMode;
        set => inputMode = value;
    }
    public string CurrentFacingName => lastDirection.ToString();

    // Events
    public event Action<Vector2> OnHeroMoved;  // Invoked with world position after movement

    // Runtime state
    private bool isMoving;                 // True while following a MoveToPoint target
    private Vector2 targetPosition;        // Destination for MoveToPoint mode (world)

    // 4-way facing (legacy name for saves), while animator uses 8-way via lastLook
    private MoveDirection lastDirection = MoveDirection.Idle;

    // Analog input (-1..1). Set by OverworldManager each frame.
    private Vector2 analogInput;

    // Directional click override (-1..1). Latched while pressed.
    private bool directionalActive;
    private Vector2 directionalOverride;

    // 8-way facing memory for idle pose. Defaults to down.
    private Vector2 lastLook = Vector2.down;

    // Pathfinding (A*)
    private bool usePathfinding = true;
    private int navCellSize = 1;              // In world units
    private float navObstacleBuffer = 0.05f;   // Extra clearance from walls
    private int navMaxExpanded = 8000;        // Solver cap
    private float waypointArrive = 0.1f;      // Waypoint arrive distance

    private List<Vector2> _path; // world waypoints
    private int _pathIndex;

    // Direction stabilization for animator (prevents flicker between pure down and diagonals)
    private const float axisSnapEpsilon = 0.12f;   // if minor axis below this, snap to 0
    private const float axisDominance = 1.35f;     // major must be >= minor * dominance

    // Collision center and radius
    // Always sample collisions at the Animator/Sprite pivot (transform.position) plus an optional feet offset.
    private Vector2 collisionFeetOffsetLocal = Vector2.zero; // local-space offset from pivot to feet (e.g., Vector2.down * 0.05f)
    private bool useFixedCollisionRadius = true;             // stabilize radius across animation frames
    private float collisionRadiusWorld = 0.2f;               // world units radius when fixed

    // Destination marker prefab to spawn on click
    private GameObject destinationMarkerPrefab;

    private void Awake()
    {
        // Auto-bind core components using exact hierarchy paths
        worldCamera = Camera.main;

        // Map terrain (SpriteRenderer + MapTerrain provider)
        var terrainGo = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain);
        if (terrainGo != null)
        {
            terrainSprite = terrainGo.GetComponent<SpriteRenderer>();
            collisionProvider = terrainGo.GetComponent<MapTerrain>();
        }

        // Hero sprite and animator
        heroSprite = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        // Initialize animator with default idle facing
        ApplyAnimatorParameters(lastLook, 0f);

        // Cache destination marker prefab
        if (PrefabLibrary.Prefabs.TryGetValue("DestinationMarkerPrefab", out var prefab))
            destinationMarkerPrefab = prefab;
    }

    private void Update()
    {
        switch (inputMode)
        {
            case OverworldHeroInputMode.VirtualJoystick:
                TickVirtualJoystick();
                break;
            case OverworldHeroInputMode.ClickToMove:
                TickClickToMove();
                break;
            case OverworldHeroInputMode.FollowCursor:
                TickDirectionalPress();
                break;
        }
    }

    // ---------------- VirtualJoystick ----------------
    private void TickVirtualJoystick()
    {
        Vector2 effectiveInput = Vector2.zero;
        bool joystickActive = allowVirtualJoystick && (analogInput.sqrMagnitude > 0.01f);
        if (joystickActive)
        {
            effectiveInput = Vector2.ClampMagnitude(analogInput, 1f);
            directionalActive = false; // joystick cancels directional latch
        }

        if (effectiveInput.sqrMagnitude > 1e-6f)
        {
            if (requireVisibleToMove && !IsVisible()) { if (idleWhileOffscreen) SetIdle(); return; }

            Vector2 current = GetPosition();
            float inputMag = Mathf.Clamp01(effectiveInput.magnitude);
            Vector2 dir = inputMag > 1e-6f ? effectiveInput / inputMag : Vector2.zero;
            float baseStepLen = moveSpeed * Time.deltaTime * inputMag;
            float mult = GetSpeedMultiplier(current + dir * (baseStepLen * speedSampleAheadFactor));
            Vector2 step = dir * (baseStepLen * mult);

            SetAnimationFromInput(dir, step.magnitude); // always drive animator

            Vector2 desired = ClampToMap(current + step);
            Vector2 next = ResolveCollision(current, desired);
            Vector2 frameDelta = next - current;

            SetPosition(next);
            if (frameDelta.sqrMagnitude > 1e-6f) OnHeroMoved?.Invoke(next);

            isMoving = false; _path = null; // ensure click path cancelled
        }
        else
        {
            SetIdle();
            isMoving = false; _path = null;
        }
    }

    // ---------------- ClickToMove ----------------
    private void TickClickToMove()
    {
        // Ignore analog input entirely
        directionalActive = false; // ensure no directional override

        if (!AllowClickToMove)
        {
            SetIdle(); isMoving = false; _path = null; return;
        }

        if (!isMoving)
        {
            SetIdle(); return;
        }

        if (requireVisibleToMove && !IsVisible()) { if (idleWhileOffscreen) SetIdle(); return; }

        Vector2 cur = GetPosition();

        if (_path != null && _pathIndex < _path.Count)
        {
            Vector2 wp = _path[_pathIndex];
            Vector2 toWp = wp - cur; float distWp = toWp.magnitude;
            if (distWp <= Mathf.Max(waypointArrive, snapThreshold))
            {
                _pathIndex++;
                if (_pathIndex >= _path.Count)
                {
                    SetPosition(wp); OnHeroMoved?.Invoke(wp); isMoving = false; SetIdle(); return;
                }
                wp = _path[_pathIndex]; toWp = wp - cur; distWp = toWp.magnitude;
            }

            float maxStep = moveSpeed * Time.deltaTime;
            Vector2 dir = distWp > 1e-6f ? (toWp / distWp) : Vector2.zero;
            float moveMult = GetSpeedMultiplier(cur + dir * (maxStep * speedSampleAheadFactor));
            float stepLen = Mathf.Min(maxStep * moveMult, distWp);
            Vector2 desiredNext = ClampToMap(cur + dir * stepLen);
            Vector2 nextPos = ResolveCollision(cur, desiredNext);
            Vector2 delta = nextPos - cur;

            if (delta.sqrMagnitude > 1e-6f)
            {
                SetAnimation(delta);
                SetPosition(nextPos);
                OnHeroMoved?.Invoke(nextPos);
            }
            else
            {
                _path = null; isMoving = false; SetIdle();
            }
            return;
        }

        if (Vector2.Distance(cur, targetPosition) <= snapThreshold)
        {
            SetPosition(targetPosition); OnHeroMoved?.Invoke(targetPosition); isMoving = false; SetIdle(); return;
        }

        Vector2 toTarget = targetPosition - cur; float dist = toTarget.magnitude;
        float maxStep2 = moveSpeed * Time.deltaTime; Vector2 stepDir = dist > 1e-6f ? (toTarget / dist) : Vector2.zero;
        float moveMult2 = GetSpeedMultiplier(cur + stepDir * (maxStep2 * speedSampleAheadFactor));
        float stepLen2 = Mathf.Min(maxStep2 * moveMult2, dist);
        Vector2 desiredNext2 = ClampToMap(cur + stepDir * stepLen2);
        Vector2 nextPos2 = ResolveCollision(cur, desiredNext2); Vector2 delta2 = nextPos2 - cur;

        if (delta2.sqrMagnitude > 1e-6f)
        {
            SetAnimation(delta2);
            SetPosition(nextPos2);
            OnHeroMoved?.Invoke(nextPos2);
        }
        else
        {
            isMoving = false; SetIdle();
        }
    }

    // ---------------- DirectionalPress ----------------
    private void TickDirectionalPress()
    {
        // Ignore analog input, use directional override only
        Vector2 effectiveInput = (directionalActive && directionalOverride.sqrMagnitude > 1e-6f)
            ? Vector2.ClampMagnitude(directionalOverride, 1f) : Vector2.zero;

        if (effectiveInput.sqrMagnitude > 1e-6f)
        {
            if (requireVisibleToMove && !IsVisible()) { if (idleWhileOffscreen) SetIdle(); return; }

            Vector2 current = GetPosition();
            float inputMag = Mathf.Clamp01(effectiveInput.magnitude);
            Vector2 dir = inputMag > 1e-6f ? effectiveInput / inputMag : Vector2.zero;
            float baseStepLen = moveSpeed * Time.deltaTime * inputMag;
            float mult = GetSpeedMultiplier(current + dir * (baseStepLen * speedSampleAheadFactor));
            Vector2 step = dir * (baseStepLen * mult);

            SetAnimationFromInput(dir, step.magnitude);

            Vector2 desired = ClampToMap(current + step);
            Vector2 next = ResolveCollision(current, desired);
            Vector2 frameDelta = next - current;

            SetPosition(next);
            if (frameDelta.sqrMagnitude > 1e-6f) OnHeroMoved?.Invoke(next);
        }
        else
        {
            SetIdle();
        }

        // While in directional mode we never follow click paths
        isMoving = false; _path = null;
    }

    // ---------------- External input API ----------------

    // Accepts analog input from OverworldManager each frame.
    public void SetAnalogInput(Vector2 input)
    {
        analogInput = Vector2.ClampMagnitude(input, 1f);
        if (inputMode == OverworldHeroInputMode.VirtualJoystick && analogInput.sqrMagnitude > 0.01f)
        {
            isMoving = false;        // cancel click-to-move while analog active
            directionalActive = false;
            _path = null;
        }
    }

    // Click/tap entry from screen space (Content parameter ignored; kept for API compatibility)
    public void HandleClickScreen(Vector2 screenPos, UnityEngine.RectTransform _)
    {
        var cam = worldCamera != null ? worldCamera : Camera.main;
        float zDist = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));
        HandleClickLocal(new Vector2(wp.x, wp.y));
    }

    public void HandleClickLocal(Vector2 world)
    {
        if (inputMode != OverworldHeroInputMode.ClickToMove) return;
        if (!AllowClickToMove) return;
        if (requireVisibleToMove && ignoreClicksWhenOffscreen && !IsVisible()) return;

        SetDestinationLocal(world);
    }

    // Sets a click-to-move destination (world space)
    public void SetDestinationLocal(Vector2 world)
    {
        if (inputMode != OverworldHeroInputMode.ClickToMove) return;
        if (!AllowClickToMove) return;

        targetPosition = ClampToMap(world);

        Vector2 delta = targetPosition - GetPosition();
        if (delta.sqrMagnitude > 1e-6f) SetAnimation(delta);

        isMoving = true;
        directionalActive = false; // ensure point-move owns motion

        // Build path (if enabled)
        _path = null; _pathIndex = 0;
        if (usePathfinding && collisionProvider != null)
        {
            if (TryComputePath(GetPosition(), targetPosition, out var path))
            {
                _path = path;
                _pathIndex = 0;
            }
        }

        // Choose a visual marker position matching the final stop
        Vector2 markerPos;
        if (_path != null && _path.Count > 0)
        {
            markerPos = _path[_path.Count - 1];
        }
        else
        {
            markerPos = PredictStop(GetPosition(), targetPosition);
        }

        // Spawn a destination marker (self-managed)
        if (destinationMarkerPrefab != null)
        {
            var marker = Instantiate(destinationMarkerPrefab);
            marker.transform.position = new Vector3(markerPos.x, markerPos.y, transform.position.z);
        }
    }

    // Hold-to-move directional clicks (screen param kept for compatibility)
    public void BeginDirectionalFromScreen(Vector2 screenPos, UnityEngine.RectTransform _)
    {
        if (inputMode != OverworldHeroInputMode.FollowCursor) return;
        var cam = worldCamera != null ? worldCamera : Camera.main;
        float zDist = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));
        SetDirectionalOverride(new Vector2(wp.x, wp.y));
    }

    public void UpdateDirectionalFromScreen(Vector2 screenPos, UnityEngine.RectTransform _)
    {
        if (inputMode != OverworldHeroInputMode.FollowCursor) return;
        if (!directionalActive) return;
        var cam = worldCamera != null ? worldCamera : Camera.main;
        float zDist = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));
        SetDirectionalOverride(new Vector2(wp.x, wp.y));
    }

    public void FullStop()
    {
        directionalActive = false;
        directionalOverride = Vector2.zero;
        SetIdle();
    }

    // ---------------- Animation helpers (8-way blend tree) ----------------

    private void SetDirectionalOverride(Vector2 world)
    {
        if (inputMode != OverworldHeroInputMode.FollowCursor) return;
        Vector2 delta = ClampToMap(world) - GetPosition();
        float dist = delta.magnitude;
        if (dist < 1e-6f)
        {
            directionalActive = false;
            return;
        }

        // Map distance -> analog magnitude [0..1] so we reuse joystick movement
        float ramp = followSpeedRampDistance > 0f ? (dist / followSpeedRampDistance) : 1f;
        float mag = Mathf.Clamp01(ramp) * Mathf.Clamp01(directionalClickMagnitude);
        Vector2 dir = (delta / dist) * mag;

        directionalOverride = dir;
        directionalActive = true;

        // Immediate visual feedback
        SetAnimation(dir);
        isMoving = false; // use analog-like path instead
    }

    private void SetAnimation(Vector2 delta)
    {
        float speed = delta.magnitude;           // units moved this frame
        Vector2 dir = speed > 1e-6f ? delta.normalized : lastLook;
        dir = StabilizeDirectionForBlend(dir);

        lastLook = dir;                          // remember facing for idle
        lastDirection = DetermineDirection4Way(dir); // maintain legacy 4-way for save text

        ApplyAnimatorParameters(dir, speed);
    }

    private void SetAnimationFromInput(Vector2 dir, float speed)
    {
        if (dir.sqrMagnitude < 1e-6f)
        {
            SetIdle();
            return;
        }
        dir = dir.normalized;
        dir = StabilizeDirectionForBlend(dir);
        lastLook = dir;
        lastDirection = DetermineDirection4Way(dir);
        ApplyAnimatorParameters(dir, speed);
    }

    private void SetIdle()
    {
        lastDirection = MoveDirection.Idle;
        ApplyAnimatorParameters(lastLook, 0f);
    }

    private void ApplyAnimatorParameters(Vector2 dir, float speed)
    {
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
        // Convert per-frame distance into units/second so transitions with Speed>0.1f work reliably
        float speedPerSecond = speed;
        if (Application.isPlaying && Time.deltaTime > 0f)
            speedPerSecond = speed / Time.deltaTime;
        animator.SetFloat("Speed", speedPerSecond);
    }

    private Vector2 StabilizeDirectionForBlend(Vector2 dir)
    {
        if (dir.sqrMagnitude < 1e-6f) return dir;
        float ax = Mathf.Abs(dir.x);
        float ay = Mathf.Abs(dir.y);
        Vector2 d = dir;

        // Snap to pure vertical when vertical dominates and horizontal is tiny
        if (ay >= ax * axisDominance && ax < axisSnapEpsilon)
        {
            d.x = 0f;
            d = d.normalized;
        }
        // Snap to pure horizontal when horizontal dominates and vertical is tiny
        else if (ax >= ay * axisDominance && ay < axisSnapEpsilon)
        {
            d.y = 0f;
            d = d.normalized;
        }
        return d;
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

    // ---------------- Visibility and clamping (world space) ----------------

    private bool IsVisible()
    {
        if (!requireVisibleToMove) return true;
        var cam = worldCamera != null ? worldCamera : Camera.main;
        Vector3 v = cam.WorldToViewportPoint(transform.position);
        return v.z > 0f && v.x >= 0f && v.x <= 1f && v.y >= 0f && v.y <= 1f;
    }

    private Vector2 ClampToMap(Vector2 p)
    {
        // World-space clamp against sprite bounds
        Bounds b = terrainSprite.bounds;
        float cx = Mathf.Clamp(p.x, b.min.x, b.max.x);
        float cy = Mathf.Clamp(p.y, b.min.y, b.max.y);
        return new Vector2(cx, cy);
    }

    // ---------------- Collision and speed sampling ----------------

    public void BindWorld(SpriteRenderer map, Camera cam)
    {
        terrainSprite = map;
        worldCamera = cam;
        if (collisionProvider == null && terrainSprite != null)
            collisionProvider = terrainSprite.GetComponent<MapTerrain>();
    }

    public void BindCollisionProvider(MapTerrain provider)
    {
        collisionProvider = provider;
    }

  
    private float GetSpeedMultiplier(Vector2 world)
    {
        return 1f; // constant speed (slow zones can be added later)
    }

    // Simple normal-based slide + axis fallback
    private Vector2 ResolveCollision(Vector2 current, Vector2 desired)
    {
        if (IsWalkableWorld(desired))
            return desired;

        Vector2 step = desired - current;
        if (step.sqrMagnitude <= 1e-10f)
            return current;

        // Try sliding along the obstacle using a contact normal (at the collision-probe center)
        Vector2 desiredCenter = GetVisualCenter(desired);
        float pr = GetProbeRadius();
        float nRadius = Mathf.Max(2f, pr > 0f ? pr * 0.5f : 6f);
        int nRays = Mathf.Max(8, 8);

        Vector2 n = collisionProvider.EstimateObstacleNormal(desiredCenter, nRadius, nRays);
        if (n.sqrMagnitude > 1e-6f)
        {
            // Remove normal component -> tangent slide
            Vector2 tangent = step - Vector2.Dot(step, n) * n;
            // Try full slide first, then smaller fractions with a small push away from the wall
            if (tangent.sqrMagnitude > 1e-6f)
            {
                float frac = 1f;
                for (int i = 0; i < Mathf.Max(1, wallSlideAttempts); i++)
                {
                    Vector2 candidate = ClampToMap(current + tangent * frac + n * wallSlideUnstick);
                    if (IsWalkableWorld(candidate))
                        return candidate;
                    frac *= 0.5f;
                    if (frac < wallSlideMinFraction) break;
                }
            }
        }

        // Fallback: axis-aligned slides (robust on corners) with slight unstick when available
        Vector2 tryX = new Vector2(desired.x, current.y);
        tryX = ClampToMap(tryX);
        if (IsWalkableWorld(tryX))
            return tryX;

        Vector2 tryY = new Vector2(current.x, desired.y);
        tryY = ClampToMap(tryY);
        if (IsWalkableWorld(tryY))
            return tryY;

        // Blocked; stay put
        return current;
    }

    // Walkable according to MapTerrain (world space)
    private bool IsWalkableWorld(Vector2 p)
    {
        Vector2 center = GetVisualCenter(p); // pivot + feet offset at desired world pos
        float radius = GetProbeRadius();
        int rays = Mathf.Max(8, 8);

        if (!collisionProvider.IsWalkableLocal(center)) return false;
        if (radius > 0f && rays > 0)
        {
            float step = 360f / rays;
            for (int i = 0; i < rays; i++)
            {
                float ang = step * i * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
                if (!collisionProvider.IsWalkableLocal(center + offset))
                    return false;
            }
        }
        return true;
    }

    private float GetProbeRadius()
    {
        if (useFixedCollisionRadius && collisionRadiusWorld > 0f)
            return collisionRadiusWorld;

        // Fallback: infer from current sprite bounds (can vary with animation)
        return Mathf.Min(
            heroSprite.bounds.extents.x * probeRadiusMultiplier,
            heroSprite.bounds.extents.y * probeRadiusMultiplier);
    }

    // Collision-probe center: always use Animator/Sprite pivot plus optional feet offset.
    private Vector2 GetVisualCenter(Vector2 desiredWorldPosition)
    {
        // In 2D SpriteRenderer, transform.position == sprite pivot in world units.
        // We evaluate at the desired position passed in, not the current transform, to test feasibility.
        Vector2 pivotWorld = desiredWorldPosition;
        if (collisionFeetOffsetLocal != Vector2.zero)
        {
            // Convert local-space offset to world considering only rotation (scale assumed uniform for world-space sprites)
            Vector2 worldOffset = (Vector2)(transform.rotation * (Vector3)collisionFeetOffsetLocal);
            pivotWorld += worldOffset;
        }
        return pivotWorld;
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

    // ---------------- A* Pathfinding helpers (world space) ----------------

    private bool TryComputePath(Vector2 startWorld, Vector2 goalWorld, out System.Collections.Generic.List<Vector2> path)
    {
        path = null;
        if (navCellSize <= 0) return false;

        Bounds b = terrainSprite.bounds;
        float minX = b.min.x, maxX = b.max.x;
        float minY = b.min.y, maxY = b.max.y;

        int cols = Mathf.Max(1, Mathf.CeilToInt((maxX - minX) / navCellSize));
        int rows = Mathf.Max(1, Mathf.CeilToInt((maxY - minY) / navCellSize));

        System.Func<Vector2, (int gx, int gy)> W2G = (Vector2 p) =>
        {
            int gx = Mathf.Clamp((int)Mathf.Floor((p.x - minX) / navCellSize), 0, cols - 1);
            int gy = Mathf.Clamp((int)Mathf.Floor((p.y - minY) / navCellSize), 0, rows - 1);
            return (gx, gy);
        };
        System.Func<int, int, Vector2> G2W = (int gx, int gy) =>
        {
            float x = minX + (gx + 0.5f) * navCellSize;
            float y = minY + (gy + 0.5f) * navCellSize;
            return new Vector2(x, y);
        };

        return ComputePathImpl(startWorld, goalWorld, W2G, G2W, cols, rows, out path);
    }

    private bool ComputePathImpl(
        Vector2 start,
        Vector2 goal,
        System.Func<Vector2, (int gx, int gy)> toGrid,
        System.Func<int, int, Vector2> toSpace,
        int cols,
        int rows,
        out System.Collections.Generic.List<Vector2> path)
    {
        path = null;

        // Clearance radius (hero body + buffer)
        float heroRadius = GetProbeRadius();
        float clearance = heroRadius + Mathf.Max(0f, navObstacleBuffer);
        int rays = 8;

        // Cell walkability cache
        var walkCache = new System.Collections.Generic.Dictionary<int, bool>(1024);
        System.Func<int, int, bool> CellWalkable = (int gx, int gy) =>
        {
            if (gx < 0 || gy < 0 || gx >= cols || gy >= rows) return false;
            int key = (gy * 32749) ^ gx; // simple hash
            if (walkCache.TryGetValue(key, out bool ok)) return ok;
            Vector2 sample = toSpace(gx, gy);
            bool w = collisionProvider.IsWalkableLocal(sample, clearance, rays);
            walkCache[key] = w;
            return w;
        };

        // A* node structure
        var open = new System.Collections.Generic.List<(int gx, int gy, float f, float g)>();
        var came = new System.Collections.Generic.Dictionary<int, (int px, int py)>();
        var gScore = new System.Collections.Generic.Dictionary<int, float>();
        var closed = new System.Collections.Generic.HashSet<int>();

        (int sx, int sy) = toGrid(start);
        (int tx, int ty) = toGrid(goal);

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

        System.Func<int, int, float> Heuristic = (int gx, int gy) =>
        {
            // Octile distance (grid-diagonal aware)
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
        gScore[(sy * 32749) ^ sx] = 0f;

        int expanded = 0;
        int[] n8x = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] n8y = { -1, -1, -1, 0, 0, 1, 1, 1 };
        float[] nCost = { 1.41421356f, 1f, 1.41421356f, 1f, 1f, 1.41421356f, 1f, 1.41421356f };

        while (open.Count > 0)
        {
            // pop lowest f (linear scan; lists remain small)
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
                rev.Add(toSpace(cx, cy));
                while (came.TryGetValue(ckey, out var p))
                {
                    cx = p.px; cy = p.py; ckey = (cy * 32749) ^ cx;
                    rev.Add(toSpace(cx, cy));
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

    // ---------------- helpers ----------------

    private Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    private void SetPosition(Vector2 v)
    {
        transform.position = new Vector3(v.x, v.y, transform.position.z);
    }

    // Inspector toggles via code (optional helpers)
    public void SetMoveSpeed(float unitsPerSecond) => moveSpeed = Mathf.Max(0f, unitsPerSecond);
    public void SetSnapThreshold(float value) => snapThreshold = Mathf.Max(0f, value);
    public void SetPathfinding(bool enabled) => usePathfinding = enabled;
    public void SetInputMode(OverworldHeroInputMode mode) => inputMode = mode;

    // Exposed setters for tuning friction and clearance
    public void SetProbeRadiusMultiplier(float value) => probeRadiusMultiplier = Mathf.Max(0f, value);
    public void SetNavClearance(float value) => navObstacleBuffer = Mathf.Max(0f, value);
    public void SetWallSlideUnstick(float epsilon) => wallSlideUnstick = Mathf.Max(0f, epsilon);
    public void SetFollowSpeedRampDistance(float dist) => followSpeedRampDistance = Mathf.Max(0.01f, dist);

    // New: control collision sampling relative to animator pivot
    public void SetFeetOffsetLocal(Vector2 offset) => collisionFeetOffsetLocal = offset;
    public void SetFixedCollisionRadius(float worldRadius) { useFixedCollisionRadius = true; collisionRadiusWorld = Mathf.Max(0f, worldRadius); }
    public void UseInferredCollisionRadius(bool enabled) => useFixedCollisionRadius = !enabled;
}
