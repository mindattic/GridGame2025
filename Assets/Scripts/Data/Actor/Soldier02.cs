using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Soldier02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Soldier02,
                Groups = ActorGroup.Soldier | ActorGroup.Humanoid,
                Description = "A jittery scout with sharp eyes.",
                Expectations = "Skirmisher with good initiative but poor staying power.",
                Lore = "Never stops scanning the horizon.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 2f,
                    Agility = 5f,
                    Speed = 4f,
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 1f,
                    Luck = 2f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.3f,
                    Vitality = 0.3f,
                    Agility = 0.7f,
                    Speed = 0.6f,
                    Stamina = 0.3f,
                    Intelligence = 0.3f,
                    Wisdom = 0.2f,
                    Luck = 0.4f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.5f, Vitality = 0.3f, Agility = 0.6f, Speed = 0.7f, Stamina = 0.3f, Intelligence = 0.3f, Wisdom = 0.3f, Luck = 0.4f } },
                    { 10, new StatGrowth { Strength = 0.7f, Vitality = 0.4f, Agility = 0.7f, Speed = 0.8f, Stamina = 0.4f, Intelligence = 0.4f, Wisdom = 0.4f, Luck = 0.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -0.55f, 0.0f),
                    Scale = new Vector3(3.0f, 3.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier02}"),
                Details = new ActorDetails
                {
                    Description = "A jittery scout with sharp eyes.",
                    Card = "Fast to act, quick to flee.",
                    Trivia = new List<string>
                    {
                        "Sniffs everything",
                        "Allergic to slime"
                    }
                }
            };
        }
    }
}
