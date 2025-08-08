// --- File: Assets/Scripts/Managers/SequenceManager.cs ---
using Assets.Scripts.Models;
using System;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Events;
using g = Assets.Helpers.GameManagerHelper;

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
    public SequenceEvent LastStartedEvent { get; private set; }

    /// <summary>
    /// The type name of the most recently started event.
    /// </summary>
    public string LastStartedEventName { get; private set; }

    /// <summary>
    /// The event most recently completed.
    /// Updated immediately after its Execute coroutine finishes.
    /// </summary>
    public SequenceEvent LastCompletedEvent { get; private set; }

    /// <summary>
    /// The type name of the most recently completed event.
    /// </summary>
    public string LastCompletedEventName { get; private set; }

    /// <summary>
    /// True while Execute is actively draining the queue.
    /// </summary>
    public bool IsExecuting => isExecuting;

    /// <summary>
    /// Current queued item count.
    /// </summary>
    public int QueueCount => queue.Count;

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
    public event Action<SequenceEvent> OnSequenceItemStarted;

    /// <summary>
    /// Raised when an individual SequenceEvent completes. Provides the event instance.
    /// </summary>
    public event Action<SequenceEvent> OnSequenceItemCompleted;

    // ======================================================================
    // Queue management
    // ======================================================================

    /// <summary>
    /// Adds a SequenceEvent to the end of the queue.
    /// </summary>
    public void Add(SequenceEvent e)
    {
        if (e == null) return;

        Debug.Log($"[SequenceManager] Add: {e.GetType().Name}");
        queue.Add(e);
    }

    /// <summary>
    /// Adds a SequenceEvent to the front of the queue.
    /// </summary>
    public void AddFirst(SequenceEvent e)
    {
        if (e == null) return;

        Debug.Log($"[SequenceManager] AddFirst: {e.GetType().Name}");
        queue.AddFirst(e);
    }

    /// <summary>
    /// Clears any pending items from the queue and resets tracking of the current run.
    /// Does not stop a currently running item. Use CancelCurrentRun to stop execution.
    /// </summary>
    public void Clear()
    {
        while (queue.Count > 0)
            queue.Remove();
    }

    /// <summary>
    /// Resets the LastStarted and LastCompleted tracking fields.
    /// </summary>
    public void ResetTracking()
    {
        LastStartedEvent = null;
        LastStartedEventName = null;

        LastCompletedEvent = null;
        LastCompletedEventName = null;
    }

    // ======================================================================
    // Execution control
    // ======================================================================

    /// <summary>
    /// Starts executing queued SequenceEvents if not already running.
    /// Safe to call multiple times.
    /// </summary>
    public void TriggerExecute()
    {
        if (queue.Count == 0)
        {
            Debug.Log("[SequenceManager] TriggerExecute ignored. Queue is empty.");
            return;
        }

        if (isExecuting)
        {
            // Already draining the queue. Items added during execution will be picked up.
            return;
        }

        Debug.Log($"[SequenceManager] TriggerExecute. QueueCount={queue.Count}");
        runningCoroutine = StartCoroutine(Execute());
    }

    /// <summary>
    /// Best-effort nudge to resume execution if items are pending but not running.
    /// This is safe to call from the outside if you ever suspect a stall.
    /// </summary>
    public void EnsureRunning()
    {
        if (!isExecuting && queue.Count > 0)
            TriggerExecute();
    }

    /// <summary>
    /// Stops the currently running Execute coroutine, if any, and resets state.
    /// Pending queued items remain intact.
    /// </summary>
    public void CancelCurrentRun()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        isExecuting = false;
        Debug.Log("[SequenceManager] Current run canceled.");
    }

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
                LastStartedEvent = current;
                LastStartedEventName = current?.GetType().Name;

                Debug.Log($"[SequenceManager] Execute Start: {LastStartedEventName}");
                OnSequenceItemStarted?.Invoke(current);

                // Run to completion
                if (current != null)
                    yield return StartCoroutine(current.Execute());

                // Track completion
                LastCompletedEvent = current;
                LastCompletedEventName = current?.GetType().Name;

                Debug.Log($"[SequenceManager] Execute Done: {LastCompletedEventName}");
                OnSequenceItemCompleted?.Invoke(current);
            }

            // Batch complete
            Debug.Log("[SequenceManager] Batch complete. Queue is empty.");
            OnSequenceComplete?.Invoke();
        }
        finally
        {
            isExecuting = false;
            runningCoroutine = null;

            var name = current != null ? current.GetType().Name : "(none)";
            Debug.Log($"[SequenceManager] Execute finally. Last processed: {name}");
        }
    }

    // ======================================================================
    // Lifecycle
    // ======================================================================

    private void OnDisable()
    {
        // Stop active run
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        // Reset and drop pending items to avoid leaking into next scene
        isExecuting = false;
        Clear();

        Debug.Log("[SequenceManager] OnDisable. Stopped and cleared.");
    }

    // ======================================================================
    // Diagnostics helpers
    // ======================================================================

    /// <summary>
    /// Returns a human readable line that summarizes recent activity.
    /// </summary>
    public string GetLastActivitySummary()
    {
        string started = LastStartedEventName ?? "(none)";
        string completed = LastCompletedEventName ?? "(none)";

        return $"Executing={isExecuting}\nStarted={started}\nCompleted={completed}\nQueueCount={queue.Count}";
    }
}
