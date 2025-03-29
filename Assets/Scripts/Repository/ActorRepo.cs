using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "ActorRepo", menuName = "Repositories/ActorRepo")]
public class ActorRepo : ScriptableObject
{
    //Singleton
    private static ActorRepo Instance;

    public static ActorRepo instance
    {
        get
        {
            if (Instance == null)
                Debug.LogError("ActorRepo accessed before being initialized! Ensure it's assigned in Awake().");
            return Instance;
        }
    }

    //Assign
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            Instance = Resources.Load<ActorRepo>("Repositories/ActorRepo");
            if (Instance == null)
                Debug.LogError("ActorRepo asset not found in Resources/Repositories/ActorRepo");
        }
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, ActorData> Actors;

    private void OnEnable()
    {
        Reload();
    }

    private void Reload()
    {

        Actors = new Dictionary<string, ActorData>
        {

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Barbarian
            { CharacterHelper.Barbarian,
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
                    Speed = 2,
                    Luck = 4
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 2.4f,
                    Vitality = 1.2f,
                    Agility = 0.5f,
                    Speed = 0.3f,
                    Luck = 0.6f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(2f, 1f, 0f, 0f, 0.5f) },
                    { 10, new StatGrowth(3f, 2f, 1f, 0f, 1.0f) },
                    { 20, new StatGrowth(4f, 2f, 1f, 1f, 1.5f) },
                    { 40, new StatGrowth(6f, 3f, 2f, 1f, 2.0f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 420,
                    Y = 50,
                    Width = 256,
                    Height = 256
                },

                Details = new ActorDetails
                {
                    Description = "A warrior driven by rage.",
                    Card = "Gains <color=#FF0033>[Rage]</color> when attacking or being attacked. Will eventually go <color=#FF0000>[Berserk]</color> and attack multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Bat
            { CharacterHelper.Bat,
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
                    Speed = 7,
                    Luck = 5
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.2f,
                    Agility = 1.2f,
                    Speed = 1.4f,
                    Luck = 0.6f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.5f, 0.2f, 2.0f, 2.0f, 1.0f) },
                    { 10, new StatGrowth(1.0f, 0.5f, 1.5f, 2.5f, 1.5f) },
                    { 20, new StatGrowth(2.0f, 1.0f, 2.5f, 3.0f, 2.0f) },
                    { 40, new StatGrowth(3.0f, 2.0f, 4.0f, 4.0f, 2.5f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 200,
                    Y = 380,
                    Width = 512,
                    Height = 512
                },

                Details = new ActorDetails
                {
                    Description = "A warrior driven by rage.",
                    Card = "Intermittently goes <color=#FF0033>[Berserk]</color> attacking multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Cleric
            { CharacterHelper.Cleric,
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
                    Speed = 3,
                    Luck = 9
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 1.2f,
                    Agility = 0.8f,
                    Speed = 0.8f,
                    Luck = 2.5f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.0f, 1.0f, 0.0f, 0.0f, 3.0f) },
                    { 10, new StatGrowth(0.5f, 1.5f, 1.0f, 0.5f, 4.0f) },
                    { 20, new StatGrowth(1.0f, 2.0f, 1.0f, 1.0f, 5.0f) },
                    { 40, new StatGrowth(1.5f, 2.5f, 2.0f, 2.0f, 6.0f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 455,
                    Y = 60,
                    Width = 215,
                    Height = 215
                },

                Details = new ActorDetails
                {
                    Description = "An adherent to the Lightbearer Orthodoxy.",
                    Card = "Casts <color=#00FF00>[Cure]</color> when supporting an attacker.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Ninja
            { CharacterHelper.Ninja,
            new ActorData
            {
                Character = CharacterHelper.Ninja,
                Description = "A stealthy assassin.",

                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 6,
                    Vitality = 3,
                    Agility = 10,
                    Speed = 10,
                    Luck = 4
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.6f,
                    Vitality = 0.5f,
                    Agility = 2.2f,
                    Speed = 2.0f,
                    Luck = 0.8f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(1.0f, 0.0f, 2.0f, 2.0f, 0.5f) },
                    { 10, new StatGrowth(2.0f, 0.5f, 3.0f, 2.5f, 1.0f) },
                    { 20, new StatGrowth(3.0f, 1.0f, 4.0f, 3.0f, 1.5f) },
                    { 40, new StatGrowth(5.0f, 1.5f, 5.0f, 4.0f, 2.0f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 380,
                    Y = 50,
                    Width = 196,
                    Height = 196
                },

                Details = new ActorDetails
                {
                    Description = "A stealthy assassin raised in the shadows; trained to dispatch Lightbearers.",
                    Card = "Intermittently enters <color=#0000FF>[Stealth]</color> to avoid enemy results",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Paladin
            { CharacterHelper.Paladin,
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
                    Speed = 2,
                    Luck = 5
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.4f,
                    Vitality = 2.0f,
                    Agility = 0.6f,
                    Speed = 0.6f,
                    Luck = 0.9f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(1.0f, 2.0f, 0.0f, 0.0f, 0.5f) },
                    { 10, new StatGrowth(2.0f, 2.0f, 0.5f, 0.5f, 1.0f) },
                    { 20, new StatGrowth(3.0f, 3.5f, 0.5f, 0.5f, 1.5f) },
                    { 40, new StatGrowth(5.0f, 5.0f, 1.0f, 1.0f, 2.0f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 430,
                    Y = 60,
                    Width = 196,
                    Height = 196
                },

                Details = new ActorDetails
                {
                    Description = "A holy knight honor bound to defend the Lightbearer Orthodoxy.",
                    Card = "Intermittently uses <color=#FF0000>[Taunt]</color> to force enemies to focus their results.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Scorpion
            { CharacterHelper.Scorpion,
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
                    Speed = 3,
                    Luck = 2
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.0f,
                    Vitality = 2.2f,
                    Agility = 0.6f,
                    Speed = 0.8f,
                    Luck = 0.4f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(1.0f, 3.0f, 0.0f, 0.0f, 0.5f) },
                    { 10, new StatGrowth(2.0f, 3.5f, 0.5f, 0.5f, 0.8f) },
                    { 20, new StatGrowth(3.0f, 4.5f, 1.0f, 1.0f, 1.0f) },
                    { 40, new StatGrowth(4.0f, 6.0f, 1.5f, 1.5f, 1.5f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 200,
                    Y = 380,
                    Width = 256,
                    Height = 256
                },

                Details = new ActorDetails
                {
                    Description = "A poisonous insectile predator.",
                    Card = "Attacks have chance of inflicting <color=#00FF00>[Poison]</color>",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },

            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Slime
            { CharacterHelper.Slime,
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
                    Speed = 1,
                    Luck = 1
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.2f,
                    Vitality = 0.3f,
                    Agility = 0.2f,
                    Speed = 0.2f,
                    Luck = 0.2f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
                    { 10, new StatGrowth(0.8f, 0.8f, 0.8f, 0.8f, 0.8f) },
                    { 20, new StatGrowth(1.0f, 1.0f, 1.0f, 1.0f, 1.0f) },
                    { 40, new StatGrowth(1.5f, 1.5f, 1.5f, 1.5f, 1.5f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 200,
                    Y = 250,
                    Width = 512,
                    Height = 512
                },

                Details = new ActorDetails
                {
                    Description = "A weak and squishy creature.",
                    Card = "The most common denizen of the dark; neither good nor evil, a monster still has to eat.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }
            },
            
            // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| Yeti
            { CharacterHelper.Yeti,
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
                    Speed = 1,
                    Luck = 3
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 2.0f,
                    Vitality = 2.5f,
                    Agility = 0.4f,
                    Speed = 0.3f,
                    Luck = 0.6f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 10, new StatGrowth(3.0f, 3.5f, 0.5f, 0.5f, 0.5f) },
                    { 20, new StatGrowth(4.0f, 4.5f, 1.0f, 1.0f, 1.0f) },
                    { 40, new StatGrowth(5.0f, 6.0f, 1.5f, 1.5f, 1.5f) }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    X = 200,
                    Y = 150,
                    Width = 256,
                    Height = 256
                },

                Details = new ActorDetails
                {
                    Description = "A large humanoid underdweller.",
                    Card = "Intermittently goes <color=#FF0000>[Berserk]</color> attacking multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            }},

        };
    }


}
