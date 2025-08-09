using Assets.Helper;
using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // RenderGameSpeed renders a dropdown to select the game speed and an Apply button.
    private void RenderGameSpeed()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Game Speed", GUILayout.Width(Screen.width * 0.25f));
        selectedGameFocus = (GameSpeedOption)EditorGUILayout.EnumPopup(selectedGameFocus, GUILayout.Width(Screen.width * 0.5f));
        if (GUILayout.Button("Apply", GUILayout.Width(Screen.width * 0.25f)))
            OnGameSpeedChange();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // OnGameSpeedChange adjusts the game speed based on the selected option.
    private void OnGameSpeedChange()
    {
        switch (selectedGameFocus)
        {
            case GameSpeedOption.Paused:
                g.GameSpeed = 0f;
                break;
            case GameSpeedOption.Slower:
                g.GameSpeed = 0.25f;
                break;
            case GameSpeedOption.Slow:
                g.GameSpeed = 0.5f;
                break;
            case GameSpeedOption.Normal:
                g.GameSpeed = 1f;
                break;
            case GameSpeedOption.Fast:
                g.GameSpeed = 2f;
                break;
            case GameSpeedOption.Faster:
                g.GameSpeed = 4f;
                break;
        }
    }
}
