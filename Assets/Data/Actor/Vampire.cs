using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
public static class Vampire
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Vampire,
            Description = "A shadowy predator who thrives in darkness.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 4,
                Vitality = 3,
                Agility = 5,
                Stamina = 2,
                Intelligence = 6,
                Wisdom = 5,
                Luck = 5
            },
            StatGrowth = new StatGrowth
            {
                Strength = 0.6f,
                Vitality = 0.5f,
                Agility = 1.0f,
                Stamina = 0.4f,
                Intelligence = 1.5f,
                Wisdom = 1.2f,
                Luck = 1.0f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5,  new StatGrowth(1.0f, 0.6f, 1.5f, 0.5f, 2.0f, 1.5f, 1.0f) },
                { 10, new StatGrowth(1.5f, 1.0f, 2.0f, 0.7f, 2.5f, 2.0f, 1.5f) },
                { 20, new StatGrowth(2.0f, 1.5f, 2.5f, 1.0f, 3.0f, 2.5f, 2.0f) },
                { 40, new StatGrowth(3.0f, 2.0f, 3.0f, 1.5f, 4.0f, 3.0f, 2.5f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, -1.4f, 0),
                Scale = new Vector3(5f, 5f, 0),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Vampire}"),
            Details = new ActorDetails
            {
                Description = "A shadowy predator who thrives in darkness.",
                Card = "Heals for 30% of magic damage dealt. Resistant to <color=#800080>[Dark]</color>.",
                Lore = new List<string> { "Sleeps in a crate", "Allergic to dawn" }
            }
        };
    }
}
