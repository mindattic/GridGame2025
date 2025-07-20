using Assets.Scripts.Models;
using Game.Behaviors;
using UnityEngine;
using game = GameManagerHelper;

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

public static class Formulas
{
    private static LogManager log => GameManager.instance.logManager;

    public static float LuckModifier(ActorStats stats)
    {
        float multiplier = 0.01f * stats.Level;
        return Random.Float(1f, 1f + stats.Luck * multiplier);
    }

    public static float Accuracy(ActorStats stats)
    {
        float baseAccuracy = 75f + stats.Level * 0.5f;
        float precision = stats.Wisdom * 1.25f;
        float luck = LuckModifier(stats);
        return baseAccuracy + precision + luck;
    }

    public static float Evasion(ActorStats stats)
    {
        float agility = stats.Agility * 2f;
        float staminaBonus = stats.Stamina * 0.5f;
        float luck = LuckModifier(stats);
        return agility + staminaBonus + luck;
    }

    public static bool IsHit(ActorInstance attacker, ActorInstance defender)
    {
        float accuracy = Accuracy(attacker.stats);
        float evade = Evasion(defender.stats);
        float chance = accuracy - evade;
        float roll = Random.Float(0, 100);
        return roll < Mathf.Clamp(chance, 5f, 95f);
    }

    public static bool IsCriticalHit(ActorInstance attacker)
    {
        float baseCrit = 5f;
        float focus = attacker.stats.Wisdom * 0.4f;
        float luck = attacker.stats.Luck * 0.3f;
        float critChance = baseCrit + focus + luck;
        return Random.Float(0, 100) < critChance;
    }

    public static float Health(ActorStats stats)
    {
        return 50 + stats.Vitality * 10f + stats.Level * 2f;
    }

    public static float Offense(ActorStats stats, float weaponPower = 0f)
    {
        float gearModifier = 0f; // reserved for gear modifiers
        float baseDamage = stats.Strength * 2f + weaponPower + gearModifier;
        float luck = LuckModifier(stats);
        return baseDamage * luck;
    }

    public static float Defense(ActorStats stats, float armorRating = 0f)
    {
        float gearModifier = 0f; // reserved for gear modifiers
        float baseDefense = stats.Vitality * 1.5f + stats.Stamina * 0.5f + armorRating + gearModifier;
        float luck = LuckModifier(stats);
        return baseDefense * luck;
    }

    public static float MagicOffense(ActorStats stats)
    {
        float gearModifier = 0f; // reserved for gear modifiers
        float magicPower = stats.Intelligence * 2.5f + stats.Wisdom * 1f + gearModifier;
        float luck = LuckModifier(stats);
        return magicPower * luck;
    }

    public static float MagicResistance(ActorStats stats)
    {
        float gearModifier = 0f; // reserved for gear modifiers
        float resistance = stats.Intelligence * 1.5f + stats.Wisdom * 1f + stats.Stamina * 0.5f + gearModifier;
        float luck = LuckModifier(stats);
        return resistance * luck;
    }

    public static int CalculateDamage(ActorInstance attacker, ActorInstance defender, float weaponPower = 0f, float armorRating = 0f, ElementalDamageType element = ElementalDamageType.Physical, float resistance = 0f)
    {
        float offense = Offense(attacker.stats, weaponPower);
        float defense = Defense(defender.stats, armorRating);
        float rawDamage = offense - defense;

        if (IsCriticalHit(attacker))
            rawDamage *= 1.5f;

        float adjusted = ApplyResistance(rawDamage, resistance);
        return Mathf.Max(Mathf.FloorToInt(adjusted), 1);
    }

    public static int CalculateMagicDamage(ActorInstance caster, ActorInstance target, ElementalDamageType element = ElementalDamageType.Arcane, float resistance = 0f)
    {
        float offense = MagicOffense(caster.stats);
        float resist = MagicResistance(target.stats);
        float damage = offense - resist;

        float adjusted = ApplyResistance(damage, resistance);
        return Mathf.Max(Mathf.FloorToInt(adjusted), 1);
    }

    public static float ApplyResistance(float baseDamage, float resistance)
    {
        if (resistance >= 0)
        {
            return baseDamage * (100f / (100f + resistance));
        }
        else
        {
            return baseDamage * (1f + Mathf.Abs(resistance) / 100f);
        }
    }

    public static float APRegen(ActorStats stats)
    {
        float gearModifier = 0f; // reserved for gear modifiers
        return 5f + stats.Intelligence * 0.6f + stats.Stamina * 0.4f + stats.Level * 0.2f + gearModifier;
    }
}
