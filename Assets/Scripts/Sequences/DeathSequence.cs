using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Sequences
{
    public class DeathSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            yield return DeathHelper.ExecuteTrigger();
        }
    }
}
