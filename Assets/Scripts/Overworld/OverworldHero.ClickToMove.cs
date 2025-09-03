using UnityEngine;
using System.Collections.Generic;

public partial class OverworldHero
{
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
            Vector2 step = dir * stepLen;

            // Look-ahead: stop before intersecting a wall
            if (WillHitWall(cur, step)) { SetIdle(); isMoving = false; return; }

            if (ShouldUseCast(step))
            {
                MoveWithCast(step);
            }
            else
            {
                Vector2 desiredNext = ClampToMap(cur + step);
                Vector2 nextPos = ResolveCollision(cur, desiredNext);
                Vector2 delta = nextPos - cur;

                bool moved = delta.sqrMagnitude > 1e-6f;
                if (moved)
                {
                    SetAnimation(delta);
                    SetPosition(nextPos);
                    OnHeroMoved?.Invoke(nextPos);
                }
                else
                {
                    _path = null; isMoving = false; SetIdle();
                }
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
        Vector2 step2 = stepDir * stepLen2;

        // Look-ahead: stop before intersecting a wall
        if (WillHitWall(cur, step2)) { SetIdle(); isMoving = false; return; }

        if (ShouldUseCast(step2))
        {
            MoveWithCast(step2);
        }
        else
        {
            Vector2 desiredNext2 = ClampToMap(cur + step2);
            Vector2 nextPos2 = ResolveCollision(cur, desiredNext2); Vector2 delta2 = nextPos2 - cur;

            bool moved2 = delta2.sqrMagnitude > 1e-6f;
            if (moved2)
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
    }

    // Click/tap entry from screen space (Content parameter ignored; kept for API compatibility)
    public void HandleClickScreen(Vector2 screenPos, UnityEngine.RectTransform _)
    {
        var cam = worldCamera != null ? worldCamera : Camera.main;
        Vector3 wp = Mode7CameraController.ScreenToWorldOnZPlane(cam, screenPos, transform.position.z);
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
        out List<Vector2> path)
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


}
