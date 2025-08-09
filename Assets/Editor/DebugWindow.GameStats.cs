using Assets.Helper;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // Draws focused actor, input mode, current turn, and sequence details.
    // Safe during scene switches: avoids HasFocusedActor and null-guards all managers.
    private void RenderGameStats()
    {
        if (!Application.isPlaying || !SceneHelper.IsGameScene)
            return;

        if (!SceneHelper.IsGameScene)
            return;

        GUILayout.BeginHorizontal();

        var focusedActor = g.Actors.FocusedActor != null ? g.Actors.FocusedActor.characterName : null ?? "-";
        GUILayout.Label($"Focused Actor: {focusedActor}", GUILayout.ExpandWidth(true));

        var inputMode = g.InputManager.inputMode;
        GUILayout.Label($"Input Mode: {inputMode}", GUILayout.ExpandWidth(true));

        var currentTurn = g.TurnManager.isHeroTurn ? "Player" : "Opponent";
        GUILayout.Label($"Current Turn: {currentTurn}", GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        var sequenceDetails = g.SequenceManager.GetDetails() ?? "-";
        GUILayout.Label(sequenceDetails, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
    }
}
