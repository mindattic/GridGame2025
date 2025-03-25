using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualEffectRepo", menuName = "Repositories/VisualEffectRepo")]
public class VisualEffectRepo : ScriptableObject
{
    //Singleton
    private static VisualEffectRepo Instance;

    public static VisualEffectRepo instance
    {
        get
        {
            if (Instance == null)
                Debug.LogError("VisualEffectRepo accessed before being initialized! Ensure it's assigned in Awake().");
            return Instance;
        }
    }

    //Assign
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            Instance = Resources.Load<VisualEffectRepo>("Repositories/VisualEffectRepo");
            if (Instance == null)
                Debug.LogError("VisualEffectRepo asset not found in Resources/Repositories/VisualEffectRepo");
        }
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, VFXData> VisualEffects;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
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

    public VFXData Get(string name)
    {
        var data = VisualEffects[name];
        if (data == null)
            Debug.LogError($"Unable to retrieve visual effect for `{name}`");

        return new VFXData(data); //Return a new copy instead of a shared reference
    }

}
