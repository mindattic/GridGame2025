using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedPlayerManager : MonoBehaviour
{
    // External properties
    protected Card cardManager => GameManager.instance.cardManager;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
    protected Vector3 mouseOffset { get => GameManager.instance.mouseOffset; set => GameManager.instance.mouseOffset = value; }
    protected ActorInstance selectedActor { get => GameManager.instance.focusedActor; set => GameManager.instance.focusedActor = value; }
    protected ActorInstance previousMovingPlayer { get => GameManager.instance.previousSelectedPlayer; set => GameManager.instance.previousSelectedPlayer = value; }
    protected ActorInstance movingPlayer { get => GameManager.instance.selectedPlayer; set => GameManager.instance.selectedPlayer = value; }
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    protected bool hasSelectedActor => selectedActor != null;
    protected bool hasMovingPlayer => movingPlayer != null;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;

    private void Start()
    {

    }

    public void Select()
    {
        // Check abort conditions.
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
        selectedActor = actor;
        selectedActor.render.SetSelectionBoxEnabled(isEnabled: true);

        // Calculate mouse offset.
        mouseOffset = selectedActor.position - mousePosition3D;

        // Update the card UI.
        cardManager.Assign(selectedActor);

        if (selectedActor.isPlayer)
            StartCoroutine(selectedActor.move.MoveTowardCursor());
    }

    public void Deselect()
    {
        // If no focused actor, do nothing.
        if (!hasSelectedActor)
            return;

        // If nothing is being moved, snap the focused actor back to its current tile.
        if (!hasMovingPlayer)
            selectedActor.position = selectedActor.currentTile.position;

        selectedActor = null;
    }

    public void Drag()
    {
        // Check abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || !hasSelectedActor || selectedActor.isEnemy)
            return;

        // Set the selected player to be moved.
        movingPlayer = selectedActor;
        Deselect();


        // When the phase switches to Move on the player turn, start the timer and enable enemy AP checking.
        audioManager.Play("Load");
        timerBar.Play();
        actorManager.CheckEnemyAP();
        if (movingPlayer != null)
            StartCoroutine(movingPlayer.move.MoveTowardCursor());

        // Change phase from Start to Move.
        turnManager.SetPhase(TurnPhase.Move);
    }

    public void Drop()
    {
        //Check abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasMovingPlayer)
            return;

        //Snap the moving player to the closest tile.
        var closestTile = Geometry.GetClosestTile(movingPlayer.position);
        movingPlayer.location = closestTile.location;
        movingPlayer.position = closestTile.position;
        previousMovingPlayer = movingPlayer;
        movingPlayer = null;

        //Reset UI and other elements.
        tileManager.Reset();
        cardManager.Reset();
        timerBar.Pause();

        //Now that the player has dropped their unit,
        //add the PlayerAttackAction and trigger the execution of pending turn actions.
        actionManager.Add(new PincerAttackAction());
        turnManager.SetPhase(TurnPhase.Attack);
    }
}
