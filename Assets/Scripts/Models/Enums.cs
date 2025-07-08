
using NUnit.Framework;

public enum Team
{
    Hero,
    Enemy,
    Neutral
}


public enum Direction
{
    None,
    North,
    East,
    South,
    West,
    NorthWest,
    NorthEast,
    SouthEast,
    SouthWest,
    Up,
    Down
}

public enum Axis
{
    Horizontal,
    Vertical
}



public enum Status
{
    None,
    Poisoned,
    Cursed,
    Sleeping,
    Doom
}



//public enum GlowState
//{
//   Off,
//   On
//}


public enum Glow
{
    None,
    White,
    Red,
    Green,
    Blue
}

public enum Shadow
{
    None,
    White,
    Red,
    Green,
    Blue
}

public enum AttackStrategy
{
    AttackClosest,
    AttackWeakest,
    AttackStrongest,
    AttackRandom,
    MoveAnywhere
}

//public enum BumpStage
//{
//   Execute,
//   MoveToward,
//   MoveAway,
//   End
//}

public enum DodgeStage
{
    Start,
    TwistForward,
    TwistBackward,
    End
}

public enum AttackOutcome
{
    None = 0,
    Miss = 1,
    Hit = 2,
    CriticalHit = 3
}

public enum CoinState
{
    Start,
    Move,
    Stop,
    Destroy
}

public enum WeaponType
{
    Dagger,
    Hammer,
    Katana,
    Mace,
    Spear, 
    Sword,
    Wand
}

public enum DebugOptions
{
    None,
    DodgeTest,
    SpinTest,
    ShakeTest,
    BumpTest,
    SingleCombo,
    TripleCombo,
    CoinTest,
    PortraitSlideIn,
    PortraitPopIn,
    SpawnDamageText,
    DamageTextBounceTest,
    SupportLineTest,
    AttackLineTest,
    EnemyAttackTest,
    TitleTest,
    TooltipTest,
    TutorialTest,
    FireballTest,
    HealTest,
    RandomizeBackground
}

public enum VFX
{
    None,
    BlueSlash1,
    BlueSlash2,
    BlueSlash3,
    BlueSword,
    BlueSword4X,
    BloodClaw,
    LevelUp,
    YellowHit,
    DoubleClaw,
    LightningExplosion,
    BuffLife,
    RotaryKnife,
    AirSlash,
    FireRain,
    VFXTest_Ray_Blast,
    LightningStrike,
    PuffyExplosion,
    RedSlash2X,
    GodRays,
    AcidSplash,
    GreenBuff,
    GoldBuff,
    HexShield,
    ToxicCloud,
    OrangeSlash,
    MoonFeather,
    PinkSpark,
    BlueYellowSword,
    BlueYellowSword3X,
    RedSword
}

public enum GameFocusOption
{
    Paused = 0,
    Slower = 1,
    Slow = 2,
    Normal = 3,
    Fast = 4,
    Faster = 5
}


public enum DottedLineSegment
{
    None,
    Vertical,
    Horizontal,
    TurnTopLeft,
    TurnTopRight,
    TurnBottomLeft,
    TurnBottomRight,
    ArrowUp,
    ArrowDown,
    ArrowLeft,
    ArrowRight
}

public enum ProjectilePath
{
    AnimationCurve,
    BezierCurve
}

public enum TextMotionStyle
{
    None,
    Float,
    Oscillate,
    Bounce
}

public enum TooltipPlacement
{
    Top,
    Right,
    Bottom,
    Left
}



public enum TypewriterMode
{
    CharacterByCharacter,
    LineByLine
}

public enum TooltipTextAlignment
{
    Center,
    TopLeft
}

public enum BackgroundSet
{
    BrutalistPond,
    CandleLitPath,
    CrystalDesert,
    Moors,
    RedThorns,
    UnderTheBridge,
    CyberNecropolis,
}