using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.GUI;
using Assets.Scripts.Libraries;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Game.Behaviors;
using Game.Instances;
using Game.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    // Device
    [HideInInspector] public string deviceType;

    // Settings
    [HideInInspector] public TextureResolution textureResolution = TextureResolution.NormalResolution;
    [HideInInspector] public TargetFrameRate targetFramerate = TargetFrameRate.Fps60;
    [HideInInspector] public VSyncCount vSyncCount = VSyncCount.VSync1;
    [HideInInspector] public float dragSensitivity = 0.05f;
    [HideInInspector] public float coinCountMultiplier = 0.05f;


    public float gameSpeed = 1.0f;
    public bool applyMovementTilt = false;

    // Selection behavior toggle for hero control during hero turns
    [SerializeField] public TurnSelectionMode turnSelectionMode = TurnSelectionMode.FreeSelect;

    //Debug
    public bool reloadThumbnailSettings = false;

    // Audio
    [HideInInspector] public AudioSource soundSource;
    [HideInInspector] public AudioSource musicSource;

    // Canvas
    [HideInInspector] public ActorCard card;
    [HideInInspector] public TutorialPopup tutorialPopup;
    [HideInInspector] public Vector2 viewport;
    [HideInInspector] public float tileSize;
    [HideInInspector] public Vector3 tileScale;
    [HideInInspector] public Canvas canvas3D;
    [HideInInspector] public WaveAnnouncement waveAnnouncement;
    [HideInInspector] public TargetModeOverlay targetModeOverlay;
    [HideInInspector] public TitleBarInstance titleBar;

    // Managers
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public CameraManager cameraManager;
    [HideInInspector] public StageManager stageManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public TurnManager turnManager;
    [HideInInspector] public SupportLineManager supportLineManager;
    [HideInInspector] public AttackLineManager attackLineManager;
    [HideInInspector] public CombatTextManager combatTextManager;
    [HideInInspector] public GhostManager ghostManager;
    [HideInInspector] public PortraitManager portraitManager;
    [HideInInspector] public ActorManager actorManager;
    [HideInInspector] public SelectionManager selectedHeroManager;
    [HideInInspector] public HeroManager heroManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public TileManager tileManager;
    [HideInInspector] public FootstepManager footstepManager;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public VisualEffectManager visualEffectManager;
    [HideInInspector] public CoinManager coinManager;
    [HideInInspector] public PauseMenu pauseMenu;
    [HideInInspector] public DebugManager debugManager;
    [HideInInspector] public ConsoleManager consoleManager;
    [HideInInspector] public LogManager logManager;
    [HideInInspector] public DottedLineManager dottedLineManager;
    [HideInInspector] public ProjectileManager projectileManager;
    [HideInInspector] public SequenceManager sequenceManager;
    [HideInInspector] public PincerAttackManager pincerAttackManager;
    [HideInInspector] public SortingManager sortingManager;
    [HideInInspector] public TargetLineManager targetLineManager;
    [HideInInspector] public AbilityButtonManager abilityButtonManager;
    [HideInInspector] public AbilityManager abilityManager;
    [HideInInspector] public SynergyLineManager synergyLineManager;
    [HideInInspector] public ManaPoolManager manaPoolManager;
    


    [HideInInspector] public Timeline timeline;
    [HideInInspector] public BackgroundInstance background;

    // Board
    [HideInInspector] public BoardOverlay boardOverlay;


    // Input
    [HideInInspector] public Vector3 touchPosition2D;
    [HideInInspector] public Vector3 touchPosition3D;
    [HideInInspector] public Vector3 touchOffset;
    [HideInInspector] public float cursorFocus;
    [HideInInspector] public float swapFocus;
    [HideInInspector] public float moveFocus;
    [HideInInspector] public float dragThreshold;
    [HideInInspector] public float bumpFocus;

    // Actors
    [HideInInspector] public List<ActorInstance> actors;
    [HideInInspector] public IEnumerable<ActorInstance> heroes => actors.Where(x => x.team == Team.Hero);
    [HideInInspector] public IEnumerable<ActorInstance> enemies => actors.Where(x => x.team == Team.Enemy);


    [HideInInspector] public ActorInstance preselectHero;
    [HideInInspector] public bool hasPreselectHero => preselectHero != null;


    [HideInInspector] public ActorInstance selectedActor;
    [HideInInspector] public bool hasSelectedActor => selectedActor != null;

    [HideInInspector] public ActorInstance movingHero;
    [HideInInspector] public bool hasMovingHeroHero => movingHero != null;

    [HideInInspector] public ActorInstance targetActor;
    [HideInInspector] public bool hasTargetActor => targetActor != null;

    // Instances
    [HideInInspector] public TileMap tileMap;
    [HideInInspector] public TimerBar timerBar;
    [HideInInspector] public RectTransform portraitsRect;
    [HideInInspector] public RectTransform timelineContainer;
    [HideInInspector] public RectTransform timelineViewport;
    [HideInInspector] public RectTransform timelineContent;
    [HideInInspector] public BoardInstance board;
    [HideInInspector] public List<TileInstance> tiles;
    [HideInInspector] public List<SupportLineInstance> supportLines;
    [HideInInspector] public List<AttackLineInstance> attackLines;

    // CoinManager
    [HideInInspector] public CoinCounter coinCounter;

    // Audio indices
    [HideInInspector] public const int SoundSourceIndex = 0;
    [HideInInspector] public const int MusicSourceIndex = 1;


    // Debug


    private void Awake()
    {
        if (!ProfileHelper.HasProfiles())
            return;

        var canvasRoot = GameObject.Find("Canvas");

        var go = Instantiate(PrefabLibrary.Get("PauseMenu"), canvasRoot.transform);
        go.name = "PauseMenu";
        pauseMenu = go.GetComponent<PauseMenu>();

        go = Instantiate(PrefabLibrary.Get("TutorialPopup"), canvasRoot.transform);
        go.name = "TutorialPopup";
        tutorialPopup = go.GetComponent<TutorialPopup>();

        // Apply settings
        Application.targetFrameRate = targetFramerate.ToInt();
        QualitySettings.vSyncCount = VSyncCount.VSync1.ToInt();

        float width97Percent = UnitConversionHelper.World.VisibleRect().width * 0.97f;
        tileSize = width97Percent / 6f;
        tileScale = new Vector3(tileSize, tileSize, 1f);
        tileMap = new TileMap();

        cursorFocus = tileSize * 0.5f;
        swapFocus = tileSize * 0.1666f;
        moveFocus = tileSize * 0.125f;
        bumpFocus = tileSize * 0.08f;
        dragThreshold = tileSize * 0.125f;

        ShakeIntensity.Initialize(tileSize);

        // Canvas
        card = GameObjectHelper.Game.Card.Instance;
        canvas3D = GameObjectHelper.Game.Canvas3D;
        timerBar = GameObjectHelper.Game.TimerBar.Instance;
        portraitsRect = GameObjectHelper.Game.Portraits;
        titleBar = GameObjectHelper.Game.TitleBar.Instance;

        // Timeline children
        timelineContainer = GameObject.Find(GameObjectHelper.Game.TimelineContainer).GetComponent<RectTransform>();
        timelineViewport = timelineContainer.Find("Viewport").GetComponent<RectTransform>();
        timelineContent = timelineViewport.Find("Content").GetComponent<RectTransform>();
        timeline = timelineContainer.GetComponent<Timeline>();

        coinCounter = GameObject.Find(GameObjectHelper.Game.CoinCounter).GetComponent<CoinCounter>();
        waveAnnouncement = GameObjectHelper.Game.WaveAnnouncement.Root.GetComponent<WaveAnnouncement>();
        background = GameObject.Find(GameObjectHelper.Game.Background.Root).GetComponent<BackgroundInstance>();

        // Board
        board = GameObjectHelper.Game.Board.Instance;
        boardOverlay = GameObjectHelper.Game.Board.BoardOverlay;
        targetModeOverlay = GameObjectHelper.Game.Board.TargetModeOverlay;

        var gameRoot = GameObject.Find("Game");

        // Audio
        soundSource = gameRoot.GetComponents<AudioSource>()[SoundSourceIndex];
        musicSource = gameRoot.GetComponents<AudioSource>()[MusicSourceIndex];

        // Managers
        cameraManager = gameRoot.GetComponent<CameraManager>();
        stageManager = gameRoot.GetComponent<StageManager>();
        boardManager = gameRoot.GetComponent<BoardManager>();
        turnManager = gameRoot.GetComponent<TurnManager>();
        inputManager = gameRoot.GetComponent<InputManager>();
        actorManager = gameRoot.GetComponent<ActorManager>();
        supportLineManager = gameRoot.GetComponent<SupportLineManager>();
        attackLineManager = gameRoot.GetComponent<AttackLineManager>();
        combatTextManager = gameRoot.GetComponent<CombatTextManager>();
        ghostManager = gameRoot.GetComponent<GhostManager>();
        portraitManager = gameRoot.GetComponent<PortraitManager>();
        selectedHeroManager = gameRoot.GetComponent<SelectionManager>();
        heroManager = gameRoot.GetComponent<HeroManager>();
        enemyManager = gameRoot.GetComponent<EnemyManager>();
        tileManager = gameRoot.GetComponent<TileManager>();
        footstepManager = gameRoot.GetComponent<FootstepManager>();
        audioManager = gameRoot.GetComponent<AudioManager>();
        debugManager = gameRoot.GetComponent<DebugManager>();
        consoleManager = gameRoot.GetComponent<ConsoleManager>();
        logManager = gameRoot.GetComponent<LogManager>();
        visualEffectManager = gameRoot.GetComponent<VisualEffectManager>();
        coinManager = gameRoot.GetComponent<CoinManager>();
        dottedLineManager = gameRoot.GetComponent<DottedLineManager>();
        projectileManager = gameRoot.GetComponent<ProjectileManager>();
        sequenceManager = gameRoot.GetComponent<SequenceManager>();
        pincerAttackManager = gameRoot.GetComponent<PincerAttackManager>();
        sortingManager = gameRoot.GetComponent<SortingManager>();
        targetLineManager = gameRoot.GetComponent<TargetLineManager>();
        abilityButtonManager = gameRoot.GetComponent<AbilityButtonManager>();
        abilityManager = gameRoot.GetComponent<AbilityManager>();
        synergyLineManager = gameRoot.GetComponent<SynergyLineManager>();
        manaPoolManager = gameRoot.GetComponent<ManaPoolManager>();


        // Platform-dependent compilation
#if UNITY_STANDALONE_WIN
        deviceType = "UNITY_STANDALONE_WIN";
#elif UNITY_STANDALONE_LINUX
        deviceType = "UNITY_STANDALONE_LINUX";
#elif UNITY_IPHONE
        deviceType = "UNITY_IPHONE";
#elif UNITY_STANDALONE_OSX
        deviceType = "UNITY_STANDALONE_OSX";
#elif UNITY_WEBPLAYER
        deviceType = "UNITY_WEBPLAYER";
#elif UNITY_WEBGL
        deviceType = "UNITY_WEBGL";
#else
        deviceType = "Unknown";
#endif
    }

    private void Start()
    {
        if (!ProfileHelper.HasProfiles())
            return;

        // Initialize UI/Managers that require instantiated prefabs
        if (pauseMenu != null) pauseMenu.Initialize();
        if (tutorialPopup != null) tutorialPopup.Initialize();

        // Ensure board reference is valid after scene loads/reloads
        if (board == null)
            board = GameObjectHelper.Game.Board.Instance;

        // Show in specific order
        if (board != null) board.Initialize();
        if (stageManager != null) stageManager.Initialize();
        if (targetModeOverlay != null) targetModeOverlay.Initialize();
        if (timeline != null) timeline.Initialize();
        if (timerBar != null) timerBar.Initialize();
        if (turnManager != null) turnManager.Initialize();

        GameReady.Confirm();
    }
}
