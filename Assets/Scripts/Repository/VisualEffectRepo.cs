using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public static class VisualEffectRepo
{
    private static Dictionary<string, VisualEffectAsset> visualEffects;
    private static bool isLoaded = false;

    public static Dictionary<string, VisualEffectAsset> VisualEffects
    {
        get
        {
            if (!isLoaded)
                Load();
            return visualEffects;
        }
    }

    private static void Load()
    {
        if (isLoaded) return;

        visualEffects = new Dictionary<string, VisualEffectAsset>
        {
            { "AcidSplash", new VisualEffectAsset
                {
                Name = "AcidSplash",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/AcidSplash"),
                RelativeOffset = new Vector3(0f, 0.01f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "AirSlash", new VisualEffectAsset
                {
                Name = "AirSlash",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/AirSlash"),
                RelativeOffset = new Vector3(0.01f, -0.15f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.15f, 0.15f, 0.15f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BloodClaw", new VisualEffectAsset
            {
                Name = "BloodClaw",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BloodClaw"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.15f, 0.15f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSlash1", new VisualEffectAsset
                {
                Name = "BlueSlash1",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueSlash1"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(45f, 0f, -45f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0.12f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSlash2", new VisualEffectAsset
                {
                Name = "BlueSlash2",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueSlash2"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSlash3", new VisualEffectAsset
                {
                Name = "BlueSlash3",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueSlash3"),
                RelativeOffset = new Vector3(0.02f, -0.15f, 0f),
                AngularRotation = new Vector3(30f, 30f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSword", new VisualEffectAsset
                {
                Name = "BlueSword",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueSword"),
                RelativeOffset = new Vector3(0f, 0.05f, 0f),
                AngularRotation = new Vector3(30f, 30f, 0f),
                RelativeScale = new Vector3(0.12f, 0.08f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BlueSword4X", new VisualEffectAsset
                {
                Name = "BlueSword4X",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueSword4X"),
                RelativeOffset = new Vector3(-0.05f, -0.1f, 0f),
                AngularRotation = new Vector3(30f, 30f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "BlueYellowSword", new VisualEffectAsset
                {
                Name = "BlueYellowSword",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueYellowSword"),
                RelativeOffset = new Vector3(0.03f, 0.01f, 0f),
                AngularRotation = new Vector3(60f, 0f, 0f),
                RelativeScale = new Vector3(0.07f, 0.07f, 0.07f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
             }
            },
            { "BlueYellowSword3X", new VisualEffectAsset
                {
                Name = "BlueYellowSword3X",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BlueYellowSword3X"),
                RelativeOffset = new Vector3(0.02f, -0.05f, 0f),
                AngularRotation = new Vector3(60f, 0f, 0f),
                RelativeScale = new Vector3(0.07f, 0.07f, 0.07f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "BuffLife", new VisualEffectAsset
                {
                Name = "BuffLife",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/BuffLife"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.16f, 0.16f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "DoubleClaw", new VisualEffectAsset
                {
                Name = "DoubleClaw",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/DoubleClaw"),
                RelativeOffset = new Vector3(-0.03f, -0.1f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.12f, 0.12f, 0f),
                Delay = 0.22f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "FireRain", new VisualEffectAsset
                {
                Name = "FireRain",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/FireRain"),
                RelativeOffset = new Vector3(0.03f, -0.05f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                Delay = 0f,
                Duration = 4f,
                IsLoop = false
                }
            },
            { "GodRays", new VisualEffectAsset
                {
                Name = "GodRays",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/GodRays"),
                RelativeOffset = new Vector3(0f, -0.25f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.07f, 0.07f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "GoldBuff", new VisualEffectAsset
                {
                Name = "GoldBuff",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/GoldBuff"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "GreenBuff", new VisualEffectAsset
                {
                Name = "GreenBuff",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/GreenBuff"),
                RelativeOffset = new Vector3(0.02f, -0.25f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "HexShield", new VisualEffectAsset
                {
                Name = "HexShield",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/HexShield"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.16f, 0.16f, 0.16f),
                Delay = 0f,
                Duration = 6f,
                IsLoop = false
                }
            },
            { "LevelUp", new VisualEffectAsset
                {
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/LevelUp"),
                Name = "LevelUp",
                RelativeOffset = new Vector3(0f, -0.15f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.3f, 0.3f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "LightningExplosion", new VisualEffectAsset
                {
                Name = "LightningExplosion",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/LightningExplosion"),
                RelativeOffset = new Vector3(0f, -0.1f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "LightningStrike", new VisualEffectAsset
                {
                Name = "LightningStrike",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/LightningStrike"),
                RelativeOffset = new Vector3(-0.07f, 0.1f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.05f, 0.05f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "MoonFeather", new VisualEffectAsset
                {
                Name = "MoonFeather",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/MoonFeather"),
                RelativeOffset = new Vector3(0f, -0.02f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(4f, 4f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "OrangeSlash", new VisualEffectAsset
                {
                Name = "OrangeSlash",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/OrangeSlash"),
                RelativeOffset = new Vector3(-0.12f, 0.01f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.03f, 0.03f, 0.03f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "PinkSpark", new VisualEffectAsset
                {
                Name = "PinkSpark",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/PinkSpark"),
                RelativeOffset = new Vector3(0f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.04f, 0.04f, 0.04f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "PuffyExplosion", new VisualEffectAsset
                {
                Name = "PuffyExplosion",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/PuffyExplosion"),
                RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.2f, 0.2f, 0f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "RayBlast", new VisualEffectAsset
                {
                Name = "RayBlast",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/RayBlast"),
                RelativeOffset = new Vector3(0.02f, -0.02f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                Delay = 0f,
                Duration = 3f,
                IsLoop = false
                }
            },
            { "RedSlash2X", new VisualEffectAsset
                {
                Name = "RedSlash2X",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/RedSlash2X"),
                RelativeOffset = new Vector3(0.05f, -0.07f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                Delay = 0f,
                Duration = 1f,
                IsLoop = false
                }
            },
            { "RedSword", new VisualEffectAsset
                {
                Name = "RedSword",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/RedSword"),
                RelativeOffset = new Vector3(-0.06f, 0.05f, 0f),
                AngularRotation = new Vector3(0f, 0f, 142f),
                RelativeScale = new Vector3(0.2f, 0.2f, 0.2f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "RotaryKnife", new VisualEffectAsset
                {
                Name = "RotaryKnife",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/RotaryKnife"),
                RelativeOffset = new Vector3(0.03f, -0.05f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.25f, 0.25f, 0f),
                Delay = 0f,
                Duration = 1f,
                IsLoop = false
                }
            },
            { "ToxicCloud", new VisualEffectAsset
                {
                Name = "ToxicCloud",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/ToxicCloud"),
                RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                AngularRotation = new Vector3(0f, 0f, 0f),
                RelativeScale = new Vector3(0.15f, 0.15f, 0.15f),
                Delay = 0f,
                Duration = 2f,
                IsLoop = false
                }
            },
            { "YellowHit", new VisualEffectAsset
                {
                Name = "YellowHit",
                Prefab = AssetHelper.LoadAsset<GameObject>($"VisualEffects/YellowHit"),
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

    public static VisualEffectAsset Get(string name)
    {
        var data = visualEffects[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve visual effect for `{name}`");

        return new VisualEffectAsset(data); //Return a new copy instead of a shared reference
    }

}
