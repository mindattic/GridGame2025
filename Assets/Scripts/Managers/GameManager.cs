using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Canvas.Timeline;
using Assets.Scripts.GUI;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Instances;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
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
    
    //Debug
    public bool reloadThumbnailSettings = false;

    // Audio
    [HideInInspector] public AudioSource soundSource;
    [HideInInspector] public AudioSource musicSource;

    // Canvas
    [HideInInspector] public Card card;
    [HideInInspector] public TutorialPopup tutorialPopup;
    [HideInInspector] public Vector2 viewport;
    [HideInInspector] public float tileSize;
    [HideInInspector] public Vector3 tileScale;
    [HideInInspector] public Canvas canvas3D;
    [HideInInspector] public WaveAnnouncement waveAnnouncement;
    [HideInInspector] public TargetModeOverlay targetModeOverlay;

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
    [HideInInspector] public Portrait2DManager portrait2DManager;
    [HideInInspector] public Portrait3DManager portrait3DManager;
    [HideInInspector] public ActorManager actorManager;
    [HideInInspector] public SelectedHeroManager selectedHeroManager;
    [HideInInspector] public HeroManager heroManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public TileManager tileManager;
    [HideInInspector] public FootstepManager footstepManager;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public VfxManager vfxManager;
    [HideInInspector] public CoinManager coinManager;
    [HideInInspector] public PauseManager pauseManager;
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
    [HideInInspector] public SynergyLineManager synergyLineManager;
    [HideInInspector] public Timeline timeline;

    [HideInInspector] public BackgroundInstance background;

    // Board
    [HideInInspector] public BoardOverlay boardOverlay;
    [HideInInspector] public FocusIndicator focusIndicator;
    [HideInInspector] public TargetIndicator targetIndicator;

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

    [HideInInspector] public ActorInstance focusedActor;
    [HideInInspector] public bool hasFocusedActor => focusedActor != null;

    [HideInInspector] public ActorInstance selectedHero;
    [HideInInspector] public bool hasSelectedHero => selectedHero != null;

    [HideInInspector] public ActorInstance targetActor;
    [HideInInspector] public bool hasTargetActor => targetActor != null;

    // Instances
    [HideInInspector] public TileMap tileMap;
    [HideInInspector] public TimerBar2D timerBar2D;
    [HideInInspector] public RectTransform portraitsContainer;
    [HideInInspector] public RectTransform timelineRoot;
    [HideInInspector] public RectTransform timelineViewport;
    [HideInInspector] public RectTransform timelineContent;
    [HideInInspector] public Image timelineIndicator;
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

        //Apply settings
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
        tutorialPopup = GameObject.Find(GameObjectHelper.Game.TutorialPopup).GetComponent<TutorialPopup>();
        card = GameObject.Find(GameObjectHelper.Game.Card.Root).GetComponent<Card>();
        canvas3D = GameObject.Find(GameObjectHelper.Game.Canvas3D).GetComponent<Canvas>();
        timerBar2D = GameObject.Find(GameObjectHelper.Game.TimerBar2D.Root).GetComponent<TimerBar2D>();
        portraitsContainer = GameObject.Find(GameObjectHelper.Game.Portraits).GetComponent<RectTransform>();
        timelineRoot = GameObject.Find(GameObjectHelper.Game.TimelineRoot).GetComponent<RectTransform>();

        // Timeline children
        timelineViewport = timelineRoot.Find("Viewport").GetComponent<RectTransform>();
        timelineContent = timelineViewport.Find("Content").GetComponent<RectTransform>();
        timelineIndicator = timelineViewport.Find("Indicator").GetComponent<Image>();
        timeline = timelineRoot.GetComponent<Timeline>();

        coinCounter = GameObject.Find(GameObjectHelper.Game.CoinCounter).GetComponent<CoinCounter>();
        waveAnnouncement = GameObject.Find(GameObjectHelper.Game.WaveAnnouncement).GetComponent<WaveAnnouncement>();
        targetModeOverlay = GameObject.Find(GameObjectHelper.Game.TargetModeOverlay).GetComponent<TargetModeOverlay>();
        background = GameObject.Find(GameObjectHelper.Game.Background.Root).GetComponent<BackgroundInstance>();

        // Board
        board = GameObject.Find(GameObjectHelper.Game.Board.Root).GetComponent<BoardInstance>();
        boardOverlay = GameObject.Find(GameObjectHelper.Game.Board.BoardOverlay).GetComponent<BoardOverlay>();
        focusIndicator = GameObject.Find(GameObjectHelper.Game.Board.FocusIndicator).GetComponent<FocusIndicator>();
        targetIndicator = GameObject.Find(GameObjectHelper.Game.Board.TargetIndicator).GetComponent<TargetIndicator>();

        var game = GameObject.Find("Game");

        // Audio
        soundSource = game.GetComponents<AudioSource>()[SoundSourceIndex];
        musicSource = game.GetComponents<AudioSource>()[MusicSourceIndex];

        // Managers
        cameraManager = game.GetComponent<CameraManager>();
        stageManager = game.GetComponent<StageManager>();
        boardManager = game.GetComponent<BoardManager>();
        turnManager = game.GetComponent<TurnManager>();
        inputManager = game.GetComponent<InputManager>();
        actorManager = game.GetComponent<ActorManager>();
        supportLineManager = game.GetComponent<SupportLineManager>();
        attackLineManager = game.GetComponent<AttackLineManager>();
        combatTextManager = game.GetComponent<CombatTextManager>();
        ghostManager = game.GetComponent<GhostManager>();
        portrait2DManager = game.GetComponent<Portrait2DManager>();
        portrait3DManager = game.GetComponent<Portrait3DManager>();
        selectedHeroManager = game.GetComponent<SelectedHeroManager>();
        heroManager = game.GetComponent<HeroManager>();
        enemyManager = game.GetComponent<EnemyManager>();
        tileManager = game.GetComponent<TileManager>();
        footstepManager = game.GetComponent<FootstepManager>();
        audioManager = game.GetComponent<AudioManager>();
        debugManager = game.GetComponent<DebugManager>();
        consoleManager = game.GetComponent<ConsoleManager>();
        logManager = game.GetComponent<LogManager>();
        vfxManager = game.GetComponent<VfxManager>();
        coinManager = game.GetComponent<CoinManager>();
        pauseManager = game.GetComponent<PauseManager>();
        dottedLineManager = game.GetComponent<DottedLineManager>();
        projectileManager = game.GetComponent<ProjectileManager>();
        sequenceManager = game.GetComponent<SequenceManager>();
        pincerAttackManager = game.GetComponent<PincerAttackManager>();
        sortingManager = game.GetComponent<SortingManager>();
        targetLineManager = game.GetComponent<TargetLineManager>();
        abilityButtonManager = game.GetComponent<AbilityButtonManager>();
        synergyLineManager = game.GetComponent<SynergyLineManager>();
      
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

        // Show in specific order
        board.Initialize();
        stageManager.Initialize();
        focusIndicator.Initialize();
        targetIndicator.Initialize();
        targetModeOverlay.Initialize();
        timeline.Initialize();
        timerBar2D.Initialize();
        turnManager.Initialize();
    }
}
