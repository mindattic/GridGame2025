using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
public static class Thief
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Thief,
            Description = "A nimble rogue with sticky fingers.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 5,
                Vitality = 2,
                Agility = 6,
                Stamina = 2,
                Intelligence = 2,
                Wisdom = 2,
                Luck = 7
            },
            StatGrowth = new StatGrowth
            {
                Strength = 0.4f,
                Vitality = 0.3f,
                Agility = 1.5f,
                Stamina = 0.5f,
                Intelligence = 0.4f,
                Wisdom = 0.4f,
                Luck = 2.0f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(0.5f, 0.3f, 2.0f, 0.6f, 0.5f, 0.5f, 2.5f) },
                { 10, new StatGrowth(0.6f, 0.4f, 2.5f, 0.7f, 0.6f, 0.6f, 3.0f) },
                { 20, new StatGrowth(0.8f, 0.5f, 3.0f, 0.8f, 0.8f, 0.8f, 4.0f) },
                { 40, new StatGrowth(1.0f, 0.7f, 3.5f, 1.0f, 1.0f, 1.0f, 5.0f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.61f, -1.56f, 0f),
                Scale = new Vector3(5.3f, 5.3f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Thief}"),
            Details = new ActorDetails
            {
                Description = "A nimble rogue with sticky fingers.",
                Card = "Has a high chance to evade. Can steal from enemies.",
                Lore = new List<string> { "Loves coin", "Allergic to jail cells" }
            }
        };
    }
}
