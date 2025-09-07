using System;
using Assets.Helper;
using UnityEngine;

// OverworldFollower: follows a leader (e.g., OverworldHero) keeping a configurable distance
[ExecuteAlways]
public class OverworldFollower : MonoBehaviour
{
    [Header("Leader")]
    [SerializeField] private Transform leader;                // Target to follow (OverworldHero or any Transform)
    public Transform Leader => leader;                        // Read-only access for party queries

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.3f;          // Base move speed in units/sec
    [SerializeField] private float followDistance = 0.75f;    // Desired trailing distance
    [SerializeField] private float arriveBuffer = 0.05f;      // Dead-zone around followDistance to prevent jitter
    [SerializeField] private float catchupMultiplier = 2.0f;  // Max speed multiplier when very far
    [SerializeField] private float teleportIfBeyond = 25f;    // Teleport to leader if separated beyond this distance (0 to disable)

    [Header("Collision (optional)")]
    [SerializeField] private bool enableCollision = false;    // Uses rigidbody casts like the hero
    [SerializeField] private float skin = 0.01f;
    [SerializeField] private int maxSlideIterations = 3;

    [Header("World Bounds (optional)")]
    [SerializeField] private bool clampToMap = true;

    [Header("Party Collision")]
    [Tooltip("If enabled, ignore collisions between this follower and its leader (e.g., OverworldHero) while still colliding with world objects.")]
    [SerializeField] private bool ignoreLeaderCollision = true;

    // Animator driving 8-way blend tree (same params as hero: MoveX, MoveY, Speed)
    public Animator animator;

    // World references (resolved automatically if possible)
    private SpriteRenderer terrainSprite;  // For clamping
    private Camera worldCamera;            // Not currently used, reserved

    // Runtime
    private Rigidbody2D rb;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hitBuffer;

    // Cache colliders for ignore rules
    private Collider2D[] selfColliders;
    private Transform lastLeaderForIgnore;

    // Animation state (mirrors OverworldHero)
    private Vector2 lastLook = Vector2.down;                 // 8-way facing memory
    private MoveDirection lastDirection = MoveDirection.Idle; // 4-way legacy

    // Stabilization constants (same as hero)
    private const float axisSnapEpsilon = 0.12f;   // if minor axis below this, snap to 0
    private const float axisDominance = 1.35f;     // major must be >= minor * dominance

    // Events
    public event Action<Vector2> OnFollowerMoved;  // Invoked with world position after movement

    private void Awake()
    {
        worldCamera = Camera.main;

        // Try resolve terrain for clamping
        var terrainGo = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain);
        if (terrainGo != null) terrainSprite = terrainGo.GetComponent<SpriteRenderer>();

        if (animator == null) animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            hitBuffer = new RaycastHit2D[16];
            contactFilter.useTriggers = false;
            contactFilter.useLayerMask = true;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            if (rb.bodyType == RigidbodyType2D.Dynamic)
                rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Cache colliders for party ignore rules
        CacheSelfColliders();
        ApplyIgnoreWithLeader();

        // Initialize animator to idle facing
        ApplyAnimatorParameters(lastLook, 0f);
    }

    private void OnEnable()
    {
        // Ensure ignore rules get applied when enabled (e.g., after scene load)
        CacheSelfColliders();
        ApplyIgnoreWithLeader();
    }

    private void OnDisable()
    {
        // Restore any ignore rules to avoid leaking state if re-enabled with a different leader
        if (Application.isPlaying)
        {
            if (lastLeaderForIgnore != null)
            {
                ToggleIgnoreWith(lastLeaderForIgnore, false);
                lastLeaderForIgnore = null;
            }
        }
    }

    private void OnDestroy()
    {
        // Also restore on destroy (play mode)
        if (Application.isPlaying)
        {
            if (lastLeaderForIgnore != null)
            {
                ToggleIgnoreWith(lastLeaderForIgnore, false);
                lastLeaderForIgnore = null;
            }
        }
    }

    private void OnValidate()
    {
        // Keep ignore rules updated in editor when settings change during play
        if (!Application.isPlaying) return;
        CacheSelfColliders();
        ApplyIgnoreWithLeader();
    }

    private void Update()
    {
        TickFollowLeader();
    }

    public void SetLeader(Transform t)
    {
        if (t == leader) return;
        // Remove rules for previous leader
        if (Application.isPlaying && lastLeaderForIgnore != null)
        {
            ToggleIgnoreWith(lastLeaderForIgnore, false);
            lastLeaderForIgnore = null;
        }
        leader = t;
        ApplyIgnoreWithLeader();
    }

    public void SetLeader(OverworldHero h)
    {
        SetLeader(h != null ? h.transform : null);
    }

    public void SetMoveSpeed(float s) => moveSpeed = Mathf.Max(0f, s);
    public void SetFollowDistance(float d) => followDistance = Mathf.Max(0f, d);

    private void TickFollowLeader()
    {
        if (leader == null)
        {
            SetIdle();
            return;
        }

        Vector2 current = GetPosition();
        Vector3 lp3 = leader.position;
        Vector2 leaderPos = new Vector2(lp3.x, lp3.y);
        Vector2 toLeader = leaderPos - current;
        float dist = toLeader.magnitude;

        // Teleport if extremely far to keep party coherent
        if (teleportIfBeyond > 0f && dist > teleportIfBeyond)
        {
            Vector2 snap = leaderPos;
            if (followDistance > 1e-4f && dist > 1e-6f)
                snap = leaderPos - toLeader / dist * followDistance;
            SetPosition(ClampToMapIfNeeded(snap));
            ApplyAnimatorParameters(lastLook, 0f);
            OnFollowerMoved?.Invoke(GetPosition());
            return;
        }

        // If outside the comfort ring, move toward leader; else idle
        float outer = Mathf.Max(0f, followDistance + arriveBuffer);
        if (dist > outer)
        {
            Vector2 dir = toLeader / Mathf.Max(dist, 1e-6f);

            // Distance-based catchup (speeds up when far, clamped by catchupMultiplier)
            float catchup = 1f;
            if (followDistance > 1e-4f)
            {
                float t = Mathf.InverseLerp(followDistance, followDistance * 4f, dist);
                catchup = Mathf.Lerp(1f, Mathf.Max(1f, catchupMultiplier), t);
            }

            float stepLen = moveSpeed * catchup * Time.deltaTime;
            Vector2 step = dir * stepLen;

            // Attempt not to overshoot near the target ring edge
            float overshoot = dist - outer;
            if (step.magnitude > overshoot)
                step = dir * overshoot;

            MoveWithCast(step);
            SetAnimationFromInput(dir, step.magnitude);
        }
        else
        {
            SetIdle();
        }
    }

    // ---------------- Position helpers ----------------
    private Vector2 GetPosition()
    {
        if (rb != null) return rb.position;
        return new Vector2(transform.position.x, transform.position.y);
    }

    private void SetPosition(Vector2 v)
    {
        if (rb != null) rb.position = v;
        transform.position = new Vector3(v.x, v.y, transform.position.z);
        Physics2D.SyncTransforms();
    }

    private Vector2 ClampToMapIfNeeded(Vector2 p)
    {
        if (!clampToMap || terrainSprite == null) return p;
        Bounds b = terrainSprite.bounds;
        float cx = Mathf.Clamp(p.x, b.min.x, b.max.x);
        float cy = Mathf.Clamp(p.y, b.min.y, b.max.y);
        return new Vector2(cx, cy);
    }

    // ---------------- Collision (optional) ----------------
    private void MoveWithCast(Vector2 displacement)
    {
        if (displacement.sqrMagnitude <= 1e-10f) return;

        if (!enableCollision || rb == null)
        {
            Vector2 freeNext = ClampToMapIfNeeded(GetPosition() + displacement);
            SetPosition(freeNext);
            OnFollowerMoved?.Invoke(freeNext);
            return;
        }

        Vector2 currentPos = GetPosition();
        Vector2 nextPos = currentPos;
        PlanCastMove(ref nextPos, displacement.normalized, displacement.magnitude);
        if ((nextPos - currentPos).sqrMagnitude > 1e-8f)
        {
            SetPosition(nextPos);
            OnFollowerMoved?.Invoke(nextPos);
        }
    }

    private bool IsLeaderCollider(Collider2D c)
    {
        if (c == null || leader == null) return false;
        var t = c.transform;
        return t != null && (t == leader || t.IsChildOf(leader));
    }

    private void PlanCastMove(ref Vector2 nextPos, Vector2 dir, float distance)
    {
        if (rb == null || distance <= 1e-6f || dir.sqrMagnitude <= 1e-12f) return;
        dir = dir.normalized;

        Vector2 origin = rb.position;
        int hitCount = rb.Cast(dir, contactFilter, hitBuffer, distance + skin);
        if (hitCount == 0)
        {
            origin += dir * distance;
            nextPos = ClampToMapIfNeeded(origin);
            return;
        }

        float closest = Mathf.Infinity;
        int closestIndex = -1;
        bool anyOverlap = false;
        Vector2 overlapNormalSum = Vector2.zero;
        for (int h = 0; h < hitCount; h++)
        {
            var hit = hitBuffer[h];
            if (IsLeaderCollider(hit.collider))
                continue; // ignore leader entirely

            float d = hit.distance;
            if (d >= 0f)
            {
                if (d < closest) { closest = d; closestIndex = h; }
            }
            else
            {
                anyOverlap = true;
                overlapNormalSum += hit.normal;
            }
        }

        if (closestIndex < 0)
        {
            if (!anyOverlap)
            {
                origin += dir * distance;
                nextPos = ClampToMapIfNeeded(origin);
                return;
            }
            // Fully overlapping at origin: nudge outward to depenetrate
            Vector2 push;
            if (overlapNormalSum.sqrMagnitude > 1e-6f)
                push = overlapNormalSum.normalized * Mathf.Max(skin, 0.02f);
            else
                push = dir * Mathf.Max(skin, 0.02f);
            origin += push;
            nextPos = ClampToMapIfNeeded(origin);
            return;
        }

        float allowed = Mathf.Max(0f, closest - skin);
        origin += dir * allowed;

        float remain = Mathf.Max(0f, distance - allowed);
        if (remain <= 1e-6f) { nextPos = ClampToMapIfNeeded(origin); return; }

        Vector2 moveDir = dir;
        int iterations = Mathf.Max(1, maxSlideIterations);
        for (int i = 0; i < iterations && remain > 1e-6f; i++)
        {
            Vector2 n = hitBuffer[closestIndex].normal;
            Vector2 remainingVec = moveDir * remain;
            Vector2 slide = remainingVec - Vector2.Dot(remainingVec, n) * n;
            if (slide.sqrMagnitude <= 1e-8f) break;

            Vector2 sDir = slide.normalized;
            float sLen = slide.magnitude;

            int hitsSlide = rb.Cast(sDir, contactFilter, hitBuffer, sLen + skin);
            if (hitsSlide == 0)
            {
                origin += sDir * sLen;
                remain = 0f;
                break;
            }
            else
            {
                float closestSlide = Mathf.Infinity;
                int slideHitIndex = -1;
                for (int h = 0; h < hitsSlide; h++)
                {
                    var hit = hitBuffer[h];
                    if (IsLeaderCollider(hit.collider))
                        continue; // ignore leader during slide too

                    float d = hit.distance;
                    if (d >= 0f && d < closestSlide) { closestSlide = d; slideHitIndex = h; }
                }

                if (slideHitIndex < 0)
                {
                    origin += sDir * sLen;
                    remain = 0f;
                    break;
                }

                float allowSlide = Mathf.Max(0f, closestSlide - skin);
                if (allowSlide > 1e-6f)
                {
                    origin += sDir * allowSlide;
                    remain = Mathf.Max(0f, remain - allowSlide);
                    moveDir = sDir;
                    closestIndex = slideHitIndex;
                }
                else
                {
                    origin += sDir * Mathf.Max(0.001f, skin * 0.5f);
                    break;
                }
            }
        }

        nextPos = ClampToMapIfNeeded(origin);
    }

    // ---------------- Party self-collision rules ----------------
    private void CacheSelfColliders()
    {
        selfColliders = GetComponentsInChildren<Collider2D>(true);
    }

    private void ApplyIgnoreWithLeader()
    {
        if (!Application.isPlaying) return;

        // Remove previous ignore rules if leader changed or feature disabled
        if (lastLeaderForIgnore != null && (lastLeaderForIgnore != leader || !ignoreLeaderCollision))
        {
            ToggleIgnoreWith(lastLeaderForIgnore, false);
            lastLeaderForIgnore = null;
        }

        if (!ignoreLeaderCollision) return;
        if (leader == null) return;

        if (selfColliders == null || selfColliders.Length == 0)
            CacheSelfColliders();

        ToggleIgnoreWith(leader, true);
        lastLeaderForIgnore = leader;
    }

    private void ToggleIgnoreWith(Transform other, bool ignore)
    {
        if (other == null) return;
        if (selfColliders == null || selfColliders.Length == 0) return;
        var otherColliders = other.GetComponentsInChildren<Collider2D>(true);
        if (otherColliders == null || otherColliders.Length == 0) return;

        for (int i = 0; i < selfColliders.Length; i++)
        {
            var a = selfColliders[i];
            if (a == null) continue;
            for (int j = 0; j < otherColliders.Length; j++)
            {
                var b = otherColliders[j];
                if (b == null) continue;
                Physics2D.IgnoreCollision(a, b, ignore);
            }
        }
    }

    // ---------------- Animation (mirrors hero) ----------------
    private void SetAnimation(Vector2 delta)
    {
        float speed = delta.magnitude;
        Vector2 dir = speed > 1e-6f ? delta.normalized : lastLook;
        dir = StabilizeDirectionForBlend(dir);

        lastLook = dir;
        lastDirection = DetermineDirection4Way(dir);

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
        if (animator == null) return;
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
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

        if (ay >= ax * axisDominance && ax < axisSnapEpsilon)
        {
            d.x = 0f;
            d = d.normalized;
        }
        else if (ax >= ay * axisDominance && ay < axisSnapEpsilon)
        {
            d.y = 0f;
            d = d.normalized;
        }
        return d;
    }

    private static MoveDirection DetermineDirection4Way(Vector2 delta)
    {
        if (delta.sqrMagnitude < 1e-6f) return MoveDirection.Idle;
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            return delta.x >= 0 ? MoveDirection.Right : MoveDirection.Left;
        else
            return delta.y >= 0 ? MoveDirection.Up : MoveDirection.Down;
    }

    // Optional: force a facing via string (same as hero API)
    public void SetFacing(string facingName)
    {
        if (string.IsNullOrEmpty(facingName)) return;
        MoveDirection dir;
        if (!Enum.TryParse(facingName, true, out dir))
            dir = MoveDirection.Idle;

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
        lastDirection = dir;
        ApplyAnimatorParameters(lastLook, 0f);
    }
}
