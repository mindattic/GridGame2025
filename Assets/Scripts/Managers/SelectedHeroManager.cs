using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameManagerHelper;

/// <summary>
/// Handles selection, dragging, and dropping of g.Actors.Heroes during the correct turn/phase.
/// Interacts with multiple core game systems via the GameManager singleton.
/// </summary>
public class SelectedHeroManager : MonoBehaviour
{
    /// <summary>
    /// Selects an actor under the mouse cursor, updating the focus indicator and actor card UI.
    /// Allowed ONLY during the hero's turn and at the start of their phase.
    /// </summary>
    public void Focus()
    {
        // Only allow focus selection during the hero's turn and Start phase.
        if (!g.TurnManager.isHeroTurn || g.TurnManager.currentPhase != TurnPhase.Start)
            return;

        var collisions = Physics2D.OverlapPointAll(g.TouchPosition3D);
        if (collisions == null) return;
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null) return;
        var actor = collider.gameObject.GetComponent<ActorInstance>();

        if (actor == null || !actor.isPlaying) return;

        if (g.Actors.FocusedActor == actor)
            return;

        g.Actors.FocusedActor = actor;
        g.SortingManager.OnActorFocus();

        if (g.Actors.FocusedActor.isHero)
            g.AbilityButtonManager.Show(g.Actors.FocusedActor);

        g.TouchOffset = g.Actors.FocusedActor.position - g.TouchPosition3D;

        g.FocusIndicator.Assign();
        g.Card.Assign();

        // Notify editor to reload
#if UNITY_EDITOR
        GameManager.instance.reloadThumbnailSettings = true;
#endif
    }

    /// <summary>
    /// Handles dragging an actor, setting up move. Starts Move phase if at Start, otherwise continues drag in Move phase.
    /// </summary>
    public void Drag()
    {
        // Only proceed if it's the hero's turn, a hero is focused, and that actor is not an enemy.
        if (!g.TurnManager.isHeroTurn || !g.Actors.HasFocusedActor || g.Actors.FocusedActor.isEnemy)
            return;

        // Accept drag ONLY if we are in Start or Move phase:
        if (g.TurnManager.currentPhase != TurnPhase.Start && g.TurnManager.currentPhase != TurnPhase.Move)
            return;

        // If at Start, this drag triggers the transition to Move phase:
        if (g.TurnManager.currentPhase == TurnPhase.Start)
        {
            g.TurnManager.SetPhase(TurnPhase.Move);
            // Optional: if you want to only allow drag *after* Move phase started, return here
            // return;
        }

        g.Actors.SelectedHero = g.Actors.FocusedActor;
        g.SortingManager.OnSelectedHeroDrag();

        // If the selected hero is already moving, do not process further drag logic.
        if (g.Actors.SelectedHero.flags.IsMoving)
            return;

        g.Card.Clear();
        g.FocusIndicator.Clear();

        g.AudioManager.Play("Click");
        g.TimerBar.Play();
        g.ActorManager.CheckEnemyAP();

        g.Actors.SelectedHero.move.TriggerMoveTowardsCursor();
    }

    /// <summary>
    /// Handles dropping a dragged hero, snapping them to the grid and checking for attackResults.
    /// </summary>
    public void Drop()
    {
        // Only proceed if it's the hero's turn, Move phase is active, and there's a selected hero currently moving.



        if (!g.TurnManager.isHeroTurn 
            || g.TurnManager.currentPhase != TurnPhase.Move 
            || !g.Actors.HasSelectedHero || !g.Actors.SelectedHero.flags.IsMoving)
        {
            if (g.Actors.HasFocusedActor)
                g.Actors.FocusedActor.position = g.Actors.FocusedActor.currentTile.position;
            return;
        }

        g.Actors.SelectedHero.move.ToLocation();
        g.SortingManager.OnSelectedHeroDrop();

        g.Actors.SelectedHero = null;
        g.Actors.FocusedActor = null;

        g.TimerBar.Pause();
        g.PincerAttackManager.Check(Team.Hero);

        // Do NOT advance phase here—TurnManager/UI is responsible for that!
    }

}
