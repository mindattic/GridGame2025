// --- File: Assets/Scripts/Canvas/Timeline.cs ---
using Assets.Helper;
using Assets.Scripts.Canvas.Timeline; // for TimelineBlockInstance
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Helper.GameObjectHelper.Game;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Timeline is the single source of truth for turn order.
/// All actors (heroes and enemies) use the same cadence formula based on their own speed.
/// Exactly one-block advance per completed turn.
/// Forward-only forecast that is always extended so you never see the end.
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

    [Header("Step Tuning")] // applied to both heroes and enemies
    [SerializeField] private int baseMinStep = 4;
    [SerializeField] private int baseMaxStep = 8;
    [SerializeField] private float speedDivisor = 6f;

    [Header("Indicator")] 
    [Tooltip("Horizontal offset of the indicator inside the viewport (in pixels). 0 = flush with left edge.")]
    [SerializeField] private float indicatorOffsetX = 16f;

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
        public ActorInstance actor;      // actor for this block
        public string label;
        public Color color;
        public Sprite portrait;
        public TimelineBlockInstance instance;
    }

    private class SimEntry
    {
        public ActorInstance actor;
        public int delay;                 // ticks until ready
        public int speed;
    }

    private readonly List<Block> blocks = new List<Block>();
    private readonly List<SimEntry> sim = new List<SimEntry>();
    
    private int nextBlockId;
    private int currentIndex;
    private float contentX;
    private float targetContentX;

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

        BuildSim();

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

        // The acting unit just finished. Give it a fresh step based on its agility.
        var a = current.actor;
        if (a != null && a.IsPlaying)
        {
            var s = sim.FirstOrDefault(z => z.actor == a);
            if (s != null) s.delay = StepFromSpeed(s.actor, s.speed);
        }

        // Move to next block and slide there.
        currentIndex = Mathf.Min(currentIndex + 1, Math.Max(0, blocks.Count - 1));
        targetContentX = GetTargetXForIndex(currentIndex);

        // Clean and extend the future.
        RemoveDeadBlocks();
        ExtendForecastUntil(currentIndex + forecastVisibleAhead);

        // Trim old history and relayout.
        TrimPastBlocks(trimLeftKeep);
        SetupLayout();

        // After forecast and layout are ready, update labels using forecast distance (enemies only).
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
        int idx = FindNextIndex(b => !b.isHero && b.actor == enemy, currentIndex);
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
        return b != null && !b.isHero ? b.actor : null;
    }

    private void Update()
    {
        float dist = Mathf.Abs(targetContentX - contentX);
        float step = slideBase * Time.deltaTime + slideScale * Time.deltaTime * dist;
        contentX = Mathf.MoveTowards(contentX, targetContentX, step);

        content.anchoredPosition = new Vector2(contentX, 0f);
    }

    // -------------- Forecast build --------------

    private void BuildSim()
    {
        sim.Clear();

        foreach (var h in g.Actors.Heroes.Where(x => x != null && x.IsPlaying))
        {
            int spd = h.Stats.Speed.ToInt();
            int seed = StepFromSpeed(h, spd);
            sim.Add(new SimEntry { actor = h, delay = seed, speed = spd });
        }

        foreach (var e in g.Actors.Enemies.Where(x => x != null && x.IsPlaying))
        {
            int spd = e.Stats.Speed.ToInt();
            int seed = StepFromSpeed(e, spd);
            sim.Add(new SimEntry { actor = e, delay = seed, speed = spd });
        }

        // Labels are updated after forecast is extended (see UpdateAllEnemyDelayLabels).
    }

    /// <summary>
    /// Extend the forecast to at least requiredCount blocks.
    /// </summary>
    private void ExtendForecastUntil(int requiredCount)
    {
        if (blocks.Count >= requiredCount) return;

        // Local lookahead copy so we do not mutate the real sim while forecasting.
        var look = sim.Select(s => new SimEntry
        {
            actor = s.actor,
            delay = s.delay,
            speed = s.speed
        }).ToList();

        // Ensure at least one block is added even if values are degenerate.
        if (blocks.Count == 0 && look.Count == 0) return;

        while (blocks.Count < requiredCount)
        {
            int minDelay = look.Count > 0 ? look.Min(s => s.delay) : int.MaxValue;

            if (minDelay > 0 && minDelay < int.MaxValue)
            {
                // Advance virtual time by the smallest pending delay.
                for (int i = 0; i < look.Count; i++)
                    look[i].delay = Mathf.Max(0, look[i].delay - minDelay);
            }

            // Ready actors take priority. Break ties by agility.
            var ready = look.Where(s => s.delay <= 0 && s.actor != null && s.actor.IsPlaying)
                            .OrderByDescending(s => s.speed)
                            .ToList();

            if (ready.Count > 0)
            {
                var pick = ready[0];
                AddActorBlock(pick.actor);
                // Reseed the picked actor in the lookahead.
                pick.delay = StepFromSpeed(pick.actor, pick.speed);
                continue;
            }

            // Safety: if everyone has infinite delay, reseed a random one to make progress.
            if (minDelay == int.MaxValue)
            {
                var any = look.FirstOrDefault(s => s.actor != null && s.actor.IsPlaying);
                if (any == null) break;
                AddActorBlock(any.actor);
                any.delay = StepFromSpeed(any.actor, any.speed);
            }
        }
    }

    private int StepFromSpeed(ActorInstance actor, int speed)
    {
        int baseStep = RNG.Int(baseMinStep, baseMaxStep);
        float raw = baseStep - (speed / Mathf.Max(1f, speedDivisor));
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
            if (b.instance == null) continue;

            float x = i * UnitWidth;
            b.instance.Rect.anchoredPosition = new Vector2(x, 0f);
            b.instance.Resize(blockSize, blockSize);
        }

        // Content width large enough to hold all blocks or at least the viewport.
        float width = Mathf.Max(viewport.rect.width, blocks.Count * UnitWidth);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    private void AddActorBlock(ActorInstance actor)
    {
        if (actor == null) return;

        bool isHero = actor.IsHero;
        var b = new Block
        {
            isHero = isHero,
            actor = actor,
            label = string.IsNullOrEmpty(actor.characterName) ? (isHero ? "Hero" : "Enemy") : actor.characterName,
            color = isHero ? ColorHelper.Transparent.White : ColorHelper.Solid.GunMetal,
            portrait = actor.Render.thumbnail.sprite
        };

        var go = Instantiate(blockPrefab, content);
        if (isHero)
            go.SetPortraitYOffset(0f);        // Center portrait for hero blocks
        go.SetSquareMask(blockSize);
        go.Set(b.label, b.color, b.portrait);
        b.instance = go;
        b.instance.name = $"TimelineBlock_{nextBlockId++}";

        blocks.Add(b);
    }

    private void RemoveDeadBlocks()
    {
        for (int i = blocks.Count - 1; i >= currentIndex; i--)
        {
            var b = blocks[i];
            if (b.actor == null || !b.actor.IsPlaying)
            {
                if (b.instance != null) Destroy(b.instance.gameObject);
                blocks.RemoveAt(i);
            }
        }

        sim.RemoveAll(s => s.actor == null || !s.actor.IsPlaying);
    }

    private void TrimPastBlocks(int keepLeft)
    {
        int removable = Mathf.Max(0, currentIndex - keepLeft);
        if (removable <= 0) return;

        for (int i = 0; i < removable; i++)
        {
            var b = blocks[0];
            if (b.instance != null) Destroy(b.instance.gameObject);
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
        // Align the LEFT edge of the current block to the indicator's X (in viewport local space).
        // Block i left edge is at i * UnitWidth in content space.
        float blockLeft = index * UnitWidth;
        float indicatorLocalX = 0f;
        if (indicator != null)
        {
            var r = indicator.rectTransform;
            indicatorLocalX = r.anchoredPosition.x; // since anchor/pivot are left/center, this is from viewport's left
        }
        float offset = indicatorLocalX - blockLeft; // move content so block's left aligns to indicator X
        return offset;
    }

    private void SetupIndicator()
    {
        if (indicator == null || viewport == null) return;

        var r = indicator.rectTransform;

        // Anchor the indicator to the far LEFT, vertically centered, with a left-edge pivot.
        r.anchorMin = new Vector2(0f, 0.5f);
        r.anchorMax = new Vector2(0f, 0.5f);
        r.pivot = new Vector2(0f, 0.5f);
        r.anchoredPosition = new Vector2(indicatorOffsetX, 0f);

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
            if (b.instance != null) Destroy(b.instance.gameObject);

        blocks.Clear();
        sim.Clear();
        currentIndex = 0;
        contentX = 0f;
        targetContentX = 0f;
        nextBlockId = 0; // restart numbering on full rebuild
    }

    // -------------- Turn delay label helpers --------------

    // Returns number of blocks until this enemy acts next (1 means 'next block').
    private int BlocksUntilNextTurn(ActorInstance enemy)
    {
        if (enemy == null) return -1;
        for (int i = currentIndex + 1; i < blocks.Count; i++)
        {
            var b = blocks[i];
            if (!b.isHero && b.actor == enemy)
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
            if (s.actor != null && s.actor.IsPlaying && s.actor.IsEnemy)
                UpdateEnemyDelayLabel(s.actor);
        }
    }
}
