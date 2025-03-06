using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trigger
{
    private IEnumerator routine;
    private bool isAsync;
    private MonoBehaviour context;

    // Indicates whether the trigger has executed.
    public bool HasTriggered { get; private set; }

    // Dictionary for storing attributes.
    private Dictionary<string, object> attributes = new Dictionary<string, object>();


    public Trigger() { }

    // Constructor accepts a routine and an optional isAsync flag.
    public Trigger(IEnumerator routine, bool isAsync = false)
    {
        this.routine = routine;
        this.isAsync = isAsync;
        HasTriggered = false;
    }

    // Sets the MonoBehaviour context for running the coroutine.
    public void SetContext(MonoBehaviour context)
    {
        this.context = context;
    }

    // Sets an attribute value with a given key.
    public void SetAttribute(string key, object value)
    {
        attributes[key] = value;
    }

    // Gets an attribute value by key.
    public object GetAttribute(string key, object defaultValue)
    {
        object value;
        if (attributes.TryGetValue(key, out value))
            return value;

        return defaultValue;
    }

    // Starts the routine; if isAsync is true, it doesn't wait for completion.
    public IEnumerator StartCoroutine()
    {
        if (context == null)
            yield break;

        if (routine == null)
            yield break;

        if (isAsync)
        {
            //Fire and Forget: Start the routine without yielding
            context.StartCoroutine(routine);
            HasTriggered = true;
            yield break;
        }
        else
        {
            //Wait until the routine completes
            yield return context.StartCoroutine(routine);
            HasTriggered = true;
        }
    }
}
