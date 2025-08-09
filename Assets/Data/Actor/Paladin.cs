namespace Assets.Data.Actor
{
    using Assets.Helpers;
    using Assets.Scripts.Models;
    using System.Collections.Generic;
    using UnityEngine;
    using g = Assets.Helpers.GameHelper;
    public static class Paladin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Paladin,
                Description = "A holy warrior clad in armor.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 7,
                    Vitality = 8,
                    Agility = 3,
                    Stamina = 6,
                    Intelligence = 3,
                    Wisdom = 6,
                    Luck = 3
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.5f,
                    Vitality = 2.0f,
                    Agility = 0.5f,
                    Stamina = 1.5f,
                    Intelligence = 0.6f,
                    Wisdom = 1.8f,
                    Luck = 0.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 1.0f, Vitality = 2.0f, Agility = 0.5f, Stamina = 1.5f, Intelligence = 0.5f, Wisdom = 1.5f, Luck = 0.5f } },
                    { 10, new StatGrowth { Strength = 1.5f, Vitality = 2.5f, Agility = 0.8f, Stamina = 2.0f, Intelligence = 1.0f, Wisdom = 2.0f, Luck = 1.0f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 3.0f, Agility = 1.0f, Stamina = 2.5f, Intelligence = 1.5f, Wisdom = 2.5f, Luck = 1.5f } },
                    { 40, new StatGrowth { Strength = 3.0f, Vitality = 4.0f, Agility = 1.5f, Stamina = 3.5f, Intelligence = 2.0f, Wisdom = 3.5f, Luck = 2.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0f),
                    Scale = new Vector3(5f, 5f, 0),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Paladin}"),
                Details = new ActorDetails
                {
                    Description = "A holy warrior clad in armor.",
                    Card = "Shields nearby allies with <color=#FFD700>[Radiant Guard]</color>. Takes reduced <color=#CCCCCC>[Physical]</color> and <color=#FFCCFF>[Dark]</color> damage.",
                    Lore = new List<string>
                    {
                        "Sworn to protect",
                        "Answers only to the Lightbearer Council"
                    }
                }
            };
        }
    }
}
