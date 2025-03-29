using Assets.Scripts.Models;
using Assets.Scripts.Repositories;
using Assets.Scripts.Utilities;
using Game.Behaviors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class ResourceManager : MonoBehaviour
{
    //Quick Reference Properties
    //protected LogManager logManager => GameManager.instance.logManager;

    //Fields
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> backgrounds = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Texture2D>> portraits = new Dictionary<string, ResourceItem<Texture2D>>();
    [SerializeField] public Dictionary<string, ResourceItem<AudioClip>> soundEffects = new Dictionary<string, ResourceItem<AudioClip>>();
    [SerializeField] public Dictionary<string, ResourceItem<AudioClip>> musicTracks = new Dictionary<string, ResourceItem<AudioClip>>();
    [SerializeField] public Dictionary<string, ResourceItem<Material>> materials = new Dictionary<string, ResourceItem<Material>>();
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> seamless = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> sprites = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Sprite>> weaponTypes = new Dictionary<string, ResourceItem<Sprite>>();
    [SerializeField] public Dictionary<string, ResourceItem<Texture2D>> textures = new Dictionary<string, ResourceItem<Texture2D>>();
    [SerializeField] public Dictionary<string, TrailResource> trailEffects = new Dictionary<string, TrailResource>();
    [SerializeField] public Dictionary<string, Tutorial> tutorials = new Dictionary<string, Tutorial>();
    [SerializeField] public Dictionary<string, VFXResource> visualEffects = new Dictionary<string, VFXResource>();

    public void Awake()
    {
    }

    public void Initialize()
    {
        List<string> keys = new List<string>();

        //Backgrounds
        keys.SetRange(
            "CandleLitPath");
        backgrounds = LoadResources<Sprite>(ResourceFolderHelper.Backgrounds, keys);

        //Portraits
        keys.SetRange(
            "Barbarian", "Bat", "Cleric", "Ninja", "Paladin", "PandaGirl", "Scorpion", "Sentinel", "Slime", "Yeti");
        portraits = LoadResources<Texture2D>(ResourceFolderHelper.Portraits, keys);

        //Sound Effects
        keys.SetRange(
            "Click", "Death", "Move1", "Move2", "Move3", "Move4", "Move5", "Move6", "NextTurn", "PlayerGlow", "Portrait", 
            "Rumble", "Slash1", "Slash2", "Slash3", "Slash4", "Slash5", "Slash6", "Slash7", "Slide");
        soundEffects = LoadResources<AudioClip>(ResourceFolderHelper.SoundEffects, keys);

        //Music Tracks
        keys.SetRange(
            "MelancholyLull");
        musicTracks = LoadResources<AudioClip>(ResourceFolderHelper.MusicTracks, keys);

        //Materials
        keys.SetRange(
            "EnemyParallax", "PlayerParallax");
        materials = LoadResources<Material>(ResourceFolderHelper.Materials, keys);

        //Seamless Sprites
        keys.SetRange(
            "BlackFire1", "BlackFire2", "Fire1", "RedFire1", "Swords1", "Swords2", "WhiteFire1", "WhiteFire2");
        seamless = LoadResources<Sprite>(ResourceFolderHelper.Seamless, keys);

        //Sprites
        keys.SetRange(
            "DottedLine", "DottedLineArrow", "DottedLineTurn", "Footstep", "Pause", "Paused", "Forest");
        sprites = LoadResources<Sprite>(ResourceFolderHelper.Sprites, keys);

        //Weapon Types
        keys.SetRange(
            "Bow", "Claw", "Crossbow", "Dagger", "Grenade", "Hammer", "Katana", "Mace", "Pistol", "Polearm", "Potion",
            "Scythe", "Shield", "Shuriken", "Spear", "Staff", "Sword", "Wand");
        weaponTypes = LoadResources<Sprite>(ResourceFolderHelper.WeaponTypes, keys);

        //Textures
        keys.SetRange(
            "Tutorial.1-1", "Tutorial.1-2", "Tutorial.1-3");
        textures = LoadResources<Texture2D>(ResourceFolderHelper.Textures, keys);

        //Trail Effects
        keys.SetRange(
            "BlueGlow", "Bubble", "Feather", "Fireball", "Flame", "GoldSparkle", "GreenSparkle", "IceSparkle",
            "PinkDust", "RosePetal", "StarSparkle");
        trailEffects = LoadTrailEffects(keys);

        //Tutorials
        keys.SetRange(
            "Tutorial1");
        tutorials = LoadTutorials(keys);

        //Visual Effects
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
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (backgrounds.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve background `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Texture2D> Portrait(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (portraits.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve portrait texture2D `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<AudioClip> SoundEffect(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (soundEffects.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve sound effect `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<AudioClip> MusicTrack(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (musicTracks.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve music track `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Material> Material(string key, Texture2D texture = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        static bool SetMaterialTexture(ref ResourceItem<Material> entry, Texture2D texture = null)
        {
            if (texture != null)
                entry.Value.mainTexture = texture;
            return true;
        }

        if (materials.TryGetValue(key, out var entry) && SetMaterialTexture(ref entry, texture))
            return entry;

        Debug.Log($"Failed to retrieve material `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Sprite> Seamless(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (seamless.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve seamless sprite `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Sprite> Sprite(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (sprites.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve sprite `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Sprite> WeaponType(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (weaponTypes.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve weapon type sprite `{key}` from resource manager.");
        return null;
    }

    public ResourceItem<Texture2D> Texture(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (textures.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve texture2D `{key}` from resource manager.");
        return null;
    }

    public Tutorial Tutorial(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (tutorials.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve tutorial `{key}` from resource manager.");
        return null;
    }

    public VFXResource VisualEffect(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (visualEffects.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve visual effect `{key}` from resource manager.");
        return null;
    }

    public TrailResource TrailEffect(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (trailEffects.TryGetValue(key, out var entry))
            return entry;

        Debug.Log($"Failed to retrieve trailInstance effect `{key}` from resource manager.");
        return null;
    }


    public T Load<T>(string resourcePath) where T : UnityEngine.Object
    {
        T resource = Resources.Load<T>(resourcePath);
        if (resource == null)
            Debug.Log($"Failed to load external resource from `{resourcePath}`");

        return resource;
    }

    private Dictionary<string, ResourceItem<T>> LoadResources<T>(string resourcePath, List<string> keys) where T : UnityEngine.Object
    {
        Dictionary<string, ResourceItem<T>> entries = new Dictionary<string, ResourceItem<T>>();

        try
        {
            foreach (var key in keys)
            {
                //Assign the sprite
                T value = Resources.Load<T>($"{resourcePath}/{key}");
                if (value == null)
                {
                    Debug.Log($"Resource `{key}` not found at resource path `{resourcePath}`");
                    continue;
                }

                //Assign the matching JSON file for parameters
                List<ResourceParameter> parameters = LoadResourceParameters(resourcePath, key);

                entries.Add(key, new ResourceItem<T>
                {
                    Value = value,
                    Parameters = parameters
                });
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return entries;
    }

    ///<summary>
    ///Assign parameters from a JSON file matching the resource name.
    ///</summary>
    private List<ResourceParameter> LoadResourceParameters(string folderPath, string resourceName)
    {
        string jsonPath = Path.Combine(Application.dataPath, "Resources", folderPath, $"{resourceName}.json");

        if (!File.Exists(jsonPath))
            return null;

        string json = File.ReadAllText(jsonPath);
        return JsonConvert.DeserializeObject<ResourceParameterList>(json).Parameters;
    }

    public Dictionary<string, TrailResource> LoadTrailEffects(List<string> keys)
    {
        Dictionary<string, TrailResource> entries = new Dictionary<string, TrailResource>();

        try
        {
            foreach (var key in keys)
            {
                //DEBUG: Should the JSON parsing be here in the Resource Manager? Or depend on OOO?...
                var data = TrailEffectRepo.instance.Get(key);
                if (data == null)
                {
                    Debug.Log($"Trail Effect Entry `{key}` is null");
                    continue;
                }

                var resourcePath = ResourceFolderHelper.TrailEffects.ToString();
                var prefab = Resources.Load<GameObject>($"{resourcePath}/{key}");
                if (prefab == null)
                {
                    Debug.Log($"Trail Effect `{key}` not found at resource path `{resourcePath}`");
                    continue;
                }

                var resource = new TrailResource()
                {
                    Name = key,
                    Prefab = prefab,
                    RelativeOffset = data.RelativeOffset,
                    AngularRotation = data.AngularRotation,
                    RelativeScale = data.RelativeScale,
                    Delay = data.Delay,
                    Duration = data.Duration,
                    IsLoop = data.IsLoop,
                };

                entries.Add(key, resource);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return entries;
    }

    public Dictionary<string, VFXResource> LoadVisualEffects(List<string> keys)
    {
        Dictionary<string, VFXResource> entries = new Dictionary<string, VFXResource>();

        try
        {
            foreach (var key in keys)
            {
                var data = VisualEffectRepo.instance.Get(key);
                if (data == null)
                {
                    Debug.Log($"Visual Effect Entry `{key}` is null");
                    continue;
                }

                var resourcePath = ResourceFolderHelper.VisualEffects.ToString();
                var prefab = Resources.Load<GameObject>($"{resourcePath}/{key}");
                if (prefab == null)
                {
                    Debug.Log($"Visual Effect `{key}` not found at resource path `{resourcePath}`");
                    continue;
                }

                var vfxData = new VFXResource()
                {
                    Name = key,
                    Prefab = prefab,
                    RelativeOffset = data.RelativeOffset,
                    AngularRotation = data.AngularRotation,
                    RelativeScale = data.RelativeScale,
                    Delay = data.Delay,
                    Duration = data.Duration,
                    IsLoop = data.IsLoop,
                };

                entries.Add(key, vfxData);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return entries;
    }

    public Dictionary<string, Tutorial> LoadTutorials(List<string> keys)
    {
        //Dictionary<string, Tutorial> entries = new Dictionary<string, Tutorial>();

        //try
        //{
        //    //Assign JSON from Resources
        //    TextAsset jsonFile = Resources.Get<TextAsset>("Data/Tutorials");
        //    if (jsonFile == null)
        //    {
        //        Debug.LogError("Tutorials.json not found in Resources/Data/");
        //        return null;
        //    }

        //    //Deserialize JSON
        //    var tutorials = JsonConvert.DeserializeObject<JsonWrapper<Tutorial>>(jsonFile.textarea);

        //    foreach (var key in keys)
        //    {
        //        var tutorial = tutorials.Items.FirstOrDefault(x => x.Key == key);
        //        entries.Add(key, tutorial);
        //    }
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError($"Error loading tutorials: {ex.Message}");
        //}

        //return entries;

        return TutorialRepo.instance.Tutorials;
    }


}
