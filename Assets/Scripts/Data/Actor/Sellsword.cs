using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Sellsword
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Sellsword,
                Description = "A blade-for-hire who fights for coin.",
                Expectations = "Flexible baseline combatant. Never the best, rarely the worst. Trades consistently into most foes.",
                Lore = "Signed more contracts than most nobles sign letters.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +3
                    Vitality = 6f,
                    Agility = 5f,
                    Speed = 6f,     // +1
                    Stamina = 4f,
                    Intelligence = 3f,
                    Wisdom = 3f,
                    Luck = 6f       // +1
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.5f
                    Vitality = 1.5f,
                    Agility = 1.2f,
                    Speed = 1.2f,
                    Stamina = 0.9f,
                    Intelligence = 0.6f,
                    Wisdom = 0.6f,
                    Luck = 1.3f      // was 1.1f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.2f, Vitality = 1.9f, Agility = 1.5f, Speed = 1.5f, Stamina = 1.1f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 1.6f } },
                    { 10, new StatGrowth { Strength = 2.7f, Vitality = 2.4f, Agility = 1.8f, Speed = 1.8f, Stamina = 1.3f, Intelligence = 1.0f, Wisdom = 1.0f, Luck = 2.0f } },
                    { 20, new StatGrowth { Strength = 3.7f, Vitality = 3.3f, Agility = 2.2f, Speed = 2.2f, Stamina = 1.8f, Intelligence = 1.5f, Wisdom = 1.5f, Luck = 2.6f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Sellsword}"),
                Details = new ActorDetails
                {
                    Description = "A blade-for-hire who fights for coin.",
                    Card = "Steady DPS baseline. Always ready for work.",
                }
            };
        }
    }
}
