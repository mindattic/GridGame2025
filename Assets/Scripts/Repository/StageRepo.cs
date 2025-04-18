using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "StageRepo", menuName = "Repositories/StageRepo")]
public class StageRepo : ScriptableObject
{
    // Singleton instance
    private static StageRepo Instance;

    public static StageRepo instance
    {
        get
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<StageRepo>("Repositories/StageRepo");
                handle.WaitForCompletion(); // Block until the asset is loaded
                Instance = handle.Result;
            }

            if (Instance == null)
                Debug.LogError("StageRepo accessed before being initialized!");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static async void AutoInitialize()
    //{
    //    if (Instance == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<StageRepo>("Repositories/StageRepo");
    //        Instance = await handle.Task;

    //        if (Instance == null)
    //            Debug.LogError("StageRepo asset not found in Addressables with key 'Repositories/StageRepo'");
    //    }
    //}

    // Synchronous fallback for loading the StageRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<StageRepo>("Repositories/StageRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    Instance = handle.Result;

    //    if (Instance == null)
    //        Debug.LogError("Failed to load StageRepo synchronously from Addressables.");
    //}

    //Serialized fields
    [SerializeField] public Dictionary<string, Stage> Stages;


    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Stages = new Dictionary<string, Stage>
    {
        { "Stage 1", new Stage
            {
                Name = "Stage 1",
                Description = "Intro Battle",
                CompletionCondition = "DefeatAllEnemies",
                CompletionValue = 0,
                NextStage = "Stage 2",
                //Tutorials = new List<string> { "Tutorial1" },
                Waves = new List<StageWave>()
                {
                    new StageWave()
                    {
                        Actors = new List<StageActor>()
                        {
                            new StageActor { Character = CharacterHelper.Slime, Team = Team.Enemy, Location = new Vector2Int(5, 6) }
                        },
                        DottedLines = new List<StageDottedLine>
                        {
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 3) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 4) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 5) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 6) },
                            new StageDottedLine { Segment = DottedLineSegment.TurnBottomRight, Location = new Vector2Int(2, 2) },
                            new StageDottedLine { Segment = DottedLineSegment.Horizontal, Location = new Vector2Int(3, 2) },
                            new StageDottedLine { Segment = DottedLineSegment.Horizontal, Location = new Vector2Int(4, 2) },
                            new StageDottedLine { Segment = DottedLineSegment.TurnBottomLeft, Location = new Vector2Int(5, 2) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 3) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 4) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 5) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 6) },
                            new StageDottedLine { Segment = DottedLineSegment.TurnTopRight, Location = new Vector2Int(5, 7) },
                            new StageDottedLine { Segment = DottedLineSegment.TurnTopLeft, Location = new Vector2Int(6, 7) },
                            new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(6, 6) },
                            new StageDottedLine { Segment = DottedLineSegment.ArrowUp, Location = new Vector2Int(6, 5) }
                        },
                    },
                    new StageWave()
                    {
                        Actors = new List<StageActor>()
                        {
                            new StageActor { Character = CharacterHelper.Slime, Team = Team.Enemy },
                            new StageActor { Character = CharacterHelper.Slime, Team = Team.Enemy },
                            new StageActor { Character = CharacterHelper.Slime, Team = Team.Enemy }
                        },
                    },
                    new StageWave()
                    {
                        Actors = new List<StageActor>()
                        {
                            new StageActor { Character = CharacterHelper.Slime, Team = Team.Enemy },
                            new StageActor { Character = CharacterHelper.Bat, Team = Team.Enemy },
                            new StageActor { Character = CharacterHelper.Bat, Team = Team.Enemy },
                            new StageActor { Character = CharacterHelper.Bat, Team = Team.Enemy, SpawnTurn = 2 },
                            new StageActor { Character = CharacterHelper.Bat, Team = Team.Enemy, SpawnTurn = 3 },
                            new StageActor { Character = CharacterHelper.Bat, Team = Team.Enemy, SpawnTurn = 4 },
                        },
                    },
                }
            }
        },
        { "Stage 2", new Stage
            {
                Name = "Stage 2",
                Description = "DefeatAllEnemies",
                CompletionCondition = "DefeatAllEnemies",
                CompletionValue = 0,
                NextStage = "Stage 3",
                Waves = GenerateWaves(4, new List<string> { CharacterHelper.Slime, CharacterHelper.Scorpion, CharacterHelper.Bat })
            }
        },
        { "Stage 3", new Stage
            {
                Name = "Stage 3",
                Description = "DefeatAllEnemies",
                CompletionCondition = "DefeatAllEnemies",
                CompletionValue = 0,
                NextStage = "Stage 4",
                Waves = GenerateWaves(5, new List<string> { CharacterHelper.Slime, CharacterHelper.Yeti, CharacterHelper.Scorpion, CharacterHelper.Bat })
            }
        },
        { "Stage 4", new Stage
            {
                Name = "Stage 4",
                Description = "DefeatAllEnemies",
                CompletionCondition = "DefeatAllEnemies",
                CompletionValue = 0,
                NextStage = "Stage 5",
                Waves = GenerateWaves(3, new List<string> { CharacterHelper.Yeti, CharacterHelper.Slime, CharacterHelper.Bat, CharacterHelper.Scorpion })
            }
        },
        { "Stage 5", new Stage
            {
                Name = "Stage 5",
                Description = "DefeatAllEnemies",
                CompletionCondition = "DefeatAllEnemies",
                CompletionValue = 0,
                NextStage = "Stage 6",
                Waves = GenerateWaves(5, new List<string> { CharacterHelper.Yeti, CharacterHelper.Slime, CharacterHelper.Scorpion, CharacterHelper.Bat })
            }
        }
    };
    }

    /// <summary>
    /// Generates random waves of enemies for a stage.
    /// </summary>
    /// <param name="waveCount">Index of waves.</param>
    /// <param name="possibleEnemies">List of enemy types to use in waves.</param>
    /// <returns>A list of StageWave.</returns>
    private List<StageWave> GenerateWaves(int waveCount, List<string> possibleEnemies)
    {
        //DEBUG: Manually enter heros in here for now, should be stored in ProfileRepo




        List<StageWave> waves = new List<StageWave>();
        System.Random rng = new System.Random();

        for (int i = 0; i < waveCount; i++)
        {
            StageWave wave = new StageWave
            {
                WaveID = i + 1,
                Actors = new List<StageActor>(),
                DottedLines = new List<StageDottedLine>()
            };

            // Generate 2-5 random enemies per wave
            int enemyCount = rng.Next(2, 6);
            for (int j = 0; j < enemyCount; j++)
            {
                string randomEnemy = possibleEnemies[rng.Next(possibleEnemies.Count)];
                wave.Actors.Add(new StageActor
                {
                    Character = randomEnemy,
                    Team = Team.Enemy,
                    Location = new Vector2Int(rng.Next(1, 6), rng.Next(1, 6))
                });
            }

            waves.Add(wave);
        }

        return waves;
    }

  

    public Stage Get(string name)
    {
        var data = Stages[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve stage for `{name}`");

        return new Stage(data); //Return a new copy instead of a shared reference
    }

}
