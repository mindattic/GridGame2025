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

public class StageManager : MonoBehaviour
{
    // Internal property:
    public int enemyCount => g.Actors.All.FindAll(x => x.IsEnemy).Count;

    // Fields:
    private GameObject actorPrefab;
    public Stage currentStage;
    private int currentWave = 0; // Track the current wave

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




        currentStage = StageLibrary.Get(latestSave.Stage.CurrentStage);
        RestartStage();
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
            SpawnActor(stageActor);
        }

        //HACK: For some reason enemies might spawn on top of g.Actors.Heroes because they aren't loaded at same time...
        //g.Actors.All.ForEach(s => s.Flags.HasSpawned = true);

        // Load the wave based on currentWave.
        if (currentStage.Waves.Count > 0)
        {
            LoadWave(currentWave);
        }
        else
        {
            Debug.LogError($"Stage {currentStage.Name} has no waves defined.");
        }


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
            SpawnActor(stageActor);
        }

        // Show dotted supportLines' for this wave
        foreach (var stageDottedLine in wave.DottedLines)
        {
            var segment = stageDottedLine.Segment;
            var location = stageDottedLine.Location;
            g.DottedLineManager.Spawn(segment, location);
        }

        g.WaveAnnouncement.Show(waveIndex + 1, currentStage.Waves.Count);
    }


    /// <summary>
    /// Spawns a new actor on a guaranteed free tile.
    /// Always assigns a fresh unoccupied location to the StageActor.
    /// </summary>
    public ActorInstance SpawnActor(StageActor stageActor)
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

        return instance;
    }

    /// <summary>
    /// Called when an actor dies. Triggers checks for game over or stage completion.
    /// </summary>
    public void OnActorDeath()
    {
        CheckGameOver();
        CheckWaveCompletion();
    }

    /// <summary>
    /// Checks if the current wave is complete and moves to the next wave or completes the stage.
    /// </summary>
    private void CheckWaveCompletion()
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
            Debug.Log("All waves completed. Stage is complete.");
            OnStageComplete();
        }
    }

    /// <summary>
    /// Handles what happens when all waves of a stage are completed.
    /// </summary>
    private void OnStageComplete()
    {
        //IEnumerator loadNextStageRoutine()
        //{
        //    var stageName = currentStage.NextStage;
        //    currentStage = StageLibrary.Get(stageName);
        //    RestartStage();
        //    yield return Wait.None();
        //}

       
        scene.Change.ToOverworld();

        //scene.FadeOut(scene.Change.ToOverworld());
    }

    /// <summary>
    /// Checks whether the game is over.
    /// </summary>
    private void CheckGameOver()
    {
        bool allPlayersDead = g.Actors.Heroes.All(x => x.Flags.HasSpawned && x.IsDead);
        if (!allPlayersDead)
            return;

        IEnumerator reloadStageRoutine()
        {
            RestartStage();
            yield return Wait.None();
        }

        scene.FadeOut(reloadStageRoutine());
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
