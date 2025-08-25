using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Canvas.Timeline
{
    /// <summary>
    /// Renders a horizontal schedule where the center indicator always represents "now".
    /// Builds a 6 second hero window followed by enemy turns ordered by AP.
    /// The content is positioned every frame so that elapsed time aligns beneath the center.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Timeline : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private TimelineBlockInstance blockPrefab;

        private RectTransform viewport;
        private RectTransform content;
        private Image indicator;

        [Header("Config")]
        [SerializeField] private float heroWindowSeconds = 6f;
        [SerializeField, Range(0f, 1f)] private float indicatorViewportRatio = 0.5f;
        [SerializeField] private float pixelsPerSecond = 160f;
        [SerializeField] private float blockGapPixels = 4f;

        private readonly List<TimelineItem> plan = new List<TimelineItem>();
        private readonly List<TimelineBlockInstance> pool = new List<TimelineBlockInstance>();

        private float elapsedInCurrent;
        private bool isRunning;
        private float contentX;

        private struct TimelineItem
        {
            public bool isHero;
            public ActorInstance actor;
            public float seconds;
            public Sprite portrait;
            public Color color;
            public string label;
        }

        /// <summary>
        /// Finds Viewport, Content, and Indicator under this object, centers the indicator,
        /// then builds the plan and snaps the timeline so time 0 is under the center.
        /// </summary>
        public void Initialize()
        {
            viewport = transform.Find("Viewport")?.GetComponent<RectTransform>();
            if (viewport == null)
            {
                Debug.LogError("TimelineManager: Viewport not found under TimelineRoot.");
                return;
            }

            content = viewport.Find("Content")?.GetComponent<RectTransform>();
            if (content == null)
            {
                Debug.LogError("TimelineManager: Content not found under Viewport.");
                return;
            }

            indicator = viewport.Find("Indicator")?.GetComponent<Image>();
            if (indicator == null)
            {
                Debug.LogError("TimelineManager: Indicator not found under Viewport.");
                return;
            }

            if (blockPrefab == null)
            {
                Debug.LogError("TimelineManager: Block Prefab is not assigned.");
                return;
            }

            CenterIndicator();
            Rebuild();
            PositionAtTime(0f);
        }

        /// <summary>
        /// Rebuilds the timeline sequence: hero window then enemies by AP.
        /// If no enemy is at max AP, adds another hero window.
        /// After layout, positions content so the current elapsed time remains centered.
        /// </summary>
        public void Rebuild()
        {
            plan.Clear();

            plan.Add(new TimelineItem
            {
                isHero = true,
                actor = null,
                seconds = Mathf.Max(0.1f, heroWindowSeconds),
                portrait = null,
                color = Color.white,
                label = "Heroes"
            });

            var enemies = g.Actors.Enemies
                .Where(a => a != null && a.IsPlaying)
                .OrderByDescending(a => a.Stats.AP)
                .ToList();

            foreach (var e in enemies)
            {
                float estimate = Mathf.Max(0.25f, e.EstimateTurnSeconds());
                plan.Add(new TimelineItem
                {
                    isHero = false,
                    actor = e,
                    seconds = estimate,
                    portrait = (e.Render != null && e.Render.thumbnail != null) ? e.Render.thumbnail.sprite : null,
                    color = new Color(0.1f, 0.6f, 1f, 1f),
                    label = e.name
                });
            }

            if (!enemies.Any(a => a.HasMaxAP))
            {
                plan.Add(new TimelineItem
                {
                    isHero = true,
                    actor = null,
                    seconds = Mathf.Max(0.1f, heroWindowSeconds),
                    portrait = null,
                    color = Color.white,
                    label = "Heroes"
                });
            }

            elapsedInCurrent = Mathf.Clamp(elapsedInCurrent, 0f, GetTotalSeconds(plan));
            LayoutBlocks();
            PositionAtTime(elapsedInCurrent);
        }

        /// <summary>
        /// Starts a new hero window and holds at t=0 under the center until the player acts.
        /// </summary>
        public void StartHeroTurnWindow()
        {
            elapsedInCurrent = 0f;
            isRunning = false;
            Rebuild();
            PositionAtTime(0f);
        }

        /// <summary>
        /// Toggles whether the timeline advances. Call true when the hero is actively acting,
        /// false when idle or after action completes.
        /// </summary>
        public void HeroActionActive(bool active)
        {
            isRunning = active;
        }

        /// <summary>
        /// Updates the indicator alignment ratio and relayouts.
        /// </summary>
        public void SetIndicatorRatio(float ratio)
        {
            indicatorViewportRatio = Mathf.Clamp01(ratio);
            CenterIndicator();
            PositionAtTime(elapsedInCurrent);
        }

        /// <summary>
        /// Updates hero window seconds and relayouts.
        /// </summary>
        public void SetHeroWindowSeconds(float seconds)
        {
            heroWindowSeconds = Mathf.Max(0.1f, seconds);
            Rebuild();
        }

        /// <summary>
        /// Updates pixels per second and relayouts.
        /// </summary>
        public void SetPixelsPerSecond(float pxPerSec)
        {
            pixelsPerSecond = Mathf.Max(4f, pxPerSec);
            LayoutBlocks();
            PositionAtTime(elapsedInCurrent);
        }

        /// <summary>
        /// Advances time and keeps the current time centered under the indicator.
        /// When the plan completes, rebuilds and restarts at t=0.
        /// </summary>
        private void Tick(float dt)
        {
            if (!isRunning || plan.Count == 0)
                return;

            elapsedInCurrent += Mathf.Max(0f, dt);
            float total = GetTotalSeconds(plan);

            if (elapsedInCurrent >= total)
            {
                elapsedInCurrent = 0f;
                Rebuild();
                isRunning = true;
            }

            PositionAtTime(elapsedInCurrent);
        }

        /// <summary>
        /// Creates or reuses pooled UI blocks and places them left to right based on duration.
        /// </summary>
        private void LayoutBlocks()
        {
            if (viewport == null || content == null)
                return;

            EnsurePoolSize(plan.Count);

            if (pool.Count < plan.Count)
            {
                Debug.LogError($"TimelineManager: Pool size {pool.Count} is less than plan size {plan.Count}.");
                return;
            }

            float x = 0f;
            float height = viewport.rect.height > 0f ? viewport.rect.height : 120f;

            for (int i = 0; i < plan.Count; i++)
            {
                var item = plan[i];
                var ui = pool[i];
                float width = item.seconds * pixelsPerSecond;

                ui.gameObject.SetActive(true);
                ui.transform.SetParent(content, false);
                ui.SetSize(width, height);
                ui.SetStyle(item.isHero, item.color);
                ui.SetLabel(item.label);

                if (!item.isHero && item.portrait != null)
                    ui.SetPortrait(item.portrait, true);
                else
                    ui.SetPortrait(null, false);

                ui.Rect.anchoredPosition = new Vector2(x, 0f);
                x += width + blockGapPixels;
            }

            for (int i = plan.Count; i < pool.Count; i++)
                pool[i].gameObject.SetActive(false);
        }

        /// <summary>
        /// Ensures the pool has at least count instances ready to use.
        /// </summary>
        private void EnsurePoolSize(int count)
        {
            if (blockPrefab == null)
            {
                Debug.LogError("TimelineManager: Block Prefab is null. Assign it in the inspector.");
                return;
            }

            while (pool.Count < count)
            {
                var inst = Instantiate(blockPrefab, content);
                inst.gameObject.name = $"TimelineBlock_{pool.Count:D2}";
                pool.Add(inst);
            }
        }

        /// <summary>
        /// Places content so that the provided time (in seconds from plan start)
        /// aligns under the indicator at the center ratio.
        /// </summary>
        private void PositionAtTime(float secondsFromPlanStart)
        {
            if (viewport == null || content == null)
                return;

            float pxAtTime = secondsFromPlanStart * pixelsPerSecond;
            float indicatorX = viewport.rect.width * indicatorViewportRatio;
            contentX = indicatorX - pxAtTime;
            content.anchoredPosition = new Vector2(contentX, content.anchoredPosition.y);
        }

        /// <summary>
        /// Centers the indicator by anchors and updates its horizontal position based on ratio.
        /// </summary>
        private void CenterIndicator()
        {
            if (indicator == null || viewport == null)
                return;

            var rt = indicator.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);
        }

        /// <summary>
        /// Sums the seconds in the plan.
        /// </summary>
        private static float GetTotalSeconds(List<TimelineItem> items)
        {
            float s = 0f;
            for (int i = 0; i < items.Count; i++)
                s += items[i].seconds;
            return s;
        }

        /// <summary>
        /// Unity Update loop. Advances time while running and keeps "now" centered.
        /// </summary>
        private void Update()
        {
            if (isRunning)
                Tick(Time.deltaTime);
        }
    }
}
