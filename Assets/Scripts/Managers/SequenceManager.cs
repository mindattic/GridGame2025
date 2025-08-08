// --- File: Assets/Scripts/Managers/SequenceManager.cs ---
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Events;
using System;
using g = Assets.Helpers.GameManagerHelper;
using Unity.VisualScripting;

public class SequenceManager : MonoBehaviour
{
    // Fields
    private QueueCollection<SequenceEvent> queue = new QueueCollection<SequenceEvent>();

    // Tracks whether an Execute pass is currently running
    private bool isExecuting;

    // Holds the running Execute coroutine so it can be stopped if needed
    private Coroutine runningCoroutine;

    // Event raised when all sequences are completed
    public event Action OnSequenceComplete;

    // ---------------------------------------------------------------------
    // Queue management
    // ---------------------------------------------------------------------

    /// <summary>
    /// Adds a SequenceEvent to the end of the queue.
    /// </summary>
    public void Add(SequenceEvent e)
    {
        Debug.Log($"Sequence Add: {e.GetType().Name}");
        queue.Add(e);
    }

    /// <summary>
    /// Adds a SequenceEvent to the front of the queue.
    /// </summary>
    public void AddFirst(SequenceEvent e)
    {
        Debug.Log($"Sequence AddFirst: {e.GetType().Name}");
        queue.AddFirst(e);
    }

    /// <summary>
    /// Clears any pending items from the queue without running them.
    /// Useful when changing scenes or cancelling turn context.
    /// </summary>
    public void Clear()
    {
        // QueueCollection does not expose Clear, so drain it
        while (queue.Count > 0)
            queue.Remove();
    }

    // ---------------------------------------------------------------------
    // Execution control
    // ---------------------------------------------------------------------

    /// <summary>
    /// Starts executing queued SequenceEvents if not already running.
    /// Safe to call multiple times; only the first call will start a run
    /// until the current batch completes.
    /// </summary>
    public void TriggerExecute()
    {
        // No work to do
        if (queue.Count == 0)
            return;

        // Prevent concurrent runs
        if (isExecuting)
            return;

        // Start a single controlled pass
        runningCoroutine = StartCoroutine(Execute());
    }

    /// <summary>
    /// Executes all queued SequenceEvents one by one, then raises OnSequenceComplete.
    /// This method sets and clears isExecuting, and nulls the runningCoroutine handle.
    /// </summary>
    public IEnumerator Execute()
    {
        // Double guard in case Execute was started directly
        if (isExecuting)
            yield break;

        isExecuting = true;
        SequenceEvent e = null;

        try
        {
            
            // Process until queue is empty
            while (queue.Count > 0)
            {
                e = queue.Remove();

                // Run each SequenceEvent to completion
                // Each SequenceEvent internally yields for any async work

                Debug.Log($"Sequence Execute: {e.GetType().Name}");
                yield return StartCoroutine(e.Execute());
            }

            // All sequences complete, raise event exactly once per batch
            OnSequenceComplete?.Invoke();
        }
        finally
        {
            // Always reset execution state
            isExecuting = false;
            runningCoroutine = null;
            Debug.Log($"Sequence Execute finally: {e.GetType().Name}");
        }
    }

    // ---------------------------------------------------------------------
    // Lifecycle
    // ---------------------------------------------------------------------

    /// <summary>
    /// If this manager is disabled while executing, stop gracefully and
    /// clear any queued work to avoid leaking callbacks into the next scene.
    /// </summary>
    private void OnDisable()
    {
        // Stop the active Execute pass
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        // Reset state and drop pending events
        isExecuting = false;
        Clear();
    }
}
