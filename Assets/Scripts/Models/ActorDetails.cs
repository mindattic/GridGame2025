using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActorDetails
{
    public ActorDetails() { }

    public ActorDetails(ActorDetails other)
    {
        Description = other.Description;
        Card = other.Card;
        Lore = other.Lore != null ? new List<string>(other.Lore) : new List<string>();
    }

    public string Description;
    public string Card;
    public List<string> Lore = new List<string>();
}
