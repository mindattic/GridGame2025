using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Paladin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Paladin,
                Description = "A holy warrior clad in armor.",
                Expectations = "Durable frontliner with moderate DPS and high accuracy. Expects to outlast and win long trades.",
                Lore = "Knight of the Lightbearer Council, shield raised against every darkness.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 7f,
                    Vitality = 8f,
                    Agility = 2f,
                    Speed = 3f,
                    Stamina = 6f,
                    Intelligence = 3f,
                    Wisdom = 6f,
                    Luck = 3f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.4f,
                    Vitality = 1.9f,
                    Agility = 0.4f,
                    Speed = 0.5f,
                    Stamina = 1.4f,
                    Intelligence = 0.6f,
                    Wisdom = 1.5f,
                    Luck = 0.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.9f, Vitality = 1.9f, Agility = 0.4f, Speed = 0.5f, Stamina = 1.4f, Intelligence = 0.5f, Wisdom = 1.2f, Luck = 0.5f } },
                    { 10, new StatGrowth { Strength = 1.2f, Vitality = 2.3f, Agility = 0.6f, Speed = 0.8f, Stamina = 1.8f, Intelligence = 1.0f, Wisdom = 1.6f, Luck = 1.0f } },
                    { 20, new StatGrowth { Strength = 1.7f, Vitality = 2.8f, Agility = 0.9f, Speed = 1.0f, Stamina = 2.3f, Intelligence = 1.5f, Wisdom = 2.0f, Luck = 1.5f } },
                    { 40, new StatGrowth { Strength = 2.7f, Vitality = 3.8f, Agility = 1.2f, Speed = 1.5f, Stamina = 3.2f, Intelligence = 2.0f, Wisdom = 2.8f, Luck = 2.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Paladin}"),
                Details = new ActorDetails
                {
                    Description = "A holy warrior clad in armor.",
                    Card = "Shields nearby allies with [Radiant Guard]. Takes reduced [Physical] and [Dark] damage.",

                }
            };
        }
    }
}
