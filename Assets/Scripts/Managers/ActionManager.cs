using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using Action = Assets.Scripts.Models.PhaseAction;

public class ActionManager : MonoBehaviour
{

    // External properties
    protected TurnManager turnManager => GameManager.instance.turnManager;

    //Fields
    private ActionQueue<Action> pendingActions = new ActionQueue<Action>();

    public void Add(Action action)
    {
        pendingActions.Add(action);
    }

    public void Insert(Action action)
    {
        pendingActions.Insert(action);
    }

    public void TriggerExecute()
    {
        StartCoroutine(Execute());
    }

    private IEnumerator Execute()
    {
        while (pendingActions.Count > 0)
        {
            var action = pendingActions.Remove();
            yield return StartCoroutine(action.Execute());
        }

        turnManager.NextTurn(); // Move to the next turn after all actions are executed
    }

}
