using Assets.Scripts.Models;
using Game.Behaviors;
using UnityEngine;

public static class Formulas
{
    private static LogManager log => GameManager.instance.logManager;

    const float baseHitRate = 66.6666f;
    const float armorWeightPenalty = 0.1666f;

    public static float LuckModifier(ActorStats stats)
    {
        float multiplier = stats.Level * 0.01f;
        return Random.Float(1, 1f + stats.Luck * multiplier);
    }

    public static float Accuracy(ActorStats stats)
    {
        float baseAccuracy = baseHitRate + ((stats.Level - 1) / 99.0f) * baseHitRate;
        float focus = stats.Wisdom * 1.5f;
        float luck = LuckModifier(stats);
        return Mathf.FloorToInt(baseAccuracy + focus + luck);
    }

    public static float Evade(ActorStats stats)
    {
        float agility = stats.Agility * 2f;
        float stamina = stats.Stamina * 0.5f; // helps mitigate fatigue penalties
        float luck = LuckModifier(stats);
        float armorPenalty = 10 * armorWeightPenalty; // assumed fixed armor weight for now
        return Mathf.FloorToInt(agility + stamina + luck - armorPenalty);
    }

    public static bool IsHit(ActorInstance attacker, ActorInstance defender)
    {
        return true;

        float accuracy = Accuracy(attacker.stats);
        float evade = Evade(defender.stats);
        int d100 = Random.Int(1, 100);
        bool isHit = (accuracy - evade) >= d100;
        return isHit;
    }

    public static bool IsCriticalHit(ActorInstance attacker, ActorInstance defender)
    {
        float baseCritChance = 5f; // base 5% chance
        float focusBonus = attacker.stats.Wisdom * 0.5f;
        float luckBonus = attacker.stats.Luck * 0.3f;
        float critChance = baseCritChance + focusBonus + luckBonus;
        return Random.Float(0, 100) < critChance;
    }

    public static float Health(ActorStats stats)
    {
        return 30 + (stats.Vitality * 5f) + (stats.Level * 2f);
    }

    public static float Offense(ActorStats stats)
    {

        float weapon = 100; // placeholder
        float baseDamage = stats.Strength * 2f;
        float focusBonus = stats.Wisdom * 0.5f; // accuracy improves quality of attackResult
        float luck = LuckModifier(stats);
        return Mathf.FloorToInt(baseDamage + weapon + focusBonus + luck);
    }

    public static float Defense(ActorStats stats)
    {
        float armor = 10; // placeholder
        float vitality = stats.Vitality * 1.5f;
        float staminaBonus = stats.Stamina * 0.5f;
        float luck = LuckModifier(stats);
        return Mathf.FloorToInt(vitality + armor + staminaBonus + luck);
    }

    public static float MagicOffense(ActorStats stats)
    {
        float intelligence = stats.Intelligence * 2f;
        float focus = stats.Wisdom * 1f;
        float luck = LuckModifier(stats);
        return Mathf.FloorToInt(intelligence + focus + luck);
    }

    public static float MagicResistance(ActorStats stats)
    {
        float intelligence = stats.Intelligence * 1.5f;
        float stamina = stats.Stamina * 0.5f;
        float luck = LuckModifier(stats);
        return Mathf.FloorToInt(intelligence + stamina + luck);
    }

    public static int CalculateDamage(ActorInstance attacker, ActorInstance defender)
    {
        float offense = Offense(attacker.stats);
        float defense = Defense(defender.stats);
        float rawDamage = offense - defense;
        return Mathf.Max(Mathf.FloorToInt(rawDamage), 1); // Never less than 1 damage
    }

    public static int CalculateMagicDamage(ActorInstance caster, ActorInstance target)
    {
        float magic = MagicOffense(caster.stats);
        float resist = MagicResistance(target.stats);
        float damage = magic - resist;
        return Mathf.Max(Mathf.FloorToInt(damage), 1);
    }

    public static float APRegen(ActorStats stats)
    {
        return 5f + (stats.Stamina * 0.5f) + (stats.Level * 0.2f);
    }
}
