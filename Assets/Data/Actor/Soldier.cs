using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
public static class Soldier00
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Soldier00,
            Description = "A low-ranked fort guard.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 3,
                Vitality = 2,
                Agility = 2,
                Stamina = 2,
                Intelligence = 1,
                Wisdom = 1,
                Luck = 1
            },
            StatGrowth = new StatGrowth(0.5f, 0.4f, 0.3f, 0.2f, 0.2f, 0.2f, 0.2f),
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(0.6f, 0.5f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f) },
                { 10, new StatGrowth(0.8f, 0.6f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.71f, -1.5f, 0f),
                Scale = new Vector3(5f, 5f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier00}"),
            Details = new ActorDetails
            {
                Description = "A low-ranked fort guard.",
                Card = "Basic soldier. Vulnerable but alert.",
                Lore = new List<string> { "Fresh recruit", "Sleeps in armor" }
            }
        };
    }
}

public static class Soldier01
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Soldier01,
            Description = "A rookie trying too hard.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 4,
                Vitality = 1,
                Agility = 3,
                Stamina = 1,
                Intelligence = 1,
                Wisdom = 1,
                Luck = 2
            },
            StatGrowth = new StatGrowth(0.6f, 0.3f, 0.5f, 0.2f, 0.2f, 0.2f, 0.3f),
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(0.7f, 0.3f, 0.6f, 0.3f, 0.3f, 0.3f, 0.4f) },
                { 10, new StatGrowth(0.9f, 0.4f, 0.7f, 0.4f, 0.4f, 0.4f, 0.5f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, -1.4f, 0f),
                Scale = new Vector3(5f, 5f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier01}"),
            Details = new ActorDetails
            {
                Description = "A rookie trying too hard.",
                Card = "Faster than most soldiers, but fragile.",
                Lore = new List<string>
                {
                    "Broke 3 spears in training",
                    "Carries lucky bone charm"
                }
            }
        };
    }
}

public static class Soldier02
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Soldier02,
            Description = "A jittery scout with sharp eyes.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 2,
                Vitality = 2,
                Agility = 4,
                Stamina = 2,
                Intelligence = 2,
                Wisdom = 1,
                Luck = 2
            },
            StatGrowth = new StatGrowth(0.3f, 0.3f, 0.6f, 0.3f, 0.3f, 0.2f, 0.4f),
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(0.5f, 0.3f, 0.7f, 0.3f, 0.3f, 0.3f, 0.4f) },
                { 10, new StatGrowth(0.7f, 0.4f, 0.8f, 0.4f, 0.4f, 0.4f, 0.5f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, -0.55f, 0f),
                Scale = new Vector3(3f, 3f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier02}"),
            Details = new ActorDetails
            {
                Description = "A jittery scout with sharp eyes.",
                Card = "Fast to act, quick to flee.",
                Lore = new List<string> { "Sniffs everything", "Allergic to slime" }
            }
        };
    }
}

public static class Soldier03
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Soldier03,
            Description = "A washed-up old fighter.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 3,
                Vitality = 3,
                Agility = 1,
                Stamina = 2,
                Intelligence = 1,
                Wisdom = 2,
                Luck = 1
            },
            StatGrowth = new StatGrowth(0.4f, 0.5f, 0.2f, 0.3f, 0.4f, 0.2f, 0.3f),
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(0.5f, 0.6f, 0.2f, 0.3f, 0.5f, 0.3f, 0.3f) },
                { 10, new StatGrowth(0.6f, 0.7f, 0.3f, 0.4f, 0.6f, 0.3f, 0.4f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.7f, -1.4f, 0f),
                Scale = new Vector3(5f, 5f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier03}"),
            Details = new ActorDetails
            {
                Description = "A washed-up old fighter.",
                Card = "Takes a hit better than he gives one.",
                Lore = new List<string> { "Once held rank", "Talks in riddles" }
            }
        };
    }
}