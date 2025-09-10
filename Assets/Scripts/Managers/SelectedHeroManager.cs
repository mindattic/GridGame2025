// --- File: Assets/Scripts/Managers/SelectedHeroManager.cs ---
using Assets.Helpers;
using Assets.Scripts.Sequences;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Handles focus, drag, and drop for heroes during the hero turn.
/// Promotes to SelectedHero once the drag moved at least half a tile.
/// Focus is independent from the active actor; you can inspect any actor.
/// </summary>
public class SelectedHeroManager : MonoBehaviour
{
    private ActorInstance pendingActor;
    private bool hasPendingDrag;
    private float dragThreshold;

    public void Awake()
    {
        dragThreshold = g.TileMap.tileSize / 2f;
    }

    public void Focus(ActorInstance actor = null)
    {
        // Allow focusing at any time to inspect stats
        var target = actor ?? TouchHelper.GetActorAtTouchPosition();

        if (target == null || !target.IsPlaying)
        {
            if (g.Board.IsInsideBoard(g.TouchPosition3D))
            {
                g.Actors.FocusedActor = null;
                g.AbilityButtonManager.Hide();
                g.Actors.All.ForEach(x => x.Render.SetFocusIndicatorEnabled(false));
                g.Card.Clear();
                g.Timeline?.RefreshSelectionHighlight();
            }
            return;
        }

        // If unchanged, just refresh visuals
        if (g.Actors.FocusedActor == target)
        {
            g.Timeline?.RefreshSelectionHighlight();
            return;
        }

        g.AbilityButtonManager.Hide();
        g.Actors.FocusedActor = target;
        g.SortingManager.OnActorFocus();

        // Show abilities only when a hero is focused
        if (g.Actors.FocusedActor.IsHero)
            g.AbilityButtonManager.Show(g.Actors.FocusedActor);

        g.TouchOffset = g.Actors.FocusedActor.Position - g.TouchPosition3D;

        hasPendingDrag = false;
        pendingActor = null;

        // Board: toggle focus indicators
        g.Actors.All.ForEach(x => x.Render.SetFocusIndicatorEnabled(x == g.Actors.FocusedActor));
        // Timeline: toggle focus highlight across all blocks
        g.Timeline?.RefreshSelectionHighlight();

        g.Card.Assign();

#if UNITY_EDITOR
        GameManager.instance.reloadThumbnailSettings = true;
#endif
    }

    public void Drag()
    {
        // Only allow dragging during hero turn and when focused actor is the active actor and is a hero
        if (!g.TurnManager.IsHeroTurn || !g.Actors.HasFocusedActor)
            return;

        var actor = g.Actors.FocusedActor;
        if (actor == null || actor.IsEnemy) return;

        // In ActiveOnly mode, restrict dragging to the current ActiveActor. In other modes allow any hero.
        var mode = g.TurnSelectionMode;
        bool restrictToActive = mode == Assets.Scripts.Models.TurnSelectionMode.ActiveOnly;
        if (restrictToActive && actor != g.TurnManager.ActiveActor)
            return;

        // Require a press/hold
        bool pressing = Input.GetMouseButton(0) || Input.touchCount > 0;
        if (!pressing) return;

        // Do not require the pointer to remain over the actor after initial focus.
        // Focus() was set when the press began on the actor; allow drag to proceed smoothly.

        if (!hasPendingDrag || pendingActor != actor)
        {
            hasPendingDrag = true;
            pendingActor = actor;

            g.TouchOffset = actor.Position - g.TouchPosition3D;

            if (!pendingActor.Flags.IsMoving)
                pendingActor.Move.MoveTowardCursor();

            return;
        }

        if (!pendingActor.Flags.IsMoving)
            pendingActor.Move.MoveTowardCursor();

        if (g.Actors.HasSelectedHero)
            return;

        float moved = Vector3.Distance(pendingActor.Position, pendingActor.currentTile.position);
        if (moved >= dragThreshold)
        {
            g.Actors.SelectedHero = pendingActor; // promote the active hero to selected player
            g.SortingManager.OnSelectedHeroDrag();

            g.TimerBar2D.SetDuration(6f);
            g.TimerBar2D.ResetToFull();
            g.TimerBar2D.Play();

            g.Card.Clear();
            g.AudioManager.Play("Click");
            g.ActorManager.CheckEnemyAP();
        }
    }

    public void Drop()
    {
        bool validSelectedMove =
            g.TurnManager.IsHeroTurn &&
            g.Actors.HasSelectedHero &&
            g.Actors.SelectedHero.Flags.IsMoving;

        if (!validSelectedMove)
        {
            if (hasPendingDrag && pendingActor != null)
            {
                pendingActor.Move.ToLocation();
                pendingActor.Flags.IsMoving = false;
                pendingActor.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else if (g.Actors.HasFocusedActor)
            {
                g.Actors.FocusedActor.Move.ToLocation();
                g.Actors.FocusedActor.Flags.IsMoving = false;
                g.Actors.FocusedActor.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            hasPendingDrag = false;
            pendingActor = null;
            return;
        }

        g.TimerBar2D.Pause();

        var hero = g.Actors.SelectedHero;
        hero.Move.ToLocation();
        hero.Flags.IsMoving = false;
        g.SortingManager.OnSelectedHeroDrop();

        // Suspend all touch input until the turn system restores it
        g.InputManager.InputMode = InputMode.None;

        g.Actors.SelectedHero = null;
        hasPendingDrag = false;
        pendingActor = null;

        bool anyPincer = g.PincerAttackManager.Check(Team.Hero, hero);

        if (!anyPincer)
        {
            g.SequenceManager.Add(new DeathSequence());
            g.SequenceManager.Add(new EndTurnSequence());
            g.SequenceManager.Execute();
        }
    }
}
