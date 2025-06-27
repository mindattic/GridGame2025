using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class PortraitPopInEvent : AwaitEvent
    {
        private PortraitManager portraitManager => GameManager.instance.portraitManager;

        private ActorInstance actor;
        public PortraitPopInEvent(ActorInstance actor)
        {
            this.actor = actor;
        }

        public override IEnumerator Execute()
        {
            float scale;

            if (actor.HasAdjacent(Direction.North, 1))
                scale = 0.075f;
            else if (actor.HasAdjacent(Direction.North, 2))
                scale = 0.1f;
            else
                scale = 0.1666f;

            yield return portraitManager.PopIn(actor, scale);
        }
    }


}
