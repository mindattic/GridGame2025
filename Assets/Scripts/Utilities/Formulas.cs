using Game.Behaviors;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Utilities
{
    public static class Formulas
    {
        private static LogManager log => GameManager.instance.logManager;

        const float baseHitRate = 66.6666f;
        const float armorWeightPenalty = 0.1666f;

        
        public static float StatGrowth(int level)
        {
            return Mathf.Round(100f * (level / 100.0f) * Random.Float(0.4f, 0.8f));
        }

        public static ActorStats RandomStats(int level)
        {
            ActorStats stats = new ActorStats()
            {
                Level = level,
                Strength = StatGrowth(level),
                Vitality = StatGrowth(level),
                Agility = StatGrowth(level),
                Speed = StatGrowth(level),
                Luck = StatGrowth(level),
            };

            stats.MaxHP = level * 3 + StatGrowth(level);
            stats.HP = stats.MaxHP;

            return stats;
        }

        public static float LuckModifier(ActorStats stats)
        {
            var multiplier = stats.Level * 0.01f;
            return Random.Float(1, 1f + stats.Luck * multiplier);
        }

        public static float Accuracy(ActorStats stats)
        {
            var baseAccuracy = baseHitRate + ((stats.Level - 1) / 99.0f) * baseHitRate;
            var multiplier = 2.0f;
            var agi = stats.Agility * multiplier;
            var lck = LuckModifier(stats);
            return Mathf.Round(baseAccuracy + agi + lck);
        }

        public static float Evasion(ActorStats stats)
        {
            var multiplier = 1.0f;
            var spd = stats.Speed * multiplier;
            var lck = LuckModifier(stats);
            var armor = 10 * armorWeightPenalty;

            return Mathf.Round(spd + lck - armor);
        }

        public static bool IsHit(ActorInstance attacker, ActorInstance target)
        {
            var accuracy = Accuracy(attacker.stats);
            var evasion = Evasion(target.stats);
            var d100 = Random.Int(1, 100);
            var isHit = accuracy - evasion >= d100;

            var msg
                = $"{attacker.name} vs {target.name}: "
                + $@"Accuracy(<color=""yellow"">{accuracy}</color>) - "
                + $@"Evasion(<color=""yellow"">{evasion}</color>) "
                + $@"{(isHit ? ">" : "<")} "
                + $@"1d100(<color=""yellow"">{d100}</color>) => "
                + $@"{(isHit ? "Hit" : "Miss")}";
            log.Info(msg);
            return isHit;
        }

        public static bool IsCriticalHit(ActorInstance attacker, ActorInstance target)
        {
            return false;
        }

        public static float Offense(ActorStats stats)
        {
            var multiplier = 2.0f;
            var atk = stats.Strength * multiplier;
            var weapon = 10;
            var weaponModifier = weapon * multiplier;
            var lck = LuckModifier(stats);

            return Mathf.Round(atk + weaponModifier + lck);
        }

        public static float Defense(ActorStats stats)
        {
            var multiplier = 1.0f;
            var def = stats.Vitality * multiplier;
            var armor = 10;
            var armorModifier = armor * 1.0f;
            var lck = LuckModifier(stats);

            return Mathf.Round(def + armorModifier + lck);
        }

        public static int CalculateDamage(ActorInstance attacker, ActorInstance defender)
        {
            var offense = Offense(attacker.stats);
            var defense = Defense(defender.stats);
            var damage = Math.Clamp((int)Math.Round(offense - defense), 1, 999);
            var msg
                = $"{attacker.name} vs {defender.name}: "
                + $@"Offense(<color=""yellow"">{offense}</color>) "
                + $@"- Defense(<color=""yellow"">{defense}</color>) => "
                + $@"Damage(<color=""yellow"">{damage}</color>)";
            log.Info(msg);

            return damage;
        }


        public static int CalculateTurnDelay(ActorStats stats)
        {
            const float baseDelay = 33.3333f;
            var spd = (int)Math.Round(baseDelay / Mathf.Max(stats.Speed, 1));
            return Random.Int(2, Math.Min(spd, 9));
        }


    }
}
