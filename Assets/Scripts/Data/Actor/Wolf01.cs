using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Data.Actor
{
    public static class Wolf01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                Character = CharacterHelper.Wolf01,
                Groups = ActorGroup.Beast | ActorGroup.Melee,
                Description = "A skittish young wolf.",
                Expectations = "High speed, paper hide.",
                Lore = "Survives by running faster than the pack.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 3f,
                    Vitality = 1f,
                    Agility = 4f,
                    Speed = 4f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.6f, 0.25f, 0.8f, 0.8f, 0.25f, 0.1f, 0.1f, 0.2f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.7f,0.3f,1.0f,0.9f,0.3f,0.1f,0.1f,0.3f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.44f, 0.37f, 0f),
                    Scale = new Vector3(1f, 1f, 0f),
                },
                Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Wolf01}"),
                Details = new ActorDetails { Description = "Fast but fragile.", Card = "Acts often; dies fast." }
            };
        }
    }
}