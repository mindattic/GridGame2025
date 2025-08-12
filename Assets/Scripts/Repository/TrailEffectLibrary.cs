using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
public static class TrailEffectLibrary
{
    private static Dictionary<string, TrailEffectAsset> trailEffects;
    private static bool isLoaded = false;

    public static Dictionary<string, TrailEffectAsset> TrailEffects
    {
        get
        {
            if (!isLoaded)
                Load();
            return trailEffects;
        }
    }

    private static void Load()
    {
        if (isLoaded) return;

        trailEffects = new Dictionary<string, TrailEffectAsset>
        {
            { "BlueGlow", new TrailEffectAsset
                {
                    Name = "BlueGlow",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/BlueGlow"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Bubble", new TrailEffectAsset
                {
                    Name = "Bubble",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/Bubble"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Feather", new TrailEffectAsset
                {
                    Name = "Feather",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/Feather"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Fireball", new TrailEffectAsset
                {
                    Name = "Fireball",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/Fireball"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.5f, 0.5f, 0.5f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "Flame", new TrailEffectAsset
                {
                    Name = "Flame",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/Flame"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "GoldSparkle", new TrailEffectAsset
                {
                    Name = "GoldSparkle",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/GoldSparkle"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(2.5f, 2.5f, 2.5f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "GreenSparkle", new TrailEffectAsset
                {
                    Name = "GreenSparkle",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/GreenSparkle"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(2.5f, 2.5f, 2.5f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "IceSparkle", new TrailEffectAsset
                {
                    Name = "IceSparkle",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/IceSparkle"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "PinkDust", new TrailEffectAsset
                {
                    Name = "PinkDust",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/PinkDust"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "RosePetal", new TrailEffectAsset
                {
                    Name = "RosePetal",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/RosePetal"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            },
            { "StarSparkle", new TrailEffectAsset
                {
                    Name = "StarSparkle",
                    Prefab = AssetHelper.LoadAsset<GameObject>("TrailEffects/StarSparkle"),
                    RelativeOffset = Vector3.zero,
                    AngularRotation = Vector3.zero,
                    RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                    Delay = 0f,
                    Duration = 2f,
                    IsLoop = true
                }
            }
        };

        isLoaded = true;
    }

    public static TrailEffectAsset Get(string name)
    {
        if (!TrailEffects.ContainsKey(name))
        {
            Debug.LogError($"Unable to retrieve trail effect for `{name}`");
            return null;
        }

        return new TrailEffectAsset(TrailEffects[name]);  // Return a new copy
    }
}
