using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Helpers
{
    /// <summary>
    /// ExperienceTable centralizes all XP-related formulas used by the game.
    /// Design goals:
    /// - Pure static, stateless helpers (safe to call from anywhere, no side effects).
    /// - Easy to tune. Both the "XP needed per level" curve and "XP reward on kill" scaling
    ///   are expressed with simple constants and documented here.
    /// - Readable and predictable behavior for designers and programmers.
    ///
    /// Typical usage:
    /// - To check if an actor can level up: compare actor.Stats.Experience against XpToNextLevel(actorLevel).
    /// - To award XP on kill: ExperienceTable.XpReward(defeatedActor).
    ///
    /// Notes:
    /// - All values are returned as ints since most gameplay systems (UI, saves) use integers for XP.
    /// - Keep these formulas simple and transparent so they can be balanced without hidden complexity.
    /// </summary>
    public static class ExperienceHelper
    {
        /// <summary>
        /// Returns the XP required to advance from the given 'level' to the next level.
        /// Uses a classic JRPG-style quadratic curve that ramps up quickly at higher levels.
        ///
        /// Formula:
        ///   XP_to_next = 50 + (level^2) * 10
        ///
        /// Rationale:
        /// - The quadratic term (level^2) makes higher levels noticeably more demanding.
        /// - The base 50 prevents the earliest levels from being trivial.
        /// - The multiplier (10) controls overall steepness and can be tuned for pacing.
        ///
        /// Edge cases:
        /// - If 'level' is less than 1, it is clamped to 1 to avoid 0/negative inputs.
        ///
        /// Examples:
        /// - Level 1 -> 2: 50 + 1^2 * 10 = 60 XP
        /// - Level 2 -> 3: 50 + 4 * 10 = 90 XP
        /// - Level 3 -> 4: 50 + 9 * 10 = 140 XP
        /// - Level 5 -> 6: 50 + 25 * 10 = 300 XP
        /// - Level 10 -> 11: 50 + 100 * 10 = 1050 XP
        ///
        /// Tuning tips:
        /// - Lower the base (50) to speed up the earliest levels.
        /// - Lower the multiplier (10) to reduce late-game grind, or raise it for longer endgame.
        /// - Replace the quadratic with another function (e.g., linear or exponential) if desired.
        /// </summary>
        public static int NextLevel(int level)
        {
            level = Mathf.Max(1, level);
            return 50 + (level * level) * 10; // L1->2: 60; L5->6: 300
        }

        /// <summary>
        /// Computes the XP reward for defeating a specific actor based on their level and combat stats.
        /// Higher-level, higher-stat opponents yield more XP.
        ///
        /// Components:
        /// - Base authorable reward (ActorData.XPReward) so designers can nudge specific species.
        /// - Physical and magical power via Formulas (offense/defense and magic offense/resistance).
        /// - MaxHP contribution as a proxy for durability.
        /// - Level contribution so higher-level enemies pay out more even with similar stats.
        ///
        /// The coefficients are tuned to keep XP values in a reasonable range (tens to low hundreds),
        /// but can be adjusted for your pacing.
        /// </summary>
        public static int Calculate(ActorInstance defeated)
        {
            if (defeated == null || string.IsNullOrEmpty(defeated.characterName))
                return 0;

            var actorData = ActorLibrary.Get(defeated.characterName);
            if (actorData == null)
                return 0;

            var stats = defeated.Stats;

            // Derive composite power from existing combat formulas to avoid duplicating logic.
            float physPower = Formulas.Offense(stats, 0f) + Formulas.Defense(stats, 0f);       // weapon/armor not considered for XP
            float magicPower = Formulas.MagicOffense(stats) + Formulas.MagicResistance(stats); // spell capability + resistance
            float hpScore = Mathf.Max(stats.MaxHP, stats.HP);                                   // use MaxHP primarily; clamp via max
            float levelScore = Mathf.Max(1f, stats.Level);

            // Weighted sum -> scale down to XP range
            // Coefficients are conservative to keep rewards readable and avoid huge spikes.
            float powerScore =
                physPower * 0.10f +   // physical contribution
                magicPower * 0.08f +  // magical contribution
                hpScore * 0.05f +   // durability contribution
                levelScore * 2.0f;    // direct level influence

            // Final reward: baseline plus scaled power, rounded to int, clamped to >= 1
            int reward = Mathf.Max(1, Mathf.RoundToInt(powerScore + actorData.BonusXP));

            return reward;
        }


        // Add XP to an actor and roll level-ups using ExperienceHelper.NextLevel thresholds.
        public static void Gain(ActorInstance actor, int amount)
        {
            if (actor == null || amount <= 0) return;

            actor.Stats.Experience += amount;

            // Level-up loop while we have enough XP for next level
            while (actor.Stats.Experience >= NextLevel(actor.Stats.Level))
            {
                int needed = NextLevel(actor.Stats.Level);
                actor.Stats.Experience -= needed;
                ApplyLevelUp(actor);
            }
        }

        // Single level-up application: bump level, rebuild stats from ActorLibrary, keep XP progress.
        private static void ApplyLevelUp(ActorInstance actor)
        {
            actor.Stats.Level = Mathf.Max(1f, actor.Stats.Level + 1f).ToInt();

            var data = ActorLibrary.Get(actor.characterName);
            if (data == null) return;

            var next = data.GetStats(Mathf.RoundToInt(actor.Stats.Level));

            int carryXp = actor.Stats.Experience;

            actor.Stats = new ActorStats(next)
            {
                Experience = carryXp,
                HP = next.MaxHP,
                PreviousHP = next.MaxHP
            };

            actor.HealthBar.Update();

            // Simple feedback (tweak to your UI/VFX)
            g.CombatTextManager.Spawn("Level Up!", actor.Position, "Heal");
            if (VfxLibrary.VisualEffects.TryGetValue("LevelUp", out var vfx))
                g.VfxManager.Spawn(vfx, actor.Position);
        }
    }
}
