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
    //    if (Instance == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<SoundEffectRepo>("Repositories/SoundEffectRepo");
    //        Instance = await handle.Task;

    //        if (Instance == null)
    //            Debug.LogError("SoundEffectRepo asset not found in Addressables with key 'Repositories/SoundEffectRepo'");
    //    }
    //}

    // Synchronous fallback for loading the SoundEffectRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<SoundEffectRepo>("Repositories/SoundEffectRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    Instance = handle.Result;

    //    if (Instance == null)
    //        Debug.LogError("Failed to load SoundEffectRepo synchronously from Addressables.");
    //}

    // Serialized fields
    [SerializeField] public Dictionary<string, AudioClip> SoundEffects;

    private async void OnEnable()
    {
        await Load();
    }

    private async Task Load()
    {
        // Initialize the SoundEffects dictionary
        SoundEffects = new Dictionary<string, AudioClip>();

        // List of sound effect keys
        string[] soundKeys = new string[]
        {
            "Click", "Death", "Move1", "Move2", "Move3", "Move4", "Move5", "Move6",
            "NextTurn", "PlayerGlow", "Portrait", "Rumble", "Slash1", "Slash2", "Slash3",
            "Slash4", "Slash5", "Slash6", "Slash7", "Slide"
        };

        // Load sound effects asynchronously
        var tasks = soundKeys.Select(async key =>
        {
            string address = $"SoundEffects/{key}";
            try
            {
                var clip = await AssetHelper.LoadAssetAsync<AudioClip>(address);
                if (clip != null)
                {
                    SoundEffects[key] = clip;
                }
                else
                {
                    Debug.LogWarning($"Sound effect not found at address: {address}");
                }
            }
            catch (UnityException ex)
            {
                Debug.LogError($"Failed to load sound effect at address: {address}. Exception: {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);
    }
}