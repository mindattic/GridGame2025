using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Game.Behaviors;
using Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Quick Reference Properties:
    // These properties provide direct access to various core systems managed by the GameManager singleton.
    protected Fade fade => GameManager.instance.fade;                            // Handles fade in/out effects.
    protected DataManager dataManager => GameManager.instance.dataManager;          // Manages game data (e.g., stage data).
    protected ResourceManager resourceManager => GameManager.instance.resourceManager; // Provides access to assets such as tutorials.
    protected ProfileManager profileManager => GameManager.instance.profileManager; // Manages player profile data.
    public int totalCoins
    {
        get => GameManager.instance.totalCoins;
        set => GameManager.instance.totalCoins = value;
    }
    protected TurnManager turnManager => GameManager.instance.turnManager;          // Manages turn order and phase transitions.
    protected ActorManager actorManager => GameManager.instance.actorManager;       // Manages actor-specific logic.
    protected DottedLineManager dottedLineManager => GameManager.instance.dottedLineManager; // Handles dotted line rendering (e.g., movement paths).
    protected CoinBar coinBar => GameManager.instance.coinBar;                      // UI element displaying coin count.
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;      // UI overlay for stage-related transitions.
    protected BoardInstance board => GameManager.instance.board;                    // Represents the game board layout.
    protected TutorialPopup tutorialPopup => GameManager.instance.tutorialPopup;      // Displays tutorial popups.
    protected IQueryable<ActorInstance> players => GameManager.instance.players;      // Queryable collection of player actors.
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;      // Queryable collection of enemy actors.

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
        var stageName = profileManager.currentProfile.Stage.CurrentStageName;
        currentStage = dataManager.GetStage(stageName);
        LoadStage();
    }

    /// <summary>
    /// Loads the previous stage by iterating over all stages to find one whose NextStage property equals the current stage name.
    /// </summary>
    public void Previous()
    {
        // Iterate over all available stages.
        foreach (var stage in dataManager.Stages.Values)
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
        if (!string.IsNullOrEmpty(currentStage.NextStage) && dataManager.Stages.ContainsKey(currentStage.NextStage))
        {
            currentStage = dataManager.Stages[currentStage.NextStage];
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
        actors.Clear();
        coinBar.Refresh();
        actorManager.Clear();
        dottedLineManager.Clear();
        turnManager.Initialize();

        // The following canvasOverlay code is commented out but could be used for UI transitions.
        //canvasOverlay.Clear();
        //canvasOverlay.Show($"{currentStageName.Name}");
        //canvasOverlay.TriggerFadeOut(Interval.OneSecond);

        // Spawn actors defined in the stage data.
        foreach (var stageActor in currentStage.Actors)
        {
            // Convert string values from stage data into their respective types.
            var character = ConvertString.ToCharacter(stageActor.Character);
            var team = ConvertString.ToTeam(stageActor.Team);
            var spawnTurn = stageActor.SpawnTurn;
            var location = ConvertString.ToVector2Int(stageActor.Location);
            SpawnActor(character, team, spawnTurn, location);
        }

        // Spawn dotted lines if specified in the stage data.
        foreach (var stageDottedLine in currentStage.DottedLines)
        {
            var segment = ConvertString.ToDottedLineSegment(stageDottedLine.Segment);
            var location = ConvertString.ToVector2Int(stageDottedLine.Location);
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
        StartCoroutine(fade.FadeIn(showTutorial()));
    }

    /*
    // The following Update method is commented out.
    // It could be used to check if the stage is complete and log a message.
    private void Update()
    {
       if (currentStageName != null && currentStageName.IsStageComplete(this))
       {
           Debug.Log($"StageData '{currentStageName.Name}' Complete!");
       }
    }
    */

    /// <summary>
    /// Spawns a new actor by instantiating the actor prefab and initializing its properties
    /// based on the provided parameters. The actor is then added to the global actors list.
    /// </summary>
    /// <param name="character">Character type for the actor.</param>
    /// <param name="team">Team to which the actor belongs.</param>
    /// <param name="spawnTurn">Turn number at which the actor should spawn.</param>
    /// <param name="location">Initial grid location for the actor.</param>
    public void SpawnActor(
        Character character,
        Team team,
        int spawnTurn,
        Vector2Int location)
    {
        // Instantiate the actor prefab at (0,0) with no rotation.
        var prefab = Instantiate(actorPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<ActorInstance>();
        // Set the parent to the board to keep the actor within the game area.
        instance.transform.parent = board.transform;
        // Initialize actor properties.
        instance.character = character;
        instance.name = $"{character}_{Guid.NewGuid():N}"; // Ensure unique name.
        instance.team = team;
        instance.stats = dataManager.GetStats(character);
        instance.transform.localScale = GameManager.instance.tileScale;
        instance.spawnTurn = spawnTurn;

        // Determine the start location:
        // If spawnTurn is 1 or lower and a valid location is provided, use it.
        // Otherwise, assign a random unoccupied location.
        var startLocation = spawnTurn <= 1 && location != Location.Nowhere ? location : Random.UnoccupiedLocation;
        instance.Spawn(startLocation);
        // Add the new actor instance to the global actors list.
        actors.Add(instance);
    }

    /// <summary>
    /// Convenience method for adding a new enemy actor.
    /// </summary>
    /// <param name="character">Character type for the enemy.</param>
    public void AddEnemy(Character character)
    {
        SpawnActor(character, Team.Enemy, 0, Random.UnoccupiedLocation);
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
            currentStage = dataManager.GetStage(stageName);
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
