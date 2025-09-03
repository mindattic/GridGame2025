using UnityEngine;

public partial class OverworldHero
{
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
        return collisionRadiusWorld;
    }

    // Collision-probe center: always use Animator/Sprite pivot plus optional feet offset.
    private Vector2 GetVisualCenter(Vector2 desiredWorldPosition)
    {
        // In 2D SpriteRenderer, transform.position == sprite pivot in world units.
        // We evaluate at the desired position passed in, not the current transform, to test feasibility.
        Vector2 pivotWorld = desiredWorldPosition;
        if (feetOffset != Vector2.zero)
        {
            // Convert local-space offset to world considering only rotation (scale assumed uniform for world-space sprites)
            Vector2 worldOffset = (Vector2)(transform.rotation * (Vector3)feetOffset);
            pivotWorld += worldOffset;
        }
        return pivotWorld;
    }

    // Try to compute a sliding target along the obstacle tangent; returns true if a slide position is valid
    private bool TrySlide(Vector2 current, Vector2 step, out Vector2 slideTarget)
    {
        slideTarget = current;
        if (collisionProvider == null) return false;
        if (step.sqrMagnitude <= 1e-10f) return false;

        Vector2 desired = ClampToMap(current + step);

        // Estimate obstacle normal near the desired position first, then fallback to current
        float sampleRadius = Mathf.Max(0.05f, GetProbeRadius() + navObstacleBuffer);
        Vector2 centerDesired = GetVisualCenter(desired);
        Vector2 n = collisionProvider.EstimateObstacleNormal(centerDesired, sampleRadius, 12);
        if (n == Vector2.zero)
        {
            Vector2 centerCurrent = GetVisualCenter(current);
            n = collisionProvider.EstimateObstacleNormal(centerCurrent, sampleRadius, 12);
        }
        if (n == Vector2.zero) return false;

        Vector2 dir = step.normalized;
        // Tangent: remove the component into the normal
        Vector2 tangent = dir - Vector2.Dot(dir, n) * n;
        if (tangent.sqrMagnitude < 1e-6f) return false;
        tangent.Normalize();

        float stepLen = step.magnitude;
        // Slide along tangent plus a tiny nudge away from wall to avoid re-penetration
        float nudge = Mathf.Max(0.001f, GetProbeRadius() * 0.15f);
        Vector2 candidate = ClampToMap(current + tangent * stepLen + n * nudge);

        if (IsWalkableWorld(candidate))
        {
            slideTarget = candidate;
            return true;
        }

        // Try without nudge if the nudged position was too aggressive
        Vector2 candidateNoNudge = ClampToMap(current + tangent * stepLen);
        if (IsWalkableWorld(candidateNoNudge))
        {
            slideTarget = candidateNoNudge;
            return true;
        }

        // Final small nudge along normal only, helps separate when stuck
        Vector2 candidateNudgeOnly = ClampToMap(current + n * nudge);
        if (IsWalkableWorld(candidateNudgeOnly))
        {
            slideTarget = candidateNudgeOnly;
            return true;
        }

        return false;
    }

    // Resolve collision by accepting desired if walkable, or sliding along obstacle if not
    private Vector2 ResolveCollision(Vector2 current, Vector2 desired)
    {
        if (IsWalkableWorld(desired))
            return desired;

        // Blocked: attempt to slide
        Vector2 step = desired - current;
        if (TrySlide(current, step, out var slide))
            return slide;

        // Blocked; stay put
        return current;
    }

    private bool WillHitWall(Vector2 current, Vector2 step)
    {
        if (step.sqrMagnitude <= 1e-10f) return false;

        // Desired hero pivot position after the step
        Vector2 desired = ClampToMap(current + step);

        // If fully walkable, nothing to block
        if (IsWalkableWorld(desired)) return false;

        // If no collision provider, fall back to conservative block
        if (collisionProvider == null) return true;

        // Compute percentage coverage of blocked samples on the forward semicircle
        float coverage = ForwardBlockedCoverage(desired, step, GetProbeRadius(), forwardCoverageSamples);

        // If coverage exceeds threshold, treat as wall UNLESS we can slide along it
        if (coverage >= forwardCoverageBlockThreshold)
        {
            // If a slide is possible, we shouldn't treat this as a hard wall block
            if (TrySlide(current, step, out _))
                return false;
            return true;
        }

        return false;
    }

    // Sample blocked ratio on the forward semicircle of the hero's probe at a desired position
    private float ForwardBlockedCoverage(Vector2 desiredPosition, Vector2 step, float radius, int samples)
    {
        // Use the collision-probe center (pivot + feet offset) at the desired world position
        Vector2 center = GetVisualCenter(desiredPosition);

        // If probe radius is non-positive, just test the center
        if (radius <= 0f || samples <= 0)
            return collisionProvider.IsWalkableLocal(center) ? 0f : 1f;

        Vector2 dir = step.sqrMagnitude > 1e-12f ? step.normalized : Vector2.zero;
        if (dir == Vector2.zero)
            return 0f;

        // Sample the forward semicircle: angles from -90 to +90 around 'dir'
        int blocked = 0;
        int total = Mathf.Max(1, samples);

        for (int i = 0; i < total; i++)
        {
            // Equal-spaced semicircle, centered on dir
            float t = (i + 0.5f) / total;              // 0..1
            float ang = (t - 0.5f) * Mathf.PI;         // -PI/2 .. +PI/2
            // Build a basis around dir: rotate dir by ang on the XY plane
            float s = Mathf.Sin(ang);
            float c = Mathf.Cos(ang);
            Vector2 rotated = new Vector2(
                dir.x * c - dir.y * s,
                dir.x * s + dir.y * c
            );

            Vector2 sample = center + rotated * radius;

            // Count blocked samples
            if (!collisionProvider.IsWalkableLocal(sample))
                blocked++;
        }

        return (float)blocked / total;
    }

}
