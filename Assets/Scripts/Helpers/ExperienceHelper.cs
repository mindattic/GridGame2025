using Assets.Scripts.Libraries;
using Assets.Scripts.Managers;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

namespace Assets.Helpers
{
    public static class ExperienceHelper
    {
        public static int NextLevel(int level)
        {
            level = Mathf.Max(1, level);
            return 50 + (level * level) * 10;
        }

        public static int Calculate(ActorInstance defeated)
        {
            if (defeated == null || string.IsNullOrEmpty(defeated.characterName))
                return 0;

            var actorData = ActorLibrary.Get(defeated.characterName);
            if (actorData == null)
                return 0;

            var stats = defeated.Stats;

            float physPower = Formulas.Offense(stats, 0f) + Formulas.Defense(stats, 0f);
            float magicPower = Formulas.MagicOffense(stats) + Formulas.MagicResistance(stats);
            float hpScore = Mathf.Max(stats.MaxHP, stats.HP);
            float levelScore = Mathf.Max(1f, stats.Level);

            float powerScore =
                physPower * 0.10f +
                magicPower * 0.08f +
                hpScore * 0.05f +
                levelScore * 2.0f;

            int reward = Mathf.Max(1, Mathf.RoundToInt(powerScore + actorData.BonusXP));
            return reward;
        }

        // Add XP to an actor. CurrentXP tracks progress since last level-up, TotalXP is lifetime.
        public static void Gain(ActorInstance actor, int amount)
        {
            if (actor == null || amount <= 0) return;

            actor.Stats.TotalXP += amount;
            actor.Stats.CurrentXP += amount;

            // Level-up loop while we have enough XP for next level
            while (actor.Stats.CurrentXP >= NextLevel(actor.Stats.Level))
            {
                int needed = NextLevel(actor.Stats.Level);
                actor.Stats.CurrentXP -= needed;
                ApplyLevelUp(actor);
            }

            // Persist hero progress unless Endless mode (Campaign only)
            if (actor.IsHero && !GameModeHelper.IsEndless)
                SaveHeroProgress(actor);
        }

        // Single level-up: bump level, rebuild stats template, carry CurrentXP & TotalXP
        private static void ApplyLevelUp(ActorInstance actor)
        {
            actor.Stats.Level = Mathf.Max(1, actor.Stats.Level + 1);

            var data = ActorLibrary.Get(actor.characterName);
            if (data == null) return;

            var next = data.GetStats(actor.Stats.Level);

            int carryXp = actor.Stats.CurrentXP;
            int totalXp = actor.Stats.TotalXP;

            actor.Stats = new ActorStats(next)
            {
                CurrentXP = carryXp,
                TotalXP = totalXp,
                HP = next.MaxHP,
                PreviousHP = next.MaxHP
            };

            actor.HealthBar.Update();

            g.CombatTextManager?.Spawn("Level Up!", actor.Position, "Heal");
            if (VisualEffectLibrary.VisualEffects.TryGetValue("LevelUp", out var vfx))
                g.VisualEffectManager?.Spawn(vfx, actor.Position);
        }

        private static void SaveHeroProgress(ActorInstance actor)
        {
            var party = ProfileHelper.CurrentProfile?.CurrentSave?.Party?.Members;
            if (party == null) return;

            var entry = party.FirstOrDefault(m => m != null && m.Character == actor.characterName);
            if (entry == null) return;

            entry.Level = actor.Stats.Level;
            entry.CurrentXP = actor.Stats.CurrentXP;
            entry.TotalXP = actor.Stats.TotalXP;

            //ProfileHelper.Save(true);
        }
    }
}
