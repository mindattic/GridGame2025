using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MusicTrackRepo", menuName = "Repositories/MusicTrackRepo")]
public class MusicTrackRepo : ScriptableObject
{
    // Singleton instance
    private static MusicTrackRepo Instance;
    public static MusicTrackRepo instance
    {
        get
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<MusicTrackRepo>("Repositories/MusicTrackRepo");
                handle.WaitForCompletion(); // Block until the asset is loaded
                Instance = handle.Result;
            }

            if (Instance == null)
                Debug.LogError("MusicTrackRepo accessed before being initialized! Ensure it's assigned in Addressables.");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static async void AutoInitialize()
    //{
    //    if (Instance == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<MusicTrackRepo>("Repositories/MusicTrackRepo");
    //        Instance = await handle.Task;

    //        if (Instance == null)
    //            Debug.LogError("MusicTrackRepo asset not found in Addressables with key 'Repositories/MusicTrackRepo'");
    //    }
    //}

    // Synchronous fallback for loading the MusicTrackRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<MusicTrackRepo>("Repositories/MusicTrackRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    Instance = handle.Result;

    //    if (Instance == null)
    //        Debug.LogError("Failed to load MusicTrackRepo synchronously from Addressables.");
    //}

    //Serialized fields
    [SerializeField] public Dictionary<string, AudioClip> MusicTracks;

    private void OnEnable()
    {
        Load();
    }

    private async void Load()
    {
        MusicTracks = new Dictionary<string, AudioClip>
        {
            { "MelancholyLull", await AssetHelper.LoadAssetAsync<AudioClip>("MusicTracks/MelancholyLull") },
        };
    }

}
