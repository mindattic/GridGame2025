using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Captain
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Captain00,
                Description = "A captain.",
                Expectations = "Frontline commander with strong early Stats and respectable scaling. Wins sustained trades but does not burst.",
                Lore = "Veteran of two sieges, known for steady hands and short speeches.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10f,
                    Vitality = 10f,
                    Agility = 4f,
                    Speed = 4f,
                    Stamina = 5f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 4f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.5f,
                    Vitality = 1.0f,
                    Agility = 0.8f,
                    Speed = 0.7f,
                    Stamina = 0.9f,
                    Intelligence = 0.4f,
                    Wisdom = 0.5f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 1.5f, Agility = 1.0f, Speed = 0.9f, Stamina = 1.1f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 0.8f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 2.0f, Agility = 1.2f, Speed = 1.1f, Stamina = 1.3f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 1.0f } },
                    { 20, new StatGrowth { Strength = 2.9f, Vitality = 2.5f, Agility = 1.6f, Speed = 1.5f, Stamina = 1.7f, Intelligence = 1.0f, Wisdom = 1.0f, Luck = 1.2f } },
                    { 40, new StatGrowth { Strength = 3.8f, Vitality = 3.5f, Agility = 2.2f, Speed = 2.1f, Stamina = 2.1f, Intelligence = 1.5f, Wisdom = 1.5f, Luck = 1.8f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.41f, -1.5f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.Generate(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Captain00}"),
                Details = new ActorDetails
                {
                    Description = "A captain.",
                    Card = "A captain.",
                    Trivia = new List<string>
                    {
                        "Likes jerky",
                        "Hates Reptiles"
                    }
                }
            };
        }
    }
}
