using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

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
        // Only allow focus selection during the hero's turn and Bounce phase.
        if (!g.TurnManager.isHeroTurn)
            return;

        var target = TouchHelper.GetActorAtTouchPosition();

        if (target == null || !target.isPlaying) return;

        if (g.Actors.FocusedActor == target)
            return;

        g.Actors.FocusedActor = target;
        g.SortingManager.OnActorFocus();

        if (g.Actors.FocusedActor.isHero)
            g.AbilityButtonManager.Show(g.Actors.FocusedActor);

        g.TouchOffset = g.Actors.FocusedActor.position - g.TouchPosition3D;

        g.FocusIndicator.Show();
        g.Card.Assign();

        // Notify editor to reload
#if UNITY_EDITOR
        GameManager.instance.reloadThumbnailSettings = true;
#endif
    }

    /// <summary>
    /// Handles dragging an actor, setting up move. Starts Seek phase if at Bounce, otherwise continues drag in Seek phase.
    /// </summary>
    public void Drag()
    {
        // Only proceed if it's the hero's turn, a hero is focused, and that actor is not an enemy.
        if (!g.TurnManager.isHeroTurn || !g.Actors.HasFocusedActor || g.Actors.FocusedActor.isEnemy)
            return;

        g.Actors.SelectedHero = g.Actors.FocusedActor;
        g.SortingManager.OnSelectedHeroDrag();

        // If the selected hero is already moving, do not process further drag logic.
        if (g.Actors.SelectedHero.flags.IsMoving)
            return;

        g.Card.Clear();
        g.FocusIndicator.Hide();

        g.AudioManager.Play("Click");
        g.TimerBar2D.Play();
        g.ActorManager.CheckEnemyAP();

        g.Actors.SelectedHero.move.TriggerMoveTowardsCursor();
    }

    /// <summary>
    /// Handles dropping a dragged hero, snapping them to the grid and checking for attackResults.
    /// </summary>
    public void Drop()
    {
        // Only proceed if it's the hero's turn, Seek phase is active, and there's a selected hero currently moving.
        if (!g.TurnManager.isHeroTurn 
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

        g.TimerBar2D.Pause();
        g.PincerAttackManager.Check(Team.Hero);

        // Do NOT advance phase here—TurnManager/UI is responsible for that!
    }

}
