using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class ActionQueue<T>
    {
        private LinkedList<T> queue = new LinkedList<T>();

        public void Insert(T item) => queue.AddFirst(item); // Enqueue to top

        public void Add(T item) => queue.AddLast(item); // Normal enqueue

        public T Dequeue()
        {
            if (queue.Count == 0) return default;
            T value = queue.First.Value;
            queue.RemoveFirst();
            return value;
        }

        public int Count => queue.Count;
    }
}
