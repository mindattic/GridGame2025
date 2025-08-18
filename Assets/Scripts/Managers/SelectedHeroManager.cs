// --- File: Assets/Scripts/Managers/SelectedHeroManager.cs ---
using Assets.Helpers;
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
    /// Selects an actor under the mouse cursor, updates focus visuals, and refreshes ability buttons.
    /// Allowed only during the hero's turn.
    /// </summary>
    public void Focus()
    {
        // Abort if not the hero's turn.
        if (!g.TurnManager.isHeroTurn)
            return;

        // Try to find an actor under the current touch position.
        var target = TouchHelper.GetActorAtTouchPosition();
        if (target == null || !target.isPlaying)
            return;

        // If selecting the same actor again, do nothing.
        if (g.Actors.FocusedActor == target)
            return;

        // Always clear ability buttons when changing selection.
        g.AbilityButtonManager.Hide();

        // Update focus reference and sorting when selection changes.
        g.Actors.FocusedActor = target;
        g.SortingManager.OnActorFocus();

        // Recreate ability buttons for heroes only. Enemies leave the container empty.
        if (g.Actors.FocusedActor.isHero)
            g.AbilityButtonManager.Show(g.Actors.FocusedActor);

        // Cache touch offset so drag begins from the same relative point.
        g.TouchOffset = g.Actors.FocusedActor.position - g.TouchPosition3D;

        // Update focus indicator and card UI.
        g.FocusIndicator.Show();
        g.Card.Assign();

        // Notify editor to reload thumbnails while in the editor.
#if UNITY_EDITOR
        GameManager.instance.reloadThumbnailSettings = true;
#endif
    }

    /// <summary>
    /// Begins dragging the focused hero if eligible and starts movement toward the cursor.
    /// Also handles initial UI and sound feedback for a drag.
    /// </summary>
    public void Drag()
    {
        // Only proceed if it is the hero's turn, a hero is focused, and that actor is not an enemy.
        if (!g.TurnManager.isHeroTurn || !g.Actors.HasFocusedActor || g.Actors.FocusedActor.isEnemy)
            return;

        // Mark this focused hero as the selected hero for movement.
        g.Actors.SelectedHero = g.Actors.FocusedActor;
        g.SortingManager.OnSelectedHeroDrag();

        // If the selected hero is already moving, do not process further drag logic.
        if (g.Actors.SelectedHero.Flags.IsMoving)
            return;

        // Start moving the selected hero toward the cursor.
        g.Actors.SelectedHero.Move.MoveTowardCursor();
    
        g.Card.Clear();
        g.FocusIndicator.Hide();
        g.AudioManager.Play("Click");
        g.TimerBar2D.Play();
        g.ActorManager.CheckEnemyAP();  
    }

    /// <summary>
    /// Drops a dragged hero. Snaps to nearest tile, updates sorting, and checks pincer attacks.
    /// Phase advancement is handled by the turn system/UI, not here.
    /// </summary>
    public void Drop()
    {
        // If not a valid drop state, gently reset any focused actor back to its tile.
        if (!g.TurnManager.isHeroTurn
            || !g.Actors.HasSelectedHero || !g.Actors.SelectedHero.Flags.IsMoving)
        {
            if (g.Actors.HasFocusedActor)
                g.Actors.FocusedActor.position = g.Actors.FocusedActor.currentTile.position;
            return;
        }

        // Complete movement and restore sorting.
        g.Actors.SelectedHero.Move.ToLocation();
        g.SortingManager.OnSelectedHeroDrop();

        // Clear selection references.
        g.Actors.SelectedHero = null;
        g.Actors.FocusedActor = null;

        // Pause timer and evaluate any pincer attacks for heroes.
        g.TimerBar2D.Pause();
        g.PincerAttackManager.Check(Team.Hero);

        // Do not advance phase here. TurnManager/UI is responsible for that.
    }
}
