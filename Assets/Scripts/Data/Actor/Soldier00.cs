using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Soldier00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Soldier00,
                Groups = ActorGroup.Soldier | ActorGroup.Humanoid,
                Description = "A low-ranked fort guard.",
                Expectations = "Entry level foe. Falls off quickly at higher levels.",
                Lore = "Knows every watchpost in the fort by name.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 3f,
                    Vitality = 2f,
                    Agility = 2f,
                    Speed = 2f,
                    Stamina = 2f,
                    Intelligence = 1f,
                    Wisdom = 1f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.5f,
                    Vitality = 0.4f,
                    Agility = 0.3f,
                    Speed = 0.3f,
                    Stamina = 0.2f,
                    Intelligence = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 0.2f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.6f, Vitality = 0.5f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.3f, Wisdom = 0.3f, Luck = 0.3f } },
                    { 10, new StatGrowth { Strength = 0.8f, Vitality = 0.6f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.4f, Intelligence = 0.4f, Wisdom = 0.4f, Luck = 0.4f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.71f, -1.5f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Soldier00}"),
                Details = new ActorDetails
                {
                    Description = "A low-ranked fort guard.",
                    Card = "Basic soldier. Vulnerable but alert.",
                }
            };
        }
    }
}
