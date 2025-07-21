using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Instances;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using g = GameManagerHelper;

public class GameManager : Singleton<GameManager>
{
    //Device
    [HideInInspector] public string deviceType;
    [HideInInspector] public int targetFramerate = 60;  //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-targetFrameRate.html
    [HideInInspector] public int vSyncCount = 2;        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/QualitySettings-vSyncCount.html

    //AudioManager
    [HideInInspector] public AudioSource soundSource;
    [HideInInspector] public AudioSource musicSource;

    //GUI
    [HideInInspector] public Card card;
    [HideInInspector] public TutorialPopup tutorialPopup;

    //Managers
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public CameraManager cameraManager;
    [HideInInspector] public StageManager stageManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public TurnManager turnManager;
    [HideInInspector] public SupportLineManager supportLineManager;
    [HideInInspector] public AttackLineManager attackLineManager;
    [HideInInspector] public DamageTextManager damageTextManager;
    [HideInInspector] public GhostManager ghostManager;
    [HideInInspector] public PortraitManager portraitManager;
    [HideInInspector] public ActorManager actorManager;
    [HideInInspector] public SelectedHeroManager selectedHeroManager;
    [HideInInspector] public HeroManager heroManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public TileManager tileManager;
    [HideInInspector] public FootstepManager footstepManager;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public VFXManager vfxManager;
    [HideInInspector] public TrailManager trailManager;
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

    [HideInInspector] public BackgroundInstance background;

    //BoardManager
    [HideInInspector] public BoardOverlay boardOverlay;
    [HideInInspector] public FocusIndicator focusIndicator;
    [HideInInspector] public TargetIndicator targetIndicator;

    //Canvas
    [HideInInspector] public CanvasOverlay canvasOverlay;
    [HideInInspector] public Vector2 viewport;
    [HideInInspector] public float tileSize;
    [HideInInspector] public Vector3 tileScale;
    [HideInInspector] public Canvas canvas2D;
    [HideInInspector] public Canvas canvas3D;
    [HideInInspector] public WaveAnnouncement waveAnnouncement;
    [HideInInspector] public TargetModeOverlay targetModeOverlay;


    //Mouse
    [HideInInspector] public Vector3 touchPosition2D;
    [HideInInspector] public Vector3 touchPosition3D;
    [HideInInspector] public Vector3 touchOffset;
    [HideInInspector] public float cursorFocus;
    [HideInInspector] public float swapFocus;
    [HideInInspector] public float moveFocus;
    //[HideInInspector] public float snapThreshold;
    [HideInInspector] public float dragThreshold;
    [HideInInspector] public float bumpFocus;

    //Actors
    [HideInInspector] public List<ActorInstance> actors;
    [HideInInspector] public IEnumerable<ActorInstance> heroes => actors.Where(x => x.team.Equals(Team.Hero));
    [HideInInspector] public IEnumerable<ActorInstance> enemies => actors.Where(x => x.team.Equals(Team.Enemy));
    [HideInInspector] public ActorInstance focusedActor;
    [HideInInspector] public bool hasFocusedActor => focusedActor != null;
    [HideInInspector] public ActorInstance selectedHero;
    [HideInInspector] public bool hasSelectedPlayer => selectedHero != null;

    [HideInInspector] public ActorInstance targetActor;
    [HideInInspector] public bool hasTargetActor => targetActor != null;

    //Instances
    [HideInInspector] public FadeInstance fade;
    [HideInInspector] public TileMap tileMap;
    [HideInInspector] public TimerBar timerBar;
    [HideInInspector] public BoardInstance board;
    [HideInInspector] public List<TileInstance> tiles;
    [HideInInspector] public List<SupportLineInstance> supportLines;
    [HideInInspector] public List<AttackLineInstance> attackLines;

    //CoinManager
    [HideInInspector] public CoinBar coinBar;
    [HideInInspector] public int totalCoins;

    //Properties
    public float gameFocus { get => Time.timeScale; set => Time.timeScale = value; }
    public float previousGameFocus;

    //Debug Window
    public bool reloadThumbnailSettings = false;

    private void Awake()
    {
        if (!ProfileRepo.HasProfiles())
            return;

        Application.targetFrameRate = targetFramerate;
        QualitySettings.vSyncCount = vSyncCount;

        previousGameFocus = Time.timeScale;

        //DEBUG: Need to add buffer so tile doesn't align to left-most and right-most edge,
        //however this causes actors to not align properly after moving for some reason
        var oneSixth = ScreenHelper.ScreenInWorldUnits.Width / 6;
        //var tenPercentOfOneSixth = oneSixth * 0.1f;
        //var fivePercentOfOneSixth = oneSixth * 0.05f;

        tileSize = oneSixth;
        tileScale = new Vector3(tileSize, tileSize, 1f);
        tileMap = new TileMap();

        cursorFocus = tileSize * 0.5f;
        swapFocus = tileSize * 0.1666f;
        moveFocus = tileSize * 0.125f;
        bumpFocus = tileSize * 0.08f;

        dragThreshold = tileSize * 0.125f;
        ShakeIntensity.Initialize(tileSize);

        totalCoins = 0;

        //Canvas2D
        tutorialPopup = GameObject.Find(GameObjectHelper.Game.TutorialPopup).GetComponent<TutorialPopup>();
        card = GameObject.Find(GameObjectHelper.Game.Card.Root).GetComponent<Card>();
        fade = GameObject.Find(GameObjectHelper.Game.Fade).GetComponent<FadeInstance>();
        canvas2D = GameObject.Find(GameObjectHelper.Game.Canvas2D).GetComponent<Canvas>();
        canvas3D = GameObject.Find(GameObjectHelper.Game.Canvas3D).GetComponent<Canvas>();
        timerBar = GameObject.Find(GameObjectHelper.Game.TimerBar).GetComponent<TimerBar>();
        coinBar = GameObject.Find(GameObjectHelper.Game.CoinBar).GetComponent<CoinBar>();
        waveAnnouncement = GameObject.Find(GameObjectHelper.Game.WaveAnnouncement).GetComponent<WaveAnnouncement>();
        canvasOverlay = GameObject.Find(GameObjectHelper.Game.CanvasOverlay).GetComponent<CanvasOverlay>();
        targetModeOverlay = GameObject.Find(GameObjectHelper.Game.TargetModeOverlay).GetComponent<TargetModeOverlay>();

        background = GameObject.Find(GameObjectHelper.Game.Background.Root).GetComponent<BackgroundInstance>();

        //BoardManager
        board = GameObject.Find(GameObjectHelper.Game.Board.Root).GetComponent<BoardInstance>();
        boardOverlay = GameObject.Find(GameObjectHelper.Game.Board.BoardOverlay).GetComponent<BoardOverlay>();
        focusIndicator = GameObject.Find(GameObjectHelper.Game.Board.FocusIndicator).GetComponent<FocusIndicator>();
        targetIndicator = GameObject.Find(GameObjectHelper.Game.Board.TargetIndicator).GetComponent<TargetIndicator>();

        var game = GameObject.Find("Game"); // No helper constant provided for the root "Game" object

        //AudioManager
        soundSource = game.GetComponents<AudioSource>()[Constants.SoundSourceIndex];
        musicSource = game.GetComponents<AudioSource>()[Constants.MusicSourceIndex];

        //Managers
        cameraManager = game.GetComponent<CameraManager>();
        stageManager = game.GetComponent<StageManager>();
        boardManager = game.GetComponent<BoardManager>();
        turnManager = game.GetComponent<TurnManager>();
        inputManager = game.GetComponent<InputManager>();
        actorManager = game.GetComponent<ActorManager>();
        supportLineManager = game.GetComponent<SupportLineManager>();
        attackLineManager = game.GetComponent<AttackLineManager>();
        damageTextManager = game.GetComponent<DamageTextManager>();
        ghostManager = game.GetComponent<GhostManager>();
        portraitManager = game.GetComponent<PortraitManager>();
        selectedHeroManager = game.GetComponent<SelectedHeroManager>();
        heroManager = game.GetComponent<HeroManager>();
        enemyManager = game.GetComponent<EnemyManager>();
        tileManager = game.GetComponent<TileManager>();
        footstepManager = game.GetComponent<FootstepManager>();
        audioManager = game.GetComponent<AudioManager>();
        debugManager = game.GetComponent<DebugManager>();
        consoleManager = game.GetComponent<ConsoleManager>();
        logManager = game.GetComponent<LogManager>();
        vfxManager = game.GetComponent<VFXManager>();
        trailManager = game.GetComponent<TrailManager>();
        coinManager = game.GetComponent<CoinManager>();
        pauseManager = game.GetComponent<PauseManager>();
        dottedLineManager = game.GetComponent<DottedLineManager>();
        projectileManager = game.GetComponent<ProjectileManager>();
        sequenceManager = game.GetComponent<SequenceManager>();
        pincerAttackManager = game.GetComponent<PincerAttackManager>();
        sortingManager = game.GetComponent<SortingManager>();
        targetLineManager = game.GetComponent<TargetLineManager>();
        abilityButtonManager = game.GetComponent<AbilityButtonManager>();

        #region Platform Dependent Compilation

        //https://docs.unity3d.com/520/Documentation/Manual/PlatformDependentCompilation.html
#if UNITY_STANDALONE_WIN
        deviceType = "UNITY_STANDALONE_WIN";
#elif UNITY_STANDALONE_LINUX
         deviceType = "UNITY_STANDALONE_LINUX";
#elif UNITY_IPHONE
               deviceType = "UNITY_IPHONE";
#elif UNITY_STANDALONE_OSX
           deviceType = "UNITY_STANDALONE_OSX"
#elif UNITY_WEBPLAYER
         deviceType = "UNITY_WEBPLAYER";
#elif UNITY_WEBGL
         deviceType = "UNITY_WEBGL";
#else
        deviceType = "Unknown";
#endif
        //Debug.Log($"Running on `{deviceType}`");

        //#if UNITY_EDITOR
        //        Debug.Log($"Emulated on UNITY_EDITOR");
        //#endif

        #endregion
    }

    //Method which is automatically called before the first frame update  
    void Start()
    {
        // By now, profiles are guaranteed to be loaded.
        if (!ProfileRepo.HasProfiles())
            return;


        //Assign in specific order:
        board.Initialize();             //01
        stageManager.Initialize();      //02
        focusIndicator.Initialize();    //03
        targetIndicator.Initialize();   //04
        targetModeOverlay.Initialize(); //05
        timerBar.Initialize();          //06
    }

}
