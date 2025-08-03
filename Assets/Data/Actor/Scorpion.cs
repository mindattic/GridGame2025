using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;
public static class Scorpion
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Scorpion,
            Description = "A hulking brute with a barbed tail and armored shell.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 7,
                Vitality = 1,
                Agility = 2,
                Stamina = 7,
                Intelligence = 1,
                Wisdom = 2,
                Luck = 3
            },
            StatGrowth = new StatGrowth
            {
                Strength = 1.5f,
                Vitality = 2.5f,
                Agility = 0.5f,
                Stamina = 2.0f,
                Intelligence = 0.4f,
                Wisdom = 0.6f,
                Luck = 0.6f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth(2.0f, 3.5f, 0.8f, 2.5f, 0.5f, 0.8f, 1.0f) },
                { 10, new StatGrowth(2.5f, 4.5f, 1.0f, 3.0f, 0.8f, 1.0f, 1.2f) },
                { 20, new StatGrowth(3.0f, 6.0f, 1.2f, 4.0f, 1.0f, 1.2f, 1.5f) },
                { 40, new StatGrowth(4.0f, 8.0f, 1.5f, 5.0f, 1.5f, 1.5f, 2.0f) }
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, 0.28f, 0f),
                Scale = new Vector3(2f, 2f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Scorpion}"),
            Details = new ActorDetails
            {
                Description = "A hulking brute with a barbed tail and armored shell.",
                Card = "Takes <color=#00FF00>[reduced damage]</color> from frontal attacks. Has a chance to <color=#FF3300>[counterattack]</color> when hit.",
                Lore = new List<string>
                {
                    "Harder than stone",
                    "Feels no pain"
                }
            }
        };
    }
}
