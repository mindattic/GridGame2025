// --- File: Assets/Scripts/Managers/SelectedHeroManager.cs ---
using Assets.Helpers;
using Assets.Scripts.Sequences;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Handles focus, drag, and drop for heroes during the hero turn.
/// Promotes to SelectedHero once the drag moved at least half a tile.
/// On drop:
///  - If a pincer exists, it enqueues the pincer chain which ends the turn.
///  - If no pincer exists, it enqueues DeathSequence and EndTurnSequence and executes.
/// Either way, the timeline advances exactly one block.
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
        if (!g.TurnManager.IsHeroTurn)
            return;

        var target = actor ?? TouchHelper.GetActorAtTouchPosition();

        if (target == null || !target.IsPlaying)
        {
            if (g.Board.IsInsideBoard(g.TouchPosition3D))
            {
                g.Actors.FocusedActor = null;
                g.AbilityButtonManager.Hide();
                g.FocusIndicator.Hide();
                g.Card.Clear();
            }
            return;
        }

        // New: enforce selection rules depending on mode
        if (!SelectionRules.CanControlHero(target))
        {
            // Feedback hook could go here (sound/UI)
            return;
        }

        if (g.Actors.FocusedActor == target)
            return;

        g.AbilityButtonManager.Hide();
        g.Actors.FocusedActor = target;
        g.SortingManager.OnActorFocus();

        if (g.Actors.FocusedActor.IsHero)
            g.AbilityButtonManager.Show(g.Actors.FocusedActor);

        // Optional: bonus hook for PreferActiveWithBonus mode
        if (g.TurnSelectionMode == Assets.Scripts.Models.TurnSelectionMode.PreferActive)
        {
            var activeHero = g.Timeline != null ? g.Timeline.GetCurrentHero() : null;
            if (activeHero != null && activeHero == g.Actors.FocusedActor)
            {
                // TODO: apply a bonus buff/effect here if desired.
            }
        }

        g.TouchOffset = g.Actors.FocusedActor.Position - g.TouchPosition3D;

        hasPendingDrag = false;
        pendingActor = null;

        g.FocusIndicator.Show();
        g.Card.Assign();

#if UNITY_EDITOR
        GameManager.instance.reloadThumbnailSettings = true;
#endif
    }

    public void Drag()
    {
        if (!g.TurnManager.IsHeroTurn || !g.Actors.HasFocusedActor || g.Actors.FocusedActor.IsEnemy)
            return;

        var actor = g.Actors.FocusedActor;

        if (!hasPendingDrag || pendingActor != actor)
        {
            hasPendingDrag = true;
            pendingActor = actor;

            g.TouchOffset = actor.Position - g.TouchPosition3D;

            if (!pendingActor.Flags.IsMoving)
                pendingActor.Move.MoveTowardCursor();

            // g.TurnManager.RestoreFullSaturation(); // disabled per request

            return;
        }

        if (!pendingActor.Flags.IsMoving)
            pendingActor.Move.MoveTowardCursor();

        if (g.Actors.HasSelectedHero)
            return;

        float moved = Vector3.Distance(pendingActor.Position, pendingActor.currentTile.position);
        if (moved >= dragThreshold)
        {
            g.Actors.SelectedHero = pendingActor;
            g.SortingManager.OnSelectedHeroDrag();

            g.TimerBar2D.SetDuration(6f);
            g.TimerBar2D.ResetToFull();
            g.TimerBar2D.Play();

            g.Card.Clear();
            g.FocusIndicator.Hide();
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

        // Stop the hero timer.
        g.TimerBar2D.Pause();

        // Complete movement for promoted hero.
        var hero = g.Actors.SelectedHero;
        hero.Move.ToLocation();
        hero.Flags.IsMoving = false;
        g.SortingManager.OnSelectedHeroDrop();

        // Clear selection and focus.
        g.Actors.SelectedHero = null;
        g.Actors.FocusedActor = null;
        hasPendingDrag = false;
        pendingActor = null;

        // Try a pincer chain that starts with this hero.
        bool anyPincer = g.PincerAttackManager.Check(Team.Hero, hero);

        if (!anyPincer)
        {
            // No pincer. Resolve deaths, then end the turn once.
            g.SequenceManager.Add(new DeathSequence());
            g.SequenceManager.Add(new EndTurnSequence());
            g.SequenceManager.Execute();
        }
        // If anyPincer is true, EnqueueRoutine will end the turn after the chain.
    }
}
