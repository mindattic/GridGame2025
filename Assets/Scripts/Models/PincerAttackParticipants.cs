using Game.Behaviors.Actor;
using System.Collections.Generic;
using System.Linq;

public class PincerAttackParticipants
{
    public List<ActorPair> alignedPairs = new List<ActorPair>();
    public List<ActorPair> attackingPairs = new List<ActorPair>();
    public List<ActorPair> supportingPairs = new List<ActorPair>();

    public PincerAttackParticipants() { }

    //Load all participants
    public List<ActorInstance> Get()
    {
        return attackingPairs
                .Concat(supportingPairs)
                .SelectMany(x => new[] { x.actor1, x.actor2 }.Concat(x.opponents).Concat(x.allies))
                .Distinct()
                .ToList();
    }

    //Load all participants in actor x
    public List<ActorInstance> Get(ActorPair actorPair)
    {

        return new[] { actorPair.actor1, actorPair.actor2 }
            .Concat(actorPair.opponents)
            .Concat(actorPair.allies)
            .Distinct()
            .ToList();
    }


    public void Clear()
    {
        foreach (var list in new[] { alignedPairs, attackingPairs, supportingPairs })
        {
            list.ForEach(x => x.Reset());
            list.Clear();
        }
    }

    public bool IsAlignedPair(ActorInstance actor1, ActorInstance actor2)
    {
        return alignedPairs.Count > 0 && alignedPairs.Any(x => x.ContainsActorPair(actor1, actor2));
    }


    public bool HasAttackingPair(ActorPair actorPair)
    {
        return attackingPairs.Count > 0 && attackingPairs.Any(x => x.ContainsActorPair(actorPair.actor1, actorPair.actor2));
    }

    public bool HasSupportingPair(ActorPair actorPair)
    {
        return supportingPairs.Count > 0 && supportingPairs.Any(x => x.ContainsActorPair(actorPair.actor1, actorPair.actor2));
    }

}
