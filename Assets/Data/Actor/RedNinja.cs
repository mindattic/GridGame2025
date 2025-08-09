using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
public static class RedNinja
{
    public static ActorData Data()
    {
        return new ActorData
        {
            Character = CharacterHelper.RedNinja,
            Description = "A deadly assassin wielding forbidden arts.",
            BaseStats = new ActorStats
            {
                Level = 1,
                Strength = 7,
                Vitality = 4,
                Agility = 7,
                Stamina = 5,
                Intelligence = 3,
                Wisdom = 2,
                Luck = 5
            },
            StatGrowth = new StatGrowth
            {
                Strength = 1.8f,
                Vitality = 1.0f,
                Agility = 1.8f,
                Stamina = 1.2f,
                Intelligence = 0.6f,
                Wisdom = 0.5f,
                Luck = 1.2f
            },
            MilestoneStatGrowth = new Dictionary<int, StatGrowth>
            {
                { 5, new StatGrowth { Strength = 2.0f, Vitality = 1.0f, Agility = 2.2f, Stamina = 1.2f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 1.2f } },
                { 10, new StatGrowth { Strength = 2.5f, Vitality = 1.2f, Agility = 2.5f, Stamina = 1.5f, Intelligence = 1.0f, Wisdom = 0.8f, Luck = 1.5f } },
                { 20, new StatGrowth { Strength = 3.0f, Vitality = 1.5f, Agility = 3.0f, Stamina = 2.0f, Intelligence = 1.5f, Wisdom = 1.0f, Luck = 2.0f } },
                { 40, new StatGrowth { Strength = 4.0f, Vitality = 2.0f, Agility = 4.0f, Stamina = 2.5f, Intelligence = 2.0f, Wisdom = 1.5f, Luck = 2.5f } },
            },
            Stats = new ActorStats(),
            ThumbnailSettings = new ThumbnailSettings
            {
                Position = new Vector3(0.5f, -1.4f, 0f),
                Scale = new Vector3(5f, 5f, 0),
            },
            Portrait = AssetHelper.LoadAsset<Sprite>($"{g.TextureResolution.ToInt()}/{CharacterHelper.RedNinja}"),
            Details = new ActorDetails
            {
                Description = "A deadly assassin wielding forbidden arts.",
                Card = "Applies <color=#990000>[Poison]</color> with melee attacks. May <color=#AA0000>[Vanish]</color> when hit.",
                Lore = new List<string> { "Trained in the shadows", "Seeks vengeance" }
            }
        };
    }
}
