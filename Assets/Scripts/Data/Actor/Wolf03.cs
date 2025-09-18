using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Wolf03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Wolf03,
                Groups = ActorGroup.Beast | ActorGroup.Melee,
                Description = "A bold pack runner.",
                Expectations = "Hardest-hitting of the low wolves.",
                Lore = "Tests prey before committing.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 4f,
                    Vitality = 1f,
                    Agility = 3f,
                    Speed = 3f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.8f, 0.25f, 0.6f, 0.6f, 0.25f, 0.1f, 0.1f, 0.25f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(1.0f,0.3f,0.7f,0.7f,0.3f,0.1f,0.1f,0.3f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.27f, 0.1f, 0f),
                    Scale = new Vector3(2f, 2f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Wolf03}"),
                Details = new ActorDetails { Description = "Lunges first, asks later.", Card = "High damage, low staying power." }
            };
        }
    }
}