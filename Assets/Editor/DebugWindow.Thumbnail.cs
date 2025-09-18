using Assets.Helper;
using System;
using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

public partial class DebugWindow
{
    // Class-level fields
    private string thumbnailPositionX = "0.5";
    private string thumbnailPositionY = "-1.4";
    private string thumbnailScaleX = "5";
    private string thumbnailScaleY = "5";

    private void RenderThumbnailSettings()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("Thumbnail Options", EditorStyles.boldLabel, GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

#if UNITY_EDITOR

        if (s.ReloadThumbnailSettings && g.Actors.HasFocusedActor)
        {
            var t = g.Actors.FocusedActor.Thumbnail;
            thumbnailPositionX = t.settings.Position.x.ToString("F2");
            thumbnailPositionY = t.settings.Position.y.ToString("F2");
            thumbnailScaleX = t.settings.Scale.x.ToString("F2");
            thumbnailScaleY = t.settings.Scale.y.ToString("F2");
            s.ReloadThumbnailSettings = false;
        }

        float containerWidth = EditorGUIUtility.currentViewWidth * Increment.Percent33;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(containerWidth));

        // Parse values
        float.TryParse(thumbnailPositionX, out float pX);
        float.TryParse(thumbnailPositionY, out float pY);
        float.TryParse(thumbnailScaleX, out float sX);
        float.TryParse(thumbnailScaleY, out float sY);

        float oldPX = pX, oldPY = pY, oldSX = sX, oldSY = sY;

        // InputManager fields
        pX = EditorGUILayout.FloatField("pX", pX);
        pY = EditorGUILayout.FloatField("pY", pY);
        sX = EditorGUILayout.FloatField("sX", sX);
        sY = EditorGUILayout.FloatField("sY", sY);

        void update()
        {
            if (GameManager.instance.focusedActor != null)
            {
                var position = new Vector3(pX, pY, 0f);
                var scale = new Vector3(sX, sY, 1f);
                GameManager.instance.focusedActor.Thumbnail.Set(position, scale);
            }
        }

        if (!Mathf.Approximately(pX, oldPX) ||
            !Mathf.Approximately(pY, oldPY) ||
            !Mathf.Approximately(sX, oldSX) ||
            !Mathf.Approximately(sY, oldSY))
        {
            update();
        }

        // Save back to strings
        thumbnailPositionX = pX.ToString("F2");
        thumbnailPositionY = pY.ToString("F2");
        thumbnailScaleX = sX.ToString("F2");
        thumbnailScaleY = sY.ToString("F2");

        // Buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update", GUILayout.Width(64))) update();

        if (GUILayout.Button("Export", GUILayout.Width(64)))
        {
            string exportText =
                $"    Position = new Vector3({pX}f, {pY}f, 0f),\n" +
                $"    Scale = new Vector3({sX}f, {sY}f, 0f),";

            EditorGUIUtility.systemCopyBuffer = exportText;
            Debug.Log($"Copied `{GameManager.instance.focusedActor.characterName}` ThumbnailSettings to clipboard.");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
#endif
    }
}
