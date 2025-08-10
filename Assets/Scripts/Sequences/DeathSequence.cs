using Assets.Helpers;
using Assets.Scripts.Events;
using System.Collections;

namespace Assets.Scripts.Sequences
{
    public class DeathSequence : SequenceEvent
    {
        public override IEnumerator ProcessRoutine()
        {
            yield return DeathHelper.ProcessRoutine();
        }
    }
}
