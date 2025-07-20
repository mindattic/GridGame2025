using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class Sellsword
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Sellsword,
            Description = "A blade-for-hire who fights for coin.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 6,
                Vitality = 6,
                Agility = 5,
                Stamina = 4,
                Intelligence = 3,
                Wisdom = 3,
                Luck = 5
            },
            StatGrowth = new StatGrowth
            {
                Strength = 1.6f,
                Vitality = 1.6f,
                Agility = 1.2f,
                Stamina = 1.0f,
                Intelligence = 0.6f,
                Wisdom = 0.6f,
                Luck = 1.2f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(2.0f, 2.0f, 1.5f, 1.2f, 0.8f, 0.8f, 1.5f) },
                { 10, new StatGrowth(2.5f, 2.5f, 1.8f, 1.5f, 1.0f, 1.0f, 2.0f) },
                { 20, new StatGrowth(3.5f, 3.5f, 2.2f, 2.0f, 1.5f, 1.5f, 2.5f) },
                { 40, new StatGrowth(4.5f, 4.5f, 3.0f, 2.5f, 2.0f, 2.0f, 3.0f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, -1.4f, 0f),
                Scale = new Vector3(5f, 5f, 0f)
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"Actor-Portraits/{CharacterHelper.Sellsword}"),
            Details = new ActorDetails
            {
                Description = "A blade-for-hire who fights for coin.",
                Card = "Can equip a wide variety of weapons. Passive chance to gain <color=#00FFAA>[Extra Gold]</color> after battle.",
                Lore = new List<string>
                {
                    "Loyal only to coin",
                    "Prefers contracts over causes"
                }
            }
        };
    }
}
