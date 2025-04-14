using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{

    [Serializable]
    public class ActorStats
    {
        public float Level = 1;
        public float PreviousHP;
        public float HP;
        public float MaxHP;
        public float PreviousAP;
        public float AP;
        public float MaxAP;
        public float Strength;
        public float Vitality;
        public float Agility;
        public float Speed;
        public float Luck;

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
            Speed = other.Speed;
            Luck = other.Luck;
        }
    }


    [Serializable]
    public class StatGrowth
    {
        public float Strength;
        public float Vitality;
        public float Agility;
        public float Speed;
        public float Luck;

        public StatGrowth() { }

        public StatGrowth(float str, float vit, float agi, float spd, float luck)
        {
            Strength = str;
            Vitality = vit;
            Agility = agi;
            Speed = spd;
            Luck = luck;
        }

        public StatGrowth(StatGrowth other)
        {
            Strength = other.Strength;
            Vitality = other.Vitality;
            Agility = other.Agility;
            Speed = other.Speed;
            Luck = other.Luck;
        }

        public static StatGrowth operator +(StatGrowth a, StatGrowth b) => new StatGrowth
        {
            Strength = a.Strength + b.Strength,
            Vitality = a.Vitality + b.Vitality,
            Agility = a.Agility + b.Agility,
            Speed = a.Speed + b.Speed,
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
                status.Vitality += Mathf.FloorToInt(StatGrowth.Vitality);
                status.Agility += Mathf.FloorToInt(StatGrowth.Agility);
                status.Speed += Mathf.FloorToInt(StatGrowth.Speed);
                status.Luck += Mathf.FloorToInt(StatGrowth.Luck);

                // Then apply any milestone boost (if applicable)
                if (MilestoneStatGrowth.TryGetValue(lvl, out var boost))
                {
                    status.Strength += Mathf.FloorToInt(boost.Strength);
                    status.Vitality += Mathf.FloorToInt(boost.Vitality);
                    status.Agility += Mathf.FloorToInt(boost.Agility);
                    status.Speed += Mathf.FloorToInt(boost.Speed);
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
    public class VFXData
    {
        public VFXData() { }

        public VFXData(VFXData other)
        {
            Name = other.Name;
            RelativeOffset = other.RelativeOffset;
            AngularRotation = other.AngularRotation;
            RelativeScale = other.RelativeScale;
            Delay = other.Delay;
            Duration = other.Duration;
            IsLoop = other.IsLoop;
            Prefab = other.Prefab;
        }

        public string Name;
        public Vector3 RelativeOffset;
        public Vector3 AngularRotation;
        public Vector3 RelativeScale;
        public float Delay;
        public float Duration;
        public bool IsLoop;
        public GameObject Prefab;
    }



    [Serializable]
    public enum StageCompletionCondition
    {
        DefeatAllEnemies,
        CollectCoins,
        SurviveTurns
    }



  
}
