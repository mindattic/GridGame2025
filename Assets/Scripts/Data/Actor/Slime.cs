using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Slime
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Slime,
                Description = "A jiggly nuisance barely held together.",
                Expectations = "Training dummy. Exists to make heroes feel strong.",
                Lore = "Somewhere between a pet and a puddle.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.1f,
                    Vitality = 0.1f,
                    Agility = 0.1f,
                    Speed = 0.1f,
                    Stamina = 0.1f,
                    Intelligence = 0.0f,
                    Wisdom = 0.0f,
                    Luck = 0.0f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.2f, Vitality = 0.2f, Agility = 0.2f, Speed = 0.2f, Stamina = 0.2f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                    { 10, new StatGrowth { Strength = 0.3f, Vitality = 0.3f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                    { 20, new StatGrowth { Strength = 0.4f, Vitality = 0.4f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.4f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                    { 40, new StatGrowth { Strength = 0.5f, Vitality = 0.5f, Agility = 0.5f, Speed = 0.5f, Stamina = 0.5f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, 0.5f, 0.0f),
                    Scale = new Vector3(2.0f, 2.0f, 0f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Slime}"),
                Details = new ActorDetails
                {
                    Description = "A jiggly nuisance barely held together.",
                    Card = "Lowest Stats in the game. Designed to die in one hit.",
                }
            };
        }
    }
}
