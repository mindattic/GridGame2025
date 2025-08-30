using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Slime01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Slime01,
                Description = "A spry little slime.",
                Expectations = "Still fragile, just wigglier.",
                Lore = "Developed a taste for bouncing.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 2f,
                    Speed = 2f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth(0.1f, 0.1f, 0.15f, 0.15f, 0.1f, 0f, 0f, 0.05f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.15f,0.2f,0.2f,0.15f,0f,0f,0.1f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Slime01}"),
                Details = new ActorDetails { Description = "Quick to flee.", Card = "Evasive but still a pushover." }
            };
        }
    }
}