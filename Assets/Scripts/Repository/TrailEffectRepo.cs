using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "TrailEffectRepo", menuName = "Repositories/TrailEffectRepo")]
public class TrailEffectRepo : ScriptableObject
{
    // Singleton instance
    private static TrailEffectRepo Instance;

    public static TrailEffectRepo instance
    {
        get
        {
            if (Instance == null)
            {
                Debug.LogWarning("TrailEffectRepo instance is null. Attempting to load synchronously.");
                LoadSynchronously();
            }

            if (Instance == null)
                Debug.LogError("TrailEffectRepo accessed before being initialized!");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void AutoInitialize()
    {
        if (Instance == null)
        {
            var handle = Addressables.LoadAssetAsync<TrailEffectRepo>("Repositories/TrailEffectRepo");
            Instance = await handle.Task;

            if (Instance == null)
                Debug.LogError("TrailEffectRepo asset not found in Addressables with key 'Repositories/TrailEffectRepo'");
        }
    }

    // Synchronous fallback for loading the TrailEffectRepo
    private static void LoadSynchronously()
    {
        var handle = Addressables.LoadAssetAsync<TrailEffectRepo>("Repositories/TrailEffectRepo");
        handle.WaitForCompletion(); // Block until the asset is loaded
        Instance = handle.Result;

        if (Instance == null)
            Debug.LogError("Failed to load TrailEffectRepo synchronously from Addressables.");
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, TrailEffectAsset> TrailEffects;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        TrailEffects = new Dictionary<string, TrailEffectAsset>
        {
            { "BlueGlow", new TrailEffectAsset
                {
                    Name = "BlueGlow",
                    Prefab = AssetHelper.LoadAsset<GameObject>($"TrailEffects/BlueGlow"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Bubble", new TrailEffectAsset
                {
                    Name = "Bubble",
                    Prefab = AssetHelper.LoadAsset<GameObject>($"TrailEffects/Bubble"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Feather", new TrailEffectAsset
                {
                    Name = "Feather",
                    Prefab = AssetHelper.LoadAsset<GameObject>($"TrailEffects/Feather"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Fireball", new TrailEffectAsset
                {
                    Name = "Fireball",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/Fireball"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.5f, 0.5f, 0.5f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Flame", new TrailEffectAsset
                {
                    Name = "Flame",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/Flame"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "GoldSparkle", new TrailEffectAsset
                {
                    Name = "GoldSparkle",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/GoldSparkle"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(2.5f, 2.5f, 2.5f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "GreenSparkle", new TrailEffectAsset
                {
                    Name = "GreenSparkle",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/GreenSparkle"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(2.5f, 2.5f, 2.5f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "IceSparkle", new TrailEffectAsset
                {
                    Name = "IceSparkle",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/IceSparkle"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "PinkDust", new TrailEffectAsset
                {
                    Name = "PinkDust",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/PinkDust"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "RosePetal", new TrailEffectAsset
                {
                    Name = "RosePetal",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/RosePetal"),
                    RelativeOffset = new Vector3(0, 0, 0),
                    AngularRotation = new Vector3(0, 0, 0),
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "StarSparkle", new TrailEffectAsset
                {
                    Name = "StarSparkle",
                    Prefab = AssetHelper.LoadAsset < GameObject >($"TrailEffects/StarSparkle"),
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

    public TrailEffectAsset Get(string name)
    {
        var data = TrailEffects[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve trailInstance effect for `{name}`");

        return new TrailEffectAsset(data); //Return a new copy instead of a shared reference
    }

}
