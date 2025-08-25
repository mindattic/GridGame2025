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
    /// Timeline shows a row of fixed-size blocks that represent turn order.
    /// The center indicator is "now". Each hero block is 6 seconds, enemies
    /// get their own blocks. Order is simulated from enemy AP fill rates.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Timeline : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private TimelineBlockInstance blockPrefab;

        private RectTransform viewport;
        private RectTransform content;
        private Image indicator;

        [Header("Layout")]
        [SerializeField] private float blockSizePixels = 96f;
        [SerializeField] private float blockGapPixels = 6f;
        [SerializeField, Range(0f, 1f)] private float indicatorViewportRatio = 0.5f;
        [SerializeField] private float snapLerp = 12f;

        [Header("Turns")]
        [SerializeField] private float heroSeconds = 6f;
        [SerializeField] private Color enemyTint = new Color(0.10f, 0.60f, 1f, 1f);

        [Header("AP Simulation")]
        [Tooltip("AP gained per second per point of Intelligence used by the simulator to predict order.")]
        [SerializeField] private float apGainPerIntPerSecond = 0.10f;

        [Header("Queue Size")]
        [Tooltip("How many blocks to keep scheduled ahead of the current one.")]
        [SerializeField] private int futureBlocks = 10;

        public void Pause() { running = false; }
        public void Resume() { running = true; }


        private readonly List<TurnBlock> blocks = new List<TurnBlock>();
        private readonly List<TimelineBlockInstance> pool = new List<TimelineBlockInstance>();
        private readonly List<EnemySim> sim = new List<EnemySim>();

        private int currentIndex;
        private float elapsedInBlock;
        private float contentX;
        private float targetContentX;
        private bool running;

        private struct TurnBlock
        {
            public bool isHero;
            public ActorInstance enemy;
            public float seconds;
            public Sprite portrait;
            public string label;
            public Color color;
        }

        private sealed class EnemySim
        {
            public ActorInstance enemy;
            public float ap;
            public float max;
            public float gainPerSecond;
        }

        /// <summary>
        /// Finds Viewport, Content, Indicator, centers the indicator, then builds and snaps.
        /// </summary>
        public void Initialize()
        {
            viewport = transform.Find("Viewport")?.GetComponent<RectTransform>();
            if (viewport == null) { Debug.LogError("Timeline: Viewport not found."); return; }

            content = viewport.Find("Content")?.GetComponent<RectTransform>();
            if (content == null) { Debug.LogError("Timeline: Content not found."); return; }

            indicator = viewport.Find("Indicator")?.GetComponent<Image>();
            if (indicator == null) { Debug.LogError("Timeline: Indicator not found."); return; }

            if (blockPrefab == null) { Debug.LogError("Timeline: Block prefab is not assigned."); return; }

            CenterIndicator();
            BuildSimulationFromScene();
            BuildQueue(futureBlocks);
            LayoutAll();
            SnapToCurrent();
            running = true;
        }

        /// <summary>
        /// Rebuilds the enemy AP simulation from live actors.
        /// </summary>
        private void BuildSimulationFromScene()
        {
            sim.Clear();

            var enemies = g.Actors.Enemies
                .Where(a => a != null && a.IsPlaying)
                .ToList();

            foreach (var e in enemies)
            {
                var s = new EnemySim
                {
                    enemy = e,
                    ap = e.Stats.AP,
                    max = e.Stats.MaxAP,
                    gainPerSecond = Mathf.Max(0.001f, e.Stats.Intelligence * apGainPerIntPerSecond)
                };
                sim.Add(s);
            }
        }

        /// <summary>
        /// Builds or extends the queue using the current simulator state.
        /// Hero squares are inserted whenever no enemy is ready.
        /// During a hero square, all enemies accrue AP.
        /// When an enemy becomes ready, that enemy gets the next square and its AP resets.
        /// </summary>
        private void BuildQueue(int neededAhead)
        {
            if (blocks.Count == 0)
            {
                currentIndex = 0;
                elapsedInBlock = 0f;
            }

            int need = Mathf.Max(0, (currentIndex + neededAhead) - (blocks.Count - 1));
            while (need > 0)
            {
                // Any enemy already ready?
                var ready = sim
                    .Where(s => s.ap >= s.max)
                    .OrderByDescending(s => s.ap)
                    .FirstOrDefault();

                if (ready != null)
                {
                    // Schedule this enemy and reset its AP in the simulator.
                    blocks.Add(new TurnBlock
                    {
                        isHero = false,
                        enemy = ready.enemy,
                        seconds = Mathf.Clamp(ready.enemy.EstimateTurnSeconds(), 0.5f, 6f),
                        portrait = (ready.enemy.Render != null && ready.enemy.Render.thumbnail != null)
                                   ? ready.enemy.Render.thumbnail.sprite
                                   : null,
                        label = ready.enemy.name,
                        color = enemyTint
                    });

                    ready.ap = 0f;
                    need--;
                    // During enemy turn we do not accrue AP in the simulator.
                    continue;
                }

                // No enemy is ready; add a hero square and accrue AP for its duration.
                blocks.Add(new TurnBlock
                {
                    isHero = true,
                    enemy = null,
                    seconds = heroSeconds,
                    portrait = null,
                    label = "Heroes",
                    color = Color.white
                });

                // Enemies gain AP while the hero square plays.
                float dt = heroSeconds;
                for (int i = 0; i < sim.Count; i++)
                {
                    var s = sim[i];
                    s.ap = Mathf.Min(s.max, s.ap + s.gainPerSecond * dt);
                }

                need--;
            }
        }

        /// <summary>
        /// Creates or reuses pooled visuals and places them in a horizontal strip.
        /// Squares are uniform width and height.
        /// </summary>
        private void LayoutAll()
        {
            EnsurePool(blocks.Count);

            float height = viewport.rect.height > 0f ? viewport.rect.height : blockSizePixels;
            float size = Mathf.Max(1f, Mathf.Min(blockSizePixels, height)); // Square
            float x = 0f;

            for (int i = 0; i < blocks.Count; i++)
            {
                var b = blocks[i];
                var ui = pool[i];

                ui.gameObject.SetActive(true);
                ui.transform.SetParent(content, false);
                ui.SetSize(size, size);
                ui.SetStyle(b.isHero, b.color);
                ui.SetLabel(b.label);

                if (!b.isHero && b.portrait != null)
                    ui.SetPortrait(b.portrait, true);
                else
                    ui.SetPortrait(null, false);

                ui.Rect.anchoredPosition = new Vector2(x, Mathf.Round((height - size) * 0.5f));
                x += size + blockGapPixels;
            }

            for (int i = blocks.Count; i < pool.Count; i++)
                pool[i].gameObject.SetActive(false);
        }

        /// <summary>
        /// Aligns the current block so its center sits under the indicator.
        /// </summary>
        private void SnapToCurrent()
        {
            targetContentX = GetTargetXForIndex(currentIndex);
            contentX = targetContentX;
            content.anchoredPosition = new Vector2(contentX, content.anchoredPosition.y);
        }

        /// <summary>
        /// Smoothly slides content toward the current target.
        /// </summary>
        private void SlideTowardTarget(float dt)
        {
            contentX = Mathf.Lerp(contentX, targetContentX, Mathf.Clamp01(dt * snapLerp));
            content.anchoredPosition = new Vector2(contentX, content.anchoredPosition.y);
        }

        /// <summary>
        /// Returns the content X position needed to center the given index under the indicator.
        /// </summary>
        private float GetTargetXForIndex(int index)
        {
            float height = viewport.rect.height > 0f ? viewport.rect.height : blockSizePixels;
            float size = Mathf.Max(1f, Mathf.Min(blockSizePixels, height));
            float start = index * (size + blockGapPixels);
            float centerOfBlock = start + size * 0.5f;
            float indicatorX = viewport.rect.width * indicatorViewportRatio;
            return indicatorX - centerOfBlock;
        }

        /// <summary>
        /// Ensures the visual pool has at least count instances.
        /// </summary>
        private void EnsurePool(int count)
        {
            while (pool.Count < count)
            {
                var inst = Instantiate(blockPrefab, content);
                inst.gameObject.name = $"TimelineBlock_{pool.Count:D2}";
                pool.Add(inst);
            }
        }

        /// <summary>
        /// Centers the indicator anchors.
        /// </summary>
        private void CenterIndicator()
        {
            var rt = indicator.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);
        }

        /// <summary>
        /// Advances the timeline clock and switches blocks when the active block finishes.
        /// Extends the queue as you approach the end so the belt never runs out.
        /// </summary>
        private void Tick(float dt)
        {
            if (!running || blocks.Count == 0)
                return;

            elapsedInBlock += Mathf.Max(0f, dt);

            var current = blocks[currentIndex];
            if (elapsedInBlock >= current.seconds)
            {
                elapsedInBlock = 0f;
                currentIndex = Mathf.Min(currentIndex + 1, blocks.Count - 1);

                // If we are getting close to the end, extend using the same simulator state.
                if (currentIndex >= blocks.Count - 3)
                {
                    BuildQueue(futureBlocks);
                    LayoutAll();
                }

                targetContentX = GetTargetXForIndex(currentIndex);
            }

            SlideTowardTarget(dt);
        }

        /// <summary>
        /// Useful if something changed the actors list at runtime.
        /// Rebuilds the simulator and the queue from the present moment.
        /// </summary>
        public void RecomputeFromScene()
        {
            blocks.Clear();
            BuildSimulationFromScene();
            BuildQueue(futureBlocks);
            LayoutAll();
            currentIndex = 0;
            elapsedInBlock = 0f;
            SnapToCurrent();
        }

        /// <summary>
        /// External control: jump back to the first block and pause.
        /// </summary>
        public void ResetAndPause()
        {
            currentIndex = 0;
            elapsedInBlock = 0f;
            running = false;
            SnapToCurrent();
        }

        private void Update()
        {
            if (running)
                Tick(Time.deltaTime);
        }

        // Put this inside the Timeline class
        public void StartHeroTurnNow()
        {
            // Ensure we have a sim and some blocks to work with
            if (sim.Count == 0)
                BuildSimulationFromScene();
            if (blocks.Count == 0)
            {
                BuildQueue(futureBlocks);
                LayoutAll();
                currentIndex = 0;
            }

            // Create a hero block for "now"
            var hero = new TurnBlock
            {
                isHero = true,
                enemy = null,
                seconds = Mathf.Max(0.01f, heroSeconds),
                portrait = null,
                label = "Heroes",
                color = Color.white
            };

            // If current is already a hero block, refresh its duration; otherwise insert one at the current index
            if (currentIndex < blocks.Count && blocks[currentIndex].isHero)
            {
                blocks[currentIndex] = hero;
            }
            else
            {
                blocks.Insert(currentIndex, hero);
            }

            // While the hero turn is scheduled, enemies accrue AP in the simulator
            for (int i = 0; i < sim.Count; i++)
            {
                var s = sim[i];
                s.ap = Mathf.Min(s.max, s.ap + s.gainPerSecond * heroSeconds);
            }

            // Make sure we have enough future blocks, then relayout
            BuildQueue(futureBlocks);
            LayoutAll();

            // Reset timer for this block and center it under the indicator
            elapsedInBlock = 0f;
            targetContentX = GetTargetXForIndex(currentIndex);
            contentX = targetContentX;
            content.anchoredPosition = new Vector2(contentX, content.anchoredPosition.y);

            // Ensure the conveyor is running
            running = true;
        }




    }
}
