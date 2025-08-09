
using Assets.Helper;
using UnityEngine;

public static class Constants
{
    public const string Global = "Global";
    public const string Game = "Game";
    public const string Resources = "Resources";
    public const string Canvas = "Canvas";
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
    public const string CardPortrait = "Card/Portrait3DManager";
    public const string CardTitle = "Card/Title";
    public const string CardDetails = "Card/Details";



    public const int MaxPartyMemberCount = 6;
}
