using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class BaseStats
    {
        public float Strength;      // Physical damage output
        public float Vitality;      // Max HP and survivability
        public float Agility;       // Crit, Dodge, block, parry 
        public float Stamina;       // AP regen, action economy
        public float Intelligence;  // Magic damage output 
        public float Wisdom;        // Accuracy, crit chance, and precision  
        public float Luck;          // Determines random effects 
    }

    [Serializable]
    public class ActorStats : BaseStats
    {
        public float Level = 1;
        public float PreviousHP;
        public float HP;
        public float MaxHP;
        public float PreviousAP;
        public float AP;
        public float MaxAP;

        public ActorStats() { }

        public ActorStats(ActorStats other)
        {
            Level = other.Level;
            PreviousHP = other.HP;
            HP = other.HP;
            MaxHP = other.MaxHP;
            PreviousAP = 0;
            AP = 0;
            MaxAP = 100;

            Strength = other.Strength;
            Vitality = other.Vitality;
            Agility = other.Agility;
            Stamina = other.Stamina;
            Intelligence = other.Intelligence;
            Wisdom = other.Wisdom;
            Luck = other.Luck;
        }
    }

    [Serializable]
    public class StatGrowth : BaseStats
    {
        public StatGrowth() { }

        public StatGrowth(float strength, float vitality, float agility, float stamina, float intelligence, float wisdom, float luck)
        {
            Strength = strength;
            Vitality = vitality;
            Agility = agility;
            Stamina = stamina;
            Intelligence = intelligence;
            Wisdom = wisdom;
            Luck = luck;
        }

        public StatGrowth(StatGrowth other)
        {
            Strength = other.Strength;
            Vitality = other.Vitality;
            Agility = other.Agility;
            Stamina = other.Stamina;
            Intelligence = other.Intelligence;
            Wisdom = other.Wisdom;
            Luck = other.Luck;
        }

        public static StatGrowth operator +(StatGrowth a, StatGrowth b) => new StatGrowth
        {
            Strength = a.Strength + b.Strength,
            Vitality = a.Vitality + b.Vitality,
            Agility = a.Agility + b.Agility,
            Stamina = a.Stamina + b.Stamina,
            Intelligence = a.Intelligence + b.Intelligence,
            Wisdom = a.Wisdom + b.Wisdom,
            Luck = a.Luck + b.Luck
        };
    }

    [Serializable]
    public class ActorData
    {
        public int Level = 1;
        public string Character;
        public string Description;

        public ActorStats BaseStats;
        public ActorStats Stats;

        public StatGrowth StatGrowth = new();
        public Dictionary<int, StatGrowth> MilestoneStatGrowth = new();

        public ThumbnailSettings ThumbnailSettings;
        public ActorDetails Details;

        public Sprite Portrait;

        public ActorData() { }

        public ActorData(ActorData other)
        {
            Level = other.Level;
            Character = other.Character;
            Description = other.Description;

            BaseStats = other.BaseStats != null ? new ActorStats(other.BaseStats) : new ActorStats();
            Stats = GetStats(Level);

            StatGrowth = other.StatGrowth != null ? new StatGrowth(other.StatGrowth) : new StatGrowth();

            MilestoneStatGrowth = other.MilestoneStatGrowth != null
                ? new Dictionary<int, StatGrowth>(other.MilestoneStatGrowth)
                : new Dictionary<int, StatGrowth>();

            ThumbnailSettings = other.ThumbnailSettings != null ? new ThumbnailSettings(other.ThumbnailSettings) : new ThumbnailSettings();
            Details = other.Details != null ? new ActorDetails(other.Details) : new ActorDetails();
        }

        public void RecalculateStats()
        {
            Stats = GetStats(Level);
        }

        public ActorStats GetStats(int level)
        {
            if (level < 1) level = 1;

            var status = new ActorStats(BaseStats);

            for (int lvl = 2; lvl <= level; lvl++)
            {
                status.Strength += Mathf.FloorToInt(StatGrowth.Strength);
                status.Vitality += Mathf.FloorToInt(StatGrowth.Vitality);
                status.Agility += Mathf.FloorToInt(StatGrowth.Agility);
                status.Stamina += Mathf.FloorToInt(StatGrowth.Stamina);
                status.Intelligence += Mathf.FloorToInt(StatGrowth.Intelligence);
                status.Wisdom += Mathf.FloorToInt(StatGrowth.Wisdom);
                status.Luck += Mathf.FloorToInt(StatGrowth.Luck);

                if (MilestoneStatGrowth.TryGetValue(lvl, out var boost))
                {
                    status.Strength += Mathf.FloorToInt(boost.Strength);
                    status.Vitality += Mathf.FloorToInt(boost.Vitality);
                    status.Agility += Mathf.FloorToInt(boost.Agility);
                    status.Stamina += Mathf.FloorToInt(boost.Stamina);
                    status.Intelligence += Mathf.FloorToInt(boost.Intelligence);
                    status.Wisdom += Mathf.FloorToInt(boost.Wisdom);
                    status.Luck += Mathf.FloorToInt(boost.Luck);
                }
            }

            status.Level = level;
            status.HP = Formulas.Health(status);
            status.MaxHP = status.HP;

            return status;
        }
    }
}
