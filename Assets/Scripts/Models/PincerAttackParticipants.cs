using Game.Behaviors.Actor;
using System.Collections.Generic;
using System.Linq;

public class PincerAttackParticipants
{
    // Only maintain the list of aligned pairs.
    public List<ActorPair> alignedPairs = new List<ActorPair>();

    public PincerAttackParticipants() { }

    /// <summary>
    /// Gets all unique actor instances that are participating in any aligned pair.
    /// </summary>
    public List<ActorInstance> Get()
    {
        return alignedPairs
            .SelectMany(pair => new[] { pair.actor1, pair.actor2 }
                .Concat(pair.opponents)) // 'opponents' can be used if still relevant.
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Clears all aligned pairs and resets each pair.
    /// </summary>
    public void Clear()
    {
        alignedPairs.ForEach(pair => pair.Reset());
        alignedPairs.Clear();
    }

    /// <summary>
    /// Checks if an aligned pair already exists.
    /// </summary>
    public bool IsAlignedPair(ActorInstance actor1, ActorInstance actor2)
    {
        return alignedPairs.Any(pair => pair.ContainsActorPair(actor1, actor2));
    }
}
