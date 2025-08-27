using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Soldier03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Soldier03,
                Groups = ActorGroup.Soldier | ActorGroup.Humanoid,
                Description = "A washed-up old fighter.",
                Expectations = "Sturdy baseline with weak offense.",
                Lore = "Claims to have trained captains when captains were young.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 3f,
                    Vitality = 3f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 2f,
                    Intelligence = 1f,
                    Wisdom = 2f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.5f,
                    Agility = 0.2f,
                    Speed = 0.2f,
                    Stamina = 0.3f,
                    Intelligence = 0.4f,
                    Wisdom = 0.2f,
                    Luck = 0.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.5f, Vitality = 0.6f, Agility = 0.2f, Speed = 0.2f, Stamina = 0.3f, Intelligence = 0.5f, Wisdom = 0.3f, Luck = 0.3f } },
                    { 10, new StatGrowth { Strength = 0.6f, Vitality = 0.7f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.4f, Intelligence = 0.6f, Wisdom = 0.3f, Luck = 0.4f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.7f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier03}"),
                Details = new ActorDetails
                {
                    Description = "A washed-up old fighter.",
                    Card = "Takes a hit better than he gives one.",
                    Trivia = new List<string>
                    {
                        "Once held rank",
                        "Talks in riddles"
                    }
                }
            };
        }
    }
}
