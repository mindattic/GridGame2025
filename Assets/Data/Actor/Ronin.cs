using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
public static class Ronin
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.Ronin,
            Description = "A masterless warrior guided by honor.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 8,
                Vitality = 6,
                Agility = 5,
                Stamina = 4,
                Intelligence = 2,
                Wisdom = 3,
                Luck = 4
            },
            StatGrowth = new StatGrowth
            {
                Strength = 2.0f,
                Vitality = 1.2f,
                Agility = 1.0f,
                Stamina = 1.0f,
                Intelligence = 0.4f,
                Wisdom = 0.8f,
                Luck = 0.8f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth { Strength = 2.5f, Vitality = 1.5f, Agility = 1.2f, Stamina = 1.0f, Intelligence = 0.5f, Wisdom = 1.0f, Luck = 1.0f } },
                { 10, new StatGrowth { Strength = 3.0f, Vitality = 2.0f, Agility = 1.5f, Stamina = 1.5f, Intelligence = 1.0f, Wisdom = 1.5f, Luck = 1.2f } },
                { 20, new StatGrowth { Strength = 4.0f, Vitality = 2.5f, Agility = 2.0f, Stamina = 2.0f, Intelligence = 1.5f, Wisdom = 2.0f, Luck = 1.5f } },
                { 40, new StatGrowth { Strength = 5.0f, Vitality = 3.0f, Agility = 2.5f, Stamina = 2.5f, Intelligence = 2.0f, Wisdom = 2.5f, Luck = 2.0f } },
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.35f, -1.34f, 0f),
                Scale = new Vector3(5f, 5f, 0f),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.Ronin}"),
            Details = new ActorDetails
            {
                Description = "A masterless warrior guided by honor.",
                Card = "Has a chance to <color=#FFFF00>[Counter]</color> when attacked. Deals extra damage on the first strike.",
                Lore = new List<string>
                {
                    "Once served a great house",
                    "Walks the path of redemption"
                }
            }
        };
    }
}
