using Assets.Scripts.Models;
using Game.Behaviors;
using UnityEngine;

public enum ElementalDamageType
{
    Physical,
    Fire,
    Ice,
    Lightning,
    Acid,
    Dark,
    Light,
    Earth,
    Water,
    Wind,
    Psychic,
    Poison,
    Arcane
}

public enum HitOutcome
{
    Normal,
    Critical,
    Weak
}

/// <summary>
/// Centralized numeric formulas for combat, movement pacing, and stat conversions.
/// All methods are pure utilities and must not mutate game state.
/// Calculations use float throughout and convert to int only at the final step where required.
/// </summary>
public static class Formulas
{
    // Add explicit level advantage so +5 feels "nearly impossible"
    private const float HitShiftPerLevel = 4f;        // +4% hit per level advantage
    private const float DamageScalePerLevel = 1.10f;  // +10% damage per level advantage
    private static int LevelAdvantage(ActorStats atk, ActorStats def)
        => Mathf.RoundToInt(atk.Level - def.Level);

    // Note: Do not keep cached managers here. These helpers must stay pure.

    /// <summary>
    /// Returns a small multiplicative tilt based on Luck to bias random variance slightly.
    /// </summary>
    private static float LuckTilt(ActorStats stats)
    {
        float t = Mathf.Clamp01(stats.Luck / 100f);
        return Mathf.Lerp(0.98f, 1.02f, t);
    }

    /// <summary>
    /// Samples a multiplicative variance in [1 - range, 1 + range], slightly biased by Luck.
    /// No rounding occurs here.
    /// </summary>
    private static float SampleVarianceWithLuck(ActorStats stats, float rangeFraction)
    {
        float minV = 1f - rangeFraction;
        float maxV = 1f + rangeFraction;

        float roll = RNG.Float(minV, maxV);
        float adjusted = roll * LuckTilt(stats);

        return Mathf.Clamp(adjusted, minV, maxV);
    }

    /// <summary>
    /// Returns a small multiplicative modifier based on Level and Luck.
    /// No rounding occurs here.
    /// </summary>
    public static float LuckModifier(ActorStats stats)
    {
        // Make Luck a small additive bonus, not a huge RNG spike that scales with Level.
        // ~0–20 points at 0–100 Luck
        return Mathf.Clamp(stats.Luck * 0.2f, 0f, 20f);
    }

    /// <summary>
    /// Attacker hit accuracy score before compare with defender evasion.
    /// Pure float math; no rounding.
    /// </summary>
    public static float Accuracy(ActorStats stats)
    {
        float baseAccuracy = 75f + stats.Level * 0.25f; // slightly softer level scaling
        float precision = stats.Wisdom * 0.8f;          // lower coeff to avoid easy 95% caps
        float luck = LuckModifier(stats);
        return baseAccuracy + precision + luck;
    }

    /// <summary>
    /// Defender evasion score before compare with attacker accuracy.
    /// Pure float math; no rounding.
    /// </summary>
    public static float Evasion(ActorStats stats)
    {
        // Keep Agility primary but avoid trivial 5%/95% saturation
        float agility = stats.Agility * 1.4f;
        float speed = stats.Speed * 0.9f;
        float staminaBonus = stats.Stamina * 0.3f;
        float luck = Mathf.Min(10f, LuckModifier(stats)); // cap Luck influence on evasion
        return agility + speed + staminaBonus + luck;
    }

    /// <summary>
    /// Rolls Normal, CriticalHit, or GlancingBlow based on accuracy vs evasion and crit chance.
    /// Uses float comparisons only; no integer conversions here.
    /// </summary>
    public static HitOutcome CalculateHitType(ActorInstance attacker, ActorInstance opponent)
    {
        float accuracy = Accuracy(attacker.Stats);
        float evade = Evasion(opponent.Stats);
        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);

        float hitChance = Mathf.Clamp(accuracy - evade + adv * HitShiftPerLevel, 5f, 95f);
        if (RNG.Float(0f, 100f) >= hitChance)
            return HitOutcome.Weak;

        float baseCrit = 5f;
        float focus = attacker.Stats.Wisdom * 0.35f;
        float luck = Mathf.Min(20f, attacker.Stats.Luck * 0.25f);
        float critChance = Mathf.Clamp(baseCrit + focus + luck, 0f, 60f);
        return RNG.Float(0f, 100f) < critChance ? HitOutcome.Critical : HitOutcome.Normal;
    }

    /// <summary>
    /// Derived max health from core Stats.
    /// Pure float math; callers decide if they need rounding for display.
    /// </summary>
    public static float Health(ActorStats stats)
    {
        return 50f + stats.Vitality * 10f + stats.Level * 2f;
    }

    /// <summary>
    /// Physical offense score including optional Weapon power.
    /// FloatRoutine only; no rounding.
    /// </summary>
    public static float Offense(ActorStats stats, float weaponPower = 0f)
    {
        return stats.Strength * 2f + weaponPower;
    }

    /// <summary>
    /// Physical defense score including optional armor rating.
    /// FloatRoutine only; no rounding.
    /// </summary>
    public static float Defense(ActorStats stats, float armorRating = 0f)
    {
        return stats.Vitality * 1.5f + stats.Stamina * 0.5f + armorRating;
    }

    /// <summary>
    /// Magical offense score from Intelligence and Wisdom.
    /// FloatRoutine only; no rounding.
    /// </summary>
    public static float MagicOffense(ActorStats stats)
    {
        return stats.Intelligence * 2.5f + stats.Wisdom * 1f;
    }

    /// <summary>
    /// Magical resistance score from Intelligence, Wisdom, and Stamina.
    /// FloatRoutine only; no rounding.
    /// </summary>
    public static float MagicResistance(ActorStats stats)
    {
        return stats.Intelligence * 1.5f + stats.Wisdom * 1f + stats.Stamina * 0.5f;
    }

    /// <summary>
    /// Applies positive or negative resistance to a base value as a float multiplier.
    /// No rounding here.
    /// </summary>
    public static float ApplyResistance(float baseValue, float resistance)
    {
        if (resistance >= 0f)
            return baseValue * (100f / (100f + resistance));

        return baseValue * (1f + Mathf.Abs(resistance) / 100f);
    }

    /// <summary>
    /// Computes a fully populated physical AttackResult for attacker vs opponent.
    /// All math remains float until the final conversion to int damage.
    /// </summary>
    public static AttackResult CalculateAttackResult(
        ActorInstance attacker, ActorInstance opponent,
        float weaponPower = 0f, float armorRating = 0f,
        ElementalDamageType element = ElementalDamageType.Physical, float resistance = 0f)
    {
        float off = Offense(attacker.Stats, weaponPower);
        float def = Defense(opponent.Stats, armorRating);
        float raw = off - def;

        float resisted = ApplyResistance(raw, resistance);
        float varied = resisted * SampleVarianceWithLuck(attacker.Stats, 0.20f); // ±20%

        HitOutcome type = CalculateHitType(attacker, opponent);
        float typeMult = type == HitOutcome.Critical ? 1.5f : (type == HitOutcome.Weak ? 0.5f : 1f);

        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);
        float levelMult = Mathf.Pow(DamageScalePerLevel, adv);

        int finalDamage = Mathf.Max(1, Mathf.FloorToInt(varied * typeMult * levelMult));
        return new AttackResult(attacker, opponent, finalDamage, type);
    }

    /// <summary>
    /// Computes a fully populated magical AttackResult for caster vs target.
    /// All math remains float until the final conversion to int damage.
    /// </summary>
    public static AttackResult CalculateMagicDamage(
        ActorInstance caster, ActorInstance target,
        ElementalDamageType element = ElementalDamageType.Arcane, float resistance = 0f)
    {
        float off = MagicOffense(caster.Stats);
        float res = MagicResistance(target.Stats);
        float raw = off - res;

        float resisted = ApplyResistance(raw, resistance);
        float varied = resisted * SampleVarianceWithLuck(caster.Stats, 0.20f);

        HitOutcome type = CalculateHitType(caster, target);
        float typeMult = type == HitOutcome.Critical ? 1.5f : (type == HitOutcome.Weak ? 0.5f : 1f);

        int adv = LevelAdvantage(caster.Stats, target.Stats);
        float levelMult = Mathf.Pow(DamageScalePerLevel, adv);

        int finalDamage = Mathf.Max(1, Mathf.FloorToInt(varied * typeMult * levelMult));
        return new AttackResult(caster, target, finalDamage, type);
    }

    /// <summary>
    /// Clamps HP to nonnegative values while alive. FloatRoutine domain.
    /// </summary>
    public static float ClampAlive(float hp)
    {
        return hp < 1f ? 0f : hp;
    }

    /// <summary>
    /// Action point regeneration per tick based on Intelligence, Stamina, and Level. FloatRoutine domain.
    /// </summary>
    public static float APRegen(ActorStats stats)
    {
        return 5f + stats.Intelligence * 0.6f + stats.Stamina * 0.4f + stats.Level * 0.2f;
    }

    /// <summary>
    /// Returns the percentage chance (0–100) of landing a critical hit, based on attacker Stats.
    /// This is the same formula used internally in CalculateHitType for crit determination.
    /// </summary>
    public static float CriticalHitPercent(ActorInstance attacker, ActorInstance opponent)
    {
        // First, compute hit chance to ensure attack connects.
        float accuracy = Accuracy(attacker.Stats);
        float evade = Evasion(opponent.Stats);
        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);

        float hitChance = Mathf.Clamp(accuracy - evade + adv * HitShiftPerLevel, 5f, 95f);
        // If it can't hit at all, crit chance is effectively 0.
        if (hitChance <= 0f)
            return 0f;

        // Crit chance formula from CalculateHitType.
        float baseCrit = 5f;
        float focus = attacker.Stats.Wisdom * 0.35f;
        float luck = Mathf.Min(20f, attacker.Stats.Luck * 0.25f);
        return Mathf.Clamp(baseCrit + focus + luck, 0f, 60f);
    }

    /// <summary>
    /// Returns the percentage chance (0–100) of a glancing blow, based on attacker vs opponent Stats.
    /// This is the same formula used internally in CalculateHitType for glancing determination.
    /// </summary>
    public static float GlancingBlowPercent(ActorInstance attacker, ActorInstance opponent)
    {
        float accuracy = Accuracy(attacker.Stats);
        float evade = Evasion(opponent.Stats);
        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);

        float hitChance = Mathf.Clamp(accuracy - evade + adv * HitShiftPerLevel, 5f, 95f);
        return Mathf.Clamp(100f - hitChance, 0f, 100f);
    }
}
