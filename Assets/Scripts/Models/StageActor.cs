using Assets.Scripts.Models;
using System;
using UnityEngine;

[Serializable]
public class StageActor
{
    public string Character;
    public Team Team;
    public int Level = 1;
    public int SpawnTurn;
    public Vector2Int? Location;

    [NonSerialized]
    public ActorStats Stats;

    public StageActor() { }

    public StageActor(StageActor other, Vector2Int? location = null)
    {
        Character = other.Character;
        Team = other.Team;
        Level = other.Level;
        SpawnTurn = other.SpawnTurn;
        Location = location.HasValue ? location.Value : other.Location;
        AssignStats();
    }

    public StageActor(string character, Team team = Team.Enemy, int level = 1, Vector2Int? location = null)
    {
        Character = character;
        Team = team;
        Level = level;
        SpawnTurn = 0;
        Location = location.HasValue ? location.Value : null;
        AssignStats();
    }

    public void AssignStats()
    {
        if (ActorRepo.instance != null && ActorRepo.instance.Actors.ContainsKey(Character))
        {
            var actor = ActorRepo.instance.Actors[Character];
            Stats = actor.GetStats(Level);
        }
        else
        {
            Debug.LogError($"StageActor failed to assign stats for Character: {Character}");
        }
    }
}
