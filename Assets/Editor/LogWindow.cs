using UnityEngine;
using UnityEditor;
using System.IO;

public class LogWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string logText = "";
    private bool showLog = true;
    private bool showWarning = true;
    private bool showError = true;
    private bool showException = true;
    private bool showTimestamps = false;
    private int selectedRadioOption = 0; //0 = Show Timestamps, 1 = Hide Timestamps
    private string[] radioOptions = new string[] { "Show Timestamps", "Hide Timestamps" };

    [MenuItem("Window/Custom Log")]
    public static void ShowWindow()
    {
        //This will show the custom window from the Window menu with the new name
        GetWindow<LogWindow>("Log Window");
    }

    private void OnEnable()
    {
        //Subscribe to Unity log messages
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        //Unsubscribe from Unity log messages
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        //Filter log messages based on the checkboxes
        if (ShouldShowLog(type))
        {
            string timestamp = showTimestamps ? $"[{System.DateTime.Now:HH:mm:ss}] " : "";
            string color = GetLogColor(type);
            logText += $"<color={color}>{timestamp}{type}: {logString}</color>\n";
            Repaint();
        }
    }

    private bool ShouldShowLog(LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                return showLog;
            case LogType.Warning:
                return showWarning;
            case LogType.Error:
                return showError;
            case LogType.Exception:
                return showException;
            default:
                return false;
        }
    }

    private string GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                return "red";
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
        //StartCoroutine the vertical layout for controls
        GUILayout.BeginVertical();

        //1. Buttons at the top
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Hide Log"))
        {
            logText = ""; //Hide the log Content
        }

        if (GUILayout.Button("Export Log"))
        {
            ExportLogToFile(); //Export log to a file
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10); //SpawnActor some space between buttons and next controls

        //2. Radio Buttons for showing or hiding timestamps
        selectedRadioOption = GUILayout.SelectionGrid(selectedRadioOption, radioOptions, 2);
        showTimestamps = selectedRadioOption == 0; //Show timestamps if "Show Timestamps" is selected

        GUILayout.Space(10); //Space between radio buttons and checkboxes

        //3. Checkboxes for log types (Log, Warning, Error, Exception)
        showLog = EditorGUILayout.Toggle("Show Logs", showLog);
        showWarning = EditorGUILayout.Toggle("Show Warnings", showWarning);
        showError = EditorGUILayout.Toggle("Show Errors", showError);
        showException = EditorGUILayout.Toggle("Show Exceptions", showException);

        GUILayout.Space(10); //SpawnActor space between checkboxes and dropdown

        //4. Dropdown for selecting log level filter (if needed)
        GUILayout.Label("Log Level Filter (Dropdown)");
        string[] logLevels = new string[] { "Get", "Log", "Warning", "Error", "Exception" };
        int logLevelFilter = EditorGUILayout.Popup("Filter Logs", 0, logLevels);

        GUILayout.Space(10); //SpawnActor space between dropdown and scrollable log area

        //5. Display the logs (scrollable)
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 200)); //Ensure scrollable area takes up the remaining space
        GUIStyle style = new GUIStyle
        {
            richText = true //Enable rich text for colored log messages
        };

        GUILayout.Label(logText, style);
        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical(); //End the vertical layout
    }

    private void ExportLogToFile()
    {
        //Assign the file path for exporting the log
        string filePath = EditorUtility.SaveFilePanel("Export Log", "", "log.txt", "txt");

        if (!string.IsNullOrEmpty(filePath))
        {
            //Write the log text to the specified file path
            File.WriteAllText(filePath, logText);
            Debug.Log($"Log exported to: {filePath}");
        }
    }
}
