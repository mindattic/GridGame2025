using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Soldier01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Soldier01,
                Groups = ActorGroup.Soldier | ActorGroup.Humanoid,
                Description = "A rookie trying too hard.",
                Expectations = "Quick but fragile. Dangerous in groups.",
                Lore = "Painted his shield by hand the night before deployment.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 4f,
                    Vitality = 1f,
                    Agility = 4f,
                    Speed = 3f,
                    Stamina = 1f,
                    Intelligence = 1f,
                    Wisdom = 1f,
                    Luck = 2f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.6f,
                    Vitality = 0.3f,
                    Agility = 0.5f,
                    Speed = 0.5f,
                    Stamina = 0.2f,
                    Intelligence = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 0.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.7f, Vitality = 0.3f, Agility = 0.6f, Speed = 0.6f, Stamina = 0.3f, Intelligence = 0.3f, Wisdom = 0.3f, Luck = 0.4f } },
                    { 10, new StatGrowth { Strength = 0.9f, Vitality = 0.4f, Agility = 0.7f, Speed = 0.7f, Stamina = 0.4f, Intelligence = 0.4f, Wisdom = 0.4f, Luck = 0.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.44f, 0f),
                    Scale = new Vector3(5f, 5f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier01}"),
                Details = new ActorDetails
                {
                    Description = "A rookie trying too hard.",
                    Card = "Faster than most soldiers, but fragile.",
                    Trivia = new List<string>
                    {
                        "Broke 3 spears in training",
                        "Carries lucky bone charm"
                    }
                }
            };
        }
    }
}
