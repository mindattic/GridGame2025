using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Slime03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Slime03,
                Description = "A mischievous wobble.",
                Expectations = "Still weak; might dodge once.",
                Lore = "Learns tricks by imitation.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 2f,
                    Speed = 1f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.1f, 0.1f, 0.15f, 0.1f, 0.1f, 0f, 0f, 0.15f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.15f,0.2f,0.1f,0.15f,0f,0f,0.2f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings { 
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Slime03}"),
                Details = new ActorDetails { Description = "Tries to get lucky.", Card = "Occasional crits, still fragile." }
            };
        }
    }
}