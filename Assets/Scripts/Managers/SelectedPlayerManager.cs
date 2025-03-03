using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedPlayerManager : MonoBehaviour
{
    //Quick Reference Properties
    protected Card cardManager => GameManager.instance.cardManager;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
    protected Vector3 mouseOffset { get => GameManager.instance.mouseOffset; set => GameManager.instance.mouseOffset = value; }
    protected ActorInstance focusedActor { get => GameManager.instance.focusedActor; set => GameManager.instance.focusedActor = value; }
    protected ActorInstance previousSelectedPlayer { get => GameManager.instance.previousSelectedPlayer; set => GameManager.instance.previousSelectedPlayer = value; }
    protected ActorInstance selectedPlayer { get => GameManager.instance.selectedPlayer; set => GameManager.instance.selectedPlayer = value; }
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected TimerBar timerBar => GameManager.instance.timerBar;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;
    protected AttackManager attackManager => GameManager.instance.attackManager;

    private void Start()
    {

    }

    public void Focus()
    {
        // TriggerEnqueueAttacks abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase)
            return;

        // Find actor using collision overlap.
        var collisions = Physics2D.OverlapPointAll(mousePosition3D);
        if (collisions == null)
            return;
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null)
            return;
        var actor = collider.gameObject.GetComponent<ActorInstance>();
        if (actor == null || !actor.isActive || !actor.isAlive)
            return;

        // Clear selection boxes from all actors, then select this actor.
        actors.ForEach(x => x.render.SetSelectionBoxEnabled(isEnabled: false));
        focusedActor = actor;
        focusedActor.render.SetSelectionBoxEnabled(isEnabled: true);

        // Calculate mouse offset.
        mouseOffset = focusedActor.position - mousePosition3D;

        // Update the card UI.
        cardManager.Assign(focusedActor);

        if (focusedActor.isPlayer)
            //StartCoroutine(focusedActor.movement.MoveTowardCursor());
            focusedActor.onDragDetected?.Invoke();


    }

    public void Drag()
    {
        //Check abort conditions
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || !hasFocusedActor || focusedActor.isEnemy)
            return;

        //Set the selected player to be moved.
        selectedPlayer = focusedActor;


        // When the phase switches to Move on the player turn, start the timer and enable enemy AP checking.
        audioManager.Play("Load");
        timerBar.Play();
        actorManager.CheckEnemyAP();
        if (selectedPlayer != null)
            selectedPlayer.onDragDetected?.Invoke();

        // Change phase from Start to Move.
        turnManager.SetPhase(TurnPhase.Move);
    }

    public void Drop()
    {
        //Check abort conditions
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasSelectedPlayer || !selectedPlayer.flags.IsMoving)
        {
            if (hasFocusedActor)
                focusedActor.position = focusedActor.currentTile.position;
            return;
        }

        //Snap the moving player to the closest tile.
        selectedPlayer.movement.SnapToLocation();
        previousSelectedPlayer = selectedPlayer;
        selectedPlayer = null;
        focusedActor = null;

        //Reset UI and other elements.
        tileManager.Reset();
        cardManager.Reset();
        timerBar.Pause();

        attackManager.Check();
    }
}
