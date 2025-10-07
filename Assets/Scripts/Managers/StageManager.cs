using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Game.Manager;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using scene = Assets.Helpers.SceneHelper;
using Assets.Scripts.Managers; // added
using Assets.Scripts.Sequences;
using Assets.Scripts.Libraries; // NEW

public class StageManager : MonoBehaviour
{
    // Internal property:
    public int enemyCount => g.Actors.All.FindAll(x => x.IsEnemy).Count;

    // Fields:
    private GameObject actorPrefab;
    public Stage currentStage;
    private int currentWave = 0; // Track the current wave

    // Endless state
    private bool IsEndless => GameModeHelper.IsEndless;

    public void Awake()
    {
        actorPrefab = PrefabLibrary.Prefabs["ActorPrefab"];
    }

    /// <summary>
    /// Initializes the StageManager by retrieving the stage name from the hero's profile,
    /// loading the corresponding Stage, and then loading the stage.
    /// </summary>
    public void Initialize()
    {
        var latestSave = ProfileHelper.CurrentProfile.LatestSave; // Assumes a helper property LatestSave is defined.
        if (latestSave == null)
        {
            Debug.LogError("No saved game state found.");
            return;
        }

        // Begin a new XP session with current party participants (shared flow)
        var participants = ProfileHelper.CurrentProfile.CurrentSave?.Party?.Members?.Select(m => m.Character);
        ExperienceTracker.StartSession(participants);

        if (IsEndless)
        {
            InitializeEndless();
            return;
        }

        currentStage = StageLibrary.Get(latestSave.Stage.CurrentStage);
        RestartStage();
    }

    private void InitializeEndless()
    {
        // Reset
        currentWave = 0;
        g.ActorManager.Clear();
        g.DottedLineManager.Clear();
        g.SynergyLineManager.Clear();
        g.CoinCounter.Refresh();
        g.TileManager.Reset();

        // Build a nominal stage placeholder
        currentStage = new Stage
        {
            Name = "Endless",
            Description = "Endless",
            CompletionCondition = "Endless",
            CompletionValue = 0,
            Waves = new System.Collections.Generic.List<StageWave>()
        };

        // Spawn party heroes from the save directly (no PartyManager / no level overrides)
        foreach (var partyMember in ProfileHelper.CurrentProfile.CurrentSave.Party.Members)
        {
            var hero = ActorLibrary.Actors[partyMember.Character];
            int level = Mathf.Max(1, partyMember.Level);
            var stageActor = new StageActor(partyMember.Character, Team.Hero, level, location: RNG.UnoccupiedLocation);
            SpawnActor(stageActor, rebuildTimeline: false);
        }

        // Generate and load wave 1
        LoadEndlessWave(0);

        // After all actors for the initial setup are spawned, rebuild timeline once
        g.Timeline?.RebuildFromScene();

        scene.FadeIn();
    }

    private void LoadEndlessWave(int waveIndex)
    {
        int nextWaveNumber = waveIndex + 1;
        var wave = Assets.Scripts.Managers.EndlessWaveGenerator.Generate(nextWaveNumber, GameModeHelper.Tags);

        // Track current index
        currentWave = waveIndex;

        // Pre-spawn actors; those with SpawnTurn > current will stay inactive until turn threshold
        foreach (var stageActor in wave.Actors)
        {
            SpawnActor(stageActor, rebuildTimeline: false);
        }

        // Recalculate timeline once per wave start
        g.Timeline?.RebuildFuturePreservingCurrent();

        // Announcement (total unknown/infinite)
        g.WaveAnnouncement.ShowEndless(nextWaveNumber);
    }

    /// <summary>
    /// Loads the selected stage and initializes the first wave.
    /// </summary>
    public void RestartStage()
    {
        // Reset everything for a new stage.
        currentWave = ProfileHelper.CurrentProfile.CurrentSave.Stage.CurrentWave;
        g.ActorManager.Clear();
        g.DottedLineManager.Clear();
        g.SynergyLineManager.Clear();
        g.CoinCounter.Refresh();
        g.TileManager.Reset();
        //g.TurnManager.Initialize();

        // Show persistent hero actors from ProfileHelper
        foreach (var partyMember in ProfileHelper.CurrentProfile.CurrentSave.Party.Members)
        {
            var hero = ActorLibrary.Actors[partyMember.Character];
            var stageActor = new StageActor(partyMember.Character, Team.Hero, hero.Level, location: RNG.UnoccupiedLocation);
            // Defer timeline rebuild during bulk spawns
            SpawnActor(stageActor, rebuildTimeline: false);
        }

        // Load the wave based on currentWave.
        if (currentStage.Waves.Count > 0)
        {
            LoadWave(currentWave);
        }
        else
        {
            Debug.LogError($"Stage {currentStage.Name} has no waves defined.");
        }

        // After all actors for the initial setup are spawned, rebuild timeline once
        g.Timeline?.RebuildFromScene();

        scene.FadeIn();
    }

    /// <summary>
    /// Loads the given wave index.
    /// </summary>
    private void LoadWave(int waveIndex)
    {
        if (waveIndex >= currentStage.Waves.Count)
        {
            Debug.LogError($"Wave index {waveIndex} is out of bounds for stage {currentStage.Name}.");
            return;
        }

        StageWave wave = currentStage.Waves[waveIndex];

        // Show actors for this wave
        foreach (var stageActor in wave.Actors)
        {
            // Defer timeline rebuild until all spawns are finished
            SpawnActor(stageActor, rebuildTimeline: false);
        }

        // Show dotted supportLines' for this wave
        foreach (var stageDottedLine in wave.DottedLines)
        {
            var segment = stageDottedLine.Segment;
            var location = stageDottedLine.Location;
            g.DottedLineManager.Spawn(segment, location);
        }

        // Recalculate timeline once per wave start
        g.Timeline?.RebuildFuturePreservingCurrent();

        g.WaveAnnouncement.Show(waveIndex + 1, currentStage.Waves.Count);
    }


    /// <summary>
    /// Spawns a new actor on a guaranteed free tile.
    /// Always assigns a fresh unoccupied location to the StageActor.
    /// </summary>
    public ActorInstance SpawnActor(StageActor stageActor, bool rebuildTimeline = true)
    {
        // Instantiate and parent under the board
        var go = Instantiate(actorPrefab, Vector2.zero, Quaternion.identity);
        var instance = go.GetComponent<ActorInstance>();
        instance.transform.SetParent(g.Board.transform, false);
        instance.name = $"{stageActor.characterName}_{Guid.NewGuid():N}";
        instance.characterName = stageActor.characterName;
        instance.team = stageActor.Team;

        // Stats and metadata
        instance.Stats = ActorLibrary.Actors[stageActor.characterName].GetStats(stageActor.Level);
        instance.transform.localScale = GameManager.instance.tileScale;
        instance.spawnTurn = stageActor.SpawnTurn;

        // Pick and assign location, then spawn
        var location = RNG.UnoccupiedLocation;

        // This ensures that the game's stage data "knows" where this actor is starting.
        // Used for saving, AI planning, and any systems that read StageActor info.
        stageActor.Location = location;

        // This physically places the GameObject in the scene and updates the tile's occupancy.
        instance.Spawn(location);

        // Register the new actor
        g.Actors.All.Add(instance);

        // If requested, rebuild the timeline after spawning (useful for ad-hoc spawns)
        if (rebuildTimeline)
        {
            g.Timeline?.RebuildFuturePreservingCurrent();
        }

        return instance;
    }

    /// <summary>
    /// Called once per turn advance (from TurnManager) to activate any actors whose spawnTurn has arrived.
    /// </summary>
    public void OnTurnAdvanced()
    {
        foreach (var a in g.Actors.All)
        {
            if (a == null) continue;
            a.ActivateIfSpawnable();
        }
    }

    /// <summary>
    /// Called when an actor dies. Triggers checks for game over or stage completion.
    /// </summary>
    public void OnActorDeath()
    {
        CheckBattleLost();
        if (IsEndless) CheckEndlessWaveComplete(); else CheckWaveComplete();
    }

    /// <summary>
    /// Endless flow: when all enemies are dead, generate and load the next wave.
    /// </summary>
    private void CheckEndlessWaveComplete()
    {
        bool allEnemiesDead = g.Actors.Enemies.All(x => x.Flags.HasSpawned && x.IsDead);
        if (!allEnemiesDead)
            return;

        currentWave++;
        LoadEndlessWave(currentWave);
    }

    /// <summary>
    /// Checks if the current wave is complete and moves to the next wave or completes the stage.
    /// </summary>
    private void CheckWaveComplete()
    {
        bool allEnemiesDead = g.Actors.Enemies.All(x => x.Flags.HasSpawned && x.IsDead);
        if (!allEnemiesDead)
            return;

        currentWave++;

        if (currentWave < currentStage.Waves.Count)
        {
            Debug.Log($"All enemies defeated. Loading next wave: {currentWave + 1}");
            LoadWave(currentWave);
        }
        else
        {
            CheckBattleWon();
        }
    }

    /// <summary>
    /// Handles what happens when all waves of a stage are completed.
    /// </summary>
    private void CheckBattleWon()
    {
        if (currentWave < currentStage.Waves.Count)
            return;

        bool allEnemiesDead = g.Actors.Enemies.All(x => x.Flags.HasSpawned && x.IsDead);
        if (!allEnemiesDead)
            return;

        g.SequenceManager.Add(new BattleWonSequence());
        g.SequenceManager.Execute();
    }

    /// <summary>
    /// Checks whether the game is over.
    /// </summary>
    private void CheckBattleLost()
    {
        bool allHeroesDead = g.Actors.Heroes.All(x => x.Flags.HasSpawned && x.IsDead);
        if (!allHeroesDead)
            return;

        g.SequenceManager.Add(new BattleLostSequence());
        g.SequenceManager.Execute();
    }

    /// <summary>
    /// Convenience method for adding a new attacker actor.
    /// </summary>
    /// <param name="character">characterName type for the attacker.</param>
    public ActorInstance AddEnemy(string character)
    {
        var stageActor = new StageActor(character, Team.Enemy, level: 1, location: RNG.UnoccupiedLocation);
        return SpawnActor(stageActor);
    }

}
