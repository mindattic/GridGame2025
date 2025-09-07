#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Serialization;

[CustomEditor(typeof(MapPropEditorBootstrapper))]
public class MapPropLoader : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var loader = (MapPropEditorBootstrapper)target;

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Save Props"))
            {
                if (loader != null && loader.gameObject != null)
                {
                    var root = loader.GetType()
                                     .GetField("propsRoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                     ?.GetValue(loader) as Transform;

                    var mapPath = loader.GetType()
                                        .GetField("mapPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                        ?.GetValue(loader) as string;

                    if (root == null || string.IsNullOrEmpty(mapPath))
                    {
                        Debug.LogError("Assign Props Root and Map Path before saving.");
                    }
                    else
                    {
                        PropMapIO.SaveFrom(root, mapPath, includeInactive: true, bakeOrphans: true);
                    }
                }
            }

            if (GUILayout.Button("Load Props"))
            {
                var root = loader.GetType()
                                 .GetField("propsRoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                 ?.GetValue(loader) as Transform;

                var mapPath = loader.GetType()
                                    .GetField("mapPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                    ?.GetValue(loader) as string;

                if (root == null || string.IsNullOrEmpty(mapPath))
                {
                    Debug.LogError("Assign Props Root and Map Path before loading.");
                }
                else
                {
                    PropMapIO.LoadInto(root, mapPath, clearExisting: true);
                }
            }
        }
    }
}
#endif