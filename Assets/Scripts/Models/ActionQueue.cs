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
            var nodeRef = queue.Find(node);
            if (nodeRef == null)
                throw new UnityException($"Node `{nodeRef}` not found.");

            if (order == InsertOrder.Before)
                queue.AddBefore(nodeRef, item);
            else
                queue.AddAfter(nodeRef, item);
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
