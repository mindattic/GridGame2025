using Assets.Helpers;
using Assets.Scripts.Libraries;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using Tag = ActorTag;

namespace Assets.Data.Actor
{

    /*

    public static class CharacterTemplate
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CharacterNameHere",
                CharacterClass = CharacterClass.None, // Replace with proper enum value
                Tags = Tag.Hero | Tag.Humanoid, // Adjust tags as needed
                Description = "Short character description.",
                Expectations = "Gameplay summary (role, strengths, weaknesses).",
                Lore = "Brief lore text.",
                Card = "Summary card text. Include [keywords] for clarity.",

                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 0f,
                    Vitality = 0f,
                    Agility = 0f,
                    Speed = 0f,
                    Stamina = 0f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0f,
                    Vitality = 0f,
                    Agility = 0f,
                    Speed = 0f,
                    Stamina = 0f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0f, Vitality = 0f, Agility = 0f, Speed = 0f, Stamina = 0f, Intelligence = 0f, Wisdom = 0f, Luck = 0f } },
                    { 10, new StatGrowth { Strength = 0f, Vitality = 0f, Agility = 0f, Speed = 0f, Stamina = 0f, Intelligence = 0f, Wisdom = 0f, Luck = 0f } },
                    { 20, new StatGrowth { Strength = 0f, Vitality = 0f, Agility = 0f, Speed = 0f, Stamina = 0f, Intelligence = 0f, Wisdom = 0f, Luck = 0f } },
                    { 40, new StatGrowth { Strength = 0f, Vitality = 0f, Agility = 0f, Speed = 0f, Stamina = 0f, Intelligence = 0f, Wisdom = 0f, Luck = 0f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0f, 0f, 0f),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.None}") // Replace with class name
            };
        }
    }

    */

    public static class Alchemist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Alchemist",
                CharacterClass = CharacterClass.Alchemist, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A battlefield support that sustains allies and manipulates tempo.",
                Expectations = "Backline support. Uses [heal], [cleanse], and [buff] to stabilize fights.",
                Lore = "Knows every reagent, every breath, and when to spend both.",
                Card = "Backline [healer][buffer]. Restores tempo with [cleanse] and [aid].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 15f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.60f,

                    Speed = 0.65f,

                    Stamina = 0.65f,
                    Intelligence = 0.95f,
                    Wisdom = 0.90f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.21f, Speed = 0.23f, Stamina = 0.23f, Intelligence = 0.33f, Wisdom = 0.32f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.27f, Speed = 0.29f, Stamina = 0.29f, Intelligence = 0.43f, Wisdom = 0.41f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.36f, Speed = 0.39f, Stamina = 0.39f, Intelligence = 0.57f, Wisdom = 0.54f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.48f, Speed = 0.52f, Stamina = 0.52f, Intelligence = 0.76f, Wisdom = 0.72f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Alchemist}")
            };
        }
    }

    public static class Assassain
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Assassain",
                CharacterClass = CharacterClass.Assassain, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Assassain}")
            };
        }
    }

    public static class Barbarian
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Barbarian",
                CharacterClass = CharacterClass.Barbarian, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Barbarian}")
            };
        }
    }

    public static class Basher
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Basher",
                CharacterClass = CharacterClass.Basher, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Basher}")
            };
        }
    }


    public static class Bat00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Bat00",
                CharacterClass = CharacterClass.Bat00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Flying,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings(new Vector2Int(235, 796), new Vector2(2f, 2f), 1024),

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Bat00}")
            };
        }
    }

    public static class Bat01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Bat01",
                CharacterClass = CharacterClass.Bat01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Flying,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Bat01}")
            };
        }
    }

    public static class Bat02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Bat02",
                CharacterClass = CharacterClass.Bat02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Flying,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Bat02}")
            };
        }
    }
    public static class BlackNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "BlackNinja",
                CharacterClass = CharacterClass.BlackNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.BlackNinja}")
            };
        }
    }

    public static class BlackWitch
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "BlackWitch",
                CharacterClass = CharacterClass.BlackWitch, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid | Tag.Magic,

                Description = "A ranged spellcaster that shapes the fight with arcane control and damage.",
                Expectations = "Midline control. Uses [debuff], [aoe], and [curse]. Keep safely behind the front.",
                Lore = "Disciplines of ink and flame taught that power begins in focus.",
                Card = "Arcane [controller][debuffer]. Shapes fights with [aoe][curse].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 7f,
                    Vitality = 8f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 10f,
                    Intelligence = 20f,
                    Wisdom = 18f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.35f,
                    Vitality = 0.45f,
                    Agility = 0.55f,

                    Speed = 0.60f,

                    Stamina = 0.55f,
                    Intelligence = 1.20f,
                    Wisdom = 1.00f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.12f, Vitality = 0.16f, Agility = 0.19f, Speed = 0.21f, Stamina = 0.19f, Intelligence = 0.42f, Wisdom = 0.35f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.16f, Vitality = 0.2f, Agility = 0.25f, Speed = 0.27f, Stamina = 0.25f, Intelligence = 0.54f, Wisdom = 0.45f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.21f, Vitality = 0.27f, Agility = 0.33f, Speed = 0.36f, Stamina = 0.33f, Intelligence = 0.72f, Wisdom = 0.6f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.28f, Vitality = 0.36f, Agility = 0.44f, Speed = 0.48f, Stamina = 0.44f, Intelligence = 0.96f, Wisdom = 0.8f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.BlackWitch}")
            };
        }
    }

    public static class BlueLion
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "BlueLion",
                CharacterClass = CharacterClass.BlueLion, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(5f, 5f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.BlueLion}")
            };
        }
    }

    public static class BlueNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "BlueNinja",
                CharacterClass = CharacterClass.BlueNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.BlueNinja}")
            };
        }
    }

    public static class Bruiser
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Bruiser",
                CharacterClass = CharacterClass.Bruiser, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Bruiser}")
            };
        }
    }

    public static class Captain
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Captain00",
                CharacterClass = CharacterClass.Captain, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Captain}")
            };
        }
    }

    public static class ChromaNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "ChromaNinja",
                CharacterClass = CharacterClass.ChromaNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.ChromaNinja}")
            };
        }
    }

    public static class Cleric
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cleric",
                CharacterClass = CharacterClass.Cleric, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A battlefield support that sustains allies and manipulates tempo.",
                Expectations = "Backline support. Uses [heal], [cleanse], and [buff] to stabilize fights.",
                Lore = "Knows every reagent, every breath, and when to spend both.",
                Card = "Backline [healer][buffer]. Restores tempo with [cleanse] and [aid].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 15f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.60f,

                    Speed = 0.65f,

                    Stamina = 0.65f,
                    Intelligence = 0.95f,
                    Wisdom = 0.90f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.21f, Speed = 0.23f, Stamina = 0.23f, Intelligence = 0.33f, Wisdom = 0.32f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.27f, Speed = 0.29f, Stamina = 0.29f, Intelligence = 0.43f, Wisdom = 0.41f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.36f, Speed = 0.39f, Stamina = 0.39f, Intelligence = 0.57f, Wisdom = 0.54f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.48f, Speed = 0.52f, Stamina = 0.52f, Intelligence = 0.76f, Wisdom = 0.72f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings(new Vector2Int(414, 230), new Vector2(5f, 5f), 1024),

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cleric}")
            };
        }
    }

    public static class Courier
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Courier",
                CharacterClass = CharacterClass.Courier, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A battlefield support that sustains allies and manipulates tempo.",
                Expectations = "Backline support. Uses [heal], [cleanse], and [buff] to stabilize fights.",
                Lore = "Knows every reagent, every breath, and when to spend both.",
                Card = "Backline [healer][buffer]. Restores tempo with [cleanse] and [aid].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 15f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.60f,

                    Speed = 0.65f,

                    Stamina = 0.65f,
                    Intelligence = 0.95f,
                    Wisdom = 0.90f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.21f, Speed = 0.23f, Stamina = 0.23f, Intelligence = 0.33f, Wisdom = 0.32f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.27f, Speed = 0.29f, Stamina = 0.29f, Intelligence = 0.43f, Wisdom = 0.41f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.36f, Speed = 0.39f, Stamina = 0.39f, Intelligence = 0.57f, Wisdom = 0.54f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.48f, Speed = 0.52f, Stamina = 0.52f, Intelligence = 0.76f, Wisdom = 0.72f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Courier}")
            };
        }
    }

    public static class CyberZombie00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CyberZombie00",
                CharacterClass = CharacterClass.CyberZombie00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Mechanical | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CyberZombie00}")
            };
        }
    }

    public static class CyberZombie01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CyberZombie01",
                CharacterClass = CharacterClass.CyberZombie01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Mechanical | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CyberZombie01}")
            };
        }
    }

    public static class CyberZombie02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CyberZombie02",
                CharacterClass = CharacterClass.CyberZombie02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Mechanical | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CyberZombie02}")
            };
        }
    }

    public static class CyberZombie03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CyberZombie03",
                CharacterClass = CharacterClass.CyberZombie03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Mechanical | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CyberZombie03}")
            };
        }
    }

    public static class CyberZombie04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CyberZombie04",
                CharacterClass = CharacterClass.CyberZombie04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Mechanical | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CyberZombie04}")
            };
        }
    }

    public static class Cyclops00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cyclops00",
                CharacterClass = CharacterClass.Cyclops00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cyclops00}")
            };
        }
    }

    public static class Cyclops01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cyclops01",
                CharacterClass = CharacterClass.Cyclops01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cyclops01}")
            };
        }
    }

    public static class Cyclops02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cyclops02",
                CharacterClass = CharacterClass.Cyclops02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cyclops02}")
            };
        }
    }

    public static class Cyclops03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cyclops03",
                CharacterClass = CharacterClass.Cyclops03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cyclops03}")
            };
        }
    }

    public static class Cyclops04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cyclops04",
                CharacterClass = CharacterClass.Cyclops04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cyclops04}")
            };
        }
    }

    public static class Cyclops06
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Cyclops06",
                CharacterClass = CharacterClass.Cyclops06, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cyclops06}")
            };
        }
    }

    public static class DarkTemplar
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "DarkTemplar",
                CharacterClass = CharacterClass.DarkTemplar, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.DarkTemplar}")
            };
        }
    }

    public static class Defender
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Defender",
                CharacterClass = CharacterClass.Defender, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Defender}")
            };
        }
    }

    public static class DemonLord
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "DemonLord",
                CharacterClass = CharacterClass.DemonLord, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.DemonLord}")
            };
        }
    }

    public static class Dervish
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Dervish",
                CharacterClass = CharacterClass.Dervish, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A battlefield support that sustains allies and manipulates tempo.",
                Expectations = "Backline support. Uses [heal], [cleanse], and [buff] to stabilize fights.",
                Lore = "Knows every reagent, every breath, and when to spend both.",
                Card = "Backline [healer][buffer]. Restores tempo with [cleanse] and [aid].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 15f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.60f,

                    Speed = 0.65f,

                    Stamina = 0.65f,
                    Intelligence = 0.95f,
                    Wisdom = 0.90f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.21f, Speed = 0.23f, Stamina = 0.23f, Intelligence = 0.33f, Wisdom = 0.32f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.27f, Speed = 0.29f, Stamina = 0.29f, Intelligence = 0.43f, Wisdom = 0.41f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.36f, Speed = 0.39f, Stamina = 0.39f, Intelligence = 0.57f, Wisdom = 0.54f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.48f, Speed = 0.52f, Stamina = 0.52f, Intelligence = 0.76f, Wisdom = 0.72f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Dervish}")
            };
        }
    }

    public static class Doctor
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Doctor",
                CharacterClass = CharacterClass.Doctor, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A battlefield support that sustains allies and manipulates tempo.",
                Expectations = "Backline support. Uses [heal], [cleanse], and [buff] to stabilize fights.",
                Lore = "Knows every reagent, every breath, and when to spend both.",
                Card = "Backline [healer][buffer]. Restores tempo with [cleanse] and [aid].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 15f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.60f,

                    Speed = 0.65f,

                    Stamina = 0.65f,
                    Intelligence = 0.95f,
                    Wisdom = 0.90f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.21f, Speed = 0.23f, Stamina = 0.23f, Intelligence = 0.33f, Wisdom = 0.32f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.27f, Speed = 0.29f, Stamina = 0.29f, Intelligence = 0.43f, Wisdom = 0.41f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.36f, Speed = 0.39f, Stamina = 0.39f, Intelligence = 0.57f, Wisdom = 0.54f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.48f, Speed = 0.52f, Stamina = 0.52f, Intelligence = 0.76f, Wisdom = 0.72f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Doctor}")
            };
        }
    }

    public static class Drifter
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Drifter",
                CharacterClass = CharacterClass.Drifter, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A battlefield support that sustains allies and manipulates tempo.",
                Expectations = "Backline support. Uses [heal], [cleanse], and [buff] to stabilize fights.",
                Lore = "Knows every reagent, every breath, and when to spend both.",
                Card = "Backline [healer][buffer]. Restores tempo with [cleanse] and [aid].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 15f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.60f,

                    Speed = 0.65f,

                    Stamina = 0.65f,
                    Intelligence = 0.95f,
                    Wisdom = 0.90f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.21f, Speed = 0.23f, Stamina = 0.23f, Intelligence = 0.33f, Wisdom = 0.32f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.27f, Speed = 0.29f, Stamina = 0.29f, Intelligence = 0.43f, Wisdom = 0.41f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.36f, Speed = 0.39f, Stamina = 0.39f, Intelligence = 0.57f, Wisdom = 0.54f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.48f, Speed = 0.52f, Stamina = 0.52f, Intelligence = 0.76f, Wisdom = 0.72f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Drifter}")
            };
        }
    }

    public static class Duelist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Duelist",
                CharacterClass = CharacterClass.Duelist, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Duelist}")
            };
        }
    }

    public static class Engineer
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Engineer",
                CharacterClass = CharacterClass.Engineer, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Engineer}")
            };
        }
    }

    public static class Fencer
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Fencer",
                CharacterClass = CharacterClass.Fencer, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Fencer}")
            };
        }
    }

    public static class Fighter
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Fighter",
                CharacterClass = CharacterClass.Fighter, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Fighter}")
            };
        }
    }

    public static class FlyingMonkey
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "FlyingMonkey",
                CharacterClass = CharacterClass.FlyingMonkey, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Flying,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.FlyingMonkey}")
            };
        }
    }

    public static class Frog00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Frog00",
                CharacterClass = CharacterClass.Frog00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Aquatic,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Frog00}")
            };
        }
    }

    public static class Frog01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Frog01",
                CharacterClass = CharacterClass.Frog01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Aquatic,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Frog01}")
            };
        }
    }

    public static class Frog02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Frog02",
                CharacterClass = CharacterClass.Frog02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Aquatic,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Frog02}")
            };
        }
    }

    public static class Frog03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Frog_03",
                CharacterClass = CharacterClass.Frog03,
                Tags = Tag.Aquatic,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Frog03}")
            };
        }
    }

    public static class Ganger00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger00",
                CharacterClass = CharacterClass.Ganger00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger00}")
            };
        }
    }

    public static class Ganger01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger01",
                CharacterClass = CharacterClass.Ganger01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger01}")
            };
        }
    }

    public static class Ganger02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger02",
                CharacterClass = CharacterClass.Ganger02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger02}")
            };
        }
    }

    public static class Ganger03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger03",
                CharacterClass = CharacterClass.Ganger03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger03}")
            };
        }
    }

    public static class Ganger04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger04",
                CharacterClass = CharacterClass.Ganger04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger04}")
            };
        }
    }

    public static class Ganger05
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger05",
                CharacterClass = CharacterClass.Ganger05, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger05}")
            };
        }
    }

    public static class Ganger06
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ganger06",
                CharacterClass = CharacterClass.Ganger06, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ganger06}")
            };
        }
    }

    public static class Ghost
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ghost",
                CharacterClass = CharacterClass.Ghost, // If this enum does not exist, replace accordingly.
                Tags = Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ghost}")
            };
        }
    }

    public static class GoblinThug00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "GoblinThug00",
                CharacterClass = CharacterClass.GoblinThug00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.GoblinThug00}")
            };
        }
    }

    public static class GreenNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "GreenNinja",
                CharacterClass = CharacterClass.GreenNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.GreenNinja}")
            };
        }
    }

    public static class Hag00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Hag00",
                CharacterClass = CharacterClass.Hag00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Hag00}")
            };
        }
    }

    public static class Hag01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Hag01",
                CharacterClass = CharacterClass.Hag01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Hag01}")
            };
        }
    }

    public static class Hag02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Hag02",
                CharacterClass = CharacterClass.Hag02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Hag02}")
            };
        }
    }

    public static class Hag03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Hag03",
                CharacterClass = CharacterClass.Hag03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Hag03}")
            };
        }
    }

    public static class Harbinger
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Harbinger",
                CharacterClass = CharacterClass.Harbinger, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Harbinger}")
            };
        }
    }


    public static class IceMauler
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "IceMauler",
                CharacterClass = CharacterClass.IceMauler, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.IceMauler}")
            };
        }
    }

    public static class JadeKnight
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "JadeKnight",
                CharacterClass = CharacterClass.JadeKnight, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.JadeKnight}")
            };
        }
    }

    public static class Knight
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Knight",
                CharacterClass = CharacterClass.Knight, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Knight}")
            };
        }
    }

    public static class Lancer
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Lancer",
                CharacterClass = CharacterClass.Lancer, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Lancer}")
            };
        }
    }

    public static class Lurker00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Lurker00",
                CharacterClass = CharacterClass.Lurker00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Lurker00}")
            };
        }
    }

    public static class Lurker01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Lurker01",
                CharacterClass = CharacterClass.Lurker01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Lurker01}")
            };
        }
    }

    public static class Lurker02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Lurker02",
                CharacterClass = CharacterClass.Lurker02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Lurker02}")
            };
        }
    }

    public static class Machinist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Machinist",
                CharacterClass = CharacterClass.Machinist, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Machinist}")
            };
        }
    }

    public static class Mannequin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Mannequin",
                CharacterClass = CharacterClass.Mannequin, // If this enum does not exist, replace accordingly.
                Tags = Tag.Humanoid,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Mannequin}")
            };
        }
    }

    public static class MarshShambler00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MarshShambler00",
                CharacterClass = CharacterClass.MarshShambler00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MarshShambler00}")
            };
        }
    }

    public static class MarshShambler01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MarshShambler01",
                CharacterClass = CharacterClass.MarshShambler01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MarshShambler01}")
            };
        }
    }

    public static class MarshShambler03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MarshShambler03",
                CharacterClass = CharacterClass.MarshShambler03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MarshShambler03}")
            };
        }
    }

    public static class MartialArtist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MartialArtist",
                CharacterClass = CharacterClass.MartialArtist, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MartialArtist}")
            };
        }
    }

    public static class MechaArmor00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MechaArmor00",
                CharacterClass = CharacterClass.MechaArmor00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MechaArmor00}")
            };
        }
    }

    public static class MechaArmor01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MechaArmor01",
                CharacterClass = CharacterClass.MechaArmor01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MechaArmor01}")
            };
        }
    }

    public static class MechaArmor02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MechaArmor02",
                CharacterClass = CharacterClass.MechaArmor02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MechaArmor02}")
            };
        }
    }

    public static class Monk
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Monk",
                CharacterClass = CharacterClass.Monk, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Monk}")
            };
        }
    }

    public static class MountainTroll
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "MountainTroll",
                CharacterClass = CharacterClass.MountainTroll, // If this enum does not exist, replace accordingly.
                Tags = Tag.Humanoid,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.MountainTroll}")
            };
        }
    }

    public static class Myrmidon
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Myrmidon",
                CharacterClass = CharacterClass.Myrmidon, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Myrmidon}")
            };
        }
    }

    public static class Naga00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Naga00",
                CharacterClass = CharacterClass.Naga00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Aquatic,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Naga00}")
            };
        }
    }

    public static class NightHunter
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "NightHunter",
                CharacterClass = CharacterClass.NightHunter, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.NightHunter}")
            };
        }
    }

    public static class Odachi
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Odachi",
                CharacterClass = CharacterClass.Odachi, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Odachi}")
            };
        }
    }

    public static class Oni00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Oni00",
                CharacterClass = CharacterClass.Oni00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Oni00}")
            };
        }
    }

    public static class Oni01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Oni01",
                CharacterClass = CharacterClass.Oni01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Oni01}")
            };
        }
    }

    public static class Oni02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Oni02",
                CharacterClass = CharacterClass.Oni02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Oni02}")
            };
        }
    }

    public static class Operative
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Operative",
                CharacterClass = CharacterClass.Operative, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Operative}")
            };
        }
    }
   
    public static class Paladin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Paladin",
                CharacterClass = CharacterClass.Paladin, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings(new Vector2Int(428, 230), new Vector2(5f, 5f), 1024),

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Paladin}")
            };
        }
    }
    public static class CeramicKnight00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CeramicKnight00",
                CharacterClass = CharacterClass.CeramicKnight00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight00}")
            };
        }
    }

    public static class CeramicKnight01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CeramicKnight01",
                CharacterClass = CharacterClass.CeramicKnight01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight01}")
            };
        }
    }

    public static class CeramicKnight02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ceramicnight02",
                CharacterClass = CharacterClass.CeramicKnight02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight02}")
            };
        }
    }

    public static class CeramicKnight03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CeramicKnight03",
                CharacterClass = CharacterClass.CeramicKnight03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight03}")
            };
        }
    }

    public static class CeramicKnight04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CeramicKnight04",
                CharacterClass = CharacterClass.CeramicKnight04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight04}")
            };
        }
    }

    public static class CeramicKnight05
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CeramicKnight05",
                CharacterClass = CharacterClass.CeramicKnight05, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight05}")
            };
        }
    }

    public static class CeramicKnight06
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "CeramicKnight06",
                CharacterClass = CharacterClass.CeramicKnight06, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.CeramicKnight06}")
            };
        }
    }

    public static class PandaGirl
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PandaGirl",
                CharacterClass = CharacterClass.PandaGirl, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PandaGirl}")
            };
        }
    }

    public static class Phantom
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Phantom",
                CharacterClass = CharacterClass.Phantom, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Phantom}")
            };
        }
    }

    public static class PrizeFighter
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PrizeFighter",
                CharacterClass = CharacterClass.PrizeFighter, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PrizeFighter}")
            };
        }
    }

    public static class Pugilist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Pugilist",
                CharacterClass = CharacterClass.Pugilist, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Pugilist}")
            };
        }
    }

    public static class PurplePrototype00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PurplePrototype00",
                CharacterClass = CharacterClass.PurplePrototype00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PurplePrototype00}")
            };
        }
    }

    public static class PurplePrototype01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PurplePrototype01",
                CharacterClass = CharacterClass.PurplePrototype01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PurplePrototype01}")
            };
        }
    }

    public static class PurplePrototype02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PurplePrototype02",
                CharacterClass = CharacterClass.PurplePrototype02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PurplePrototype02}")
            };
        }
    }

    public static class PurplePrototype03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PurplePrototype03",
                CharacterClass = CharacterClass.PurplePrototype03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PurplePrototype03}")
            };
        }
    }

    public static class PurplePrototype04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "PurplePrototype04",
                CharacterClass = CharacterClass.PurplePrototype04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.PurplePrototype04}")
            };
        }
    }

    public static class Raider
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Raider",
                CharacterClass = CharacterClass.Raider, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Raider}")
            };
        }
    }

    public static class Reaper
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Reaper",
                CharacterClass = CharacterClass.Reaper, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Reaper}")
            };
        }
    }

    public static class RedMage
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "RedMage",
                CharacterClass = CharacterClass.RedMage, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid | Tag.Magic,

                Description = "A ranged spellcaster that shapes the fight with arcane control and damage.",
                Expectations = "Midline control. Uses [debuff], [aoe], and [curse]. Keep safely behind the front.",
                Lore = "Disciplines of ink and flame taught that power begins in focus.",
                Card = "Arcane [controller][debuffer]. Shapes fights with [aoe][curse].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 7f,
                    Vitality = 8f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 10f,
                    Intelligence = 20f,
                    Wisdom = 18f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.35f,
                    Vitality = 0.45f,
                    Agility = 0.55f,

                    Speed = 0.60f,

                    Stamina = 0.55f,
                    Intelligence = 1.20f,
                    Wisdom = 1.00f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.12f, Vitality = 0.16f, Agility = 0.19f, Speed = 0.21f, Stamina = 0.19f, Intelligence = 0.42f, Wisdom = 0.35f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.16f, Vitality = 0.2f, Agility = 0.25f, Speed = 0.27f, Stamina = 0.25f, Intelligence = 0.54f, Wisdom = 0.45f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.21f, Vitality = 0.27f, Agility = 0.33f, Speed = 0.36f, Stamina = 0.33f, Intelligence = 0.72f, Wisdom = 0.6f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.28f, Vitality = 0.36f, Agility = 0.44f, Speed = 0.48f, Stamina = 0.44f, Intelligence = 0.96f, Wisdom = 0.8f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.RedMage}")
            };
        }
    }

    public static class RedNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "RedNinja",
                CharacterClass = CharacterClass.RedNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.RedNinja}")
            };
        }
    }

    public static class Ripper
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ripper",
                CharacterClass = CharacterClass.Ripper, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ripper}")
            };
        }
    }

    public static class Ritualist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ritualist",
                CharacterClass = CharacterClass.Ritualist, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid | Tag.Magic,

                Description = "A ranged spellcaster that shapes the fight with arcane control and damage.",
                Expectations = "Midline control. Uses [debuff], [aoe], and [curse]. Keep safely behind the front.",
                Lore = "Disciplines of ink and flame taught that power begins in focus.",
                Card = "Arcane [controller][debuffer]. Shapes fights with [aoe][curse].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 7f,
                    Vitality = 8f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 10f,
                    Intelligence = 20f,
                    Wisdom = 18f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.35f,
                    Vitality = 0.45f,
                    Agility = 0.55f,

                    Speed = 0.60f,

                    Stamina = 0.55f,
                    Intelligence = 1.20f,
                    Wisdom = 1.00f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.12f, Vitality = 0.16f, Agility = 0.19f, Speed = 0.21f, Stamina = 0.19f, Intelligence = 0.42f, Wisdom = 0.35f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.16f, Vitality = 0.2f, Agility = 0.25f, Speed = 0.27f, Stamina = 0.25f, Intelligence = 0.54f, Wisdom = 0.45f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.21f, Vitality = 0.27f, Agility = 0.33f, Speed = 0.36f, Stamina = 0.33f, Intelligence = 0.72f, Wisdom = 0.6f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.28f, Vitality = 0.36f, Agility = 0.44f, Speed = 0.48f, Stamina = 0.44f, Intelligence = 0.96f, Wisdom = 0.8f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ritualist}")
            };
        }
    }

    public static class Ronin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Ronin",
                CharacterClass = CharacterClass.Ronin, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ronin}")
            };
        }
    }

    public static class Sage
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Sage",
                CharacterClass = CharacterClass.Sage, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid | Tag.Magic,

                Description = "A ranged spellcaster that shapes the fight with arcane control and damage.",
                Expectations = "Midline control. Uses [debuff], [aoe], and [curse]. Keep safely behind the front.",
                Lore = "Disciplines of ink and flame taught that power begins in focus.",
                Card = "Arcane [controller][debuffer]. Shapes fights with [aoe][curse].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 7f,
                    Vitality = 8f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 10f,
                    Intelligence = 20f,
                    Wisdom = 18f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.35f,
                    Vitality = 0.45f,
                    Agility = 0.55f,

                    Speed = 0.60f,

                    Stamina = 0.55f,
                    Intelligence = 1.20f,
                    Wisdom = 1.00f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.12f, Vitality = 0.16f, Agility = 0.19f, Speed = 0.21f, Stamina = 0.19f, Intelligence = 0.42f, Wisdom = 0.35f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.16f, Vitality = 0.2f, Agility = 0.25f, Speed = 0.27f, Stamina = 0.25f, Intelligence = 0.54f, Wisdom = 0.45f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.21f, Vitality = 0.27f, Agility = 0.33f, Speed = 0.36f, Stamina = 0.33f, Intelligence = 0.72f, Wisdom = 0.6f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.28f, Vitality = 0.36f, Agility = 0.44f, Speed = 0.48f, Stamina = 0.44f, Intelligence = 0.96f, Wisdom = 0.8f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Sage}")
            };
        }
    }

    public static class SandMaw
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SandMaw",
                CharacterClass = CharacterClass.SandMaw, // If this enum does not exist, replace accordingly.
                Tags = Tag.None,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SandMaw}")
            };
        }
    }

    public static class Scorpion
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Scorpion",
                CharacterClass = CharacterClass.Scorpion, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings(new Vector2Int(214, 588), new Vector2(3f, 3f), 1024),

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Scorpion}")
            };
        }
    }

    public static class Sellsword
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Sellsword",
                CharacterClass = CharacterClass.Sellsword, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Sellsword}")
            };
        }
    }

    public static class ShieldMaiden
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "ShieldMaiden",
                CharacterClass = CharacterClass.ShieldMaiden, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.ShieldMaiden}")
            };
        }
    }

    public static class Sister
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Sister",
                CharacterClass = CharacterClass.Sister, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Sister}")
            };
        }
    }

    public static class Skelepede00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Skelepede00",
                CharacterClass = CharacterClass.Skelepede00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Insect | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Skelepede00}")
            };
        }
    }

    public static class Skelepede01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Skelepede01",
                CharacterClass = CharacterClass.Skelepede01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Insect | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Skelepede01}")
            };
        }
    }

    public static class Skelepede02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Skelepede02",
                CharacterClass = CharacterClass.Skelepede02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Insect | Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Skelepede02}")
            };
        }
    }

    public static class Slasher
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Slasher",
                CharacterClass = CharacterClass.Slasher, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slasher}")
            };
        }
    }

    public static class Slime00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Slime00",
                CharacterClass = CharacterClass.Slime00, // If this enum does not exist, replace accordingly.
                Tags = Tag.None,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(2f, 2f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime00}")
            };
        }
    }

    public static class Slime01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Slime01",
                CharacterClass = CharacterClass.Slime01, // If this enum does not exist, replace accordingly.
                Tags = Tag.None,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(2f, 2f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime01}")
            };
        }
    }

    public static class Slime02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Slime02",
                CharacterClass = CharacterClass.Slime02, // If this enum does not exist, replace accordingly.
                Tags = Tag.None,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(2f, 2f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime02}")
            };
        }
    }

    public static class Slime03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Slime03",
                CharacterClass = CharacterClass.Slime03, // If this enum does not exist, replace accordingly.
                Tags = Tag.None,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(2f, 2f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime03}")
            };
        }
    }

    public static class Soldier00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Soldier00",
                CharacterClass = CharacterClass.Soldier00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(2f, 2f, 0f),
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier00}")
            };
        }
    }
    public static class Soldier01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Soldier01",
                CharacterClass = CharacterClass.Soldier01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier01}")
            };
        }
    }

    public static class Soldier02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Soldier02",
                CharacterClass = CharacterClass.Soldier02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier02}")
            };
        }
    }

    public static class Soldier03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Soldier03",
                CharacterClass = CharacterClass.Soldier03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier03}")
            };
        }
    }

    public static class Speedster
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Speedster",
                CharacterClass = CharacterClass.Speedster, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Speedster}")
            };
        }
    }

    public static class SteppinRazor00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SteppinRazor00",
                CharacterClass = CharacterClass.SteppinRazor00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SteppinRazor00}")
            };
        }
    }

    public static class SteppinRazor01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SteppinRazor01",
                CharacterClass = CharacterClass.SteppinRazor01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SteppinRazor01}")
            };
        }
    }

    public static class SteppinRazor02
    {

        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SteppinRazor02",
                CharacterClass = CharacterClass.SteppinRazor02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SteppinRazor02}")
            };
        }
    }

    public static class SteppinRazor04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SteppinRazor04",
                CharacterClass = CharacterClass.SteppinRazor04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SteppinRazor04}")
            };
        }
    }

    public static class SteppinRazor05
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SteppinRazor05",
                CharacterClass = CharacterClass.SteppinRazor05, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SteppinRazor05}")
            };
        }
    }

    public static class StreetFighter
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "StreetFighter",
                CharacterClass = CharacterClass.StreetFighter, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.StreetFighter}")
            };
        }
    }
    public static class Striker
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Striker",
                CharacterClass = CharacterClass.Striker, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Striker}")
            };
        }
    }
    public static class SwampMistress00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SwampMistress00",
                CharacterClass = CharacterClass.SwampMistress00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SwampMistress00}")
            };
        }
    }

    public static class SwordMaster
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "SwordMaster",
                CharacterClass = CharacterClass.SwordMaster, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A close-quarters specialist that thrives in direct engagements.",
                Expectations = "Brawler. Uses [cleave], [stagger], and [execute]. Press close advantage.",
                Lore = "Every scar is a lesson. Every lesson becomes an opening.",
                Card = "Close-range [brawler][cleaver]. Wins trades with [stagger].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 16f,
                    Vitality = 14f,
                    Agility = 13f,

                    Speed = 13f,

                    Stamina = 15f,
                    Intelligence = 9f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.80f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.85f,
                    Intelligence = 0.45f,
                    Wisdom = 0.45f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.28f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.3f, Intelligence = 0.16f, Wisdom = 0.16f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.36f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.38f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.48f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.51f, Intelligence = 0.27f, Wisdom = 0.27f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.64f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.68f, Intelligence = 0.36f, Wisdom = 0.36f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.SwordMaster}")
            };
        }
    }

    public static class Tank
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Tank",
                CharacterClass = CharacterClass.Tank, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Tank}")
            };
        }
    }

    public static class TechGremlin00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TechGremlin00",
                CharacterClass = CharacterClass.TechGremlin00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TechGremlin00}")
            };
        }
    }

    public static class TechGremlin01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TechGremlin01",
                CharacterClass = CharacterClass.TechGremlin01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TechGremlin01}")
            };
        }
    }

    public static class TechGremlin02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TechGremlin02",
                CharacterClass = CharacterClass.TechGremlin02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TechGremlin02}")
            };
        }
    }

    public static class Technician
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Technician",
                CharacterClass = CharacterClass.Technician, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A gadgeteer who leverages devices, traps, and tactical tools.",
                Expectations = "Utility controller. Uses [trap], [turret], and [gadget] effects.",
                Lore = "Blueprints and boldness turned problems into levers and switches.",
                Card = "Tactical [engineer][controller]. Deploys [traps][turrets][gadgets].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 10f,
                    Vitality = 11f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 16f,
                    Wisdom = 14f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.55f,
                    Vitality = 0.60f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.95f,
                    Wisdom = 0.85f,
                    Luck = 0.60f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.19f, Vitality = 0.21f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.21f } },
                    { 10, new StatGrowth { Strength = 0.25f, Vitality = 0.27f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.43f, Wisdom = 0.38f, Luck = 0.27f } },
                    { 20, new StatGrowth { Strength = 0.33f, Vitality = 0.36f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.57f, Wisdom = 0.51f, Luck = 0.36f } },
                    { 40, new StatGrowth { Strength = 0.44f, Vitality = 0.48f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.76f, Wisdom = 0.68f, Luck = 0.48f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Technician}")
            };
        }
    }


    public static class Templar00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Templar00",
                CharacterClass = CharacterClass.Templar00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Templar00}")
            };
        }
    }

    public static class Templar01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Templar01",
                CharacterClass = CharacterClass.Templar01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Templar01}")
            };
        }
    }

    public static class Templar02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Templar02",
                CharacterClass = CharacterClass.Templar02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Templar02}")
            };
        }
    }

    public static class Templar03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Templar03",
                CharacterClass = CharacterClass.Templar03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Templar03}")
            };
        }
    }

    public static class Templar04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Templar04",
                CharacterClass = CharacterClass.Templar04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Templar04}")
            };
        }
    }

    public static class Templar05
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Templar05",
                CharacterClass = CharacterClass.Templar05, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Templar05}")
            };
        }
    }

    public static class Thief
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Thief",
                CharacterClass = CharacterClass.Thief, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Thief}")
            };
        }
    }

    public static class Tinkerer
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Tinkerer",
                CharacterClass = CharacterClass.Tinkerer, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Tinkerer}")
            };
        }
    }

    public static class Toad00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Toad00",
                CharacterClass = CharacterClass.Toad00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Aquatic,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Toad00}")
            };
        }
    }

    public static class TreeGolem00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TreeGolem00",
                CharacterClass = CharacterClass.TreeGolem00, // If this enum does not exist, replace accordingly.
                Tags = Tag.PlantBased | Tag.Mechanical,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TreeGolem00}")
            };
        }
    }

    public static class TreeGolem01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TreeGolem01",
                CharacterClass = CharacterClass.TreeGolem01, // If this enum does not exist, replace accordingly.
                Tags = Tag.PlantBased | Tag.Mechanical,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TreeGolem01}")
            };
        }
    }

    public static class TreeGolem02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TreeGolem02",
                CharacterClass = CharacterClass.TreeGolem02, // If this enum does not exist, replace accordingly.
                Tags = Tag.PlantBased | Tag.Mechanical,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TreeGolem02}")
            };
        }
    }

    public static class TreeGolem03
    {

        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TreeGolem03",
                CharacterClass = CharacterClass.TreeGolem03, // If this enum does not exist, replace accordingly.
                Tags = Tag.PlantBased | Tag.Mechanical,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TreeGolem03}")
            };
        }
    }


    public static class TreeGolem04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TreeGolem04",
                CharacterClass = CharacterClass.TreeGolem04, // If this enum does not exist, replace accordingly.
                Tags = Tag.PlantBased | Tag.Mechanical,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TreeGolem04}")
            };
        }
    }

    public static class TreeGolem06
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "TreeGolem06",
                CharacterClass = CharacterClass.TreeGolem06, // If this enum does not exist, replace accordingly.
                Tags = Tag.PlantBased | Tag.Mechanical,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.TreeGolem06}")
            };
        }
    }

    public static class Undead00
    {

        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Undead00",
                CharacterClass = CharacterClass.Undead00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Undead00}")
            };
        }
    }

    public static class Undead01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Undead01",
                CharacterClass = CharacterClass.Undead01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Undead01}")
            };
        }
    }

    public static class Undead02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Undead02",
                CharacterClass = CharacterClass.Undead02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Undead02}")
            };
        }
    }

    public static class Undead04
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Undead04",
                CharacterClass = CharacterClass.Undead04, // If this enum does not exist, replace accordingly.
                Tags = Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Undead04}")
            };
        }
    }

    public static class Vampire
    {

        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Vampire",
                CharacterClass = CharacterClass.Vampire, // If this enum does not exist, replace accordingly.
                Tags = Tag.Undead,

                Description = "An unnatural entity that endures through grim resilience and dread.",
                Expectations = "Attrition fighter. Uses [drain], [fear], and [resist] to grind opponents down.",
                Lore = "Life ended. Purpose remained. The rest is hunger and memory.",
                Card = "Grim [attrition][drain]. Pressures with [fear] and [rot].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 14f,
                    Vitality = 15f,
                    Agility = 10f,

                    Speed = 10f,

                    Stamina = 16f,
                    Intelligence = 10f,
                    Wisdom = 12f,
                    Luck = 9f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.85f,
                    Vitality = 0.90f,
                    Agility = 0.50f,

                    Speed = 0.50f,

                    Stamina = 0.95f,
                    Intelligence = 0.55f,
                    Wisdom = 0.70f,
                    Luck = 0.40f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.3f, Vitality = 0.32f, Agility = 0.17f, Speed = 0.17f, Stamina = 0.33f, Intelligence = 0.19f, Wisdom = 0.24f, Luck = 0.14f } },
                    { 10, new StatGrowth { Strength = 0.38f, Vitality = 0.41f, Agility = 0.23f, Speed = 0.23f, Stamina = 0.43f, Intelligence = 0.25f, Wisdom = 0.32f, Luck = 0.18f } },
                    { 20, new StatGrowth { Strength = 0.51f, Vitality = 0.54f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.57f, Intelligence = 0.33f, Wisdom = 0.42f, Luck = 0.24f } },
                    { 40, new StatGrowth { Strength = 0.68f, Vitality = 0.72f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.76f, Intelligence = 0.44f, Wisdom = 0.56f, Luck = 0.32f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Vampire}")
            };
        }
    }

    public static class Vulture
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Vulture",
                CharacterClass = CharacterClass.Vulture, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Flying,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Vulture}")
            };
        }
    }

    public static class WarChief
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "WarChief",
                CharacterClass = CharacterClass.WarChief, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A stalwart defender that anchors the line and absorbs pressure.",
                Expectations = "Frontline anchor. Uses [guard], [taunt], and [disrupt]. Protects allies.",
                Lore = "Built on vows and iron patience, they stand where others fall back.",
                Card = "Frontline [guardian][bruiser]. Absorbs pressure and [taunts].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 17f,
                    Vitality = 18f,
                    Agility = 10f,

                    Speed = 11f,

                    Stamina = 18f,
                    Intelligence = 8f,
                    Wisdom = 9f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.05f,
                    Vitality = 1.00f,
                    Agility = 0.55f,

                    Speed = 0.55f,

                    Stamina = 1.00f,
                    Intelligence = 0.40f,
                    Wisdom = 0.45f,
                    Luck = 0.45f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.37f, Vitality = 0.35f, Agility = 0.19f, Speed = 0.19f, Stamina = 0.35f, Intelligence = 0.14f, Wisdom = 0.16f, Luck = 0.16f } },
                    { 10, new StatGrowth { Strength = 0.47f, Vitality = 0.45f, Agility = 0.25f, Speed = 0.25f, Stamina = 0.45f, Intelligence = 0.18f, Wisdom = 0.2f, Luck = 0.2f } },
                    { 20, new StatGrowth { Strength = 0.63f, Vitality = 0.6f, Agility = 0.33f, Speed = 0.33f, Stamina = 0.6f, Intelligence = 0.24f, Wisdom = 0.27f, Luck = 0.27f } },
                    { 40, new StatGrowth { Strength = 0.84f, Vitality = 0.8f, Agility = 0.44f, Speed = 0.44f, Stamina = 0.8f, Intelligence = 0.32f, Wisdom = 0.36f, Luck = 0.36f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.WarChief}")
            };
        }
    }

    public static class Werewolf00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Werewolf00",
                CharacterClass = CharacterClass.Werewolf00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Humanoid,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Werewolf00}")
            };
        }
    }

    public static class WhiteNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "WhiteNinja",
                CharacterClass = CharacterClass.WhiteNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.WhiteNinja}")
            };
        }
    }
    public static class WhiteWitch
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "WhiteWitch",
                CharacterClass = CharacterClass.WhiteWitch, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid | Tag.Magic,

                Description = "A ranged spellcaster that shapes the fight with arcane control and damage.",
                Expectations = "Midline control. Uses [debuff], [aoe], and [curse]. Keep safely behind the front.",
                Lore = "Disciplines of ink and flame taught that power begins in focus.",
                Card = "Arcane [controller][debuffer]. Shapes fights with [aoe][curse].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 7f,
                    Vitality = 8f,
                    Agility = 11f,

                    Speed = 12f,

                    Stamina = 10f,
                    Intelligence = 20f,
                    Wisdom = 18f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.35f,
                    Vitality = 0.45f,
                    Agility = 0.55f,

                    Speed = 0.60f,

                    Stamina = 0.55f,
                    Intelligence = 1.20f,
                    Wisdom = 1.00f,
                    Luck = 0.50f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.12f, Vitality = 0.16f, Agility = 0.19f, Speed = 0.21f, Stamina = 0.19f, Intelligence = 0.42f, Wisdom = 0.35f, Luck = 0.17f } },
                    { 10, new StatGrowth { Strength = 0.16f, Vitality = 0.2f, Agility = 0.25f, Speed = 0.27f, Stamina = 0.25f, Intelligence = 0.54f, Wisdom = 0.45f, Luck = 0.23f } },
                    { 20, new StatGrowth { Strength = 0.21f, Vitality = 0.27f, Agility = 0.33f, Speed = 0.36f, Stamina = 0.33f, Intelligence = 0.72f, Wisdom = 0.6f, Luck = 0.3f } },
                    { 40, new StatGrowth { Strength = 0.28f, Vitality = 0.36f, Agility = 0.44f, Speed = 0.48f, Stamina = 0.44f, Intelligence = 0.96f, Wisdom = 0.8f, Luck = 0.4f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.WhiteWitch}")
            };
        }
    }
    public static class WildChild
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "WildChild",
                CharacterClass = CharacterClass.WildChild, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A flexible fighter with no single defining specialty.",
                Expectations = "Adaptable combatant. Uses [combo] tools across multiple ranges.",
                Lore = "Walks many roads, never lost for lack of a single path.",
                Card = "Flexible [fighter]. Converts [openings] into [combo finishes].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 12f,
                    Agility = 12f,

                    Speed = 12f,

                    Stamina = 12f,
                    Intelligence = 12f,
                    Wisdom = 12f,
                    Luck = 12f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.70f,
                    Agility = 0.70f,

                    Speed = 0.70f,

                    Stamina = 0.70f,
                    Intelligence = 0.70f,
                    Wisdom = 0.70f,
                    Luck = 0.70f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.24f, Agility = 0.24f, Speed = 0.24f, Stamina = 0.24f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.24f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.32f, Agility = 0.32f, Speed = 0.32f, Stamina = 0.32f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.32f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.42f, Agility = 0.42f, Speed = 0.42f, Stamina = 0.42f, Intelligence = 0.42f, Wisdom = 0.42f, Luck = 0.42f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.56f, Agility = 0.56f, Speed = 0.56f, Stamina = 0.56f, Intelligence = 0.56f, Wisdom = 0.56f, Luck = 0.56f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.WildChild}")
            };
        }
    }
    public static class Wolf00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Wolf00",
                CharacterClass = CharacterClass.Wolf00, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf00}")
            };
        }
    }

    public static class Wolf01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Wolf01",
                CharacterClass = CharacterClass.Wolf01, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf01}")
            };
        }
    }
    public static class Wolf02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Wolf02",
                CharacterClass = CharacterClass.Wolf02, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf02}")
            };
        }
    }

    public static class Wolf03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Wolf03",
                CharacterClass = CharacterClass.Wolf03, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf03}")
            };
        }
    }

    public static class YellowNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "YellowNinja",
                CharacterClass = CharacterClass.YellowNinja, // If this enum does not exist, replace accordingly.
                Tags = Tag.Hero | Tag.Humanoid,

                Description = "A swift combatant who wins through speed, angles, and precision strikes.",
                Expectations = "High mobility. Uses [flank], [crit], and [reposition]. Avoid extended trades.",
                Lore = "Years of quiet training forged a will that moves faster than thought.",
                Card = "Mobile [striker][flanker]. Leans on [crit] and [backstab] windows.",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 12f,
                    Vitality = 10f,
                    Agility = 18f,

                    Speed = 18f,

                    Stamina = 11f,
                    Intelligence = 12f,
                    Wisdom = 10f,
                    Luck = 14f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.70f,
                    Vitality = 0.55f,
                    Agility = 1.10f,

                    Speed = 1.05f,

                    Stamina = 0.65f,
                    Intelligence = 0.55f,
                    Wisdom = 0.50f,
                    Luck = 0.75f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.24f, Vitality = 0.19f, Agility = 0.39f, Speed = 0.37f, Stamina = 0.23f, Intelligence = 0.19f, Wisdom = 0.17f, Luck = 0.26f } },
                    { 10, new StatGrowth { Strength = 0.32f, Vitality = 0.25f, Agility = 0.5f, Speed = 0.47f, Stamina = 0.29f, Intelligence = 0.25f, Wisdom = 0.23f, Luck = 0.34f } },
                    { 20, new StatGrowth { Strength = 0.42f, Vitality = 0.33f, Agility = 0.66f, Speed = 0.63f, Stamina = 0.39f, Intelligence = 0.33f, Wisdom = 0.3f, Luck = 0.45f } },
                    { 40, new StatGrowth { Strength = 0.56f, Vitality = 0.44f, Agility = 0.88f, Speed = 0.84f, Stamina = 0.52f, Intelligence = 0.44f, Wisdom = 0.4f, Luck = 0.6f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.YellowNinja}")
            };
        }
    }

    public static class Yeti
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Yeti00",
                CharacterClass = CharacterClass.Yeti, // If this enum does not exist, replace accordingly.
                Tags = Tag.Beast | Tag.Humanoid,

                Description = "A feral threat that overwhelms foes with instinct and power.",
                Expectations = "Pouncer. Uses [maul], [pounce], and [roar] to break formations.",
                Lore = "Born to the deep places, it runs by instinct and strikes without doubt.",
                Card = "Ferocious [pouncer][mauler]. Disrupts with [roar] and [knockdown].",

                BaseStats = new ActorStats
                {
                    Level = 1,

                    Strength = 15f,
                    Vitality = 12f,
                    Agility = 15f,

                    Speed = 14f,

                    Stamina = 14f,
                    Intelligence = 8f,
                    Wisdom = 8f,
                    Luck = 11f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 0.95f,
                    Vitality = 0.75f,
                    Agility = 0.90f,

                    Speed = 0.85f,

                    Stamina = 0.85f,
                    Intelligence = 0.40f,
                    Wisdom = 0.40f,
                    Luck = 0.55f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.33f, Vitality = 0.26f, Agility = 0.32f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.14f, Wisdom = 0.14f, Luck = 0.19f } },
                    { 10, new StatGrowth { Strength = 0.43f, Vitality = 0.34f, Agility = 0.41f, Speed = 0.38f, Stamina = 0.38f, Intelligence = 0.18f, Wisdom = 0.18f, Luck = 0.25f } },
                    { 20, new StatGrowth { Strength = 0.57f, Vitality = 0.45f, Agility = 0.54f, Speed = 0.51f, Stamina = 0.51f, Intelligence = 0.24f, Wisdom = 0.24f, Luck = 0.33f } },
                    { 40, new StatGrowth { Strength = 0.76f, Vitality = 0.6f, Agility = 0.72f, Speed = 0.68f, Stamina = 0.68f, Intelligence = 0.32f, Wisdom = 0.32f, Luck = 0.44f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    PixelPosition = new Vector2Int(512, 140),
                    Scale = new Vector3(1f, 1f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),

                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Yeti}")
            };
        }
    }
}
    



