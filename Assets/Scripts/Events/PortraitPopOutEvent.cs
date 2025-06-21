using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class PortraitPopOutEvent : GameEvent
    {
        private PortraitManager portraitManager => GameManager.instance.portraitManager;

        private ActorInstance actor;
        public PortraitPopOutEvent(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            yield return portraitManager.PopOut(actor);
        }
    }


}
