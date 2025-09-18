using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using s = Assets.Helpers.SettingsHelper;

namespace Assets.Data.Actor
{
    public static class Bat
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Bat,
                Description = "A flying menace.",
                Expectations = "Evasive magic harasser. Relies on spells and high speed to peck away. Avoids direct trades with heavy melee.",
                Lore = "Flock-runner of the midnight caves, guided by echoes and hunger.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 1f,
                    Agility = 8f,
                    Speed = 6f,
                    Stamina = 2f,
                    Intelligence = 7f,
                    Wisdom = 3f,
                    Luck = 5f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.2f,
                    Agility = 1.1f,
                    Speed = 1.2f,
                    Stamina = 0.6f,
                    Intelligence = 0.75f,
                    Wisdom = 0.5f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.5f, Vitality = 0.2f, Agility = 1.4f, Speed = 1.5f, Stamina = 0.5f, Intelligence = 1.1f, Wisdom = 0.6f, Luck = 0.8f } },
                    { 10, new StatGrowth { Strength = 0.6f, Vitality = 0.5f, Agility = 1.3f, Speed = 1.2f, Stamina = 0.7f, Intelligence = 1.4f, Wisdom = 0.8f, Luck = 1.0f } },
                    { 20, new StatGrowth { Strength = 1.1f, Vitality = 0.8f, Agility = 1.8f, Speed = 1.8f, Stamina = 1.0f, Intelligence = 1.8f, Wisdom = 1.1f, Luck = 1.4f } },
                    { 40, new StatGrowth { Strength = 1.6f, Vitality = 1.2f, Agility = 2.6f, Speed = 2.6f, Stamina = 1.4f, Intelligence = 2.2f, Wisdom = 1.5f, Luck = 1.8f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, 0.5f, 0.0f),
                    Scale = new Vector3(2.0f, 2.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{s.TextureResolution.ToInt()}/{CharacterHelper.Bat}"),
                Details = new ActorDetails
                {
                    Description = "A flying menace.",
                    Card = "Intermittently goes [Berserk] attacking multiple nearby enemies.",
                    Trivia = new List<string>
                    {
                        "Echolocation expert",
                        "Sleeps upside down"
                    }
                }
            };
        }
    }
}
