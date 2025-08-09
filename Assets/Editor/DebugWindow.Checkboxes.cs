using Assets.Helper;
using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // RenderCheckboxes provides several toggles for various debug options.
    private void RenderCheckboxes()
    {
        if (!Application.isPlaying || !SceneHelper.IsGameScene)
            return;

        bool onCheckChanged;

        GUILayout.BeginHorizontal();

        // Toggle to show or hide actor name tags.
        onCheckChanged = EditorGUILayout.Toggle("YieldSpawn Actor Name?", g.DebugManager.showActorNameTag, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.showActorNameTag != onCheckChanged)
        {
            g.DebugManager.showActorNameTag = onCheckChanged;
            g.Actors.All.ForEach(x => x.render.SetNameTagEnabled(onCheckChanged));
        }

        // Toggle to show or hide actor frames.
        onCheckChanged = EditorGUILayout.Toggle("Show Actor Frame?", g.DebugManager.showActorFrame, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.showActorFrame != onCheckChanged)
        {
            g.DebugManager.showActorFrame = onCheckChanged;
            g.Actors.All.ForEach(x => x.render.SetFrameEnabled(onCheckChanged));
        }

        // Toggle to show or hide tutorial popups.
        onCheckChanged = EditorGUILayout.Toggle("Show Tutorials", g.DebugManager.showTutorials, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.showTutorials != onCheckChanged)
        {
            g.DebugManager.showTutorials = onCheckChanged;
            g.TutorialPopup.gameObject.SetActive(g.DebugManager.showTutorials);
        }

        // Toggle for hero invincibility.
        onCheckChanged = EditorGUILayout.Toggle("Are Heroes Invincible?", g.DebugManager.isHeroInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isHeroInvincible != onCheckChanged)
            g.DebugManager.isHeroInvincible = onCheckChanged;

        // Toggle for enemy invincibility.
        onCheckChanged = EditorGUILayout.Toggle("Are Enemies Invincible?", g.DebugManager.isEnemyInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isEnemyInvincible != onCheckChanged)
            g.DebugManager.isEnemyInvincible = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        // Toggle for infinite timer.
        onCheckChanged = EditorGUILayout.Toggle("Is Timer Infinite?", g.DebugManager.isTimerInfinite, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isTimerInfinite != onCheckChanged)
            g.DebugManager.isTimerInfinite = onCheckChanged;

        // Toggle for enemy stunned state.
        onCheckChanged = EditorGUILayout.Toggle("Is Opponent Stunned?", g.DebugManager.isEnemyStunned, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isEnemyStunned != onCheckChanged)
            g.DebugManager.isEnemyStunned = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
}
