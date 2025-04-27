using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MaterialRepo", menuName = "Repositories/MaterialRepo")]
public class MaterialRepo : ScriptableObject
{
    // Singleton instance
    private static MaterialRepo Instance;
    public static MaterialRepo instance
    {
        get
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<MaterialRepo>("Repositories/MaterialRepo");
                handle.WaitForCompletion(); // Block until the asset is loaded
                Instance = handle.Result;
            }

            if (Instance == null)
                Debug.LogError("MaterialRepo accessed before being initialized! Ensure it's assigned in Addressables.");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static async void AutoInitialize()
    //{
    //    if (_ == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<MaterialRepo>("Repositories/MaterialRepo");
    //        _ = await handle.Task;

    //        if (_ == null)
    //            Debug.LogError("MaterialRepo asset not found in Addressables with key 'Repositories/MaterialRepo'");
    //    }
    //}

    // Synchronous fallback for loading the MaterialRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<MaterialRepo>("Repositories/MaterialRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    _ = handle.Result;

    //    if (_ == null)
    //        Debug.LogError("Failed to load MaterialRepo synchronously from Addressables.");
    //}

    //Serialized fields
    [SerializeField] public Dictionary<string, Material> Materials;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Materials = new Dictionary<string, Material>
        {
            { "EnemyParallax", AssetHelper.LoadAsset<Material>("Materials/EnemyParallax") },
            { "PlayerParallax", AssetHelper.LoadAsset<Material>("Materials/PlayerParallax") },
            { "SpriteOutline", AssetHelper.LoadAsset<Material>("Materials/SpriteOutline") },
            { "SpritePan", AssetHelper.LoadAsset<Material>("Materials/SpritePan") },
        };
    }

}
