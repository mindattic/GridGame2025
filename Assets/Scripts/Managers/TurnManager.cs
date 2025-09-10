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
        public ActorInstance ActiveActor { get; private set; }

        public void Initialize()
        {
            ResolveActiveFromTimeline();
            StartTurn();
        }

        private void ResolveActiveFromTimeline()
        {
            var enemy = g.Timeline != null ? g.Timeline.GetCurrentEnemy() : null;
            IsHeroTurn = enemy == null;
            ActiveActor = IsHeroTurn ? g.Timeline?.GetCurrentHero() : enemy;
        }

        public void NextTurn()
        {
            CurrentTurn++;

            g.Timeline?.NextBlock();
            ResolveActiveFromTimeline();

            if (g.Timeline != null)
            {
                if (IsHeroTurn) g.Timeline.FocusOnHero();
                else g.Timeline.FocusOnEnemy(ActiveActor);
            }

            // Restore input mode based on the current side
            g.InputManager.InputMode = IsHeroTurn ? InputMode.PlayerTurn : InputMode.EnemyTurn;

            UpdateActiveIndicators();
            HandleHeroTurnFocus();

            if (IsHeroTurn)
            {
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                g.SequenceManager.Add(new EnemyTakeTurnSequence(ActiveActor));
            }
        }

        private void StartTurn()
        {
            if (g.Timeline != null)
            {
                var enemyAtCursor = g.Timeline.GetCurrentEnemy();
                IsHeroTurn = enemyAtCursor == null;
                ActiveActor = IsHeroTurn ? g.Timeline.GetCurrentHero() : enemyAtCursor;

                if (IsHeroTurn) g.Timeline.FocusOnHero();
                else g.Timeline.FocusOnEnemy(enemyAtCursor);
            }

            // Ensure input mode matches the side at the start
            g.InputManager.InputMode = IsHeroTurn ? InputMode.PlayerTurn : InputMode.EnemyTurn;

            UpdateActiveIndicators();
            HandleHeroTurnFocus();

            if (IsHeroTurn)
            {
                g.SequenceManager.Add(new HeroStartSequence());
            }
            else
            {
                g.SequenceManager.Add(new EnemyTakeTurnSequence(ActiveActor));
            }
        }

        private void UpdateActiveIndicators()
        {
            // Board: enable on active only; disable on others
            foreach (var a in g.Actors.All)
            {
                if (a == null || !a.IsPlaying) continue;
                a.Render.SetActiveIndicatorEnabled(a == ActiveActor);
            }

            // Timeline: Refresh (current block toggled by Timeline.UpdateSelectionHighlight)
            g.Timeline?.RefreshSelectionHighlight();
        }

        private void HandleHeroTurnFocus()
        {
            if (!IsHeroTurn) return;

            if (g.TurnSelectionMode == TurnSelectionMode.ActiveOnly)
            {
                if (ActiveActor != null)
                {
                    g.SelectedHeroManager.Focus(ActiveActor); // focus can later change freely
                    ActiveActor.Glow?.Play();
                }
            }
            else // FreeSelect
            {
                g.HeroManager?.Glow();
            }
        }

        // Saturation helpers remain as implemented earlier
        public void ApplyHeroTurnDesaturation(List<ActorInstance> ignoreList = null) { /* existing implementation kept */ }
        public void RestoreFullSaturation(List<ActorInstance> ignoreList = null) { /* existing implementation kept */ }
    }
}
