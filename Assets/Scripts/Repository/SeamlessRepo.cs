using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SeamlessRepo", menuName = "Repositories/SeamlessRepo")]
public class SeamlessRepo : ScriptableObject
{
    // Singleton instance
    private static SeamlessRepo Instance;
    public static SeamlessRepo instance
    {
        get
        {
            if (Instance == null)
            {
                Debug.LogWarning("SeamlessRepo instance is null. Attempting to load synchronously.");
                LoadSynchronously();
            }

            if (Instance == null)
                Debug.LogError("SeamlessRepo accessed before being initialized! Ensure it's assigned in Addressables.");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void AutoInitialize()
    {
        if (Instance == null)
        {
            var handle = Addressables.LoadAssetAsync<SeamlessRepo>("Repositories/SeamlessRepo");
            Instance = await handle.Task;

            if (Instance == null)
                Debug.LogError("SeamlessRepo asset not found in Addressables with key 'Repositories/SeamlessRepo'");
        }
    }

    // Synchronous fallback for loading the SeamlessRepo
    private static void LoadSynchronously()
    {
        var handle = Addressables.LoadAssetAsync<SeamlessRepo>("Repositories/SeamlessRepo");
        handle.WaitForCompletion(); // Block until the asset is loaded
        Instance = handle.Result;

        if (Instance == null)
            Debug.LogError("Failed to load SeamlessRepo synchronously from Addressables.");
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, Sprite> Seamless;


    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Seamless = new Dictionary<string, Sprite>
        {
            { "BlackFire1", AssetHelper.LoadAsset<Sprite>("Seamless/BlackFire1") },
            { "BlackFire2", AssetHelper.LoadAsset<Sprite>("Seamless/BlackFire2") },
            { "Fire1", AssetHelper.LoadAsset<Sprite>("Seamless/Fire1") },
            { "RedFire1", AssetHelper.LoadAsset<Sprite>("Seamless/RedFire1") },
            { "Swords1", AssetHelper.LoadAsset<Sprite>("Seamless/Swords1") },
            { "Swords2", AssetHelper.LoadAsset<Sprite>("Seamless/Swords2") },
            { "WhiteFire1", AssetHelper.LoadAsset<Sprite>("Seamless/WhiteFire1") },
            { "WhiteFire2", AssetHelper.LoadAsset<Sprite>("Seamless/WhiteFire2") },
        };
    }

}
