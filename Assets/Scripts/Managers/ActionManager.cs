using Assets.Scripts.Models;
using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    // External properties
    protected TurnManager turnManager => GameManager.instance.turnManager;

    private ActionQueue<TurnAction> pendingActions = new ActionQueue<TurnAction>();


    public void AddAction(TurnAction action)
    {
        pendingActions.Add(action);
    }

    public void InsertAction(TurnAction action)
    {
        pendingActions.Insert(action);
    }

    public void TriggerExecuteActions()
    {
        StartCoroutine(ExecuteActions());
    }

    private IEnumerator ExecuteActions()
    {
        while (pendingActions.Count > 0)
        {
            TurnAction action = pendingActions.Dequeue();
            yield return StartCoroutine(action.Execute());
        }

        turnManager.NextTurn(); // Move to the next turn after all actions are executed
    }

}
