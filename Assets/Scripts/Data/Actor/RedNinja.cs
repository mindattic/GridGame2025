using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class RedNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.RedNinja,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A deadly assassin wielding forbidden arts.",
                Expectations = "Ambusher with burst windows. Leans on poison and vanish to reset fights.",
                Lore = "Blade-broker of the Crimson Pact, paid in secrets as often as coin.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10f, // +3
                    Vitality = 4f,
                    Agility = 8f,
                    Speed = 8f,     // +1
                    Stamina = 5f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 8f       // +3
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.9f, // was 1.55f
                    Vitality = 1.0f,
                    Agility = 1.9f,
                    Speed = 1.8f,    // was 1.6f
                    Stamina = 1.1f,
                    Intelligence = 0.6f,
                    Wisdom = 0.5f,
                    Luck = 1.6f      // was 1.1f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.1f, Vitality = 1.0f, Agility = 2.3f, Speed = 2.2f, Stamina = 1.1f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 1.4f } },
                    { 10, new StatGrowth { Strength = 2.5f, Vitality = 1.2f, Agility = 2.7f, Speed = 2.5f, Stamina = 1.4f, Intelligence = 0.9f, Wisdom = 0.8f, Luck = 1.8f } },
                    { 20, new StatGrowth { Strength = 3.1f, Vitality = 1.5f, Agility = 3.2f, Speed = 3.0f, Stamina = 1.8f, Intelligence = 1.4f, Wisdom = 1.0f, Luck = 2.3f } },
                    { 40, new StatGrowth { Strength = 3.9f, Vitality = 2.0f, Agility = 4.1f, Speed = 3.8f, Stamina = 2.3f, Intelligence = 1.9f, Wisdom = 1.5f, Luck = 2.7f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterClass.RedNinja}"),
                Card = "Applies [Poison] with melee attacks. May [Vanish] when hit.",
            };
        }
    }
}
