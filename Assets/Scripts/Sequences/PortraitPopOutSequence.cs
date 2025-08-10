using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class PortraitPopOutSequence : SequenceEvent
    {
        private ActorInstance actor;
        public PortraitPopOutSequence(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            yield return g.Portrait3DManager.PopOutRoutine(actor);
        }
    }


}
