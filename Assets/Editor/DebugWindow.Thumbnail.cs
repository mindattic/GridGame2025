using Assets.Helper;
using System;
using UnityEditor;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

public partial class DebugWindow
{
    // Class-level fields (now using pixel focus instead of offset)
    private string thumbnailPixelX = "512";
    private string thumbnailPixelY = "512";
    private string thumbnailScaleX = "5";
    private string thumbnailScaleY = "5";
    private string thumbnailTextureSize = "1024";

    private void RenderThumbnailSettings()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("Thumbnail Options", EditorStyles.boldLabel, GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

#if UNITY_EDITOR

        if (s.ReloadThumbnailSettings && g.Actors.HasSelectedActor)
        {
            var t = g.Actors.SelectedActor.Thumbnail;
            int texSize = 1024;
            if (t != null && t.sprite != null && t.sprite.texture != null)
            {
                // Prefer larger dimension as canonical size
                var tex = t.sprite.texture;
                texSize = Mathf.Max(tex.width, tex.height);
            }

            // Load from current settings
            thumbnailPixelX = t.settings.PixelPosition.x.ToString();
            thumbnailPixelY = t.settings.PixelPosition.y.ToString();
            thumbnailScaleX = t.settings.Scale.x.ToString("F2");
            thumbnailScaleY = t.settings.Scale.y.ToString("F2");
            thumbnailTextureSize = texSize.ToString();
            s.ReloadThumbnailSettings = false;
        }

        float containerWidth = EditorGUIUtility.currentViewWidth * Increment.Percent33;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(containerWidth));

        // Parse values
        int.TryParse(thumbnailPixelX, out int pX);
        int.TryParse(thumbnailPixelY, out int pY);
        float.TryParse(thumbnailScaleX, out float sX);
        float.TryParse(thumbnailScaleY, out float sY);
        int.TryParse(thumbnailTextureSize, out int tSize);

        int oldPX = pX, oldPY = pY, oldT = tSize;
        float oldSX = sX, oldSY = sY;

        // Inputs
        pX = EditorGUILayout.IntField("pixelX", pX);
        pY = EditorGUILayout.IntField("pixelY", pY);
        sX = EditorGUILayout.FloatField("scaleX", sX);
        sY = EditorGUILayout.FloatField("scaleY", sY);
        tSize = Mathf.Max(1, EditorGUILayout.IntField("textureSize", Mathf.Max(1, tSize)));

        void apply()
        {
            var selected = GameManager.instance.selectedActor;
            if (selected != null && selected.Thumbnail != null)
            {
                var ts = new Assets.Scripts.Models.ThumbnailSettings(new Vector2Int(pX, pY), new Vector2(sX, sY), tSize);
                selected.Thumbnail.settings = ts;

                // Apply to transform for immediate preview in world
                selected.Thumbnail.transform.localPosition = ts.Offset;
                selected.Thumbnail.transform.localScale = ts.Scale;
            }
        }

        if (pX != oldPX || pY != oldPY ||
            !Mathf.Approximately(sX, oldSX) ||
            !Mathf.Approximately(sY, oldSY) ||
            tSize != oldT)
        {
            apply();
        }

        // Save back to strings
        thumbnailPixelX = pX.ToString();
        thumbnailPixelY = pY.ToString();
        thumbnailScaleX = sX.ToString("F2");
        thumbnailScaleY = sY.ToString("F2");
        thumbnailTextureSize = tSize.ToString();

        // Buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update", GUILayout.Width(64))) apply();

        if (GUILayout.Button("Export", GUILayout.Width(64)))
        {
            // Export snippet using pixel-based constructor
            string exportText =
                $"    ThumbnailSettings = new ThumbnailSettings(new Vector2Int({pX}, {pY}), new Vector2({sX}f, {sY}f), {tSize}),";

            EditorGUIUtility.systemCopyBuffer = exportText;
            Debug.Log($"Copied `{GameManager.instance.selectedActor.characterClass}` ThumbnailSettings (pixel-based) to clipboard.");
        }
        GUILayout.EndHorizontal();

        // Info: show derived offset (read-only) for reference
        var sel = GameManager.instance.selectedActor;
        if (sel != null && sel.Thumbnail != null && sel.Thumbnail.settings != null)
        {
            var off = sel.Thumbnail.settings.Offset;
            EditorGUILayout.LabelField("derivedOffset", $"({off.x:F2}, {off.y:F2})");
        }

        GUILayout.Space(10);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
#endif
    }
}
