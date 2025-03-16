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
    protected Fade fade => GameManager.instance.fade;
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

    protected IEnumerable<ActorInstance> players => GameManager.instance.players;
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
    private int currentWaveIndex = 0; // Track the current wave

    /// <summary>
    /// Initializes the StageManager by retrieving the stage name from the player's profile,
    /// loading the corresponding Stage, and then loading the stage.
    /// </summary>
    public void Initialize()
    {
        var stageName = ProfileStore.instance.CurrentProfile.LatestSave.Stage.CurrentStageName;
        currentStage = StageStore.instance.GetStage(stageName);
        currentWaveIndex = 0;
        LoadStage();
    }

    /// <summary>
    /// Loads the selected stage and initializes the first wave.
    /// </summary>
    public void LoadStage()
    {
        // Reset everything for a new stage.
        actorManager.Clear();
        dottedLineManager.Clear();
        supportLineManager.Clear();
        coinBar.Refresh();
        tileManager.Reset();
        turnManager.Initialize();

        currentWaveIndex = 0;

        // Spawn persistent player actors from ProfileStore
        foreach (var playerActor in ProfileStore.instance.PlayerActors)
        {
            SpawnActor(new StageActor(playerActor));
        }


        // Load the first wave
        if (currentStage.Waves.Count > 0)
        {
            LoadWave(currentWaveIndex);
        }
        else
        {
            Debug.LogError($"Stage {currentStage.Name} has no waves defined.");
        }

        // Start fade-in effect
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

        // Spawn actors for this wave
        foreach (var stageActor in wave.Actors)
        {
            SpawnActor(new StageActor(stageActor));
        }

        // Spawn dotted lines for this wave
        foreach (var stageDottedLine in wave.DottedLines)
        {
            var segment = stageDottedLine.Segment;
            var location = stageDottedLine.Location;
            dottedLineManager.Spawn(segment, location);
        }

        waveAnnouncement.ShowWave(waveIndex + 1, currentStage.Waves.Count);
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
        instance.character = stageActor.Character;
        instance.friendlyName = instance.character.ToString().Split("_instance")[0];
        instance.name = $"{stageActor.Character}_instance{Guid.NewGuid():N}";
        instance.team = stageActor.Team;
        instance.stats = ActorStore.instance.GetStats(stageActor.Character);
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

        currentWaveIndex++;

        if (currentWaveIndex < currentStage.Waves.Count)
        {
            Debug.Log($"All enemies defeated. Loading next wave: {currentWaveIndex + 1}");
            LoadWave(currentWaveIndex);
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
            currentStage = StageStore.instance.GetStage(stageName);
            LoadStage();
            yield return null;
        }

        StartCoroutine(fade.FadeOut(loadNextStage()));
    }

    /// <summary>
    /// Checks whether the game is over.
    /// </summary>
    private void CheckGameOver()
    {
        bool allPlayersDead = players.All(x => x.flags.HasSpawned && x.isDead);
        if (!allPlayersDead)
            return;

        IEnumerator reloadStage()
        {
            LoadStage();
            yield return null;
        }

        StartCoroutine(fade.FadeOut(reloadStage()));
    }


    /// <summary>
    /// Convenience method for adding a new enemy actor.
    /// </summary>
    /// <param name="character">Character type for the enemy.</param>
    public void AddEnemy(Character character)
    {
        SpawnActor(new StageActor(character, Team.Enemy));
    }

}
