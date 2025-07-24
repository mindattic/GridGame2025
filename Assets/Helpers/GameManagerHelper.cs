using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Manager;
using Game.Models;
using Game.Models.Profile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Assets.Helpers
{
    public static class GameManagerHelper
    {
        private static GameManager GM => GameManager.instance;

        public static bool ReloadThumbnailSettings
        {
            get => GM != null && GM.reloadThumbnailSettings;
            set { if (GM != null) GM.reloadThumbnailSettings = value; }
        }

        public static float GameSpeed
        {
            get => GM != null ? GM.gameSpeed : 1f;
            set { if (GM != null) GM.gameSpeed = value; }
        }

        public static AudioSource SoundSource => GM?.soundSource;
        public static InputManager InputManager => GM?.inputManager;
        public static CameraManager CameraManager => GM?.cameraManager;
        public static StageManager StageManager => GM?.stageManager;
        public static BoardManager BoardManager => GM?.boardManager;
        public static TurnManager TurnManager => GM?.turnManager;
        public static SupportLineManager SupportLineManager => GM?.supportLineManager;
        public static AttackLineManager AttackLineManager => GM?.attackLineManager;
        public static DamageTextManager DamageTextManager => GM?.damageTextManager;
        public static GhostManager GhostManager => GM?.ghostManager;
        public static PortraitManager PortraitManager => GM?.portraitManager;
        public static TileManager TileManager => GM?.tileManager;
        public static FootstepManager FootstepManager => GM?.footstepManager;
        public static AudioManager AudioManager => GM?.audioManager;
        public static VFXManager VfxManager => GM?.vfxManager;
        public static TrailManager TrailManager => GM?.trailManager;
        public static CoinManager CoinManager => GM?.coinManager;
        public static PauseManager PauseManager => GM?.pauseManager;
        public static DebugManager DebugManager => GM?.debugManager;
        public static ConsoleManager ConsoleManager => GM?.consoleManager;
        public static LogManager LogManager => GM?.logManager;
        public static ActorManager ActorManager => GM?.actorManager;
        public static SelectedHeroManager SelectedHeroManager => GM?.selectedHeroManager;
        public static DottedLineManager DottedLineManager => GM?.dottedLineManager;
        public static ProjectileManager ProjectileManager => GM?.projectileManager;
        public static SequenceManager SequenceManager => GM?.sequenceManager;
        public static PincerAttackManager PincerAttackManager => GM?.pincerAttackManager;
        public static SortingManager SortingManager => GM?.sortingManager;
        public static TargetLineManager TargetLineManager => GM?.targetLineManager;
        public static AbilityButtonManager AbilityButtonManager => GM?.abilityButtonManager;

        public static BackgroundInstance Background => GM?.background;
        public static BoardOverlay BoardOverlay => GM?.boardOverlay;
        public static FocusIndicator FocusIndicator => GM?.focusIndicator;
        public static TargetIndicator TargetIndicator => GM?.targetIndicator;

        public static CanvasOverlay CanvasOverlay => GM?.canvasOverlay;
        public static Vector2 Viewport => GM?.viewport ?? Vector2.zero;
        public static float TileSize => GM?.tileSize ?? 1f;
        public static Vector3 TileScale => GM?.tileScale ?? Vector3.one;
        public static Canvas Canvas2D => GM?.canvas2D;
        public static Canvas Canvas3D => GM?.canvas3D;
        public static WaveAnnouncement WaveAnnouncement => GM?.waveAnnouncement;
        public static TargetModeOverlay TargetModeOverlay => GM?.targetModeOverlay;
        public static Card Card => GM?.card;
        public static TutorialPopup TutorialPopup => GM?.tutorialPopup;

        public static Vector3 TouchPosition2D => GM?.touchPosition2D ?? Vector3.zero;
        public static Vector3 TouchPosition3D => GM?.touchPosition3D ?? Vector3.zero;

        public static Vector3 TouchOffset
        {
            get => GM?.touchOffset ?? Vector3.zero;
            set { if (GM != null) GM.touchOffset = value; }
        }

        public static float CursorFocus => GM?.cursorFocus ?? 0f;
        public static float SwapFocus => GM?.swapFocus ?? 0f;
        public static float MoveFocus => GM?.moveFocus ?? 0f;
        public static float SnapThreshold => GM?.actorManager?.snapTheshold ?? 0f;
        public static float DragThreshold => GM?.dragThreshold ?? 0f;
        public static float BumpFocus => GM?.bumpFocus ?? 0f;

        public static class Actors
        {
            public static List<ActorInstance> All
            {
                get => GM?.actors;
                set { if (GM != null) GM.actors = value; }
            }

            public static IEnumerable<ActorInstance> Heroes => GM?.heroes ?? Enumerable.Empty<ActorInstance>();
            public static IEnumerable<ActorInstance> Enemies => GM?.enemies ?? Enumerable.Empty<ActorInstance>();

            public static ActorInstance FocusedActor
            {
                get => GM?.focusedActor;
                set { if (GM != null) GM.focusedActor = value; }
            }

            public static bool HasFocusedActor => GM?.hasFocusedActor ?? false;

            public static ActorInstance SelectedHero
            {
                get => GM?.selectedHero;
                set { if (GM != null) GM.selectedHero = value; }
            }

            public static bool HasSelectedHero => GM?.hasSelectedPlayer ?? false;

            public static ActorInstance TargetActor
            {
                get => GM?.targetActor;
                set { if (GM != null) GM.targetActor = value; }
            }

            public static bool HasTargetActor => GM?.hasTargetActor ?? false;
        }

        public static FadeInstance Fade => GM?.fade;
        public static TileMap TileMap => GM?.tileMap;
        public static TimerBar TimerBar => GM?.timerBar;
        public static BoardInstance Board => GM?.board;

        public static bool HasTargetActor => GM?.hasTargetActor ?? false;
        public static ActorInstance TargetActor => GM?.targetActor;
        public static bool HasFocusedActor => GM?.hasFocusedActor ?? false;
        public static ActorInstance FocusedActor => GM?.focusedActor;

        public static List<TileInstance> Tiles => GM?.tiles;

        public static CoinBar CoinBar => GM?.coinBar;
        public static int TotalCoins
        {
            get => GM?.totalCoins ?? 0;
            set { if (GM != null) GM.totalCoins = value; }
        }
    }
}
