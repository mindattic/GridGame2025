using System.Collections;

namespace Assets.Scripts.Events
{
    public abstract class GameEvent
    {
        // StartCoroutine returns an IEnumerator so that it can yield for asynchronous operations.
        public abstract IEnumerator Execute();
    }
}
