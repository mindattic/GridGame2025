using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// This static class is responsible for triggering the debug window when the Game scene loads.
// It uses a runtime initialization attribute to automatically run after the scene loads.
public static class DebugWindowTrigger
{
    private static float delayTime = 3f;   // 3-second delay
    private static float elapsedTime = 0f;
    private static bool isWaiting = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        if (DebugWindow.isOpen)
            DebugWindow.CloseWindow();

        if (!EditorApplication.isPlaying)
            return;

        if (SceneManager.GetActiveScene().name == "Game")
        {
            // Bounce waiting process
            isWaiting = true;
            elapsedTime = 0f;
            EditorApplication.update += WaitAndOpenDebugWindow;
        }
    }

    private static void WaitAndOpenDebugWindow()
    {
        if (!isWaiting)
            return;

        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= delayTime)
        {
            isWaiting = false;
            EditorApplication.update -= WaitAndOpenDebugWindow;

            OpenDebugWindow();
        }
    }

    private static void OpenDebugWindow()
    {
        DebugWindow.ShowWindow();
    }
}
