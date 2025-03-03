using Game.Behaviors;
using Game.Manager;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class DebugWindowTrigger
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        if (!EditorApplication.isPlaying)
            return;

        // EnqueueAttacks if the Game scene is loaded
        if (SceneManager.GetActiveScene().name == "Game")
        {
            //Debug.Log("[DebugWindow] Game scene loaded, opening Debug Window.");
            EditorApplication.delayCall += OpenDebugWindow;
        }
    }

    private static void OpenDebugWindow()
    {
        DebugWindow.ShowWindow();
    }
}

[InitializeOnLoad] //This attribute ensures that the static constructor is called on load
public class DebugWindow : EditorWindow
{
    private static DebugWindow instance;
    private static bool isWindowOpen = false;

    private Vector2 scrollPosition;
    private DateTime lastUpdateTime;
    private float updateInterval = 1.0f;

    private GameManager gameManager;
    private DebugManager debugManager;
    private ConsoleManager consoleManager;
    private TurnManager turnManager;
    private StageManager stageManager;
    private LogManager logManager;
    private ProfileManager profileManager;
    private SelectedPlayerManager selectedPlayerManager;

    private GameSpeedOption selectedGameSpeed = GameSpeedOption.Normal;
    private DebugOptions selectedOption = DebugOptions.None;
    private VFX selectedVfx = VFX.None;

    static DebugWindow()
    {
        //Subscribe to the play mode state change event
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem("Window/Debug Window")]
    public static void ShowWindow()
    {
        instance = GetWindow<DebugWindow>("Debug Window");
        isWindowOpen = true;
    }

    public static void CloseWindow()
    {
        if (instance == null)
            return;

        instance.Close();
        instance = null;
        isWindowOpen = false;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
#if UNITY_EDITOR_WIN
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Close the window when entering Play Mode
            if (isWindowOpen)
                CloseWindow();

            // Wait for Game scene to load, then reopen the window
            EditorApplication.delayCall += WaitForGameScene;
        }
#endif
    }

#if UNITY_EDITOR_WIN
    private static void WaitForGameScene()
    {
        // EnqueueAttacks every frame if the Game scene is loaded
        EditorApplication.update += CheckSceneLoad;
    }

    private static void CheckSceneLoad()
    {
        if (!EditorApplication.isPlaying) return;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game")
        {
            //Debug.Log("[DebugWindow] Game scene detected, opening Debug Window.");
            ShowWindow();
            EditorApplication.update -= CheckSceneLoad; // Stop checking
        }
    }
#endif


    private void OnEnable()
    {
        DelayCall(() =>
        {
            Initialize();
        });
    }

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

    private void Initialize()
    {
        if (GameManager.instance == null)
            return;

        instance = this;
        isWindowOpen = true;
        lastUpdateTime = DateTime.Now;

        gameManager = GameManager.instance;
        debugManager = gameManager.debugManager;
        consoleManager = gameManager.consoleManager;
        turnManager = gameManager.turnManager;
        stageManager = gameManager.stageManager;
        logManager = gameManager.logManager;
        profileManager = gameManager.profileManager;
        selectedPlayerManager = gameManager.selectedPlayerManager;

        //Assign initial flags
        debugManager.showActorNameTag = false;
        debugManager.showActorFrame = false;
        debugManager.showTutorials = false;
        debugManager.isPlayerInvincible = false;
        debugManager.isEnemyInvincible = false;
        debugManager.isTimerInfinite = false;
        debugManager.isEnemyStunned = false;

        //Register update method
        EditorApplication.update += OnEditorUpdate;

    }

    private void OnDisable()
    {
        isWindowOpen = false;
        instance = null;

        //Unregister events
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.delayCall = null;
    }

    private void OnEditorUpdate()
    {

        if ((DateTime.Now - lastUpdateTime).TotalSeconds >= updateInterval)
        {
            lastUpdateTime = DateTime.Now;
            Repaint(); //Repaint the window
        }



    }

    private void OnGUI()
    {
        //EnqueueAttacks abort conditions
        if (!EditorApplication.isPlaying
            || gameManager == null
            || debugManager == null
            || consoleManager == null
            || turnManager == null
            || stageManager == null
            || logManager == null
            || profileManager == null
            || selectedPlayerManager == null)
            return;

        GUILayout.BeginVertical();

        RenderKeyboard();
        RenderScenes();
        RenderStats();
        RenderCheckboxes();
        RenderGameSpeedDropdown();
        RenderDebugOptionsDropdown();
        RenderVFXDropdown();
        RenderLevelControls();
        //RenderDataControls();
        RenderSpawnControls();
        RenderActorStats();
        RenderLog();

        GUILayout.EndVertical();
    }

    private void RenderKeyboard()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Keyboard");
        GUILayout.EndHorizontal();

        bool isClicked;

        // Row 1: Up Arrow
        GUILayout.BeginHorizontal();
        GUILayout.Space(38); // Add space to center "W"
        isClicked = GUILayout.Button("\u2191", GUILayout.Width(32), GUILayout.Height(32));
        if (isClicked)
            OnKeyUp();
        GUILayout.Space(38);
        GUILayout.EndHorizontal();

        // Row 2: Left, Down, Right
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

        GUILayout.Space(10);
        Repaint(); // Ensures UI updates
    }



    private void RenderScenes()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scenes");
        GUILayout.EndHorizontal();

        bool isClicked;

        GUILayout.BeginHorizontal();

        isClicked = GUILayout.Button("Splash Screen", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneManager.LoadScene(Scene.SplashScreen);

        isClicked = GUILayout.Button("Title Screen", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneManager.LoadScene(Scene.TitleScreen);

        isClicked = GUILayout.Button("Options Screen", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneManager.LoadScene(Scene.OptionsScreen);

        isClicked = GUILayout.Button("Game", GUILayout.Width(Screen.width * Constants.percent25));
        if (isClicked)
            SceneManager.LoadScene(Scene.Game);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private void RenderStats()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label($"FPS: {consoleManager.fpsMonitor.currentFps}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"Turn: {(turnManager.isPlayerTurn ? "Player" : "Opponent")}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"Phase: {turnManager.currentPhase}", GUILayout.Width(Screen.width * 0.25f));
        GUILayout.Label($"Runtime: {Time.time:F2}", GUILayout.Width(Screen.width * 0.25f));

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private void RenderCheckboxes()
    {
        bool onCheckChanged;

        GUILayout.BeginHorizontal();

        //Show Actor Name Tags checkbox
        onCheckChanged = EditorGUILayout.Toggle("Show Actor Name Tags?", debugManager.showActorNameTag, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.showActorNameTag != onCheckChanged)
        {
            debugManager.showActorNameTag = onCheckChanged;
            gameManager.actors.ForEach(x => x.render.SetNameTagEnabled(onCheckChanged));
        }

        //Show Actor Frames checkbox
        onCheckChanged = EditorGUILayout.Toggle("Show Actor Frames?", debugManager.showActorFrame, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.showActorFrame != onCheckChanged)
        {
            debugManager.showActorFrame = onCheckChanged;
            gameManager.actors.ForEach(x => x.render.SetFrameEnabled(onCheckChanged));
        }

        //Show Actor Frames checkbox
        onCheckChanged = EditorGUILayout.Toggle("Show Tutorials", debugManager.showTutorials, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.showTutorials != onCheckChanged)
        {
            debugManager.showTutorials = onCheckChanged;
            gameManager.tutorialPopup.gameObject.SetActive(debugManager.showTutorials);
        }

        //Are Players Invinciple? checkbox
        onCheckChanged = EditorGUILayout.Toggle("Are Players Invincible?", debugManager.isPlayerInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isPlayerInvincible != onCheckChanged)
            debugManager.isPlayerInvincible = onCheckChanged;

        //Are Enemies Invinciple? checkbox
        onCheckChanged = EditorGUILayout.Toggle("Are Enemies Invincible?", debugManager.isEnemyInvincible, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isEnemyInvincible != onCheckChanged)
            debugManager.isEnemyInvincible = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        //Matches Infinite Timer? checkbox
        onCheckChanged = EditorGUILayout.Toggle("Matches Timer Infinite?", debugManager.isTimerInfinite, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isTimerInfinite != onCheckChanged)
            debugManager.isTimerInfinite = onCheckChanged;

        //Matches Opponent Stunned? checkbox
        onCheckChanged = EditorGUILayout.Toggle("Matches Opponent Stunned?", debugManager.isEnemyStunned, GUILayout.Width(Screen.width * 0.25f));
        if (debugManager.isEnemyStunned != onCheckChanged)
            debugManager.isEnemyStunned = onCheckChanged;

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }


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

    private void RenderLevelControls()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reload", GUILayout.Width(Screen.width * Constants.percent33)))
            OnReloadStageClick();

        if (GUILayout.Button("< Previous", GUILayout.Width(Screen.width * Constants.percent33)))
            OnPreviousStageClick();

        if (GUILayout.Button("Next >", GUILayout.Width(Screen.width * Constants.percent33)))
            OnNextStageClick();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    //private void RenderDataControls()
    //{
    //   bool isClicked;

    //   GUILayout.BeginHorizontal();
    //   GUILayout.Label("Data", GUILayout.Width(Screen.width));
    //   GUILayout.EndHorizontal();
    //   GUILayout.BeginHorizontal();

    //   isClicked = GUILayout.Button("Erase Schema", GUILayout.Width(Screen.width * Constants.percent50));
    //   if (isClicked)
    //       OnEraseDatabaseClick();

    //   isClicked = GUILayout.Button("Erase Profiles", GUILayout.Width(Screen.width * Constants.percent50));

    //   if (isClicked)
    //       OnEraseProfilesClick();

    //   GUILayout.EndHorizontal();
    //   GUILayout.Space(10);
    //}



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


    private void RenderActorStats()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Actors", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        foreach (var x in gameManager.players.OrderBy(x => x.name))
        {
            GUILayout.BeginHorizontal();
            string stats = $"{x.name}, IsAlive? {x.isAlive}, IsActive? {x.isActive}";
            GUILayout.Label(stats, GUILayout.Width(Screen.width));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        foreach (var x in gameManager.enemies.OrderBy(x => x.name))
        {
            GUILayout.BeginHorizontal();
            string stats = $"{x.name}, IsAlive? {x.isAlive}, IsActive? {x.isActive}";
            GUILayout.Label(stats, GUILayout.Width(Screen.width));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
    }

    private void RenderLog()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Log", GUILayout.Width(Screen.width));
        GUILayout.EndHorizontal();

        //Background color setup
        var backgroundColor = new Color(0.5f, 0.15f, 0.15f);
        var style = new GUIStyle { richText = true, padding = new RectOffset(10, 10, 10, 10) };

        //Calculate the background area
        float logHeight = position.height - 170;
        Rect backgroundRect = new Rect(0, GUILayoutUtility.GetLastRect().yMax, Screen.width, logHeight);

        //Draw the background
        Color originalColor = GUI.color;
        GUI.color = backgroundColor;
        GUI.Box(backgroundRect, GUIContent.none); //Draw the background box
        GUI.color = originalColor;

        //Make the log scrollable
        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition,
            GUILayout.Height(logHeight),
            GUILayout.ExpandHeight(true));

        //Display the logs
        GUILayout.Label(logManager.text, style);

        GUILayout.EndScrollView();
        GUILayout.Space(10);
    }

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


    //Stage methods
    private void OnReloadStageClick() => stageManager.LoadStage();
    private void OnPreviousStageClick() => stageManager.Previous();
    private void OnNextStageClick() => stageManager.Next();

    //Keyboard methods
    private void OnKeyUp() => GameManager.instance.focusedActor?.Move(Vector2Int.down);
    private void OnKeyDown() => GameManager.instance.focusedActor?.Move(Vector2Int.up);
    private void OnKeyLeft() => GameManager.instance.focusedActor?.Move(Vector2Int.left);
    private void OnKeyRight() => GameManager.instance.focusedActor?.Move(Vector2Int.right);

 

}
