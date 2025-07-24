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
using g = Assets.Helpers.GameManagerHelper;


public static class SceneHelper
{
    public static string Credits = "Credits";
    public static string Game = "Game";
    public static string Overworld = "Overworld";
    public static string ProfileCreate = "ProfileCreate";
    public static string ProfileSelect = "ProfileSelect";
    public static string SaveFileSelect = "SaveFileSelect";
    public static string SplashScreen = "SplashScreen";
    public static string Settings = "Settings";
    public static string StageSelect = "StageSelect";
    public static string TitleScreen = "TitleScreen";
    public static string PartyManager = "PartyManager";

}

public static class GameObjectHelper
{
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
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Title = "Canvas2D/Title";
        public const string ScrollView = "Canvas2D/ScrollView";
        public const string Viewport = "Canvas2D/ScrollView/Viewport";
        public const string Content = "Canvas2D/ScrollView/Viewport/Content";
        public const string Textarea = "Canvas2D/ScrollView/Viewport/Content/Textarea";
    }
    public static class Game
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Canvas3D = "Canvas3D";
        public const string PauseButton = "Canvas2D/PauseButton";
        public const string PauseMenu = "Canvas2D/PauseMenu";
        public const string WaveAnnouncement = "Canvas2D/WaveAnnouncement";
        public const string CanvasOverlay = "Canvas2D/CanvasOverlay";
        public const string TimerBar = "Canvas2D/TimerBar";
        public const string CoinBar = "Canvas2D/CoinBar";
        public const string TutorialPopup = "Canvas2D/TutorialPopup";
        public const string TargetModeOverlay = "Canvas2D/TargetModeOverlay";

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
            public const string Root = "Canvas2D/Card";
            public const string Backdrop = "Canvas2D/Card/Backdrop";
            public const string Portrait = "Canvas2D/Card/Portrait";
            public const string Title = "Canvas2D/Card/Title";
            public const string Details = "Canvas2D/Card/Details";
        }
    }


    public static class PartyManager
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Title = "Canvas2D/Title";
        public const string AddRemovePartyMemberButton = "Canvas2D/AddRemovePartyMemberButton";
        public const string AddRemovePartyMemberButtonLabel = "Canvas2D/AddRemovePartyMemberButton/Label";
        public const string PartyMemberCountLabel = "Canvas2D/PartyMemberCountLabel";
        public const string StatsDisplay = "Canvas2D/StatsDisplay";
        public const string RosterPanel = "Canvas2D/RosterCarousel/Panel";
    }

    public static class Overworld
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Title = "Canvas2D/Title";
        public const string ScrollView = "Canvas2D/ScrollView";
        public const string Viewport = "Canvas2D/ScrollView/Viewport";
        public const string Content = "Canvas2D/ScrollView/Viewport/Content";
        public const string Map = "Canvas2D/ScrollView/Viewport/Content/Map";
        public const string Hero = "Canvas2D/ScrollView/Viewport/Content/Hero";

    }

    public static class ProfileCreate
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Background = "Canvas2D/Background";
    }


    public static class ProfileSelect
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Title = "Canvas2D/Title";
        public const string ScrollView = "Canvas2D/ScrollView";
        public const string Content = "Canvas2D/ScrollView/Viewport/Content";

    }

    public static class SplashScreen
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
    }

    public static class Settings
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Title = "Canvas2D/Title";
        public const string ScrollView = "Canvas2D/ScrollView";
        public const string Content = "Canvas2D/ScrollView/Viewport/Content";
        public const string ActorPanMultiplier = "Canvas2D/ScrollView/Viewport/Content/ActorPanMultiplier";
    }

    public static class StageSelect
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Title = "Canvas2D/Title";
        public const string ScrollView = "Canvas2D/ScrollView";
        public const string Content = "Canvas2D/ScrollView/Viewport/Content";
    }

    public static class TitleScreen
    {
        public const string Canvas2D = "Canvas2D";
        public const string Fade = "Canvas2D/Fade";
        public const string Panel = "Canvas2D/Panel";
        public const string ContinueButton = "Canvas2D/Panel/ContinueButton";
        public const string LoadGameButton = "Canvas2D/Panel/LoadGameButton";
        public const string SettingsButton = "Canvas2D/Panel/SettingsButton";
        public const string CreditsButton = "Canvas2D/Panel/CreditsButton";
        public const string ProfileButton = "Canvas2D/ProfileButton";
        public const string ProfileButtonLabel = "Canvas2D/ProfileButton/Label";
    }

    public static class ConfirmationDialog
    {
        public const string Canvas2D = "Canvas2D";
        public const string ConfirmDialog = "Canvas2D/ConfirmationDialog";
        public const string Panel = ConfirmDialog + "/Panel";
        public const string Prompt = Panel + "/Prompt";
        public const string ButtonYes = Panel + "/ButtonYes";
        public const string ButtonNo = Panel + "/ButtonNo";
    }

    public static class KeyboardDialog
    {
        public const string Canvas2D = "Canvas2D";
        public const string Keyboard = "Canvas2D/Keyboard";
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

public static class Constants
{
    public const string Global = "Global";
    public const string Game = "Game";
    public const string Resources = "Resources";
    public const string Canvas2D = "Canvas2D";
    public const string Art = "Art";

    public const float PortraitSize = 1024f;

    //Date formats
    public const string dateFormat = "yyyy.MM.dd.HH.mm.ss";


    //Size
    public static readonly Vector2 Size10 = new Vector2(Increment.Percent10, Increment.Percent10);
    public static readonly Vector2 Size16 = new Vector2(Increment.Percent16, Increment.Percent16);
    public static readonly Vector2 Size25 = new Vector2(Increment.Percent25, Increment.Percent25);
    public static readonly Vector2 Size33 = new Vector2(Increment.Percent33, Increment.Percent33);
    public static readonly Vector2 Size50 = new Vector2(Increment.Percent50, Increment.Percent50);
    public static readonly Vector2 Size66 = new Vector2(Increment.Percent66, Increment.Percent66);
    public static readonly Vector2 Size75 = new Vector2(Increment.Percent75, Increment.Percent75);
    public static readonly Vector2 Size100 = new Vector2(Increment.Percent100, Increment.Percent100);



    //Card
    public const string CardBackdrop = "Card/Backdrop";
    public const string CardPortrait = "Card/PortraitManager";
    public const string CardTitle = "Card/Title";
    public const string CardDetails = "Card/Details";



    public const int MaxPartyMemberCount = 6;
}

public static class Tag
{
    public static string Board = "BoardManager";
    public static string Tile = "Tile";
    public static string Actor = "Actor";
    public static string SupportLine = "SupportLineAbove";
    public static string AttackLine = "AttackLineManager";
    public static string Trail = "Trail";
    public static string Select = "SelectProfile";
    public static string DamageText = "DamageTextManager";
    public static string AnnouncementText = "AnnouncementText";
    public static string Portrait = "ActorPortrait";
    public static string Ghost = "GhostManager";
    public static string Footstep = "FootstepManager";
    public static string Wall = "Wall";
    public static string Tooltip = "Tooltip";
    public static string VFX = "VfxManager";
    public static string DottedLine = "DottedLine";
}

public static class LocationHelper
{
    public static Vector2Int Nowhere = new Vector2Int(-1, -1);
}

public static class PositionHelper
{
    public static Vector3 Nowhere = new Vector3(-1000, -1000, -1000);
}

public static class ScreenHelper
{
    public static RectFloat ScreenInWorldUnits
    {
        get
        {
            Vector2 topRightCorner = new Vector2(1f, 1f);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            var width = edgeVector.x * 2f;
            var height = edgeVector.y * 2f;
            return new RectFloat(0, width, height, 0);
        }
    }

    public static RectFloat ScreenInPixels
    {
        get
        {
            return new RectFloat(0, Screen.width, Screen.height, 0);
        }
    }

    public static Vector3 ConvertWorldToScreenPosition(Vector3 position)
    {
        return Camera.main.WorldToScreenPoint(position);
    }

    public static Vector3 ConvertScreenToWorldPosition(Vector3 position)
    {
        return Camera.main.ScreenToWorldPoint(position);
    }
}

//public static class GameObjectHelper
//{
//    public static GameObject GetChildGameObjectByName(GameObject parent, string childName)
//    {
//        //Find the child Transform by name
//        Transform childTransform = parent.transform.Find(childName);

//        //Return the child GameObject if found, otherwise null
//        return childTransform != null ? childTransform.gameObject : null;
//    }


//    public static GameObject Find(string path)
//    {
//        if (string.IsNullOrWhiteSpace(path))
//            return null;

//        string[] names = path.Split('/');
//        GameObject current = GameObject.Find(names[0]);

//        if (current == null)
//            return null;

//        for (int i = 1; i < names.Length; i++)
//        {
//            Transform child = current.transform.Find(names[i]);
//            if (child == null)
//                return null;

//            current = child.gameObject;
//        }

//        return current;
//    }
//}


public static class RotationHelper
{
    ///<summary>
    ///Assumes sprite is facing right, if facing up subtract 90 from angle (or fix sprite)
    ///</summary>
    ///<param name="target"></param>
    ///<param name="source"></param>
    ///<returns></returns>
    public static Quaternion ByDirection(Vector3 target, Vector3 source)
    {
        var direction = target - source;
        var angle = Vector2.SignedAngle(Vector2.right, direction);
        var targetRotation = new Vector3(0, 0, angle);
        var rotation = Quaternion.Euler(targetRotation);
        return rotation;
    }
}

public static class AlignmentHelper
{
    public static bool IsInRange(float a, float b, float range)
    {
        return a <= b + range && a >= b - range;
    }

    public static bool IsBetween(float a, float b, float c)
    {
        return a > b && a < c;
    }

}

public static class ColorHelper
{
    public static Color RGB(float r, float g, float b)
    {
        return new Color(
            Mathf.Clamp(r, 0, 255) / 255,
            Mathf.Clamp(g, 0, 255) / 255,
            Mathf.Clamp(b, 0, 255) / 255,
            255 / 255);
    }

    public static Color RGBA(float r, float g, float b, float a)
    {
        return new Color(
            Mathf.Clamp(r, 0, 255) / 255,
            Mathf.Clamp(g, 0, 255) / 255,
            Mathf.Clamp(b, 0, 255) / 255,
            Mathf.Clamp(a, 0, 255) / 255);
    }

    public static class Solid
    {
        public static Color Gold = RGB(255, 215, 0);
        public static Color Black = RGB(0, 0, 0);
        public static Color Gray = RGB(128, 128, 128);
        public static Color White = RGB(255, 255, 255);
        public static Color LightBlue = RGB(128, 128, 255);
        public static Color LightRed = RGB(255, 128, 128);
        public static Color Red = RGB(255, 0, 0);
        public static Color Green = RGB(0, 255, 0);
        public static Color GunMetal = RGB(42, 52, 57);
    }

    public static class HealthBar
    {
        public static Color Green = RGB(0, 255, 0);
        public static Color Red = RGB(255, 0, 0);
        public static Color Yellow = RGB(255, 255, 0);
    }

    public static class ActionBar
    {
        public static Color Blue = RGB(0, 196, 255);
        public static Color Yellow = Color.yellow;
        public static Color Pink = RGB(100, 75, 80);
        public static Color White = Color.white;
    }

    public static class Translucent
    {
        public static Color Gold = RGBA(255, 215, 0, 128);
        public static Color White = RGBA(255, 255, 255, 128);
        public static Color Black = RGBA(0, 0, 0, 128);
        public static Color DarkBlack = RGBA(0, 0, 0, 196);
        public static Color LightBlue = RGBA(128, 128, 255, 128);
        public static Color LightRed = RGBA(255, 128, 128, 128);
        public static Color Red = RGBA(255, 0, 0, 128);
        public static Color Green = RGBA(0, 255, 0, 128);
        public static Color Yellow = RGBA(255, 255, 0, 128);
        public static Color GunMetal = RGBA(42, 52, 57, 128);
    }

    public static class Transparent
    {
        public static Color White = RGBA(255, 255, 255, 0);
        public static Color Red = RGBA(255, 0, 0, 0);
    }

    public static class Tile
    {
        public static Color White = RGBA(255, 255, 255, 96);
        public static Color Yellow = RGBA(255, 255, 0, 96);
    }
}

public static class CoroutineHelper
{
    public static IEnumerator WaitForAll(MonoBehaviour context, params IEnumerator[] coroutines)
    {
        var runningCoroutines = new List<Coroutine>();

        foreach (var coroutine in coroutines)
        {
            runningCoroutines.Add(context.StartCoroutine(coroutine));
        }

        foreach (var runningCoroutine in runningCoroutines)
        {
            yield return runningCoroutine;
        }
    }

}


public static class Rarities
{
    public static Rarity Junk = new Rarity("Junk", ColorHelper.RGB(128, 128, 128));
    public static Rarity Common = new Rarity("Common", ColorHelper.RGB(255, 255, 255));
    public static Rarity Uncommon = new Rarity("Uncommon", ColorHelper.RGB(30, 255, 0));
    public static Rarity Rare = new Rarity("Rare", ColorHelper.RGB(0, 112, 221));
    public static Rarity Epic = new Rarity("Epic", ColorHelper.RGB(163, 53, 238));
    public static Rarity Legendary = new Rarity("Legendary", ColorHelper.RGB(255, 128, 0));
}


public static class Interval
{
    public static float OneTick = 0.01f;
    public static float FiveTicks = 0.05f;
    public static float TenTicks = 0.1f;
    public static float TenthSecond = 0.1f;
    public static float QuarterSecond = 0.25f;
    public static float HalfSecond = 0.5f;
    public static float OneSecond = 1.0f;
    public static float TwoSeconds = 2.0f;
    public static float ThreeSeconds = 3.0f;
    public static float FourSeconds = 4.0f;
    public static float FiveSeconds = 5.0f;


}

public static class Opacity
{
    // Standard opacity levels
    public const float Opaque = 1f;
    public const float Percent90 = 0.90f;
    public const float Percent80 = 0.80f;
    public const float Percent70 = 0.70f;
    public const float Percent60 = 0.60f;
    public const float Percent50 = 0.50f;
    public const float Percent40 = 0.40f;
    public const float Percent30 = 0.30f;
    public const float Percent20 = 0.20f;
    public const float Percent10 = 0.10f;
    public const float Transparent = 0f;

    // Opacity values based on byte alpha (0–255)
    public static class Translucent
    {
        public const float Alpha196 = 0.76862745f;
        public const float Alpha128 = 0.50196078f;
        public const float Alpha64 = 0.25098039f;
        public const float Alpha32 = 0.12549020f;
    }
}

public static class Increment
{
    // Common Percent Constants
    public const float Percent1 = 0.01f;
    public const float Percent2 = 0.02f;
    public const float Percent3 = 0.03f;
    public const float Percent4 = 0.04f;
    public const float Percent5 = 0.05f;
    public const float Percent6 = 0.06f;
    public const float Percent7 = 0.07f;
    public const float Percent8 = 0.08f;
    public const float Percent9 = 0.09f;
    public const float Percent10 = 0.1f;
    public const float Percent16 = 0.16666667f;
    public const float Percent20 = 0.2f;
    public const float Percent25 = 0.25f;
    public const float Percent30 = 0.3f;
    public const float Percent33 = 0.33333334f;
    public const float Percent40 = 0.4f;
    public const float Percent50 = 0.5f;
    public const float Percent60 = 0.6f;
    public const float Percent66 = 0.6666667f;
    public const float Percent70 = 0.7f;
    public const float Percent75 = 0.75f;
    public const float Percent80 = 0.8f;
    public const float Percent90 = 0.9f;
    public const float Percent100 = 1.0f;

    // Opacity Values
    public const float Opaque = 1f;
    public const float Transparent = 0f;

    public static class HealthBar
    {
        public const float Drain = 1.0f;
    }

    public static class ActionBar
    {
        public const float Drain = 1.0f;
    }
}


public static class Intermission
{
    public static class Before
    {

        public static class Enemy
        {
            public static float Move = 0;
            public static float Attack = 0;
        }

        public static class Player
        {
            public static float Attack = 0;
        }

        public static class Portrait
        {
            public static float SlideIn = 0;
        }

        public static class HealthBar
        {
            public static float Drain = Interval.HalfSecond;
        }

        public static class ActionBar
        {
            public static float Drain = 0;
        }


    }

    public static class After
    {
        public static class Player
        {
            public static float Attack = 0;
        }

        public static class HealthBar
        {
            public static float Empty = 0;
        }

    }

}


public static class Wait
{
    public static WaitForSeconds OneTick() => new WaitForSeconds(Interval.OneTick);
    public static WaitForSeconds Ticks(int amount) => new WaitForSeconds(Interval.OneTick * amount);
    public static WaitForSeconds For(float seconds) => new WaitForSeconds(seconds);
    public static IEnumerator UntilNextFrame() { yield return null; }
}

//public static class SortingOrder
//{
//    public const int Min = 0;
//    public const int Default = 50;
//    public const int BoardOverlay = 100;
//    public const int Opponent = 120;
//    public const int Supporter = 140;
//    public const int Attacker = 150;
//    public const int AttackLineManager = 200;
//    public const int Moving = 900;
//    public const int Max = 999;
//}


public static class AnimationCurveHelper
{
    /// <summary>
    /// A smooth ease-in and ease-out curve for natural acceleration and deceleration.
    /// </summary>
    public static AnimationCurve EaseInOut => AnimationCurve.EaseInOut(0, 0, 1, 1);

    /// <summary>
    /// A linear move curve, maintaining a constant speed from start to finish.
    /// </summary>
    public static AnimationCurve Linear => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );

    /// <summary>
    /// A fast start that slows down toward the end.
    /// </summary>
    public static AnimationCurve EaseOut => new AnimationCurve(
        new Keyframe(0, 0, 0, 2),
        new Keyframe(1, 1, 0, 0)
    );

    /// <summary>
    /// A slow start that speeds up toward the end.
    /// </summary>
    public static AnimationCurve EaseIn => new AnimationCurve(
        new Keyframe(0, 0, 0, 0),
        new Keyframe(1, 1, 2, 0)
    );

    /// <summary>
    /// A bounce effect that overshoots and settles back.
    /// </summary>
    public static AnimationCurve Bounce => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1.2f), // Overshoot
        new Keyframe(0.75f, 0.8f), // Rebound
        new Keyframe(1, 1)
    );

    /// <summary>
    /// A wave motion with one oscillation.
    /// </summary>
    public static AnimationCurve SingleWave => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.25f, 1),
        new Keyframe(0.5f, 0),
        new Keyframe(0.75f, -1),
        new Keyframe(1, 0)
    );

    /// <summary>
    /// A wave motion with two oscillations.
    /// </summary>
    public static AnimationCurve DoubleWave => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.2f, 1),
        new Keyframe(0.4f, 0),
        new Keyframe(0.6f, -1),
        new Keyframe(0.8f, 0),
        new Keyframe(1, 1)
    );

    /// <summary>
    /// A sudden jump with a sharp drop, useful for explosive effects.
    /// </summary>
    public static AnimationCurve SharpSpike => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.2f, 1),
        new Keyframe(0.3f, -0.5f),
        new Keyframe(0.4f, 0.75f),
        new Keyframe(0.6f, -0.25f),
        new Keyframe(1, 1)
    );

    /// <summary>
    /// An elastic move that springs back and forth before settling.
    /// </summary>
    public static AnimationCurve Elastic => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.3f, 1.2f),  // Overshoot
        new Keyframe(0.5f, -0.8f), // Undershoot
        new Keyframe(0.7f, 1.1f),  // Rebound
        new Keyframe(1, 1)
    );

    /// <summary>
    /// A steep drop followed by a slow recovery.
    /// </summary>
    public static AnimationCurve FallAndRecover => new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.2f, -1.2f),
        new Keyframe(0.5f, -0.5f),
        new Keyframe(1, 1)
    );
}

public static class BezierCurveHelper
{
    /// <summary>
    /// Generates control points for a gentle S-curve move.
    /// Ensures the perpendicular wave follows the travel direction properly.
    /// </summary>
    public static List<Vector3> Gentle(ActorInstance source, ActorInstance target, float travelModifier = 1f, float waveModifier = 1.2f)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(Vector3.up, direction).normalized; // Ensure perpendicular aligns with direction

        // Alternate the wave direction properly
        float sideModifier1 = Random.Boolean ? 1f : -1f;
        float sideModifier2 = -sideModifier1; // Ensure the second control point inverts the curve correctly

        Vector3 control1 = start
            + direction * (distance * 0.3f * travelModifier)  // Move forward
            + perpendicular * (distance * 0.3f * sideModifier1 * waveModifier) // First curve direction
            + Vector3.up * (distance * 0.2f * sideModifier1 * waveModifier); // **Now alternates up/down**

        Vector3 control2 = end
            - direction * (distance * 0.3f * travelModifier)  // Move slightly backward
            + perpendicular * (distance * 0.3f * sideModifier2 * waveModifier) // Reverse the curve direction
            + Vector3.up * (distance * 0.1f * sideModifier2 * waveModifier); // **Now alternates up/down**

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(end);

        return controlPoints;
    }


    /// <summary>
    /// Generates control points for an overshooting arc.
    /// The projectile overshoots the target before curving back.
    /// </summary>
    public static List<Vector3> Overshooting(ActorInstance source, ActorInstance target, float travelModifier = 1.6f, float waveModifier = 0.2f, bool overshoot = true)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        float verticalModifier = Random.Boolean ? 1f : -1f;

        Vector3 control1 = start
            + direction * (distance * 0.5f * travelModifier)
            + perpendicular * (distance * 0.3f * waveModifier)
            + Vector3.up * (distance * 0.7f * verticalModifier * waveModifier);

        Vector3 control2 = end
            + direction * (distance * 0.3f * travelModifier)
            - perpendicular * (distance * 0.3f * waveModifier)
            + Vector3.up * (distance * 0.5f * verticalModifier * waveModifier);

        if (overshoot)
        {
            control2 += direction * (distance * 0.2f);
        }

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(end);

        return controlPoints;
    }

    public static List<Vector3> OvershootingWave(ActorInstance source, ActorInstance target, float travelModifier = 1.6f, float waveModifier = 0.2f, bool overshoot = true)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        float verticalModifier1 = Random.Boolean ? 1f : -1f;
        float verticalModifier2 = -verticalModifier1; // Reverse the wave direction

        Vector3 control1 = start
            + direction * (distance * 0.5f * travelModifier)
            + perpendicular * (distance * 0.3f * waveModifier)
            + Vector3.up * (distance * 0.7f * verticalModifier1 * waveModifier); // Alternating up/down

        Vector3 control2 = end
            + direction * (distance * 0.3f * travelModifier)
            - perpendicular * (distance * 0.3f * waveModifier)
            + Vector3.up * (distance * 0.5f * verticalModifier2 * waveModifier); // Opposite vertical direction

        if (overshoot)
        {
            control2 += direction * (distance * 0.2f);
        }

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(end);

        return controlPoints;
    }


    /// <summary>
    /// Generates control points for a lobbed arc.
    /// Similar to how a grenade or fireball might travel.
    /// </summary>
    public static List<Vector3> LobbedArc(ActorInstance source, ActorInstance target, float travelModifier = 0.8f, float waveModifier = 1.5f)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;

        Vector3 control1 = start
            + direction * (distance * 0.5f * travelModifier)
            + Vector3.up * (distance * 1.5f * waveModifier);

        Vector3 control2 = end
            - direction * (distance * 0.2f * travelModifier)
            + Vector3.up * (distance * 0.5f * waveModifier);

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(end);

        return controlPoints;
    }

    /// <summary>
    /// Generates control points for a reverse boomerang arc.
    /// The projectile overshoots the target and curves back dramatically.
    /// </summary>
    public static List<Vector3> Boomerang(ActorInstance source, ActorInstance target, float travelModifier = 1.2f, float waveModifier = 0.8f)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        float verticalModifier = Random.Boolean ? 1f : -1f;

        Vector3 control1 = start
            + direction * (distance * 0.5f * travelModifier)
            + perpendicular * (distance * 0.3f * waveModifier)
            + Vector3.up * (distance * 1.0f * verticalModifier * waveModifier);

        Vector3 control2 = end
            + direction * (distance * 0.3f * travelModifier)
            - perpendicular * (distance * 0.3f * waveModifier)
            + Vector3.up * (distance * 0.5f * verticalModifier * waveModifier);

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(end);

        return controlPoints;
    }

    /// <summary>
    /// Generates control points for a homing spiral effect.
    /// The projectile moves in a corkscrew pattern toward the target.
    /// </summary>
    public static List<Vector3> HomingSpiral(ActorInstance source, ActorInstance target, float travelModifier = 1f, float waveModifier = 2f)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        Vector3 control1 = start
            + direction * (distance * 0.3f * travelModifier)
            + perpendicular * (distance * 0.5f * waveModifier)
            + Vector3.up * (distance * 0.5f * waveModifier);

        Vector3 control2 = start
            + direction * (distance * 0.6f * travelModifier)
            - perpendicular * (distance * 0.5f * waveModifier)
            + Vector3.up * (distance * 1.0f * waveModifier);

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(end);

        return controlPoints;
    }

    /// <summary>
    /// Generates control points for a zig-zag dash.
    /// The projectile moves erratically toward the target.
    /// </summary>
    public static List<Vector3> ZigZagDash(ActorInstance source, ActorInstance target, float travelModifier = 1.1f, float waveModifier = 1.2f)
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = source.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        Vector3 control1 = start
            + direction * (distance * 0.25f * travelModifier)
            + perpendicular * (distance * 0.4f * waveModifier);

        Vector3 control2 = start
            + direction * (distance * 0.5f * travelModifier)
            - perpendicular * (distance * 0.4f * waveModifier);

        Vector3 control3 = start
            + direction * (distance * 0.75f * travelModifier)
            + perpendicular * (distance * 0.3f * waveModifier);

        controlPoints.Add(start);
        controlPoints.Add(control1);
        controlPoints.Add(control2);
        controlPoints.Add(control3);
        controlPoints.Add(end);

        return controlPoints;
    }


}

public static class DeathHelper
{
    public static IEnumerator Process()
    {
        // find everyone who’s flagged as dying
        var dyingActors = g.Actors.All.Where(x => x.isDying).ToList();
        if (dyingActors.IsNullOrEmpty())
            yield break;

        // wait until all their HP‐bars are empty
        yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));

        // now actually kill them
        foreach (var actor in dyingActors)
        {
            actor.DieAsync();
        }
    }
}


public static class MenuHelper
{
    public static void Initialize(Button[] buttons)
    {
        RectTransform parentRect = buttons[0].transform.parent as RectTransform;
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        float buttonWidth = 0.9f * parentWidth;
        float buttonHeight = parentHeight * 0.0625f;
        float spacing = 0.01f * parentHeight;

        float totalButtonHeight = buttonHeight * buttons.Length;
        float totalHeight = totalButtonHeight + spacing * (buttons.Length - 1);

        float startY = totalHeight / 2f; // We'll subtract from this for each button.

        foreach (var button in buttons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            rectTransform.anchoredPosition = new Vector2(0, startY - buttonHeight / 2f);
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            label.fontSize = buttonHeight / 2f;
            button.interactable = true;

            startY -= (buttonHeight + spacing);
        }

    }

    public static void DisableButtons(Button[] buttons)
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

}


public static class FolderHelper
{
    public static class Folder
    {
        /*
        Application.persistentDataPath resolves to:

        Windows     | C:\Users\<User>\AppData\LocalLow\<CompanyName>\<ProductName>
        macOS       | ~/Library/Application Support/<CompanyName>/<ProductName>
        Linux       | ~/.config/unity3d/<CompanyName>/<ProductName>
        Android     | /storage/emulated/0/Android/data/<package-name>/files
        iOS         | /var/mobile/Containers/Data/Application/<guid>/Documents
        WebGL       | IndexedDB (no filesystem access)
        */

        public static string Profiles
        {
            get
            {
#if UNITY_WEBGL
                Debug.LogError("File system operations are not supported on WebGL.");
                return string.Empty;
#else
                string path = Path.Combine(Application.persistentDataPath, "Profiles");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
#endif
            }
        }
    }

    /// <summary>
    /// Creates a folder at the specified path if it doesn't exist.
    /// </summary>
    public static string Create(string basePath, string folderName)
    {
#if UNITY_WEBGL
        Debug.LogError("Folder creation is not supported on WebGL.");
        return string.Empty;
#else
        var path = Path.Combine(basePath, folderName);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
#endif
    }

    /// <summary>
    /// Returns a list of directories within the specified base path.
    /// </summary>
    public static List<string> Get(string basePath)
    {
#if UNITY_WEBGL
        Debug.LogError("Directory listing is not supported on WebGL.");
        return new List<string>();
#else
        return Directory.GetDirectories(basePath).ToList();
#endif
    }
}

public static class DateTimeHelper
{

    public static DateTime ParseUtcTimestamp(string timestamp)
    {
        return DateTime.ParseExact(
            timestamp,
            Constants.dateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
        );
    }

    public static string ParseTimeElapsed(DateTime timestamp)
    {
        TimeSpan elapsed = DateTime.UtcNow - timestamp;

        if (elapsed.TotalSeconds < 60)
        {
            int seconds = (int)elapsed.TotalSeconds;
            return seconds == 1 ? "1 second ago" : $"{seconds} seconds ago";
        }
        else if (elapsed.TotalMinutes < 60)
        {
            int minutes = (int)elapsed.TotalMinutes;
            int seconds = elapsed.Seconds;
            string minutePart = minutes == 1 ? "1 minute" : $"{minutes} minutes";
            string secondPart = seconds == 1 ? "1 second" : $"{seconds} seconds";
            return seconds > 0 ? $"{minutePart}, {secondPart} ago" : $"{minutePart} ago";
        }
        else if (elapsed.TotalHours < 24)
        {
            int hours = (int)elapsed.TotalHours;
            int minutes = elapsed.Minutes;
            string hourPart = hours == 1 ? "1 hour" : $"{hours} hours";
            string minutePart = minutes == 1 ? "1 minute" : $"{minutes} minutes";
            return minutes > 0 ? $"{hourPart}, {minutePart} ago" : $"{hourPart} ago";
        }
        else if (elapsed.TotalDays < 30)
        {
            int days = (int)elapsed.TotalDays;
            int hours = elapsed.Hours;
            string dayPart = days == 1 ? "1 day" : $"{days} days";
            string hourPart = hours == 1 ? "1 hour" : $"{hours} hours";
            return hours > 0 ? $"{dayPart}, {hourPart} ago" : $"{dayPart} ago";
        }
        else
        {
            // Approximate a month as 30 days.
            int months = (int)(elapsed.TotalDays / 30);
            int days = (int)(elapsed.TotalDays % 30);
            string monthPart = months == 1 ? "1 month" : $"{months} months";
            string dayPart = days == 1 ? "1 day" : $"{days} days";
            return days > 0 ? $"{monthPart}, {dayPart} ago" : $"{monthPart} ago";
        }
    }


}

//public static class ResourceFolderHelper
//{
//    public static string Backgrounds = "Backgrounds";
//    public static string Portraits = "Portraits";
//    public static string SoundEffects = "SoundEffects";
//    public static string MusicTracks = "MusicTracks";
//    public static string Materials = "Materials";
//    public static string Seamless = "Seamless";
//    public static string Sprites = "Sprites";
//    public static string Textures = "Textures";
//    public static string TrailEffects = "TrailEffects";
//    public static string WeaponTypes = "Sprites/WeaponTypes";
//    public static string VisualEffects = "VisualEffects";
//    public static string Prefabs = "Prefabs";
//}


public static class AssetFolderHelper
{
    public static string Prefabs = "Prefabs";
}





public static class RepositoryHelper
{
    public const string ProfileRepo = "Repositories/ProfileRepo";
}

public static class ProfileHelper
{
    public const string SettingsFileName = "Settings.json";

    public static ProfileSettings DefaultSettings = new ProfileSettings()
    {
        ActorPanMultiplier = 0.05f,
        GameFocus = 1.0f,
    };

    public static GlobalSaveData DefaultGlobal = new GlobalSaveData()
    {
        TotalCoins = 0,
    };

    public static StageSaveData DefaultStage = new StageSaveData()
    {
        CurrentStage = "Stage 1",
        CurrentWave = 0,
    };

    public static RosterSaveData DefaultRoster = new RosterSaveData()
    {
        Members = new List<CharacterLevelPair>() {
        new CharacterLevelPair(CharacterHelper.Paladin),
        new CharacterLevelPair(CharacterHelper.Barbarian),
        new CharacterLevelPair(CharacterHelper.Cleric),
        new CharacterLevelPair(CharacterHelper.GreenNinja),
        new CharacterLevelPair(CharacterHelper.Pugilist),
        new CharacterLevelPair(CharacterHelper.RedNinja),
        new CharacterLevelPair(CharacterHelper.Ronin),
        new CharacterLevelPair(CharacterHelper.Sellsword),
        new CharacterLevelPair(CharacterHelper.Thief),
        new CharacterLevelPair(CharacterHelper.Vampire),
    }
    };

    public static PartySaveData DefaultParty = new PartySaveData()
    {
        Members = new List<CharacterLevelPair>() {
            new CharacterLevelPair(CharacterHelper.Paladin),
            new CharacterLevelPair(CharacterHelper.Barbarian),
            new CharacterLevelPair(CharacterHelper.Cleric),
        }
    };

}

public static class CharacterHelper
{
    public const string Barbarian = "Barbarian";
    public const string Bat = "Bat";
    public const string Cleric = "Cleric";
    public const string Captain00 = "Captain00";
    public const string GreenNinja = "GreenNinja";
    public const string Paladin = "Paladin";
    public const string PandaGirl = "PandaGirl";
    public const string Pugilist = "Pugilist";
    public const string RedNinja = "RedNinja";
    public const string Ronin = "Ronin";
    public const string Sellsword = "Sellsword";
    public const string Scorpion = "Scorpion";
    public const string Soldier00 = "Soldier00";
    public const string Soldier01 = "Soldier02";
    public const string Soldier02 = "Soldier01";
    public const string Soldier03 = "Soldier03";
    public const string Slime = "Slime";
    public const string Thief = "Thief";
    public const string Vampire = "Vampire";
    public const string Yeti = "Yeti";
}


public static class TextureHelper
{
    public static Texture2D CreateNewTexture(Texture2D originalTexture, Rect rect)
    {
        // Ensure the rect dimensions are within the bounds of the original texture
        int rectX = Mathf.Clamp((int)rect.x, 0, originalTexture.width);
        int rectY = Mathf.Clamp((int)rect.y, 0, originalTexture.height);
        int rectWidth = Mathf.Clamp((int)rect.width, 0, originalTexture.width - rectX);
        int rectHeight = Mathf.Clamp((int)rect.height, 0, originalTexture.height - rectY);

        // Data a new texture with the specified dimensions
        Texture2D newTexture = new Texture2D(rectWidth, rectHeight);

        // Copy the pixel actors from the original texture to the new texture
        Color[] pixels = originalTexture.GetPixels(rectX, rectY, rectWidth, rectHeight);
        newTexture.SetPixels(pixels);

        // Apply the changes to the new texture
        newTexture.Apply();

        return newTexture;
    }
}

public static class AssetHelper
{
    public static async Task<T> LoadAssetAsync<T>(string address)
    {
        var handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }

        Debug.LogError($"Failed to load {typeof(T)} at address: {address}");
        return default(T);
    }

    public static T LoadAsset<T>(string address)
    {
        var handle = Addressables.LoadAssetAsync<T>(address);
        handle.WaitForCompletion(); // Block until the asset is fully loaded


        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }

        Debug.LogError($"Failed to load {typeof(T)} at address: {address}");
        return default(T);
    }
}

/*
 * Alternative: Let the Caller Handle the Release
If you want the caller to manage the handle (e.g., for long-term use of the asset), you can return the AsyncOperationHandle<Sprite> instead of the Sprite itself:

public static async Task<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite>> LoadSpriteHandleAsync(string address)
{
    var handle = Addressables.LoadAssetAsync<Sprite>(address);
    await handle.Task;

    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
    {
        return handle;
    }

    Debug.LogError($"Failed to load sprite at address: {address}");
    return default; // Return an empty handle if loading fails
}

*/

