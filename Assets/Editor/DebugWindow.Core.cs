using Assets.Helper;
using Assets.Scripts.Repositories;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using g = Assets.Helpers.GameHelper;

// This attribute ensures that the static constructor is called when the Unity Editor loads.
[InitializeOnLoad]
public partial class DebugWindow : EditorWindow
{
    // Singleton instance of the DebugWindow.
    public static DebugWindow instance;
    // Tracks whether the window is currently open.
    public static bool isOpen = false;

    // Scroll position for the log area.
    private Vector2 scrollPosition;
    // Timestamp of the last window update.
    private DateTime lastUpdateTime;
    // Interval between UI updates (in seconds).
    private float updateInterval = 1.0f;

    // Debug window UI selections for game speed, debug options, and VfxManager testing.
    private GameSpeedOption selectedGameFocus = GameSpeedOption.Normal;
    private DebugOptions selectedOption = DebugOptions.None;
    private VFX selectedVfx = VFX.None;

    // Static constructor is called on load in the editor and subscribes to play mode state changes.
    static DebugWindow()
    {
        // Subscribe to changes in play mode (e.g., entering or exiting play mode).
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    // Adds a menu item in the Unity Editor under "Window/Debug Window".
    [MenuItem("Window/Debug Window")]
    public static void ShowWindow()
    {
        // Open or focus the debug window.
        instance = GetWindow<DebugWindow>("Debug Window");
        isOpen = true;
    }

    // Closes the debug window.
    public static void CloseWindow()
    {
        if (instance == null)
            return;

        instance.Close();
        instance = null;
        isOpen = false;
    }

    // Called when play mode state changes (e.g., entering play mode).
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
#if UNITY_EDITOR_WIN
        // When entering play mode, close any open debug window.
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (isOpen)
                CloseWindow();

            // Delay re-opening the window until the Game scene has loaded.
            EditorApplication.delayCall += WaitForGameScene;
        }
#endif
    }

#if UNITY_EDITOR_WIN
    // Enqueues a call to check for the Game scene loading.
    private static void WaitForGameScene()
    {
        // Check every frame whether the Game scene is loaded.
        EditorApplication.update += CheckSceneLoad;
    }

    // Checks if the active scene is "Game" and opens the debug window when it is.
    private static void CheckSceneLoad()
    {
        if (!Application.isPlaying || !SceneHelper.IsGameScene)
        {
            CloseWindow();
            return;
        }

        if (SceneManager.GetActiveScene().name == "Game")
        {
            // Uncomment Debug.Log below for logging purposes.
            // Debug.Log("[DebugWindow] Game scene detected, opening Debug Window.");
            ShowWindow();
            // Once the scene is confirmed, stop checking.
            EditorApplication.update -= CheckSceneLoad;
        }
    }
#endif

    // OnEnable is called when the window is opened or re-enabled.
    private void OnEnable()
    {
        // Delay initialization until after the CurrentProfile frame (and only if playing).
        DelayCall(() =>
        {
            Initialize();
        });
    }

    // Helper method to delay a call using EditorApplication.delayCall.
    private static void DelayCall(Action action)
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlaying)
            {
                action();
            }
        };
    }

    // Show sets up references to game systems from the GameManager.
    private void Initialize()
    {
        // If GameManager hasn't been initialized, do not proceed.
        if (GameManager.instance == null)
            return;

        instance = this;
        isOpen = true;
        lastUpdateTime = DateTime.Now;

        // Show initial debug flag values.
        GameManager.instance.debugManager.showActorNameTag = false;
        GameManager.instance.debugManager.showActorFrame = false;
        GameManager.instance.debugManager.showTutorials = false;
        GameManager.instance.debugManager.isHeroInvincible = false;
        GameManager.instance.debugManager.isEnemyInvincible = false;
        GameManager.instance.debugManager.isTimerInfinite = false;
        GameManager.instance.debugManager.isEnemyStunned = false;

        // Register the update method so that the window repaints regularly.
        EditorApplication.update += OnEditorUpdate;
    }

    // OnDisable is called when the window is closed.
    private void OnDisable()
    {
        isOpen = false;
        instance = null;

        // Unregister event handlers.
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.delayCall = null;
    }

    // OnEditorUpdate is called repeatedly by the Editor to update the window.
    private void OnEditorUpdate()
    {
        // If the update interval has elapsed, repaint the window.
        if ((DateTime.Now - lastUpdateTime).TotalSeconds >= updateInterval)
        {
            lastUpdateTime = DateTime.Now;
            Repaint();
        }
    }

    private void OnGUI()
    {
        try
        {

            if (!Application.isPlaying || !SceneHelper.IsGameScene)
            {
                CloseWindow();
                return;
            }

            // Wrap all content inside a scroll view.
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                /* alwaysShowHorizontal */ false,
                /* alwaysShowVertical */ false,
                GUILayout.Width(position.width),
                GUILayout.Height(position.height)
            );

            // Begin a vertical layout for the entire Debug Window UI.
            GUILayout.BeginVertical();

            // Render individual UI sections.
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

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
