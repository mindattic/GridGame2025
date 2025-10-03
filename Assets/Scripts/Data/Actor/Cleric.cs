using Assets.Helpers;
using Assets.Scripts.Libraries;
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
                    Strength = 6f,  // +4: still support, but can 1–2 shot Slimes
                    Vitality = 5f,
                    Agility = 3f,
                    Speed = 4f,     // +1: better cadence
                    Stamina = 2f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 10f      // +1: high hit/crit identity
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.2f, // was 0.4f
                    Vitality = 1.2f,
                    Agility = 0.6f,
                    Speed = 1.0f,    // was 0.8f
                    Stamina = 0.5f,
                    Intelligence = 0.9f, // was 0.8f
                    Wisdom = 0.9f,       // was 0.8f
                    Luck = 2.5f          // was 2.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.8f, Vitality = 1.0f, Agility = 0.5f, Speed = 0.3f, Stamina = 0.6f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 3.0f } },
                    { 10, new StatGrowth { Strength = 1.4f, Vitality = 1.6f, Agility = 0.7f, Speed = 0.5f, Stamina = 0.7f, Intelligence = 1.1f, Wisdom = 1.1f, Luck = 3.8f } },
                    { 20, new StatGrowth { Strength = 1.8f, Vitality = 2.2f, Agility = 1.0f, Speed = 1.2f, Stamina = 1.1f, Intelligence = 1.6f, Wisdom = 1.6f, Luck = 4.8f } },
                    { 40, new StatGrowth { Strength = 2.4f, Vitality = 2.8f, Agility = 1.4f, Speed = 2.2f, Stamina = 2.1f, Intelligence = 2.1f, Wisdom = 2.1f, Luck = 6.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Cleric}"),
                Abilities = new List<Ability>() {
                    AbilityLibrary.Heal(),
                    AbilityLibrary.Smite()
                },
                Details = new ActorDetails
                {
                    Description = "A strict adherent to the church.",
                    Card = "Calls down healing light and protective wards."
                }
            };
        }
    }
}
