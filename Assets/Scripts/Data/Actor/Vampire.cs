using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Vampire
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Vampire,
                Description = "A shadowy predator who thrives in darkness.",
                Expectations = "Sustained magic DPS with life steal. Wants to fight from safety and drain foes down.",
                Lore = "The last heir of a night-court, more rumor than citizen.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 4f,
                    Vitality = 3f,
                    Agility = 5f,
                    Speed = 5f,
                    Stamina = 2f,
                    Intelligence = 6f,
                    Wisdom = 5f,
                    Luck = 5f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.5f,
                    Agility = 0.9f,
                    Speed = 0.9f,
                    Stamina = 0.4f,
                    Intelligence = 0.75f,
                    Wisdom = 0.6f,
                    Luck = 0.8f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.7f, Vitality = 0.6f, Agility = 1.2f, Speed = 1.2f, Stamina = 0.5f, Intelligence = 1.1f, Wisdom = 0.9f, Luck = 0.9f } },
                    { 10, new StatGrowth { Strength = 0.9f, Vitality = 0.9f, Agility = 1.5f, Speed = 1.5f, Stamina = 0.7f, Intelligence = 1.4f, Wisdom = 1.1f, Luck = 1.1f } },
                    { 20, new StatGrowth { Strength = 1.2f, Vitality = 1.2f, Agility = 1.9f, Speed = 1.9f, Stamina = 1.0f, Intelligence = 1.8f, Wisdom = 1.5f, Luck = 1.5f } },
                    { 40, new StatGrowth { Strength = 1.6f, Vitality = 1.6f, Agility = 2.3f, Speed = 2.3f, Stamina = 1.4f, Intelligence = 2.4f, Wisdom = 1.9f, Luck = 2.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.Generate(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Vampire}"),
                Details = new ActorDetails
                {
                    Description = "A shadowy predator who thrives in darkness.",
                    Card = "Heals for 30% of magic damage dealt. Resistant to [Dark].",
                    Trivia = new List<string>
                    {
                        "Sleeps in a crate",
                        "Allergic to dawn"
                    }
                }
            };
        }
    }
}
