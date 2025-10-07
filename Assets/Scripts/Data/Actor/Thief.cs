using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class Thief
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Thief,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A nimble rogue with sticky fingers.",
                Expectations = "Mobile crit fisher. Leans on evasion and luck to win long trades. Avoids armored foes.",
                Lore = "Knows three hundred pockets by heart.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 8f,  // +3
                    Vitality = 2f,
                    Agility = 7f,
                    Speed = 7f,     // +1
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 9f       // +2
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.5f
                    Vitality = 0.5f,
                    Agility = 1.8f,
                    Speed = 2.1f,    // was 2.0f
                    Stamina = 0.6f,
                    Intelligence = 0.5f,
                    Wisdom = 0.7f,
                    Luck = 2.5f      // was 2.2f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.1f, Vitality = 0.5f, Agility = 2.3f, Speed = 2.6f, Stamina = 0.7f, Intelligence = 0.6f, Wisdom = 0.8f, Luck = 3.0f } },
                    { 10, new StatGrowth { Strength = 2.4f, Vitality = 0.6f, Agility = 3.1f, Speed = 3.1f, Stamina = 0.9f, Intelligence = 0.8f, Wisdom = 1.0f, Luck = 3.8f } },
                    { 20, new StatGrowth { Strength = 2.8f, Vitality = 0.8f, Agility = 3.9f, Speed = 3.6f, Stamina = 1.1f, Intelligence = 1.0f, Wisdom = 1.2f, Luck = 4.9f } },
                    { 40, new StatGrowth { Strength = 3.2f, Vitality = 1.0f, Agility = 4.9f, Speed = 4.4f, Stamina = 1.3f, Intelligence = 1.2f, Wisdom = 1.5f, Luck = 6.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.61f, -1.56f, 0.0f),
                    Scale = new Vector3(5.3f, 5.3f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterClass.Thief}"),
                Card = "Has a high chance to evade. Can steal from enemies.",
                Trivia = new List<string>
                    {
                        "Loves coin",
                        "Allergic to jail cells"
                    }
            };
        }
    }
}
