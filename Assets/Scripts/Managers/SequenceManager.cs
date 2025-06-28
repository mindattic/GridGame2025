using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Events;

public class SequenceManager : MonoBehaviour
{

    //Quick Reference Properties
    protected TurnManager turnManager => GameManager.instance.turnManager;

    //Fields
    private QueueCollection<SequenceEvent> queue = new QueueCollection<SequenceEvent>();

    public void Add(SequenceEvent e)
    {
        queue.Add(e);
    }

    public void Insert(SequenceEvent e)
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
