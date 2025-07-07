using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles selection, dragging, and dropping of heroes during the correct turn/phase.
/// Interacts with multiple core game systems via the GameManager singleton.
/// </summary>
public class SelectedHeroManager : MonoBehaviour
{
    // Quick reference properties for accessing core game systems
    protected InputManager inputManager => GameManager.instance.inputManager;
    protected AbilityButtonManager abilityButtonManager => GameManager.instance.abilityButtonManager;
    protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;
    protected PincerAttackManager attackManager => GameManager.instance.pincerAttackManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected TimerBar timerBar => GameManager.instance.timerBar;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActorInstance focusedActor { 
        get => GameManager.instance.focusedActor; 
        set => GameManager.instance.focusedActor = value; 
    }
    protected bool hasFocusedActor => focusedActor != null;
    protected ActorInstance targetActor
    {
        get => GameManager.instance.targetActor;
        set => GameManager.instance.targetActor = value;
    }
    protected bool hasTargetActor => targetActor != null;

    protected ActorInstance selectedHero { 
        get => GameManager.instance.selectedHero; 
        set => GameManager.instance.selectedHero = value; 
    }
    protected bool hasSelectedHero => selectedHero != null;

    protected Card card => GameManager.instance.card;
    protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator;
    protected Vector3 touchOffset { get => GameManager.instance.touchOffset; set => GameManager.instance.touchOffset = value; }
    protected Vector3 touchPosition3D => GameManager.instance.touchPosition3D;
    protected float tileSize => GameManager.instance.tileSize;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }

    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;

    /// <summary>
    /// Selects an actor under the mouse cursor, updating the focus indicator and actor card UI.
    /// Allowed ONLY during the hero's turn and at the start of their phase.
    /// </summary>
    public void Focus()
    {
        // Only allow focus selection during the hero's turn and Start phase.
        if (!turnManager.isHeroTurn || turnManager.currentPhase != TurnPhase.Start)
            return;

        var collisions = Physics2D.OverlapPointAll(touchPosition3D);
        if (collisions == null) return;
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null) return;
        var actor = collider.gameObject.GetComponent<ActorInstance>();

        if (actor == null || !actor.isPlaying) return;

        if (focusedActor == actor)
            return;

        focusedActor = actor;
        sortingManager.OnActorFocus();

        if (focusedActor.isHero)
            abilityButtonManager.Show(focusedActor);

        touchOffset = focusedActor.position - touchPosition3D;

        focusIndicator.Assign();
        card.Assign();
    }

    /// <summary>
    /// Handles dragging an actor, setting up move. Starts Move phase if at Start, otherwise continues drag in Move phase.
    /// </summary>
    public void Drag()
    {
        // Only proceed if it's the hero's turn, a hero is focused, and that actor is not an enemy.
        if (!turnManager.isHeroTurn || !hasFocusedActor || focusedActor.isEnemy)
            return;

        // Accept drag ONLY if we are in Start or Move phase:
        if (turnManager.currentPhase != TurnPhase.Start && turnManager.currentPhase != TurnPhase.Move)
            return;

        // If at Start, this drag triggers the transition to Move phase:
        if (turnManager.currentPhase == TurnPhase.Start)
        {
            turnManager.SetPhase(TurnPhase.Move);
            // Optional: if you want to only allow drag *after* Move phase started, return here
            // return;
        }

        selectedHero = focusedActor;
        sortingManager.OnSelectedHeroDrag();

        // If the selected hero is already moving, do not process further drag logic.
        if (selectedHero.flags.IsMoving)
            return;

        card.Clear();
        focusIndicator.Clear();

        audioManager.Play("Click");
        timerBar.Play();
        actorManager.CheckEnemyAP();

        selectedHero.move.TriggerMoveTowardsCursor();
    }

    /// <summary>
    /// Handles dropping a dragged hero, snapping them to the grid and checking for attackResults.
    /// </summary>
    public void Drop()
    {
        // Only proceed if it's the hero's turn, Move phase is active, and there's a selected hero currently moving.
        if (!turnManager.isHeroTurn || turnManager.currentPhase != TurnPhase.Move || !hasSelectedHero || !selectedHero.flags.IsMoving)
        {
            if (hasFocusedActor)
                focusedActor.position = focusedActor.currentTile.position;
            return;
        }

        selectedHero.move.ToLocation();
        sortingManager.OnSelectedHeroDrop();

        selectedHero = null;
        focusedActor = null;

        timerBar.Pause();
        attackManager.Check(Team.Hero);

        // Do NOT advance phase here—TurnManager/UI is responsible for that!
    }

}
