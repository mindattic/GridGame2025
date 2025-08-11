using Assets.Scripts.GUI;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Helpers
{
    public static class GameHelper
    {

        //public static Canvas Canvas => CanvasHelper.Canvas;
        //public static RectTransform CanvasRect => CanvasHelper.CanvasRect;

        public static bool ReloadThumbnailSettings
        {
            get => GameManager.instance.reloadThumbnailSettings;
            set => GameManager.instance.reloadThumbnailSettings = value;
        }

        public static float GameSpeed
        {
            get => GameManager.instance.gameSpeed;
            set => GameManager.instance.gameSpeed = value;
        }

        public static bool ApplyMovementTilt
        {
            get => GameManager.instance.applyMovementTilt;
            set => GameManager.instance.applyMovementTilt = value;
        }

        public static TextureResolution TextureResolution
        {
            get => GameManager.instance.textureResolution;
            set => GameManager.instance.textureResolution = value;
        }

        public static AudioSource SoundSource => GameManager.instance.soundSource;

        // Component properties
        public static InputManager InputManager => GameManager.instance.inputManager;
        public static CameraManager CameraManager => GameManager.instance.cameraManager;
        public static StageManager StageManager => GameManager.instance.stageManager;
        public static BoardManager BoardManager => GameManager.instance.boardManager;
        public static TurnManager TurnManager => GameManager.instance.turnManager;

        public static SupportLineManager SupportLineManager => GameManager.instance.supportLineManager;
        public static AttackLineManager AttackLineManager => GameManager.instance.attackLineManager;
        public static CombatTextManager CombatTextManager => GameManager.instance.combatTextManager;
        public static GhostManager GhostManager => GameManager.instance.ghostManager;

        public static Portrait2DManager Portrait2DManager => GameManager.instance.portrait2DManager;
        public static Portrait3DManager Portrait3DManager => GameManager.instance.portrait3DManager;
        // public static ActorManager ActorManager => GameManager.actorManager;
        // public static SelectedHeroManager SelectedHeroManager => GameManager.selectedHeroManager;
        // public static HeroManager HeroManager => GameManager.heroManager;
        // public static EnemyManager EnemyManager => GameManager.enemyManager;
        public static TileManager TileManager => GameManager.instance.tileManager;
        public static FootstepManager FootstepManager => GameManager.instance.footstepManager;
        public static AudioManager AudioManager => GameManager.instance.audioManager;
        public static VfxManager VfxManager => GameManager.instance.vfxManager;
        public static TrailManager TrailManager => GameManager.instance.trailManager;
        public static CoinManager CoinManager => GameManager.instance.coinManager;
        public static PauseManager PauseManager => GameManager.instance.pauseManager;
        public static DebugManager DebugManager => GameManager.instance.debugManager;
        public static ConsoleManager ConsoleManager => GameManager.instance.consoleManager;
        public static LogManager LogManager => GameManager.instance.logManager;
        public static ActorManager ActorManager => GameManager.instance.actorManager;

        public static SelectedHeroManager SelectedHeroManager => GameManager.instance.selectedHeroManager;
        public static DottedLineManager DottedLineManager => GameManager.instance.dottedLineManager;
        public static ProjectileManager ProjectileManager => GameManager.instance.projectileManager;
        public static SequenceManager SequenceManager => GameManager.instance.sequenceManager;
        public static PincerAttackManager PincerAttackManager => GameManager.instance.pincerAttackManager;
        public static SortingManager SortingManager => GameManager.instance.sortingManager;
        public static TargetLineManager TargetLineManager => GameManager.instance.targetLineManager;
        public static AbilityButtonManager AbilityButtonManager => GameManager.instance.abilityButtonManager;


        // Board visuals and overlays
        public static BackgroundInstance Background => GameManager.instance.background;
        public static BoardOverlay BoardOverlay => GameManager.instance.boardOverlay;
        public static FocusIndicator FocusIndicator => GameManager.instance.focusIndicator;
        public static TargetIndicator TargetIndicator => GameManager.instance.targetIndicator;

        // Canvas and UI
        public static PauseOverlay PauseOverlay => GameManager.instance.pauseOverlay;
        public static Vector2 Viewport => GameManager.instance.viewport;
        public static float TileSize => GameManager.instance.tileSize;
        public static Vector3 TileScale => GameManager.instance.tileScale;
        public static Canvas Canvas3D => GameManager.instance.canvas3D;
        public static WaveAnnouncement WaveAnnouncement => GameManager.instance.waveAnnouncement;
        public static TargetModeOverlay TargetModeOverlay => GameManager.instance.targetModeOverlay;
        public static Card Card => GameManager.instance.card;
        public static TutorialPopup TutorialPopup => GameManager.instance.tutorialPopup;


        // Mouse-related fields
        public static Vector3 TouchPosition2D => GameManager.instance.touchPosition2D;
        public static Vector3 TouchPosition3D => GameManager.instance.touchPosition3D;


        public static Vector3 TouchOffset
        {
            get => GameManager.instance.touchOffset;
            set => GameManager.instance.touchOffset = value;
        }


        public static Vector3 TouchPosition => TouchPosition3D + TouchOffset;

        public static float CursorFocus => GameManager.instance.cursorFocus;
        public static float SwapFocus => GameManager.instance.swapFocus;
        public static float MoveFocus => GameManager.instance.moveFocus;
        public static float SnapThreshold => GameManager.instance.actorManager.snapTheshold;

        public static float DragThreshold => GameManager.instance.dragThreshold;
        public static float BumpFocus => GameManager.instance.bumpFocus;

        // Actor references
        public static class Actors
        {
            public static List<ActorInstance> All
            {
                get => GameManager.instance.actors;
                set => GameManager.instance.actors = value;
            }

            public static IEnumerable<ActorInstance> Heroes => GameManager.instance.heroes;

            public static IEnumerable<ActorInstance> Enemies => GameManager.instance.enemies;

            public static ActorInstance FocusedActor
            {
                get => GameManager.instance.focusedActor;
                set => GameManager.instance.focusedActor = value;
            }

            public static bool HasFocusedActor => GameManager.instance.hasFocusedActor;

            public static ActorInstance SelectedHero
            {
                get => GameManager.instance.selectedHero;
                set => GameManager.instance.selectedHero = value;
            }

            public static bool HasSelectedHero => GameManager.instance.hasSelectedHero;

            public static ActorInstance TargetActor
            {
                get => GameManager.instance.targetActor;
                set => GameManager.instance.targetActor = value;
            }
            public static bool HasTargetActor => GameManager.instance.hasTargetActor;
        }

        // World instances
        public static TileMap TileMap => GameManager.instance.tileMap;
        public static TimerBar2D TimerBar2D => GameManager.instance.timerBar2D;
        public static BoardInstance Board => GameManager.instance.board;

        public static List<TileInstance> Tiles => GameManager.instance.tiles;

        //public static TimerBar3D TimerBar3D => GameManager.instance.timerBar3D;
        public static RectTransform PortraitsContainer => GameManager.instance.portraitsContainer;


        // CoinManager
        public static CoinCounter CoinCounter => GameManager.instance.coinCounter;
        public static int TotalCoins
        {
            get => GameManager.instance.totalCoins;
            set => GameManager.instance.totalCoins = value;
        }
    }

}
