using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

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
                    Strength = 12f,  // +3 for early punch
                    Vitality = 6f,
                    Agility = 3f,
                    Speed = 4f,      // +1 for turn pace
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 1f,
                    Luck = 6f        // +2 for hit/crit reliability
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.55f
                    Vitality = 1.1f,
                    Agility = 0.5f,
                    Speed = 0.6f,    // was 0.5f
                    Stamina = 0.6f,
                    Intelligence = 0.0f,
                    Wisdom = 0.3f,
                    Luck = 0.7f      // was 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.0f, Vitality = 1.8f, Agility = 0.6f, Speed = 1.0f, Stamina = 1.1f, Intelligence = 0.3f, Wisdom = 0.4f, Luck = 0.6f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 2.0f, Agility = 0.7f, Speed = 1.1f, Stamina = 1.5f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 0.7f } },
                    { 15, new StatGrowth { Strength = 2.6f, Vitality = 2.3f, Agility = 0.8f, Speed = 1.3f, Stamina = 1.9f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 0.9f } },
                    { 20, new StatGrowth { Strength = 2.9f, Vitality = 2.6f, Agility = 1.0f, Speed = 1.5f, Stamina = 2.2f, Intelligence = 0.8f, Wisdom = 0.7f, Luck = 1.1f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.8f, -1.25f, 0f),
                    Scale = new Vector3(5f, 5f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Barbarian}"),
      
                Card = "Gains [Rage] when attacking or being attacked. Will eventually go [Berserk] and attack multiple nearby enemies."

            };
        }
    }
}
