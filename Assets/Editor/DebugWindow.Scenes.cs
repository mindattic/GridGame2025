using Assets.Helper;
using Assets.Scripts.Repositories;
using UnityEditor;
using UnityEngine;

public partial class DebugWindow
{
    // RenderScenes draws buttons to switch between different game scenes.
    private void RenderScenes()
    {
        if (!Application.isPlaying || !SceneHelper.IsGameScene)
            return;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Scenes");
        GUILayout.EndHorizontal();

        bool isClicked;

        GUILayout.BeginHorizontal();
        isClicked = GUILayout.Button("SplashScreen Screen", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.SplashScreen);

        isClicked = GUILayout.Button("TitleScreen Screen", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.TitleScreen);

        isClicked = GUILayout.Button("Settings", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.Settings);

        isClicked = GUILayout.Button("Stage Select", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.StageSelect);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        isClicked = GUILayout.Button("Load Profile", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.ProfileSelect);

        isClicked = GUILayout.Button("Load Save", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.SaveFileSelect);

        isClicked = GUILayout.Button("Overworld", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.Overworld);

        isClicked = GUILayout.Button("Game", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.Game);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
}
