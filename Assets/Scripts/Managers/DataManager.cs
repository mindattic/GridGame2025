using Assets.Scripts.Models;
using Game.Behaviors;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    protected LogManager logManager => GameManager.instance.logManager;

    public static class Resource
    {
        public static string Actors = "Actors";
        public static string Stages = "Stages";
        public static string TrailEffects = "TrailEffects";
        public static string VisualEffects = "VisualEffects";
    }

    //public List<ActorData> Actors = new List<ActorData>();
    //public List<StageData> Stages = new List<StageData>();
    //public List<TrailData> TrailEffects = new List<TrailData>();

    public readonly Dictionary<string, ActorData> Actors = new Dictionary<string, ActorData>
    {
        { "Barbarian", new ActorData
            {
                Character = "Barbarian",
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
                Character = "Bat",
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
                Character = "Cleric",
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
                Character = "Ninja",
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
                    Card = "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy attacks",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
        },
        { "Paladin", new ActorData
            {
                Character = "Paladin",
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
                    Card = "Intermittently uses <color=#FF0000>[Taunt]</color> to force enemies to focus their attacks.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
        },
        { "Scorpion", new ActorData
            {
                Character = "Scorpion",
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
                Character = "Slime",
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
                Character = "Yeti",
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



    public readonly Dictionary<string, StageData> Stages = new Dictionary<string, StageData>
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
                    new StageActor { Character = "Paladin", Team = "Player", Location = "(2, 7)" },
                    new StageActor { Character = "Slime", Team = "Enemy", Location = "(5, 6)" },
                    new StageActor { Character = "Barbarian", Team = "Player", Location = "(4, 5)" }
                },
                DottedLines = new List<StageDottedLine>
                {
                    new StageDottedLine { Segment = "Vertical", Location = "(2, 3)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(2, 4)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(2, 5)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(2, 6)" },
                    new StageDottedLine { Segment = "TurnBottomRight", Location = "(2, 2)" },
                    new StageDottedLine { Segment = "Horizontal", Location = "(3, 2)" },
                    new StageDottedLine { Segment = "Horizontal", Location = "(4, 2)" },
                    new StageDottedLine { Segment = "TurnBottomLeft", Location = "(5, 2)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(5, 3)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(5, 4)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(5, 5)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(5, 6)" },
                    new StageDottedLine { Segment = "TurnTopRight", Location = "(5, 7)" },
                    new StageDottedLine { Segment = "TurnTopLeft", Location = "(6, 7)" },
                    new StageDottedLine { Segment = "Vertical", Location = "(6, 6)" },
                    new StageDottedLine { Segment = "ArrowUp", Location = "(6, 5)" }
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
                    new StageActor { Character = "Paladin", Team = "Player" },
                    new StageActor { Character = "Barbarian", Team = "Player" },
                    new StageActor { Character = "Cleric", Team = "Player" },
                    new StageActor { Character = "Slime", Team = "Enemy" },
                    new StageActor { Character = "Slime", Team = "Enemy", SpawnTurn = 2 },
                    new StageActor { Character = "Slime", Team = "Enemy", SpawnTurn = 3 },
                    new StageActor { Character = "Slime", Team = "Enemy", SpawnTurn = 4 },
                    new StageActor { Character = "Slime", Team = "Enemy", SpawnTurn = 5 },
                    new StageActor { Character = "Slime", Team = "Enemy", SpawnTurn = 6 }
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
                    new StageActor { Character = "Paladin", Team = "Player" },
                    new StageActor { Character = "Barbarian", Team = "Player" },
                    new StageActor { Character = "Cleric", Team = "Player" },
                    new StageActor { Character = "Ninja", Team = "Player" },
                    new StageActor { Character = "Scorpion", Team = "Enemy" },
                    new StageActor { Character = "Bat", Team = "Enemy" },
                    new StageActor { Character = "Bat", Team = "Enemy" },
                    new StageActor { Character = "Bat", Team = "Enemy" }
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
                NextStage = "None",
                Actors = new List<StageActor>
                {
                    new StageActor { Character = "Paladin", Team = "Player" },
                    new StageActor { Character = "Barbarian", Team = "Player" },
                    new StageActor { Character = "Cleric", Team = "Player" },
                    new StageActor { Character = "Ninja", Team = "Player" },
                    new StageActor { Character = "Yeti", Team = "Enemy" },
                    new StageActor { Character = "Bat", Team = "Enemy" },
                    new StageActor { Character = "Slime", Team = "Enemy" },
                    new StageActor { Character = "Slime", Team = "Enemy", SpawnTurn = 5 }
                },
                DottedLines = new List<StageDottedLine>(),
                Tutorials = new List<string>()
            }
        }
    };


    public readonly Dictionary<string, TrailData> TrailEffects = new Dictionary<string, TrailData>
    {
        { "BlueGlow", new TrailData { Name = "BlueGlow", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "Bubble", new TrailData { Name = "Bubble", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "Feather", new TrailData { Name = "Feather", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "Fireball", new TrailData { Name = "Fireball", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.5, 0.5, 0.5)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "Flame", new TrailData { Name = "Flame", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.08, 0.08, 0.08)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "GoldSparkle", new TrailData { Name = "GoldSparkle", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(2.5, 2.5, 2.5)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "GreenSparkle", new TrailData { Name = "GreenSparkle", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(2.5, 2.5, 2.5)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "IceSparkle", new TrailData { Name = "IceSparkle", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "PinkDust", new TrailData { Name = "PinkDust", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "RosePetal", new TrailData { Name = "RosePetal", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } },
        { "StarSparkle", new TrailData { Name = "StarSparkle", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = true } }
    };

    public readonly Dictionary<string, Tutorial> Tutorials = new Dictionary<string, Tutorial>
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

    public readonly Dictionary<string, VFXData> VisualEffects = new Dictionary<string, VFXData>
    {
        { "AcidSplash", new VFXData { Name = "AcidSplash", RelativeOffset = "(0, 0.01, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "AirSlash", new VFXData { Name = "AirSlash", RelativeOffset = "(0.01, -0.15, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.15, 0.15, 0.15)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BloodClaw", new VFXData { Name = "BloodClaw", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.15, 0.15, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BlueSlash1", new VFXData { Name = "BlueSlash1", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0.12f, Duration = 2f, IsLoop = false } },
        { "BlueSlash2", new VFXData { Name = "BlueSlash2", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0.1)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BlueSlash3", new VFXData { Name = "BlueSlash3", RelativeOffset = "(0.02, -0.15, 0)", AngularRotation = "(30, 30, 0)", RelativeScale = "(0.08, 0.08, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BlueSword", new VFXData { Name = "BlueSword", RelativeOffset = "(0, 0.05, 0)", AngularRotation = "(30, 30, 0)", RelativeScale = "(0.12, 0.08, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BlueSword4X", new VFXData { Name = "BlueSword4X", RelativeOffset = "(-0.05, -0.1, 0)", AngularRotation = "(30, 30, 0)", RelativeScale = "(0.08, 0.08, 0)", Delay = 0f, Duration = 3f, IsLoop = false } },
        { "BlueYellowSword", new VFXData { Name = "BlueYellowSword", RelativeOffset = "(0.03, 0.01, 0)", AngularRotation = "(60, 0, 0)", RelativeScale = "(0.07, 0.07, 0.07)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BlueYellowSword3X", new VFXData { Name = "BlueYellowSword3X", RelativeOffset = "(0.02, -0.05, 0)", AngularRotation = "(60, 0, 0)", RelativeScale = "(0.07, 0.07, 0.07)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "BuffLife", new VFXData { Name = "BuffLife", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.16, 0.16, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "DoubleClaw", new VFXData { Name = "DoubleClaw", RelativeOffset = "(-0.03, -0.1, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.12, 0.12, 0)", Delay = 0.22f, Duration = 2f, IsLoop = false } },
        { "FireRain", new VFXData { Name = "FireRain", RelativeOffset = "(0.03, -0.05, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0)", Delay = 0f, Duration = 4f, IsLoop = false } },
        { "GodRays", new VFXData { Name = "GodRays", RelativeOffset = "(0, -0.25, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.07, 0.07, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "GoldBuff", new VFXData { Name = "GoldBuff", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.08, 0.08, 0.08)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "GreenBuff", new VFXData { Name = "GreenBuff", RelativeOffset = "(0.02, -0.25, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.08, 0.08, 0.08)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "HexShield", new VFXData { Name = "HexShield", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.16, 0.16, 0.16)", Delay = 0f, Duration = 6f, IsLoop = false } },
        { "LevelUp", new VFXData { Name = "LevelUp", RelativeOffset = "(0, -0.15, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.3, 0.3, 0)", Delay = 0f, Duration = 3f, IsLoop = false } },
        { "LightningExplosion", new VFXData { Name = "LightningExplosion", RelativeOffset = "(0, -0.1, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0)", Delay = 0f, Duration = 3f, IsLoop = false } },
        { "LightningStrike", new VFXData { Name = "LightningStrike", RelativeOffset = "(-0.07, 0.1, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.05, 0.05, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "MoonFeather", new VFXData { Name = "MoonFeather", RelativeOffset = "(0, -0.02, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(4, 4, 0)", Delay = 0f, Duration = 3f, IsLoop = false } },
        { "OrangeSlash", new VFXData { Name = "OrangeSlash", RelativeOffset = "(-0.12, 0.01, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.03, 0.03, 0.03)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "PinkSpark", new VFXData { Name = "PinkSpark", RelativeOffset = "(0, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.04, 0.04, 0.04)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "PuffyExplosion", new VFXData { Name = "PuffyExplosion", RelativeOffset = "(-0.02, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.2, 0.2, 0)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "RayBlast", new VFXData { Name = "RayBlast", RelativeOffset = "(0.02, -0.02, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.1, 0.1, 0)", Delay = 0f, Duration = 3f, IsLoop = false } },
        { "RedSlash2X", new VFXData { Name = "RedSlash2X", RelativeOffset = "(0.05, -0.07, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.08, 0.08, 0)", Delay = 0f, Duration = 1f, IsLoop = false } },
        { "RedSword", new VFXData { Name = "RedSword", RelativeOffset = "(-0.06, 0.05, 0)", AngularRotation = "(0, 0, 142)", RelativeScale = "(0.2, 0.2, 0.2)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "RotaryKnife", new VFXData { Name = "RotaryKnife", RelativeOffset = "(0.03, -0.05, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.25, 0.25, 0)", Delay = 0f, Duration = 1f, IsLoop = false } },
        { "ToxicCloud", new VFXData { Name = "ToxicCloud", RelativeOffset = "(-0.02, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.15, 0.15, 0.15)", Delay = 0f, Duration = 2f, IsLoop = false } },
        { "YellowHit", new VFXData { Name = "YellowHit", RelativeOffset = "(-0.02, 0, 0)", AngularRotation = "(0, 0, 0)", RelativeScale = "(0.2, 0.2, 0)", Delay = 0f, Duration = 2f, IsLoop = false } }
    };


    //public List<T> ParseJson<T>(string resource)
    //{
    //    string filePath = $"Data/{resource}";
    //    //Debug.Log(filePath);
    //    TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

    //    if (jsonFile == null)
    //    {
    //        logManager.Error($"File {filePath} not found in Resources.");
    //        return null;
    //    }

    //    var collection = JsonConvert.DeserializeObject<JsonWrapper<T>>(jsonFile.text);
    //    return collection.Items;
    //}

    public void Initialize()
    {
        //Actors = ParseJson<ActorData>(Resource.Actors);
        //Stages = ParseJson<StageData>(Resource.Stages);
        //TrailEffects = ParseJson<TrailData>(Resource.TrailEffects);
        //VisualEffects = ParseJson<VFXData>(Resource.VisualEffects);
    }

    public ActorStats GetStats(Character character)
    {
        var data = Actors[character.ToString()].Stats;
        if (data == null)
            logManager.Error($"Unable to retrieve actor stats for `{character}`");


        return new ActorStats(data); //Return a new copy instead of a shared reference
        //return data;
    }

    public ThumbnailSettings GetThumbnailSetting(Character character)
    {
        var data = Actors[character.ToString()].ThumbnailSettings;
        if (data == null)
            logManager.Error($"Unable to retrieve thumnail settings for `{name}`");

        return new ThumbnailSettings(data); //Return a new copy instead of a shared reference
        //return data;
    }

    public ActorDetails GetDetails(Character character)
    {
        var data = Actors[character.ToString()].Details;
        if (data == null)
            logManager.Error($"Unable to retrieve actor details for `{character}`");

        return new ActorDetails(data); //Return a new copy instead of a shared reference
    }

    public StageData GetStage(string name)
    {
        var data = Stages[name];
        if (data == null)
            logManager.Error($"Unable to retrieve stage for `{name}`");

        return new StageData(data); //Return a new copy instead of a shared reference
    }
    public TrailData GetTrailEffect(string name)
    {
        var data = TrailEffects[name];
        if (data == null)
            logManager.Error($"Unable to retrieve trailInstance effect for `{name}`");

        return new TrailData(data); //Return a new copy instead of a shared reference
    }

    public VFXData GetVisualEffect(string name)
    {
        var data = VisualEffects[name];
        if (data == null)
            logManager.Error($"Unable to retrieve visual effect for `{name}`");

        return new VFXData(data); //Return a new copy instead of a shared reference
    }

}
