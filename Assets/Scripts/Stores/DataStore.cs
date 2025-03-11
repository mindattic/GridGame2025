using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataStore", menuName = "Stores/DataStore")]
public class DataStore : ScriptableObject
{
    //Singleton
    private static DataStore _instance;

    public static DataStore instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("DataStore accessed before being initialized! Ensure it's assigned in Awake().");
            return _instance;
        }
    }

    //Initialize
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<DataStore>("Stores/DataStore");
            if (_instance == null)
                Debug.LogError("DataStore asset not found in Resources/Stores/DataStore");
        }
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, ActorData> Actors;
    [SerializeField] public Dictionary<string, StageData> Stages;
    [SerializeField] public Dictionary<string, TrailResource> TrailEffects;
    [SerializeField] public Dictionary<string, Tutorial> Tutorials;
    [SerializeField] public Dictionary<string, VFXData> VisualEffects;

    private void OnEnable()
    {
        LoadActors();
        LoadStages();
        LoadTrailEffects();
        LoadTutorials();
        LoadVisualEffects();
    }

    private void LoadActors()
    {
        Actors = new Dictionary<string, ActorData>
        {
            { "Barbarian", new ActorData
            {
                Character = Character.Barbarian,
                Description = "A warrior driven by rage.",
                Stats = new ActorStats
                {
                    Level = 1, PreviousHP = 40, HP = 40, MaxHP = 40,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 9, Vitality = 6, Agility = 4, Speed = 5, Luck = 5
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = -60, OffsetY = -80, Width = 256, Height = 256 },
                Details = new ActorDetails
                {
                    Description = "A warrior driven by rage.",
                    Card = "Gains <color=#FF0000>[Rage]</color> when attacking or being attacked. Will eventually go <color=#FF0000>[Berserk]</color> and attack multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Bat", new ActorData
            {
                Character = Character.Bat,
                Description = "A flying menace.",
                Stats = new ActorStats
                {
                    Level = 1, PreviousHP = 5, HP = 5, MaxHP = 5,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 2, Vitality = 4, Agility = 1, Speed = 1, Luck = 6
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = 0, OffsetY = 0, Width = 1024, Height = 1024 },
                Details = new ActorDetails
                {
                    Description = "A warrior driven by rage.",
                    Card = "Intermittently goes <color=#FF0000>[Berserk]</color> attacking multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Cleric", new ActorData
            {
                Character = Character.Cleric,
                Description = "A strict adherent to the church.",
                Stats = new ActorStats
                {
                    Level = 1, PreviousHP = 30, HP = 30, MaxHP = 30,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 2, Vitality = 5, Agility = 3, Speed = 3, Luck = 9
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = 0, OffsetY = -50, Width = 326, Height = 316 },
                Details = new ActorDetails
                {
                    Description = "An adherent to the Lightbearer Orthodoxy.",
                    Card = "Casts <color=#00FF00>[Cure]</color> when supporting an attacker.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Ninja", new ActorData
            {
                Character = Character.Ninja,
                Description = "A stealthy assassin.",
                Stats = new ActorStats
                {
                    Level = 1, PreviousHP = 35, HP = 35, MaxHP = 35,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 4, Vitality = 4, Agility = 10, Speed = 10, Luck = 5
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = 0, OffsetY = 0, Width = 196, Height = 196 },
                Details = new ActorDetails
                {
                    Description = "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                    Card = "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy results",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Paladin", new ActorData
            {
                Character = Character.Paladin,
                Description = "A holy knight.",
                Stats = new ActorStats
                {
                    Level = 1, PreviousHP = 50, HP = 50, MaxHP = 50,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 6, Vitality = 8, Agility = 3, Speed = 3, Luck = 6
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = 20, OffsetY = -30, Width = 196, Height = 196 },
                Details = new ActorDetails
                {
                    Description = "A holy knight honor bound to defend the Lightbearer Orthodoxy.",
                    Card = "Intermittently uses <color=#FF0000>[Taunt]</color> to force enemies to focus their results.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Scorpion", new ActorData
            {
                Character = Character.Scorpion,
                Description = "A poisonous insectile predator.",
                Stats = new ActorStats
                {
                    Level = 2, PreviousHP = 100, HP = 100, MaxHP = 100,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 4, Vitality = 2, Agility = 1, Speed = 4, Luck = 2
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = -125, OffsetY = -300, Width = 256, Height = 256 },
                Details = new ActorDetails
                {
                    Description = "A poisonous insectile predator.",
                    Card = "Attacks have chance of inflicting <color=#00FF00>[Poison]</color>",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Slime", new ActorData
            {
                Character = Character.Slime,
                Description = "A weak and squishy creature.",
                Stats = new ActorStats
                {
                    Level = 1, PreviousHP = 8, HP = 8, MaxHP = 8,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 1, Vitality = 1, Agility = 1, Speed = 1, Luck = 1
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = -150, OffsetY = -300, Width = 512, Height = 512 },
                Details = new ActorDetails
                {
                    Description = "A weak and squishy creature.",
                    Card = "The most common denizen of the dark; neither good nor evil, a monster still has to eat.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            { "Yeti", new ActorData
            {
                Character = Character.Yeti,
                Description = "A large humanoid underdweller.",
                Stats = new ActorStats
                {
                    Level = 5, PreviousHP = 50, HP = 50, MaxHP = 50,
                    PreviousAP = 0, AP = 0, MaxAP = 100,
                    Strength = 4, Vitality = 10, Agility = 1, Speed = 1, Luck = 5
                },
                ThumbnailSettings = new ThumbnailSettings { OffsetX = -150, OffsetY = -100, Width = 256, Height = 256 },
                Details = new ActorDetails
                {
                    Description = "A large humanoid underdweller.",
                    Card = "Intermittently goes <color=#FF0000>[Berserk]</color> attacking multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            }
        };
    }

    private void LoadStages()
    {
        Stages = new Dictionary<string, StageData>
        {
            { "Stage 1", new StageData
                {
                    Name = "Stage 1",
                    Description = "Intro Battle",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 2",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player, Location = new Vector2Int(2, 7) },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, Location = new Vector2Int(5, 6) },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player, Location = new Vector2Int(4, 5) }
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
                    Tutorials = new List<string> { "Tutorial1" }
                }
            },
            { "Stage 2", new StageData
                {
                    Name = "Stage 2",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 3",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 2 },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 3 },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 4 },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 6 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 3", new StageData
                {
                    Name = "Stage 3",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 4",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Scorpion, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 4", new StageData
                {
                    Name = "Stage 4",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 5",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 5", new StageData
                {
                    Name = "Stage 5",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 6",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 6", new StageData
                {
                    Name = "Stage 6",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 7",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 7", new StageData
                {
                    Name = "Stage 7",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 8",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 8", new StageData
                {
                    Name = "Stage 8",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 9",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 9", new StageData
                {
                    Name = "Stage 9",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 10",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 10", new StageData
                {
                    Name = "Stage 10",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 11",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 11", new StageData
                {
                    Name = "Stage 11",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 12",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 12", new StageData
                {
                    Name = "Stage 12",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 13",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 13", new StageData
                {
                    Name = "Stage 13",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 14",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 14", new StageData
                {
                    Name = "Stage 14",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 15",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 15", new StageData
                {
                    Name = "Stage 15",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 16",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 16", new StageData
                {
                    Name = "Stage 16",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 17",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 17", new StageData
                {
                    Name = "Stage 17",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 18",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 18", new StageData
                {
                    Name = "Stage 18",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 19",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 19", new StageData
                {
                    Name = "Stage 19",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 20",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 20", new StageData
                {
                    Name = "Stage 20",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 21",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 21", new StageData
                {
                    Name = "Stage 21",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 22",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 22", new StageData
                {
                    Name = "Stage 22",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 23",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 23", new StageData
                {
                    Name = "Stage 23",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 24",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 24", new StageData
                {
                    Name = "Stage 24",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 25",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
            { "Stage 25", new StageData
                {
                    Name = "Stage 25",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 1",
                    Actors = new List<StageActor>
                    {
                        new StageActor { Character = Character.Paladin, Team = Team.Player },
                        new StageActor { Character = Character.Barbarian, Team = Team.Player },
                        new StageActor { Character = Character.Cleric, Team = Team.Player },
                        new StageActor { Character = Character.Ninja, Team = Team.Player },
                        new StageActor { Character = Character.Yeti, Team = Team.Enemy },
                        new StageActor { Character = Character.Bat, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy },
                        new StageActor { Character = Character.Slime, Team = Team.Enemy, SpawnTurn = 5 }
                    },
                    DottedLines = new List<StageDottedLine>(),
                    Tutorials = new List<string>()
                }
            },
        };
    }

    private void LoadTrailEffects()
    {
        TrailEffects = new Dictionary<string, TrailResource>
    {
        { "BlueGlow", new TrailResource
            {
                Name = "BlueGlow",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "Bubble", new TrailResource
            {
                Name = "Bubble",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "Feather", new TrailResource
            {
                Name = "Feather",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "Fireball", new TrailResource
            {
                Name = "Fireball",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.5f, 0.5f, 0.5f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "Flame", new TrailResource
            {
                Name = "Flame",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "GoldSparkle", new TrailResource
            {
                Name = "GoldSparkle",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(2.5f, 2.5f, 2.5f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "GreenSparkle", new TrailResource
            {
                Name = "GreenSparkle",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(2.5f, 2.5f, 2.5f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "IceSparkle", new TrailResource
            {
                Name = "IceSparkle",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "PinkDust", new TrailResource
            {
                Name = "PinkDust",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "RosePetal", new TrailResource
            {
                Name = "RosePetal",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        },
        { "StarSparkle", new TrailResource
            {
                Name = "StarSparkle",
                Prefab = null,
                RelativeOffset = new Vector3(0, 0, 0),
                AngularRotation = new Vector3(0, 0, 0),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = true
            }
        }
    };

    }

    private void LoadTutorials()
    {
        Tutorials = new Dictionary<string, Tutorial>
    {
        { "Tutorial1", new Tutorial
            {
                Key = "Tutorial1",
                Pages = new List<TutorialPage>
                {
                    new TutorialPage { TextureKey = "Tutorial.1-1", Title = "Tutorial 1-1", Content = "This is the first page of the tutorial." },
                    new TutorialPage { TextureKey = "Tutorial.1-2", Title = "Tutorial 1-2", Content = "This is the second page of the tutorial." },
                    new TutorialPage { TextureKey = "Tutorial.1-3", Title = "Tutorial 1-3", Content = "This is the third page of the tutorial." }
                }
            }
        }
    };

    }

    private void LoadVisualEffects()
    {
        VisualEffects = new Dictionary<string, VFXData>
        {
            { "AcidSplash", new VFXData
                {
                Name = "AcidSplash",
                RelativeOffset = new Vector3(0f, 0.01f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "AirSlash", new VFXData
                {
                Name = "AirSlash",
                RelativeOffset = new Vector3(0.01f, -0.15f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.15f, 0.15f, 0.15f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BloodClaw", new VFXData
            {
                Name = "BloodClaw",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.15f, 0.15f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSlash1", new VFXData
                {
                Name = "BlueSlash1",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0.12f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSlash2", new VFXData
                {
                Name = "BlueSlash2",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSlash3", new VFXData
                {
                Name = "BlueSlash3",
                RelativeOffset = new Vector3(0.02f, -0.15f, 0f),
                AngularRotation = new Vector3(30f, 30f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSword", new VFXData
                {
                Name = "BlueSword",
                RelativeOffset = new Vector3(0f, 0.05f, 0f),
                AngularRotation = new Vector3(30f, 30f, 0f),
                RelativeScale = new Vector3(0.12f, 0.08f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSword4X", new VFXData
                {
                Name = "BlueSword4X",
                RelativeOffset = new Vector3(-0.05f, -0.1f, 0f),
                AngularRotation = new Vector3(30f, 30f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "BlueYellowSword", new VFXData
                {
                Name = "BlueYellowSword",
                RelativeOffset = new Vector3(0.03f, 0.01f, 0f),
                AngularRotation = new Vector3(60f, 0f, 0f),
                RelativeScale = new Vector3(0.07f, 0.07f, 0.07f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
             }
            },
            { "BlueYellowSword3X", new VFXData
                {
                Name = "BlueYellowSword3X",
                RelativeOffset = new Vector3(0.02f, -0.05f, 0f),
                AngularRotation = new Vector3(60f, 0f, 0f),
                RelativeScale = new Vector3(0.07f, 0.07f, 0.07f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BuffLife", new VFXData
                {
                Name = "BuffLife",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.16f, 0.16f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "DoubleClaw", new VFXData
                {
                Name = "DoubleClaw",
                RelativeOffset = new Vector3(-0.03f, -0.1f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.12f, 0.12f, 0f),
                Delay = 0.22f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "FireRain", new VFXData
                {
                Name = "FireRain",
                RelativeOffset = new Vector3(0.03f, -0.05f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                Delay = 0f,
                Duration = 4f,
                IsLoop = false
                }
            },
            { "GodRays", new VFXData
                {
                Name = "GodRays",
                RelativeOffset = new Vector3(0f, -0.25f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.07f, 0.07f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "GoldBuff", new VFXData
                {
                Name = "GoldBuff",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "GreenBuff", new VFXData
                {
                Name = "GreenBuff",
                RelativeOffset = new Vector3(0.02f, -0.25f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "HexShield", new VFXData
                {
                Name = "HexShield",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.16f, 0.16f, 0.16f),
                Delay = 0f,
                Duration = 6f,
                IsLoop = false
                }
            },
            { "LevelUp", new VFXData
                {
                Name = "LevelUp",
                RelativeOffset = new Vector3(0f, -0.15f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.3f, 0.3f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "LightningExplosion", new VFXData
                {
                Name = "LightningExplosion",
                RelativeOffset = new Vector3(0f, -0.1f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "LightningStrike", new VFXData
                {
                Name = "LightningStrike",
                RelativeOffset = new Vector3(-0.07f, 0.1f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.05f, 0.05f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "MoonFeather", new VFXData
                {
                Name = "MoonFeather",
                RelativeOffset = new Vector3(0f, -0.02f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(4f, 4f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "OrangeSlash", new VFXData
                {
                Name = "OrangeSlash",
                RelativeOffset = new Vector3(-0.12f, 0.01f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.03f, 0.03f, 0.03f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "PinkSpark", new VFXData
                {
                Name = "PinkSpark",
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.04f, 0.04f, 0.04f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "PuffyExplosion", new VFXData
                {
                Name = "PuffyExplosion",
                RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.2f, 0.2f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "RayBlast", new VFXData
                {
                Name = "RayBlast",
                RelativeOffset = new Vector3(0.02f, -0.02f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "RedSlash2X", new VFXData
                {
                Name = "RedSlash2X",
                RelativeOffset = new Vector3(0.05f, -0.07f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                Delay = 0f,
                Duration = 1f,
                IsLoop = false
                }
            },
            { "RedSword", new VFXData
                {
                Name = "RedSword",
                RelativeOffset = new Vector3(-0.06f, 0.05f, 0f),
                AngularRotation = new Vector3(0f, 0f, 142f),
                RelativeScale = new Vector3(0.2f, 0.2f, 0.2f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "RotaryKnife", new VFXData
                {
                Name = "RotaryKnife",
                RelativeOffset = new Vector3(0.03f, -0.05f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.25f, 0.25f, 0f),
                Delay = 0f,
                Duration = 1f,
                IsLoop = false
                }
            },
            { "ToxicCloud", new VFXData
                {
                Name = "ToxicCloud",
                RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.15f, 0.15f, 0.15f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "YellowHit", new VFXData
                {
                Name = "YellowHit",
                RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.2f, 0.2f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            }
        };
    }

    public ActorStats GetStats(Character character)
    {
        var data = Actors[character.ToString()].Stats;
        if (data == null)
            Debug.LogError($"Unable to retrieve actor stats for `{character}`");

        return new ActorStats(data); //Return a new copy instead of a shared reference
    }

    public ThumbnailSettings GetThumbnailSetting(Character character)
    {
        var data = Actors[character.ToString()].ThumbnailSettings;
        if (data == null)
            Debug.LogError($"Unable to retrieve thumnail settings for `{character}`");

        return new ThumbnailSettings(data); //Return a new copy instead of a shared reference
    }

    public ActorDetails GetDetails(Character character)
    {
        var data = Actors[character.ToString()].Details;
        if (data == null)
            Debug.LogError($"Unable to retrieve actor details for `{character}`");

        return new ActorDetails(data); //Return a new copy instead of a shared reference
    }

    public StageData GetStage(string name)
    {
        var data = Stages[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve stage for `{name}`");

        return new StageData(data); //Return a new copy instead of a shared reference
    }

    public TrailResource GetTrailEffect(string name)
    {
        var data = TrailEffects[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve trailInstance effect for `{name}`");

        return new TrailResource(data); //Return a new copy instead of a shared reference
    }

    public VFXData GetVisualEffect(string name)
    {
        var data = VisualEffects[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve visual effect for `{name}`");

        return new VFXData(data); //Return a new copy instead of a shared reference
    }

}
