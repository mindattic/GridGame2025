using Assets.Scripts.Models;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ActorData
{
    public int Level = 1;
    public string Character;
    public string Description;
    public string Expectations;
    public string Lore;

    public ActorGroup Groups { get; set; }



    public ActorStats BaseStats;
    public ActorStats Stats;

    public StatGrowth StatGrowth = new StatGrowth();
    public Dictionary<int, StatGrowth> MilestoneStatGrowth = new Dictionary<int, StatGrowth>();

    public ThumbnailSettings ThumbnailSettings;
    public ActorDetails Details;
    public Sprite Portrait;

    private const int DefaultMilestoneWindow = 5;

    public ActorData() { }

    public ActorData(ActorData other)
    {
        if (other == null) return;

        Level = other.Level;
        Character = other.Character;
        Description = other.Description;

        BaseStats = other.BaseStats != null ? new ActorStats(other.BaseStats) : new ActorStats();
        StatGrowth = other.StatGrowth != null ? new StatGrowth(other.StatGrowth) : new StatGrowth();

        MilestoneStatGrowth = other.MilestoneStatGrowth != null
            ? new Dictionary<int, StatGrowth>(other.MilestoneStatGrowth)
            : new Dictionary<int, StatGrowth>();

        ThumbnailSettings = other.ThumbnailSettings != null
            ? new ThumbnailSettings(other.ThumbnailSettings)
            : new ThumbnailSettings();

        Details = other.Details != null
            ? new ActorDetails(other.Details)
            : new ActorDetails();

        Stats = GetStats(Level);
    }

    public void RecalculateStats()
    {
        Stats = GetStats(Level);
    }

    public ActorStats GetStats(int level)
    {
        return GetStatsWithOptions(level, DefaultMilestoneWindow, true);
    }

    public ActorStats GetStatsLegacy(int level)
    {
        return GetStatsWithOptions(level, 1, false);
    }

    public ActorStats GetStatsWithOptions(int level, int milestoneWindow, bool distributeMilestones)
    {
        if (level < 1) level = 1;
        if (milestoneWindow < 1) milestoneWindow = 1;

        var stats = new ActorStats
        {
            Level = 1f,
            Strength = BaseStats != null ? BaseStats.Strength : 0f,
            Vitality = BaseStats != null ? BaseStats.Vitality : 0f,
            Agility = BaseStats != null ? BaseStats.Agility : 0f,
            Speed = BaseStats != null ? BaseStats.Speed : 0f,
            Stamina = BaseStats != null ? BaseStats.Stamina : 0f,
            Intelligence = BaseStats != null ? BaseStats.Intelligence : 0f,
            Wisdom = BaseStats != null ? BaseStats.Wisdom : 0f,
            Luck = BaseStats != null ? BaseStats.Luck : 0f,
            PreviousHP = 0f,
            HP = 0f,
            MaxHP = 0f,
            PreviousAP = 0f,
            AP = 0f,
            MaxAP = 100f
        };

        var distributed = new List<(int start, int end, StatGrowth perLevel)>();
        if (distributeMilestones && MilestoneStatGrowth != null)
        {
            foreach (var kvp in MilestoneStatGrowth)
            {
                var bonus = kvp.Value ?? new StatGrowth();
                int start = kvp.Key;
                int end = start + milestoneWindow - 1;

                var per = new StatGrowth
                {
                    Strength = bonus.Strength / milestoneWindow,
                    Vitality = bonus.Vitality / milestoneWindow,
                    Agility = bonus.Agility / milestoneWindow,
                    Speed = bonus.Speed / milestoneWindow,
                    Stamina = bonus.Stamina / milestoneWindow,
                    Intelligence = bonus.Intelligence / milestoneWindow,
                    Wisdom = bonus.Wisdom / milestoneWindow,
                    Luck = bonus.Luck / milestoneWindow
                };

                distributed.Add((start, end, per));
            }
        }

        // Accumulate exact fractional growth
        for (int L = 2; L <= level; L++)
        {
            stats.Level = L;

            stats.Strength += StatGrowth != null ? StatGrowth.Strength : 0f;
            stats.Vitality += StatGrowth != null ? StatGrowth.Vitality : 0f;
            stats.Agility += StatGrowth != null ? StatGrowth.Agility : 0f;
            stats.Speed += StatGrowth != null ? StatGrowth.Speed : 0f;
            stats.Stamina += StatGrowth != null ? StatGrowth.Stamina : 0f;
            stats.Intelligence += StatGrowth != null ? StatGrowth.Intelligence : 0f;
            stats.Wisdom += StatGrowth != null ? StatGrowth.Wisdom : 0f;
            stats.Luck += StatGrowth != null ? StatGrowth.Luck : 0f;

            if (distributeMilestones)
            {
                foreach (var slice in distributed)
                {
                    if (L >= slice.start && L <= slice.end)
                    {
                        stats.Strength += slice.perLevel.Strength;
                        stats.Vitality += slice.perLevel.Vitality;
                        stats.Agility += slice.perLevel.Agility;
                        stats.Speed += slice.perLevel.Speed;
                        stats.Stamina += slice.perLevel.Stamina;
                        stats.Intelligence += slice.perLevel.Intelligence;
                        stats.Wisdom += slice.perLevel.Wisdom;
                        stats.Luck += slice.perLevel.Luck;
                    }
                }
            }
            else if (MilestoneStatGrowth != null && MilestoneStatGrowth.TryGetValue(L, out var instant))
            {
                stats.Strength += instant.Strength;
                stats.Vitality += instant.Vitality;
                stats.Agility += instant.Agility;
                stats.Speed += instant.Speed;
                stats.Stamina += instant.Stamina;
                stats.Intelligence += instant.Intelligence;
                stats.Wisdom += instant.Wisdom;
                stats.Luck += instant.Luck;
            }
        }

        // Floor once at the end to avoid shaving each level
        stats.Strength = Mathf.Floor(stats.Strength);
        stats.Vitality = Mathf.Floor(stats.Vitality);
        stats.Agility = Mathf.Floor(stats.Agility);
        stats.Speed = Mathf.Floor(stats.Speed);
        stats.Stamina = Mathf.Floor(stats.Stamina);
        stats.Intelligence = Mathf.Floor(stats.Intelligence);
        stats.Wisdom = Mathf.Floor(stats.Wisdom);
        stats.Luck = Mathf.Floor(stats.Luck);

        stats.HP = Formulas.Health(stats);
        stats.MaxHP = stats.HP;

        if (stats.HP < 1f) stats.HP = 0f;

        return stats;
    }

    /// <summary>
    /// Returns true if the actor has all groups in mask.
    /// </summary>
    public bool InGroups(ActorGroup mask) => (Groups & mask) == mask;
    public void AddGroups(ActorGroup groups) => Groups |= groups;
    public void RemoveGroups(ActorGroup groups) => Groups &= ~groups;

}