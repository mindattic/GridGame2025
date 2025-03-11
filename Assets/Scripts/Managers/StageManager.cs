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

    protected IEnumerable<ActorInstance> players => GameManager.instance.players;
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;




    // Access the list of actors from the GameManager.
    protected List<ActorInstance> actors
    {
        get => GameManager.instance.actors;
        set => GameManager.instance.actors = value;
    }

    // Internal property:
    // Gets the number of enemy actors currently managed.
    public int enemyCount => actors.FindAll(x => x.isEnemy).Count;

    // Fields:
    [SerializeField] public GameObject actorPrefab;  // Prefab used for instantiating actor objects.
    public StageData currentStage;                    // Data for the current stage.

    /// <summary>
    /// Initializes the StageManager by retrieving the stage name from the player's profile,
    /// loading the corresponding StageData, and then loading the stage.
    /// </summary>
    public void Initialize()
    {
        var stageName = ProfileStore.instance.current.Stage.CurrentStageName;
        currentStage = DataStore.instance.GetStage(stageName);
        LoadStage();
    }

    /// <summary>
    /// Loads the previous stage by iterating over all stages to find one whose NextStage property equals the current stage name.
    /// </summary>
    public void Previous()
    {
        // Iterate over all available stages.
        foreach (var stage in DataStore.instance.Stages.Values)
        {
            // Identify the stage that lists the current stage as its next stage.
            if (stage.NextStage == currentStage.Name)
            {
                currentStage = stage;
                LoadStage();
                return;
            }
        }
    }

    /// <summary>
    /// Loads the next stage if available by checking the current stage's NextStage property.
    /// </summary>
    public void Next()
    {
        // Ensure that a valid next stage exists.
        if (!string.IsNullOrEmpty(currentStage.NextStage) && DataStore.instance.Stages.ContainsKey(currentStage.NextStage))
        {
            currentStage = DataStore.instance.Stages[currentStage.NextStage];
            LoadStage();
        }
    }

    /// <summary>
    /// Loads the current stage by:
    /// - Clearing previous game elements (actors, coin bar, dotted lines, etc.)
    /// - Initializing the turn manager.
    /// - Spawning actors and dotted lines as defined in the stage data.
    /// - Displaying any relevant tutorial.
    /// </summary>
    public void LoadStage()
    {
        // Clear existing elements to prepare for a new stage.      
        actorManager.Clear();
        dottedLineManager.Clear();
        supportLineManager.Clear();
        coinBar.Refresh();
        tileManager.Reset();
        turnManager.Initialize();

        // The following canvasOverlay code is commented out but could be used for UI transitions.
        //canvasOverlay.Reset();
        //canvasOverlay.Show($"{currentStage.Name}");
        //canvasOverlay.TriggerFadeOut(Interval.OneSecond);

        // Spawn actors defined in the stage data.
        foreach (var stageActor in currentStage.Actors)
        {
            SpawnActor(new StageActor(stageActor));
        }

        // Spawn dotted lines if specified in the stage data.
        foreach (var stageDottedLine in currentStage.DottedLines)
        {
            var segment = stageDottedLine.Segment;
            var location = stageDottedLine.Location;
            dottedLineManager.Spawn(segment, location);
        }

        // Show the first tutorial for the stage if available.
        IEnumerator showTutorial()
        {
            var tutorialKey = currentStage.Tutorials.FirstOrDefault();
            var tutorial = resourceManager.Tutorial(tutorialKey);
            tutorialPopup.Load(tutorial);
            yield return null;
        }

        // Start a fade-in effect, and once the fade completes, display the tutorial.
        //StartCoroutine(fade.FadeIn(showTutorial()));
        StartCoroutine(fade.FadeIn());
    }


    /// <summary>
    /// Spawns a new actor by instantiating the actor prefab and initializing its properties
    /// based on the provided parameters. The actor is then added to the global actors list.
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
        instance.stats = DataStore.instance.GetStats(stageActor.Character);
        instance.transform.localScale = GameManager.instance.tileScale;
        instance.spawnTurn = stageActor.SpawnTurn;
        instance.Spawn(stageActor.Location.Value);

        //Add the new actor _instance to the global actors list.
        actors.Add(instance);
    }

    /// <summary>
    /// Convenience method for adding a new enemy actor.
    /// </summary>
    /// <param name="character">Character type for the enemy.</param>
    public void AddEnemy(Character character)
    {
        SpawnActor(new StageActor(character, Team.Enemy));
    }

    /// <summary>
    /// Called when an actor dies. Triggers checks for game over or stage completion.
    /// </summary>
    public void OnActorDeath()
    {
        CheckGameOver();
        CheckStageCompletion();
    }

    /// <summary>
    /// Checks whether the stage is complete by ensuring all enemy actors have spawned and are dead.
    /// If the stage is complete, initiates a fade-out transition and loads the next stage.
    /// </summary>
    private void CheckStageCompletion()
    {
        // Verify that all enemy actors (that have spawned) are dead.
        bool allEnemiesDead = enemies.All(x => x.flags.HasSpawned && x.isDead);
        if (!allEnemiesDead)
            return;

        // Coroutine to load the next stage.
        IEnumerator loadNextStage()
        {
            var stageName = currentStage.NextStage;
            currentStage = DataStore.instance.GetStage(stageName);
            LoadStage();
            yield return null;
        }

        // Start a fade-out effect before loading the next stage.
        StartCoroutine(fade.FadeOut(loadNextStage()));
    }

    /// <summary>
    /// Checks whether the game is over by verifying that all player actors have spawned and are dead.
    /// If the game is over, initiates a fade-out transition and reloads the current stage.
    /// </summary>
    private void CheckGameOver()
    {
        bool allPlayersDead = players.All(x => x.flags.HasSpawned && x.isDead);
        if (!allPlayersDead)
            return;

        // Coroutine to reload the stage.
        IEnumerator reloadStage()
        {
            LoadStage();
            yield return null;
        }

        // Start a fade-out effect before reloading the stage.
        StartCoroutine(fade.FadeOut(reloadStage()));
    }
}
