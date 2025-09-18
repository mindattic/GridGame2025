using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Wolf02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Wolf02,
                Groups = ActorGroup.Beast | ActorGroup.Melee,
                Description = "An older, cautious wolf.",
                Expectations = "More measured strike, still low HP.",
                Lore = "Knows when to pick a fight.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 2f,
                    Agility = 2f,
                    Speed = 3f,
                    Stamina = 2f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.5f, 0.35f, 0.5f, 0.6f, 0.35f, 0.1f, 0.1f, 0.2f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.6f,0.4f,0.6f,0.7f,0.4f,0.1f,0.1f,0.25f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.05f, 0.1f, 0f),
                    Scale = new Vector3(2f, 2f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Wolf02}"),
                Details = new ActorDetails { Description = "Balanced for a wolf.", Card = "Still goes down fast." }
            };
        }
    }
}