using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class Paladin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Paladin,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A holy warrior clad in armor.",
                Expectations = "Durable frontliner with moderate DPS and high accuracy. Expects to outlast and win long trades.",
                Lore = "Knight of the Lightbearer Council, shield raised against every darkness.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +2
                    Vitality = 8f,
                    Agility = 2f,
                    Speed = 4f,     // +1
                    Stamina = 6f,
                    Intelligence = 3f,
                    Wisdom = 6f,
                    Luck = 4f       // +1
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.7f, // was 1.4f
                    Vitality = 1.9f,
                    Agility = 0.4f,
                    Speed = 0.6f,    // was 0.5f
                    Stamina = 1.4f,
                    Intelligence = 0.6f,
                    Wisdom = 1.5f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.2f, Vitality = 1.9f, Agility = 0.4f, Speed = 0.6f, Stamina = 1.4f, Intelligence = 0.5f, Wisdom = 1.2f, Luck = 0.6f } },
                    { 10, new StatGrowth { Strength = 1.5f, Vitality = 2.3f, Agility = 0.6f, Speed = 0.9f, Stamina = 1.8f, Intelligence = 1.0f, Wisdom = 1.6f, Luck = 1.1f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 2.8f, Agility = 0.9f, Speed = 1.1f, Stamina = 2.3f, Intelligence = 1.5f, Wisdom = 2.0f, Luck = 1.6f } },
                    { 40, new StatGrowth { Strength = 3.0f, Vitality = 3.8f, Agility = 1.2f, Speed = 1.6f, Stamina = 3.2f, Intelligence = 2.0f, Wisdom = 2.8f, Luck = 2.2f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Paladin}"),
                Card = "Shields nearby allies with [Radiant Guard]. Takes reduced [Physical] and [Dark] damage.",
            };
        }
    }
}
