using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Action = Assets.Scripts.Models.PhaseAction;

namespace Assets.Scripts.Models
{

    public enum InsertOrder
    {
        Before,
        After
    }

    public class ActionQueue<Action>
    {
        //Properties
        public int Count => queue.Count;

        //Fields
        private LinkedList<Action> queue = new LinkedList<Action>();

        public void Add(Action item) => queue.AddLast(item); // Normal enqueue

        public void Insert(Action item) => queue.AddFirst(item); // Add to top

      
        public void Insert(Action item, Action node, InsertOrder order = InsertOrder.Before)
        {
            var @this = queue.Find(node);
            if (@this == null)
                throw new UnityException($"Node `{@this}` not found.");

            if (order == InsertOrder.Before)
                queue.AddBefore(@this, item);
            else
                queue.AddAfter(@this, item);
        }

        public Action Remove()
        {
            if (queue.Count == 0) return default;
            Action value = queue.First.Value;
            queue.RemoveFirst();
            return value;
        }

    }

}
