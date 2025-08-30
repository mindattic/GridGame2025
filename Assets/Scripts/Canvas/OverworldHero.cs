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
    public float moveSpeed = 60f;

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

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (animator == null) animator = GetComponent<Animator>();

        if (mapRect == null)
        {
            var go = GameObject.Find(GameObjectHelper.Overworld.Map);
            if (go != null) mapRect = go.GetComponent<RectTransform>();
        }
        if (viewport == null)
        {
            var vp = GameObject.Find(GameObjectHelper.Overworld.Viewport);
            if (vp != null) viewport = vp.GetComponent<RectTransform>();
        }
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
            Vector2 step = effectiveInput * moveSpeed * Time.deltaTime;
            Vector2 next = ClampToMap(current + step);
            Vector2 frameDelta = next - current;

            if (frameDelta.sqrMagnitude > 1e-6f)
                SetAnimation(frameDelta);

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

        // Advance towards target and animate
        Vector2 nextPos = Vector2.MoveTowards(cur, targetPosition, moveSpeed * Time.deltaTime);
        Vector2 delta = nextPos - cur;
        if (delta.sqrMagnitude > 1e-6f)
            SetAnimation(delta);

        rect.anchoredPosition = nextPos;
        OnHeroMoved?.Invoke(nextPos);
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
}
