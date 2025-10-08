using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper; // Added alias
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class Slime02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime02,
                Tags = Tag.Enemy,
                Description = "A thicker, stickier slime.",
                Expectations = "Sometimes survives the opening hit.",
                Lore = "Extra goo, extra stubborn.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 2f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 2f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth(0.1f, 0.15f, 0.1f, 0.1f, 0.15f, 0f, 0f, 0.05f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.2f,0.1f,0.1f,0.2f,0f,0f,0.1f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings { 
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f) 
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime02}"),
                Card = "Barely tanky by slime standards."
            };
        }
    }
}