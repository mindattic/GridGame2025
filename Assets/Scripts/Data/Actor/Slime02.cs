using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Slime02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Slime02,
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
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Slime02}"),
                Details = new ActorDetails { Description = "Goo mass with momentum.", Card = "Barely tanky by slime standards." }
            };
        }
    }
}