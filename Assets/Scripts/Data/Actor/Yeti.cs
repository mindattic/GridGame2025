using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Yeti
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Yeti,
                Description = "A towering beast of cold fury.",
                Expectations = "Heavy hitter with armor-chunking swings. Slow turns but high impact.",
                Lore = "Snow moves around it as if afraid to touch.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10f,
                    Vitality = 2f,
                    Agility = 1f,
                    Speed = 2f,
                    Stamina = 1f,
                    Intelligence = 1f,
                    Wisdom = 1f,
                    Luck = 2f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.5f,
                    Vitality = 0.3f,
                    Agility = 0.2f,
                    Speed = 0.4f,
                    Stamina = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 0.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.2f, Vitality = 0.425f, Agility = 0.25f, Speed = 0.5f, Stamina = 0.25f, Intelligence = 0.0f, Wisdom = 0.2f, Luck = 0.5f } }, // 1.7 -> 0.425, 1.0 -> 0.25
                    { 10, new StatGrowth { Strength = 3.0f, Vitality = 0.525f, Agility = 0.30f, Speed = 0.6f, Stamina = 0.3f,  Intelligence = 0.0f, Wisdom = 0.3f, Luck = 0.6f } }, // 2.1 -> 0.525, 1.2 -> 0.3
                    { 20, new StatGrowth { Strength = 3.8f, Vitality = 0.625f, Agility = 0.40f, Speed = 0.8f, Stamina = 0.4f,  Intelligence = 0.0f, Wisdom = 0.4f, Luck = 0.8f } }, // 2.5 -> 0.625, 1.6 -> 0.4
                    { 40, new StatGrowth { Strength = 4.8f, Vitality = 0.8f,   Agility = 0.50f, Speed = 1.0f, Stamina = 0.5f,  Intelligence = 0.0f, Wisdom = 0.5f, Luck = 1.0f } }  // 3.2 -> 0.8, 2.0 -> 0.5
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(1.3f, -1.0f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Yeti}"),
                Card = "Delivers powerful [Ice] attacks that ignore 25% of defense.",
                Trivia = new List<string>
                    {
                        "Hates heat",
                        "Used to be a myth"
                    }
            };
        }
    }
}
