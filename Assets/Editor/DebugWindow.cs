using Assets.Scripts.Repositories;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using g = Assets.Helpers.GameManagerHelper;

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
            // Start waiting process
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


// This attribute ensures that the static constructor is called when the Unity Editor loads.
[InitializeOnLoad]
public class DebugWindow : EditorWindow
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
        // If not playing or missing essential references, exit.
        if (!EditorApplication.isPlaying)
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
        RenderStats();
        RenderThumbnailSettings();
        RenderGameSpeedDropdown();
        RenderDebugOptionsDropdown();
        RenderVFXDropdown();
        RenderKeyboard();
        RenderCheckboxes();
        RenderLevelControls();
        RenderScenes();
        RenderSpawnControls();
        RenderActorStats();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    // RenderStats displays key game statistics such as FPS, turn info, phase, and runtime.
    private void RenderStats()
    {
        GUILayout.BeginHorizontal();


        GUILayout.Label($"Focused Actor: {(g.Actors.FocusedActor ? g.Actors.FocusedActor.characterName : "-")}", GUILayout.Width(Screen.width * 0.25f));
        //GUILayout.Label($"FPS: {consoleManager.fpsMonitor.currentFps}", GUILayout.Width(Screen.thumbnailScaleX * 0.25f));


        GUILayout.Label($"InputManager Mode: {g.InputManager.inputMode.ToString()}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"TurnManager: {(g.TurnManager.isHeroTurn ? "Hero" : "Opponent")}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"Phase: {g.TurnManager.currentPhase}", GUILayout.Width(Screen.width * 0.25f));
        //GUILayout.Label($"Runtime: {Time.time:F2}", GUILayout.Width(Screen.thumbnailScaleX * 0.25f));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
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
        isClicked = GUILayout.Button("SplashScreen Screen", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.SplashScreen);

        isClicked = GUILayout.Button("TitleScreen Screen", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.TitleScreen);

        isClicked = GUILayout.Button("Settings", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.Settings);

        isClicked = GUILayout.Button("Stage Select", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.StageSelect);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        isClicked = GUILayout.Button("Load Profile", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.ProfileSelect);

        isClicked = GUILayout.Button("Load Save", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.SaveFileSelect);

        isClicked = GUILayout.Button("Overworld", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.Overworld);

        isClicked = GUILayout.Button("Game", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            SceneRepo.LoadScene(SceneHelper.Game);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }


    // RenderCheckboxes provides several toggles for various debug options.
    private void RenderCheckboxes()
    {
        bool onCheckChanged;

        GUILayout.BeginHorizontal();

        // Toggle to show or hide actor name tags.
        onCheckChanged = EditorGUILayout.Toggle("Spawn Actor Name?", g.DebugManager.showActorNameTag, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.showActorNameTag != onCheckChanged)
        {
            g.DebugManager.showActorNameTag = onCheckChanged;
            g.Actors.All.ForEach(x => x.render.SetNameTagEnabled(onCheckChanged));
        }

        // Toggle to show or hide actor frames.
        onCheckChanged = EditorGUILayout.Toggle("Show Actor Frame?", g.DebugManager.showActorFrame, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.showActorFrame != onCheckChanged)
        {
            g.DebugManager.showActorFrame = onCheckChanged;
            g.Actors.All.ForEach(x => x.render.SetFrameEnabled(onCheckChanged));
        }

        // Toggle to show or hide tutorial popups.
        onCheckChanged = EditorGUILayout.Toggle("Show Tutorials", g.DebugManager.showTutorials, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.showTutorials != onCheckChanged)
        {
            g.DebugManager.showTutorials = onCheckChanged;
            g.TutorialPopup.gameObject.SetActive(g.DebugManager.showTutorials);
        }

        // Toggle for hero invincibility.
        onCheckChanged = EditorGUILayout.Toggle("Are Heroes Invincible?", g.DebugManager.isHeroInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isHeroInvincible != onCheckChanged)
            g.DebugManager.isHeroInvincible = onCheckChanged;

        // Toggle for enemy invincibility.
        onCheckChanged = EditorGUILayout.Toggle("Are Enemies Invincible?", g.DebugManager.isEnemyInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isEnemyInvincible != onCheckChanged)
            g.DebugManager.isEnemyInvincible = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        // Toggle for infinite timer.
        onCheckChanged = EditorGUILayout.Toggle("Is Timer Infinite?", g.DebugManager.isTimerInfinite, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isTimerInfinite != onCheckChanged)
            g.DebugManager.isTimerInfinite = onCheckChanged;

        // Toggle for enemy stunned state.
        onCheckChanged = EditorGUILayout.Toggle("Is Opponent Stunned?", g.DebugManager.isEnemyStunned, GUILayout.Width(Screen.width * 0.25f));
        if (g.DebugManager.isEnemyStunned != onCheckChanged)
            g.DebugManager.isEnemyStunned = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderGameSpeedDropdown renders a dropdown to select the game speed and an Apply button.
    private void RenderGameSpeedDropdown()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Game Speed", GUILayout.Width(Screen.width * 0.25f));
        selectedGameFocus = (GameSpeedOption)EditorGUILayout.EnumPopup(selectedGameFocus, GUILayout.Width(Screen.width * 0.5f));
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

    // RenderVFXDropdown renders a dropdown to select a VfxManager option and a Start button.
    private void RenderVFXDropdown()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("VfxManager", GUILayout.Width(Screen.width * 0.25f));
        selectedVfx = (VFX)EditorGUILayout.EnumPopup(selectedVfx, GUILayout.Width(Screen.width * 0.5f));
        if (GUILayout.Button("Start", GUILayout.Width(Screen.width * 0.25f)))
            OnPlayVFXClick();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderLevelControls renders buttons for stage control: Load, Previous, and Next.
    private void RenderLevelControls()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load", GUILayout.Width(Screen.width * Increment.Percent33)))
            OnReloadStageClick();

        //if (GUILayout.Button("< Previous", GUILayout.Width(Screen.thumbnailScaleX * Constants.percent33)))
        //    OnPreviousStageClick();

        //if (GUILayout.Button("Next >", GUILayout.Width(Screen.thumbnailScaleX * Constants.percent33)))
        //    OnNextStageClick();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // Class-level fields
    private string thumbnailPositionX = "0.5";
    private string thumbnailPositionY = "-1.4";
    private string thumbnailScaleX = "5";
    private string thumbnailScaleY = "5";

    private void RenderThumbnailSettings()
    {
#if UNITY_EDITOR

        if (g.ReloadThumbnailSettings && g.Actors.HasFocusedActor)
        {
            var t = g.Actors.FocusedActor.thumbnail;
            thumbnailPositionX = t.settings.Position.x.ToString("F2");
            thumbnailPositionY = t.settings.Position.y.ToString("F2");
            thumbnailScaleX = t.settings.Scale.x.ToString("F2");
            thumbnailScaleY = t.settings.Scale.y.ToString("F2");
            g.ReloadThumbnailSettings = false;
        }

        float containerWidth = EditorGUIUtility.currentViewWidth * Increment.Percent33;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(containerWidth));

        GUILayout.Label("Thumbnail Settings", EditorStyles.boldLabel);

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
                GameManager.instance.focusedActor.thumbnail.Set(position, scale);
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

    // RenderSpawnControls renders buttons to spawn various enemy types.
    private void RenderSpawnControls()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawn", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        bool isClicked;
        GUILayout.BeginHorizontal();

        isClicked = GUILayout.Button("Slime", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            g.DebugManager.SpawnSlime();

        isClicked = GUILayout.Button("Bat", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            g.DebugManager.SpawnBat();

        isClicked = GUILayout.Button("Scorpion", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            g.DebugManager.SpawnScorpion();

        isClicked = GUILayout.Button("Yeti", GUILayout.Width(Screen.width * Increment.Percent25));
        if (isClicked)
            g.DebugManager.SpawnYeti();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    // RenderActorStats displays a list of all hero and enemy actors with basic status info.
    private void RenderActorStats()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Actors", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        // Display hero stats sorted by name.
        foreach (var x in g.Actors.Heroes.OrderBy(x => x.name))
        {
            GUILayout.BeginHorizontal();
            string stats = $"{x.name}, IsAlive? {x.isAlive}, IsActive? {x.isActive}";
            GUILayout.Label(stats, GUILayout.Width(Screen.width));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        // Display enemy stats sorted by name.
        foreach (var x in g.Actors.Enemies.OrderBy(x => x.name))
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
        switch (selectedGameFocus)
        {
            case GameSpeedOption.Paused:
                g.GameSpeed = 0f;
                break;
            case GameSpeedOption.Slower:
                g.GameSpeed = 0.25f;
                break;
            case GameSpeedOption.Slow:
                g.GameSpeed = 0.5f;
                break;
            case GameSpeedOption.Normal:
                g.GameSpeed = 1f;
                break;
            case GameSpeedOption.Fast:
                g.GameSpeed = 2f;
                break;
            case GameSpeedOption.Faster:
                g.GameSpeed = 4f;
                break;
        }
    }

    // OnDebugOptionRunClick executes a test based on the selected debug option.
    private void OnDebugOptionRunClick()
    {
        switch (selectedOption)
        {
            case DebugOptions.KillEnemies: g.DebugManager.KillEnemies(); break;
            case DebugOptions.DodgeTest: g.DebugManager.DodgeTest(); break;
            case DebugOptions.SpinTest: g.DebugManager.SpinTest(); break;
            case DebugOptions.ShakeTest: g.DebugManager.ShakeTest(); break;
            case DebugOptions.SingleCombo: g.DebugManager.SingleCombo(); break;
            case DebugOptions.TripleCombo: g.DebugManager.TripleCombo(); break;
            case DebugOptions.CoinTest: g.DebugManager.CoinTest(); break;
            case DebugOptions.PortraitSlideIn: g.DebugManager.PortraitSlideIn(); break;
            case DebugOptions.PortraitPopIn: g.DebugManager.PortraitPopIn(); break;
            case DebugOptions.SpawnDamageText: g.DebugManager.SpawnDamageText(); break;
            case DebugOptions.BumpTest: g.DebugManager.BumpTest(); break;
            case DebugOptions.SupportLineTest: g.DebugManager.SupportLineTest(); break;
            case DebugOptions.AttackLineTest: g.DebugManager.AttackLineTest(); break;
            case DebugOptions.EnemyAttackTest: g.DebugManager.EnemyAttackTest(); break;
            case DebugOptions.TitleTest: g.DebugManager.TitleTest(); break;
            case DebugOptions.TooltipTest: g.DebugManager.TooltipTest(); break;
            case DebugOptions.TutorialTest: g.DebugManager.TooltipTest(); break;
            case DebugOptions.FireballTest: g.DebugManager.FireballTest(); break;
            case DebugOptions.HealTest: g.DebugManager.HealTest(); break;
            case DebugOptions.RandomizeBackground: g.DebugManager.RandomizeBackground(); break;
            default: Debug.LogWarning("OnDebugOptionRunClick failed."); break;
        }
    }

    // OnPlayVFXClick triggers a visual effects test based on the selected VfxManager option.
    private void OnPlayVFXClick()
    {
        switch (selectedVfx)
        {
            case VFX.BlueSlash1: g.DebugManager.VFXTest_BlueSlash1(); break;
            case VFX.BlueSlash2: g.DebugManager.VFXTest_BlueSlash2(); break;
            case VFX.BlueSlash3: g.DebugManager.VFXTest_BlueSlash3(); break;
            case VFX.BlueSword: g.DebugManager.VFXTest_BlueSword(); break;
            case VFX.BlueSword4X: g.DebugManager.VFXTest_BlueSword4X(); break;
            case VFX.BloodClaw: g.DebugManager.VFXTest_BloodClaw(); break;
            case VFX.LevelUp: g.DebugManager.VFXTest_LevelUp(); break;
            case VFX.YellowHit: g.DebugManager.VFXTest_YellowHit(); break;
            case VFX.DoubleClaw: g.DebugManager.VFXTest_DoubleClaw(); break;
            case VFX.LightningExplosion: g.DebugManager.VFXTest_LightningExplosion(); break;
            case VFX.BuffLife: g.DebugManager.VFXTest_BuffLife(); break;
            case VFX.RotaryKnife: g.DebugManager.VFXTest_RotaryKnife(); break;
            case VFX.AirSlash: g.DebugManager.VFXTest_AirSlash(); break;
            case VFX.FireRain: g.DebugManager.VFXTest_FireRain(); break;
            case VFX.VFXTest_Ray_Blast: g.DebugManager.VFXTest_RayBlast(); break;
            case VFX.LightningStrike: g.DebugManager.VFXTest_LightningStrike(); break;
            case VFX.PuffyExplosion: g.DebugManager.VFXTest_PuffyExplosion(); break;
            case VFX.RedSlash2X: g.DebugManager.VFXTest_RedSlash2X(); break;
            case VFX.GodRays: g.DebugManager.VFXTest_GodRays(); break;
            case VFX.AcidSplash: g.DebugManager.VFXTest_AcidSplash(); break;
            case VFX.GreenBuff: g.DebugManager.VFXTest_GreenBuff(); break;
            case VFX.GoldBuff: g.DebugManager.VFXTest_GoldBuff(); break;
            case VFX.HexShield: g.DebugManager.VFXTest_HexShield(); break;
            case VFX.ToxicCloud: g.DebugManager.VFXTest_ToxicCloud(); break;
            case VFX.OrangeSlash: g.DebugManager.VFXTest_OrangeSlash(); break;
            case VFX.MoonFeather: g.DebugManager.VFXTest_MoonFeather(); break;
            case VFX.PinkSpark: g.DebugManager.VFXTest_PinkSpark(); break;
            case VFX.BlueYellowSword: g.DebugManager.VFXTest_BlueYellowSword(); break;
            case VFX.BlueYellowSword3X: g.DebugManager.VFXTest_BlueYellowSword3X(); break;
            case VFX.RedSword: g.DebugManager.VFXTest_RedSword(); break;
            default: Debug.LogWarning("OnPlayVFXClick failed."); break;
        }
    }

    // Stage control methods:
    // Reloads the CurrentProfile stage.
    private void OnReloadStageClick() => g.StageManager.RestartStage();
    // Moves to the previous stage.
    //private void OnPreviousStageClick() => g.StageManager.Previous();
    // Moves to the next stage.
    //private void OnNextStageClick() => g.StageManager.Next();

    // Keyboard control methods for actor move.
    private void OnKeyUp() => g.Actors.FocusedActor?.Move(Vector2Int.down);
    private void OnKeyDown() => g.Actors.FocusedActor?.Move(Vector2Int.up);
    private void OnKeyLeft() => g.Actors.FocusedActor?.Move(Vector2Int.left);
    private void OnKeyRight() => g.Actors.FocusedActor?.Move(Vector2Int.right);
}
