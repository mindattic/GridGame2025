using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Thief
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Thief,
                Description = "A nimble rogue with sticky fingers.",
                Expectations = "Mobile crit fisher. Leans on evasion and luck to win long trades. Avoids armored foes.",
                Lore = "Knows three hundred pockets by heart.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 5f,
                    Vitality = 2f,
                    Agility = 7f,
                    Speed = 6f,
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 7f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.5f,
                    Vitality = 0.5f,
                    Agility = 1.8f,
                    Speed = 2.0f,
                    Stamina = 0.6f,
                    Intelligence = 0.5f,
                    Wisdom = 0.7f,
                    Luck = 2.2f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 0.5f, Agility = 2.2f, Speed = 2.5f, Stamina = 0.7f, Intelligence = 0.6f, Wisdom = 0.8f, Luck = 2.8f } },
                    { 10, new StatGrowth { Strength = 2.1f, Vitality = 0.6f, Agility = 3.0f, Speed = 3.0f, Stamina = 0.9f, Intelligence = 0.8f, Wisdom = 1.0f, Luck = 3.4f } },
                    { 20, new StatGrowth { Strength = 2.5f, Vitality = 0.8f, Agility = 3.8f, Speed = 3.5f, Stamina = 1.1f, Intelligence = 1.0f, Wisdom = 1.2f, Luck = 4.5f } },
                    { 40, new StatGrowth { Strength = 2.9f, Vitality = 1.0f, Agility = 4.8f, Speed = 4.3f, Stamina = 1.3f, Intelligence = 1.2f, Wisdom = 1.5f, Luck = 5.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.61f, -1.56f, 0.0f),
                    Scale = new Vector3(5.3f, 5.3f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Thief}"),
                Details = new ActorDetails
                {
                    Description = "A nimble rogue with sticky fingers.",
                    Card = "Has a high chance to evade. Can steal from enemies.",
                    Trivia = new List<string>
                    {
                        "Loves coin",
                        "Allergic to jail cells"
                    }
                }
            };
        }
    }
}
