using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class GreenNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.GreenNinja,
                Description = "A swift and elusive assassin.",
                Expectations = "Skirmisher that relies on speed and crits. Low base damage per hit but high turn economy.",
                Lore = "Silent courier of the Jade Clique, paid to make problems vanish.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 5f,
                    Vitality = 3f,
                    Agility = 9f,
                    Speed = 8f,
                    Stamina = 4f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 6f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.7f,
                    Vitality = 0.6f,
                    Agility = 2.4f,
                    Speed = 2.2f,
                    Stamina = 1.0f,
                    Intelligence = 0.6f,
                    Wisdom = 0.4f,
                    Luck = 1.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.4f, Vitality = 0.6f, Agility = 3.2f, Speed = 2.8f, Stamina = 0.6f, Intelligence = 0.6f, Wisdom = 0.3f, Luck = 1.2f } },
                    { 10, new StatGrowth { Strength = 2.0f, Vitality = 1.1f, Agility = 4.2f, Speed = 3.8f, Stamina = 1.1f, Intelligence = 1.0f, Wisdom = 0.6f, Luck = 1.7f } },
                    { 20, new StatGrowth { Strength = 2.6f, Vitality = 1.6f, Agility = 5.4f, Speed = 4.8f, Stamina = 1.6f, Intelligence = 1.6f, Wisdom = 1.1f, Luck = 2.7f } },
                    { 40, new StatGrowth { Strength = 3.2f, Vitality = 2.1f, Agility = 7.0f, Speed = 6.5f, Stamina = 2.1f, Intelligence = 2.0f, Wisdom = 1.6f, Luck = 3.8f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.GreenNinja}"),
                Details = new ActorDetails
                {
                    Description = "A swift and elusive assassin.",
                    Card = "Evades [first attack] each round. Gains [Momentum] when undamaged.",
                }
            };
        }
    }
}
