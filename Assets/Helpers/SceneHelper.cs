using Assets.Helper;
using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Helpers
{
    /// <summary>
    /// Centralized scene changes with fade out, scene change, async load, and fade in.
    /// Usage:
    ///   using scene = Assets.Helpers.SceneHelper;
    ///   scene.Change.ToSettings();
    ///   scene.FadeIn();
    /// </summary>
    public static class SceneHelper
    {
        // Scene name constants
        public static string Credits = "Credits";
        public static string Game = "Game";
        public static string LoadingScreen = "LoadingScreen";
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
        /// Returns true if the active scene matches the provided name.
        /// </summary>
        public static bool IsCurrentScene(string sceneName) =>
            SceneManager.GetActiveScene().name == sceneName;

        /// <summary>
        /// True if the current scene is the main game scene.
        /// </summary>
        public static bool IsGameScene => IsCurrentScene(Game);

        /// <summary>
        /// Calls FadeIn on the active FadeOverlay if it exists.
        /// </summary>
        public static void FadeIn(IEnumerator routine = null)
        {
            var overlay = FadeOverlayHelper.Overlay;
            if (overlay != null)
            {
                overlay.FadeIn(routine);
            }
            else
            {
                Debug.LogWarning("SceneHelper.FadeIn called but no FadeOverlay found in scene.");
            }
        }

        /// <summary>
        /// Calls FadeOut with a provided IEnumerator or action.
        /// </summary>
        public static void FadeOut(IEnumerator routine)
        {
            var overlay = FadeOverlayHelper.Overlay;
            if (overlay != null)
            {
                overlay.FadeOut(routine);
            }
            else
            {
                Debug.LogWarning("SceneHelper.FadeOut called but no FadeOverlay found in scene.");
            }
        }

        /// <summary>
        /// Fluent scene change API that encapsulates fade and loading flow.
        /// </summary>
        public static class Change
        {
            public static void To(string sceneName)
            {
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    Debug.LogError("SceneHelper.Change.To received an empty scene name.");
                    return;
                }

                IEnumerator afterFade()
                {
                    SceneLoader.Load(sceneName);
                    yield return Wait.None();
                }

                FadeOut(afterFade());
            }

            public static void ToPreviousScene(string defaultScene = "Game")
            {
                IEnumerator afterFade()
                {
                    SceneLoader.LoadPreviousScene(defaultScene);
                    yield return Wait.None();
                }

                FadeOut(afterFade());
            }

            // Strongly typed helpers
            public static void ToCredits() => To(Credits);
            public static void ToGame() => To(Game);
            public static void ToOverworld() => To(Overworld);
            public static void ToPartyManager() => To(PartyManager);
            public static void ToProfileCreate() => To(ProfileCreate);
            public static void ToProfileSelect() => To(ProfileSelect);
            public static void ToSaveFileSelect() => To(SaveFileSelect);
            public static void ToSplashScreen() => To(SplashScreen);
            public static void ToSettings() => To(Settings);
            public static void ToStageSelect() => To(StageSelect);
            public static void ToTitleScreen() => To(TitleScreen);
        }
    }
}
