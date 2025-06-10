using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{

    public class BaseStats
    {
        public float Strength;          //Physical damage output
        public float Agility;           //Crit, Dodge, block, parry 
        public float Intelligence;      //Magic damage output 
        public float Wisdom;             //Accuracy, crit chance, and precision
        public float Vitality;          //Max HP and survivability
        public float Stamina;           //AP regen, action economy
        public float Luck;              //Determines random effects 
    }


    [Serializable]
    public class ActorStats: BaseStats
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
            Agility = other.Agility;
            Intelligence = other.Intelligence;  
            Wisdom = other.Wisdom;
            Vitality = other.Vitality;
            Stamina = other.Stamina;
            Luck = other.Luck;
        }
    }


    [Serializable]
    public class StatGrowth : BaseStats
    {
        public StatGrowth() { }

        public StatGrowth(float strength, float agility, float intelligence, float focus, float vitality, float stamina, float luck)
        {
            Strength = strength;
            Agility = agility;
            Intelligence = intelligence;
            Wisdom = focus;
            Vitality = vitality;
            Stamina = stamina;
            Luck = luck;
        }

        public StatGrowth(StatGrowth other)
        {
            Strength = other.Strength;
            Agility = other.Agility;
            Intelligence = other.Intelligence;
            Wisdom = other.Wisdom;
            Vitality = other.Vitality;
            Stamina = other.Stamina;
            Luck = other.Luck;
        }

        public static StatGrowth operator +(StatGrowth a, StatGrowth b) => new StatGrowth
        {
            Strength = a.Strength + b.Strength,
            Agility = a.Agility + b.Agility,
            Intelligence = a.Intelligence + b.Intelligence,
            Wisdom = a.Wisdom + b.Wisdom,
            Vitality = a.Vitality + b.Vitality,
            Stamina = a.Stamina + b.Stamina,
            Luck = a.Luck + b.Luck
        };
    }

    [Serializable]
    public class ActorData
    {
        public int Level = 1;
        public string Character;
        public string Description;

        public ActorStats BaseStats; // Template baseline
        public ActorStats Stats;     // Final calculated stats

        public StatGrowth StatGrowth = new();  // Default per-level growth
        public Dictionary<int, StatGrowth> MilestoneStatGrowth = new(); // Level-based boosts

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

            var status = new ActorStats(BaseStats); // Clone base

            for (int lvl = 2; lvl <= level; lvl++)
            {
                // Always apply default growth
                status.Strength += Mathf.FloorToInt(StatGrowth.Strength);
                status.Agility += Mathf.FloorToInt(StatGrowth.Agility);
                status.Intelligence += Mathf.FloorToInt(StatGrowth.Intelligence);
                status.Wisdom += Mathf.FloorToInt(StatGrowth.Wisdom);
                status.Vitality += Mathf.FloorToInt(StatGrowth.Vitality);
                status.Stamina += Mathf.FloorToInt(StatGrowth.Stamina);
                status.Luck += Mathf.FloorToInt(StatGrowth.Luck);

                // Then apply any milestone boost (if applicable)
                if (MilestoneStatGrowth.TryGetValue(lvl, out var boost))
                {
                    status.Strength += Mathf.FloorToInt(boost.Strength);
                    status.Agility += Mathf.FloorToInt(boost.Agility);
                    status.Intelligence += Mathf.FloorToInt(boost.Intelligence);
                    status.Wisdom += Mathf.FloorToInt(boost.Wisdom);
                    status.Vitality += Mathf.FloorToInt(boost.Vitality);
                    status.Stamina += Mathf.FloorToInt(boost.Stamina);
                    status.Luck += Mathf.FloorToInt(boost.Luck);
                }          
            }

            //Assign current level
            status.Level = level;

            //Calculate health based on stats
            status.HP = Formulas.Health(status);
            status.MaxHP = status.HP;

            return status;
        }
    }

    [Serializable]
    public enum StageCompletionCondition
    {
        DefeatAllEnemies,
        CollectCoins,
        SurviveTurns
    }



  
}
