using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utilities
{

#if UNITY_STANDALONE_WIN

    public class LogWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string logText = "";
        private int logLevelFilter = 0; //Index of selected textMesh level
        private string[] logLevels = new string[] { "Get", "Log", "Warning", "Error", "Exception" };

        [MenuItem("Window/Custom Log Window")]
        public static void ShowWindow()
        {
            GetWindow<LogWindow>("Log Window");
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            //Only add textMesh if it matches the selected filter
            if (ShouldShowLog(type))
            {
                string color = GetLogColor(type);
                logText += $"<color={color}>{type}: {logString}</color>\n";
                Repaint();
            }
        }

        private bool ShouldShowLog(LogType type)
        {
            if (logLevelFilter == 0) return true; //"Get"

            //Filter textMesh by selected textMesh level
            return logLevelFilter == (int)type;
        }

        private string GetLogColor(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    return "red";
                case LogType.Assert:
                    return "yellow";
                case LogType.Warning:
                    return "orange";
                case LogType.Log:
                    return "white";
                case LogType.Exception:
                    return "purple";
                default:
                    return "white";
            }
        }

        private void OnGUI()
        {
            //Dropdown for selecting textMesh type filter
            logLevelFilter = EditorGUILayout.Popup("Log Level Filter", logLevelFilter, logLevels);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUIStyle style = new GUIStyle
            {
                richText = true //Enable rich text
            };

            GUILayout.Label(logText, style);
            EditorGUILayout.EndScrollView();
        }
    }

#endif

}
