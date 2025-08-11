using System;

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
    CyberNecropolis,
    Moors,
    RedThorns,
    UnderTheBridge,
    ElectricWasteland
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
    Bounce,
    Seek,
    Despawn
}

public enum DebugOptions
{
    None,
    ArrangeSingleCombo,
    ArrangeDoubleCombo,
    ArrangeTripleCombo,
    ArrangeSurroundCombo,
    Bump,
    Dodge,
    Fireball,
    Heal,
    KillEnemies,
    PortraitPopIn,
    Portrait2DSlideIn,
    Portrait3DSlideIn,
    RandomizeBackground,
    Shake,
    Spin,
    SpawnCoins,
    SpawnDamageText,
    SpawnHealText,
    SpawnSupportLines,
    SpawnTitle,
    SpawnTooltip1,
    SpawnTooltip2,
    TriggerEnemyMoveAttack,
    TriggerEnemyAttack,
}


public enum Direction
{
    None,
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest,
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

public enum GameSpeedOption
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
    None,
    Player,
    AbilityTarget,
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

public enum TextMotion
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

public enum TextureResolution
{
    NormalResolution = 1024,
    HighResolution = 4096
}

[Flags]
public enum ActorGroup : uint
{
    None = 0,
    Soldier = 1u << 0,
    Goblin = 1u << 1,
    Undead = 1u << 2,
    Beast = 1u << 3,
    Boss = 1u << 4,
    Humanoid = 1u << 5,
    Mechanical = 1u << 6,
    Elite = 1u << 7,
    Flying = 1u << 8,
    Ranged = 1u << 9,
    Melee = 1u << 10
}