using Game.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Constants
{
    public const string Global = "Global";
    public const string Game = "Game";
    public const string Resources = "Resources";
    public const string Board = "Board";
    public const string BoardOverlay = "BoardOverlay";
    public const string Canvas2D = "Canvas2D";
    public const string Canvas3D = "Canvas3D";
    public const string Art = "Art";
    public const string CanvasOverlay = "CanvasOverlay";
    public const string Card = "Card";
    public const string TimerBar = "TimerBar";
    public const string CoinBar = "CoinBar";
    public const string TutorialPopup = "TutorialPopup";


    //Percent
    public const float percent10 = 0.1f;
    public const float percent16 = 0.166666f;
    public const float percent25 = 0.25f;
    public const float percent33 = 0.333333f;
    public const float percent50 = 0.5f;
    public const float percent66 = 0.666666f;
    public const float percent75 = 0.75f;
    public const float percent100 = 1.0f;
    public const float percent333 = 3.333333f;
    public const float percent666 = 6.666666f;

    //Size
    public static readonly Vector2 size10 = new Vector2(percent10, percent10);
    public static readonly Vector2 size16 = new Vector2(percent16, percent16);
    public static readonly Vector2 size25 = new Vector2(percent25, percent25);
    public static readonly Vector2 size33 = new Vector2(percent33, percent33);
    public static readonly Vector2 size50 = new Vector2(percent50, percent50);
    public static readonly Vector2 size66 = new Vector2(percent66, percent66);
    public static readonly Vector2 size75 = new Vector2(percent75, percent75);
    public static readonly Vector2 size100 = new Vector2(percent100, percent100);
    public static readonly Vector2 size333 = new Vector2(percent333, percent333);
    public static readonly Vector2 size666 = new Vector2(percent666, percent666);


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
    public static string Select = "Load";
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
            public static float Move = Interval.OneSecond;
            public static float Attack = Interval.HalfSecond;
        }

        public static class Player
        {
            public static float Attack = Interval.HalfSecond;
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
            public static float Drain = Interval.OneSecond;
        }


    }

    public static class After
    {
        public static class Player
        {
            public static float Attack = Interval.QuarterSecond;
        }

        public static class HealthBar
        {
            public static float Empty = Interval.HalfSecond;
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
    public const int SupportLine = 100;
    public const int Target = 120;
    public const int Supporter = 140;
    public const int Attacker = 150;
    public const int AttackLine = 200;
    public const int Moving = 900;
    public const int Max = 999;
}

public static class Scene
{
    public static string Game = "Game";
    public static string Options = "Options";
    public static string TitleScreen = "TitleScreen";
    public static string SplashScreen = "SplashScreen";

}

public static class FolderHelper
{
    public static class Folders
    {
        //persistentDataPath == C:\Users\<YourUsername>\AppData\LocalLow\<CompanyName>\<ProductName>\

        public static string Profiles;

        static Folders()
        {
            if (string.IsNullOrWhiteSpace(Application.persistentDataPath))
            {
                Debug.LogError("Application.persistentDataPath is null or whitespace.");
                return;
            }

            Profiles = Path.Combine(Application.persistentDataPath, "Profiles");
        }
    }

    public static string CreateFolder(string basePath, string folderName)
    {
        var path = Path.Combine(basePath, folderName);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static List<string> GetFolders(string basePath)
    {
        return Directory.GetDirectories(basePath).ToList();
    }
}


