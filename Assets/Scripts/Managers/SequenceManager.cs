using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Events;
using System;
using g = Assets.Helpers.GameManagerHelper;

public class SequenceManager : MonoBehaviour
{
    //Fields
    private QueueCollection<SequenceEvent> queue = new QueueCollection<SequenceEvent>();

    // Event raised when all sequences are completed
    public event Action OnSequenceComplete;

    /// <summary>
    /// Adds a SequenceEvent to the queue.
    /// </summary>
    public void Add(SequenceEvent e)
    {
        queue.Add(e);
    }

    /// <summary>
    /// Adds a SequenceEvent to the front of the queue.
    /// </summary>
    public void AddFirst(SequenceEvent e)
    {
        queue.AddFirst(e);
    }

    /// <summary>
    /// Starts executing queued SequenceEvents.
    /// </summary>
    public void TriggerExecute()
    {
        StartCoroutine(Execute());
    }

    /// <summary>
    /// Executes all queued SequenceEvents one by one, raising OnSequenceComplete at the end.
    /// </summary>
    public IEnumerator Execute()
    {
        while (queue.Count > 0)
        {
            var e = queue.Remove();
            yield return StartCoroutine(e.Execute());
        }
        // All sequences complete, raise event
        OnSequenceComplete?.Invoke();
    }
}
