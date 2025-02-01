using Game.Behaviors;
using Game.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedPlayerManager : MonoBehaviour
{
    #region Properties
    protected CombatManager combatManager => GameManager.instance.combatManager;
    protected Card cardManager => GameManager.instance.cardManager;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
    protected Vector3 mouseOffset { get => GameManager.instance.mouseOffset; set => GameManager.instance.mouseOffset = value; }
    protected ActorInstance focusedActor { get => GameManager.instance.focusedActor; set => GameManager.instance.focusedActor = value; }
    protected ActorInstance previousSelectedPlayer { get => GameManager.instance.previousSelectedPlayer; set => GameManager.instance.previousSelectedPlayer = value; }
    protected ActorInstance selectedPlayer { get => GameManager.instance.selectedPlayer; set => GameManager.instance.selectedPlayer = value; }
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    #endregion

    public void Focus()
    {
        //Check abort conditions
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase)
            return;

        //Find actor using collision overlap
        var collisions = Physics2D.OverlapPointAll(mousePosition3D);
        if (collisions == null)
            return;
        var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
        if (collider == null)
            return;
        var actor = collider.gameObject.GetComponent<ActorInstance>();
        if (actor == null || !actor.isActive || !actor.isAlive)
            return;

        //TODO: SaveProfile Card display...
        actors.ForEach(x => x.render.SetSelectionBoxEnabled(isEnabled: false));
        focusedActor = actor;
        focusedActor.render.SetSelectionBoxEnabled(isEnabled: true);

        //Assign mouse relative offset (how off center was selection?)
        mouseOffset = focusedActor.position - mousePosition3D;

        cardManager.Assign(focusedActor);

        if (focusedActor.isPlayer)
            StartCoroutine(focusedActor.move.MoveTowardCursor());
    }

    public void Unfocus()
    {
        //Verify *HAS* focused actor...
        if (!hasFocusedActor)
            return;

        if (!hasSelectedPlayer)
            focusedActor.position = focusedActor.currentTile.position;

        focusedActor = null;
    }

    public void Select()
    {
        //Check abort conditions
        if (!turnManager.isPlayerTurn || !turnManager.isStartPhase || focusedActor == null || focusedActor.isEnemy)
            return;

        //Select player
        selectedPlayer = focusedActor;
        Unfocus();

        turnManager.currentPhase = TurnPhase.Move;
        audioManager.Play("Select");
        timerBar.Play();
        actorManager.CheckEnemyAP();
        StartCoroutine(selectedPlayer.move.MoveTowardCursor());
    }

    public void Unselect()
    {
        //Check abort conditions
        if (!turnManager.isPlayerTurn || !turnManager.isMovePhase || !hasSelectedPlayer)
            return;

        //Assign boardLocation and boardPosition
        var closestTile = Geometry.GetClosestTile(selectedPlayer.position);
        closestTile.spriteRenderer.color = ColorHelper.Translucent.White;
        selectedPlayer.location = closestTile.location;
        selectedPlayer.position = closestTile.position;
        previousSelectedPlayer = selectedPlayer;
        selectedPlayer = null;

        //ghostManager.Stop();
        //footstepManager.Stop();

        tileManager.Reset();
        cardManager.Reset();
        timerBar.Pause();
        turnManager.currentPhase = TurnPhase.Attack;

        combatManager.TriggerCombat();
    }


}
