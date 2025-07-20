using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class Yeti
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Yeti,
            Description = "A towering beast of cold fury.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 10,
                Vitality = 8,
                Agility = 2,
                Stamina = 5,
                Intelligence = 1,
                Wisdom = 1,
                Luck = 2
            },
            StatGrowth = new StatGrowth
            {
                Strength = 2.0f,
                Vitality = 1.5f,
                Agility = 0.4f,
                Stamina = 1.0f,
                Intelligence = 0.0f,
                Wisdom = 0.2f,
                Luck = 0.5f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5,  new StatGrowth(3.0f, 2.0f, 0.5f, 1.2f, 0.0f, 0.2f, 0.5f) },
                { 10, new StatGrowth(4.0f, 2.5f, 0.6f, 1.5f, 0.0f, 0.3f, 0.6f) },
                { 20, new StatGrowth(5.0f, 3.0f, 0.8f, 2.0f, 0.0f, 0.4f, 0.8f) },
                { 40, new StatGrowth(6.0f, 4.0f, 1.0f, 2.5f, 0.0f, 0.5f, 1.0f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(1.3f, -1f, 0),
                Scale = new Vector3(5f, 5f, 0),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"Actor-Portraits/{CharacterHelper.Yeti}"),
            Details = new ActorDetails
            {
                Description = "A towering beast of cold fury.",
                Card = "Delivers powerful <color=#00FFFF>[Ice]</color> attacks that ignore 25% of defense.",
                Lore = new List<string> { "Hates heat", "Used to be a myth" }
            }
        };
    }
}
