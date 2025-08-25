using Assets.Helper;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // Draws focused actor, input mode, current turn, and sequence details.
    // Safe during scene switches: avoids HasFocusedActor and null-guards all managers.
    private void RenderGameStats()
    {
        GUILayout.BeginHorizontal();

        var focusedActor = g.Actors.FocusedActor != null ? g.Actors.FocusedActor.characterName : null ?? "-";
        GUILayout.Label($"Focused Actor: {focusedActor}", GUILayout.Width(Screen.width * 0.25f));

        var inputMode = g.InputManager.InputMode;
        GUILayout.Label($"Input Mode: {inputMode}", GUILayout.Width(Screen.width * 0.25f));

        var currentTurn = g.TurnManager.IsHeroTurn ? "Player" : "Opponent";
        GUILayout.Label($"Current Turn: {currentTurn}", GUILayout.Width(Screen.width * 0.25f));

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        //GUILayout.BeginHorizontal();
        //var sequenceDetails = g.SequenceManager.GetDetails() ?? "-";
        //GUILayout.Label(sequenceDetails, GUILayout.Width(Screen.width * 0.25f));
        //GUILayout.EndHorizontal();
    }
}
