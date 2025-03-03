using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedPlayerManager : MonoBehaviour
{
    //Quick Reference Properties
    protected Card cardManager => GameManager.instance.card;
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
    protected PincerAttackManager attackManager => GameManager.instance.pincerAttackManager;
    protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator;
    protected Card card => GameManager.instance.card;
    protected float tileSize => GameManager.instance.tileSize;

    public void Focus()
    {
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase)
            return;

        var collisions = Physics2D.OverlapPointAll(mousePosition3D);
        if (collisions == null)
            return;
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null)
            return;
        var actor = collider.gameObject.GetComponent<ActorInstance>();
        if (actor == null || !actor.isPlaying)
            return;

        if (focusedActor == actor)
            return;

        focusedActor = actor;
        focusIndicator.Assign();
        mouseOffset = focusedActor.position - mousePosition3D;
        cardManager.Assign();
    }

    public void Drag()
    {
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || !hasFocusedActor || focusedActor.isEnemy)
            return;

        selectedPlayer = focusedActor;

        if (selectedPlayer.flags.IsMoving)
            return;

        card.Clear();
        focusIndicator.Clear();
        audioManager.Play("Load");
        timerBar.Play();
        actorManager.CheckEnemyAP();
        turnManager.SetPhase(TurnPhase.Move);

        selectedPlayer.onDragDetected?.Invoke();
    }

    public void Drop()
    {
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasSelectedPlayer || !selectedPlayer.flags.IsMoving)
        {
            if (hasFocusedActor)
                focusedActor.position = focusedActor.currentTile.position;
            return;
        }

        selectedPlayer.movement.SnapToLocation();
        previousSelectedPlayer = selectedPlayer;
        selectedPlayer = null;
        focusedActor = null;

        timerBar.Pause();
        attackManager.Check();
    }
}
