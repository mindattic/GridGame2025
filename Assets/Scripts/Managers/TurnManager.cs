// --- File: Assets/Scripts/Managers/TurnManager.cs ---
using Assets.Scripts.Sequences;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using Assets.Scripts.Models;
using System.Linq;

namespace Assets.Scripts.Managers
{
    public class TurnManager : MonoBehaviour
    {
        public bool IsHeroTurn { get; private set; }
        public bool IsEnemyTurn => !IsHeroTurn;
        public int CurrentTurn = 0;

        public void Initialize()
        {
            var enemyAtCursor = (g.Timeline != null)
                ? g.Timeline.GetCurrentEnemy()
                : null;

            IsHeroTurn = enemyAtCursor == null;
            StartTurn();
        }

        public void NextTurn()
        {
            CurrentTurn++;

            if (g.Timeline != null)
                g.Timeline.NextBlock();

            var enemyAtCursor = (g.Timeline != null)
                ? g.Timeline.GetCurrentEnemy()
                : null;

            IsHeroTurn = enemyAtCursor == null;

            if (g.Timeline != null)
            {
                if (IsHeroTurn)
                    g.Timeline.FocusOnHero();
                else
                    g.Timeline.FocusOnEnemy(enemyAtCursor);
            }

            TryGlowActiveHero();

            // Saturation temporarily disabled pending visual fixes
            // if (IsHeroTurn) ApplyHeroTurnDesaturation(); else RestoreFullSaturation();

            if (IsHeroTurn)
            {
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                g.SequenceManager.Add(new EnemyTakeTurnSequence(enemyAtCursor));
            }
        }

        private void StartTurn()
        {
            if (g.Timeline != null)
            {
                var enemyAtCursor = g.Timeline.GetCurrentEnemy();
                IsHeroTurn = enemyAtCursor == null;

                if (IsHeroTurn)
                    g.Timeline.FocusOnHero();
                else
                    g.Timeline.FocusOnEnemy(enemyAtCursor);
            }

            TryGlowActiveHero();

            // Saturation temporarily disabled pending visual fixes
            // if (IsHeroTurn) ApplyHeroTurnDesaturation(); else RestoreFullSaturation();

            if (IsHeroTurn)
            {
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                var enemyAtCursor = g.Timeline?.GetCurrentEnemy();
                g.SequenceManager.Add(new EnemyTakeTurnSequence(enemyAtCursor));
            }
        }

        private void TryGlowActiveHero()
        {
            if (!IsHeroTurn) return;

            var hero = g.Timeline != null ? g.Timeline.GetCurrentHero() : null;
            if (hero != null && hero.IsPlaying && hero.IsHero && hero.Glow != null)
            {
                hero.Glow.Glow();
            }
        }

        // Keep these as no-ops for now to avoid breaking callers
        public void ApplyHeroTurnDesaturation() { }
        public void RestoreFullSaturation() { }
    }
}
