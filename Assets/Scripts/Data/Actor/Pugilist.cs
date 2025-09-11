using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Pugilist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Pugilist,
                Description = "A disciplined master of martial strikes.",
                Expectations = "Evasive sustained DPS. Performs best in long fights where counters and flurries add up.",
                Lore = "A monk from the Stone Steppe, speechless but eloquent in motion.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 6f,
                    Vitality = 5f,
                    Agility = 9f,
                    Speed = 8f,
                    Stamina = 6f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 4f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.75f,
                    Vitality = 1.0f,
                    Agility = 2.0f,
                    Speed = 2.2f,
                    Stamina = 1.5f,
                    Intelligence = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 1.0f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.0f, Vitality = 0.8f, Agility = 2.3f, Speed = 2.5f, Stamina = 1.2f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 1.2f } },
                    { 10, new StatGrowth { Strength = 1.6f, Vitality = 1.0f, Agility = 2.8f, Speed = 3.0f, Stamina = 1.5f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 1.5f } },
                    { 20, new StatGrowth { Strength = 2.2f, Vitality = 1.5f, Agility = 3.3f, Speed = 3.5f, Stamina = 2.0f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 2.0f } },
                    { 40, new StatGrowth { Strength = 3.2f, Vitality = 2.0f, Agility = 4.5f, Speed = 5.0f, Stamina = 2.5f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 2.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Pugilist}"),
                Details = new ActorDetails
                {
                    Description = "A disciplined master of martial strikes.",
                    Card = "Has a chance to counterattack with [Flurry] when evading an attack.",
                    Trivia = new List<string>
                    {
                        "Once punched a bear",
                        "Trains in silence"
                    }
                }
            };
        }
    }
}
