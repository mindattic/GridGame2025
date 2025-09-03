using UnityEngine;

public partial class OverworldHero
{
    // ---------------- FollowCursor (DirectionalPress) ----------------
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

            // Look-ahead: stop before intersecting a wall
            if (WillHitWall(current, step)) { SetIdle(); return; }

            SetAnimationFromInput(dir, step.magnitude);

            Vector2 desired = ClampToMap(current + step);
            Vector2 next = ResolveCollision(current, desired);
            Vector2 frameDelta = next - current;

            SetPosition(next);
            bool moved = frameDelta.sqrMagnitude > 1e-6f;
            if (moved) OnHeroMoved?.Invoke(next);
        }
        else
        {
            SetIdle();
        }

        // While in directional mode we never follow click paths
        isMoving = false; _path = null;
    }

    // Hold-to-move directional clicks (screen param kept for compatibility)
    public void BeginDirectionalFromScreen(Vector2 screenPos, RectTransform _)
    {
        if (inputMode != OverworldHeroInputMode.FollowCursor) return;
        var cam = worldCamera != null ? worldCamera : Camera.main;
        Vector3 wp = Mode7CameraController.ScreenToWorldOnZPlane(cam, screenPos, transform.position.z);
        SetDirectionalOverride(new Vector2(wp.x, wp.y));
    }

    public void UpdateDirectionalFromScreen(Vector2 screenPos, RectTransform _)
    {
        if (inputMode != OverworldHeroInputMode.FollowCursor) return;
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
        if (inputMode != OverworldHeroInputMode.FollowCursor) return;
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
