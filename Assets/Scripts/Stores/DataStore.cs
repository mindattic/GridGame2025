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
    [SerializeField] public Dictionary<string, TrailResource> TrailEffects;
    [SerializeField] public Dictionary<string, Tutorial> Tutorials;
    [SerializeField] public Dictionary<string, VFXData> VisualEffects;

    private void OnEnable()
    {
    
        LoadTrailEffects();
        LoadTutorials();
        LoadVisualEffects();
    }

    /// <summary>
    /// Generates random waves of enemies for a stage.
    /// </summary>
    /// <param name="waveCount">Number of waves.</param>
    /// <param name="possibleEnemies">List of enemy types to use in waves.</param>
    /// <returns>A list of StageWave.</returns>
    private List<StageWave> GenerateWaves(int waveCount, List<Character> possibleEnemies)
    {
        //DEBUG: Manually enter heros in here for now, should be stored in ProfileStore




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
                Character randomEnemy = possibleEnemies[rng.Next(possibleEnemies.Count)];
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
