using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Repositories
{
    public static class SceneRepo
    {
        // Fields to track scenes
        private static string previousScene = "TitleScreen";
        private static string currentScene = "TitleScreen";

        /// <summary>
        /// Loads a new scene asynchronously, tracking the previous scene.
        /// </summary>
        public static IEnumerator LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("Invalid scene name provided.");
                yield break;
            }

            previousScene = SceneManager.GetActiveScene().name;
            currentScene = sceneName;

            yield return SceneManager.LoadSceneAsync(currentScene);
        }

        /// <summary>
        /// Loads the previously active scene. If none is tracked, loads the default scene.
        /// </summary>
        public static IEnumerator LoadPreviousScene(string defaultScene = "Game")
        {
            if (string.IsNullOrWhiteSpace(previousScene))
            {
                previousScene = defaultScene;
            }

            string targetScene = previousScene;
            previousScene = SceneManager.GetActiveScene().name;
            currentScene = targetScene;

            yield return SceneManager.LoadSceneAsync(currentScene);
        }

        /// <summary>
        /// Gets the name of the current tracked scene.
        /// </summary>
        public static string GetCurrentScene() => currentScene;

        /// <summary>
        /// Gets the name of the previously tracked scene.
        /// </summary>
        public static string GetPreviousScene() => previousScene;
    }
}
