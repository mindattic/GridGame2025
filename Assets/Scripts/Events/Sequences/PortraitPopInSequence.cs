using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class PortraitPopInSequence : SequenceEvent
    {
        private ActorInstance actor;
        public PortraitPopInSequence(ActorInstance actor)
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

            yield return g.PortraitManager.PopIn(actor, scale);
        }
    }


}
