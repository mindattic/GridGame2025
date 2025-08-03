using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;
public static class Pugilist
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Pugilist,
            Description = "A disciplined master of martial strikes.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 6,
                Vitality = 5,
                Agility = 8,
                Stamina = 6,
                Intelligence = 2,
                Wisdom = 2,
                Luck = 4
            },
            StatGrowth = new StatGrowth
            {
                Strength = 1.6f,
                Vitality = 1.0f,
                Agility = 2.2f,
                Stamina = 1.4f,
                Intelligence = 0.2f,
                Wisdom = 0.2f,
                Luck = 1.0f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth { Strength = 1.0f, Vitality = 0.8f, Agility = 2.5f, Stamina = 1.2f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 1.2f } },
                { 10, new StatGrowth { Strength = 1.5f, Vitality = 1.0f, Agility = 3.0f, Stamina = 1.5f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 1.5f } },
                { 20, new StatGrowth { Strength = 2.0f, Vitality = 1.5f, Agility = 3.5f, Stamina = 2.0f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 2.0f } },
                { 40, new StatGrowth { Strength = 3.0f, Vitality = 2.0f, Agility = 5.0f, Stamina = 2.5f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 2.5f } },
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, -1.4f, 0f),
                Scale = new Vector3(5f, 5f, 0),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Pugilist}"),
            Details = new ActorDetails
            {
                Description = "A disciplined master of martial strikes.",
                Card = "Has a chance to counterattack with <color=#FFAA00>[Flurry]</color> when evading an attack.",
                Lore = new List<string> { "Once punched a bear", "Trains in silence" }
            }
        };
    }
}
