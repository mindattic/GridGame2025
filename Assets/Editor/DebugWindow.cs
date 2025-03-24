using Assets.Scripts.Models;
using Assets.Scripts.Store;
using Game.Behaviors;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// This static class is responsible for triggering the debug window when the Game scene loads.
// It uses a runtime initialization attribute to automatically run after the scene loads.
public static class DebugWindowTrigger
{
    // Called automatically after a scene is loaded.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // Only proceed if the Editor is playing.
        if (!EditorApplication.isPlaying)
            return;

        // If the active scene is the "Game" scene, enqueue a call to open the debug window.
        if (SceneManager.GetActiveScene().name == "Game")
        {
            // Uncomment the Debug.Log below for logging if needed.
            // Debug.Log("[DebugWindow] Game scene loaded, opening Debug Window.");
            EditorApplication.delayCall += OpenDebugWindow;
        }
    }

    // Opens the Debug Window by calling its static ShowWindow() method.
    private static void OpenDebugWindow()
    {
        DebugWindow.ShowWindow();
    }
}

// This attribute ensures that the static constructor is called when the Unity Editor loads.
[InitializeOnLoad]
public class DebugWindow : EditorWindow
{
    // Singleton instance of the DebugWindow.
    private static DebugWindow instance;
    // Tracks whether the window is currently open.
    private static bool isWindowOpen = false;

    // Scroll position for the log area.
    private Vector2 scrollPosition;
    // Timestamp of the last window update.
    private DateTime lastUpdateTime;
    // Interval between UI updates (in seconds).
    private float updateInterval = 1.0f;

    // References to various game systems retrieved from GameManager.
    private GameManager gameManager;
    private DebugManager debugManager;
    private ConsoleManager consoleManager;
    private TurnManager turnManager;
    private StageManager stageManager;
    private LogManager logManager;
    private SelectedPlayerManager selectedPlayerManager;

    // Debug window UI selections for game speed, debug options, and VFX testing.
    private GameSpeedOption selectedGameSpeed = GameSpeedOption.Normal;
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
        isWindowOpen = true;
    }

    // Closes the debug window.
    public static void CloseWindow()
    {
        if (instance == null)
            return;

        instance.Close();
        instance = null;
        isWindowOpen = false;
    }

    // Called when play mode state changes (e.g., entering play mode).
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
#if UNITY_EDITOR_WIN
        // When entering play mode, close any open debug window.
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (isWindowOpen)
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
        if (!EditorApplication.isPlaying) return;

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

    // Assign sets up references to game systems from the GameManager.
    private void Initialize()
    {
        // If GameManager hasn't been initialized, do not proceed.
        if (GameManager.instance == null)
            return;

        instance = this;
        isWindowOpen = true;
        lastUpdateTime = DateTime.Now;

        // Retrieve references from the GameManager.
        gameManager = GameManager.instance;
        debugManager = gameManager.debugManager;
        consoleManager = gameManager.consoleManager;
        turnManager = gameManager.turnManager;
        stageManager = gameManager.stageManager;
        logManager = gameManager.logManager;
        selectedPlayerManager = gameManager.selectedPlayerManager;

        // Set initial debug flag values.
        debugManager.showActorNameTag = false;
        debugManager.showActorFrame = false;
        debugManager.showTutorials = false;
        debugManager.isPlayerInvincible = false;
        debugManager.isEnemyInvincible = false;
        debugManager.isTimerInfinite = false;
        debugManager.isEnemyStunned = false;

        // Register the update method so that the window repaints regularly.
        EditorApplication.update += OnEditorUpdate;
    }

    // OnDisable is called when the window is closed.
    private void OnDisable()
    {
        isWindowOpen = false;
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
        // If not playing or missing essential references, exit.
        if (!EditorApplication.isPlaying ||
            gameManager == null ||
            debugManager == null ||
            consoleManager == null ||
            turnManager == null ||
            stageManager == null ||
            logManager == null ||
            selectedPlayerManager == null)
            return;

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
        RenderKeyboard();
        RenderScenes();
        RenderStats();
        RenderCheckboxes();
        RenderGameSpeedDropdown();
        RenderDebugOptionsDropdown();
        RenderVFXDropdown();
        RenderLevelControls();
        RenderThumbnailSettings();
        RenderSpawnControls();
        RenderActorStats();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    // RenderKeyboard draws UI buttons that simulate keyboard arrow keys.
    private void RenderKeyboard()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Keyboard");
        GUILayout.EndHorizontal();

        bool isClicked;

        // Render "Up" arrow in the center.
        GUILayout.BeginHorizontal();
        GUILayout.Space(38); // Space to center the button.
        isClicked = GUILayout.Button("\u2191", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyUp();
        GUILayout.Space(38);
        GUILayout.EndHorizontal();

        // Render "Left", "Down", and "Right" arrows.
        GUILayout.BeginHorizontal();
        isClicked = GUILayout.Button("\u2190", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyLeft();

        isClicked = GUILayout.Button("\u2193", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyDown();

        isClicked = GUILayout.Button("\u2192", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyRight();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(10);
        Repaint(); // Force a UI update.
    }

    // RenderScenes draws buttons to switch between different game scenes.
    private void RenderScenes()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scenes");
        GUILayout.EndHorizontal();

        bool isClicked;

        GUILayout.BeginHorizontal();
        isClicked = GUILayout.Button("Splash Screen", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneStore.instance.LoadScene(SceneHelper.Splash);

        isClicked = GUILayout.Button("Title Screen", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneStore.instance.LoadScene(SceneHelper.Title);

        isClicked = GUILayout.Button("Options Screen", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneStore.instance.LoadScene(SceneHelper.Settings);

        isClicked = GUILayout.Button("Game", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneStore.instance.LoadScene(SceneHelper.Game);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderStats displays key game statistics such as FPS, turn info, phase, and runtime.
    private void RenderStats()
    {
        GUILayout.BeginHorizontal();
        //GUILayout.Label($"FPS: {consoleManager.fpsMonitor.currentFps}", GUILayout.Width(Screen.thumbnailWidth * 0.25f));
        GUILayout.Label($"Turn: {(turnManager.isPlayerTurn ? "Player" : "Opponent")}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"Phase: {turnManager.currentPhase}", GUILayout.Width(Screen.width * 0.25f));
        //GUILayout.Label($"Runtime: {Time.time:F2}", GUILayout.Width(Screen.thumbnailWidth * 0.25f));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderCheckboxes provides several toggles for various debug options.
    private void RenderCheckboxes()
    {
        bool onCheckChanged;

        GUILayout.BeginHorizontal();

        // Toggle to show or hide actor name tags.
        onCheckChanged = EditorGUILayout.Toggle("Show Actor Key Tags?", debugManager.showActorNameTag, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.showActorNameTag != onCheckChanged)
        {
            debugManager.showActorNameTag = onCheckChanged;
            gameManager.actors.ForEach(x => x.render.SetNameTagEnabled(onCheckChanged));
        }

        // Toggle to show or hide actor frames.
        onCheckChanged = EditorGUILayout.Toggle("Show Actor Frames?", debugManager.showActorFrame, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.showActorFrame != onCheckChanged)
        {
            debugManager.showActorFrame = onCheckChanged;
            gameManager.actors.ForEach(x => x.render.SetFrameEnabled(onCheckChanged));
        }

        // Toggle to show or hide tutorial popups.
        onCheckChanged = EditorGUILayout.Toggle("Show Tutorials", debugManager.showTutorials, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.showTutorials != onCheckChanged)
        {
            debugManager.showTutorials = onCheckChanged;
            gameManager.tutorialPopup.gameObject.SetActive(debugManager.showTutorials);
        }

        // Toggle for player invincibility.
        onCheckChanged = EditorGUILayout.Toggle("Are Players Invincible?", debugManager.isPlayerInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isPlayerInvincible != onCheckChanged)
            debugManager.isPlayerInvincible = onCheckChanged;

        // Toggle for enemy invincibility.
        onCheckChanged = EditorGUILayout.Toggle("Are Enemies Invincible?", debugManager.isEnemyInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isEnemyInvincible != onCheckChanged)
            debugManager.isEnemyInvincible = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        // Toggle for infinite timer.
        onCheckChanged = EditorGUILayout.Toggle("Matches Timer Infinite?", debugManager.isTimerInfinite, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isTimerInfinite != onCheckChanged)
            debugManager.isTimerInfinite = onCheckChanged;

        // Toggle for enemy stunned state.
        onCheckChanged = EditorGUILayout.Toggle("Matches Opponent Stunned?", debugManager.isEnemyStunned, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isEnemyStunned != onCheckChanged)
            debugManager.isEnemyStunned = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderGameSpeedDropdown renders a dropdown to select the game speed and an Apply button.
    private void RenderGameSpeedDropdown()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Game Speed", GUILayout.Width(Screen.width * 0.25f));
        selectedGameSpeed = (GameSpeedOption)EditorGUILayout.EnumPopup(selectedGameSpeed, GUILayout.Width(Screen.width * 0.5f));
        if (GUILayout.Button("Apply", GUILayout.Width(Screen.width * 0.25f)))
            OnGameSpeedChange();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderDebugOptionsDropdown renders a dropdown for various debug options and a Run button.
    private void RenderDebugOptionsDropdown()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Debug Options", GUILayout.Width(Screen.width * 0.25f));
        selectedOption = (DebugOptions)EditorGUILayout.EnumPopup(selectedOption, GUILayout.Width(Screen.width * 0.5f));
        if (GUILayout.Button("Run", GUILayout.Width(Screen.width * 0.25f)))
            OnDebugOptionRunClick();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderVFXDropdown renders a dropdown to select a VFX option and a Start button.
    private void RenderVFXDropdown()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("VFX", GUILayout.Width(Screen.width * 0.25f));
        selectedVfx = (VFX)EditorGUILayout.EnumPopup(selectedVfx, GUILayout.Width(Screen.width * 0.5f));
        if (GUILayout.Button("Start", GUILayout.Width(Screen.width * 0.25f)))
            OnPlayVFXClick();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderLevelControls renders buttons for stage control: Reload, Previous, and Next.
    private void RenderLevelControls()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reload", GUILayout.Width(Screen.width * Constants.percent33)))
            OnReloadStageClick();

        //if (GUILayout.Button("< Previous", GUILayout.Width(Screen.thumbnailWidth * Constants.percent33)))
        //    OnPreviousStageClick();

        //if (GUILayout.Button("Next >", GUILayout.Width(Screen.thumbnailWidth * Constants.percent33)))
        //    OnNextStageClick();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // Class-level fields
    private string thumbnailX = "0";
    private string thumbnailY = "0";
    private string thumbnailWidth = "256";
    private string thumbnailHeight = "256";

    private void RenderThumbnailSettings()
    {
        // First horizontal row: the header label.
        GUILayout.BeginHorizontal();
        GUILayout.Label("Thumbnail Settings", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        // Second horizontal row: the controls.
        GUILayout.BeginHorizontal();
        GUILayout.Label("X:", GUILayout.Width(20));
        thumbnailX = GUILayout.TextField(thumbnailX, GUILayout.Width(64));
        GUILayout.Label("Y:", GUILayout.Width(20));
        thumbnailY = GUILayout.TextField(thumbnailY, GUILayout.Width(64));
        GUILayout.Label("Width:", GUILayout.Width(50));
        thumbnailWidth = GUILayout.TextField(thumbnailWidth, GUILayout.Width(64));
        GUILayout.Label("Height:", GUILayout.Width(50));
        thumbnailHeight = GUILayout.TextField(thumbnailHeight, GUILayout.Width(64));

        // Generate button.
        if (GUILayout.Button("Generate", GUILayout.Width(64)))
        {
            // Try parsing the inputs to integers.
            if (int.TryParse(thumbnailX, out int x) &&
                int.TryParse(thumbnailY, out int y) &&
                int.TryParse(thumbnailWidth, out int w) &&
                int.TryParse(thumbnailHeight, out int h))
            {
                var settings = new ThumbnailSettings(x, y, w, h);
                GameManager.instance.focusedActor.thumbnail.Generate(settings);
            }
            else
            {
                Debug.Log("Invalid input! Please enter valid integer values.");
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }


    // RenderSpawnControls renders buttons to spawn various enemy types.
    private void RenderSpawnControls()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("TriggerSpawn", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        bool isClicked;
        GUILayout.BeginHorizontal();

        isClicked = GUILayout.Button("Slime", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            debugManager.SpawnSlime();

        isClicked = GUILayout.Button("Bat", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            debugManager.SpawnBat();

        isClicked = GUILayout.Button("Scorpion", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            debugManager.SpawnScorpion();

        isClicked = GUILayout.Button("Yeti", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            debugManager.SpawnYeti();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderActorStats displays a list of all player and enemy actors with basic status info.
    private void RenderActorStats()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Actors", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        // Display player stats sorted by name.
        foreach (var x in gameManager.players.OrderBy(x => x.name))
        {
            GUILayout.BeginHorizontal();
            string stats = $"{x.name}, IsAlive? {x.isAlive}, IsActive? {x.isActive}";
            GUILayout.Label(stats, GUILayout.Width(Screen.width));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        // Display enemy stats sorted by name.
        foreach (var x in gameManager.enemies.OrderBy(x => x.name))
        {
            GUILayout.BeginHorizontal();
            string stats = $"{x.name}, IsAlive? {x.isAlive}, IsActive? {x.isActive}";
            GUILayout.Label(stats, GUILayout.Width(Screen.width));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
    }


    // OnGameSpeedChange adjusts the game speed based on the selected option.
    private void OnGameSpeedChange()
    {
        switch (selectedGameSpeed)
        {
            case GameSpeedOption.Paused:
                gameManager.gameSpeed = 0f;
                break;
            case GameSpeedOption.Slower:
                gameManager.gameSpeed = 0.25f;
                break;
            case GameSpeedOption.Slow:
                gameManager.gameSpeed = 0.5f;
                break;
            case GameSpeedOption.Normal:
                gameManager.gameSpeed = 1f;
                break;
            case GameSpeedOption.Fast:
                gameManager.gameSpeed = 2f;
                break;
            case GameSpeedOption.Faster:
                gameManager.gameSpeed = 4f;
                break;
        }
    }

    // OnDebugOptionRunClick executes a test based on the selected debug option.
    private void OnDebugOptionRunClick()
    {
        switch (selectedOption)
        {
            case DebugOptions.DodgeTest: debugManager.DodgeTest(); break;
            case DebugOptions.SpinTest: debugManager.SpinTest(); break;
            case DebugOptions.ShakeTest: debugManager.ShakeTest(); break;
            case DebugOptions.AlignTest: debugManager.AlignTest(); break;
            case DebugOptions.CoinTest: debugManager.CoinTest(); break;
            case DebugOptions.PortraitTest: debugManager.PortraitTest(); break;
            case DebugOptions.DamageTextTest: debugManager.DamageTextTest(); break;
            case DebugOptions.DamageTextBounceTest: debugManager.DamageTextBounceTest(); break;
            case DebugOptions.BumpTest: debugManager.BumpTest(); break;
            case DebugOptions.SupportLineTest: debugManager.SupportLineTest(); break;
            case DebugOptions.AttackLineTest: debugManager.AttackLineTest(); break;
            case DebugOptions.EnemyAttackTest: debugManager.EnemyAttackTest(); break;
            case DebugOptions.TitleTest: debugManager.TitleTest(); break;
            case DebugOptions.TooltipTest: debugManager.TooltipTest(); break;
            case DebugOptions.TutorialTest: debugManager.TooltipTest(); break;
            case DebugOptions.FireballTest: debugManager.FireballTest(); break;
            case DebugOptions.HealTest: debugManager.HealTest(); break;
            default: Debug.LogWarning("OnDebugOptionRunClick failed."); break;
        }
    }

    // OnPlayVFXClick triggers a visual effects test based on the selected VFX option.
    private void OnPlayVFXClick()
    {
        switch (selectedVfx)
        {
            case VFX.BlueSlash1: debugManager.VFXTest_BlueSlash1(); break;
            case VFX.BlueSlash2: debugManager.VFXTest_BlueSlash2(); break;
            case VFX.BlueSlash3: debugManager.VFXTest_BlueSlash3(); break;
            case VFX.BlueSword: debugManager.VFXTest_BlueSword(); break;
            case VFX.BlueSword4X: debugManager.VFXTest_BlueSword4X(); break;
            case VFX.BloodClaw: debugManager.VFXTest_BloodClaw(); break;
            case VFX.LevelUp: debugManager.VFXTest_LevelUp(); break;
            case VFX.YellowHit: debugManager.VFXTest_YellowHit(); break;
            case VFX.DoubleClaw: debugManager.VFXTest_DoubleClaw(); break;
            case VFX.LightningExplosion: debugManager.VFXTest_LightningExplosion(); break;
            case VFX.BuffLife: debugManager.VFXTest_BuffLife(); break;
            case VFX.RotaryKnife: debugManager.VFXTest_RotaryKnife(); break;
            case VFX.AirSlash: debugManager.VFXTest_AirSlash(); break;
            case VFX.FireRain: debugManager.VFXTest_FireRain(); break;
            case VFX.VFXTest_Ray_Blast: debugManager.VFXTest_RayBlast(); break;
            case VFX.LightningStrike: debugManager.VFXTest_LightningStrike(); break;
            case VFX.PuffyExplosion: debugManager.VFXTest_PuffyExplosion(); break;
            case VFX.RedSlash2X: debugManager.VFXTest_RedSlash2X(); break;
            case VFX.GodRays: debugManager.VFXTest_GodRays(); break;
            case VFX.AcidSplash: debugManager.VFXTest_AcidSplash(); break;
            case VFX.GreenBuff: debugManager.VFXTest_GreenBuff(); break;
            case VFX.GoldBuff: debugManager.VFXTest_GoldBuff(); break;
            case VFX.HexShield: debugManager.VFXTest_HexShield(); break;
            case VFX.ToxicCloud: debugManager.VFXTest_ToxicCloud(); break;
            case VFX.OrangeSlash: debugManager.VFXTest_OrangeSlash(); break;
            case VFX.MoonFeather: debugManager.VFXTest_MoonFeather(); break;
            case VFX.PinkSpark: debugManager.VFXTest_PinkSpark(); break;
            case VFX.BlueYellowSword: debugManager.VFXTest_BlueYellowSword(); break;
            case VFX.BlueYellowSword3X: debugManager.VFXTest_BlueYellowSword3X(); break;
            case VFX.RedSword: debugManager.VFXTest_RedSword(); break;
            default: Debug.LogWarning("OnPlayVFXClick failed."); break;
        }
    }

    // Stage control methods:
    // Reloads the CurrentProfile stage.
    private void OnReloadStageClick() => stageManager.RestartStage();
    // Moves to the previous stage.
    //private void OnPreviousStageClick() => stageManager.Previous();
    // Moves to the next stage.
    //private void OnNextStageClick() => stageManager.Next();

    // Keyboard control methods for actor movement.
    private void OnKeyUp() => GameManager.instance.focusedActor?.Move(Vector2Int.down);
    private void OnKeyDown() => GameManager.instance.focusedActor?.Move(Vector2Int.up);
    private void OnKeyLeft() => GameManager.instance.focusedActor?.Move(Vector2Int.left);
    private void OnKeyRight() => GameManager.instance.focusedActor?.Move(Vector2Int.right);
}
