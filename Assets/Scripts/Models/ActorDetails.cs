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
        Trivia = other.Trivia != null ? new List<string>(other.Trivia) : new List<string>();
    }

    public string Description;
    public string Card;
    public List<string> Trivia = new List<string>();
}
