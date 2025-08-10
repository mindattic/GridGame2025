using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Scorpion
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Scorpion,
                Description = "A hulking brute with a barbed tail and armored shell.",
                Expectations = "Slow bruiser with very high durability. Damage is steady. Punishes opponents who stay in front.",
                Lore = "Ancient desert crawler whose shell rings like iron under the moon.",

                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 7f,
                    Vitality = 1f,
                    Agility = 2f,
                    Stamina = 1.75f,
                    Intelligence = 1f,
                    Wisdom = 2f,
                    Luck = 3f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.4f,
                    Vitality = 0.65f,
                    Agility = 0.5f,
                    Stamina = 0.525f,
                    Intelligence = 0.4f,
                    Wisdom = 0.6f,
                    Luck = 0.6f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 0.95f,  Agility = 0.8f, Stamina = 0.65f,  Intelligence = 0.5f, Wisdom = 0.8f, Luck = 1.0f } }, // 3.8 -> 0.95, 2.6 -> 0.65
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 1.2f,   Agility = 1.0f, Stamina = 0.8f,   Intelligence = 0.8f, Wisdom = 1.0f, Luck = 1.2f } }, // 4.8 -> 1.2, 3.2 -> 0.8
                    { 20, new StatGrowth { Strength = 2.8f, Vitality = 1.6f,   Agility = 1.2f, Stamina = 1.05f,  Intelligence = 1.0f, Wisdom = 1.2f, Luck = 1.5f } }, // 6.4 -> 1.6, 4.2 -> 1.05
                    { 40, new StatGrowth { Strength = 3.8f, Vitality = 2.125f, Agility = 1.5f, Stamina = 1.3f,   Intelligence = 1.5f, Wisdom = 1.5f, Luck = 2.0f } }  // 8.5 -> 2.125, 5.2 -> 1.3
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(1.02f, 0.04f, 0.0f),
                    Scale = new Vector3(4.0f, 4.0f, 0f)
                },

                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Scorpion}"),

                Details = new ActorDetails
                {
                    Description = "A hulking brute with a barbed tail and armored shell.",
                    Card = "Takes [reduced damage] from frontal attacks. Has a chance to [counterattack] when hit.",
                }
            };
        }
    }
}
