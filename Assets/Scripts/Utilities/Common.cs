using Game.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class SceneHelper
{
    public static string Credits = "Credits";
    public static string Game = "Game";
    public static string Overworld = "Overworld";
    public static string ProfileSelect = "ProfileSelect";
    public static string Splash = "Splash";
    public static string Settings = "Settings";
    public static string StageSelect = "StageSelect";
    public static string Title = "Title";
}

public static class ComponentHelper
{
    public static class Credits
    {
        public static string Canvas2D = "Canvas2D";
        public static string Fade = "Canvas2D/Fade";
        public static string Content = "Canvas2D/ScrollView/Viewport/Content";
    }

    public static class Game
    {
        public static string Canvas2D = "Canvas2D";
        public static string Canvas3D = "Canvas3D";
        public static string Fade = "Canvas2D/Fade";
        public static string PauseButton = "Canvas2D/PauseButton";
        public static string PauseMenu = "Canvas2D/PauseMenu";
    }

    public static class Overworld
    {
        public static string Canvas2D = "Canvas2D";
        public static string Title = "Canvas2D/Title";
        public static string ScrollView = "Canvas2D/ScrollView";
        public static string Viewport = "Canvas2D/ScrollView/Viewport";
        public static string Content = "Canvas2D/ScrollView/Viewport/Content";
        public static string Map = "Canvas2D/ScrollView/Viewport/Content/Map";
        public static string Player = "Canvas2D/ScrollView/Viewport/Content/Player";
        public static string Fade = "Canvas2D/Fade";
    }

    public static class ProfileSelect
    {
        public static string Canvas2D = "Canvas2D";
        public static string Header = "Canvas2D/Header";
        public static string ScrollView = "Canvas2D/ScrollView";
        public static string Content = "Canvas2D/ScrollView/Viewport/Content";
        public static string Fade = "Canvas2D/Fade";
    }

    public static class Splash
    {
        public static string Canvas2D = "Canvas2D";
        public static string Fade = "Canvas2D/Fade";
    }


    public static class Settings
    {
        public static string Canvas2D = "Canvas2D";
        public static string Header = "Canvas2D/Header";
        public static string ScrollView = "Canvas2D/ScrollView";
        public static string Content = "Canvas2D/ScrollView/Viewport/Content";
        public static string Fade = "Canvas2D/Fade";
        public static string Credits = "Canvas2D/ScrollView/Viewport/Content/Credits";
        public static string ProfileManager = "ProfileManager";
    }

    public static class StageSelect
    {
        public static string Canvas2D = "Canvas2D";
        public static string Header = "Canvas2D/Header";
        public static string ScrollView = "Canvas2D/ScrollView";
        public static string Content = "Canvas2D/ScrollView/Viewport/Content";
        public static string Fade = "Canvas2D/Fade";
    }

    public static class Title
    {
        public static string Canvas2D = "Canvas2D";
        public static string Fade = "Canvas2D/Fade";
        public static string MainMenu = "Canvas2D/MainMenu";
        public static string ProfileManager = "ProfileManager";
    }
}

public static class Constants
{
    public const string Global = "Global";
    public const string Game = "Game";
    public const string Resources = "Resources";
    public const string Board = "Board";
    public const string BoardOverlay = "BoardOverlay";
    public const string FocusIndicator = "FocusIndicator";
    public const string Art = "Art";
    public const string CanvasOverlay = "CanvasOverlay";
    public const string Card = "Card";
    public const string TimerBar = "TimerBar";
    public const string CoinBar = "CoinBar";
    public const string TutorialPopup = "TutorialPopup";
    public const string ProfileManager = "ProfileManager";

    //Percent
    public const float percent10 = 0.1f;
    public const float percent16 = 0.166666f;
    public const float percent25 = 0.25f;
    public const float percent33 = 0.333333f;
    public const float percent50 = 0.5f;
    public const float percent66 = 0.666666f;
    public const float percent75 = 0.75f;
    public const float percent100 = 1.0f;

    //Size
    public static readonly Vector2 size10 = new Vector2(percent10, percent10);
    public static readonly Vector2 size16 = new Vector2(percent16, percent16);
    public static readonly Vector2 size25 = new Vector2(percent25, percent25);
    public static readonly Vector2 size33 = new Vector2(percent33, percent33);
    public static readonly Vector2 size50 = new Vector2(percent50, percent50);
    public static readonly Vector2 size66 = new Vector2(percent66, percent66);
    public static readonly Vector2 size75 = new Vector2(percent75, percent75);
    public static readonly Vector2 size100 = new Vector2(percent100, percent100);


    //Card
    public const string CardBackdrop = "Card/Backdrop";
    public const string CardPortrait = "Card/Portrait";
    public const string CardTitle = "Card/Title";
    public const string CardDetails = "Card/Details";

    //Audio sources
    public const int SoundSourceIndex = 0;
    public const int MusicSourceIndex = 1;
}

public static class Tag
{
    public static string Board = "Board";
    public static string Tile = "Tile";
    public static string Actor = "Actor";
    public static string SupportLine = "SupportLine";
    public static string AttackLine = "AttackLine";
    public static string Trail = "Trail";
    public static string Select = "Select";
    public static string DamageText = "DamageText";
    public static string AnnouncementText = "AnnouncementText";
    public static string Portrait = "ActorPortrait";
    public static string Ghost = "Ghost";
    public static string Footstep = "Footstep";
    public static string Wall = "Wall";
    public static string Tooltip = "Tooltip";
    public static string VFX = "VFX";
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

public static class GameObjectHelper
{
    public static GameObject GetChildGameObjectByName(GameObject parent, string childName)
    {
        //Find the child Transform by name
        Transform childTransform = parent.transform.Find(childName);

        //Return the child GameObject if found, otherwise null
        return childTransform != null ? childTransform.gameObject : null;
    }
}


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

public static class Opacity
{
    public static float Opaque = 1f;
    public static float Percent90 = 0.90f;
    public static float Percent80 = 0.80f;
    public static float Percent70 = 0.70f;
    public static float Percent60 = 0.60f;
    public static float Percent50 = 0.50f;
    public static float Percent40 = 0.40f;
    public static float Percent30 = 0.30f;
    public static float Percent20 = 0.20f;
    public static float Percent10 = 0.10f;
    public static float Transparent = 0f;
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

public static class Increment
{
    public static float OnePercent = 0.01f;
    public static float TwoPercent = 0.02f;
    public static float FivePercent = 0.05f;
    public static float TenPercent = 0.1f;
    public static float FiftyPercent = 0.5f;
    public static float HundredPercent = 1.0f;


    public static class HealthBar
    {
        public static float Drain = 0.25f;
    }

    public static class ActionBar
    {
        public static float Drain = 0.25f;
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
            public static float SlideIn = Interval.TwoSeconds;
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

public static class SortingOrder
{
    public const int Min = 0;
    public const int Default = 50;
    public const int BoardOverlay = 100;
    public const int Opponent = 120;
    public const int Supporter = 140;
    public const int Attacker = 150;
    public const int AttackLine = 200;
    public const int Moving = 900;
    public const int Max = 999;
}


public static class AnimationCurveHelper
{
    /// <summary>
    /// A smooth ease-in and ease-out curve for natural acceleration and deceleration.
    /// </summary>
    public static AnimationCurve EaseInOut => AnimationCurve.EaseInOut(0, 0, 1, 1);

    /// <summary>
    /// A linear movement curve, maintaining a constant speed from start to finish.
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
    /// An elastic movement that springs back and forth before settling.
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
    /// Generates control points for a gentle S-curve movement.
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
    /// The spell overshoots the target before curving back.
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
    /// The spell overshoots the target and curves back dramatically.
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
    /// The spell moves in a corkscrew pattern toward the target.
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
    /// The spell moves erratically toward the target.
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
        //Wait until all dying actor's HP bars are fully drained
        var dyingActors = GameManager.instance.actors.Where(x => x.isDying).ToList();
        if (dyingActors.Any())
            yield return new WaitUntil(() => dyingActors.All(x => x.healthBar.isEmpty));
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
        public static string Profiles;

        static Folder()
        {
            if (!System.IO.Directory.Exists(Application.persistentDataPath))
            {
                Debug.LogError("Application.persistentDataPath is null or whitespace.");
                return;
            }

            Profiles = Path.Combine(Application.persistentDataPath, "Profiles");
        }
    }

    public static string Create(string basePath, string folderName)
    {
        var path = Path.Combine(basePath, folderName);
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        return path;
    }

    public static List<string> Get(string basePath)
    {
        return System.IO.Directory.GetDirectories(basePath).ToList();
    }
}


