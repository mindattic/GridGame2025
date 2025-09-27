using Assets.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Libraries
{
    public static class VfxLibrary
    {
        private static Dictionary<string, VFXAsset> visualEffects;
        private static bool isLoaded = false;

        public static Dictionary<string, VFXAsset> VisualEffects
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

            GameObject LoadPrefab(string key) => AssetHelper.LoadAsset<GameObject>(key);

            visualEffects = new Dictionary<string, VFXAsset>
                {
                    {
                        "AcidSplash",
                        new VFXAsset
                        {
                            Name = "AcidSplash",
                            Prefab = LoadPrefab("Vfx/AcidSplash"),
                            RelativeOffset = new Vector3(0f, 0.01f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "AirSlash",
                        new VFXAsset
                        {
                            Name = "AirSlash",
                            Prefab = LoadPrefab("Vfx/AirSlash"),
                            RelativeOffset = new Vector3(0.01f, -0.15f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.15f, 0.15f, 0.15f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BloodClaw",
                        new VFXAsset
                        {
                            Name = "BloodClaw",
                            Prefab = LoadPrefab("Vfx/BloodClaw"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.15f, 0.15f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueSlash1",
                        new VFXAsset
                        {
                            Name = "BlueSlash1",
                            Prefab = LoadPrefab("Vfx/BlueSlash1"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                            Apex = 0.12f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueSlash2",
                        new VFXAsset
                        {
                            Name = "BlueSlash2",
                            Prefab = LoadPrefab("Vfx/BlueSlash2"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueSlash3",
                        new VFXAsset
                        {
                            Name = "BlueSlash3",
                            Prefab = LoadPrefab("Vfx/BlueSlash3"),
                            RelativeOffset = new Vector3(0.02f, -0.15f, 0f),
                            AngularRotation = new Vector3(30f, 30f, 0f),
                            RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueSword",
                        new VFXAsset
                        {
                            Name = "BlueSword",
                            Prefab = LoadPrefab("Vfx/BlueSword"),
                            RelativeOffset = new Vector3(0f, 0.05f, 0f),
                            AngularRotation = new Vector3(30f, 30f, 0f),
                            RelativeScale = new Vector3(0.12f, 0.08f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueSword4X",
                        new VFXAsset
                        {
                            Name = "BlueSword4X",
                            Prefab = LoadPrefab("Vfx/BlueSword4X"),
                            RelativeOffset = new Vector3(-0.05f, -0.1f, 0f),
                            AngularRotation = new Vector3(30f, 30f, 0f),
                            RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                            Apex = 0f,
                            Duration = 3f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueYellowSword",
                        new VFXAsset
                        {
                            Name = "BlueYellowSword",
                            Prefab = LoadPrefab("Vfx/BlueYellowSword"),
                            RelativeOffset = new Vector3(0.03f, 0.01f, 0f),
                            AngularRotation = new Vector3(60f, 0f, 0f),
                            RelativeScale = new Vector3(0.07f, 0.07f, 0.07f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BlueYellowSword3X",
                        new VFXAsset
                        {
                            Name = "BlueYellowSword3X",
                            Prefab = LoadPrefab("Vfx/BlueYellowSword3X"),
                            RelativeOffset = new Vector3(0.02f, -0.05f, 0f),
                            AngularRotation = new Vector3(60f, 0f, 0f),
                            RelativeScale = new Vector3(0.07f, 0.07f, 0.07f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "BuffLife",
                        new VFXAsset
                        {
                            Name = "BuffLife",
                            Prefab = LoadPrefab("Vfx/BuffLife"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.16f, 0.16f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "DoubleClaw",
                        new VFXAsset
                        {
                            Name = "DoubleClaw",
                            Prefab = LoadPrefab("Vfx/DoubleClaw"),
                            RelativeOffset = new Vector3(-0.03f, -0.1f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.12f, 0.12f, 0f),
                            Apex = 0.22f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "FireRain",
                        new VFXAsset
                        {
                            Name = "FireRain",
                            Prefab = LoadPrefab("Vfx/FireRain"),
                            RelativeOffset = new Vector3(0.03f, -0.05f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                            Apex = 0f,
                            Duration = 4f,
                            IsLoop = false
                        }
                    },
                    {
                        "GodRays",
                        new VFXAsset
                        {
                            Name = "GodRays",
                            Prefab = LoadPrefab("Vfx/GodRays"),
                            RelativeOffset = new Vector3(0f, -0.25f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.07f, 0.07f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "GoldBuff",
                        new VFXAsset
                        {
                            Name = "GoldBuff",
                            Prefab = LoadPrefab("Vfx/GoldBuff"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "GreenBuff",
                        new VFXAsset
                        {
                            Name = "GreenBuff",
                            Prefab = LoadPrefab("Vfx/GreenBuff"),
                            RelativeOffset = new Vector3(0.02f, -0.25f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.08f, 0.08f, 0.08f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "HexShield",
                        new VFXAsset
                        {
                            Name = "HexShield",
                            Prefab = LoadPrefab("Vfx/HexShield"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.16f, 0.16f, 0.16f),
                            Apex = 0f,
                            Duration = 6f,
                            IsLoop = false
                        }
                    },
                    {
                        "LevelUp",
                        new VFXAsset
                        {
                            Name = "LevelUp",
                            Prefab = LoadPrefab("Vfx/LevelUp"),
                            RelativeOffset = new Vector3(0f, -0.15f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.3f, 0.3f, 0f),
                            Apex = 0f,
                            Duration = 3f,
                            IsLoop = false
                        }
                    },
                    {
                        "LightningExplosion",
                        new VFXAsset
                        {
                            Name = "LightningExplosion",
                            Prefab = LoadPrefab("Vfx/LightningExplosion"),
                            RelativeOffset = new Vector3(0f, -0.1f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                            Apex = 0f,
                            Duration = 3f,
                            IsLoop = false
                        }
                    },
                    {
                        "LightningStrike",
                        new VFXAsset
                        {
                            Name = "LightningStrike",
                            Prefab = LoadPrefab("Vfx/LightningStrike"),
                            RelativeOffset = new Vector3(-0.07f, 0.1f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.05f, 0.05f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "MoonFeather",
                        new VFXAsset
                        {
                            Name = "MoonFeather",
                            Prefab = LoadPrefab("Vfx/MoonFeather"),
                            RelativeOffset = new Vector3(0f, -0.02f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(4f, 4f, 0f),
                            Apex = 0f,
                            Duration = 3f,
                            IsLoop = false
                        }
                    },
                    {
                        "OrangeSlash",
                        new VFXAsset
                        {
                            Name = "OrangeSlash",
                            Prefab = LoadPrefab("Vfx/OrangeSlash"),
                            RelativeOffset = new Vector3(-0.12f, 0.01f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.03f, 0.03f, 0.03f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "PinkSpark",
                        new VFXAsset
                        {
                            Name = "PinkSpark",
                            Prefab = LoadPrefab("Vfx/PinkSpark"),
                            RelativeOffset = new Vector3(0f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.04f, 0.04f, 0.04f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "PuffyExplosion",
                        new VFXAsset
                        {
                            Name = "PuffyExplosion",
                            Prefab = LoadPrefab("Vfx/PuffyExplosion"),
                            RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.2f, 0.2f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "RayBlast",
                        new VFXAsset
                        {
                            Name = "RayBlast",
                            Prefab = LoadPrefab("Vfx/RayBlast"),
                            RelativeOffset = new Vector3(0.02f, -0.02f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.1f, 0.1f, 0f),
                            Apex = 0f,
                            Duration = 3f,
                            IsLoop = false
                        }
                    },
                    {
                        "RedSlash2X",
                        new VFXAsset
                        {
                            Name = "RedSlash2X",
                            Prefab = LoadPrefab("Vfx/RedSlash2X"),
                            RelativeOffset = new Vector3(0.05f, -0.07f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.08f, 0.08f, 0f),
                            Apex = 0f,
                            Duration = 1f,
                            IsLoop = false
                        }
                    },
                    {
                        "RedSword",
                        new VFXAsset
                        {
                            Name = "RedSword",
                            Prefab = LoadPrefab("Vfx/RedSword"),
                            RelativeOffset = new Vector3(-0.06f, 0.05f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 142f),
                            RelativeScale = new Vector3(0.2f, 0.2f, 0.2f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "RotaryKnife",
                        new VFXAsset
                        {
                            Name = "RotaryKnife",
                            Prefab = LoadPrefab("Vfx/RotaryKnife"),
                            RelativeOffset = new Vector3(0.03f, -0.05f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.25f, 0.25f, 0f),
                            Apex = 0f,
                            Duration = 1f,
                            IsLoop = false
                        }
                    },
                    {
                        "ToxicCloud",
                        new VFXAsset
                        {
                            Name = "ToxicCloud",
                            Prefab = LoadPrefab("Vfx/ToxicCloud"),
                            RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.15f, 0.15f, 0.15f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },
                    {
                        "YellowHit",
                        new VFXAsset
                        {
                            Name = "YellowHit",
                            Prefab = LoadPrefab("Vfx/YellowHit"),
                            RelativeOffset = new Vector3(-0.02f, 0f, 0f),
                            AngularRotation = new Vector3(0f, 0f, 0f),
                            RelativeScale = new Vector3(0.2f, 0.2f, 0f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = false
                        }
                    },

                    // Looping VFX
                    {
                        "BlueGlow",
                        new VFXAsset
                        {
                            Name = "BlueGlow",
                            Prefab = LoadPrefab("Vfx/Loop/BlueGlow"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "Bubble",
                        new VFXAsset
                        {
                            Name = "Bubble",
                            Prefab = LoadPrefab("Vfx/Loop/Bubble"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "Feather",
                        new VFXAsset
                        {
                            Name = "Feather",
                            Prefab = LoadPrefab("Vfx/Loop/Feather"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "Fireball",
                        new VFXAsset
                        {
                            Name = "Fireball",
                            Prefab = LoadPrefab("Vfx/Loop/Fireball"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "Flame",
                        new VFXAsset
                        {
                            Name = "Flame",
                            Prefab = LoadPrefab("Vfx/Loop/Flame"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "GoldSparkle",
                        new VFXAsset
                        {
                            Name = "GoldSparkle",
                            Prefab = LoadPrefab("Vfx/Loop/GoldSparkle"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                             RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "GreenSparkle",
                        new VFXAsset
                        {
                            Name = "GreenSparkle",
                            Prefab = LoadPrefab("Vfx/Loop/GreenSparkle"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(2f, 2f, 0f),
                            IsLoop = true
                        }
                    },
                    {
                        "IceSparkle",
                        new VFXAsset
                        {
                            Name = "IceSparkle",
                            Prefab = LoadPrefab("Vfx/Loop/IceSparkle"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            IsLoop = true
                        }
                    },
                    {
                        "PinkDust",
                        new VFXAsset
                        {
                            Name = "PinkDust",
                            Prefab = LoadPrefab("Vfx/Loop/PinkDust"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(1, 1, 1),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = true
                        }
                    },
                    {
                        "RosePetal",
                        new VFXAsset
                        {
                            Name = "RosePetal",
                            Prefab = LoadPrefab("Vfx/Loop/RosePetal"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = true
                        }
                    },
                    {
                        "StarSparkle",
                        new VFXAsset
                        {
                            Name = "StarSparkle",
                            Prefab = LoadPrefab("Vfx/Loop/StarSparkle"),
                            RelativeOffset = Vector3.zero,
                            AngularRotation = Vector3.zero,
                            RelativeScale = new Vector3(0.1f, 0.1f, 0.1f),
                            Apex = 0f,
                            Duration = 2f,
                            IsLoop = true
                        }
                    },
                };

            isLoaded = true;
        }

        public static VFXAsset Get(string name)
        {
            if (!isLoaded) Load();
            var data = visualEffects.ContainsKey(name) ? visualEffects[name] : null;
            if (data == null)
                Debug.LogError($"Unable to retrieve visual effect for `{name}`");
            return data != null ? new VFXAsset(data) : null;
        }
    }
}
