using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Store
{
    [CreateAssetMenu(fileName = "SceneStore", menuName = "Stores/SceneStore")]
    public class SceneStore : ScriptableObject
    {
        // Singleton reference
        private static SceneStore _instance;

        public static SceneStore instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("SceneStore accessed before being initialized!");
                }
                return _instance;
            }
        }

        //Initialize
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (_instance == null)
            {
                _instance = Resources.Load<SceneStore>("Stores/SceneStore");
                if (_instance == null)
                    Debug.LogError("SceneStore asset not found in Resources/Stores/SceneStore");
            }
        }

        //Fields
        [SerializeField] public string PreviousScene = "Title";
        [SerializeField] public string CurrentScene = "Title";

        public IEnumerator LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("Invalid scene name provided.");
                yield break;
            }

            PreviousScene = SceneManager.GetActiveScene().name;
            CurrentScene = sceneName;

            // Use async loading for smoother transitions
            yield return SceneManager.LoadSceneAsync(CurrentScene);
        }

        public IEnumerator LoadPreviousScene()
        {
            if (string.IsNullOrWhiteSpace(PreviousScene))
            {
                Debug.LogError("PreviousScene is not set.");
                yield break;
            }

            CurrentScene = PreviousScene;
            PreviousScene = SceneManager.GetActiveScene().name;

            // Use async loading for smoother transitions
            yield return SceneManager.LoadSceneAsync(CurrentScene);
        }
    }
}
