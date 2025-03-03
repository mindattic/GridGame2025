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
    //Quick Reference Properties
    protected Fade fade => GameManager.instance.fade;
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
    protected CoinBar coinBar => GameManager.instance.coinBar;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    protected BoardInstance board => GameManager.instance.board;
    protected TutorialPopup tutorialPopup => GameManager.instance.tutorialPopup;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;


    protected List<ActorInstance> actors
    {
        get => GameManager.instance.actors;
        set => GameManager.instance.actors = value;
    }

    //Internal properties
    public int enemyCount => actors.FindAll(x => x.isEnemy).Count;

    //Fields
    [SerializeField] public GameObject actorPrefab;
    public StageData currentStage;

    public void Initialize()
    {
        var stageName = profileManager.currentProfile.Stage.CurrentStageName;
        currentStage = dataManager.GetStage(stageName);
        LoadStage();
    }
    public void Previous()
    {
        // Find the previous stage by iterating over the dictionary
        foreach (var stage in dataManager.Stages.Values)
        {
            if (stage.NextStage == currentStage.Name) // Find which stage points to the current one
            {
                currentStage = stage;
                LoadStage();
                return;
            }
        }
    }

    public void Next()
    {
        if (!string.IsNullOrEmpty(currentStage.NextStage) && dataManager.Stages.ContainsKey(currentStage.NextStage))
        {
            currentStage = dataManager.Stages[currentStage.NextStage];
            LoadStage();
        }
    }

    public void LoadStage()
    {
        //Clear game elements
        actors.Clear();
        coinBar.Refresh();
        actorManager.Clear();
        dottedLineManager.Clear();
        turnManager.Initialize();

        //canvasOverlay.Clear();
        //canvasOverlay.Show($"{currentStageName.Name}");
        //canvasOverlay.TriggerFadeOut(Interval.OneSecond);

        //Spawn actors
        foreach (var stageActor in currentStage.Actors)
        {
            var character = ConvertString.ToCharacter(stageActor.Character);
            var team = ConvertString.ToTeam(stageActor.Team);
            var spawnTurn = stageActor.SpawnTurn;
            var location = ConvertString.ToVector2Int(stageActor.Location);
            SpawnActor(character, team, spawnTurn, location);
        }

        //Spawn dotted lines (if applicable)
        foreach (var stageDottedLine in currentStage.DottedLines)
        {
            var segment = ConvertString.ToDottedLineSegment(stageDottedLine.Segment);
            var location = ConvertString.ToVector2Int(stageDottedLine.Location);
            dottedLineManager.Spawn(segment, location);
        }

        //Show first tutorial (if applicable)
        IEnumerator showTutorial()
        {
            var tutorialKey = currentStage.Tutorials.FirstOrDefault();
            var tutorial = resourceManager.Tutorial(tutorialKey);
            tutorialPopup.Load(tutorial);
            yield return null;
        }
       

        StartCoroutine(fade.FadeIn(showTutorial()));
    }

    //private void Update()
    //{
    //   if (currentStageName != null && currentStageName.IsStageComplete(this))
    //   {
    //       Debug.Log($"StageData '{currentStageName.Name}' Complete!");
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
        var startLocation = spawnTurn <= 1 && location != Location.Nowhere ? location : Random.UnoccupiedLocation;
        instance.Spawn(startLocation);
        actors.Add(instance);
    }

    public void AddEnemy(Character character)
    {
        SpawnActor(character, Team.Enemy, 0, Random.UnoccupiedLocation);
    }



    public void OnActorDeath()
    {
        CheckGameOver();
        CheckStageCompletion();
    }

    private void CheckStageCompletion()
    {
        bool allEnemiesDead = enemies.All(x => x.flags.HasSpawned && x.isDead);
        if (!allEnemiesDead)
            return;

        IEnumerator loadNextStage()
        {
            var stageName = currentStage.NextStage;
            currentStage = dataManager.GetStage(stageName);
            LoadStage();
            yield return null;
        }

        StartCoroutine(fade.FadeOut(loadNextStage()));
    }

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

}

