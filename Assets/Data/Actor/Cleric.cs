namespace Assets.Data.Actor
{
    using Assets.Scripts.Models;
    using System.Collections.Generic;
    using UnityEngine;
    using g = Assets.Helpers.GameManagerHelper;
    public static class Cleric
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Cleric,
                Description = "A strict adherent to the church.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2,
                    Vitality = 5,
                    Agility = 3,
                    Stamina = 2,
                    Intelligence = 3,
                    Wisdom = 2,
                    Luck = 9
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 1.2f,
                    Agility = 0.8f,
                    Stamina = 0.5f,
                    Intelligence = 0.8f,
                    Wisdom = 0.8f,
                    Luck = 2.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 0.0f, Vitality = 1.0f, Agility = 0.0f, Stamina = 0.5f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 3.0f } },
                    { 10, new StatGrowth { Strength = 0.5f, Vitality = 1.5f, Agility = 0.0f, Stamina = 0.5f, Intelligence = 1.0f, Wisdom = 1.0f, Luck = 4.0f } },
                    { 20, new StatGrowth { Strength = 1.0f, Vitality = 2.0f, Agility = 1.0f, Stamina = 1.0f, Intelligence = 1.5f, Wisdom = 1.5f, Luck = 5.0f } },
                    { 40, new StatGrowth { Strength = 1.5f, Vitality = 2.5f, Agility = 2.0f, Stamina = 2.0f, Intelligence = 2.0f, Wisdom = 2.0f, Luck = 6.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0f),
                    Scale = new Vector3(5f, 5f, 0),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Cleric}"),
                Details = new ActorDetails
                {
                    Description = "An adherent to the Lightbearer Orthodoxy.",
                    Card = "Casts <color=#00FF00>[Cure]</color> when supporting an attacker.",
                    Lore = new List<string> { "Likes jerky", "Hates Reptiles" }
                }
            };
        }
    }
}
