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
    protected ActorInstance previousMovingPlayer { get => GameManager.instance.previousSelectedPlayer; set => GameManager.instance.previousSelectedPlayer = value; }
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

    public void Select()
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
            StartCoroutine(focusedActor.move.MoveTowardCursor());
    }

    public void Deselect()
    {
        // If no focused actor, do nothing.
        if (!hasFocusedActor)
            return;

        // If nothing is being moved, snap the focused actor back to its current tile.
        if (!hasSelectedPlayer)
            focusedActor.position = focusedActor.currentTile.position;

        focusedActor = null;
    }

    public void Drag()
    {
        // TriggerEnqueueAttacks abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || !hasFocusedActor || focusedActor.isEnemy)
            return;

        // Set the selected player to be moved.
        selectedPlayer = focusedActor;
        Deselect();


        // When the phase switches to Move on the player turn, start the timer and enable enemy AP checking.
        audioManager.Play("Load");
        timerBar.Play();
        actorManager.CheckEnemyAP();
        if (selectedPlayer != null)
            StartCoroutine(selectedPlayer.move.MoveTowardCursor());

        // Change phase from Start to Move.
        turnManager.SetPhase(TurnPhase.Move);
    }

    public void Drop()
    {
        //TriggerEnqueueAttacks abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasSelectedPlayer)
            return;

        //Snap the moving player to the closest tile.
        var closestTile = Geometry.GetClosestTile(selectedPlayer.position);
        selectedPlayer.location = closestTile.location;
        selectedPlayer.position = closestTile.position;
        previousMovingPlayer = selectedPlayer;
        selectedPlayer = null;

        //Reset UI and other elements.
        tileManager.Reset();
        cardManager.Reset();
        timerBar.Pause();


        attackManager.Check();

        //actionManager.Add(new PincerAttackAction());
        //turnManager.SetPhase(TurnPhase.Attack);
        //actionManager.TriggerExecute();
    }
}
