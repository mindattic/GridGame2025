using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // RenderGameStats displays key game statistics such as FPS, turn info, phase, and runtime.
    private void RenderGameStats()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label($"Focused Actor: {(g.Actors.FocusedActor ? g.Actors.FocusedActor.characterName : "-")}", GUILayout.Width(Screen.width * 0.25f));
        //GUILayout.Label($"FPS: {consoleManager.fpsMonitor.currentFps}", GUILayout.Width(Screen.thumbnailScaleX * 0.25f));

        GUILayout.Label($"Input Mode: {g.InputManager.inputMode.ToString()}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"Current Turn: {(g.TurnManager.isHeroTurn ? "Hero" : "Opponent")}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label(g.SequenceManager.GetDetails(), GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();
    }
}
