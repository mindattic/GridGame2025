// --- File: Assets/Scripts/Managers/TurnManager.cs ---
using Assets.Scripts.Sequences;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using Assets.Scripts.Models;
using System.Linq;
using System.Collections.Generic;

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

            HandleHeroTurnFocus();

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

            HandleHeroTurnFocus();

            // Saturation temporarily disabled pending visual fixes
            // if (IsHeroTurn) ApplyHeroTurnDesaturation(); else RestoreFullSaturation();

            if (IsHeroTurn)
            {
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                var enemyAtCursor = g.Timeline.GetCurrentEnemy();
                g.SequenceManager.Add(new EnemyTakeTurnSequence(enemyAtCursor));
            }
        }

        private void HandleHeroTurnFocus()
        {
            if (!IsHeroTurn) return;

            var active = g.Timeline.GetCurrentHero();

            if (g.TurnSelectionMode == TurnSelectionMode.PreferActive || g.TurnSelectionMode == TurnSelectionMode.ActiveOnly)
            {
                g.SelectedHeroManager.Focus(active);
                active.Glow.Play();
            }
            else // FreeSelect
            {
                g.HeroManager.Glow();
            }
        }

        // Apply grayscale to all playing actors except those in ignoreList. Defaults to no ignores.
        public void ApplyDesaturation(List<ActorInstance> ignoreList = null)
        {
            var ignore = ignoreList != null ? new HashSet<ActorInstance>(ignoreList) : null;
            foreach (var a in g.Actors.All)
            {
                if (a == null || !a.IsPlaying) continue;
                if (ignore != null && ignore.Contains(a))
                {
                    a.Render.SetSaturation(1f);
                    continue;
                }
                a.Render.SetSaturation(0f);
            }
        }

        // Restore full saturation to all playing actors except those in ignoreList (which are left unchanged).
        public void RestoreSaturation(List<ActorInstance> ignoreList = null)
        {
            var ignore = ignoreList != null ? new HashSet<ActorInstance>(ignoreList) : null;
            foreach (var a in g.Actors.All)
            {
                if (a == null || !a.IsPlaying) continue;
                if (ignore != null && ignore.Contains(a))
                    continue; // skip restoring on ignored actors
                a.Render.SetSaturation(1f);
            }
        }
    }
}
