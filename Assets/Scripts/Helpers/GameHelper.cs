using Assets.Scripts.GUI;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Helpers
{
    public static class GameHelper
    {
        private static GameManager gm => GameManager.instance;

        public static float DragSensitiviry
        {
            get => GameManager.instance.dragSensitivity;
            set => GameManager.instance.dragSensitivity = value;
        }

        public static float CoinCountMulitiplier
        {
            get => GameManager.instance.coinCountMultiplier;
            set => GameManager.instance.coinCountMultiplier = value;
        }

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

        public static float DragSensitivity
        {
            get => GameManager.instance.dragSensitivity;
            set => GameManager.instance.dragSensitivity = value;
        }

        // New: selection mode toggle
        public static TurnSelectionMode TurnSelectionMode
        {
            get => gm != null ? gm.turnSelectionMode : TurnSelectionMode.FreeSelect;
            set { if (gm != null) gm.turnSelectionMode = value; }
        }

        public static AudioSource SoundSource => gm != null ? gm.soundSource : null;
        public static AudioSource MusicSource => gm != null ? gm.musicSource : null;

        // Component properties
        public static InputManager InputManager => gm != null ? gm.inputManager : null;
        public static CameraManager CameraManager => gm != null ? gm.cameraManager : null;
        public static StageManager StageManager => gm != null ? gm.stageManager : null;
        public static BoardManager BoardManager => gm != null ? gm.boardManager : null;
        public static TurnManager TurnManager => gm != null ? gm.turnManager : null;

        public static SupportLineManager SupportLineManager => gm != null ? gm.supportLineManager : null;
        public static SynergyLineManager SynergyLineManager => gm != null ? gm.synergyLineManager : null;

        public static AttackLineManager AttackLineManager => gm != null ? gm.attackLineManager : null;
        public static CombatTextManager CombatTextManager => gm != null ? gm.combatTextManager : null;
        public static GhostManager GhostManager => gm != null ? gm.ghostManager : null;

        public static Portrait2DManager Portrait2DManager => gm != null ? gm.portrait2DManager : null;
        public static Portrait3DManager Portrait3DManager => gm != null ? gm.portrait3DManager : null;

        public static TileManager TileManager => gm != null ? gm.tileManager : null;
        public static FootstepManager FootstepManager => gm != null ? gm.footstepManager : null;
        public static AudioManager AudioManager => gm != null ? gm.audioManager : null;
        public static VfxManager VfxManager => gm != null ? gm.vfxManager : null;
        public static CoinManager CoinManager => gm != null ? gm.coinManager : null;
        public static PauseManager PauseManager => gm != null ? gm.pauseManager : null;
        public static DebugManager DebugManager => gm != null ? gm.debugManager : null;
        public static ConsoleManager ConsoleManager => gm != null ? gm.consoleManager : null;
        public static LogManager LogManager => gm != null ? gm.logManager : null;
        public static ActorManager ActorManager => gm != null ? gm.actorManager : null;

        public static SelectedHeroManager SelectedHeroManager => gm != null ? gm.selectedHeroManager : null;
        public static DottedLineManager DottedLineManager => gm != null ? gm.dottedLineManager : null;
        public static ProjectileManager ProjectileManager => gm != null ? gm.projectileManager : null;
        public static SequenceManager SequenceManager => gm != null ? gm.sequenceManager : null;
        public static PincerAttackManager PincerAttackManager => gm != null ? gm.pincerAttackManager : null;
        public static SortingManager SortingManager => gm != null ? gm.sortingManager : null;
        public static TargetLineManager TargetLineManager => gm != null ? gm.targetLineManager : null;
        public static AbilityButtonManager AbilityButtonManager => gm != null ? gm.abilityButtonManager : null;

  
        public static BackgroundInstance Background => gm != null ? gm.background : null;
        public static BoardOverlay BoardOverlay => gm != null ? gm.boardOverlay : null;

      
        public static Vector2 Viewport => GameManager.instance.viewport;      
        public static float TileSize => GameManager.instance.tileSize;          
        public static Vector3 TileScale => GameManager.instance.tileScale;    
        public static Canvas Canvas3D => gm != null ? gm.canvas3D : null;
        public static WaveAnnouncement WaveAnnouncement => gm != null ? gm.waveAnnouncement : null;
        public static TargetModeOverlay TargetModeOverlay => gm != null ? gm.targetModeOverlay : null;
        public static Card Card => gm != null ? gm.card : null;
        public static TutorialPopup TutorialPopup => gm != null ? gm.tutorialPopup : null;
        public static Timeline Timeline => gm != null ? gm.timeline : null;

     
        public static Vector3 TouchPosition2D => GameManager.instance.touchPosition2D; 
        public static Vector3 TouchPosition3D => GameManager.instance.touchPosition3D; 

        public static Vector3 TouchOffset
        {
            get => GameManager.instance.touchOffset; 
            set { if (gm != null) gm.touchOffset = value; }
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
                get => gm != null ? gm.actors : null;
                set { if (gm != null) gm.actors = value; }
            }

            public static IEnumerable<ActorInstance> Heroes => gm != null ? gm.heroes : Enumerable.Empty<ActorInstance>();
            public static IEnumerable<ActorInstance> Enemies => gm != null ? gm.enemies : Enumerable.Empty<ActorInstance>();

            public static ActorInstance FocusedActor
            {
                get => gm != null ? gm.focusedActor : null;
                set { if (gm != null) gm.focusedActor = value; }
            }

            public static bool HasFocusedActor => gm != null && gm.hasFocusedActor;

            public static ActorInstance SelectedHero
            {
                get => gm != null ? gm.selectedHero : null;
                set { if (gm != null) gm.selectedHero = value; }
            }

            public static bool HasSelectedHero => gm != null && gm.hasSelectedHero;

            public static ActorInstance TargetActor
            {
                get => gm != null ? gm.targetActor : null;
                set { if (gm != null) gm.targetActor = value; }
            }

            public static bool HasTargetActor => gm != null && gm.hasTargetActor;
        }

        // World instances
        public static TileMap TileMap => gm != null ? gm.tileMap : null;
        public static TimerBar2D TimerBar2D => gm != null ? gm.timerBar2D : null;
        public static BoardInstance Board => gm != null ? gm.board : null;

        public static List<TileInstance> Tiles => gm != null ? gm.tiles : null;

        public static RectTransform PortraitsContainer => gm != null ? gm.portraitsContainer : null;

        // CoinManager
        public static CoinCounter CoinCounter => gm != null ? gm.coinCounter : null;

        // Move coin total into the active save instead of GameManager
        public static int TotalCoins
        {
            get
            {
                var save = ProfileHelper.CurrentProfile?.CurrentSave;
                return save?.Global?.TotalCoins ?? 0;
            }
            set
            {
                var save = ProfileHelper.CurrentProfile?.CurrentSave;
                if (save == null || save.Global == null) return;

                save.Global.TotalCoins = Mathf.Max(0, value);
                // Persist immediately; consider batching if this is called frequently
                //ProfileHelper.Save(true);
            }
        }

        public static HeroManager HeroManager => gm != null ? gm.heroManager : null;
    }
}
