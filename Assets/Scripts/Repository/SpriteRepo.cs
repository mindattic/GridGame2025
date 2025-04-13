using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpriteRepo", menuName = "Repositories/SpriteRepo")]
public class SpriteRepo : ScriptableObject
{
    // Singleton instance
    private static SpriteRepo Instance;
    public static SpriteRepo instance
    {
        get
        {
            if (Instance == null)
            {
                Debug.LogWarning("SpriteRepo instance is null. Attempting to load synchronously.");
                LoadSynchronously();
            }

            if (Instance == null)
                Debug.LogError("SpriteRepo accessed before being initialized! Ensure it's assigned in Addressables.");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void AutoInitialize()
    {
        if (Instance == null)
        {
            var handle = Addressables.LoadAssetAsync<SpriteRepo>("Repositories/SpriteRepo");
            Instance = await handle.Task;

            if (Instance == null)
                Debug.LogError("SpriteRepo asset not found in Addressables with key 'Repositories/SpriteRepo'");
        }
    }

    // Synchronous fallback for loading the SpriteRepo
    private static void LoadSynchronously()
    {
        var handle = Addressables.LoadAssetAsync<SpriteRepo>("Repositories/SpriteRepo");
        handle.WaitForCompletion(); // Block until the asset is loaded
        Instance = handle.Result;

        if (Instance == null)
            Debug.LogError("Failed to load SpriteRepo synchronously from Addressables.");
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, Sprite> Seamless;
    [SerializeField] public Dictionary<string, Sprite> Sprites;
    [SerializeField] public Dictionary<string, Sprite> WeaponTypes;
    [SerializeField] public Dictionary<string, Sprite> Leaves;
    [SerializeField] public Dictionary<string, Sprite> TutorialPages;


    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Seamless = new Dictionary<string, Sprite>
        {
            { "BlackFire1", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/BlackFire1") },
            { "BlackFire2", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/BlackFire2") },
            { "Fire1", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/Fire1") },
            { "RedFire1", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/RedFire1") },
            { "Swords1", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/Swords1") },
            { "Swords2", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/Swords2") },
            { "WhiteFire1", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/WhiteFire1") },
            { "WhiteFire2", AssetHelper.LoadAsset<Sprite>("Sprites/Seamless/WhiteFire2") },
        };


        Sprites = new Dictionary<string, Sprite>
        {
            { "DottedLine", AssetHelper.LoadAsset<Sprite>("Sprites/DottedLine") },
            { "DottedLineArrow", AssetHelper.LoadAsset<Sprite>("Sprites/DottedLineArrow") },
            { "DottedLineTurn", AssetHelper.LoadAsset<Sprite>("Sprites/DottedLineTurn") },
            { "Footstep", AssetHelper.LoadAsset<Sprite>("Sprites/Footstep") },
            { "Pause", AssetHelper.LoadAsset<Sprite>("Sprites/Pause") },
            { "Paused", AssetHelper.LoadAsset<Sprite>("Sprites/Paused") },
            { "Forest", AssetHelper.LoadAsset<Sprite>("Sprites/Forest") },
        };

        WeaponTypes = new Dictionary<string, Sprite>
        {
            { "Bow", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Bow") },
            { "Claw", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Claw") },
            { "Crossbow", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Crossbow") },
            { "Dagger", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Dagger") },
            { "Grenade", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Grenade") },
            { "Hammer", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Hammer") },
            { "Katana", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Katana") },
            { "Mace", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Mace") },
            { "Pistol", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Pistol") },
            { "Polearm", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Polearm") },
            { "Potion", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Potion") },
            { "Scythe", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Scythe") },
            { "Shield", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Shield") },
            { "Shuriken", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Shuriken") },
            { "Spear", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Spear") },
            { "Staff", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Staff") },
            { "Sword", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Sword") },
            { "Wand", AssetHelper.LoadAsset<Sprite>("Sprites/WeaponTypes/Wand") },
        };

        Leaves = new Dictionary<string, Sprite>
        {
            { "Leaf1", AssetHelper.LoadAsset<Sprite>("Sprites/Leaves/Leaf1") },
            { "Leaf2", AssetHelper.LoadAsset<Sprite>("Sprites/Leaves/Leaf2") },
            { "MapleLeaf1", AssetHelper.LoadAsset<Sprite>("Sprites/Leaves/MapleLeaf1") },
            { "MapleLeaf2", AssetHelper.LoadAsset<Sprite>("Sprites/Leaves/MapleLeaf2") },
        };

        TutorialPages = new Dictionary<string, Sprite>
        {
            { "Tutorial.1-1", AssetHelper.LoadAsset<Sprite>("Sprites/TutorialPages/Tutorial.1-1") },
            { "Tutorial.1-2", AssetHelper.LoadAsset<Sprite>("Sprites/TutorialPages/Tutorial.1-2") },
            { "Tutorial.1-3", AssetHelper.LoadAsset<Sprite>("Sprites/TutorialPages/Tutorial.1-3") },
        };
    }

}
