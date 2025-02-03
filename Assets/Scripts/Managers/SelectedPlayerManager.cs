using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Manager;
using System.Collections;
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
    protected ActorInstance focusedActor { get => GameManager.instance.focusedActor; set => GameManager.instance.focusedActor = value; }
    protected ActorInstance previousSelectedPlayer { get => GameManager.instance.previousSelectedPlayer; set => GameManager.instance.previousSelectedPlayer = value; }
    protected ActorInstance selectedPlayer { get => GameManager.instance.selectedPlayer; set => GameManager.instance.selectedPlayer = value; }
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;


    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected TileManager tileManager => GameManager.instance.tileManager;

    private void Start()
    {
        // Subscribe to the TurnManager's OnTurnPhaseChanged event.
        turnManager.OnTurnPhaseChanged += phase =>
        {
            Debug.Log("Turn phase changed to: " + phase);

            if (phase == TurnPhase.Attack)
            {
                if (turnManager.isPlayerTurn)
                {
                    // For a player turn, we assume that a PlayerAttackAction has already been added.
                    // Trigger execution of the attack phase (for example, by calling a public method on TurnManager).
                    turnManager.TriggerExecuteTurn();
                }
                else if (turnManager.isEnemyTurn)
                {
                    // If it's enemy turn, check if any enemy is ready to attack.
                    bool anyEnemyReady = enemies.Any(x => x.isActive && x.isAlive && x.stats.AP == x.stats.MaxAP);
                    if (!anyEnemyReady)
                    {
                        // If no enemy is ready, immediately advance to the next turn (which will be player turn).
                        turnManager.NextTurn();
                    }
                    // Otherwise, enemy actions will be executed as part of the enemy turn.
                }
            }
        };
    }

    public void Focus()
    {
        // Check abort conditions
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase)
            return;

        // Find actor using collision overlap
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

    public void Unfocus()
    {
        // If no focused actor, do nothing.
        if (!hasFocusedActor)
            return;

        // If nothing is selected, snap the focused actor back.
        if (!hasSelectedPlayer)
            focusedActor.position = focusedActor.currentTile.position;

        focusedActor = null;
    }

    public void Select()
    {
        // Check abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || focusedActor == null || focusedActor.isEnemy)
            return;

        // Set the selected player.
        selectedPlayer = focusedActor;
        Unfocus();

        // Change phase from Start to Move by invoking the property.
        turnManager.CurrentPhase = TurnPhase.Move;
        audioManager.Play("Select");
        timerBar.Play();
        actorManager.CheckEnemyAP();
        StartCoroutine(selectedPlayer.move.MoveTowardCursor());
    }

    public void Unselect()
    {
        // Check abort conditions.
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasSelectedPlayer)
            return;

        // Snap the selected player to the closest tile.
        var closestTile = Geometry.GetClosestTile(selectedPlayer.position);
        closestTile.spriteRenderer.color = ColorHelper.Translucent.White;
        selectedPlayer.location = closestTile.location;
        selectedPlayer.position = closestTile.position;
        previousSelectedPlayer = selectedPlayer;
        selectedPlayer = null;

        // Reset UI and other elements.
        tileManager.Reset();
        cardManager.Reset();
        timerBar.Pause();

        // Add a PlayerAttackAction to the Attack phase.
        turnManager.AddActionToPhase(TurnPhase.Attack, new PlayerAttackAction());

        // Change the turn phase to Pre-Attack using the property so that the event fires.
        turnManager.CurrentPhase = TurnPhase.Attack;
    }
}
