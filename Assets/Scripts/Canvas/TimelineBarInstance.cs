using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Canvas
{
    [DisallowMultipleComponent]
    public sealed class TimelineBarInstance : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private RectTransform barRect; // horizontal line rect (width drives duration)
        [SerializeField] private RectTransform spawnRect; // right spawn point inside bar (optional)
        [SerializeField] private RectTransform leftLine; // left line target (x position)
        [SerializeField] private RectTransform tagsRoot; // parent for tags
        [SerializeField] private TimelineTag tagPrefab;

        [Header("Tuning")]
        [Tooltip("Baseline normalized units per second for a tag with Speed=1 (1.0 would cross bar in1s).")]
        [SerializeField] private float baseUnitsPerSec =0.08f; // tune gameplay here
        [Tooltip("Additional normalized units/sec added per Speed point.")]
        [SerializeField] private float perSpeedUnitsPerSec =0.02f;
        [Tooltip("Vertical spacing between multiple tags on the same x in local pixels.")]
        [SerializeField] private float tagRowHeight =14f;
        [SerializeField] private bool debugLogs = false;

        private readonly List<TimelineTag> active = new List<TimelineTag>();
        private bool advancing;

        private float cachedLeft;
        private float cachedSpawn;
        private bool layoutReady;

        private void Awake()
        {
            if (barRect == null) barRect = GetComponent<RectTransform>();
            if (tagsRoot == null && barRect != null)
            {
                var go = new GameObject("Tags", typeof(RectTransform));
                tagsRoot = go.GetComponent<RectTransform>();
                tagsRoot.SetParent(barRect, false);
                tagsRoot.anchorMin = new Vector2(0f,0.5f);
                tagsRoot.anchorMax = new Vector2(0f,0.5f);
                tagsRoot.pivot = new Vector2(0f,0.5f);
            }
            cachedLeft = float.NaN; cachedSpawn = float.NaN;
        }

        private void Start()
        {
            StartCoroutine(EnsureLayoutThenReposition());
            PauseAll();
        }

        private System.Collections.IEnumerator EnsureLayoutThenReposition()
        {
            for (int i =0; i <2; i++) yield return null;
            if (barRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(barRect);
            layoutReady = true;
            UpdateAllEndpoints();
            RecomputeAndRepositionIfNeeded();
            if (debugLogs) Debug.Log($"[TimelineBar] LayoutReady left={LeftEdgeX:F1} spawn={SpawnX:F1} width={barRect.rect.width:F1}");
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateAllEndpoints();
            RecomputeAndRepositionIfNeeded();
        }

        // Convert a child RectTransform world position to this bar's anchored X (0 = left edge of bar)
        private float AnchoredXFromWorld(RectTransform rt)
        {
            if (barRect == null || rt == null) return 0f;
            Vector3 local = barRect.InverseTransformPoint(rt.position); // local centered at barRect pivot
            float pivotOffset = barRect.rect.width * barRect.pivot.x; // shift to left-edge origin
            return local.x + pivotOffset;
        }

        // Important: left edge for tags is the bar's anchored X =0 (leftLine is visual only)
        private float LeftEdgeX =>0f;
        private float SpawnX => spawnRect != null ? AnchoredXFromWorld(spawnRect) : (barRect != null ? barRect.rect.width :300f);
        private float Width => Mathf.Max(1f, SpawnX - LeftEdgeX);

        private float UnitsPerSecFromSpeed(int speed)
        {
            return Mathf.Max(0.001f, baseUnitsPerSec + perSpeedUnitsPerSec * Mathf.Max(0, speed));
        }

        private IEnumerable<ActorInstance> SortedEnemiesBySpeedDesc()
        {
            return g.Actors.Enemies.Where(e => e != null && e.IsPlaying).OrderByDescending(e => e.Stats.Speed.ToInt());
        }

        public void Clear()
        {
            for (int i = active.Count -1; i >=0; i--) if (active[i] != null) Destroy(active[i].gameObject);
            active.Clear();
        }

        /// <summary>
        /// Ensure all currently playing enemies have a tag.
        /// If there are no tags yet, distribute initial positions by speed (right=fastest).
        /// Otherwise, only add missing ones at the far right.
        /// Also prunes tags whose owners are gone/inactive.
        /// </summary>
        public void EnsureTagsForAllEnemies(bool redistributeIfNone = true)
        {
            // Remove stale tags (dead or despawned)
            for (int i = active.Count -1; i >=0; i--)
            {
                var t = active[i];
                if (t == null || t.Owner == null || !t.Owner.IsPlaying)
                {
                    if (t != null) t.FadeAndDestroy(0.15f);
                    active.RemoveAt(i);
                }
            }

            var playing = g.Actors.Enemies.Where(e => e != null && e.IsPlaying).ToList();
            if (playing.Count ==0)
            {
                return;
            }

            // Add missing tags
            var missing = playing.Where(e => !active.Any(t => t != null && t.Owner == e)).ToList();

            if (active.Count ==0 && redistributeIfNone)
            {
                // Distribute along [0.1..1.0] by speed ordering
                var ordered = playing.OrderByDescending(e => e.Stats.Speed.ToInt()).ToList();
                int n = ordered.Count;
                for (int i =0; i < n; i++)
                {
                    var enemy = ordered[i];
                    float startU = n >1 ? Mathf.Lerp(1f,0.1f, i / Mathf.Max(1f, n -1f)) :1f;
                    SpawnTag(enemy, startU);
                }
            }
            else
            {
                // Only add new ones at the far right
                foreach (var enemy in missing)
                {
                    SpawnTag(enemy,1f);
                }
            }

            if (!layoutReady) StartCoroutine(EnsureLayoutThenReposition()); else { UpdateAllEndpoints(); RecomputeAndRepositionIfNeeded(); }
            // Timeline is controlled externally; default to paused until hero actually moves
            PauseAll();
        }

        public void SpawnInitialForAllEnemies()
        {
            // Backward-compat: simply ensure tags exist and distribute if empty
            EnsureTagsForAllEnemies(true);
        }

        private void PauseAll()
        { foreach (var t in active) t?.Pause(); advancing = false; }
        private void ResumeAll()
        { foreach (var t in active) t?.Resume(); advancing = true; }

        public void OnHeroStartMove() { RecomputeAndRepositionIfNeeded(); ResumeAll(); }
        public void OnHeroStopMove() { PauseAll(); }
        public void OnEnemyTurnStarted(ActorInstance enemy) { PauseAll(); }
        public void OnEnemyTurnFinished(ActorInstance enemy)
        {
            var tag = active.FirstOrDefault(t => t != null && t.Owner == enemy);
            if (tag != null)
            {
                // Ensure endpoints are current, then snap to far right and pause
                UpdateAllEndpoints();
                tag.SetU(1f);
                tag.Pause();
            }
        }

        private void OnTagReachedLeft(TimelineTag tag)
        {
            if (tag == null) return;
            g.TimerBar?.ForceComplete();
            g.InputManager.InputMode = InputMode.None;
            g.TurnManager.QueueEnemyAfterHero(tag.Owner);
            g.SelectionManager.Drop();
            PauseAll();
        }

        public float GetSecondsUntilNextEnemyReachesLeft()
        {
            if (active.Count ==0) return 0f;
            float min = float.PositiveInfinity;
            foreach (var t in active)
            {
                if (t == null || t.Owner == null || !t.Owner.IsPlaying) continue;
                float sec = t.GetSecondsRemaining();
                if (sec < min) min = sec;
            }
            return float.IsInfinity(min) ? 0f : Mathf.Max(0f, min);
        }

        private void SpawnTag(ActorInstance enemy, float startU)
        {
            if (enemy == null || !enemy.IsEnemy) return;
            if (tagPrefab == null) { Debug.LogError("TimelineBarInstance: tagPrefab not set."); return; }
            var parent = tagsRoot != null ? tagsRoot : barRect;
            var tag = Instantiate(tagPrefab, parent, false);
            tag.name = $"TimelineTag_{enemy.name}";
            int dup = active.Count(a => a != null && a.Owner == enemy);
            var tr = tag.GetComponent<RectTransform>();
            tr.anchorMin = tr.anchorMax = new Vector2(0f,0.5f);
            tr.pivot = new Vector2(0f,0.5f); // left-edge pivot for precise X
            tr.anchoredPosition = new Vector2(Mathf.Lerp(LeftEdgeX, SpawnX, startU), -dup * tagRowHeight);
            float uSpeed = UnitsPerSecFromSpeed(enemy.Stats.Speed.ToInt());
            tag.InitializeNormalized(enemy, LeftEdgeX, SpawnX, startU, uSpeed, OnTagReachedLeft);
            active.Add(tag);
        }

        private void UpdateAllEndpoints()
        {
            float left = LeftEdgeX; float spawn = SpawnX;
            foreach (var t in active) t?.UpdateEndpoints(left, spawn);
        }

        private void RecomputeAndRepositionIfNeeded()
        {
            float left = LeftEdgeX; float spawn = SpawnX;
            if (float.IsNaN(cachedLeft) || float.IsNaN(cachedSpawn) || !Mathf.Approximately(left, cachedLeft) || !Mathf.Approximately(spawn, cachedSpawn))
            {
                cachedLeft = left; cachedSpawn = spawn;
                foreach (var t in active)
                {
                    if (t == null || t.Rect == null) continue;
                    t.UpdateEndpoints(left, spawn);
                    var p = t.Rect.anchoredPosition;
                    if (p.x <= left) t.SetU(1f);
                }
            }
        }
    }
}
