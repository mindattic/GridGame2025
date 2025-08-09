// --- File: Assets/Scripts/Managers/SequenceManager.cs ---
using Assets.Scripts.Models;
using System;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Events;
using g = Assets.Helpers.GameHelper;

public class SequenceManager : MonoBehaviour
{
    // Queue holding pending sequence events
    private QueueCollection<SequenceEvent> queue = new QueueCollection<SequenceEvent>();

    // True while a batch execution pass is running
    private bool isExecuting;

    // Handle to the active Execute coroutine
    private Coroutine runningCoroutine;

    // ======================================================================
    // Tracking
    // ======================================================================

    /// <summary>
    /// The event most recently dequeued to begin execution.
    /// Updated just before its Execute coroutine is started.
    /// </summary>
    public SequenceEvent lastStartedSequence { get; private set; }

    /// <summary>
    /// The type name of the most recently started event.
    /// </summary>
    public string lastStartedSequenceName { get; private set; }

    /// <summary>
    /// The event most recently completed.
    /// Updated immediately after its Execute coroutine finishes.
    /// </summary>
    public SequenceEvent lastCompletedSequence { get; private set; }

    /// <summary>
    /// The type name of the most recently completed event.
    /// </summary>
    public string lastCompletedSequenceName { get; private set; }

    /// <summary>
    /// True while Execute is actively draining the queue.
    /// </summary>
    public bool IsExecuting => isExecuting;

    /// <summary>
    /// Current queued item count.
    /// </summary>
    public int Count => queue.Count;

    // ======================================================================
    // Events
    // ======================================================================

    /// <summary>
    /// Raised when a batch finishes with an empty queue.
    /// </summary>
    public event Action OnSequenceComplete;

    /// <summary>
    /// Raised when an individual SequenceEvent begins. Provides the event instance.
    /// </summary>
    public event Action<SequenceEvent> OnSequenceEventStarted;

    /// <summary>
    /// Raised when an individual SequenceEvent completes. Provides the event instance.
    /// </summary>
    public event Action<SequenceEvent> OnSequenceEventCompleted;

    // ======================================================================
    // Queue management
    // ======================================================================

    /// <summary>
    /// Adds a SequenceEvent to the end of the queue.
    /// </summary>
    public void Add(SequenceEvent e)
    {
        if (e == null) 
            return;

        queue.Add(e);
    }

    /// <summary>
    /// Adds a SequenceEvent to the front of the queue.
    /// </summary>
    public void AddFirst(SequenceEvent e)
    {
        if (e == null) 
            return;

        queue.AddFirst(e);
    }

    // ======================================================================
    // Execution control
    // ======================================================================

    //public void ExecuteAsync(SequenceEvent e)
    //{
    //    Add(e);
    //    ExecuteAsync();
    //}

    /// <summary>
    /// Starts executing queued SequenceEvents if not already running.
    /// Safe to call multiple times.
    /// </summary>
    public void ExecuteAsync()
    {
        if (queue.Count == 0)
        {
            Debug.Log("[SequenceManager] ExecuteAsync ignored. Queue is empty.");
            return;
        }

        if (isExecuting)
        {
            // Already draining the queue. Items added during execution will be picked up.
            return;
        }

        runningCoroutine = StartCoroutine(Execute());
    }

    //public IEnumerator Execute(SequenceEvent e)
    //{
    //    Add(e);
    //    yield return Execute();
    //}

    /// <summary>
    /// Executes all queued SequenceEvents one by one, then raises OnSequenceComplete.
    /// Controls the isExecuting state and the coroutine handle.
    /// </summary>
    public IEnumerator Execute()
    {
        if (isExecuting)
            yield break;

        isExecuting = true;
        SequenceEvent current = null;

        try
        {
            while (queue.Count > 0)
            {
                current = queue.Remove();

                // Track start
                lastStartedSequence = current;
                lastStartedSequenceName = current?.GetType().Name;

                OnSequenceEventStarted?.Invoke(current);

                // Run to completion
                if (current != null)
                    yield return StartCoroutine(current.Execute());

                // Track completion
                lastCompletedSequence = current;
                lastCompletedSequenceName = current?.GetType().Name;

                OnSequenceEventCompleted?.Invoke(current);
            }

            // Batch complete
            OnSequenceComplete?.Invoke();
        }
        finally
        {
            isExecuting = false;
            runningCoroutine = null;
        }
    }

    /// <summary>
    /// Best-effort nudge to resume execution if items are pending but not running.
    /// This is safe to call from the outside if you ever suspect a stall.
    /// </summary>
    //public void EnsureRunning()
    //{
    //    if (!isExecuting && queue.Count > 0)
    //        ExecuteAsync();
    //}

    /// <summary>
    /// Stops the currently running Execute coroutine, if any, and resets state.
    /// Pending queued items remain intact.
    /// </summary>
    //public void CancelCurrentRun()
    //{
    //    if (runningCoroutine != null)
    //    {
    //        StopCoroutine(runningCoroutine);
    //        runningCoroutine = null;
    //    }

    //    isExecuting = false;
    //}


    // ======================================================================
    // Lifecycle
    // ======================================================================

    private void OnDisable()
    {
        // Despawn active run
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        // Reset and drop pending items to avoid leaking into next scene
        isExecuting = false;
        queue.Clear();
    }

    // ======================================================================
    // Diagnostics helpers
    // ======================================================================

    /// <summary>
    /// Returns a human readable line that summarizes recent activity.
    /// </summary>
    public string GetDetails()
    {
        string started = lastStartedSequenceName ?? "-";
        string completed = lastCompletedSequenceName ?? "-";

        return $"Executing={isExecuting}\nStarted={started}\nCompleted={completed}\nQueueCount={queue.Count}";
    }
}
