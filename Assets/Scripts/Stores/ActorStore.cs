using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorStore", menuName = "Stores/ActorStore")]
public class ActorStore : ScriptableObject
{
    //Singleton
    private static ActorStore _instance;

    public static ActorStore instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("ActorStore accessed before being initialized! Ensure it's assigned in Awake().");
            return _instance;
        }
    }

    //Initialize
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<ActorStore>("Stores/ActorStore");
            if (_instance == null)
                Debug.LogError("ActorStore asset not found in Resources/Stores/ActorStore");
        }
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, ActorData> Actors;


    private void OnEnable()
    {
        Load();
    }

    private void Load()
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


}
