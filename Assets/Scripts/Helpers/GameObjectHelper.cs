namespace Assets.Helper
{
    public static class GameObjectHelper
    {
        public const string Canvas = "Canvas";

        public static class Actor
        {
            public static class Front
            {
                public const string Root = "Front";
                public const string Opaque = Root + "/Opaque";
                public const string Quality = Root + "/Quality";
                public const string Glow = Root + "/Glow";
                public const string Parallax = Root + "/Parallax";
                public const string Thumbnail = Root + "/Thumbnail";
                public const string Frame = Root + "/Frame";
                public const string StatusIcon = Root + "/StatusIcon";
                public const string NameTagText = Root + "/NameTagText";
                public const string WeaponIcon = Root + "/WeaponIcon";

                public static class HealthBar
                {
                    public const string Root = Front.Root + "/HealthBar";
                    public const string Back = Root + "/HealthBarBack";
                    public const string Drain = Root + "/HealthBarDrain";
                    public const string Fill = Root + "/HealthBarFill";
                    public const string Text = Root + "/HealthBarText";
                }

                public static class ActionBar
                {
                    public const string Root = Front.Root + "/ActionBar";
                    public const string Mask = Root + "/Mask";
                    public const string RadialBack = Root + "/RadialBack";
                    public const string RadialFill = Root + "/RadialFill";
                    public const string RadialText = Root + "/RadialText";
                    public const string TurnDelayText = Root + "/TurnDelayText";
                }
            }

            // Sibling objects (not under Front)
            public const string Armor = "Armor";
            public const string Back = "Back";
        }

        public static class Credits
        {
            public const string Title = "Canvas/Title";
            public const string ScrollView = "Canvas/ScrollView";
            public const string Viewport = "Canvas/ScrollView/Viewport";
            public const string Content = "Canvas/ScrollView/Viewport/Content";
            public const string Textarea = "Canvas/ScrollView/Viewport/Content/Textarea";
        }
        public static class Game
        {
            public const string Canvas3D = "Canvas3D";
            public const string PauseButton = "PauseButton";
            public const string PauseButtonIcon = "PauseButton";
            public const string PauseMenu = "Canvas/PauseMenu";
            public const string WaveAnnouncement = "Canvas/WaveAnnouncement";
            public const string PauseOverlay = "Canvas/PauseOverlay";

            public static class TimerBar2D
            {
                public const string Root = "Canvas/TimerBar2D";
                public const string Back = "Canvas/TimerBar2D/Back";
                public const string Fill = "Canvas/TimerBar2D/Fill";
                public const string Front = "Canvas/TimerBar2D/Front";
            }

            public const string Portraits = "Canvas/Portraits";


            //public const string TimerBar3D = "TimerBar3D";


            public const string CoinCounter = "Canvas/CoinCounter";
            public const string TutorialPopup = "Canvas/TutorialPopup";
            public const string TargetModeOverlay = "Canvas/TargetModeOverlay";

            public static class Background
            {
                public const string Root = "Background";
            }

            public static class Board
            {
                public const string Root = "Board";
                public const string BoardOverlay = "Board/BoardOverlay";
                public const string FocusIndicator = "Board/FocusIndicator";
                public const string TargetIndicator = "Board/TargetIndicator";
            }

            public static class Card
            {
                public const string Root = "Canvas/Card";
                public const string Backdrop = "Canvas/Card/Backdrop";
                public const string Portrait = "Canvas/Card/Portrait";
                public const string Title = "Canvas/Card/Title";
                public const string Details = "Canvas/Card/Details";
            }
        }


        public static class PartyManager
        {
            public const string Title = "Canvas/Title";
            public const string AddRemovePartyMemberButton = "Canvas/AddRemovePartyMemberButton";
            public const string AddRemovePartyMemberButtonLabel = "Canvas/AddRemovePartyMemberButton/Label";
            public const string PartyMemberCountLabel = "Canvas/PartyMemberCountLabel";
            public const string StatsDisplay = "Canvas/StatsDisplay";
            public const string RosterPanel = "Canvas/RosterCarousel/Panel";
        }

        public static class Overworld
        {
            public const string Title = "Canvas/Title";
            public const string ScrollView = "Canvas/ScrollView";
            public const string Viewport = "Canvas/ScrollView/Viewport";
            public const string Content = "Canvas/ScrollView/Viewport/Content";
            public const string Map = "Canvas/ScrollView/Viewport/Content/Map";
            public const string Hero = "Canvas/ScrollView/Viewport/Content/Hero";

        }

        public static class ProfileCreate
        {
            public const string Background = "Canvas/Background";
        }


        public static class ProfileSelect
        {
            public const string Title = "Canvas/Title";
            public const string ScrollView = "Canvas/ScrollView";
            public const string Content = "Canvas/ScrollView/Viewport/Content";

        }

        public static class SplashScreen
        {
        }

        public static class Settings
        {
            public const string Title = "Canvas/Title";
            public const string ScrollView = "Canvas/ScrollView";
            public const string Content = "Canvas/ScrollView/Viewport/Content";
            public const string ActorPanMultiplier = "Canvas/ScrollView/Viewport/Content/ActorPanMultiplier";
        }

        public static class StageSelect
        {
            public const string Title = "Canvas/Title";
            public const string ScrollView = "Canvas/ScrollView";
            public const string Content = "Canvas/ScrollView/Viewport/Content";
        }

        public static class TitleScreen
        {
            public const string Panel = "Canvas/Panel";
            public const string ContinueButton = "Canvas/Panel/ContinueButton";
            public const string LoadGameButton = "Canvas/Panel/LoadGameButton";
            public const string SettingsButton = "Canvas/Panel/SettingsButton";
            public const string CreditsButton = "Canvas/Panel/CreditsButton";
            public const string ProfileButton = "Canvas/ProfileButton";
            public const string ProfileButtonLabel = "Canvas/ProfileButton/Label";
        }

        public static class ConfirmationDialog
        {
            public const string ConfirmDialog = "Canvas/ConfirmationDialog";
            public const string Panel = "Canvas/ConfirmationDialog/Panel";
            public const string Prompt = "Canvas/ConfirmationDialog/Panel/Prompt";
            public const string ButtonYes = "Canvas/ConfirmationDialog/Panel/ButtonYes";
            public const string ButtonNo = "Canvas/ConfirmationDialog/Panel/ButtonNo";
        }

        public static class MessageBox
        {
            public const string ConfirmDialog = "Canvas/MessageBox";
            public const string Panel = "Canvas/MessageBox/Panel";
            public const string Prompt = "Canvas/MessageBox/Panel/Prompt";
            public const string ButtonOk = "Canvas/MessageBox/Panel/ButtonOk";
        }

        public static class KeyboardDialog
        {

            public const string Keyboard = "Canvas/Keyboard";
            public const string Panel = Keyboard + "/Panel";
            public const string Prompt = Panel + "/Prompt";
            public const string InputBackdrop = Panel + "/InputBackdrop";
            public const string InputLabel = Panel + "/InputLabel";
            public const string KeysContainer = Panel + "/KeysContainer";

            // Row 1: digits
            public const string Row1 = KeysContainer + "/Row1";
            public const string Key1 = Row1 + "/Key1";
            public const string Key2 = Row1 + "/Key2";
            public const string Key3 = Row1 + "/Key3";
            public const string Key4 = Row1 + "/Key4";
            public const string Key5 = Row1 + "/Key5";
            public const string Key6 = Row1 + "/Key6";
            public const string Key7 = Row1 + "/Key7";
            public const string Key8 = Row1 + "/Key8";
            public const string Key9 = Row1 + "/Key9";
            public const string Key0 = Row1 + "/Key0";

            // Row 2: Q–P
            public const string Row2 = KeysContainer + "/Row2";
            public const string KeyQ = Row2 + "/KeyQ";
            public const string KeyW = Row2 + "/KeyW";
            public const string KeyE = Row2 + "/KeyE";
            public const string KeyR = Row2 + "/KeyR";
            public const string KeyT = Row2 + "/KeyT";
            public const string KeyY = Row2 + "/KeyY";
            public const string KeyU = Row2 + "/KeyU";
            public const string KeyI = Row2 + "/KeyI";
            public const string KeyO = Row2 + "/KeyO";
            public const string KeyP = Row2 + "/KeyP";

            // Row 3: A–L
            public const string Row3 = KeysContainer + "/Row3";
            public const string KeyA = Row3 + "/KeyA";
            public const string KeyS = Row3 + "/KeyS";
            public const string KeyD = Row3 + "/KeyD";
            public const string KeyF = Row3 + "/KeyF";
            public const string KeyG = Row3 + "/KeyG";
            public const string KeyH = Row3 + "/KeyH";
            public const string KeyJ = Row3 + "/KeyJ";
            public const string KeyK = Row3 + "/KeyK";
            public const string KeyL = Row3 + "/KeyL";

            // Row 4: Z–M
            public const string Row4 = KeysContainer + "/Row4";
            public const string KeyZ = Row4 + "/KeyZ";
            public const string KeyX = Row4 + "/KeyX";
            public const string KeyC = Row4 + "/KeyC";
            public const string KeyV = Row4 + "/KeyV";
            public const string KeyB = Row4 + "/KeyB";
            public const string KeyN = Row4 + "/KeyN";
            public const string KeyM = Row4 + "/KeyM";

            // Row 5: CapsLock, Spacebar, Backspace, Enter
            public const string Row5 = KeysContainer + "/Row5";
            public const string KeyCapsLock = Row5 + "/KeyCapsLock";
            public const string KeySpace = Row5 + "/KeySpace";
            public const string KeyBackspace = Row5 + "/KeyBackspace";
            public const string KeyEnter = Row5 + "/KeyEnter";

            public const string ConfirmationContainer = Panel + "/ConfirmationContainer";
            public const string Confirmation = ConfirmationContainer + "/Confirmation";
            public const string ButtonYes = ConfirmationContainer + "/ButtonYes";
            public const string ButtonNo = ConfirmationContainer + "/ButtonNo";
        }
    }
}