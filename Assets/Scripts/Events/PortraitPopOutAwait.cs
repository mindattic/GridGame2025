using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class PortraitPopOutAwait : AwaitEvent
    {
        private PortraitManager portraitManager => GameManager.instance.portraitManager;

        private ActorInstance actor;
        public PortraitPopOutAwait(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            yield return portraitManager.PopOut(actor);
        }
    }


}
