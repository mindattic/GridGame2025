using UnityEngine;

public partial class OverworldHero
{
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

            // Look-ahead: stop before intersecting a wall
            if (WillHitWall(current, step)) { SetIdle(); return; }

            SetAnimationFromInput(dir, step.magnitude); // always drive animator

            Vector2 desired = ClampToMap(current + step);
            Vector2 next = ResolveCollision(current, desired);
            Vector2 frameDelta = next - current;

            SetPosition(next);
            bool moved = frameDelta.sqrMagnitude > 1e-6f;
            if (moved) OnHeroMoved?.Invoke(next);

            isMoving = false; _path = null; // ensure click path cancelled
        }
        else
        {
            SetIdle();
            isMoving = false; _path = null;
        }
    }
}
