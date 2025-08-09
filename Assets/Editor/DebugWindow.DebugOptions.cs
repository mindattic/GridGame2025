using Assets.Helper;
using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public partial class DebugWindow
{
    // RenderDebugOptions renders a dropdown for various debug options and a Run button.
    private void RenderDebugOptions()
    {
        if (!Application.isPlaying || !SceneHelper.IsGameScene)
            return;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Debug Options", GUILayout.Width(Screen.width * 0.25f));
        selectedOption = (DebugOptions)EditorGUILayout.EnumPopup(selectedOption, GUILayout.Width(Screen.width * 0.5f));
        if (GUILayout.Button("Run", GUILayout.Width(Screen.width * 0.25f)))
            OnDebugOptionRunClick();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // OnDebugOptionRunClick executes a test based on the selected debug option.
    private void OnDebugOptionRunClick()
    {
        switch (selectedOption)
        {

            case DebugOptions.ArrangeSingleCombo: g.DebugManager.ArrangeSingleCombo(); break;
            case DebugOptions.ArrangeTripleCombo: g.DebugManager.ArrangeTripleCombo(); break;
            case DebugOptions.Bump: g.DebugManager.Bump(); break;
            case DebugOptions.Dodge: g.DebugManager.Dodge(); break;
            case DebugOptions.Fireball: g.DebugManager.Fireball(); break;
            case DebugOptions.Heal: g.DebugManager.Heal(); break;
            case DebugOptions.KillEnemies: g.DebugManager.KillEnemies(); break;
            case DebugOptions.PortraitPopIn: g.DebugManager.PortraitPopIn(); break;
            case DebugOptions.Portrait2DSlideIn: g.DebugManager.Portrait2DSlideIn(); break;
            case DebugOptions.Portrait3DSlideIn: g.DebugManager.Portrait3DSlideIn(); break;
            case DebugOptions.RandomizeBackground: g.DebugManager.RandomizeBackground(); break;
            case DebugOptions.Shake: g.DebugManager.Shake(); break;
            case DebugOptions.SpawnCoins: g.DebugManager.SpawnCoints(); break;
            case DebugOptions.SpawnDamageText: g.DebugManager.SpawnDamageText(); break;
            case DebugOptions.SpawnHealText: g.DebugManager.SpawnHealText(); break;
            case DebugOptions.SpawnSupportLines: g.DebugManager.SpawnSupportLines(); break;
            case DebugOptions.SpawnTitle: g.DebugManager.TitleTest(); break;
            case DebugOptions.SpawnTooltip: g.DebugManager.SpawnTooltip(); break;
            case DebugOptions.Spin: g.DebugManager.Spin(); break;
            case DebugOptions.TriggerEnemyMoveAttack: g.DebugManager.TriggerEnemyMoveAttack(); break;
            case DebugOptions.TriggerEnemyAttack: g.DebugManager.TriggerEnemyAttack(); break;

            default: Debug.LogWarning("OnDebugOptionRunClick failed."); break;
        }
    }
}
