using Game.Behaviors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ResourceParameter
{
    public string Key;   //The parameter name (e.g., "Role", "Description")
    public string Value; //The parameter entry (e.g., "Tank", "A brave warrior")
}

[System.Serializable]
public class ResourceItem<T>
{
    public T Value;                     //The resource itself (e.g., Sprite, AudioClip)
    public List<ResourceParameter> Parameters; //Parameters loaded from the JSON file
}

public static class ResourceFolder
{
    public static string Backgrounds = "Backgrounds";
    public static string Portraits = "Portraits";
    public static string SoundEffects = "SoundEffects";
    public static string MusicTracks = "MusicTracks";
    public static string Materials = "Materials";
    public static string Seamless = "Seamless";
    public static string Sprites = "Sprites";
    public static string WeaponTypes = "Sprites/WeaponTypes";
    public static string VisualEffects = "VisualEffects";

}

//Helper class for deserializing parameter lists
[System.Serializable]
public class ParameterList
{
    public List<ResourceParameter> Parameters = new List<ResourceParameter>();
}

public class ResourceManager : MonoBehaviour
{
    #region Properties
    protected DataManager dataManager => GameManager.instance.dataManager;
    protected LogManager logManager => GameManager.instance.logManager;
    #endregion


    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> backgrounds = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Texture2D>> portraits = new Dictionary<string, ResourceItem<Texture2D>>();
    [SerializeField] public Dictionary<string, ResourceItem<AudioClip>> soundEffects = new Dictionary<string, ResourceItem<AudioClip>>();
    [SerializeField] public Dictionary<string, ResourceItem<AudioClip>> musicTracks = new Dictionary<string, ResourceItem<AudioClip>>();
    [SerializeField] public Dictionary<string, ResourceItem<Material>> materials = new Dictionary<string, ResourceItem<Material>>();
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> seamless = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> sprites = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> weaponTypes = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, VisualEffect> visualEffects = new Dictionary<string, VisualEffect>();

    public void Awake()
    {



    }

    public void Initialize()
    {
        List<string> keys = new List<string>();

        //Backgrounds
        keys.SetRange(
            "CandleLitPath");
        backgrounds = LoadResources<Sprite>(ResourceFolder.Backgrounds, keys);

        //Portraits
        keys.SetRange(
            "Barbarian", "Bat", "Cleric", "Ninja", "Paladin", "PandaGirl", "Scorpion", "Sentinel", "Slime", "Yeti");
        portraits = LoadResources<Texture2D>(ResourceFolder.Portraits, keys);

        //Sound Effects
        keys.SetRange(
            "Death", "Move1", "Move2", "Move3", "Move4", "Move5", "Move6", "NextTurn", "PlayerGlow", "Portrait", "Rumble",
            "Select", "Slash1", "Slash2", "Slash3", "Slash4", "Slash5", "Slash6", "Slash7", "Slide");
        soundEffects = LoadResources<AudioClip>(ResourceFolder.SoundEffects, keys);

        //Music Tracks
        keys.SetRange(
            "MelancholyLull");
        musicTracks = LoadResources<AudioClip>(ResourceFolder.MusicTracks, keys);

        //Materials
        keys.SetRange(
            "EnemyParallax", "PlayerParallax");
        materials = LoadResources<Material>(ResourceFolder.Materials, keys);

        //Seamless Sprites
        keys.SetRange(
            "BlackFire1", "BlackFire2", "Fire1", "RedFire1", "Swords1", "Swords2", "WhiteFire1", "WhiteFire2");
        seamless = LoadResources<Sprite>(ResourceFolder.Seamless, keys);

        //Sprites
        keys.SetRange(
            "DottedLine", "DottedLineArrow", "DottedLineTurn", "Footstep", "Pause", "Paused");
        sprites = LoadResources<Sprite>(ResourceFolder.Sprites, keys);

        //Weapon Types
        keys.SetRange(
            "Bow", "Claw", "Crossbow", "Dagger", "Grenade", "Hammer", "Katana", "Mace", "Pistol", "Polearm", "Potion",
            "Scythe", "Shield", "Shuriken", "Spear", "Staff", "Sword", "Wand");
        weaponTypes = LoadResources<Sprite>(ResourceFolder.WeaponTypes, keys);

        //Visual Effects Types
        keys.SetRange(
            "AcidSplash", "AirSlash", "BloodClaw", "BlueSlash1", "BlueSlash2", "BlueSlash3", "BlueSword", "BlueSword4X",
            "BlueYellowSword", "BlueYellowSword3X", "BuffLife", "DoubleClaw", "FireRain", "GodRays", "GoldBuff",
            "GreenBuff", "HexShield", "LevelUp", "LightningExplosion", "LightningStrike",
            "MoonFeather", "OrangeSlash", "PinkSpark", "PuffyExplosion", "RayBlast", "RedSlash2X", "RedSword",
            "RotaryKnife", "ToxicCloud", "YellowHit");
        visualEffects = LoadVisualEffects(keys);
    }

    public ResourceItem<Sprite> Background(string key)
    {
        if (backgrounds.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve background `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Texture2D> Portrait(string key)
    {
        if (portraits.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve portrait texture2D `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<AudioClip> SoundEffect(string key)
    {
        if (soundEffects.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve sound effect `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<AudioClip> MusicTrack(string key)
    {
        if (musicTracks.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve music track `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Material> Material(string key, Texture2D texture = null)
    {
        static bool SetMaterialTexture(ref ResourceItem<Material> entry, Texture2D texture = null)
        {
            if (texture != null)
                entry.Value.mainTexture = texture;
            return true;
        }

        if (materials.TryGetValue(key, out var entry) && SetMaterialTexture(ref entry, texture))
            return entry;

        logManager.Error($"Failed to retrieve material `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Sprite> Seamless(string key)
    {
        if (seamless.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve seamless sprite `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Sprite> Sprite(string key)
    {
        if (sprites.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve sprite `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Sprite> WeaponType(string key)
    {
        if (weaponTypes.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve weapon type sprite `{key}` from resource manager.");
        return null;
    }

    public VisualEffect VisualEffect(string key)
    {
        if (visualEffects.TryGetValue(key, out var entry))
            return entry;

        logManager.Error($"Failed to retrieve visual effect `{key}` from resource manager.");
        return null;
    }

    ///<summary>
    ///Load all resources from a folder and their matching JSON parameters.
    ///</summary>
    //public List<ResourceItem<Sprite>> LoadFolder(string resourcePath)
    //{
    //   Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);
    //   List<ResourceItem<Sprite>> entries = new List<ResourceItem<Sprite>>();

    //   foreach (var sprite in sprites)
    //   {
    //       //Load matching JSON file for parameters
    //       List<ResourceParameter> parameters = LoadParameters(resourcePath, sprite.name);

    //       entries.SpawnActor(new ResourceItem<Sprite>
    //       {
    //           Key = sprite.name,
    //           Value = sprite,
    //           Parameters = parameters
    //       });
    //   }

    //   return entries;
    //}

    ///<summary>
    ///Load specific resources by their IDs and matching JSON parameters.
    ///</summary>
    ///


    public T Load<T>(string resourcePath) where T : UnityEngine.Object
    {
        T resource = Resources.Load<T>(resourcePath);
        if (resource == null)
            logManager.Error($"Failed to load external resource from `{resourcePath}`");

        return resource;
    }


    public Dictionary<string, ResourceItem<T>> LoadResources<T>(string resourcePath, List<string> keys) where T : UnityEngine.Object
    {
        Dictionary<string, ResourceItem<T>> entries = new Dictionary<string, ResourceItem<T>>();

        try
        {
            foreach (var key in keys)
            {
                //Load the sprite
                T value = Resources.Load<T>($"{resourcePath}/{key}");
                if (value == null)
                {
                    logManager.Error($"Resource `{key}` not found at resource path `{resourcePath}`");
                    continue;
                }

                //Load the matching JSON file for parameters
                List<ResourceParameter> parameters = LoadParameters(resourcePath, key);

                entries.Add(key, new ResourceItem<T>
                {
                    Value = value,
                    Parameters = parameters
                });
            }
        }
        catch (Exception ex)
        {
            logManager.Error(ex.Message);
        }

        return entries;
    }





    ///<summary>
    ///Load parameters from a JSON file matching the resource name.
    ///</summary>
    private List<ResourceParameter> LoadParameters(string folderPath, string resourceName)
    {
        string jsonPath = Path.Combine(Application.dataPath, "Resources", folderPath, $"{resourceName}.json");

        if (!File.Exists(jsonPath))
            return null;

        string json = File.ReadAllText(jsonPath);
        return JsonConvert.DeserializeObject<ParameterList>(json).Parameters;
    }



    public Dictionary<string, VisualEffect> LoadVisualEffects(List<string> keys)
    {
        Dictionary<string, VisualEffect> entries = new Dictionary<string, VisualEffect>();

        try
        {
            foreach (var key in keys)
            {
                //DEBUG: Should the JSON parsing be here in the Resource Manager? Or depend on OOO?...
                var data = dataManager.GetVisualEffect(key);
                if (data == null)
                {
                    logManager.Error($"Visual Effect Entity `{key}` is null");
                    continue;
                }

                var resourcePath = ResourceFolder.VisualEffects.ToString();
                var prefab = Resources.Load<GameObject>($"{resourcePath}/{key}");
                if (prefab == null)
                {
                    logManager.Error($"Visual Effect Prefab `{key}` not found at resource path `{resourcePath}`");
                    continue;
                }

                var visualEffect = new VisualEffect()
                {
                    Name = key,
                    Prefab = prefab,
                    RelativeOffset = Convert.ToVector3(data.RelativeOffset),
                    AngularRotation = Convert.ToVector3(data.AngularRotation),
                    RelativeScale = Convert.ToVector3(data.RelativeScale),
                    Delay = data.Delay,
                    Duration = data.Duration,
                    IsLoop = data.IsLoop,
                };

                entries.Add(key, visualEffect);
            }
        }
        catch (Exception ex)
        {
            logManager.Error(ex.Message);
        }

        return entries;
    }






    //[SerializeField] public List<VisualEffect> visualEffects = new List<VisualEffect>();


    //public VisualEffect VisualEffect(string id)
    //{
    //   try
    //   {
    //       return visualEffects.First(x => x.id.Equals(id));
    //   }
    //   catch (Exception ex)
    //   {
    //       logManager.Error($"Failed to retrieve visual effect `{id}` from resource manager. | Error: {ex.Message}");
    //   }

    //   return null;
    //}

}
