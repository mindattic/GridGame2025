using Assets.Scripts.GUI;
using Game.Behaviors;
using Game.Manager;
using Game.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public string deviceType;
    [HideInInspector] public int targetFramerate = 60;
    [HideInInspector] public int vSyncCount = 1;

    public float gameSpeed
    {
        get => Time.timeScale;
        set => Time.timeScale = value;
    }

    public float previousGameSpeed;

    //Audio
    [HideInInspector] public AudioSource soundSource;
    [HideInInspector] public AudioSource musicSource;

    //GUI
    [HideInInspector] public Card cardManager;
    [HideInInspector] public TutorialPopup tutorialPopup;

    //Managers
    //[HideInInspector] public DatabaseManager databaseManager;
    [HideInInspector] public DataManager dataManager;
    [HideInInspector] public ResourceManager resourceManager;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public CameraManager cameraManager;
    [HideInInspector] public ProfileManager profileManager;
    [HideInInspector] public StageManager stageManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public TurnManager turnManager;
    [HideInInspector] public SupportLineManager supportLineManager;
    [HideInInspector] public AttackLineManager attackLineManager;
    [HideInInspector] public DamageTextManager damageTextManager;
    [HideInInspector] public GhostManager ghostManager;
    [HideInInspector] public PortraitManager portraitManager; 
    [HideInInspector] public ActorManager actorManager;
    [HideInInspector] public SelectedPlayerManager selectedPlayerManager;
    [HideInInspector] public PlayerManager playerManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public TileManager tileManager;
    [HideInInspector] public FootstepManager footstepManager;
    [HideInInspector] public TooltipManager tooltipManager;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public VFXManager vfxManager;
    [HideInInspector] public TrailManager trailManager;
    [HideInInspector] public CoinManager coinManager;
    [HideInInspector] public PauseManager pauseManager;
    [HideInInspector] public DebugManager debugManager;
    [HideInInspector] public ConsoleManager consoleManager;
    [HideInInspector] public LogManager logManager;
    [HideInInspector] public DottedLineManager dottedLineManager;
    [HideInInspector] public SpellManager spellManager;

    //Board
    [HideInInspector] public BoardOverlay boardOverlay;

    //Canvas
    [HideInInspector] public CanvasOverlay canvasOverlay;


    [HideInInspector] public Vector2 viewport;
    [HideInInspector] public float tileSize;
    [HideInInspector] public Vector3 tileScale;
    [HideInInspector] public float cardPortraitSize;
    [HideInInspector] public Canvas canvas2D;
    [HideInInspector] public Canvas canvas3D;

    //Mouse
    [HideInInspector] public Vector3 mousePosition2D;
    [HideInInspector] public Vector3 mousePosition3D;
    [HideInInspector] public Vector3 mouseOffset;
    [HideInInspector] public float cursorSpeed;
    [HideInInspector] public float swapSpeed;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float snapDistance;
    [HideInInspector] public float bumpSpeed;

    //Actor selection
    [HideInInspector] public ActorInstance focusedActor;
    [HideInInspector] public ActorInstance selectedPlayer;
    [HideInInspector] public ActorInstance previousSelectedPlayer;


    [HideInInspector] public Fade fade;


    public bool hasSelectedPlayer => selectedPlayer != null;
    public UnityEvent<Vector2Int> onSelectedPlayerLocationChanged;


    //Instances
    [HideInInspector] public TimerBarInstance timerBar;
    [HideInInspector] public List<ActorInstance> actors;
    [HideInInspector] public BoardInstance board;
    [HideInInspector] public List<TileInstance> tiles;
    [HideInInspector] public List<SupportLineInstance> lines;
    [HideInInspector] public CoinBarInstance coinBar;

    [HideInInspector] public IQueryable<ActorInstance> players => actors.Where(x => x.team.Equals(Team.Player)).AsQueryable();
    [HideInInspector] public IQueryable<ActorInstance> enemies => actors.Where(x => x.team.Equals(Team.Enemy)).AsQueryable();




    [HideInInspector] public int totalCoins;


    private void Awake()
    {
        Application.targetFrameRate = targetFramerate;
        QualitySettings.vSyncCount = vSyncCount;

        onSelectedPlayerLocationChanged = new UnityEvent<Vector2Int>();
        previousGameSpeed = Time.timeScale;

        //DEBUG: Need to add buffer so tile doesn't align to left-most and right-most edge,
        //however this causes actors to not align properly after moving for some reason
        var oneSixth = ScreenHelper.ScreenInWorldUnits.Width / 6;
        //var tenPercentOfOneSixth = oneSixth * 0.1f;
        //var fivePercentOfOneSixth = oneSixth * 0.05f;

        tileSize = oneSixth;
        tileScale = new Vector3(tileSize, tileSize, 1f);

        cardPortraitSize = ScreenHelper.ScreenInPixels.Width / 2;

        cursorSpeed = tileSize * 0.5f;
        swapSpeed = tileSize * 0.1666f;
        moveSpeed = tileSize * 0.125f;
        bumpSpeed = tileSize * 0.08f;
        snapDistance = tileSize * 0.125f;
        ShakeIntensity.Initialize(tileSize);

        totalCoins = 0;

        //Resources
        resourceManager = GameObject.Find(Constants.Resources).GetComponent<ResourceManager>() ?? throw new UnityException("ResourceManager is null");

        //GUI
        tutorialPopup = GameObject.Find(Constants.TutorialPopup).GetComponent<TutorialPopup>() ?? throw new UnityException("TutorialPopup is null");
        cardManager = GameObject.Find(Constants.Card).GetComponent<Card>() ?? throw new UnityException("CardManager is null");
        fade = GameObject.Find(Constants.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");

        board = GameObject.Find(Constants.Board).GetComponent<BoardInstance>() ?? throw new UnityException("BoardInstance is null");
        canvas2D = GameObject.Find(Constants.Canvas2D).GetComponent<Canvas>() ?? throw new UnityException("Canvas2D is null");
        canvas3D = GameObject.Find(Constants.Canvas3D).GetComponent<Canvas>() ?? throw new UnityException("Canvas3D is null");
        timerBar = GameObject.Find(Constants.TimerBar).GetComponent<TimerBarInstance>() ?? throw new UnityException("TimerBarInstance is null");
        coinBar = GameObject.Find(Constants.CoinBar).GetComponent<CoinBarInstance>() ?? throw new UnityException("CoinBarInstance is null");

        var game = GameObject.Find(Constants.Game);

        //Audio
        soundSource = game.GetComponents<AudioSource>()[Constants.SoundSourceIndex] ?? throw new UnityException("SoundSource is null");
        musicSource = game.GetComponents<AudioSource>()[Constants.MusicSourceIndex] ?? throw new UnityException("MusicSource is null");

        //Managers
        //databaseManager = game.GetComponent<DatabaseManager>() ?? throw new UnityException("DatabaseManager is null");
        dataManager = game.GetComponent<DataManager>() ?? throw new UnityException("DataManager is null");
        cameraManager = game.GetComponent<CameraManager>() ?? throw new UnityException("CameraManager is null");
        profileManager = game.GetComponent<ProfileManager>() ?? throw new UnityException("ProfileManager is null");
        stageManager = game.GetComponent<StageManager>() ?? throw new UnityException("StageManager is null");
        boardManager = game.GetComponent<BoardManager>() ?? throw new UnityException("BoardManager is null");
        turnManager = game.GetComponent<TurnManager>() ?? throw new UnityException("TurnManager is null");
        inputManager = game.GetComponent<InputManager>() ?? throw new UnityException("InputManager is null");
        actorManager = game.GetComponent<ActorManager>() ?? throw new UnityException("ActorManager is null");
        supportLineManager = game.GetComponent<SupportLineManager>() ?? throw new UnityException("SupportLineManager is null");
        attackLineManager = game.GetComponent<AttackLineManager>() ?? throw new UnityException("AttackLineManager is null");
        damageTextManager = game.GetComponent<DamageTextManager>() ?? throw new UnityException("DamageTextManager is null");
        ghostManager = game.GetComponent<GhostManager>() ?? throw new UnityException("GhostManager is null");
        portraitManager = game.GetComponent<PortraitManager>() ?? throw new UnityException("PortraitManager is null");
        actorManager = game.GetComponent<ActorManager>() ?? throw new UnityException("ActorManager is null");
        selectedPlayerManager = game.GetComponent<SelectedPlayerManager>() ?? throw new UnityException("SelectedPlayerManager is null");
        playerManager = game.GetComponent<PlayerManager>() ?? throw new UnityException("PlayerManager is null");
        enemyManager = game.GetComponent<EnemyManager>() ?? throw new UnityException("EnemyManager is null");
        tileManager = game.GetComponent<TileManager>() ?? throw new UnityException("TileManager is null");
        footstepManager = game.GetComponent<FootstepManager>() ?? throw new UnityException("FootstepManager is null");
        tooltipManager = game.GetComponent<TooltipManager>() ?? throw new UnityException("TooltipManager is null");
        audioManager = game.GetComponent<AudioManager>() ?? throw new UnityException("AudioManager is null");
        debugManager = game.GetComponent<DebugManager>() ?? throw new UnityException("DebugManager is null");
        consoleManager = game.GetComponent<ConsoleManager>() ?? throw new UnityException("ConsoleManager is null");
        logManager = game.GetComponent<LogManager>() ?? throw new UnityException("LogManager is null");
        vfxManager = game.GetComponent<VFXManager>() ?? throw new UnityException("VFXManager is null");
        trailManager = game.GetComponent<TrailManager>() ?? throw new UnityException("TrailManager is null");
        coinManager = game.GetComponent<CoinManager>() ?? throw new UnityException("CoinManager is null");
        pauseManager = game.GetComponent<PauseManager>() ?? throw new UnityException("PauseManager is null");
        dottedLineManager = game.GetComponent<DottedLineManager>() ?? throw new UnityException("DottedLineManager is null");
        spellManager = game.GetComponent<SpellManager>() ?? throw new UnityException("SpellManager is null");


        //Overlay components
        boardOverlay = GameObject.Find(Constants.BoardOverlay).GetComponent<BoardOverlay>() ?? throw new UnityException("BoardOverlay is null");
        canvasOverlay = GameObject.Find(Constants.CanvasOverlay).GetComponent<CanvasOverlay>() ?? throw new UnityException("CanvasOverlay is null");

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
        Debug.Log($"Running on {deviceType}");

#if UNITY_EDITOR
        Debug.Log($"Emulated on UNITY_EDITOR");
#endif

        #endregion
    }

    //Method which is automatically called before the first frame update  
    void Start()
    {
        //Initialize in specific order:
        dataManager.Initialize();       //01
        resourceManager.Initialize();   //02
        profileManager.Initialize();    //03
        board.Initialize();             //04
        stageManager.Initialize();      //05
    }

}
