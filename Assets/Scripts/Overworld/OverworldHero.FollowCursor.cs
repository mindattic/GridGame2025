using UnityEngine;

public partial class OverworldHero
{
    // ---------------- FollowCursor (DirectionalPress) ----------------
    private void TickFollowCursor()
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

            // Move using collider cast-and-slide
            MoveWithCast(step);

            // Drive animator from desired input direction and speed
            SetAnimationFromInput(dir, step.magnitude);
        }
        else
        {
            SetIdle();
        }

        // Always update Y-sort so the hero's order follows their Y
        var sr = spriteRenderer != null ? spriteRenderer : GetComponent<SpriteRenderer>();
        if (sr != null)
            PartySortHelper.ApplyActorYSort(sr, PartySortHelper.GlobalScale);

        // While in directional mode we never follow click paths
        isMoving = false; _path = null;
    }

    // Hold-to-move directional clicks (screen param kept for compatibility)
    public void BeginDirectionalFromScreen(Vector2 screenPos, RectTransform _)
    {
        var cam = worldCamera != null ? worldCamera : Camera.main;
        Vector3 wp = Mode7CameraController.ScreenToWorldOnZPlane(cam, screenPos, transform.position.z);
        SetDirectionalOverride(new Vector2(wp.x, wp.y));
    }

    public void UpdateDirectionalFromScreen(Vector2 screenPos, RectTransform _)
    {
        if (!directionalActive) return;
        var cam = worldCamera != null ? worldCamera : Camera.main;
        Vector3 wp = Mode7CameraController.ScreenToWorldOnZPlane(cam, screenPos, transform.position.z);
        SetDirectionalOverride(new Vector2(wp.x, wp.y));
    }

    public void FullStop()
    {
        directionalActive = false;
        directionalOverride = Vector2.zero;
        SetIdle();
    }

    private void SetDirectionalOverride(Vector2 world)
    {
        Vector2 delta = ClampToMap(world) - GetPosition();
        float dist = delta.magnitude;
        if (dist < 1e-6f)
        {
            directionalActive = false;
            return;
        }

        // Constant magnitude so FollowCursor speed is static (no distance ramp)
        float mag = Mathf.Clamp01(directionalClickMagnitude);
        Vector2 dir = (delta / dist) * mag;

        directionalOverride = dir;
        directionalActive = true;

        // Immediate visual feedback
        SetAnimation(dir);
        isMoving = false; // use analog-like path instead
    }
}
