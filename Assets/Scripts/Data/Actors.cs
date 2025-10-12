using Assets.Helpers;
using Assets.Scripts.Libraries;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;
using Tag = ActorTag;

namespace Assets.Data.Actor
{
    public static class Barbarian
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterName = "Barbarian",
                CharacterClass = CharacterClass.Barbarian,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A warrior driven by rage.",
                Expectations = "High single target physical DPS that ramps steadily. Low magic. Expects to delete fragile targets and trade evenly into tanks when Rage is stacked.",
                Lore = "Raised in the border wilds, the barbarian believes strength settles all debts.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 12f,  // +3 for early punch
                    Vitality = 6f,
                    Agility = 3f,
                    Speed = 4f,      // +1 for turn pace
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 1f,
                    Luck = 6f        // +2 for hit/crit reliability
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.55f
                    Vitality = 1.1f,
                    Agility = 0.5f,
                    Speed = 0.6f,    // was 0.5f
                    Stamina = 0.6f,
                    Intelligence = 0.0f,
                    Wisdom = 0.3f,
                    Luck = 0.7f      // was 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.0f, Vitality = 1.8f, Agility = 0.6f, Speed = 1.0f, Stamina = 1.1f, Intelligence = 0.3f, Wisdom = 0.4f, Luck = 0.6f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 2.0f, Agility = 0.7f, Speed = 1.1f, Stamina = 1.5f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 0.7f } },
                    { 15, new StatGrowth { Strength = 2.6f, Vitality = 2.3f, Agility = 0.8f, Speed = 1.3f, Stamina = 1.9f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 0.9f } },
                    { 20, new StatGrowth { Strength = 2.9f, Vitality = 2.6f, Agility = 1.0f, Speed = 1.5f, Stamina = 2.2f, Intelligence = 0.8f, Wisdom = 0.7f, Luck = 1.1f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.8f, -1.25f, 0f),
                    Scale = new Vector3(5f, 5f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Barbarian}"),

                Card = "Gains [Rage] when attacking or being attacked. Will eventually go [Berserk] and attack multiple nearby enemies."

            };
        }
    }

    public static class Bat
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Bat,
                Tags = Tag.Enemy | Tag.Beast | Tag.Flying,
                Description = "A flying menace.",
                Expectations = "Evasive magic harasser. Relies on spells and high speed to peck away. Avoids direct trades with heavy melee.",
                Lore = "Flock-runner of the midnight caves, guided by echoes and hunger.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 1f,
                    Agility = 8f,
                    Speed = 6f,
                    Stamina = 2f,
                    Intelligence = 7f,
                    Wisdom = 3f,
                    Luck = 5f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.2f,
                    Agility = 1.1f,
                    Speed = 1.2f,
                    Stamina = 0.6f,
                    Intelligence = 0.75f,
                    Wisdom = 0.5f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.5f, Vitality = 0.2f, Agility = 1.4f, Speed = 1.5f, Stamina = 0.5f, Intelligence = 1.1f, Wisdom = 0.6f, Luck = 0.8f } },
                    { 10, new StatGrowth { Strength = 0.6f, Vitality = 0.5f, Agility = 1.3f, Speed = 1.2f, Stamina = 0.7f, Intelligence = 1.4f, Wisdom = 0.8f, Luck = 1.0f } },
                    { 20, new StatGrowth { Strength = 1.1f, Vitality = 0.8f, Agility = 1.8f, Speed = 1.8f, Stamina = 1.0f, Intelligence = 1.8f, Wisdom = 1.1f, Luck = 1.4f } },
                    { 40, new StatGrowth { Strength = 1.6f, Vitality = 1.2f, Agility = 2.6f, Speed = 2.6f, Stamina = 1.4f, Intelligence = 2.2f, Wisdom = 1.5f, Luck = 1.8f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, 0.5f, 0.0f),
                    Scale = new Vector3(2.0f, 2.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Bat}"),

                Card = "Intermittently goes [Berserk] attacking multiple nearby enemies.",
                Trivia = new List<string>
                    {
                        "Echolocation expert",
                        "Sleeps upside down"
                    }
            };
        }
    }
    public static class Captain
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Captain00,
                Tags = Tag.Enemy | Tag.Soldier | Tag.Humanoid,
                Description = "A captain.",
                Expectations = "Frontline commander with strong early Stats and respectable scaling. Wins sustained trades but does not burst.",
                Lore = "Veteran of two sieges, known for steady hands and short speeches.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10f,
                    Vitality = 10f,
                    Agility = 4f,
                    Speed = 4f,
                    Stamina = 5f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 4f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.5f,
                    Vitality = 1.0f,
                    Agility = 0.8f,
                    Speed = 0.7f,
                    Stamina = 0.9f,
                    Intelligence = 0.4f,
                    Wisdom = 0.5f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 1.5f, Agility = 1.0f, Speed = 0.9f, Stamina = 1.1f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 0.8f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 2.0f, Agility = 1.2f, Speed = 1.1f, Stamina = 1.3f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 1.0f } },
                    { 20, new StatGrowth { Strength = 2.9f, Vitality = 2.5f, Agility = 1.6f, Speed = 1.5f, Stamina = 1.7f, Intelligence = 1.0f, Wisdom = 1.0f, Luck = 1.2f } },
                    { 40, new StatGrowth { Strength = 3.8f, Vitality = 3.5f, Agility = 2.2f, Speed = 2.1f, Stamina = 2.1f, Intelligence = 1.5f, Wisdom = 1.5f, Luck = 1.8f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.41f, -1.5f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Captain00}"),
                Card = "A captain.",
                Trivia = new List<string>
                    {
                        "Likes jerky",
                        "Hates Reptiles"
                    }
            };
        }
    }

    public static class Cleric
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Cleric,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A strict adherent to the church.",
                Expectations = "Support mage. Damage is steady rather than explosive. Excels when kept safe and allowed to cast.",
                Lore = "Ordained in the Lightbearer Orthodoxy, sworn to mend and to judge.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 6f,  // +4: still support, but can 1–2 shot Slimes
                    Vitality = 5f,
                    Agility = 3f,
                    Speed = 4f,     // +1: better cadence
                    Stamina = 2f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 10f      // +1: high hit/crit identity
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.2f, // was 0.4f
                    Vitality = 1.2f,
                    Agility = 0.6f,
                    Speed = 1.0f,    // was 0.8f
                    Stamina = 0.5f,
                    Intelligence = 0.9f, // was 0.8f
                    Wisdom = 0.9f,       // was 0.8f
                    Luck = 2.5f          // was 2.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.8f, Vitality = 1.0f, Agility = 0.5f, Speed = 0.3f, Stamina = 0.6f, Intelligence = 0.6f, Wisdom = 0.6f, Luck = 3.0f } },
                    { 10, new StatGrowth { Strength = 1.4f, Vitality = 1.6f, Agility = 0.7f, Speed = 0.5f, Stamina = 0.7f, Intelligence = 1.1f, Wisdom = 1.1f, Luck = 3.8f } },
                    { 20, new StatGrowth { Strength = 1.8f, Vitality = 2.2f, Agility = 1.0f, Speed = 1.2f, Stamina = 1.1f, Intelligence = 1.6f, Wisdom = 1.6f, Luck = 4.8f } },
                    { 40, new StatGrowth { Strength = 2.4f, Vitality = 2.8f, Agility = 1.4f, Speed = 2.2f, Stamina = 2.1f, Intelligence = 2.1f, Wisdom = 2.1f, Luck = 6.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Cleric}"),
                Abilities = new List<Ability>() {
                    AbilityLibrary.Heal(),
                    AbilityLibrary.Smite()
                },
                Card = "Calls down healing light and protective wards."
            };
        }
    }

    public static class GreenNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.GreenNinja,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A swift and elusive assassin.",
                Expectations = "Skirmisher that relies on speed and crits. Low base damage per hit but high turn economy.",
                Lore = "Silent courier of the Jade Clique, paid to make problems vanish.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 8f,  // +3
                    Vitality = 3f,
                    Agility = 9f,
                    Speed = 9f,     // +1
                    Stamina = 4f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 9f       // +3 for crit/hit
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 2.0f, // was 1.7f
                    Vitality = 0.6f,
                    Agility = 2.4f,
                    Speed = 2.4f,    // was 2.2f
                    Stamina = 1.0f,
                    Intelligence = 0.6f,
                    Wisdom = 0.4f,
                    Luck = 2.0f      // was 1.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 0.6f, Agility = 3.4f, Speed = 3.0f, Stamina = 0.6f, Intelligence = 0.6f, Wisdom = 0.3f, Luck = 1.6f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 1.1f, Agility = 4.5f, Speed = 4.0f, Stamina = 1.1f, Intelligence = 1.0f, Wisdom = 0.6f, Luck = 2.1f } },
                    { 20, new StatGrowth { Strength = 2.9f, Vitality = 1.6f, Agility = 5.8f, Speed = 5.2f, Stamina = 1.6f, Intelligence = 1.6f, Wisdom = 1.1f, Luck = 3.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.GreenNinja}"),
                Card = "Prefers to crit and move on."
            };
        }
    }

    public static class Paladin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Paladin,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A holy warrior clad in armor.",
                Expectations = "Durable frontliner with moderate DPS and high accuracy. Expects to outlast and win long trades.",
                Lore = "Knight of the Lightbearer Council, shield raised against every darkness.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +2
                    Vitality = 8f,
                    Agility = 2f,
                    Speed = 4f,     // +1
                    Stamina = 6f,
                    Intelligence = 3f,
                    Wisdom = 6f,
                    Luck = 4f       // +1
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.7f, // was 1.4f
                    Vitality = 1.9f,
                    Agility = 0.4f,
                    Speed = 0.6f,    // was 0.5f
                    Stamina = 1.4f,
                    Intelligence = 0.6f,
                    Wisdom = 1.5f,
                    Luck = 0.6f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.2f, Vitality = 1.9f, Agility = 0.4f, Speed = 0.6f, Stamina = 1.4f, Intelligence = 0.5f, Wisdom = 1.2f, Luck = 0.6f } },
                    { 10, new StatGrowth { Strength = 1.5f, Vitality = 2.3f, Agility = 0.6f, Speed = 0.9f, Stamina = 1.8f, Intelligence = 1.0f, Wisdom = 1.6f, Luck = 1.1f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 2.8f, Agility = 0.9f, Speed = 1.1f, Stamina = 2.3f, Intelligence = 1.5f, Wisdom = 2.0f, Luck = 1.6f } },
                    { 40, new StatGrowth { Strength = 3.0f, Vitality = 3.8f, Agility = 1.2f, Speed = 1.6f, Stamina = 3.2f, Intelligence = 2.0f, Wisdom = 2.8f, Luck = 2.2f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Paladin}"),
                Card = "Shields nearby allies with [Radiant Guard]. Takes reduced [Physical] and [Dark] damage.",
            };
        }
    }

    public static class Pugilist
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Pugilist,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A disciplined master of martial strikes.",
                Expectations = "Evasive sustained DPS. Performs best in long fights where counters and flurries add up.",
                Lore = "A monk from the Stone Steppe, speechless but eloquent in motion.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +3
                    Vitality = 5f,
                    Agility = 9f,
                    Speed = 8f,
                    Stamina = 6f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 6f       // +2
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 2.0f, // was 1.75f
                    Vitality = 1.0f,
                    Agility = 2.0f,
                    Speed = 2.2f,
                    Stamina = 1.5f,
                    Intelligence = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 1.3f      // was 1.0f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.3f, Vitality = 0.8f, Agility = 2.3f, Speed = 2.5f, Stamina = 1.2f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 1.4f } },
                    { 10, new StatGrowth { Strength = 1.8f, Vitality = 1.0f, Agility = 2.8f, Speed = 3.0f, Stamina = 1.5f, Intelligence = 0.2f, Wisdom = 0.2f, Luck = 1.7f } },
                    { 20, new StatGrowth { Strength = 2.4f, Vitality = 1.5f, Agility = 3.3f, Speed = 3.5f, Stamina = 2.0f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 2.2f } },
                    { 40, new StatGrowth { Strength = 3.4f, Vitality = 2.0f, Agility = 4.5f, Speed = 5.0f, Stamina = 2.5f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 2.8f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Pugilist}"),
                Card = "Has a chance to counterattack with [Flurry] when evading an attack.",
                Trivia = new List<string>
                    {
                        "Once punched a bear",
                        "Trains in silence"
                    }
            };
        }
    }

    public static class RedNinja
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.RedNinja,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A deadly assassin wielding forbidden arts.",
                Expectations = "Ambusher with burst windows. Leans on poison and vanish to reset fights.",
                Lore = "Blade-broker of the Crimson Pact, paid in secrets as often as coin.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10f, // +3
                    Vitality = 4f,
                    Agility = 8f,
                    Speed = 8f,     // +1
                    Stamina = 5f,
                    Intelligence = 3f,
                    Wisdom = 2f,
                    Luck = 8f       // +3
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.9f, // was 1.55f
                    Vitality = 1.0f,
                    Agility = 1.9f,
                    Speed = 1.8f,    // was 1.6f
                    Stamina = 1.1f,
                    Intelligence = 0.6f,
                    Wisdom = 0.5f,
                    Luck = 1.6f      // was 1.1f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.1f, Vitality = 1.0f, Agility = 2.3f, Speed = 2.2f, Stamina = 1.1f, Intelligence = 0.5f, Wisdom = 0.5f, Luck = 1.4f } },
                    { 10, new StatGrowth { Strength = 2.5f, Vitality = 1.2f, Agility = 2.7f, Speed = 2.5f, Stamina = 1.4f, Intelligence = 0.9f, Wisdom = 0.8f, Luck = 1.8f } },
                    { 20, new StatGrowth { Strength = 3.1f, Vitality = 1.5f, Agility = 3.2f, Speed = 3.0f, Stamina = 1.8f, Intelligence = 1.4f, Wisdom = 1.0f, Luck = 2.3f } },
                    { 40, new StatGrowth { Strength = 3.9f, Vitality = 2.0f, Agility = 4.1f, Speed = 3.8f, Stamina = 2.3f, Intelligence = 1.9f, Wisdom = 1.5f, Luck = 2.7f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.RedNinja}"),
                Card = "Applies [Poison] with melee attacks. May [Vanish] when hit.",
            };
        }
    }

    public static class Ronin
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Ronin,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A masterless warrior guided by honor.",
                Expectations = "Reliable duelist. Open strong, stays even over time. Few weaknesses, few tricks.",
                Lore = "Once sworn to a fallen house, now sworn to the road.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 11f, // +3
                    Vitality = 6f,
                    Agility = 6f,
                    Speed = 6f,     // +1
                    Stamina = 4f,
                    Intelligence = 2f,
                    Wisdom = 3f,
                    Luck = 5f       // +1
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.5f
                    Vitality = 1.0f,
                    Agility = 1.3f,
                    Speed = 1.2f,    // was 1.0f
                    Stamina = 0.9f,
                    Intelligence = 0.4f,
                    Wisdom = 0.8f,
                    Luck = 1.0f      // was 0.8f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.2f, Vitality = 1.3f, Agility = 1.5f, Speed = 1.4f, Stamina = 0.9f, Intelligence = 0.5f, Wisdom = 1.0f, Luck = 1.2f } },
                    { 10, new StatGrowth { Strength = 2.6f, Vitality = 1.8f, Agility = 1.9f, Speed = 1.7f, Stamina = 1.2f, Intelligence = 1.0f, Wisdom = 1.5f, Luck = 1.4f } },
                    { 20, new StatGrowth { Strength = 3.5f, Vitality = 2.2f, Agility = 2.4f, Speed = 2.2f, Stamina = 1.7f, Intelligence = 1.5f, Wisdom = 2.0f, Luck = 1.7f } },
                    { 40, new StatGrowth { Strength = 4.5f, Vitality = 2.7f, Agility = 3.1f, Speed = 2.8f, Stamina = 2.1f, Intelligence = 2.0f, Wisdom = 2.5f, Luck = 2.2f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.35f, -1.34f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Ronin}"),
                Card = "Has a chance to [Counter] when attacked. Deals extra damage on the first strike.",
                Trivia = new List<string>
                    {
                        "Once served a great house",
                        "Walks the path of redemption"
                    }
            };
        }
    }

    public static class Scorpion
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Scorpion,
                Tags = Tag.Enemy | Tag.Insect,
                Description = "A hulking brute with a barbed tail and armored shell.",
                Expectations = "Slow bruiser with very high durability. Damage is steady. Punishes opponents who stay in front.",
                Lore = "Ancient desert crawler whose shell rings like iron under the moon.",

                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 7f,
                    Vitality = 1f,
                    Agility = 1f,
                    Speed = 2f,
                    Stamina = 1.75f,
                    Intelligence = 1f,
                    Wisdom = 2f,
                    Luck = 3f
                },

                StatGrowth = new StatGrowth
                {
                    Strength = 1.4f,
                    Vitality = 0.65f,
                    Agility = 0.3f,
                    Speed = 0.5f,
                    Stamina = 0.525f,
                    Intelligence = 0.4f,
                    Wisdom = 0.6f,
                    Luck = 0.6f
                },

                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.8f, Vitality = 0.95f,  Agility = 0.4f, Speed = 0.8f, Stamina = 0.65f,  Intelligence = 0.5f, Wisdom = 0.8f, Luck = 1.0f } },
                    { 10, new StatGrowth { Strength = 2.3f, Vitality = 1.2f,   Agility = 0.5f, Speed = 1.0f, Stamina = 0.8f,   Intelligence = 0.8f, Wisdom = 1.0f, Luck = 1.2f } },
                    { 20, new StatGrowth { Strength = 2.8f, Vitality = 1.6f,   Agility = 0.6f, Speed = 1.2f, Stamina = 1.05f,  Intelligence = 1.0f, Wisdom = 1.2f, Luck = 1.5f } },
                    { 40, new StatGrowth { Strength = 3.8f, Vitality = 2.125f, Agility = 0.8f, Speed = 1.5f, Stamina = 1.3f,   Intelligence = 1.5f, Wisdom = 1.5f, Luck = 2.0f } }
                },

                Stats = new ActorStats(),

                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(1.02f, 0.04f, 0.0f),
                    Scale = new Vector3(4.0f, 4.0f, 0f)
                },

                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Scorpion}"),



                Card = "Takes [reduced damage] from frontal attacks. Has a chance to [counterattack] when hit.",

            };
        }
    }

    public static class Sellsword
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Sellsword,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A blade-for-hire who fights for coin.",
                Expectations = "Flexible baseline combatant. Never the best, rarely the worst. Trades consistently into most foes.",
                Lore = "Signed more contracts than most nobles sign letters.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +3
                    Vitality = 6f,
                    Agility = 5f,
                    Speed = 6f,     // +1
                    Stamina = 4f,
                    Intelligence = 3f,
                    Wisdom = 3f,
                    Luck = 6f       // +1
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.5f
                    Vitality = 1.5f,
                    Agility = 1.2f,
                    Speed = 1.2f,
                    Stamina = 0.9f,
                    Intelligence = 0.6f,
                    Wisdom = 0.6f,
                    Luck = 1.3f      // was 1.1f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.2f, Vitality = 1.9f, Agility = 1.5f, Speed = 1.5f, Stamina = 1.1f, Intelligence = 0.8f, Wisdom = 0.8f, Luck = 1.6f } },
                    { 10, new StatGrowth { Strength = 2.7f, Vitality = 2.4f, Agility = 1.8f, Speed = 1.8f, Stamina = 1.3f, Intelligence = 1.0f, Wisdom = 1.0f, Luck = 2.0f } },
                    { 20, new StatGrowth { Strength = 3.7f, Vitality = 3.3f, Agility = 2.2f, Speed = 2.2f, Stamina = 1.8f, Intelligence = 1.5f, Wisdom = 1.5f, Luck = 2.6f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Sellsword}"),
                Card = "Steady DPS baseline. Always ready for work.",
            };
        }
    }

    public static class Slime
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime,
                Tags = Tag.Enemy,
                Description = "A jiggly nuisance barely held together.",
                Expectations = "Training dummy. Exists to make heroes feel strong.",
                Lore = "Somewhere between a pet and a puddle.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.1f,
                    Vitality = 0.1f,
                    Agility = 0.1f,
                    Speed = 0.1f,
                    Stamina = 0.1f,
                    Intelligence = 0.0f,
                    Wisdom = 0.0f,
                    Luck = 0.0f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.2f, Vitality = 0.2f, Agility = 0.2f, Speed = 0.2f, Stamina = 0.2f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                    { 10, new StatGrowth { Strength = 0.3f, Vitality = 0.3f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.3f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                    { 20, new StatGrowth { Strength = 0.4f, Vitality = 0.4f, Agility = 0.4f, Speed = 0.4f, Stamina = 0.4f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } },
                    { 40, new StatGrowth { Strength = 0.5f, Vitality = 0.5f, Agility = 0.5f, Speed = 0.5f, Stamina = 0.5f, Intelligence = 0.0f, Wisdom = 0.0f, Luck = 0.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, 0.5f, 0.0f),
                    Scale = new Vector3(2.0f, 2.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime}"),
                Card = "Lowest Stats in the game. Designed to die in one hit.",
            };
        }
    }

    public static class Slime00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime00,
                Tags = Tag.Enemy,
                Description = "A tiny, wobbling blob.",
                Expectations = "Falls over to a stiff breeze.",
                Lore = "Jellied life with no ambition.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth(0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0f, 0f, 0f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.15f,0.15f,0.15f,0.15f,0f,0f,0f) },
                    { 10, new StatGrowth(0.2f,0.2f,0.2f,0.2f,0.2f,0f,0f,0f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime00}"),


                Card = "Dies in one hit most of the time."
            };
        }
    }

    public static class Slime01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime01,
                Tags = Tag.Enemy,
                Description = "A spry little slime.",
                Expectations = "Still fragile, just wigglier.",
                Lore = "Developed a taste for bouncing.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 2f,
                    Speed = 2f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth(0.1f, 0.1f, 0.15f, 0.15f, 0.1f, 0f, 0f, 0.05f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.15f,0.2f,0.2f,0.15f,0f,0f,0.1f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime01}"),
                Card = "Evasive but still a pushover."
            };
        }
    }

    public static class Slime02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime02,
                Tags = Tag.Enemy,
                Description = "A thicker, stickier slime.",
                Expectations = "Sometimes survives the opening hit.",
                Lore = "Extra goo, extra stubborn.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 2f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 2f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 0f
                },
                StatGrowth = new StatGrowth(0.1f, 0.15f, 0.1f, 0.1f, 0.15f, 0f, 0f, 0.05f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.2f,0.1f,0.1f,0.2f,0f,0f,0.1f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime02}"),
                Card = "Barely tanky by slime standards."
            };
        }
    }

    public static class Slime03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Slime03,
                Tags = Tag.Enemy,
                Description = "A mischievous wobble.",
                Expectations = "Still weak; might dodge once.",
                Lore = "Learns tricks by imitation.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 1f,
                    Vitality = 1f,
                    Agility = 2f,
                    Speed = 1f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.1f, 0.1f, 0.15f, 0.1f, 0.1f, 0f, 0f, 0.15f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.15f,0.15f,0.2f,0.1f,0.15f,0f,0f,0.2f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(2f, 2f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Slime03}"),
                Card = "Occasional crits, still fragile."
            };
        }
    }

    public static class Soldier00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Soldier00,
                Tags = Tag.Enemy | Tag.Soldier | Tag.Humanoid,
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
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier00}"),
                Card = "Basic soldier. Vulnerable but alert.",
            };
        }
    }

    public static class Soldier01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Soldier01,
                Tags = Tag.Enemy | Tag.Soldier | Tag.Humanoid,
                Description = "A rookie trying too hard.",
                Expectations = "Quick but fragile. Dangerous in groups.",
                Lore = "Painted his shield by hand the night before deployment.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 4f,
                    Vitality = 1f,
                    Agility = 4f,
                    Speed = 3f,
                    Stamina = 1f,
                    Intelligence = 1f,
                    Wisdom = 1f,
                    Luck = 2f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.6f,
                    Vitality = 0.3f,
                    Agility = 0.5f,
                    Speed = 0.5f,
                    Stamina = 0.2f,
                    Intelligence = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 0.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.7f, Vitality = 0.3f, Agility = 0.6f, Speed = 0.6f, Stamina = 0.3f, Intelligence = 0.3f, Wisdom = 0.3f, Luck = 0.4f } },
                    { 10, new StatGrowth { Strength = 0.9f, Vitality = 0.4f, Agility = 0.7f, Speed = 0.7f, Stamina = 0.4f, Intelligence = 0.4f, Wisdom = 0.4f, Luck = 0.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.44f, 0f),
                    Scale = new Vector3(5f, 5f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier01}"),

                Card = "Faster than most soldiers, but fragile.",
                Trivia = new List<string>
                    {
                        "Broke 3 spears in training",
                        "Carries lucky bone charm"
                    }
            };
        }
    }

    public static class Soldier02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Soldier02,
                Tags = Tag.Enemy | Tag.Soldier | Tag.Humanoid,
                Description = "A jittery scout with sharp eyes.",
                Expectations = "Skirmisher with good initiative but poor staying power.",
                Lore = "Never stops scanning the horizon.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 2f,
                    Agility = 5f,
                    Speed = 4f,
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 1f,
                    Luck = 2f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.3f,
                    Vitality = 0.3f,
                    Agility = 0.7f,
                    Speed = 0.6f,
                    Stamina = 0.3f,
                    Intelligence = 0.3f,
                    Wisdom = 0.2f,
                    Luck = 0.4f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.5f, Vitality = 0.3f, Agility = 0.6f, Speed = 0.7f, Stamina = 0.3f, Intelligence = 0.3f, Wisdom = 0.3f, Luck = 0.4f } },
                    { 10, new StatGrowth { Strength = 0.7f, Vitality = 0.4f, Agility = 0.7f, Speed = 0.8f, Stamina = 0.4f, Intelligence = 0.4f, Wisdom = 0.4f, Luck = 0.5f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -0.55f, 0.0f),
                    Scale = new Vector3(3.0f, 3.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier02}"),

                Card = "Fast to act, quick to flee.",
                Trivia = new List<string>
                    {
                        "Sniffs everything",
                        "Allergic to slime"
                    }
            };
        }
    }

    public static class Soldier03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Soldier03,
                Tags = Tag.Enemy | Tag.Soldier | Tag.Humanoid,
                Description = "A washed-up old fighter.",
                Expectations = "Sturdy baseline with weak offense.",
                Lore = "Claims to have trained captains when captains were young.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 3f,
                    Vitality = 3f,
                    Agility = 1f,
                    Speed = 1f,
                    Stamina = 2f,
                    Intelligence = 1f,
                    Wisdom = 2f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 0.4f,
                    Vitality = 0.5f,
                    Agility = 0.2f,
                    Speed = 0.2f,
                    Stamina = 0.3f,
                    Intelligence = 0.4f,
                    Wisdom = 0.2f,
                    Luck = 0.3f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 0.5f, Vitality = 0.6f, Agility = 0.2f, Speed = 0.2f, Stamina = 0.3f, Intelligence = 0.5f, Wisdom = 0.3f, Luck = 0.3f } },
                    { 10, new StatGrowth { Strength = 0.6f, Vitality = 0.7f, Agility = 0.3f, Speed = 0.3f, Stamina = 0.4f, Intelligence = 0.6f, Wisdom = 0.3f, Luck = 0.4f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.7f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Soldier03}"),
                Card = "Takes a hit better than he gives one.",
                Trivia = new List<string>
                    {
                        "Once held rank",
                        "Talks in riddles"
                    }
            };
        }
    }

    public static class Thief
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Thief,
                Tags = Tag.Hero | Tag.Humanoid,
                Description = "A nimble rogue with sticky fingers.",
                Expectations = "Mobile crit fisher. Leans on evasion and luck to win long trades. Avoids armored foes.",
                Lore = "Knows three hundred pockets by heart.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 8f,  // +3
                    Vitality = 2f,
                    Agility = 7f,
                    Speed = 7f,     // +1
                    Stamina = 2f,
                    Intelligence = 2f,
                    Wisdom = 2f,
                    Luck = 9f       // +2
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.8f, // was 1.5f
                    Vitality = 0.5f,
                    Agility = 1.8f,
                    Speed = 2.1f,    // was 2.0f
                    Stamina = 0.6f,
                    Intelligence = 0.5f,
                    Wisdom = 0.7f,
                    Luck = 2.5f      // was 2.2f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.1f, Vitality = 0.5f, Agility = 2.3f, Speed = 2.6f, Stamina = 0.7f, Intelligence = 0.6f, Wisdom = 0.8f, Luck = 3.0f } },
                    { 10, new StatGrowth { Strength = 2.4f, Vitality = 0.6f, Agility = 3.1f, Speed = 3.1f, Stamina = 0.9f, Intelligence = 0.8f, Wisdom = 1.0f, Luck = 3.8f } },
                    { 20, new StatGrowth { Strength = 2.8f, Vitality = 0.8f, Agility = 3.9f, Speed = 3.6f, Stamina = 1.1f, Intelligence = 1.0f, Wisdom = 1.2f, Luck = 4.9f } },
                    { 40, new StatGrowth { Strength = 3.2f, Vitality = 1.0f, Agility = 4.9f, Speed = 4.4f, Stamina = 1.3f, Intelligence = 1.2f, Wisdom = 1.5f, Luck = 6.0f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.61f, -1.56f, 0.0f),
                    Scale = new Vector3(5.3f, 5.3f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Thief}"),
                Card = "Has a high chance to evade. Can steal from enemies.",
                Trivia = new List<string>
                    {
                        "Loves coin",
                        "Allergic to jail cells"
                    }
            };
        }
    }

    public static class Vampire
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Vampire,
                Tags = Tag.Hero | Tag.Humanoid | Tag.Undead,
                Description = "A shadowy predator who thrives in darkness.",
                Expectations = "Sustained magic DPS with life steal. Wants to fight from safety and drain foes down.",
                Lore = "The last heir of a night-court, more rumor than citizen.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 9f,  // +5: makes early hits chunky
                    Vitality = 3f,
                    Agility = 5f,
                    Speed = 6f,     // +1: better turn pace
                    Stamina = 2f,
                    Intelligence = 7f, // +1: flavor (not used by simplified damage)
                    Wisdom = 5f,
                    Luck = 7f       // +2: more hit/crit
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.3f,  // was 0.4f
                    Vitality = 0.6f,
                    Agility = 1.0f,
                    Speed = 1.1f,     // was 0.9f
                    Stamina = 0.5f,
                    Intelligence = 1.0f, // was 0.75f
                    Wisdom = 0.8f,       // was 0.6f
                    Luck = 1.1f          // was 0.8f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 1.4f, Vitality = 0.7f, Agility = 1.3f, Speed = 1.3f, Stamina = 0.6f, Intelligence = 1.2f, Wisdom = 1.0f, Luck = 1.1f } },
                    { 10, new StatGrowth { Strength = 1.6f, Vitality = 1.0f, Agility = 1.6f, Speed = 1.6f, Stamina = 0.8f, Intelligence = 1.6f, Wisdom = 1.3f, Luck = 1.3f } },
                    { 20, new StatGrowth { Strength = 2.0f, Vitality = 1.3f, Agility = 2.1f, Speed = 2.1f, Stamina = 1.1f, Intelligence = 2.1f, Wisdom = 1.7f, Luck = 1.7f } },
                    { 40, new StatGrowth { Strength = 2.6f, Vitality = 1.7f, Agility = 2.6f, Speed = 2.6f, Stamina = 1.5f, Intelligence = 2.7f, Wisdom = 2.2f, Luck = 2.2f } }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.5f, -1.4f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Vampire}"),
                Card = "Heals for 30% of magic damage dealt. Resistant to [Dark].",
            };
        }
    }

    public static class Wolf00
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Wolf00,
                Tags = Tag.Enemy | Tag.Beast,
                Description = "A hungry stray wolf.",
                Expectations = "Quick nip; folds in 1–2 hits.",
                Lore = "Hunts alone at the edge of the woods.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 3f,
                    Vitality = 2f,
                    Agility = 3f,
                    Speed = 3f,
                    Stamina = 2f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.6f, 0.3f, 0.6f, 0.6f, 0.3f, 0.1f, 0.1f, 0.2f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.7f,0.4f,0.7f,0.7f,0.4f,0.1f,0.1f,0.3f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(1.06f, -0.49f, 0f),
                    Scale = new Vector3(3f, 3f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf00}"),
                Card = "Fast, low durability."
            };
        }
    }

    public static class Wolf01
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Wolf01,
                Tags = Tag.Enemy | Tag.Beast,
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
                    Position = new Vector3(0.05f, 0.1f, 0f),
                    Scale = new Vector3(2f, 2f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf01}"),
                Card = "Acts often; dies fast."
            };
        }
    }

    public static class Wolf02
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Wolf02,
                Tags = Tag.Enemy | Tag.Beast,
                Description = "An older, cautious wolf.",
                Expectations = "More measured strike, still low HP.",
                Lore = "Knows when to pick a fight.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 2f,
                    Vitality = 2f,
                    Agility = 2f,
                    Speed = 3f,
                    Stamina = 2f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.5f, 0.35f, 0.5f, 0.6f, 0.35f, 0.1f, 0.1f, 0.2f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(0.6f,0.4f,0.6f,0.7f,0.4f,0.1f,0.1f,0.25f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.05f, 0.1f, 0f),
                    Scale = new Vector3(2f, 2f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf02}"),
                Card = "Still goes down fast."
            };
        }
    }

    public static class Wolf03
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Wolf03,
                Tags = Tag.Enemy | Tag.Beast,
                Description = "A bold pack runner.",
                Expectations = "Hardest-hitting of the low wolves.",
                Lore = "Tests prey before committing.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 4f,
                    Vitality = 1f,
                    Agility = 3f,
                    Speed = 3f,
                    Stamina = 1f,
                    Intelligence = 0f,
                    Wisdom = 0f,
                    Luck = 1f
                },
                StatGrowth = new StatGrowth(0.8f, 0.25f, 0.6f, 0.6f, 0.25f, 0.1f, 0.1f, 0.25f),
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth(1.0f,0.3f,0.7f,0.7f,0.3f,0.1f,0.1f,0.3f) }
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(0.27f, 0.1f, 0f),
                    Scale = new Vector3(2f, 2f, 0f),
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Wolf03}"),
                Card = "High damage, low staying power."
            };
        }
    }

    public static class Yeti
    {
        public static ActorData Data()
        {
            return new ActorData
            {
                CharacterClass = CharacterClass.Yeti,
                Tags = Tag.Enemy | Tag.Beast,
                Description = "A towering beast of cold fury.",
                Expectations = "Heavy hitter with armor-chunking swings. Slow turns but high impact.",
                Lore = "Snow moves around it as if afraid to touch.",
                BaseStats = new ActorStats
                {
                    Level = 1,
                    Strength = 10f,
                    Vitality = 2f,
                    Agility = 1f,
                    Speed = 2f,
                    Stamina = 1f,
                    Intelligence = 1f,
                    Wisdom = 1f,
                    Luck = 2f
                },
                StatGrowth = new StatGrowth
                {
                    Strength = 1.5f,
                    Vitality = 0.3f,
                    Agility = 0.2f,
                    Speed = 0.4f,
                    Stamina = 0.2f,
                    Wisdom = 0.2f,
                    Luck = 0.5f
                },
                MilestoneStatGrowth = new Dictionary<int, StatGrowth>
                {
                    { 5,  new StatGrowth { Strength = 2.2f, Vitality = 0.425f, Agility = 0.25f, Speed = 0.5f, Stamina = 0.25f, Intelligence = 0.0f, Wisdom = 0.2f, Luck = 0.5f } }, // 1.7 -> 0.425, 1.0 -> 0.25
                    { 10, new StatGrowth { Strength = 3.0f, Vitality = 0.525f, Agility = 0.30f, Speed = 0.6f, Stamina = 0.3f,  Intelligence = 0.0f, Wisdom = 0.3f, Luck = 0.6f } }, // 2.1 -> 0.525, 1.2 -> 0.3
                    { 20, new StatGrowth { Strength = 3.8f, Vitality = 0.625f, Agility = 0.40f, Speed = 0.8f, Stamina = 0.4f,  Intelligence = 0.0f, Wisdom = 0.4f, Luck = 0.8f } }, // 2.5 -> 0.625, 1.6 -> 0.4
                    { 40, new StatGrowth { Strength = 4.8f, Vitality = 0.8f,   Agility = 0.50f, Speed = 1.0f, Stamina = 0.5f,  Intelligence = 0.0f, Wisdom = 0.5f, Luck = 1.0f } }  // 3.2 -> 0.8, 2.0 -> 0.5
                },
                Stats = new ActorStats(),
                ThumbnailSettings = new ThumbnailSettings
                {
                    Position = new Vector3(1.3f, -1.0f, 0.0f),
                    Scale = new Vector3(5.0f, 5.0f, 0f)
                },
                CanvasThumbnailSettings = CanvasThumbnailSettings.SetDefault(),
                Portrait = AssetHelper.LoadAsset<Sprite>($"{CharacterClass.Yeti}"),
                Card = "Delivers powerful [Ice] attacks that ignore 25% of defense.",
                Trivia = new List<string>
                    {
                        "Hates heat",
                        "Used to be a myth"
                    }
            };
        }
    }

}
