using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Wolf00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Wolf00,
                Groups = ActorGroup.Beast | ActorGroup.Melee,
                Description = "A hungry stray wolf.",
                Expectations = "Quick nip; folds in 1–2 hits.",
                Lore = "Hunts alone at the edge of the woods.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 3f,
                    Vitality = 2f,
                    Agility = 3f,
                    Speed = 3f,
                    Stamina = 2f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.6f, 0.3f, 0.6f, 0.6f, 0.3f, 0.1f, 0.1f, 0.2f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.7f,0.4f,0.7f,0.7f,0.4f,0.1f,0.1f,0.3f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(1.06f, -0.49f, 0f),
                    Scale = new Vector3(3f, 3f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Wolf00}"),
                Details = new ActorDetails { Description = "Strikes then retreats.", Card = "Fast, low durability." }
            };
        }
    }
}