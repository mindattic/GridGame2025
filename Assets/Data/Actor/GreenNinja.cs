namespace Assets.Data.Actor
{
    using Assets.Helpers;
    using Assets.Scripts.Models;
    using System.Collections.Generic;
    using UnityEngine;
    using g = Assets.Helpers.GameHelper;
    public static class GreenNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.GreenNinja,
                Description = "A swift and elusive assassin.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 5,
                    Vitality = 3,
                    Agility = 8,
                    Stamina = 4,
                    Intelligence = 3,
                    Wisdom = 2,
                    Luck = 6
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.0f,
                    Vitality = 0.6f,
                    Agility = 2.2f,
                    Stamina = 1.0f,
                    Intelligence = 0.6f,
                    Wisdom = 0.4f,
                    Luck = 1.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 1.0f, Vitality = 0.5f, Agility = 2.5f, Stamina = 0.5f, Intelligence = 0.5f, Wisdom = 0.2f, Luck = 1.0f } },
                    { 10, new StatGrowth { Strength = 1.5f, Vitality = 1.0f, Agility = 3.5f, Stamina = 1.0f, Intelligence = 1.0f, Wisdom = 0.5f, Luck = 1.5f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 1.5f, Agility = 4.5f, Stamina = 1.5f, Intelligence = 1.5f, Wisdom = 1.0f, Luck = 2.5f } },
                    { 40, new StatGrowth { Strength = 2.5f, Vitality = 2.0f, Agility = 6.0f, Stamina = 2.0f, Intelligence = 2.0f, Wisdom = 1.5f, Luck = 3.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0f),
                    Scale = new Vector3(5f, 5f, 0),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.GreenNinja}"),
                Details = new ActorDetails
                {
                    Description = "A swift and elusive assassin.",
                    Card = "Evades <color=#33FF66>[first attack]</color> each round. Gains <color=#00FFFF>[Momentum]</color> when undamaged.",
                    Lore = new List<string>
                    {
                        "Trained in the shadows",
                        "Prefers throwing stars to swords"
                    }
                }
            };
        }
    }
}
