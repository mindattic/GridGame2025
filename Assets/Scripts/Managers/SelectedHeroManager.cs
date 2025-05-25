using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// SelectedPlayerManager handles input and state changes for selecting, dragging, and dropping heroes.
// It interacts with multiple core game systems via the GameManager singleton.
public class SelectedHeroManager : MonoBehaviour
{
    // Quick reference properties for accessing core game systems
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;
    protected PincerAttackManager attackManager => GameManager.instance.pincerAttackManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected TimerBar timerBar => GameManager.instance.timerBar;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActorInstance focusedActor { get => GameManager.instance.focusedActor; set => GameManager.instance.focusedActor = value; }
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedHero => selectedHero != null;
    protected ActorInstance selectedHero { get => GameManager.instance.selectedHero; set => GameManager.instance.selectedHero = value; }
    protected Card card => GameManager.instance.card;
    protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator;
    protected Vector3 touchOffset { get => GameManager.instance.touchOffset; set => GameManager.instance.touchOffset = value; }
    protected Vector3 touchPosition3D => GameManager.instance.touchPosition3D;
    protected float tileSize => GameManager.instance.tileSize;
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;

    /// <summary>
    /// Selects an actor under the mouse cursor, updating the focus indicator and actor card UI.
    /// </summary>
    public void Focus()
    {
        // Only allow focus selection during the hero's turn and the start phase.
        if (!turnManager.isHeroTurn || !turnManager.isStartPhase)
            return;

        //Retrieve the ActorInstance component from the collider
        var collisions = Physics2D.OverlapPointAll(touchPosition3D);
        if (collisions == null) return;
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null) return;
        var actor = collider.gameObject.GetComponent<ActorInstance>();

        // If no ActorInstance is found or the actor is not active, exit.
        if (actor == null || !actor.isPlaying) return;

        // If the actor under the mouse is already focused, no further action is needed.
        if (focusedActor == actor)
            return;

        // Save the focused actor to the one under the mouse.
        focusedActor = actor;
        
        focusedActor.sortingLayer = Sort.Selected;
        Debug.Log(focusedActor.sortingLayer);

        // Calculate the offset between the actor's position and the mouse position.
        touchOffset = focusedActor.position - touchPosition3D;

        // Save the UI elements
        focusIndicator.Assign();
        card.Assign();
    }

    /// <summary>
    /// Handles dragging an actor, setting up movement and updating the turn phase.
    /// </summary>
    public void Drag()
    {
        // Only proceed if it's the hero's turn, the game is in the start phase,
        // there is a focused actor, and that actor is not an enemy.
        if (!turnManager.isHeroTurn || !turnManager.isStartPhase || !hasFocusedActor || focusedActor.isEnemy)
            return;

        // Assign the selected hero to be the focused actor.
        selectedHero = focusedActor;

        // If the selected hero is already moving, do not process further drag logic.
        if (selectedHero.flags.IsMoving)
            return;

        // Clear UI elements
        card.Clear();
        focusIndicator.Clear();

        // Play an audio cue to indicate that the actor has been selected for movement.
        audioManager.Play("Click");

        // Start the movement phase:
        // - Play the timer bar animation.
        // - Check enemy action points (AP) to update available moves.
        timerBar.Play();
        actorManager.CheckEnemyAP();
        // Switch the turn phase from Start to Move.
        turnManager.SetPhase(TurnPhase.Move);

        selectedHero.sortingLayer = Sort.Selected;
        Debug.Log(focusedActor.sortingLayer);

        Debug.Log("This is a log.");
        selectedHero.movement.TriggerMoveTowardsCursor();
    }

    /// <summary>
    /// Handles dropping a dragged hero, snapping them to the grid and checking for results.
    /// </summary>
    public void Drop()
    {
        // Ensure that it's the hero's turn, the move phase is active,
        // and that there is a selected hero who is currently moving.
        if (!turnManager.isHeroTurn || !turnManager.isMovePhase || !hasSelectedHero || !selectedHero.flags.IsMoving)
        {
            // If an actor was focused but not moved, reset its position to the tile it was originally on.
            if (hasFocusedActor)
                focusedActor.position = focusedActor.currentTile.position;
            return;
        }

        // Snap the selected hero's position to the nearest valid tile location on the grid.
        selectedHero.movement.SnapToLocation();

        //Clear the CurrentProfile selection and focused actor references
        selectedHero = null;
        focusedActor = null;

        // OnPauseButtonClicked the movement timer, indicating that the move phase has ended.
        timerBar.Pause();

        // Check for any potential pincer results by the hero's team now that movement is complete.
        attackManager.Check(Team.Hero);
    }
}
