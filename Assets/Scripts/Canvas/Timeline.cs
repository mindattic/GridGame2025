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
using Assets.Scripts.Models;

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


    [SerializeField] private bool alignIndicatorToBlockCenter = true;

    private List<ActorInstance> currentRoundOrder = new List<ActorInstance>();
    private int currentRoundPos = 0;
    private int roundNumber = 0;
    private readonly List<ActorInstance> nextRoundFirst = new List<ActorInstance>();
    private readonly HashSet<ActorInstance> deferredToNextRound = new HashSet<ActorInstance>();

    private void Awake()
    {
        // Find core objects
        var root = GameObject.Find(GameObjectHelper.Game.Timeline.Root).GetComponent<RectTransform>();
        viewport = GameObject.Find(GameObjectHelper.Game.Timeline.Viewport).GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.Game.Timeline.Content).GetComponent<RectTransform>();
     
        blockPrefab = PrefabLibrary.Prefabs["TimelineBlockPrefab"].GetComponent<TimelineBlockInstance>();
    }

    private class Block
    {
        public bool isHero;
        public bool isDivider;
        public ActorInstance actor;      // actor for this block (null for divider)
        public string label;
        public Sprite portrait;
        public TimelineBlockInstance instance;
    }

    private class SimEntry { public ActorInstance actor; public int delay; public int speed; }

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
        RebuildFromScene();
        UpdateSelectionHighlight();
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
        UpdateSelectionHighlight();
    }

    /// <summary>
    /// Return the hero assigned to the current block, or null for enemy blocks.
    /// </summary>
    public ActorInstance GetCurrentHero()
    {
        if (blocks.Count == 0) return null;
        var b = blocks[Mathf.Clamp(currentIndex, 0, blocks.Count - 1)];
        return b != null && b.isHero ? b.actor : null;
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
        UpdateSelectionHighlight();

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
        UpdateSelectionHighlight();
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
        UpdateSelectionHighlight();
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
    }

    /// <summary>
    /// Extend the forecast to at least requiredCount blocks using round-based ordering.
    /// Each actor appears once per round; order inside a round is by speed (desc).
    /// The very first block is forced to be the fastest hero if available.
    /// </summary>
    private void ExtendForecastUntil(int requiredCount)
    {
        if (blocks.Count >= requiredCount) return;

        // Ensure at least one actor exists.
        bool anyActive = g.Actors.All.Any(a => a != null && a.IsPlaying);
        if (!anyActive) return;

        while (blocks.Count < requiredCount)
        {
            // Build a new round if needed.
            if (currentRoundOrder == null || currentRoundPos >= currentRoundOrder.Count)
            {
                bool isFirstRoundAndEmpty = roundNumber == 0 && blocks.Count == 0;
                currentRoundOrder = BuildRoundOrder(isFirstRoundAndEmpty);
                currentRoundPos = 0;
                roundNumber++;

                // Insert a divider between rounds (not for the very first round)
                if (roundNumber > 1)
                    AddRoundDivider(roundNumber);

                // Safety: if for some reason no actors are in the round, abort.
                if (currentRoundOrder == null || currentRoundOrder.Count == 0)
                    break;
            }

            // Add the next actor from the current round.
            var actor = currentRoundOrder[currentRoundPos++];
            if (actor == null || !actor.IsPlaying)
                continue; // skip and continue filling

            AddActorBlock(actor);
        }
    }

    private List<ActorInstance> BuildRoundOrder(bool forceFirstHero)
    {
        // Start from active actors ordered by speed descending
        var active = g.Actors.All.Where(a => a != null && a.IsPlaying).ToList();
        var ordered = active.OrderByDescending(a => a.Stats.Speed.ToInt()).ToList();

        // Apply next-round promotions (actors explicitly placed at the start)
        var head = new List<ActorInstance>();
        if (!forceFirstHero)
        {
            foreach (var a in nextRoundFirst)
            {
                if (a != null && a.IsPlaying && !head.Contains(a)) head.Add(a);
            }
            foreach (var a in deferredToNextRound)
            {
                if (a != null && a.IsPlaying && !head.Contains(a)) head.Add(a);
            }
        }

        // Remove promoted/deferred actors from the remainder
        if (head.Count > 0)
            ordered.RemoveAll(a => head.Contains(a));

        // Force fastest hero as first block of the very first round
        if (forceFirstHero)
        {
            var fastestHero = ordered.Where(a => a.IsHero).OrderByDescending(a => a.Stats.Speed.ToInt()).FirstOrDefault();
            if (fastestHero != null)
            {
                ordered.Remove(fastestHero);
                head.Insert(0, fastestHero);
            }
        }

        // Compose final round order
        var round = new List<ActorInstance>(head.Count + ordered.Count);
        round.AddRange(head);
        round.AddRange(ordered);

        // Clear one-time directives after building the round
        nextRoundFirst.Clear();
        deferredToNextRound.Clear();

        return round;
    }

    /// <summary>
    /// Request that an actor takes the first turn in the next round.
    /// </summary>
    public void SetNextRoundFirst(ActorInstance actor)
    {
        if (actor == null) return;
        if (!nextRoundFirst.Contains(actor)) nextRoundFirst.Add(actor);
    }

    /// <summary>
    /// Defer an actor's current turn to the beginning of the next round.
    /// Note: Ability systems should also cancel their current turn when calling this.
    /// </summary>
    public void DeferTurnToNextRoundFront(ActorInstance actor)
    {
        if (actor == null) return;
        deferredToNextRound.Add(actor);
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
        var b = new Block
        {
            isHero = actor.IsHero,
            isDivider = false,
            actor = actor,
            label = string.IsNullOrEmpty(actor.characterName) ? (actor.IsHero ? "Hero" : "Enemy") : actor.characterName,
            portrait = actor.Render.thumbnail.sprite
        };

        var go = Instantiate(blockPrefab, content);
        go.SetOwner(actor);
        go.SetSquareMask(blockSize);
        go.Set(b.label, b.portrait);
        go.SetBackSprite(SpriteLibrary.GUI["TimelineBlock"]);

        var data = ActorLibrary.Get(actor.characterName);
        var crop = data != null && data.CanvasThumbnailSettings != null ? data.CanvasThumbnailSettings : CanvasThumbnailSettings.Default;
        go.ApplyCanvasCrop(crop);

        if (actor.IsEnemy) go.TintBackForEnemy(); else go.TintBackForHero();
        go.SetSelected(false);
        go.SetCurrent(false);

        b.instance = go; b.instance.name = $"TimelineBlock_{nextBlockId++}";
        blocks.Add(b);
    }

    private void AddRoundDivider(int currentRound)
    {
        var b = new Block { isHero = false, isDivider = true, actor = null, label = $"Round {currentRound}", portrait = null };
        var go = Instantiate(blockPrefab, content);
        go.SetOwner(null);
        go.SetSquareMask(blockSize);
        go.SetDivider(SpriteLibrary.GUI["TimelineDivider"], b.label);
        go.SetSelected(false);
        go.SetCurrent(false);
        b.instance = go; b.instance.name = $"TimelineDivider_{nextBlockId++}";
        blocks.Add(b);
    }

    private void RemoveDeadBlocks()
    {
        for (int i = blocks.Count - 1; i >= currentIndex; i--)
        {
            var b = blocks[i];
            if (b.isDivider) continue;
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
        // Block i left edge is at i * UnitWidth. Optionally align to block center.
        float targetBlockX = index * UnitWidth + (alignIndicatorToBlockCenter ? blockSize * 0.5f : 0f);

        float indicatorLocalX = 0f;
        if (indicator != null)
        {
            var r = indicator.rectTransform;
            indicatorLocalX = r.anchoredPosition.x; // from viewport's left
        }

        // Move content so targetBlockX aligns to indicator X
        float offset = indicatorLocalX - targetBlockX;
        return offset;
    }

    private void SnapToCurrent()
    {
        var snap = GetTargetXForIndex(currentIndex);
        contentX = targetContentX = snap;
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

        // Reset round state
        currentRoundOrder.Clear();
        currentRoundPos = 0;
        roundNumber = 0;
        nextRoundFirst.Clear();
        deferredToNextRound.Clear();
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
        enemy.Render.SetTurnDelayText(dist);
    }

    private void UpdateAllEnemyDelayLabels()
    {
        foreach (var e in g.Actors.Enemies)
        {
            if (e != null && e.IsPlaying)
                UpdateEnemyDelayLabel(e);
        }
    }

    public void RefreshSelectionHighlight()
    {
        UpdateSelectionHighlight();
    }

    private void UpdateSelectionHighlight()
    {
        var focused = g.Actors.FocusedActor;
        for (int i = 0; i < blocks.Count; i++)
        {
            var b = blocks[i];
            if (b.instance == null) continue;
            bool isCurrent = (i == currentIndex) && !b.isDivider;
            b.instance.SetCurrent(isCurrent); // ActiveIndicator
            bool isFocused = !b.isDivider && focused != null && b.instance.Owner == focused;
            b.instance.SetSelected(isFocused); // FocusIndicator
        }
    }
}
