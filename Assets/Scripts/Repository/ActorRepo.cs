using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class ActorRepo
{
    private static Dictionary<string, ActorData> actors;

    public static Dictionary<string, ActorData> Actors
    {
        get
        {
            if (actors == null)
                Load();
            return actors;
        }
    }

    private static void Load()
    {
        actors = new Dictionary<string, ActorData>
        {
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Barbarian
            {
                CharacterHelper.Barbarian,
                new ActorData
                {
                    Character = CharacterHelper.Barbarian,
                    Description = "A warrior driven by rage.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 9,
                        Vitality = 6,
                        Agility = 3,
                        Intelligence = 2,
                        Wisdom = 1,
                        Stamina = 2,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 2.4f,
                        Vitality = 1.2f,
                        Agility = 0.5f,
                        Wisdom = 0.3f,
                        Stamina = 0.6f,
                        Intelligence = 0.0f,
                        Luck = 0.6f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(2f, 0.5f, 0f, 0f, 1f, 0f, 0.5f) },
                        { 10, new StatGrowth(3f, 1f, 0f, 0f, 2f, 0f, 1.0f) },
                        { 20, new StatGrowth(4f, 1f, 0f, 1f, 2f, 0f, 1.5f) },
                        { 40, new StatGrowth(6f, 1.5f, 0f, 1f, 3f, 0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.1f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Barbarian}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A warrior driven by rage.",
                        Card =
                            "Gains <color=#FF0033>[Rage]</color> when attacking or being attacked. Will eventually go <color=#FF0000>[Berserk]</color> and attackResult multiple nearby enemies.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Bat
            {
                CharacterHelper.Bat,
                new ActorData
                {
                    Character = CharacterHelper.Bat,
                    Description = "A flying menace.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 2,
                        Vitality = 1,
                        Agility = 6,
                        Intelligence = 7,
                        Wisdom = 3,
                        Stamina = 2,
                        Luck = 5
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.4f,
                        Vitality = 0.2f,
                        Agility = 1.2f,
                        Intelligence = 1.4f,
                        Wisdom = 1.0f,
                        Stamina = 0.6f,
                        Luck = 0.6f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.2f, 2.0f, 2.0f, 1.0f, 0.5f, 1.0f) },
                        { 10, new StatGrowth(1.0f, 0.5f, 1.5f, 2.5f, 1.5f, 1.0f, 1.5f) },
                        { 20, new StatGrowth(2.0f, 1.0f, 2.5f, 3.0f, 2.0f, 1.5f, 2.0f) },
                        { 40, new StatGrowth(3.0f, 2.0f, 4.0f, 4.0f, 2.5f, 2.0f, 2.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, 0.5f, 0f),
                        Scale = new Vector3(2f, 2f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Bat}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A flying menace.",
                        Card =
                            "Intermittently goes <color=#FF0033>[Berserk]</color> attacking multiple nearby enemies.",
                        Lore = new List<string> { "Echolocation expert", "Sleeps upside down" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Captain00
            {
                CharacterHelper.Captain00,
                new ActorData
                {
                    Character = CharacterHelper.Captain00,
                    Description = "A captain.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 10,
                        Vitality = 10,
                        Agility = 4,
                        Intelligence = 2,
                        Wisdom = 2,
                        Stamina = 5,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.2f,
                        Vitality = 0.3f,
                        Agility = 0.2f,
                        Intelligence = 0.2f,
                        Wisdom = 0.2f,
                        Stamina = 0.2f,
                        Luck = 0.2f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                        { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                        { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.41f, -1.5f, 0f),
                        Scale = new Vector3(5f, 5f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Captain00}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A captain.",
                        Card = "A captain.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Cleric
            {
                CharacterHelper.Cleric,
                new ActorData
                {
                    Character = CharacterHelper.Cleric,
                    Description = "A strict adherent to the church.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 2,
                        Vitality = 5,
                        Agility = 3,
                        Intelligence = 3,
                        Wisdom = 2,
                        Stamina = 2,
                        Luck = 9
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.4f,
                        Vitality = 1.2f,
                        Agility = 0.8f,
                        Intelligence = 0.8f,
                        Wisdom = 0.8f,
                        Stamina = 0.5f,
                        Luck = 2.5f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.5f, 3.0f) },
                        { 10, new StatGrowth(0.5f, 1.5f, 0.0f, 0.5f, 1.0f, 1.0f, 4.0f) },
                        { 20, new StatGrowth(1.0f, 2.0f, 1.0f, 1.0f, 1.5f, 1.5f, 5.0f) },
                        { 40, new StatGrowth(1.5f, 2.5f, 2.0f, 2.0f, 2.0f, 2.0f, 6.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Cleric}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "An adherent to the Lightbearer Orthodoxy.",
                        Card = "Casts <color=#00FF00>[Cure]</color> when supporting an attacker.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| GreenNinja
            {
                CharacterHelper.GreenNinja,
                new ActorData
                {
                    Character = CharacterHelper.GreenNinja,
                    Description = "A stealthy assassin.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 3,
                        Agility = 10,
                        Intelligence = 10,
                        Wisdom = 5,
                        Stamina = 4,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.6f,
                        Vitality = 0.5f,
                        Agility = 2.2f,
                        Intelligence = 1.2f,
                        Wisdom = 2.0f,
                        Stamina = 1.0f,
                        Luck = 0.8f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.0f, 2.0f, 1.2f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 1.5f, 2.5f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 2.0f, 3.0f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 2.5f, 4.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.GreenNinja}"
                    ),
                    Details = new ActorDetails
                    {
                        Description =
                            "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                        Card =
                            "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy attackResults",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Paladin
            {
                CharacterHelper.Paladin,
                new ActorData
                {
                    Character = CharacterHelper.Paladin,
                    Description = "A holy knight.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 8,
                        Agility = 2,
                        Intelligence = 2,
                        Wisdom = 2,
                        Stamina = 3,
                        Luck = 5
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.4f,
                        Vitality = 2.0f,
                        Agility = 0.6f,
                        Intelligence = 0.4f,
                        Wisdom = 0.6f,
                        Stamina = 0.6f,
                        Luck = 0.9f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.6f, 0.0f, 0.0f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.6f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 0.8f, 0.5f, 0.5f, 3.5f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.0f, 1.0f, 1.0f, 5.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Paladin}"
                    ),
                    Details = new ActorDetails
                    {
                        Description =
                            "A holy knight honor bound to defend the Lightbearer Orthodoxy.",
                        Card =
                            "Intermittently uses <color=#FF0000>[Taunt]</color> to force enemies to focus their attackResults.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Pugilist
            {
                CharacterHelper.Pugilist,
                new ActorData
                {
                    Character = CharacterHelper.Pugilist,
                    Description = "A stealthy assassin.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 3,
                        Agility = 10,
                        Intelligence = 10,
                        Wisdom = 5,
                        Stamina = 4,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.6f,
                        Vitality = 0.5f,
                        Agility = 2.2f,
                        Intelligence = 1.2f,
                        Wisdom = 2.0f,
                        Stamina = 1.0f,
                        Luck = 0.8f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.0f, 2.0f, 1.2f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 1.5f, 2.5f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 2.0f, 3.0f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 2.5f, 4.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Pugilist}"
                    ),
                    Details = new ActorDetails
                    {
                        Description =
                            "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                        Card =
                            "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy attackResults",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| RedNinja
            {
                CharacterHelper.RedNinja,
                new ActorData
                {
                    Character = CharacterHelper.RedNinja,
                    Description = "A stealthy assassin.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 3,
                        Agility = 10,
                        Intelligence = 10,
                        Wisdom = 5,
                        Stamina = 4,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.6f,
                        Vitality = 0.5f,
                        Agility = 2.2f,
                        Intelligence = 1.2f,
                        Wisdom = 2.0f,
                        Stamina = 1.0f,
                        Luck = 0.8f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.0f, 2.0f, 1.2f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 1.5f, 2.5f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 2.0f, 3.0f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 2.5f, 4.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.RedNinja}"
                    ),
                    Details = new ActorDetails
                    {
                        Description =
                            "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                        Card =
                            "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy attackResults",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Ronin
            {
                CharacterHelper.Ronin,
                new ActorData
                {
                    Character = CharacterHelper.Ronin,
                    Description = "A stealthy assassin.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 3,
                        Agility = 10,
                        Intelligence = 10,
                        Wisdom = 5,
                        Stamina = 4,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.6f,
                        Vitality = 0.5f,
                        Agility = 2.2f,
                        Intelligence = 1.2f,
                        Wisdom = 2.0f,
                        Stamina = 1.0f,
                        Luck = 0.8f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.0f, 2.0f, 1.2f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 1.5f, 2.5f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 2.0f, 3.0f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 2.5f, 4.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.35f, -1.34f, 0f),
                        Scale = new Vector3(5f, 5f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Ronin}"
                    ),
                    Details = new ActorDetails
                    {
                        Description =
                            "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                        Card =
                            "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy attackResults",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Sellsword
            {
                CharacterHelper.Sellsword,
                new ActorData
                {
                    Character = CharacterHelper.Sellsword,
                    Description = "A mercenary that fights for coin.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 3,
                        Agility = 10,
                        Intelligence = 4,
                        Wisdom = 5,
                        Stamina = 3,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.6f,
                        Vitality = 0.5f,
                        Agility = 2.2f,
                        Intelligence = 1.0f,
                        Wisdom = 2.0f,
                        Stamina = 1.0f,
                        Luck = 0.8f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.0f, 2.0f, 1.0f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 1.5f, 2.5f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 2.0f, 3.0f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 2.5f, 4.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Sellsword}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A mercenary for hire who goes wherever the coin flows.",
                        Card =
                            "Sometimes flips <color=#FFD700>[Bribe]</color> to distract enemies or gain buffs.",
                        Lore = new List<string>
                        {
                            "Only loyal to gold",
                            "Has debts in three cities"
                        }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Scorpion
            {
                CharacterHelper.Scorpion,
                new ActorData
                {
                    Character = CharacterHelper.Scorpion,
                    Description = "A poisonous insectile predator.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 4,
                        Vitality = 8,
                        Agility = 2,
                        Intelligence = 3,
                        Wisdom = 2,
                        Stamina = 3,
                        Luck = 2
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.0f,
                        Vitality = 2.2f,
                        Agility = 0.6f,
                        Intelligence = 0.6f,
                        Wisdom = 0.8f,
                        Stamina = 0.5f,
                        Luck = 0.4f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.6f, 0.6f, 0.5f, 3.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.6f, 0.6f, 0.8f, 3.5f, 1.0f, 0.8f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 1.0f, 1.2f, 4.5f, 1.5f, 1.0f) },
                        { 40, new StatGrowth(4.0f, 1.5f, 1.5f, 1.5f, 6.0f, 2.0f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -0.15f, 0f),
                        Scale = new Vector3(2f, 2f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Scorpion}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A poisonous insectile predator.",
                        Card = "Attacks have chance of inflicting <color=#00FF00>[Poison]</color>",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Slime
            {
                CharacterHelper.Slime,
                new ActorData
                {
                    Character = CharacterHelper.Slime,
                    Description = "A weak and squishy creature.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 1,
                        Vitality = 1,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 1,
                        Stamina = 1,
                        Luck = 1
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.2f,
                        Vitality = 0.3f,
                        Agility = 0.2f,
                        Intelligence = 0.2f,
                        Wisdom = 0.2f,
                        Stamina = 0.2f,
                        Luck = 0.2f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                        { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                        { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, 0.5f, 0f),
                        Scale = new Vector3(2f, 2f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Slime}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A weak and squishy creature.",
                        Card =
                            "The most common denizen of the dark; neither good nor evil, a monster still has to eat.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Soldier 00
            {
                CharacterHelper.Soldier00,
                new ActorData
                {
                    Character = CharacterHelper.Soldier00,
                    Description = "A soldier.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 1,
                        Vitality = 1,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 1,
                        Stamina = 1,
                        Luck = 1
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.2f,
                        Vitality = 0.3f,
                        Agility = 0.2f,
                        Intelligence = 0.2f,
                        Wisdom = 0.2f,
                        Stamina = 0.2f,
                        Luck = 0.2f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                        { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                        { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.71f, -1.5f, 0f),
                        Scale = new Vector3(5f, 5f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Soldier00}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A soldier.",
                        Card = "A soldier.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Soldier 01
            {
                CharacterHelper.Soldier01,
                new ActorData
                {
                    Character = CharacterHelper.Soldier01,
                    Description = "A soldier.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 1,
                        Vitality = 1,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 1,
                        Stamina = 1,
                        Luck = 1
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.2f,
                        Vitality = 0.3f,
                        Agility = 0.2f,
                        Intelligence = 0.2f,
                        Wisdom = 0.2f,
                        Stamina = 0.2f,
                        Luck = 0.2f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                        { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                        { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Soldier01}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A soldier.",
                        Card = "A soldier.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Soldier 02
            {
                CharacterHelper.Soldier02,
                new ActorData
                {
                    Character = CharacterHelper.Soldier02,
                    Description = "A soldier.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 1,
                        Vitality = 1,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 1,
                        Stamina = 1,
                        Luck = 1
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.2f,
                        Vitality = 0.3f,
                        Agility = 0.2f,
                        Intelligence = 0.2f,
                        Wisdom = 0.2f,
                        Stamina = 0.2f,
                        Luck = 0.2f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                        { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                        { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.55f, -1.5f, 0f),
                        Scale = new Vector3(5f, 5f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Soldier02}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A soldier.",
                        Card = "A soldier.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Soldier 03
            {
                CharacterHelper.Soldier03,
                new ActorData
                {
                    Character = CharacterHelper.Soldier03,
                    Description = "A soldier.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 1,
                        Vitality = 1,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 1,
                        Stamina = 1,
                        Luck = 1
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 0.2f,
                        Vitality = 0.3f,
                        Agility = 0.2f,
                        Intelligence = 0.2f,
                        Wisdom = 0.2f,
                        Stamina = 0.2f,
                        Luck = 0.2f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                        { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                        { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.7f, -1.4f, 0f),
                        Scale = new Vector3(5f, 5f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Soldier03}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A soldier.",
                        Card = "A soldier.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Thief
            {
                CharacterHelper.Thief,
                new ActorData
                {
                    Character = CharacterHelper.Thief,
                    Description = "A stealthy assassin.",
                    BaseStats = new ActorStats
                    {
                        Level = 1,
                        Strength = 6,
                        Vitality = 3,
                        Agility = 10,
                        Intelligence = 10,
                        Wisdom = 5,
                        Stamina = 4,
                        Luck = 4
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 1.6f,
                        Vitality = 0.5f,
                        Agility = 2.2f,
                        Intelligence = 1.2f,
                        Wisdom = 2.0f,
                        Stamina = 1.0f,
                        Luck = 0.8f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 5, new StatGrowth(1.0f, 0.0f, 2.0f, 1.2f, 2.0f, 0.5f, 0.5f) },
                        { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 1.5f, 2.5f, 1.0f, 1.0f) },
                        { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 2.0f, 3.0f, 1.5f, 1.5f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 2.5f, 4.0f, 2.0f, 2.0f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.61f, -1.56f, 0f),
                        Scale = new Vector3(5.3f, 5.3f, 0f),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Thief}"
                    ),
                    Details = new ActorDetails
                    {
                        Description =
                            "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                        Card =
                            "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy attackResults",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Vampire
            {
                CharacterHelper.Vampire,
                new ActorData
                {
                    Character = CharacterHelper.Vampire,
                    Description = "A large humanoid underdweller.",
                    BaseStats = new ActorStats
                    {
                        Level = 5,
                        Strength = 6,
                        Vitality = 10,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 2,
                        Stamina = 3,
                        Luck = 3
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 2.0f,
                        Vitality = 2.5f,
                        Agility = 0.4f,
                        Intelligence = 0.6f,
                        Wisdom = 0.3f,
                        Stamina = 0.5f,
                        Luck = 0.6f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 10, new StatGrowth(3.0f, 0.5f, 0.6f, 0.5f, 3.5f, 1.0f, 0.5f) },
                        { 20, new StatGrowth(4.0f, 1.0f, 1.0f, 1.0f, 4.5f, 1.5f, 1.0f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 1.5f, 1.5f, 6.0f, 2.0f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(0.5f, -1.4f, 0),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Vampire}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A large humanoid underdweller.",
                        Card =
                            "Intermittently goes <color=#FF0000>[Berserk]</color> attacking multiple nearby enemies.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Yeti
            {
                CharacterHelper.Yeti,
                new ActorData
                {
                    Character = CharacterHelper.Yeti,
                    Description = "A large humanoid underdweller.",
                    BaseStats = new ActorStats
                    {
                        Level = 5,
                        Strength = 6,
                        Vitality = 10,
                        Agility = 1,
                        Intelligence = 1,
                        Wisdom = 2,
                        Stamina = 3,
                        Luck = 3
                    },
                    StatGrowth = new StatGrowth
                    {
                        Strength = 2.0f,
                        Vitality = 2.5f,
                        Agility = 0.4f,
                        Intelligence = 0.6f,
                        Wisdom = 0.3f,
                        Stamina = 0.5f,
                        Luck = 0.6f
                    },
                    MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                    {
                        { 10, new StatGrowth(3.0f, 0.5f, 0.6f, 0.5f, 3.5f, 1.0f, 0.5f) },
                        { 20, new StatGrowth(4.0f, 1.0f, 1.0f, 1.0f, 4.5f, 1.5f, 1.0f) },
                        { 40, new StatGrowth(5.0f, 1.5f, 1.5f, 1.5f, 6.0f, 2.0f, 1.5f) }
                    },
                    Stats = new ActorStats(),
                    ThumbnailSettings = new ThumbnailSettings
                    {
                        Position = new Vector3(1.3f, -1f, 0),
                        Scale = new Vector3(5f, 5f, 0),
                    },
                    Portrait = AssetHelper.LoadAsset<Sprite>(
                        $"Actor-Portraits/{CharacterHelper.Yeti}"
                    ),
                    Details = new ActorDetails
                    {
                        Description = "A large humanoid underdweller.",
                        Card =
                            "Intermittently goes <color=#FF0000>[Berserk]</color> attacking multiple nearby enemies.",
                        Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                    }
                }
            },
        };
    }
}
