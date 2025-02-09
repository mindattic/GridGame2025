using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    //External properties
    protected DataManager dataManager => GameManager.instance.dataManager;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected ProfileManager profileManager => GameManager.instance.profileManager;
    public int totalCoins
    {
        get => GameManager.instance.totalCoins;
        set => GameManager.instance.totalCoins = value;
    }

    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected DottedLineManager dottedLineManager => GameManager.instance.dottedLineManager;
    protected CoinBarInstance coinBar => GameManager.instance.coinBar;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    protected BoardInstance board => GameManager.instance.board;
    protected TutorialPopup tutorialPopup => GameManager.instance.tutorialPopup;
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;


    protected List<ActorInstance> actors
    {
        get => GameManager.instance.actors;
        set => GameManager.instance.actors = value;
    }

    //Internal properties
    public int enemyCount => actors.FindAll(x => x.isEnemy).Count;

    //Fields

    [SerializeField] GameObject actorPrefab;
    public StageData currentStage;

    public void Initialize()
    {
        LoadStage(profileManager.currentStage);
    }


    public void Reload()
    {
        LoadStage(currentStage.Name);
    }

    public void Previous()
    {
        var currentIndex = dataManager.Stages.FindIndex(x => x.Name == currentStage.Name);
        if (currentIndex > 0)
            currentStage = dataManager.Stages[currentIndex - 1];

        Reload();
    }

    public void Next()
    {
        var currentIndex = dataManager.Stages.FindIndex(x => x.Name == currentStage.Name);
        if (currentIndex >= 0 && currentIndex < dataManager.Stages.Count - 1)
            currentStage = dataManager.Stages[currentIndex + 1];

        Reload();
    }


    public void LoadStage(string name)
    {
        currentStage = dataManager.GetStage(name);

        //Reset game elements
        actors.Clear();
        coinBar.Refresh();
        actorManager.Clear();
        dottedLineManager.Clear();
        turnManager.Initialize();
        canvasOverlay.Reset();
        canvasOverlay.Show($"{currentStage.Name}");
        canvasOverlay.TriggerFadeOut(Interval.OneSecond);

        //Spawn actors
        foreach (var stageActor in currentStage.Actors)
        {
            var character = Convert.ToCharacter(stageActor.Character);
            var team = Convert.ToTeam(stageActor.Team);
            var spawnTurn = stageActor.SpawnTurn;
            var location = Convert.ToVector2Int(stageActor.Location);
            SpawnActor(character, team, spawnTurn, location);
        }

        //Spawn dotted lines (if applicable)
        foreach (var stageDottedLine in currentStage.DottedLines)
        {
            var segment = Convert.ToDottedLineSegment(stageDottedLine.Segment);
            var location = Convert.ToVector2Int(stageDottedLine.Location);
            dottedLineManager.Spawn(segment, location);
        }

        //Show first tutorial (if applicable)
        var tutorialKey = currentStage.Tutorials.FirstOrDefault();
        var tutorial = resourceManager.Tutorial(tutorialKey);
        tutorialPopup.Load(tutorial);
    }

    //private void Update()
    //{
    //   if (currentStage != null && currentStage.IsStageComplete(this))
    //   {
    //       Debug.Log($"StageData '{currentStage.Name}' Complete!");
    //   }
    //}

    public void SpawnActor(
        Character character, 
        Team team, 
        int spawnTurn, 
        Vector2Int location)
    {
        var prefab = Instantiate(actorPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<ActorInstance>();
        instance.transform.parent = board.transform;
        instance.character = character;
        instance.name = $"{character}_{Guid.NewGuid():N}";
        instance.team = team;
        instance.stats = dataManager.GetStats(character);
        instance.transform.localScale = GameManager.instance.tileScale;
        instance.spawnTurn = spawnTurn;

        //TODO: This should probably be cleaned up...
        var startLocation = spawnTurn <= 1 && location != board.NowhereLocation ? location : Random.UnoccupiedLocation;
        instance.Spawn(startLocation);
        actors.Add(instance);
    }

    public void AddEnemy(Character character)
    {
        SpawnActor(character, Team.Enemy, 0, Random.UnoccupiedLocation);
    }


    public void CheckStageCompletion(ActorInstance actor)
    {
        bool allEnemiesDefeated = enemies.All(x => x.hasSpawned && x.isDead);

        if (allEnemiesDefeated)
        {
            LoadStage(currentStage.NextStage);
        }
    }
}

