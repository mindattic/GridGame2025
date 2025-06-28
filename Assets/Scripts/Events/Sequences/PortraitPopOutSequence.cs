using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class PortraitPopOutSequence : SequenceEvent
    {
        private PortraitManager portraitManager => GameManager.instance.portraitManager;

        private ActorInstance actor;
        public PortraitPopOutSequence(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            yield return portraitManager.PopOut(actor);
        }
    }


}
