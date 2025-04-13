using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Repositories
{
    [CreateAssetMenu(fileName = "SceneRepo", menuName = "Repositories/SceneRepo")]
    public class SceneRepo : ScriptableObject
    {
        // Singleton reference
        private static SceneRepo Instance;

        public static SceneRepo instance
        {
            get
            {
                if (Instance == null)
                {
                    Debug.LogWarning("SceneRepo instance is null. Attempting to load synchronously.");
                    LoadSynchronously();
                }

                if (Instance == null)
                    Debug.LogError("SceneRepo accessed before being initialized!");

                return Instance;
            }
        }

        // Auto-initialize before the scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void AutoInitialize()
        {
            if (Instance == null)
            {
                var handle = Addressables.LoadAssetAsync<SceneRepo>("Repositories/SceneRepo");
                Instance = await handle.Task;

                if (Instance == null)
                    Debug.LogError("SceneRepo asset not found in Addressables with key 'Repositories/SceneRepo'");
            }
        }

        // Synchronous fallback for loading the SceneRepo
        private static void LoadSynchronously()
        {
            var handle = Addressables.LoadAssetAsync<SceneRepo>("Repositories/SceneRepo");
            handle.WaitForCompletion(); // Block until the asset is loaded
            Instance = handle.Result;

            if (Instance == null)
                Debug.LogError("Failed to load SceneRepo synchronously from Addressables.");
        }

        //Fields
        [SerializeField] public string previousScene = "TitleScreen";
        [SerializeField] public string currentScene = "TitleScreen";

        public IEnumerator LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("Invalid scene name provided.");
                yield break;
            }

            previousScene = SceneManager.GetActiveScene().name;
            currentScene = sceneName;

            // Use async loading for smoother transitions
            yield return SceneManager.LoadSceneAsync(currentScene);
        }

        public IEnumerator LoadPreviousScene(string defaultScene = "Game")
        {
            if (string.IsNullOrWhiteSpace(previousScene))
            {
                previousScene = defaultScene;
            }

            currentScene = previousScene;
            previousScene = SceneManager.GetActiveScene().name;

            // Use async loading for smoother transitions
            yield return SceneManager.LoadSceneAsync(currentScene);
        }
    }
}
