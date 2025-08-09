namespace Assets.Data.Actor
{
    using Assets.Scripts.Models;
    using System.Collections.Generic;
    using UnityEngine;
    using g = Assets.Helpers.GameHelper;
    public static class Captain
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Captain00,
                Description = "A captain.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10,
                    Vitality = 10,
                    Agility = 4,
                    Stamina = 5,
                    Intelligence = 2,
                    Wisdom = 2,
                    Luck = 4
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.2f,
                    Vitality = 0.3f,
                    Agility = 0.2f,
                    Stamina = 0.2f,
                    Intelligence = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 0.2f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 0.5f, Vitality = 0.5f, Agility = 0.5f, Stamina = 0.5f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 0.5f } },
                    { 10, new StatGrowth { Strength = 0.8f, Vitality = 0.8f, Agility = 0.8f, Stamina = 0.8f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 0.8f } },
                    { 20, new StatGrowth { Strength = 1.0f, Vitality = 1.0f, Agility = 1.0f, Stamina = 1.0f, Intelligence = 1.0f, Wisdom = 1.0f, Luck = 1.0f } },
                    { 40, new StatGrowth { Strength = 1.5f, Vitality = 1.5f, Agility = 1.5f, Stamina = 1.5f, Intelligence = 1.5f, Wisdom = 1.5f, Luck = 1.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.41f, -1.5f, 0f),
                    Scale = new Vector3(5f, 5f, 0f),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Captain00}"),
                Details = new ActorDetails
                {
                    Description = "A captain.",
                    Card = "A captain.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            };
        }
    }
}
