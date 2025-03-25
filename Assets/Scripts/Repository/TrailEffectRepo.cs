using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrailEffectRepo", menuName = "Repositories/TrailEffectRepo")]
public class TrailEffectRepo : ScriptableObject
{
    //Singleton
    private static TrailEffectRepo Instance;

    public static TrailEffectRepo instance
    {
        get
        {
            if (Instance == null)
                Debug.LogError("TrailEffectRepo accessed before being initialized! Ensure it's assigned in Awake().");
            return Instance;
        }
    }

    //Assign
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            Instance = Resources.Load<TrailEffectRepo>("Repositories/TrailEffectRepo");
            if (Instance == null)
                Debug.LogError("TrailEffectRepo asset not found in Resources/Repositories/TrailEffectRepo");
        }
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, TrailResource> TrailEffects;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
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

    public TrailResource Get(string name)
    {
        var data = TrailEffects[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve trailInstance effect for `{name}`");

        return new TrailResource(data); //Return a new copy instead of a shared reference
    }

}
