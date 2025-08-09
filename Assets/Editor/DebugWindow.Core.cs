using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// DebugWindow
/// Purpose:
///   Editor window that can stay open across scenes. It only renders content while in the Game scene.
///   Rendering is guarded to avoid null references and IMGUI imbalance during scene switches.
/// </summary>
public partial class DebugWindow : EditorWindow
{
    public static DebugWindow instance;
    public static bool isOpen = false;

    private Vector2 scrollPosition;
    private DateTime lastUpdateTime;
    private float updateInterval = 1.0f;

    private GameSpeedOption selectedGameFocus = GameSpeedOption.Normal;
    private DebugOptions selectedOption = DebugOptions.None;
    private VFX selectedVfx = VFX.None;

    static DebugWindow()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem("Window/Debug Window")]
    public static void ShowWindow()
    {
        instance = GetWindow<DebugWindow>("Debug Window");
        isOpen = true;
    }

    public static void CloseWindow()
    {
        if (instance == null)
            return;

        instance.Close();
        instance = null;
        isOpen = false;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
#if UNITY_EDITOR_WIN
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Do not close here. Let the trigger open it after Game scene loads.
            EditorApplication.delayCall += WaitForGameScene;
        }
#endif
    }

#if UNITY_EDITOR_WIN
    private static void WaitForGameScene()
    {
        EditorApplication.update += CheckSceneLoad;
    }

    private static void CheckSceneLoad()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.update -= CheckSceneLoad;
            return;
        }

        var scene = SceneManager.GetActiveScene();
        if (scene.IsValid() && scene.name == "Game")
        {
            ShowWindow();
            EditorApplication.update -= CheckSceneLoad;
        }
    }
#endif

    private void OnEnable()
    {
        DelayCall(Initialize);
    }

    private static void DelayCall(Action action)
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlaying)
                action();
        };
    }

    private void Initialize()
    {
        instance = this;
        isOpen = true;
        lastUpdateTime = DateTime.Now;

        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        isOpen = false;
        instance = null;

        EditorApplication.update -= OnEditorUpdate;
        // Do not null out EditorApplication.delayCall. That would clear all editor listeners.
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnEditorUpdate()
    {
        if ((DateTime.Now - lastUpdateTime).TotalSeconds >= updateInterval)
        {
            lastUpdateTime = DateTime.Now;
            Repaint();
        }
    }

    private void OnGUI()
    {
        // Only draw content while playing and while the active scene is Game.
        if (!Application.isPlaying || !IsActiveSceneGame())
        {
            // Render nothing. Window stays open.
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug Window is only available in `Game` scene.");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return;
        }

        try
        {
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                false,
                false,
                GUILayout.Width(position.width),
                GUILayout.Height(position.height)
            );

            GUILayout.BeginVertical();
            try
            {
                RenderGameStats();
                RenderThumbnailSettings();
                RenderGameSpeed();
                RenderDebugOptions();
                RenderVfxOptions();
                RenderKeyboard();
                RenderCheckboxes();
                RenderLevels();
                RenderScenes();
                RenderSpawnOptions();
                RenderActorStats();
            }
            finally
            {
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }
        catch (Exception ex)
        {
            // Swallow to keep IMGUI stable; do not close or reopen.
            Debug.LogError(ex);
        }
    }

    private static bool IsActiveSceneGame()
    {
        var scene = SceneManager.GetActiveScene();
        return scene.IsValid() && scene.name == "Game";
    }
}
