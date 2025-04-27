using Assets.Scripts.Models;
using Assets.Scripts.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SoundEffectRepo", menuName = "Repositories/SoundEffectRepo")]
public class SoundEffectRepo : ScriptableObject
{
    // Singleton instance
    private static SoundEffectRepo Instance;

    public static SoundEffectRepo instance
    {
        get
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<SoundEffectRepo>("Repositories/SoundEffectRepo");
                handle.WaitForCompletion(); // Block until the asset is loaded
                Instance = handle.Result;
            }

            if (Instance == null)
                Debug.LogError("SoundEffectRepo accessed before being initialized!");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static async void AutoInitialize()
    //{
    //    if (_ == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<SoundEffectRepo>("Repositories/SoundEffectRepo");
    //        _ = await handle.Task;

    //        if (_ == null)
    //            Debug.LogError("SoundEffectRepo asset not found in Addressables with key 'Repositories/SoundEffectRepo'");
    //    }
    //}

    // Synchronous fallback for loading the SoundEffectRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<SoundEffectRepo>("Repositories/SoundEffectRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    _ = handle.Result;

    //    if (_ == null)
    //        Debug.LogError("Failed to load SoundEffectRepo synchronously from Addressables.");
    //}

    // Serialized fields
    [SerializeField] public Dictionary<string, AudioClip> SoundEffects;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        // Initialize the SoundEffects dictionary
        SoundEffects = new Dictionary<string, AudioClip>
        {
            { "Click", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Click") },
            { "Death", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Death") },
            { "Move1", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move1") },
            { "Move2", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move2") },
            { "Move3", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move3") },
            { "Move4", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move4") },
            { "Move5", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move5") },
            { "Move6", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move6") },
            { "NextTurn", AssetHelper.LoadAsset<AudioClip>("SoundEffects/NextTurn") },
            { "PlayerGlow", AssetHelper.LoadAsset<AudioClip>("SoundEffects/PlayerGlow") },
            { "Portrait", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Portrait") },
            { "Rumble", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Rumble") },
            { "Slash1", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash1") },
            { "Slash2", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash2") },
            { "Slash3", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash3") },
            { "Slash4", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash4") },
            { "Slash5", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash5") },
            { "Slash6", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash6") },
            { "Slash7", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash7") },
            { "Slide", AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slide") }
        };

    }
}