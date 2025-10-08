using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class Slime00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime00,
                Tags = Tag.Enemy,
                Description = "A tiny, wobbling blob.",
                Expectations = "Falls over to a stiff breeze.",
                Lore = "Jellied life with no ambition.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth(0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0f, 0f, 0f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.15f,0.15f,0.15f,0.15f,0f,0f,0f) },
                    { 10, new StatGrowth(0.2f,0.2f,0.2f,0.2f,0.2f,0f,0f,0f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime00}"),


                Card = "Dies in one hit most of the time."
            };
        }
    }
}