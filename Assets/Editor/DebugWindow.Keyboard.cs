using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // RenderKeyboard draws UI buttons that simulate keyboard arrow keys.
    private void RenderKeyboard()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Keyboard");
        GUILayout.EndHorizontal();

        bool isClicked;

        // Render "Up" arrow in the center.
        GUILayout.BeginHorizontal();
        GUILayout.Space(38); // Space to center the button.
        isClicked = GUILayout.Button("\u2191", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyUp();
        GUILayout.Space(38);
        GUILayout.EndHorizontal();

        // Render "Left", "Down", and "Right" arrows.
        GUILayout.BeginHorizontal();
        isClicked = GUILayout.Button("\u2190", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyLeft();

        isClicked = GUILayout.Button("\u2193", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyDown();

        isClicked = GUILayout.Button("\u2192", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyRight();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(10);
        Repaint(); // Force a UI update.
    }

    // Keyboard control methods for actor move.
    private void OnKeyUp() => g.Actors.FocusedActor?.Move(Vector2Int.down);
    private void OnKeyDown() => g.Actors.FocusedActor?.Move(Vector2Int.up);
    private void OnKeyLeft() => g.Actors.FocusedActor?.Move(Vector2Int.left);
    private void OnKeyRight() => g.Actors.FocusedActor?.Move(Vector2Int.right);
}
