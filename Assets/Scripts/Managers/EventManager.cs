using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Events;

public class EventManager : MonoBehaviour
{

    //Quick Reference Properties
    protected TurnManager turnManager => GameManager.instance.turnManager;

    //Fields
    private QueueCollection<GameEvent> queue = new QueueCollection<GameEvent>();

    public void Add(GameEvent e)
    {
        queue.Add(e);
    }

    public void Insert(GameEvent e)
    {
        queue.Insert(e);
    }

    public void TriggerExecute()
    {
        StartCoroutine(Execute());
    }

    public IEnumerator Execute()
    {
        while (queue.Count > 0)
        {
            var e = queue.Remove();
            yield return StartCoroutine(e.Execute());
        }
    }

}
