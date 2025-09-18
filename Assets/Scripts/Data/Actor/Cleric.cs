using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Cleric
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Cleric,
                Description = "A strict adherent to the church.",
                Expectations = "Support mage. Damage is steady rather than explosive. Excels when kept safe and allowed to cast.",
                Lore = "Ordained in the Lightbearer Orthodoxy, sworn to mend and to judge.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 5f,
                    Agility = 3f,
                    Speed = 3f,
                    Stamina = 2f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 9f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 1.2f,
                    Agility = 0.6f,
                    Speed = 0.8f,
                    Stamina = 0.5f,
                    Intelligence = 0.8f,
                    Wisdom = 0.8f,
                    Luck = 2.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.0f, Vitality = 1.0f, Agility = 0.4f, Speed = 0.0f, Stamina = 0.5f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 2.8f } },
                    { 10, new StatGrowth { Strength = 0.5f, Vitality = 1.5f, Agility = 0.6f, Speed = 0.0f, Stamina = 0.5f, Intelligence = 0.9f, Wisdom = 0.9f, Luck = 3.6f } },
                    { 20, new StatGrowth { Strength = 1.0f, Vitality = 2.0f, Agility = 0.9f, Speed = 1.0f, Stamina = 1.0f, Intelligence = 1.4f, Wisdom = 1.4f, Luck = 4.5f } },
                    { 40, new StatGrowth { Strength = 1.5f, Vitality = 2.5f, Agility = 1.2f, Speed = 2.0f, Stamina = 2.0f, Intelligence = 1.9f, Wisdom = 1.9f, Luck = 5.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Cleric}"),
                Details = new ActorDetails
                {
                    Description = "A strict adherent to the church.",
                    Card = "Calls down healing light and protective wards."
                }
            };
        }
    }
}
