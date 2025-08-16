using Assets.Helper;
using UnityEditor;
using UnityEngine;
using scene = Assets.Helpers.SceneHelper;

public partial class DebugWindow
{
    // RenderScenes draws buttons to switch between different game scenes.
    private void RenderScenes()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("SceneOptions", EditorStyles.boldLabel, GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        bool isClicked;

        GUILayout.BeginHorizontal();
        isClicked = GUILayout.Button("SplashScreen Screen", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToSplashScreen();

        isClicked = GUILayout.Button("TitleScreen Screen", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToTitleScreen();

        isClicked = GUILayout.Button("Settings", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToSettings();

        isClicked = GUILayout.Button("Stage Select", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToStageSelect();

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        isClicked = GUILayout.Button("Load Profile", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToProfileSelect();

        isClicked = GUILayout.Button("Load Save", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToSaveFileSelect();

        isClicked = GUILayout.Button("Overworld", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToOverworld();

        isClicked = GUILayout.Button("Game", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            scene.Change.ToGame();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
}
