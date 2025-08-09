using UnityEngine.SceneManagement;

namespace Assets.Helper
{
    public static class SceneHelper
    {
        public static string Credits = "Credits";
        public static string Game = "Game";
        public static string Overworld = "Overworld";
        public static string ProfileCreate = "ProfileCreate";
        public static string ProfileSelect = "ProfileSelect";
        public static string SaveFileSelect = "SaveFileSelect";
        public static string SplashScreen = "SplashScreen";
        public static string Settings = "Settings";
        public static string StageSelect = "StageSelect";
        public static string TitleScreen = "TitleScreen";
        public static string PartyManager = "PartyManager";

        public static bool IsCurrentScene(string sceneName)
        {
            return SceneManager.GetActiveScene().name == sceneName;
        }
        public static bool IsGameScene = IsCurrentScene(Game);

    }
}