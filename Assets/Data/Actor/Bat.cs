namespace Assets.Data.Actor
{
    using Assets.Scripts.Models;
    using System.Collections.Generic;
    using UnityEngine;
    using g = Assets.Helpers.GameManagerHelper;
    public static class Bat
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Bat,
                Description = "A flying menace.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2,
                    Vitality = 1,
                    Agility = 6,
                    Stamina = 2,
                    Intelligence = 7,
                    Wisdom = 3,
                    Luck = 5
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.2f,
                    Agility = 1.2f,
                    Stamina = 0.6f,
                    Intelligence = 1.4f,
                    Wisdom = 1.0f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 0.5f, Vitality = 0.2f, Agility = 2.0f, Stamina = 0.5f, Intelligence = 2.0f, Wisdom = 1.0f, Luck = 1.0f } },
                    { 10, new StatGrowth { Strength = 1.0f, Vitality = 0.5f, Agility = 1.5f, Stamina = 1.0f, Intelligence = 2.5f, Wisdom = 1.5f, Luck = 1.5f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 1.0f, Agility = 2.5f, Stamina = 1.5f, Intelligence = 3.0f, Wisdom = 2.0f, Luck = 2.0f } },
                    { 40, new StatGrowth { Strength = 3.0f, Vitality = 2.0f, Agility = 4.0f, Stamina = 2.0f, Intelligence = 4.0f, Wisdom = 2.5f, Luck = 2.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, 0.5f, 0f),
                    Scale = new Vector3(2f, 2f, 0),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Bat}"),
                Details = new ActorDetails
                {
                    Description = "A flying menace.",
                    Card = "Intermittently goes <color=#FF0033>[Berserk]</color> attacking multiple nearby enemies.",
                    Lore = new List<string> { "Echolocation expert", "Sleeps upside down" }
                }
            };
        }
    }
}
