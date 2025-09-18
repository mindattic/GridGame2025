using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using g = Assets.Helpers.GameHelper;

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
            visualEffects = new Dictionary<string, VFXAsset>
            {
                { "AcidSplash", new VFXAsset { Name = "AcidSplash", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/AcidSplash"), RelativeOffset = new Vector3(0f, 0.01f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "AirSlash", new VFXAsset { Name = "AirSlash", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/AirSlash"), RelativeOffset = new Vector3(0.01f, -0.15f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.15f, 0.15f, 0.15f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BloodClaw", new VFXAsset { Name = "BloodClaw", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BloodClaw"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.15f, 0.15f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BlueSlash1", new VFXAsset { Name = "BlueSlash1", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueSlash1"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0, 0f, 0), RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0.12f, Duration = 2f, IsLoop = false } },
                { "BlueSlash2", new VFXAsset { Name = "BlueSlash2", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueSlash2"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BlueSlash3", new VFXAsset { Name = "BlueSlash3", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueSlash3"), RelativeOffset = new Vector3(0.02f, -0.15f, 0f), AngularRotation = new Vector3(30f, 30f, 0f), RelativeScale = new Vector3(0.08f, 0.08f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BlueSword", new VFXAsset { Name = "BlueSword", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueSword"), RelativeOffset = new Vector3(0f, 0.05f, 0f), AngularRotation = new Vector3(30f, 30f, 0f), RelativeScale = new Vector3(0.12f, 0.08f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BlueSword4X", new VFXAsset { Name = "BlueSword4X", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueSword4X"), RelativeOffset = new Vector3(-0.05f, -0.1f, 0f), AngularRotation = new Vector3(30f, 30f, 0f), RelativeScale = new Vector3(0.08f, 0.08f, 0f), Delay = 0f, Duration = 3f, IsLoop = false } },
                { "BlueYellowSword", new VFXAsset { Name = "BlueYellowSword", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueYellowSword"), RelativeOffset = new Vector3(0.03f, 0.01f, 0f), AngularRotation = new Vector3(60f, 0f, 0f), RelativeScale = new Vector3(0.07f, 0.07f, 0.07f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BlueYellowSword3X", new VFXAsset { Name = "BlueYellowSword3X", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BlueYellowSword3X"), RelativeOffset = new Vector3(0.02f, -0.05f, 0f), AngularRotation = new Vector3(60f, 0f, 0f), RelativeScale = new Vector3(0.07f, 0.07f, 0.07f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BuffLife", new VFXAsset { Name = "BuffLife", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/BuffLife"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.16f, 0.16f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "DoubleClaw", new VFXAsset { Name = "DoubleClaw", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/DoubleClaw"), RelativeOffset = new Vector3(-0.03f, -0.1f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.12f, 0.12f, 0f), Delay = 0.22f, Duration = 2f, IsLoop = false } },
                { "FireRain", new VFXAsset { Name = "FireRain", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/FireRain"), RelativeOffset = new Vector3(0.03f, -0.05f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.1f, 0.1f, 0f), Delay = 0f, Duration = 4f, IsLoop = false } },
                { "GodRays", new VFXAsset { Name = "GodRays", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/GodRays"), RelativeOffset = new Vector3(0f, -0.25f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.07f, 0.07f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "GoldBuff", new VFXAsset { Name = "GoldBuff", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/GoldBuff"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.08f, 0.08f, 0.08f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "GreenBuff", new VFXAsset { Name = "GreenBuff", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/GreenBuff"), RelativeOffset = new Vector3(0.02f, -0.25f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.08f, 0.08f, 0.08f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "HexShield", new VFXAsset { Name = "HexShield", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/HexShield"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.16f, 0.16f, 0.16f), Delay = 0f, Duration = 6f, IsLoop = false } },
                { "LevelUp", new VFXAsset { Name = "LevelUp", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/LevelUp"), RelativeOffset = new Vector3(0f, -0.15f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.3f, 0.3f, 0f), Delay = 0f, Duration = 3f, IsLoop = false } },
                { "LightningExplosion", new VFXAsset { Name = "LightningExplosion", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/LightningExplosion"), RelativeOffset = new Vector3(0f, -0.1f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.1f, 0.1f, 0f), Delay = 0f, Duration = 3f, IsLoop = false } },
                { "LightningStrike", new VFXAsset { Name = "LightningStrike", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/LightningStrike"), RelativeOffset = new Vector3(-0.07f, 0.1f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.05f, 0.05f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "MoonFeather", new VFXAsset { Name = "MoonFeather", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/MoonFeather"), RelativeOffset = new Vector3(0f, -0.02f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(4f, 4f, 0f), Delay = 0f, Duration = 3f, IsLoop = false } },
                { "OrangeSlash", new VFXAsset { Name = "OrangeSlash", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/OrangeSlash"), RelativeOffset = new Vector3(-0.12f, 0.01f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.03f, 0.03f, 0.03f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "PinkSpark", new VFXAsset { Name = "PinkSpark", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/PinkSpark"), RelativeOffset = new Vector3(0f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.04f, 0.04f, 0.04f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "PuffyExplosion", new VFXAsset { Name = "PuffyExplosion", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/PuffyExplosion"), RelativeOffset = new Vector3(-0.02f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.2f, 0.2f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "RayBlast", new VFXAsset { Name = "RayBlast", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/RayBlast"), RelativeOffset = new Vector3(0.02f, -0.02f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.1f, 0.1f, 0f), Delay = 0f, Duration = 3f, IsLoop = false } },
                { "RedSlash2X", new VFXAsset { Name = "RedSlash2X", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/RedSlash2X"), RelativeOffset = new Vector3(0.05f, -0.07f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.08f, 0.08f, 0f), Delay = 0f, Duration = 1f, IsLoop = false } },
                { "RedSword", new VFXAsset { Name = "RedSword", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/RedSword"), RelativeOffset = new Vector3(-0.06f, 0.05f, 0f), AngularRotation = new Vector3(0f, 0f, 142f), RelativeScale = new Vector3(0.2f, 0.2f, 0.2f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "RotaryKnife", new VFXAsset { Name = "RotaryKnife", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/RotaryKnife"), RelativeOffset = new Vector3(0.03f, -0.05f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.25f, 0.25f, 0f), Delay = 0f, Duration = 1f, IsLoop = false } },
                { "ToxicCloud", new VFXAsset { Name = "ToxicCloud", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/ToxicCloud"), RelativeOffset = new Vector3(-0.02f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.15f, 0.15f, 0.15f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "YellowHit", new VFXAsset { Name = "YellowHit", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/YellowHit"), RelativeOffset = new Vector3(-0.02f, 0f, 0f), AngularRotation = new Vector3(0f, 0f, 0f), RelativeScale = new Vector3(0.2f, 0.2f, 0f), Delay = 0f, Duration = 2f, IsLoop = false } },
                { "BlueGlow", new VFXAsset { Name = "BlueGlow", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/BlueGlow"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "Bubble", new VFXAsset { Name = "Bubble", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/Bubble"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "Feather", new VFXAsset { Name = "Feather", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/Feather"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "Fireball", new VFXAsset { Name = "Fireball", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/Fireball"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.25f, 0.25f, 0.25f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "Flame", new VFXAsset { Name = "Flame", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/Flame"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.08f, 0.08f, 0.08f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "GoldSparkle", new VFXAsset { Name = "GoldSparkle", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/GoldSparkle"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(2.5f, 2.5f, 2.5f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "GreenSparkle", new VFXAsset { Name = "GreenSparkle", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/GreenSparkle"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(4.5f, 4.5f, 4.5f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "IceSparkle", new VFXAsset { Name = "IceSparkle", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/IceSparkle"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "PinkDust", new VFXAsset { Name = "PinkDust", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/PinkDust"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "RosePetal", new VFXAsset { Name = "RosePetal", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/RosePetal"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
                { "StarSparkle", new VFXAsset { Name = "StarSparkle", Prefab = AssetHelper.LoadAsset<GameObject>("Vfx/Loop/StarSparkle"), RelativeOffset = Vector3.zero, AngularRotation = Vector3.zero, RelativeScale = new Vector3(0.1f, 0.1f, 0.1f), Delay = 0f, Duration = 2f, IsLoop = true } },
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
