using System;
using game = GameManagerHelper;

public enum AbilityType
{
    Passive,
    Self,
    TargetAlly,
    TargetAny,
    TargetOpponent
}

public enum AttackOutcome
{
    None = 0,
    Miss = 1,
    Hit = 2,
    CriticalHit = 3
}

public enum AttackStrategy
{
    AttackClosest,
    AttackRandom,
    AttackStrongest,
    AttackWeakest,
    MoveAnywhere
}

public enum Axis
{
    Horizontal,
    Vertical
}

public enum BackgroundSet
{
    BrutalistPond,
    CandleLitPath,
    CrystalDesert,
    CyberNecropolis,
    Moors,
    RedThorns,
    UnderTheBridge,
}

public enum BoardPoint
{
    BottomCenter,
    BottomLeft,
    BottomRight,
    MiddleCenter,
    MiddleLeft,
    MiddleRight,
    TopCenter,
    TopLeft,
    TopRight
}

public enum Characters
{
    Barbarian,
    Bat,
    Cleric,
    GreenNinja,
    Paladin,
    PandaGirl,
    RedNinja,
    Scorpion,
    Slime,
    Thief,
    Vampire,
    Yeti
}

public enum CoinState
{
    Destroy,
    Move,
    Start,
    Stop
}

public enum DebugOptions
{
    None,
    KillEnemies,
    AttackLineTest,
    BumpTest,
    CoinTest,
    DamageTextBounceTest,
    DodgeTest,
    EnemyAttackTest,
    FireballTest,
    HealTest,
    PortraitPopIn,
    PortraitSlideIn,
    RandomizeBackground,
    ShakeTest,
    SingleCombo,
    SpawnDamageText,
    SpinTest,
    SupportLineTest,
    TitleTest,
    TooltipTest,
    TripleCombo,
    TutorialTest
}

public enum Direction
{
    Down,
    East,
    North,
    NorthEast,
    NorthWest,
    None,
    South,
    SouthEast,
    SouthWest,
    Up,
    West
}

public enum DodgeStage
{
    End,
    Start,
    TwistBackward,
    TwistForward
}

public enum DottedLineSegment
{
    ArrowDown,
    ArrowLeft,
    ArrowRight,
    ArrowUp,
    Horizontal,
    None,
    TurnBottomLeft,
    TurnBottomRight,
    TurnTopLeft,
    TurnTopRight,
    Vertical
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

public enum Glow
{
    Blue,
    Green,
    None,
    Red,
    White
}

//public enum GlowState
//{
//    Off,
//    On
//}

public enum InputMode
{
    AbilityTarget,
    EnemyTurn,
    HeroTurn
}

public enum LogLevel
{
    None = 0,
    Info = 1,
    Success = 2,
    Warning = 3,
    Error = 4,
    Fatal = 5
}

public enum MoveDirection
{
    Idle = 0,
    Up = 1,
    Right = 2,
    Down = 3,
    Left = 4
}

public enum PlayStateProcess
{
    Editing,
    PreStarting,
    Ready,
    Starting
}

public enum ProjectilePath
{
    AnimationCurve,
    BezierCurve
}

public enum Shadow
{
    Blue,
    Green,
    None,
    Red,
    White
}

public enum StageCompletionCondition
{
    CollectCoins,
    DefeatAllEnemies,
    SurviveTurns
}

public enum Status
{
    None,
    Poisoned,
    Cursed,
    Sleeping,
    Doom
}

public enum Team
{
    Enemy,
    Hero,
    Neutral
}

public enum TextMotionStyle
{
    Bounce,
    Float,
    None,
    Oscillate
}

public enum TooltipPlacement
{
    Bottom,
    Left,
    Right,
    Top
}

public enum TooltipTextAlignment
{
    Center,
    TopLeft
}

public enum TurnPhase
{
    Attack,
    End,
    Move,
    PostAttack,
    PreAttack,
    Start
}

public enum TypewriterMode
{
    CharacterByCharacter,
    LineByLine
}

public enum VFX
{
    AcidSplash,
    AirSlash,
    BloodClaw,
    BlueSlash1,
    BlueSlash2,
    BlueSlash3,
    BlueSword,
    BlueSword4X,
    BlueYellowSword,
    BlueYellowSword3X,
    BuffLife,
    DoubleClaw,
    FireRain,
    GodRays,
    GoldBuff,
    GreenBuff,
    HexShield,
    LevelUp,
    LightningExplosion,
    LightningStrike,
    MoonFeather,
    None,
    OrangeSlash,
    PinkSpark,
    PuffyExplosion,
    RedSlash2X,
    RedSword,
    RotaryKnife,
    ToxicCloud,
    VFXTest_Ray_Blast,
    YellowHit
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
