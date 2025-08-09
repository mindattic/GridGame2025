#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Helper;

/// <summary>
/// DebugWindowTrigger
/// Purpose:
///   Opens DebugWindow a short time after the Game scene becomes active,
///   and guarantees cleanup when switching away from Game or exiting play mode.
/// Notes:
///   - Uses Editor-only hooks and guards all subscriptions.
///   - Never relies on a cached IsGameScene flag that can go stale on scene changes.
/// </summary>
public static class DebugWindowTrigger
{
    private const string GameSceneName = "Game";

    private static float delayTime = 3f;
    private static float elapsedTime = 0f;
    private static bool isWaiting = false;
    private static bool subscribedUpdate = false;
    private static bool subscribedScene = false;
    private static bool subscribedPlayMode = false;

    // Called after each scene load while playing in the Editor.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // Ensure scene and play mode watchers are live once per play session.
        EnsureSceneSubscription();
        EnsurePlayModeSubscription();

        // Only act if we are in play mode and currently in the Game scene.
        if (!Application.isPlaying || !IsActiveSceneGame())
        {
            CancelWait();
            SafeCloseWindow();
            return;
        }

        // Restart the delayed open every time Game loads.
        BeginWait();
    }

    // Update loop used only during the delay window.
    private static void WaitAndOpenDebugWindow()
    {
        if (!isWaiting)
            return;

        // Abort if we left play mode or the active scene is no longer Game.
        if (!Application.isPlaying || !IsActiveSceneGame())
        {
            CancelWait();
            SafeCloseWindow();
            return;
        }

        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= delayTime)
        {
            CancelWait();
            SafeOpenWindow();
        }
    }

    // Scene change watcher. Closes or cancels when leaving Game.
    private static void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        // If we left Game, stop waiting and close the window.
        if (!IsSceneGame(newScene))
        {
            CancelWait();
            SafeCloseWindow();
        }
        else
        {
            // Entered Game while playing. Start the delayed open cycle.
            if (Application.isPlaying)
                BeginWait();
        }
    }

    // Play mode watcher. Cleanup when exiting play mode to avoid stray subscriptions.
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
        {
            CancelWait();
            SafeCloseWindow();
            // Keep scene subscription; it is harmless, but all update hooks are removed.
        }
    }

    // Starts or restarts the delayed open cycle.
    private static void BeginWait()
    {
        isWaiting = true;
        elapsedTime = 0f;
        EnsureUpdateSubscription();
    }

    // Cancels any pending delayed open and detaches update if attached.
    private static void CancelWait()
    {
        isWaiting = false;
        elapsedTime = 0f;
        RemoveUpdateSubscription();
    }

    // Opens the DebugWindow with exception safety.
    private static void SafeOpenWindow()
    {
        try
        {
            // Double-check scene before opening.
            if (Application.isPlaying && IsActiveSceneGame())
                DebugWindow.ShowWindow();
        }
        catch
        {
            // Swallow to protect the Editor from transient failures during scene loads.
        }
    }

    // Closes the DebugWindow if it is open.
    private static void SafeCloseWindow()
    {
        try
        {
            if (DebugWindow.isOpen)
                DebugWindow.CloseWindow();
        }
        catch
        {
            // Swallow to avoid GUI errors if the window is in a bad state during scene changes.
        }
    }

    // Helpers
    private static bool IsActiveSceneGame()
    {
        var scene = SceneManager.GetActiveScene();
        return scene.IsValid() && scene.name == GameSceneName;
    }

    private static bool IsSceneGame(Scene scene)
    {
        return scene.IsValid() && scene.name == GameSceneName;
    }

    // Subscription management
    private static void EnsureUpdateSubscription()
    {
        if (subscribedUpdate) return;
        EditorApplication.update += WaitAndOpenDebugWindow;
        subscribedUpdate = true;
    }

    private static void RemoveUpdateSubscription()
    {
        if (!subscribedUpdate) return;
        EditorApplication.update -= WaitAndOpenDebugWindow;
        subscribedUpdate = false;
    }

    private static void EnsureSceneSubscription()
    {
        if (subscribedScene) return;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        subscribedScene = true;
    }

    private static void EnsurePlayModeSubscription()
    {
        if (subscribedPlayMode) return;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        subscribedPlayMode = true;
    }
}
#endif
