using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger
{
    //Variables
    public IEnumerator Coroutine = null;
    public bool IsAsync = true;
    public bool HasTriggered = false;
    private Dictionary<string, object> attributes = new Dictionary<string, object>();

    //Properties
    public bool IsValid => Coroutine != null && !HasTriggered;

    //Constructors
    public Trigger() { }
    public Trigger(IEnumerator coroutine)
    {
        Coroutine = coroutine;
    }
    public Trigger(IEnumerator coroutine, bool isAsync)
    {
        Coroutine = coroutine;
        IsAsync = isAsync;
    }

    public IEnumerator StartCoroutine(MonoBehaviour context = null)
    {
        if (!IsValid || HasTriggered)
            yield break;

        HasTriggered = true;

        if (!IsAsync || context == null)
            yield return Coroutine;
        else
            context.StartCoroutine(Coroutine);
    }

    public void AddAttribute(string key, object value)
    {
        attributes[key] = value;
    }

    public T GetAttribute<T>(string key, T defaultValue = default)
    {
        return attributes.TryGetValue(key, out var value) ? (T)value : defaultValue;
    }

    public bool HasAttribute(string key)
    {
        return attributes.ContainsKey(key);
    }

    ///<summary>
    ///Method which is used to create a deep clone of a Trigger object
    ///</summary>
    ///<returns></returns>
    public Trigger Clone()
    {
        var clone = new Trigger
        {
            Coroutine = this.Coroutine,
            IsAsync = this.IsAsync
        };

        //Copy attributes dictionary
        foreach (var kvp in this.attributes)
        {
            clone.AddAttribute(kvp.Key, kvp.Value);
        }

        return clone;
    }

}
