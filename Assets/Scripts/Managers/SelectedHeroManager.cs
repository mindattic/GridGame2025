// --- File: Assets/Scripts/Managers/SelectedHeroManager.cs ---
using Assets.Helpers;
using Game.Behaviors;
using Game.Manager;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;
using Assets.Scripts.Instances.Actor; // For ActorInstance

/// <summary>
/// Handles focus, drag, and drop for hero actors during the hero turn.
/// Starts moving the focused actor immediately on drag. Promotes to SelectedHero
/// only after the actor has moved at least half a tile from the drag start.
/// Dropping before promotion snaps the actor back to its recorded start position.
/// Always applies TouchOffset so there is no snap when clicking a different spot.
/// </summary>
public class SelectedHeroManager : MonoBehaviour
{
    // Drag state for delayed promotion
    private ActorInstance pendingActor;
    //private Vector3 pendingActorStartPosition;
    //private Vector2Int pendingActorStartLocation;
    private bool hasPendingDrag;
    private float dragThreshold;

    public void Awake()
    {
        dragThreshold = g.TileMap.tileSize / 2f;
    }

    /// <summary>
    /// Focus an actor under the cursor and update visuals and buttons.
    /// Only allowed during the hero turn.
    /// </summary>
    public void Focus()
    {
        if (!g.TurnManager.isHeroTurn)
            return;

        var target = TouchHelper.GetActorAtTouchPosition();
        if (target == null || !target.isPlaying)
            return;

        if (g.Actors.FocusedActor == target)
            return;

        g.AbilityButtonManager.Hide();

        g.Actors.FocusedActor = target;
        g.SortingManager.OnActorFocus();

        if (g.Actors.FocusedActor.isHero)
            g.AbilityButtonManager.Show(g.Actors.FocusedActor);

        // Cache offset from current finger to actor so the first move frame does not snap
        g.TouchOffset = g.Actors.FocusedActor.position - g.TouchPosition3D;

        // Reset pending drag when focus changes
        hasPendingDrag = false;
        pendingActor = null;

        g.FocusIndicator.Show();
        g.Card.Assign();

#if UNITY_EDITOR
        GameManager.instance.reloadThumbnailSettings = true;
#endif
    }

    /// <summary>
    /// Begin or continue a drag. The focused actor follows the cursor immediately.
    /// Promotion to SelectedHero is deferred until moved at least half a tile.
    /// Always recomputes TouchOffset at drag start to avoid snap when clicking a new spot.
    /// </summary>
    public void Drag()
    {
        if (!g.TurnManager.isHeroTurn || !g.Actors.HasFocusedActor || g.Actors.FocusedActor.isEnemy)
            return;

        var actor = g.Actors.FocusedActor;

        // Initialize pending drag on first Drag call or actor change
        if (!hasPendingDrag || pendingActor != actor)
        {
            hasPendingDrag = true;
            pendingActor = actor;
            //pendingActorStartPosition = actor.position;
            //pendingActorStartLocation = actor.location;

            // Recompute TouchOffset at drag begin so different grab points do not snap
            g.TouchOffset = actor.position - g.TouchPosition3D;

            if (!pendingActor.Flags.IsMoving)
                pendingActor.Move.MoveTowardCursor();

            return;
        }

        // Ensure movement continues while dragging
        if (!pendingActor.Flags.IsMoving)
            pendingActor.Move.MoveTowardCursor();

        // Already promoted
        if (g.Actors.HasSelectedHero)
            return;

        // Promote after clearing half a tile from start
        float moved = Vector3.Distance(pendingActor.position, pendingActor.currentTile.position);
        if (moved >= dragThreshold)
        {
            g.Actors.SelectedHero = pendingActor;
            g.SortingManager.OnSelectedHeroDrag();

            g.Card.Clear();
            g.FocusIndicator.Hide();
            g.AudioManager.Play("Click");
            g.TimerBar2D.Play();
            g.ActorManager.CheckEnemyAP();
        }
    }

    /// <summary>
    /// Handle drop.
    /// If a promoted hero is moving, snap to grid and resolve.
    /// If not promoted, snap the focused actor back to the exact start tile and position recorded at drag begin.
    /// </summary>
    public void Drop()
    {
        bool validSelectedMove =
            g.TurnManager.isHeroTurn &&
            g.Actors.HasSelectedHero &&
            g.Actors.SelectedHero.Flags.IsMoving;

        if (!validSelectedMove)
        {
            // No promotion occurred. Return focused actor to the recorded start tile and position.
            if (hasPendingDrag && pendingActor != null)
            {
                //pendingActor.location = pendingActorStartLocation;

                //var startTile = g.TileMap.GetTile(pendingActorStartLocation);
                //var snapPosition = startTile != null ? startTile.position : pendingActorStartPosition;

                pendingActor.Move.ToLocation();
                pendingActor.Flags.IsMoving = false;
                pendingActor.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else if (g.Actors.HasFocusedActor)
            {
                // Fallback: snap to the actor's current tile center
                g.Actors.FocusedActor.Move.ToLocation();
                g.Actors.FocusedActor.Flags.IsMoving = false;
                g.Actors.FocusedActor.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            // Clear pending drag state
            hasPendingDrag = false;
            pendingActor = null;
            return;
        }

        // Complete movement for promoted hero
        g.Actors.SelectedHero.Move.ToLocation();
        g.SortingManager.OnSelectedHeroDrop();

        // Clear selection references and pending state
        g.Actors.SelectedHero = null;
        g.Actors.FocusedActor = null;
        hasPendingDrag = false;
        pendingActor = null;

        // Finalize
        g.TimerBar2D.Pause();
        g.PincerAttackManager.Check(Team.Hero);
    }
}
