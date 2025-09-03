using UnityEngine;

public partial class OverworldHero
{
    // Predict a final stop position using a simple cast-based approach; fixed step for determinism
    private Vector2 PredictStop(Vector2 start, Vector2 target)
    {
        Vector2 cur = start;
        const int maxIters = 128;
        float stepLen = Mathf.Max(1f, moveSpeed / 60f); // ~60Hz equivalent

        for (int i = 0; i < maxIters; i++)
        {
            Vector2 toTarget = target - cur;
            float dist = toTarget.magnitude;
            if (dist <= snapThreshold) return target;

            Vector2 dir = dist > 1e-6f ? (toTarget / dist) : Vector2.zero;
            float len = Mathf.Min(stepLen, dist);
            Vector2 next = cur;
            PlanCastMove(ref next, dir, len);
            if ((next - cur).sqrMagnitude <= 1e-6f)
                return cur; // blocked before target
            cur = next;
        }
        return cur;
    }

    // Performs a cast and updates nextPos with the planned position (with slide)
    private void PlanCastMove(ref Vector2 nextPos, Vector2 dir, float distance)
    {
        if (distance <= 1e-6f || dir.sqrMagnitude <= 1e-12f) return;
        dir = dir.normalized;

        // Ensure casts originate from current rb position
        Vector2 origin = rb.position;
        int hitCount = rb.Cast(dir, contactFilter, hitBuffer, distance + skin);
        if (hitCount == 0)
        {
            origin += dir * distance;
            nextPos = origin;
            return;
        }

        float closest = Mathf.Infinity;
        int closestIndex = -1;
        bool anyOverlap = false;
        Vector2 overlapNormalSum = Vector2.zero;
        for (int h = 0; h < hitCount; h++)
        {
            float d = hitBuffer[h].distance;
            if (d >= 0f)
            {
                if (d < closest) { closest = d; closestIndex = h; }
            }
            else
            {
                anyOverlap = true;
                overlapNormalSum += hitBuffer[h].normal;
            }
        }

        if (closestIndex < 0)
        {
            // Fully overlapping at origin: nudge outward to depenetrate
            Vector2 push;
            if (anyOverlap && overlapNormalSum.sqrMagnitude > 1e-6f)
                push = overlapNormalSum.normalized * Mathf.Max(skin, 0.02f);
            else
                push = dir * Mathf.Max(skin, 0.02f); // no reliable normal; escape along intent
            origin += push;
            nextPos = origin;
            return;
        }

        float allowed = Mathf.Max(0f, closest - skin);
        origin += dir * allowed;

        // Slide remaining along surface with a few iterations to avoid sticking
        float remain = Mathf.Max(0f, distance - allowed);
        if (remain <= 1e-6f) { nextPos = origin; return; }

        Vector2 moveDir = dir;
        int iterations = Mathf.Max(1, maxSlideIterations);
        for (int i = 0; i < iterations && remain > 1e-6f; i++)
        {
            // Use the normal from the initial blocking hit for first slide
            Vector2 n = hitBuffer[closestIndex].normal;
            Vector2 remainingVec = moveDir * remain;
            Vector2 slide = remainingVec - Vector2.Dot(remainingVec, n) * n; // tangent component
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
                    float d = hitBuffer[h].distance;
                    if (d >= 0f && d < closestSlide) { closestSlide = d; slideHitIndex = h; }
                }

                float allowSlide = Mathf.Max(0f, closestSlide - skin);
                if (allowSlide > 1e-6f)
                {
                    origin += sDir * allowSlide;
                    remain = Mathf.Max(0f, remain - allowSlide);
                    // update moveDir to keep sliding along last direction
                    moveDir = sDir;
                    if (slideHitIndex >= 0) closestIndex = slideHitIndex;
                }
                else
                {
                    // tiny progress or corner pinch; apply micro-nudge along tangent and stop
                    origin += sDir * Mathf.Max(0.001f, skin * 0.5f);
                    break;
                }
            }
        }

        nextPos = origin;


        // Clamp inside map bounds after planning
        nextPos = ClampToMap(nextPos);
    }

    // Chooses cast-and-slide; applies position and event
    private void MoveWithCast(Vector2 displacement)
    {
        if (displacement.sqrMagnitude <= 1e-10f) return;

        // If collision is disabled, move freely (still clamp to map)
        if (!enableCollision)
        {
            Vector2 freeNext = ClampToMap(GetPosition() + displacement);
            SetPosition(freeNext);
            OnHeroMoved?.Invoke(freeNext);
            return;
        }

        // Read from Rigidbody2D when present for authoritative pose
        Vector2 currentPos = GetPosition();
        Vector2 nextPos = currentPos;
        PlanCastMove(ref nextPos, displacement.normalized, displacement.magnitude);
        if ((nextPos - currentPos).sqrMagnitude > 1e-8f)
        {
            SetPosition(nextPos);
            OnHeroMoved?.Invoke(nextPos);
        }
    }
}
