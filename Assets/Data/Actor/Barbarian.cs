using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Barbarian
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Barbarian,
                Description = "A warrior driven by rage.",
                Expectations = "High single target physical DPS that ramps steadily. Low magic. Expects to delete fragile targets and trade evenly into tanks when Rage is stacked.",
                Lore = "Raised in the border wilds, the barbarian believes strength settles all debts.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,
                    Vitality = 6f,
                    Agility = 3f,
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 1f,
                    Luck = 4f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.55f,
                    Vitality = 1.1f,
                    Agility = 0.5f,
                    Stamina = 0.6f,
                    Intelligence = 0.0f,
                    Wisdom = 0.3f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 1.8f, Agility = 0.9f, Stamina = 1.1f, Intelligence = 0.3f, Wisdom = 0.4f, Luck = 0.5f } },
                    { 10, new StatGrowth { Strength = 2.1f, Vitality = 2.0f, Agility = 1.0f, Stamina = 1.5f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 0.6f } },
                    { 15, new StatGrowth { Strength = 2.4f, Vitality = 2.3f, Agility = 1.2f, Stamina = 1.9f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 0.8f } },
                    { 20, new StatGrowth { Strength = 2.7f, Vitality = 2.6f, Agility = 1.4f, Stamina = 2.2f, Intelligence = 0.8f, Wisdom = 0.7f, Luck = 1.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.1f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0.0f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Barbarian}"),
                Details = new ActorDetails
                {
                    Description = "A warrior driven by rage.",
                    Card = "Gains [Rage] when attacking or being attacked. Will eventually go [Berserk] and attack multiple nearby enemies."
                }
            };
        }
    }
}
