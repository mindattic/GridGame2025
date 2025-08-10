using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class RedNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.RedNinja,
                Description = "A deadly assassin wielding forbidden arts.",
                Expectations = "Ambusher with burst windows. Leans on poison and vanish to reset fights.",
                Lore = "Blade-broker of the Crimson Pact, paid in secrets as often as coin.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 7f,
                    Vitality = 4f,
                    Agility = 7f,
                    Stamina = 5f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 5f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.55f,
                    Vitality = 1.0f,
                    Agility = 1.6f,
                    Stamina = 1.1f,
                    Intelligence = 0.6f,
                    Wisdom = 0.5f,
                    Luck = 1.1f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5, new StatGrowth { Strength = 1.7f, Vitality = 1.0f, Agility = 1.9f, Stamina = 1.1f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 1.1f } },
                    { 10, new StatGrowth { Strength = 2.1f, Vitality = 1.2f, Agility = 2.2f, Stamina = 1.4f, Intelligence = 0.9f, Wisdom = 0.8f, Luck = 1.4f } },
                    { 20, new StatGrowth { Strength = 2.6f, Vitality = 1.5f, Agility = 2.7f, Stamina = 1.8f, Intelligence = 1.4f, Wisdom = 1.0f, Luck = 1.9f } },
                    { 40, new StatGrowth { Strength = 3.4f, Vitality = 2.0f, Agility = 3.5f, Stamina = 2.3f, Intelligence = 1.9f, Wisdom = 1.5f, Luck = 2.3f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.RedNinja}"),
                Details = new ActorDetails
                {
                    Description = "A deadly assassin wielding forbidden arts.",
                    Card = "Applies [Poison] with melee attacks. May [Vanish] when hit.",
                }
            };
        }
    }
}
