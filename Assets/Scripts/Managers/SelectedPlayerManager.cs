using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// SelectedPlayerManager handles input and state changes for selecting, dragging, and dropping players.
// It interacts with multiple core game systems via the GameManager singleton.
public class SelectedPlayerManager : MonoBehaviour
{
    // Quick reference properties for accessing core game systems
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected PincerAttackManager attackManager => GameManager.instance.pincerAttackManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected TimerBar timerBar => GameManager.instance.timerBar;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActorInstance focusedActor { get => GameManager.instance.focusedActor; set => GameManager.instance.focusedActor = value; }
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected ActorInstance previousSelectedPlayer { get => GameManager.instance.previousSelectedPlayer; set => GameManager.instance.previousSelectedPlayer = value; }
    protected ActorInstance selectedPlayer { get => GameManager.instance.selectedPlayer; set => GameManager.instance.selectedPlayer = value; }
    protected Card card => GameManager.instance.card;
    protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator;
    protected Vector3 mouseOffset { get => GameManager.instance.mouseOffset; set => GameManager.instance.mouseOffset = value; }
    protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
    protected float tileSize => GameManager.instance.tileSize;
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;

    /// <summary>
    /// Selects an actor under the mouse cursor, updating the focus indicator and actor card UI.
    /// </summary>
    public void Focus()
    {
        // Only allow focus selection during the player's turn and the start phase.
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase)
            return;

        // Detect all colliders at the current mouse position.
        var collisions = Physics2D.OverlapPointAll(mousePosition3D);
        if (collisions == null) return;

        // Find the first collider with the "Actor" tag.
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null) return;

        // Retrieve the ActorInstance component from the collider.
        var actor = collider.gameObject.GetComponent<ActorInstance>();
        // If no ActorInstance is found or the actor is not active, exit.
        if (actor == null || !actor.isPlaying) return;

        // If the actor under the mouse is already focused, no further action is needed.
        if (focusedActor == actor)
            return;

        // Update the focused actor to the one under the mouse.
        focusedActor = actor;
        // Calculate the offset between the actor's position and the mouse position.
        mouseOffset = focusedActor.position - mousePosition3D;

        // Update the UI elements: assign the focus indicator and update the actor card with new focus.
        focusIndicator.Assign();
        card.Assign();
    }

    /// <summary>
    /// Handles dragging an actor, setting up movement and updating the turn phase.
    /// </summary>
    public void Drag()
    {
        // Only proceed if it's the player's turn, the game is in the start phase,
        // there is a focused actor, and that actor is not an enemy.
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || !hasFocusedActor || focusedActor.isEnemy)
            return;

        // Set the selected player to be the focused actor.
        selectedPlayer = focusedActor;

        // If the selected player is already moving, do not process further drag logic.
        if (selectedPlayer.flags.IsMoving)
            return;

        // Clear UI elements (actor card and focus indicator) to prepare for movement.
        card.Clear();
        focusIndicator.Clear();
        // Play an audio cue to indicate that the actor has been selected for movement.
        audioManager.Play("Load");

        // Start the movement phase:
        // - Play the timer bar animation.
        // - Check enemy action points (AP) to update available moves.
        timerBar.Play();
        actorManager.CheckEnemyAP();
        // Switch the turn phase from Start to Move.
        turnManager.SetPhase(TurnPhase.Move);

        // Invoke any events associated with detecting a drag on the selected actor.
        selectedPlayer.onDragDetected?.Invoke();
    }

    /// <summary>
    /// Handles dropping a dragged player, snapping them to the grid and checking for attacks.
    /// </summary>
    public void Drop()
    {
        // Ensure that it's the player's turn, the move phase is active,
        // and that there is a selected player who is currently moving.
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasSelectedPlayer || !selectedPlayer.flags.IsMoving)
        {
            // If an actor was focused but not moved, reset its position to the tile it was originally on.
            if (hasFocusedActor)
                focusedActor.position = focusedActor.currentTile.position;
            return;
        }

        // Snap the selected player's position to the nearest valid tile location on the grid.
        selectedPlayer.movement.SnapToLocation();

        // Record the current selected player as the previous selection,
        // then clear the current selection and focused actor references.
        previousSelectedPlayer = selectedPlayer;
        selectedPlayer = null;
        focusedActor = null;

        // Pause the movement timer, indicating that the move phase has ended.
        timerBar.Pause();
        // Check for any potential pincer attacks by the player's team now that movement is complete.
        attackManager.Check(Team.Player);
    }
}
