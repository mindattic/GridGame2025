using Assets.Scripts.Models;
using Game.Behaviors;
using UnityEngine;
using Assets.Helpers; // for CombatLogHelper

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
    Weak,
    Miss
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

    // Weak near-miss tuning (A)
    private const float WeakBandBase = 6f;
    private const float WeakBandPerLuck = 0.2f; // per Luck point

    // Outcome multipliers (keep Crit as-is, bump Weak to 0.70 as suggested)
    private const float CritMultiplier = 1.5f;
    private const float WeakMultiplier = 0.70f; // was 0.50

    // Note: Do not keep cached managers here. These helpers must stay pure.

    // ---------------
    // Logging helpers
    // ---------------
    private static void Log(string message)
    {
        CombatLogHelper.Write(message);
    }

    private static string NameOf(ActorInstance a)
    {
        if (a == null) return "<null>";
        if (!string.IsNullOrEmpty(a.characterName)) return a.characterName;
        return a.name;
    }

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
        float tilt = LuckTilt(stats);
        float adjusted = roll * tilt;
        float result = Mathf.Clamp(adjusted, minV, maxV);

        Log($"Variance: range ±{rangeFraction * 100f:F0}% roll={roll:F3} tilt(Luck={stats.Luck})={tilt:F3} -> factor={result:F3}");
        return result;
    }

    /// <summary>
    /// Returns a small additive modifier from Luck.
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
    /// Rolls outcome using a single uniform roll and a narrow near-miss band.
    /// Order: Crit -> Normal -> Weak -> Miss.
    /// Uses float comparisons only; no integer conversions here.
    /// </summary>
    public static HitOutcome CalculateHitType(ActorInstance attacker, ActorInstance opponent)
    {
        float accuracy = Accuracy(attacker.Stats);
        float evade = Evasion(opponent.Stats);
        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);

        float hitChance = Mathf.Clamp(accuracy - evade + adv * HitShiftPerLevel, 5f, 95f);

        // Crit chance (unchanged math), absolute threshold on the roll table
        float baseCrit = 5f;
        float focus = attacker.Stats.Wisdom * 0.35f;
        float luckCrit = Mathf.Min(20f, attacker.Stats.Luck * 0.25f);
        float critChance = Mathf.Clamp(baseCrit + focus + luckCrit, 0f, 60f);

        // Weak near-miss band after Hit threshold
        float weakBand = WeakBandBase + attacker.Stats.Luck * WeakBandPerLuck;
        float effectiveWeakCap = Mathf.Max(0f, 100f - hitChance); // cannot extend past 100
        float effectiveWeakBand = Mathf.Clamp(weakBand, 0f, effectiveWeakCap);

        float roll = RNG.Float(0f, 100f);
        Log($"HitOutcome roll: {NameOf(attacker)} vs {NameOf(opponent)} | Acc={accuracy:F2} Evade={evade:F2} Adv={adv} -> Hit%={hitChance:F2}, Crit%={critChance:F2}, WeakBand={weakBand:F1} (eff {effectiveWeakBand:F1}), Roll={roll:F2}");

        if (roll <= critChance)
        {
            Log("Result: CRITICAL");
            return HitOutcome.Critical;
        }
        else if (roll <= hitChance)
        {
            Log("Result: Normal hit");
            return HitOutcome.Normal;
        }
        else if (roll <= hitChance + effectiveWeakBand)
        {
            Log("Result: Weak (near-miss)");
            return HitOutcome.Weak;
        }
        else
        {
            Log("Result: MISS");
            return HitOutcome.Miss;
        }
    }

    /// <summary>
    /// Derived max health from core Stats.
    /// Pure float math; callers decide if they need rounding for display.
    /// </summary>
    public static float Health(ActorStats stats)
    {
        return Mathf.Floor(50f + stats.Vitality * 10f + stats.Level * 2f);
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

        // (B1) Defense soft cap relative to current Offense
        float effDef = Mathf.Min(def, off * 0.8f);
        float raw = off - effDef;

        float resisted = ApplyResistance(raw, resistance);
        float varied = resisted * SampleVarianceWithLuck(attacker.Stats, 0.20f); // ±20%

        HitOutcome type = CalculateHitType(attacker, opponent);
        float typeMult = type == HitOutcome.Critical ? CritMultiplier : (type == HitOutcome.Weak ? WeakMultiplier : 1f);

        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);
        float levelMult = Mathf.Pow(DamageScalePerLevel, adv);

        // Miss does 0 damage
        int finalDamage = (type == HitOutcome.Miss)
            ? 0
            : Mathf.Max(1, Mathf.FloorToInt(varied * typeMult * levelMult));

        // Compose a single multi-line log entry to capture the full breakdown
        var s =
            $"[PHYS] {NameOf(attacker)} -> {NameOf(opponent)} ({element}, Resist {resistance:+0;-0;0}%)\n" +
            $"  Offense = STR*2 + Weapon = {attacker.Stats.Strength}*2 + {weaponPower} = {off:F2}\n" +
            $"  Defense = VIT*1.5 + STA*0.5 + Armor = {opponent.Stats.Vitality}*1.5 + {opponent.Stats.Stamina}*0.5 + {armorRating} = {def:F2}\n" +
            $"  EffectiveDefense (soft cap 0.8*Off) = min({def:F2}, {off:F2}*0.8) = {effDef:F2}\n" +
            $"  Raw = Off - EffDef = {off:F2} - {effDef:F2} = {raw:F2}\n" +
            $"  After Resistance({resistance:+0;-0;0}%): {resisted:F2}\n" +
            $"  Variance factor applied -> Varied = {varied:F2}\n" +
            $"  HitOutcome: {type} x{(type == HitOutcome.Critical ? CritMultiplier : (type == HitOutcome.Weak ? WeakMultiplier : 1f)):F2}\n" +
            $"  LevelAdvantage: {adv} -> DamageScalePerLevel^{adv} = {levelMult:F3}\n" +
            $"  Final Damage = {(type == HitOutcome.Miss ? 0 : Mathf.FloorToInt(varied * typeMult * levelMult))} {(type == HitOutcome.Miss ? "(MISS)" : string.Empty)}";
        Log(s);

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

        // (B1) Resistance soft cap relative to current MagicOffense
        float effRes = Mathf.Min(res, off * 0.8f);
        float raw = off - effRes;

        float resisted = ApplyResistance(raw, resistance);
        float varied = resisted * SampleVarianceWithLuck(caster.Stats, 0.20f);

        HitOutcome type = CalculateHitType(caster, target);
        float typeMult = type == HitOutcome.Critical ? CritMultiplier : (type == HitOutcome.Weak ? WeakMultiplier : 1f);

        int adv = LevelAdvantage(caster.Stats, target.Stats);
        float levelMult = Mathf.Pow(DamageScalePerLevel, adv);

        int finalDamage = (type == HitOutcome.Miss)
            ? 0
            : Mathf.Max(1, Mathf.FloorToInt(varied * typeMult * levelMult));

        var s =
            $"[MAG] {NameOf(caster)} -> {NameOf(target)} ({element}, Resist {resistance:+0;-0;0}%)\n" +
            $"  MagicOffense = INT*2.5 + WIS*1 = {caster.Stats.Intelligence}*2.5 + {caster.Stats.Wisdom}*1 = {off:F2}\n" +
            $"  MagicResist = INT*1.5 + WIS*1 + STA*0.5 = {target.Stats.Intelligence}*1.5 + {target.Stats.Wisdom}*1 + {target.Stats.Stamina}*0.5 = {res:F2}\n" +
            $"  EffectiveResist (soft cap 0.8*Off) = min({res:F2}, {off:F2}*0.8) = {effRes:F2}\n" +
            $"  Raw = Off - EffRes = {off:F2} - {effRes:F2} = {raw:F2}\n" +
            $"  After Resistance({resistance:+0;-0;0}%): {resisted:F2}\n" +
            $"  Variance factor applied -> Varied = {varied:F2}\n" +
            $"  HitOutcome: {type} x{(type == HitOutcome.Critical ? CritMultiplier : (type == HitOutcome.Weak ? WeakMultiplier : 1f)):F2}\n" +
            $"  LevelAdvantage: {adv} -> DamageScalePerLevel^{adv} = {levelMult:F3}\n" +
            $"  Final Damage = {(type == HitOutcome.Miss ? 0 : Mathf.FloorToInt(varied * typeMult * levelMult))} {(type == HitOutcome.Miss ? "(MISS)" : string.Empty)}";
        Log(s);

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
    /// Matches CalculateHitType's crit determination (absolute threshold on roll).
    /// </summary>
    public static float CriticalHitPercent(ActorInstance attacker, ActorInstance opponent)
    {
        // Crit chance formula from CalculateHitType.
        float baseCrit = 5f;
        float focus = attacker.Stats.Wisdom * 0.35f;
        float luck = Mathf.Min(20f, attacker.Stats.Luck * 0.25f);
        return Mathf.Clamp(baseCrit + focus + luck, 0f, 60f);
    }

    /// <summary>
    /// Returns the percentage chance (0–100) of a weak (glancing) hit using the near-miss band.
    /// This approximates the band size after hit chance.
    /// </summary>
    public static float GlancingBlowPercent(ActorInstance attacker, ActorInstance opponent)
    {
        float accuracy = Accuracy(attacker.Stats);
        float evade = Evasion(opponent.Stats);
        int adv = LevelAdvantage(attacker.Stats, opponent.Stats);

        float hitChance = Mathf.Clamp(accuracy - evade + adv * HitShiftPerLevel, 5f, 95f);
        float weakBand = WeakBandBase + attacker.Stats.Luck * WeakBandPerLuck;
        float effectiveWeakCap = Mathf.Max(0f, 100f - hitChance);
        float effectiveWeakBand = Mathf.Clamp(weakBand, 0f, effectiveWeakCap);
        return Mathf.Clamp(effectiveWeakBand, 0f, 100f);
    }
}
