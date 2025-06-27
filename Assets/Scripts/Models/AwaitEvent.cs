using System.Collections;

namespace Assets.Scripts.Models
{
    public abstract class AwaitEvent
    {
        // Execute returns an IEnumerator so that it can yield for asynchronous operations.
        public abstract IEnumerator Execute();
    }
}
