using Game.Behaviors.Actor;
using System.Collections.Generic;
using System.Linq;

public class CombatParticipants
{
    public List<CombatPair> alignedPairs = new List<CombatPair>();
    public List<CombatPair> attackingPairs = new List<CombatPair>();
    public List<CombatPair> supportingPairs = new List<CombatPair>();

    public CombatParticipants() { }

    //Select all participants
    public List<ActorInstance> Get()
    {
        return attackingPairs
                .Concat(supportingPairs)
                .SelectMany(x => new[] { x.actor1, x.actor2 }.Concat(x.opponents).Concat(x.allies))
                .Distinct()
                .ToList();
    }

    //Select all participants in actor x
    public List<ActorInstance> Get(CombatPair pair)
    {

        return new[] { pair.actor1, pair.actor2 }
            .Concat(pair.opponents)
            .Concat(pair.allies)
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

    public bool HasAlignedPair(ActorInstance actor1, ActorInstance actor2)
    {
        return alignedPairs.Count > 0 && alignedPairs.Any(x => x.HasPair(actor1, actor2));
    }


    public bool HasAttackingPair(CombatPair pair)
    {
        return attackingPairs.Count > 0 && attackingPairs.Any(x => x.HasPair(pair.actor1, pair.actor2));
    }

    public bool HasSupportingPair(CombatPair pair)
    {
        return supportingPairs.Count > 0 && supportingPairs.Any(x => x.HasPair(pair.actor1, pair.actor2));
    }

}
