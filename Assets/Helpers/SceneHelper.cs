using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Helper
{
    /// <summary>
    /// Central scene constants and helpers.
    /// Includes backward compatible coroutine shims that delegate to SceneLoader.
    /// </summary>
    public static class SceneHelper
    {
        // Scene name constants
        public static string Credits = "Credits";
        public static string Game = "Game";
        public static string Overworld = "Overworld";
        public static string PartyManager = "PartyManager";
        public static string ProfileCreate = "ProfileCreate";
        public static string ProfileSelect = "ProfileSelect";
        public static string SaveFileSelect = "SaveFileSelect";
        public static string SplashScreen = "SplashScreen";
        public static string Settings = "Settings";
        public static string StageSelect = "StageSelect";
        public static string TitleScreen = "TitleScreen";


        /// <summary>
        /// Returns true if the active scene name matches the provided name.
        /// </summary>
        public static bool IsCurrentScene(string sceneName)
        {
            return SceneManager.GetActiveScene().name == sceneName;
        }

        /// <summary>
        /// Returns true if the active scene is the main Game scene.
        /// Implemented as a property so it reflects the current scene at call time.
        /// </summary>
        public static bool IsGameScene => IsCurrentScene(Game);

        public static IEnumerator LoadCredits()
        {
            yield return LoadScene(Credits);
        }
        public static IEnumerator LoadGame()
        {
            yield return LoadScene(Game);
        }
        public static IEnumerator LoadOverworld()
        {
            yield return LoadScene(Overworld);

        }
        public static IEnumerator LoadPartyManager()
        {
            yield return LoadScene(PartyManager);
        }
        public static IEnumerator LoadProfileCreate()
        {
            yield return LoadScene(ProfileCreate);
        }
        public static IEnumerator LoadProfileSelect()
        {
            yield return LoadScene(ProfileSelect);
        }
        public static IEnumerator LoadSaveFileSelect()
        {
            yield return LoadScene(SaveFileSelect);
        }
        public static IEnumerator LoadSaveSplashScreen()
        {
            yield return LoadScene(SplashScreen);
        }
        public static IEnumerator LoadSettings()
        {
            yield return LoadScene(Settings);
        }
        public static IEnumerator LoadStageSelect()
        {
            yield return LoadScene(StageSelect);
        }
        public static IEnumerator LoadTitleScreen()
        {
            yield return LoadScene(TitleScreen);
        }


        /// <summary>
        /// Backward compatible signature for old call sites:
        /// StartCoroutine(overlay.FadeOutRoutine(SceneHelper.LoadScene(SceneHelper.Game)));
        /// Delegates to SceneLoader and immediately completes.
        /// </summary>
        public static IEnumerator LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("SceneHelper.LoadScene received an empty scene name.");
                yield break;
            }

            SceneLoader.Load(sceneName);
            yield break;
        }

        /// <summary>
        /// Backward compatible signature for loading the previously tracked scene.
        /// Delegates to SceneLoader and immediately completes.
        /// </summary>
        public static IEnumerator LoadPreviousScene(string defaultScene = "Game")
        {
            SceneLoader.LoadPreviousScene(defaultScene);
            yield break;
        }

        /// <summary>
        /// Returns the currently tracked scene name from SceneLoader.
        /// </summary>
        public static string GetCurrentScene()
        {
            return SceneLoader.GetCurrentScene();
        }

        /// <summary>
        /// Returns the previously tracked scene name from SceneLoader.
        /// </summary>
        public static string GetPreviousScene()
        {
            return SceneLoader.GetPreviousScene();
        }
    }
}
