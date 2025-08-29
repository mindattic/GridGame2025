using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;

public class PlayerStageMover : MonoBehaviour
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
    [SerializeField] private bool requireVisibleToMove = true;     // Only step when visible
    [SerializeField] private bool ignoreClicksWhenOffscreen = false; // If true, ignore clicks when offscreen
    [SerializeField] private bool idleWhileOffscreen = true;       // Show Idle while paused off-screen

    [Header("Animator States (optional overrides)")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string upState = "MoveUp";
    [SerializeField] private string rightState = "MoveRight";
    [SerializeField] private string downState = "MoveDown";
    [SerializeField] private string leftState = "MoveLeft";

    private bool isMoving;
    private Vector2 targetPosition;
    private MoveDirection lastDirection = MoveDirection.Idle;

    private void Awake()
    {
        // Auto-bind common components if not set
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

        GameReady.Begin(this);
    }

    private void Update()
    {
        if (rect == null) return;
        if (!isMoving) return;

        // Pause movement while the hero is off-screen (map can still scroll)
        if (requireVisibleToMove && !IsTargetVisible(rect, viewport))
        {
            if (idleWhileOffscreen)
            {
                lastDirection = MoveDirection.Idle;
                SetAnimatorDirection(lastDirection);
            }
            return;
        }

        Vector2 current = rect.anchoredPosition;

        // Arrived (snap and finish)
        if (Vector2.Distance(current, targetPosition) <= snapThreshold)
        {
            rect.anchoredPosition = targetPosition;
            OnHeroMoved?.Invoke(targetPosition);

            isMoving = false;
            lastDirection = MoveDirection.Idle;
            SetAnimatorDirection(lastDirection);
            CrossFadeIfExists(idleState);
            return;
        }

        // Advance towards target and compute direction from actual per-frame delta
        Vector2 next = Vector2.MoveTowards(current, targetPosition, moveSpeed * Time.deltaTime);
        Vector2 frameDelta = next - current; // previous -> next determines facing each frame
        if (frameDelta.sqrMagnitude > 1e-6f)
            SetAnimation(frameDelta);

        rect.anchoredPosition = next;
        OnHeroMoved?.Invoke(next);
    }

    // Called by OverworldManager after converting to content-local
    public void SetDestinationLocal(Vector2 local)
    {
        if (!AllowClickToMove || rect == null || rect.parent == null) return;

        // If desired, ignore clicks while off-screen
        if (requireVisibleToMove && ignoreClicksWhenOffscreen && !IsTargetVisible(rect, viewport))
            return;

        targetPosition = ClampToMap(local);

        // Face toward the new target immediately
        Vector2 delta = targetPosition - rect.anchoredPosition;
        if (delta.sqrMagnitude > 1e-6f)
            SetAnimation(delta);

        isMoving = true; // Update will step toward target; scrolling can happen simultaneously
    }

    // Convenience wrapper if you get screen-space clicks here instead of in OverworldManager
    public void SetDestinationScreen(Vector2 screenPos, RectTransform content)
    {
        if (content == null) return;
        var local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        SetDestinationLocal(local);
    }

    // True if any corner of target is inside viewport
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

    private void SetAnimatorDirection(MoveDirection dir)
    {
        if (animator != null)
            animator.SetInteger("MoveDirection", (int)dir);
    }

    private static MoveDirection DetermineDirection(Vector2 delta)
    {
        // Decide facing by dominant axis from previous position -> next position
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
