using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public abstract class PhaseAction
    {
        // StartCoroutine returns an IEnumerator so that it can yield for asynchronous operations.
        public abstract IEnumerator Execute();
    }
}
