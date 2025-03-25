using System.Collections;
using UnityEngine;
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
                    Debug.LogError("SceneRepo accessed before being initialized!");
                }
                return Instance;
            }
        }

        //Assign
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance == null)
            {
                Instance = Resources.Load<SceneRepo>("Repositories/SceneRepo");
                if (Instance == null)
                    Debug.LogError("SceneRepo asset not found in Resources/Repositories/SceneRepo");
            }
        }

        //Fields
        [SerializeField] public string previousScene = "Title";
        [SerializeField] public string currentScene = "Title";

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

        public IEnumerator LoadPreviousScene()
        {
            if (string.IsNullOrWhiteSpace(previousScene))
            {
                Debug.LogError("previousScene is not set.");
                yield break;
            }

            currentScene = previousScene;
            previousScene = SceneManager.GetActiveScene().name;

            // Use async loading for smoother transitions
            yield return SceneManager.LoadSceneAsync(currentScene);
        }
    }
}
