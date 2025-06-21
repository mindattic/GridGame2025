using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Actions
{
    public class PortraitPopInAction : PhaseAction
    {
        private PortraitManager portraitManager => GameManager.instance.portraitManager;

        private ActorInstance actor;
        public PortraitPopInAction(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            yield return portraitManager.PopIn(actor);
        }
    }


}
