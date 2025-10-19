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

        private ManaPoolManager GetMana()
        {
            var go = GameObject.Find("Game");
            return go != null ? go.GetComponent<ManaPoolManager>() : null;
        }

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

        private void SelectActiveOrFallback()
        {
            if (ActiveActor != null)
            {
                g.SelectionManager.Select(ActiveActor);
                return;
            }

            // Fallback: pick any playing hero first, otherwise any playing actor
            var any = g.Actors.Heroes?.FirstOrDefault(a => a != null && a.IsPlaying)
                      ?? g.Actors.All?.FirstOrDefault(a => a != null && a.IsPlaying);
            if (any != null)
                g.SelectionManager.Select(any);
        }

        public void NextTurn()
        {
            CurrentTurn++;

            // New: allow the StageManager to activate deferred spawns scheduled by SpawnTurn
            g.StageManager?.OnTurnAdvanced();

            g.Timeline?.NextBlock();
            ResolveActiveFromTimeline();

            if (g.Timeline != null)
            {
                if (IsHeroTurn) g.Timeline.FocusOnHero();
                else g.Timeline.FocusOnEnemy(ActiveActor);
            }

            var mana = GetMana();
            if (mana != null) mana.OnTurnStarted(IsHeroTurn ? Team.Hero : Team.Enemy);

            // Always set input mode for the side taking the new turn
            g.InputManager.InputMode = IsHeroTurn ? InputMode.PlayerTurn : InputMode.EnemyTurn;

            // Ensure a selection exists and matches the actor whose turn it is
            SelectActiveOrFallback();

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

            var mana = GetMana();
            if (mana != null) mana.OnTurnStarted(IsHeroTurn ? Team.Hero : Team.Enemy);

            // Ensure input mode matches the side at the start
            g.InputManager.InputMode = IsHeroTurn ? InputMode.PlayerTurn : InputMode.EnemyTurn;

            // Ensure a selection exists and matches the actor whose turn it is
            SelectActiveOrFallback();

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
            foreach (var a in g.Actors.All)
            {
                if (a == null || !a.IsPlaying) continue;
                a.Render.SetActiveIndicatorEnabled(a == ActiveActor);
            }

            g.Timeline?.RefreshSelectionHighlight();
        }

        private void HandleHeroTurnFocus()
        {
            if (!IsHeroTurn) return;

            if (g.TurnSelectionMode == TurnSelectionMode.ActiveOnly)
            {
                if (ActiveActor != null)
                {
                    // Selection already set to ActiveActor; just add hero-specific VFX
                    ActiveActor.Glow?.Play();
                }
            }
            else
            {
                g.HeroManager?.Glow();
            }
        }

        public void ApplyHeroTurnDesaturation(List<ActorInstance> ignoreList = null) { }
        public void RestoreFullSaturation(List<ActorInstance> ignoreList = null) { }
    }
}
