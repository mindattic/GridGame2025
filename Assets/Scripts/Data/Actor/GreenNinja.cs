using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper; // Added alias

namespace Assets.Data.Actor
{
    public static class GreenNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.GreenNinja,
                Description = "A swift and elusive assassin.",
                Expectations = "Skirmisher that relies on speed and crits. Low base damage per hit but high turn economy.",
                Lore = "Silent courier of the Jade Clique, paid to make problems vanish.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 8f,  // +3
                    Vitality = 3f,
                    Agility = 9f,
                    Speed = 9f,     // +1
                    Stamina = 4f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 9f       // +3 for crit/hit
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 2.0f, // was 1.7f
                    Vitality = 0.6f,
                    Agility = 2.4f,
                    Speed = 2.4f,    // was 2.2f
                    Stamina = 1.0f,
                    Intelligence = 0.6f,
                    Wisdom = 0.4f,
                    Luck = 2.0f      // was 1.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 0.6f, Agility = 3.4f, Speed = 3.0f, Stamina = 0.6f, Intelligence = 0.6f, Wisdom = 0.3f, Luck = 1.6f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 1.1f, Agility = 4.5f, Speed = 4.0f, Stamina = 1.1f, Intelligence = 1.0f, Wisdom = 0.6f, Luck = 2.1f } },
                    { 20, new StatGrowth { Strength = 2.9f, Vitality = 1.6f, Agility = 5.8f, Speed = 5.2f, Stamina = 1.6f, Intelligence = 1.6f, Wisdom = 1.1f, Luck = 3.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.GreenNinja}"),
                Card = "Prefers to crit and move on."
            };
        }
    }
}
