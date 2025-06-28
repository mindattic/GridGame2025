using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{

    public interface IAsyncEvent
    {
        public IEnumerator Execute(MonoBehaviour context);
    }

    public class AsyncEvent: IAsyncEvent
    {
        private IEnumerator routine;
        private MonoBehaviour context;

        // Indicates whether the evt has executed.
        public bool HasExecuted;

        // Dictionary for storing attributes.
        private Dictionary<string, object> attributes = new Dictionary<string, object>();


        public AsyncEvent() { }

        // Constructor accepts a routine
        public AsyncEvent(IEnumerator routine)
        {
            this.routine = routine;
            HasExecuted = false;
        }

        //// Sets an attribute value with a given key.
        //public void SetAttribute(string key, object value)
        //{
        //    attributes[key] = value;
        //}

        //// Gets an attribute value by key.
        //public object GetAttribute(string key, object defaultValue)
        //{
        //    object value;
        //    if (attributes.TryGetValue(key, out value))
        //        return value;

        //    return defaultValue;
        //}

        // Starts the routine; if isAsync is true, it doesn't wait for completion.
        public virtual IEnumerator Execute(MonoBehaviour context)
        {
            if (context == null)
                yield break;

            if (routine == null)
                yield break;

            this.context = context;

            //Wait until the routine completes
            yield return context.StartCoroutine(routine);
            HasExecuted = true;

            //if (isAsync)
            //{
            //    //Fire and Forget: Start the routine without yielding
            //    context.Execute(routine);
            //    HasExecuted = true;
            //    yield break;
            //}
            //else
            //{
            //    //Wait until the routine completes
            //    yield return context.Execute(routine);
            //    HasExecuted = true;
            //}
        }
    }
}