using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PrefabRepo", menuName = "Repositories/PrefabRepo")]
public class PrefabRepo : ScriptableObject
{
    // Singleton instance
    private static PrefabRepo Instance;
    public static PrefabRepo instance
    {
        get
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<PrefabRepo>("Repositories/PrefabRepo");
                handle.WaitForCompletion(); // Block until the asset is loaded
                Instance = handle.Result;
            }

            if (Instance == null)
                Debug.LogError("PrefabRepo accessed before being initialized! Ensure it's assigned in Addressables.");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static async void AutoInitialize()
    //{
    //    if (_ == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<PrefabRepo>("Repositories/PrefabRepo");
    //        _ = await handle.Task;

    //        if (_ == null)
    //            Debug.LogError("PrefabRepo asset not found in Addressables with key 'Repositories/PrefabRepo'");
    //    }
    //}

    // Synchronous fallback for loading the MaterialRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<PrefabRepo>("Repositories/PrefabRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    _ = handle.Result;

    //    if (_ == null)
    //        Debug.LogError("Failed to load MaterialRepo synchronously from Addressables.");
    //}

    //Serialized fields
    [SerializeField] public Dictionary<string, GameObject> Prefabs;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Prefabs = new Dictionary<string, GameObject>
        {
            { "ActorPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ActorPrefab") },
            { "AttackLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/AttackLinePrefab") },
            { "CanvasParticlePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CanvasParticlePrefab") },
            { "CoinPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CoinPrefab") },
            { "ConfirmationDialog", AssetHelper.LoadAsset<GameObject>("Prefabs/ConfirmationDialog") },
            { "DamageTextPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/DamageTextPrefab") },
            { "DottedLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/DottedLinePrefab") },
            { "FootstepPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/FootstepPrefab") },
            { "GhostPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/GhostPrefab") },
            { "KeyboardDialog", AssetHelper.LoadAsset<GameObject>("Prefabs/KeyboardDialog") },
            { "KeyButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/KeyButtonPrefab") },
            { "PortraitPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/PortraitPrefab") },
            { "RosterSlidePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/RosterSlidePrefab") },
            { "SaveFileButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SaveFileButtonPrefab") },
            { "ScreenWidthButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ScreenWidthButtonPrefab") },
            { "SpellPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SpellPrefab") },
            { "StatRow", AssetHelper.LoadAsset<GameObject>("Prefabs/StatRow") },
            { "SupportLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SupportLinePrefab") },
            { "TilePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TilePrefab") },
            { "TooltipPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TooltipPrefab") },
            { "WallPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/WallPrefab") },
        };
    }

}
