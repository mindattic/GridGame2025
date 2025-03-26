using Assets.Scripts.Models;
using System;
using UnityEngine;

[Serializable]
public class StageActor
{
    public Character Character;
    public Team Team;
    public int Level = 1;
    public int SpawnTurn;
    public Vector2Int? Location;

    // This is NOT serialized — calculated on demand.
    [NonSerialized]
    public ActorStats Stats;

    public StageActor() { }

    public StageActor(StageActor other)
    {
        Character = other.Character;
        Team = other.Team;
        Level = other.Level;
        SpawnTurn = other.SpawnTurn;
        Location = other.Location.HasValue ? other.Location : Random.UnoccupiedLocation;
        AssignStats();
    }

    public StageActor(Character character, Team team, int level = 1)
    {
        Character = character;
        Team = team;
        Level = level;
        SpawnTurn = 0;
        Location = Random.UnoccupiedLocation;
        AssignStats();
    }

    public void AssignStats()
    {
        if (ActorRepo.instance != null && ActorRepo.instance.Actors.ContainsKey(Character.ToString()))
        {
            var actor = ActorRepo.instance.Actors[Character.ToString()];
            Stats = actor.GetStats(Level);
        }
        else
        {
            Debug.LogError($"StageActor failed to assign stats for Character: {Character}");
        }
    }
}
