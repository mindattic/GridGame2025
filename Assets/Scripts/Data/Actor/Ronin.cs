using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Ronin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Ronin,
                Description = "A masterless warrior guided by honor.",
                Expectations = "Reliable duelist. Open strong, stays even over time. Few weaknesses, few tricks.",
                Lore = "Once sworn to a fallen house, now sworn to the road.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 11f, // +3
                    Vitality = 6f,
                    Agility = 6f,
                    Speed = 6f,     // +1
                    Stamina = 4f,
                    Intelligence = 2f,
                    Wisdom = 3f,
                    Luck = 5f       // +1
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.5f
                    Vitality = 1.0f,
                    Agility = 1.3f,
                    Speed = 1.2f,    // was 1.0f
                    Stamina = 0.9f,
                    Intelligence = 0.4f,
                    Wisdom = 0.8f,
                    Luck = 1.0f      // was 0.8f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.2f, Vitality = 1.3f, Agility = 1.5f, Speed = 1.4f, Stamina = 0.9f, Intelligence = 0.5f, Wisdom = 1.0f, Luck = 1.2f } },
                    { 10, new StatGrowth { Strength = 2.6f, Vitality = 1.8f, Agility = 1.9f, Speed = 1.7f, Stamina = 1.2f, Intelligence = 1.0f, Wisdom = 1.5f, Luck = 1.4f } },
                    { 20, new StatGrowth { Strength = 3.5f, Vitality = 2.2f, Agility = 2.4f, Speed = 2.2f, Stamina = 1.7f, Intelligence = 1.5f, Wisdom = 2.0f, Luck = 1.7f } },
                    { 40, new StatGrowth { Strength = 4.5f, Vitality = 2.7f, Agility = 3.1f, Speed = 2.8f, Stamina = 2.1f, Intelligence = 2.0f, Wisdom = 2.5f, Luck = 2.2f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.35f, -1.34f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Ronin}"),
                Card = "Has a chance to [Counter] when attacked. Deals extra damage on the first strike.",
                Trivia = new List<string>
                    {
                        "Once served a great house",
                        "Walks the path of redemption"
                    }
            };
        }
    }
}
