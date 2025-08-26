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
    /// Conveyor timeline made of fixed-size blocks. The center indicator represents "now".
    /// Each block equals one turn: either Heroes or a single Enemy.
    /// Turn order is built from each enemy's TurnDelay:
    /// - If no enemy is ready (delay > 0), schedule a Hero block and tick down all delays by 1.
    /// - If one or more are ready (delay == 0), schedule them in order of Stats.Speed (desc).
    /// When a block completes, real delays are updated to match the forecast decision:
    /// - Hero block completion decrements every enemy's real TurnDelay by 1.
    /// - Enemy block completion sets that enemy's real TurnDelay to the pre-rolled next value.
    /// Hero blocks last heroSeconds (default 6). Enemy blocks use enemySeconds for the hold.
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

        [Header("Turn Durations")]
        [SerializeField] private float heroSeconds = 6f;
        [SerializeField] private float enemySeconds = 1.0f;

        [Header("Forecast Settings")]
        [Tooltip("How many blocks to keep visible ahead of the current one.")]
        [SerializeField] private int futureBlocks = 12;

        [Tooltip("New enemy delays are chosen in [minDelay, maxDelay] after each enemy acts.")]
        [SerializeField] private int minDelay = 3;

        [SerializeField] private int maxDelay = 10;

        [Header("Colors")]
        [SerializeField] private Color heroColor = Color.white;
        [SerializeField] private Color enemyTint = new Color(0.10f, 0.60f, 1f, 1f);

        // Runtime state
        private readonly List<TurnBlock> blocks = new List<TurnBlock>();
        private readonly List<TimelineBlockInstance> pool = new List<TimelineBlockInstance>();
        private readonly List<SimNode> sim = new List<SimNode>();

        private int currentIndex;
        private float holdRemaining;
        private float contentX;
        private float targetContentX;
        private bool running;

        // Model for a rendered turn block
        private struct TurnBlock
        {
            public bool isHero;
            public ActorInstance enemy;      // null for hero blocks
            public string label;
            public Color color;
            public Sprite portrait;          // enemy portrait only
            public int presetNextDelay;      // next delay to apply when this enemy finishes
        }

        // Forecast simulator node
        private sealed class SimNode
        {
            public ActorInstance enemy;
            public int delay;     // simulated delay
            public int speed;     // from Stats.Speed
            public Sprite portrait;
            public string label;
        }

        /// <summary>
        /// Find Viewport, Content, Indicator, center the indicator, ensure enemies have an initial TurnDelay,
        /// build forecast, lay out blocks, and snap to the first block.
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
            EnsureInitialEnemyDelays();

            BuildInitialForecast();
            LayoutAll();
            SnapToIndex(0);
            StartCurrentBlock();

            running = true;
        }

        /// <summary>
        /// Ensure every live enemy has a non-negative TurnDelay. If not, roll one in [minDelay, maxDelay].
        /// </summary>
        private void EnsureInitialEnemyDelays()
        {
            var enemies = g.Actors.Enemies.Where(e => e != null && e.IsPlaying).ToList();
            foreach (var e in enemies)
            {
                if (e.TurnDelay < 0)
                    e.SetInitialTurnDelay(minDelay, maxDelay);
            }
        }

        /// <summary>
        /// Builds the initial forecast from real enemy delays by filling the sim and then appending blocks.
        /// The simulator is kept in sync as we append and when real turns finish.
        /// </summary>
        private void BuildInitialForecast()
        {
            blocks.Clear();
            sim.Clear();

            var enemies = g.Actors.Enemies.Where(e => e != null && e.IsPlaying).ToList();
            foreach (var e in enemies)
            {
                sim.Add(new SimNode
                {
                    enemy = e,
                    delay = Mathf.Max(0, e.TurnDelay),
                    speed = e.Stats != null ? e.Stats.Agility.ToInt() : 0,
                    portrait = (e.Render != null && e.Render.thumbnail != null) ? e.Render.thumbnail.sprite : null,
                    label = e.name
                });
            }

            AppendUntilCount(futureBlocks);
            currentIndex = 0;
        }

        /// <summary>
        /// Appends forecast blocks using the current simulator state until we have at least desiredCount items.
        /// </summary>
        private void AppendUntilCount(int desiredCount)
        {
            // Defensive upper bound so we never infinite loop
            int safety = desiredCount + 128;

            while (blocks.Count < desiredCount && safety-- > 0)
            {
                // Who is ready now?
                var ready = sim.Where(s => s.delay == 0)
                               .OrderByDescending(s => s.speed)
                               .ToList();

                if (ready.Count == 0)
                {
                    // No enemy ready: schedule a Hero block and tick down everyone by 1
                    blocks.Add(new TurnBlock
                    {
                        isHero = true,
                        enemy = null,
                        label = "Heroes",
                        color = heroColor,
                        portrait = null,
                        presetNextDelay = -1
                    });

                    for (int i = 0; i < sim.Count; i++)
                        sim[i].delay = Mathf.Max(0, sim[i].delay - 1);
                }
                else
                {
                    // Schedule the fastest ready enemy and give it a new delay for after its action
                    var s = ready[0];

                    int next = UnityEngine.Random.Range(minDelay, maxDelay + 1);

                    blocks.Add(new TurnBlock
                    {
                        isHero = false,
                        enemy = s.enemy,
                        label = s.label,
                        color = enemyTint,
                        portrait = s.portrait,
                        presetNextDelay = next
                    });

                    // After scheduling, this enemy will get a fresh delay in the simulator
                    s.delay = next;
                }
            }
        }

        /// <summary>
        /// Begin holding on the current block and update TimerBar2D visibility if present.
        /// </summary>
        private void StartCurrentBlock()
        {
            if (blocks.Count == 0)
                return;

            var b = blocks[currentIndex];
            holdRemaining = b.isHero ? heroSeconds : enemySeconds;

            // Timer2D only during hero turns
            if (g.TimerBar2D != null)
            {
                if (b.isHero)
                {
                    g.TimerBar2D.SetDuration(heroSeconds);
                    g.TimerBar2D.ResetToFull();
                    g.TimerBar2D.Play();
                }
                else
                {
                    g.TimerBar2D.Pause();
                }
            }

            targetContentX = GetTargetXForIndex(currentIndex);
        }

        /// <summary>
        /// Complete the current block, apply its effect to real delays, advance, and extend forecast if needed.
        /// </summary>
        private void CompleteCurrentBlock()
        {
            if (blocks.Count == 0)
                return;

            var finished = blocks[currentIndex];

            // Apply to real delays so UI stays faithful to state
            if (finished.isHero)
            {
                var enemies = g.Actors.Enemies.Where(e => e != null && e.IsPlaying).ToList();
                foreach (var e in enemies)
                    e.DecrementTurnDelay(1);
            }
            else if (finished.enemy != null)
            {
                finished.enemy.ApplyNewTurnDelay(finished.presetNextDelay);
            }

            // Move to the next block
            currentIndex = Mathf.Min(currentIndex + 1, blocks.Count - 1);

            // Keep enough forecast ahead by continuing the sim
            if (blocks.Count - currentIndex < 4)
                AppendUntilCount(currentIndex + futureBlocks);

            // Relayout only newly added blocks if any were appended
            LayoutAll();

            // Begin holding on the new block
            StartCurrentBlock();
        }

        /// <summary>
        /// Lays out all blocks as fixed-size squares in a single horizontal strip.
        /// </summary>
        private void LayoutAll()
        {
            EnsurePool(blocks.Count);

            float height = viewport.rect.height > 0f ? viewport.rect.height : blockSizePixels;
            float size = Mathf.Round(Mathf.Clamp(blockSizePixels, 1f, height));
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
        /// Centers the indicator vertically and anchors it at the configured horizontal ratio.
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
        /// Instantiates pool objects as needed.
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
        /// Returns content X such that the center of the block at index is under the indicator.
        /// </summary>
        private float GetTargetXForIndex(int index)
        {
            float height = viewport.rect.height > 0f ? viewport.rect.height : blockSizePixels;
            float size = Mathf.Round(Mathf.Clamp(blockSizePixels, 1f, height));
            float start = index * (size + blockGapPixels);
            float centerOfBlock = start + size * 0.5f;
            float indicatorX = viewport.rect.width * indicatorViewportRatio;
            return indicatorX - centerOfBlock;
        }

        /// <summary>
        /// Immediately centers the current block under the indicator.
        /// </summary>
        private void SnapToIndex(int index)
        {
            targetContentX = GetTargetXForIndex(index);
            contentX = targetContentX;
            content.anchoredPosition = new Vector2(contentX, content.anchoredPosition.y);
        }

        /// <summary>
        /// Smoothly slides content toward the target position.
        /// </summary>
        private void SlideTowardTarget(float dt)
        {
            contentX = Mathf.Lerp(contentX, targetContentX, Mathf.Clamp01(dt * snapLerp));
            content.anchoredPosition = new Vector2(contentX, content.anchoredPosition.y);
        }

        /// <summary>
        /// External control: pause the conveyor.
        /// </summary>
        public void Pause() { running = false; }

        /// <summary>
        /// External control: resume the conveyor.
        /// </summary>
        public void Resume() { running = true; }

        private void Update()
        {
            if (!running || blocks.Count == 0)
                return;

            float dt = Time.deltaTime;
            holdRemaining -= dt;

            if (holdRemaining <= 0f)
                CompleteCurrentBlock();

            SlideTowardTarget(dt);
        }
    }
}
