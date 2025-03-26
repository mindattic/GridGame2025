using Assets.Scripts.Models;
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

        public static float LuckModifier(ActorStats stats)
        {
            var multiplier = stats.Level * 0.01f;
            var luckModifier = Random.Float(1, 1f + stats.Luck * multiplier);
            return luckModifier;
        }

        public static float Accuracy(ActorStats stats)
        {
            var baseAccuracy = baseHitRate + ((stats.Level - 1) / 99.0f) * baseHitRate;
            var multiplier = 2.0f;
            var agi = stats.Agility * multiplier;
            var lck = LuckModifier(stats);
            var accuracy = Mathf.FloorToInt(baseAccuracy + agi + lck);
            return accuracy;
        }

        public static float Evasion(ActorStats stats)
        {
            var multiplier = 1.0f;
            var spd = stats.Speed * multiplier;
            var lck = LuckModifier(stats);
            var armor = 10 * armorWeightPenalty;
            var eveasion = Mathf.FloorToInt(spd + lck - armor);
            return eveasion;
        }

        public static bool IsHit(ActorInstance attacker, ActorInstance opponent)
        {
            var accuracy = Accuracy(attacker.stats);
            var evasion = Evasion(opponent.stats);
            var d100 = Random.Int(1, 100);
            var isHit = accuracy - evasion >= d100;
            isHit = true; //DEBUG: It's not fun to miss...
            //var msg
            //    = $"{attacker.name} vs {opponent.name}: "
            //    + $@"Accuracy(<color=""yellow"">{accuracy}</color>) - "
            //    + $@"Evasion(<color=""yellow"">{evasion}</color>) "
            //    + $@"{(isHit ? ">" : "<")} "
            //    + $@"1d100(<color=""yellow"">{d100}</color>) => "
            //    + $@"{(isHit ? "Hit" : "Miss")}";
            //log.Info(msg);
            return isHit;
        }

        public static bool IsCriticalHit(ActorInstance attacker, ActorInstance target)
        {
            return false;
        }

        public static float Health(ActorStats stats)
        {
            return 30 + (stats.Vitality * 5) + (stats.Level * 2);
        }

        public static float Offense(ActorStats stats)
        {
            var multiplier = 2.0f;
            var atk = stats.Strength * multiplier;
            var weapon = 10;
            var weaponModifier = weapon * multiplier;
            var lck = LuckModifier(stats);
            var offense = Mathf.FloorToInt(atk + weaponModifier + lck);
            return offense;
        }

        public static float Defense(ActorStats stats)
        {
            var multiplier = 1.0f;
            var def = stats.Vitality * multiplier;
            var armor = 10;
            var armorModifier = armor * 1.0f;
            var lck = LuckModifier(stats);
            var defense = Mathf.FloorToInt(def + armorModifier + lck);
            return defense;
        }

        public static int CalculateDamage(ActorInstance attacker, ActorInstance defender)
        {
            var offense = Offense(attacker.stats);
            var defense = Defense(defender.stats);
            var damage = Mathf.FloorToInt(offense - defense);
            return damage;
        }



    }
}
