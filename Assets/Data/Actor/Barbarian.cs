namespace Assets.Data.Actor
{
    using Assets.Helpers;
    using Assets.Scripts.Models;
    using System.Collections.Generic;
    using UnityEngine;
    using g = Assets.Helpers.GameHelper;

    public static class Barbarian
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Barbarian,
                Description = "A warrior driven by rage.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9,
                    Vitality = 6,
                    Agility = 3,
                    Stamina = 2,
                    Intelligence = 2,
                    Wisdom = 1,
                    Luck = 4
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 2.4f,
                    Vitality = 1.2f,
                    Agility = 0.5f,
                    Stamina = 0.6f,
                    Intelligence = 0.0f,
                    Wisdom = 0.3f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 2.5f, Vitality = 2f, Agility = 1f, Stamina = 1.5f, Intelligence = 0.3f, Wisdom = 0.4f, Luck = 0.5f } },
                    { 10, new StatGrowth { Strength = 3f, Vitality = 2.5f, Agility = 1.2f, Stamina = 2f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 0.6f } },
                    { 15, new StatGrowth { Strength = 3.5f, Vitality = 3f, Agility = 1.5f, Stamina = 2.5f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 0.8f } },
                    { 20, new StatGrowth { Strength = 4f, Vitality = 3.5f, Agility = 1.7f, Stamina = 3f, Intelligence = 0.8f, Wisdom = 0.7f, Luck = 1f } },
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.1f, 0f),
                    Scale = new Vector3(5f, 5f, 0),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Barbarian}"),
                Details = new ActorDetails
                {
                    Description = "A warrior driven by rage.",
                    Card = "Gains <color=#FF0033>[Rage]</color> when attacking or being attacked. Will eventually go <color=#FF0000>[Berserk]</color> and attack multiple nearby enemies.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            };
        }
    }

}
