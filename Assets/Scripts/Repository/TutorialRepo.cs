using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "TutorialRepo", menuName = "Repositories/TutorialRepo")]
public class TutorialRepo : ScriptableObject
{
    // Singleton instance
    private static TutorialRepo Instance;

    public static TutorialRepo instance
    {
        get
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<TutorialRepo>("Repositories/TutorialRepo");
                handle.WaitForCompletion(); // Block until the asset is loaded
                Instance = handle.Result;
            }

            if (Instance == null)
                Debug.LogError("TutorialRepo accessed before being initialized!");

            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static async void AutoInitialize()
    //{
    //    if (_ == null)
    //    {
    //        var handle = Addressables.LoadAssetAsync<TutorialRepo>("Repositories/TutorialRepo");
    //        _ = await handle.Task;

    //        if (_ == null)
    //            Debug.LogError("TutorialRepo asset not found in Addressables with key 'Repositories/TutorialRepo'");
    //    }
    //}

    // Synchronous fallback for loading the TutorialRepo
    //private static void LoadSynchronously()
    //{
    //    var handle = Addressables.LoadAssetAsync<TutorialRepo>("Repositories/TutorialRepo");
    //    handle.WaitForCompletion(); // Block until the asset is loaded
    //    _ = handle.Result;

    //    if (_ == null)
    //        Debug.LogError("Failed to load TutorialRepo synchronously from Addressables.");
    //}

    //Serialized fields
    [SerializeField] public Dictionary<string, Tutorial> Tutorials;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Tutorials = new Dictionary<string, Tutorial>
        {
            { "Tutorial1", new Tutorial
                {
                    Key = "Tutorial1",
                    Pages = new List<TutorialPage>
                    {
                        new TutorialPage { TextureKey = "Tutorial.1-1", Title = "Tutorial 1-1", Content = "This is the first page of the tutorial." },
                        new TutorialPage { TextureKey = "Tutorial.1-2", Title = "Tutorial 1-2", Content = "This is the second page of the tutorial." },
                        new TutorialPage { TextureKey = "Tutorial.1-3", Title = "Tutorial 1-3", Content = "This is the third page of the tutorial." }
                    }
                }
            }
        };
    }

}
