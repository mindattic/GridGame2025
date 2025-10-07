using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class Vampire
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Vampire,
                Tags = Tag.Hero | Tag.Humanoid | Tag.Undead,
                Description = "A shadowy predator who thrives in darkness.",
                Expectations = "Sustained magic DPS with life steal. Wants to fight from safety and drain foes down.",
                Lore = "The last heir of a night-court, more rumor than citizen.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +5: makes early hits chunky
                    Vitality = 3f,
                    Agility = 5f,
                    Speed = 6f,     // +1: better turn pace
                    Stamina = 2f,
                    Intelligence = 7f, // +1: flavor (not used by simplified damage)
                    Wisdom = 5f,
                    Luck = 7f       // +2: more hit/crit
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.3f,  // was 0.4f
                    Vitality = 0.6f,
                    Agility = 1.0f,
                    Speed = 1.1f,     // was 0.9f
                    Stamina = 0.5f,
                    Intelligence = 1.0f, // was 0.75f
                    Wisdom = 0.8f,       // was 0.6f
                    Luck = 1.1f          // was 0.8f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.4f, Vitality = 0.7f, Agility = 1.3f, Speed = 1.3f, Stamina = 0.6f, Intelligence = 1.2f, Wisdom = 1.0f, Luck = 1.1f } },
                    { 10, new StatGrowth { Strength = 1.6f, Vitality = 1.0f, Agility = 1.6f, Speed = 1.6f, Stamina = 0.8f, Intelligence = 1.6f, Wisdom = 1.3f, Luck = 1.3f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 1.3f, Agility = 2.1f, Speed = 2.1f, Stamina = 1.1f, Intelligence = 2.1f, Wisdom = 1.7f, Luck = 1.7f } },
                    { 40, new StatGrowth { Strength = 2.6f, Vitality = 1.7f, Agility = 2.6f, Speed = 2.6f, Stamina = 1.5f, Intelligence = 2.7f, Wisdom = 2.2f, Luck = 2.2f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterClass.Vampire}"),
                Card = "Heals for 30% of magic damage dealt. Resistant to [Dark].",
            };
        }
    }
}
