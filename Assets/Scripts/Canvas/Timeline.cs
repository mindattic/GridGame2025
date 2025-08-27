// --- File: Assets/Scripts/Canvas/Timeline.cs ---
using Assets.Helper;
using Assets.Scripts.Canvas.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Helper.GameObjectHelper.Game;
using static Intermission.Before;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Timeline is the single source of truth for turn order.
/// Heroes use average agility for cadence.
/// Enemies use their own agility for cadence.
/// Exactly one-block advance per completed turn.
/// Forward-only forecast that is always extended so you never see the end.
/// The first block is always a Hero block.
/// </summary>
public sealed class Timeline : MonoBehaviour
{
    private RectTransform viewport;
    private RectTransform content;  
    private Image indicator;
    private TimelineBlockInstance blockPrefab;

    [Header("Layout")]
    [SerializeField] private float blockSize = 96f;     // square block edge
    [SerializeField] private float blockGutter = 8f;    // space between blocks

    [Header("Slide")]
    [SerializeField] private float slideBase = 60f;     // px per sec baseline
    [SerializeField] private float slideScale = 12f;    // px per sec scaled by distance
    [SerializeField] private float snapEpsilon = 0.5f;  // px; only used at init

    [Header("Forecast")]
    [SerializeField] private int forecastVisibleAhead = 32;
    [SerializeField] private int trimLeftKeep = 24;

    [Header("Agility Tuning")]
    [SerializeField] private float heroBaseStep = 6f;
    [SerializeField] private float heroAgilityDivisor = 8f;
    [SerializeField] private int enemyBaseMinStep = 4;
    [SerializeField] private int enemyBaseMaxStep = 8;
    [SerializeField] private float enemyAgilityDivisor = 6f;

    private void Awake()
    {
        // Find core objects
        var root = GameObject.Find(GameObjectHelper.Game.Timeline.Root).GetComponent<RectTransform>();
        viewport = GameObject.Find(GameObjectHelper.Game.Timeline.Viewport).GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.Game.Timeline.Content).GetComponent<RectTransform>();
        indicator = GameObject.Find(GameObjectHelper.Game.Timeline.Indicator).GetComponent<Image>();

        blockPrefab = PrefabLibrary.Prefabs["TimelineBlockPrefab"].GetComponent<TimelineBlockInstance>();
    }

    private class Block
    {
        public bool isHero;
        public ActorInstance enemy;       // null for hero blocks
        public string label;
        public Color color;
        public Sprite portrait;
        public TimelineBlockInstance view;
    }

    private class EnemySim
    {
        public ActorInstance enemy;
        public int delay;                 // ticks down on hero turns
        public int agility;
    }

    private readonly List<Block> blocks = new List<Block>();
    private readonly List<EnemySim> sim = new List<EnemySim>();

    private int currentIndex;
    private float contentX;
    private float targetContentX;

    // Countdown to next hero in real sim. Recomputed when a hero turn completes.
    private int heroDelaySim;

    private float UnitWidth => blockSize + blockGutter;

    // -------------- Public API --------------

    /// <summary>
    /// Prepare references, build initial sim and forecast, and center the indicator.
    /// Call once on stage start.
    /// </summary>
    public void Initialize()
    {
        SetupIndicator();
        RebuildFromScene();
    }

    /// <summary>
    /// Rebuild sim from scene, regenerate forecast, and center on current.
    /// Call when the actor roster changes significantly.
    /// </summary>
    public void RebuildFromScene()
    {
        Clear();

        BuildEnemySim();
        heroDelaySim = ComputeHeroStepFromAverageAgility();
        if (heroDelaySim < 1) heroDelaySim = 1;

        ExtendForecastUntil(forecastVisibleAhead);
        SetupLayout();

        // Only snap at init or rebuild. Normal advances slide.
        SnapToCurrent();
    }

    /// <summary>
    /// Advance exactly one block after the current turn completes.
    /// This is the only place that moves the belt forward.
    /// </summary>
    public void NextBlock()
    {
        if (blocks.Count == 0) return;

        var current = blocks[currentIndex];

        if (current.isHero)
        {
            // One hero turn elapsed: every enemy counts down by one in the SIM.
            for (int i = 0; i < sim.Count; i++)
                sim[i].delay = Mathf.Max(0, sim[i].delay - 1);

            // Reset hero cadence to a fresh step based on current average agility.
            heroDelaySim = ComputeHeroStepFromAverageAgility();

            // Do not mirror SIM ticks to UI here. UI shows forecast distance instead.
        }
        else
        {
            // The acting enemy just finished. Give it a fresh step based on its agility.
            var e = current.enemy;
            if (e != null && e.IsPlaying)
            {
                var s = sim.FirstOrDefault(z => z.enemy == e);
                if (s != null) s.delay = EnemyStepFromAgility(s.enemy, s.agility);
            }
        }

        // Move to next block and slide there.
        currentIndex = Mathf.Min(currentIndex + 1, Math.Max(0, blocks.Count - 1));
        targetContentX = GetTargetXForIndex(currentIndex);

        // Clean and extend the future.
        RemoveDeadEnemyBlocks();
        ExtendForecastUntil(currentIndex + forecastVisibleAhead);

        // Trim old history and relayout.
        TrimPastBlocks(trimLeftKeep);
        SetupLayout();

        // After forecast and layout are ready, update labels using forecast distance.
        UpdateAllEnemyDelayLabels();
    }

    /// <summary>
    /// Focus on the hero block at or after the current index. Slides.
    /// </summary>
    public void FocusOnHero()
    {
        int idx = FindNextIndex(b => b.isHero, currentIndex);
        if (idx >= 0) currentIndex = idx;
        targetContentX = GetTargetXForIndex(currentIndex);
    }

    /// <summary>
    /// Focus on the next block for a specific enemy. Slides.
    /// </summary>
    public void FocusOnEnemy(ActorInstance enemy)
    {
        if (enemy == null) return;
        int idx = FindNextIndex(b => !b.isHero && b.enemy == enemy, currentIndex);
        if (idx >= 0) currentIndex = idx;
        targetContentX = GetTargetXForIndex(currentIndex);
    }

    /// <summary>
    /// Return the enemy assigned to the current block, or null for hero blocks.
    /// </summary>
    public ActorInstance GetCurrentEnemy()
    {
        if (blocks.Count == 0) return null;
        var b = blocks[Mathf.Clamp(currentIndex, 0, blocks.Count - 1)];
        return b != null && !b.isHero ? b.enemy : null;
    }

    private void Update()
    {
        float dist = Mathf.Abs(targetContentX - contentX);
        float step = slideBase * Time.deltaTime + slideScale * Time.deltaTime * dist;
        contentX = Mathf.MoveTowards(contentX, targetContentX, step);

        content.anchoredPosition = new Vector2(contentX, 0f);
    }

    // -------------- Forecast build --------------

    private void BuildEnemySim()
    {
        sim.Clear();

        foreach (var e in g.Actors.Enemies.Where(x => x != null && x.IsPlaying))
        {
            int agi = e.Stats.Agility.ToInt();
            int seed = EnemyStepFromAgility(e, agi);
            sim.Add(new EnemySim { enemy = e, delay = seed, agility = agi });

            // UI label will be set after forecast is extended (see UpdateAllEnemyDelayLabels).
        }
    }

    /// <summary>
    /// Extend the forecast to at least requiredCount blocks.
    /// Always seeds an initial Hero block if the list is empty.
    /// </summary>
    private void ExtendForecastUntil(int requiredCount)
    {
        if (blocks.Count == 0)
        {
            // Force first block to be a Hero block, no matter what.
            AddHeroBlock();
        }

        if (blocks.Count >= requiredCount) return;

        // Local lookahead copy so we do not mutate the real sim while forecasting.
        int lookHero = heroDelaySim;
        var look = sim.Select(s => new EnemySim
        {
            enemy = s.enemy,
            delay = s.delay,
            agility = s.agility
        }).ToList();

        while (blocks.Count < requiredCount)
        {
            int minEnemyDelay = look.Count > 0 ? look.Min(s => s.delay) : int.MaxValue;
            int minDelay = Math.Min(lookHero, minEnemyDelay);

            if (minDelay > 0 && minDelay < int.MaxValue)
            {
                // Advance virtual time by the smallest pending delay.
                lookHero = Math.Max(0, lookHero - minDelay);
                for (int i = 0; i < look.Count; i++)
                    look[i].delay = Math.Max(0, look[i].delay - minDelay);
            }

            // Ready enemies take priority. Break ties by agility.
            var readyEnemies = look.Where(s => s.delay <= 0 && s.enemy != null && s.enemy.IsPlaying)
                                   .OrderByDescending(s => s.agility)
                                   .ToList();

            if (readyEnemies.Count > 0)
            {
                var pick = readyEnemies[0];
                AddEnemyBlock(pick.enemy);
                // Reseed the picked enemy in the lookahead.
                pick.delay = EnemyStepFromAgility(pick.enemy, pick.agility);
                continue;
            }

            // Otherwise, if hero is ready, schedule a hero block.
            if (lookHero <= 0)
            {
                AddHeroBlock();
                lookHero = ComputeHeroStepFromAverageAgility();
                continue;
            }

            // Safety: if both sides are "infinite", add a hero to make progress.
            if (minDelay == int.MaxValue)
            {
                AddHeroBlock();
                lookHero = ComputeHeroStepFromAverageAgility();
            }
        }
    }

    private int ComputeHeroStepFromAverageAgility()
    {
        var heroes = g.Actors.Heroes.Where(h => h != null && h.IsPlaying).ToList();
        if (heroes.Count == 0) return Mathf.Max(1, Mathf.RoundToInt(heroBaseStep));

        float avgAgi = heroes.Average(h => (float)h.Stats.Agility.ToInt());
        float raw = heroBaseStep - (avgAgi / Mathf.Max(1f, heroAgilityDivisor));
        int step = Mathf.Clamp(Mathf.RoundToInt(raw), 1, 12);
        return step;
    }

    private int EnemyStepFromAgility(ActorInstance enemy, int agility)
    {
        int baseStep = RNG.Int(enemyBaseMinStep, enemyBaseMaxStep);
        float raw = baseStep - (agility / Mathf.Max(1f, enemyAgilityDivisor));
        int step = Mathf.Clamp(Mathf.RoundToInt(raw), 1, 16);
        return step;
    }

    // -------------- Layout --------------

    private void SetupLayout()
    {
        // Position and size each view.
        for (int i = 0; i < blocks.Count; i++)
        {
            var b = blocks[i];
            if (b.view == null) continue;

            float x = i * UnitWidth;
            b.view.Rect.anchoredPosition = new Vector2(x, 0f);
            b.view.Resize(blockSize, blockSize);
        }

        // Content width large enough to hold all blocks or at least the viewport.
        float width = Mathf.Max(viewport.rect.width, blocks.Count * UnitWidth);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    private void AddHeroBlock()
    {
        var b = new Block
        {
            isHero = true,
            enemy = null,
            label = "Heroes",
            color = ColorHelper.Transparent.White,
            portrait = SpriteLibrary.GUI["TeamIcon"]
        };

        var view = Instantiate(blockPrefab, content);
        view.SetPortraitYOffset(0f);              // Center portrait for hero blocks
        view.SetSquareMask(blockSize);
        view.Set(b.label, b.color, b.portrait);
        b.view = view;

        blocks.Add(b);
    }

    private void AddEnemyBlock(ActorInstance enemy)
    {
        if (enemy == null) return;

        var b = new Block
        {
            isHero = false,
            enemy = enemy,
            label = string.IsNullOrEmpty(enemy.characterName) ? "Enemy" : enemy.characterName,
            color = new Color(0.10f, 0.60f, 1f, 1f),
            portrait = enemy.Render.thumbnail.sprite
        };

        var view = Instantiate(blockPrefab, content);
        view.SetSquareMask(blockSize);
        view.Set(b.label, b.color, b.portrait);
        b.view = view;

        blocks.Add(b);
    }

    private void RemoveDeadEnemyBlocks()
    {
        for (int i = blocks.Count - 1; i >= currentIndex; i--)
        {
            var b = blocks[i];
            if (!b.isHero && (b.enemy == null || !b.enemy.IsPlaying))
            {
                if (b.view != null) Destroy(b.view.gameObject);
                blocks.RemoveAt(i);
            }
        }

        sim.RemoveAll(s => s.enemy == null || !s.enemy.IsPlaying);
    }

    private void TrimPastBlocks(int keepLeft)
    {
        int removable = Mathf.Max(0, currentIndex - keepLeft);
        if (removable <= 0) return;

        for (int i = 0; i < removable; i++)
        {
            var b = blocks[0];
            if (b.view != null) Destroy(b.view.gameObject);
            blocks.RemoveAt(0);
        }

        currentIndex -= removable;
        if (currentIndex < 0) currentIndex = 0;
    }

    private int FindNextIndex(Func<Block, bool> predicate, int startIndex)
    {
        int start = Mathf.Clamp(startIndex, 0, blocks.Count - 1);
        for (int i = start; i < blocks.Count; i++)
            if (predicate(blocks[i])) return i;
        return -1;
    }

    private float GetTargetXForIndex(int index)
    {
        float blockCenter = index * UnitWidth + (blockSize * 0.5f);
        float viewCenter = viewport.rect.width * 0.5f;
        float offset = viewCenter - blockCenter;
        return offset;
    }

    private void SetupIndicator()
    {
        if (indicator == null || viewport == null) return;

        var r = indicator.rectTransform;

        // Center anchors and pivot.
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;

        // Ensure indicator height matches block height. Keep at least a thin width.
        float width = Mathf.Max(r.sizeDelta.x, 4f);
        r.sizeDelta = new Vector2(width, blockSize);
    }

    private void SnapToCurrent()
    {
        contentX = targetContentX = GetTargetXForIndex(currentIndex);
        content.anchoredPosition = new Vector2(contentX, 0f);
    }

    private void Clear()
    {
        foreach (var b in blocks)
            if (b.view != null) Destroy(b.view.gameObject);

        blocks.Clear();
        sim.Clear();
        currentIndex = 0;
        contentX = 0f;
        targetContentX = 0f;
    }

    // -------------- Turn delay label helpers --------------

    // Returns number of blocks until this enemy acts next (1 means 'next block').
    private int BlocksUntilNextTurn(ActorInstance enemy)
    {
        if (enemy == null) return -1;
        for (int i = currentIndex + 1; i < blocks.Count; i++)
        {
            var b = blocks[i];
            if (!b.isHero && b.enemy == enemy)
                return i - currentIndex; // 1-based distance in blocks
        }
        return -1;
    }

    private void UpdateEnemyDelayLabel(ActorInstance enemy)
    {
        if (enemy == null || !enemy.IsPlaying) return;
        int dist = BlocksUntilNextTurn(enemy);
        // Pass -1 to clear if not in forecast yet (shouldn’t happen as we extend ahead).
        enemy.Render.SetTurnDelayText(dist);
    }

    private void UpdateAllEnemyDelayLabels()
    {
        foreach (var s in sim)
        {
            if (s.enemy != null && s.enemy.IsPlaying)
                UpdateEnemyDelayLabel(s.enemy);
        }
    }
}
