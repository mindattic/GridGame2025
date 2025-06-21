using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Actions
{
    public class PortraitPopOutAction : PhaseAction
    {
        private PortraitManager portraitManager => GameManager.instance.portraitManager;

        private ActorInstance actor;
        public PortraitPopOutAction(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            yield return portraitManager.PopOut(actor);
        }
    }


}
