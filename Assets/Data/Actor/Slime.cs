using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;
public static class Slime
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Slime,
            Description = "A jiggly nuisance barely held together.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 1,
                Vitality = 1,
                Agility = 1,
                Stamina = 1,
                Intelligence = 0,
                Wisdom = 0,
                Luck = 0
            },
            StatGrowth = new StatGrowth
            {
                Strength = 0.1f,
                Vitality = 0.1f,
                Agility = 0.1f,
                Stamina = 0.1f,
                Intelligence = 0.0f,
                Wisdom = 0.0f,
                Luck = 0.0f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth { Strength = 0.2f, Vitality = 0.2f, Agility = 0.2f, Stamina = 0.2f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                { 10, new StatGrowth { Strength = 0.3f, Vitality = 0.3f, Agility = 0.3f, Stamina = 0.3f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                { 20, new StatGrowth { Strength = 0.4f, Vitality = 0.4f, Agility = 0.4f, Stamina = 0.4f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                { 40, new StatGrowth { Strength = 0.5f, Vitality = 0.5f, Agility = 0.5f, Stamina = 0.5f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, 0.5f, 0f),
                Scale = new Vector3(2f, 2f, 0f)
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Slime}"),
            Details = new ActorDetails
            {
                Description = "A jiggly nuisance barely held together.",
                Card = "Lowest stats in the game. Designed to die in one hit.",
                Lore = new List<string> { "Barely sentient goo", "Fears everything" }
            }
        };
    }
}
