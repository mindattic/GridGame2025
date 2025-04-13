using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Quick Reference Properties:
    protected FadeInstance fade => GameManager.instance.fade;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    public int totalCoins
    {
        get => GameManager.instance.totalCoins;
        set => GameManager.instance.totalCoins = value;
    }
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected DottedLineManager dottedLineManager => GameManager.instance.dottedLineManager;
    protected CoinBar coinBar => GameManager.instance.coinBar;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    protected BoardInstance board => GameManager.instance.board;
    protected TutorialPopup tutorialPopup => GameManager.instance.tutorialPopup;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected WaveAnnouncement waveAnnouncement => GameManager.instance.waveAnnouncement;

    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;

    // Access the list of actors from the GameManager.
    protected List<ActorInstance> actors
    {
        get => GameManager.instance.actors;
        set => GameManager.instance.actors = value;
    }

    // Internal property:
    public int enemyCount => actors.FindAll(x => x.isEnemy).Count;

    // Fields:
    [SerializeField] public GameObject actorPrefab;
    public Stage currentStage;
    private int currentWave = 0; // Track the current wave


    /// <summary>
    /// Initializes the StageManager by retrieving the stage name from the hero's profile,
    /// loading the corresponding Stage, and then loading the stage.
    /// </summary>
    public void Initialize()
    {
        var latestSave = ProfileRepo.instance.CurrentProfile.LatestSave; // Assumes a helper property LatestSave is defined.
        if (latestSave == null)
        {
            Debug.LogError("No saved game state found.");
            return;
        }
        currentStage = StageRepo.instance.Get(latestSave.Stage.CurrentStage);
        RestartStage();
    }


    /// <summary>
    /// Loads the selected stage and initializes the first wave.
    /// </summary>
    public void RestartStage()
    {
        // Reset everything for a new stage.
        currentWave = ProfileRepo.instance.CurrentProfile.CurrentSave.Stage.CurrentWave;
        actorManager.Clear();
        dottedLineManager.Clear();
        supportLineManager.Clear();
        coinBar.Refresh();
        tileManager.Reset();
        turnManager.Initialize();

        // Assign persistent hero actors from ProfileRepo
        foreach (var heroActor in ProfileRepo.instance.CurrentProfile.CurrentSave.Party.HeroActors)
        {
            SpawnActor(new StageActor(heroActor, location: Random.UnoccupiedLocation));
        }

        //HACK: For some reason enemies might spawn on top of heroes because they aren't loaded at same time...
        //actors.ForEach(x => x.flags.HasSpawned = true);

        // Load the wave based on currentWave.
        if (currentStage.Waves.Count > 0)
        {
            LoadWave(currentWave);
        }
        else
        {
            Debug.LogError($"Stage {currentStage.Name} has no waves defined.");
        }

        StartCoroutine(fade.FadeIn());
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

        // Assign actors for this wave
        foreach (var stageActor in wave.Actors)
        {
            SpawnActor(new StageActor(stageActor, location: Random.UnoccupiedLocation));
        }

        // Assign dotted lines for this wave
        foreach (var stageDottedLine in wave.DottedLines)
        {
            var segment = stageDottedLine.Segment;
            var location = stageDottedLine.Location;
            dottedLineManager.Spawn(segment, location);
        }

        waveAnnouncement.Show(waveIndex + 1, currentStage.Waves.Count);
        Debug.Log($"Wave {waveIndex + 1} of {currentStage.Waves.Count} loaded.");
    }

    /// <summary>
    /// Spawns a new actor in the scene.
    /// </summary>
    public void SpawnActor(StageActor stageActor)
    {
        var prefab = Instantiate(actorPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<ActorInstance>();
        instance.transform.parent = board.transform;

        instance.characterName = stageActor.Character;
        instance.friendlyName = instance.characterName;
        instance.name = $"{stageActor.Character}_{Guid.NewGuid():N}";
        instance.team = stageActor.Team;

        // Assign stats based on characterName and stageActor's level
        instance.stats = ActorRepo.instance.Actors[stageActor.Character].GetStats(stageActor.Level);

        instance.transform.localScale = GameManager.instance.tileScale;
        instance.spawnTurn = stageActor.SpawnTurn;
        instance.Spawn(stageActor.Location.Value);

        actors.Add(instance);
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
        bool allEnemiesDead = enemies.All(x => x.flags.HasSpawned && x.isDead);
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
        IEnumerator loadNextStage()
        {
            var stageName = currentStage.NextStage;
            currentStage = StageRepo.instance.Get(stageName);
            RestartStage();
            yield return null;
        }

        StartCoroutine(fade.FadeOut(loadNextStage()));
    }

    /// <summary>
    /// Checks whether the game is over.
    /// </summary>
    private void CheckGameOver()
    {
        bool allPlayersDead = heroes.All(x => x.flags.HasSpawned && x.isDead);
        if (!allPlayersDead)
            return;

        IEnumerator reloadStage()
        {
            RestartStage();
            yield return null;
        }

        StartCoroutine(fade.FadeOut(reloadStage()));
    }


    /// <summary>
    /// Convenience method for adding a new enemy actor.
    /// </summary>
    /// <param name="character">Character type for the enemy.</param>
    public void AddEnemy(string character)
    {
        SpawnActor(new StageActor(character, Team.Enemy, location: Random.UnoccupiedLocation));
    }

}
