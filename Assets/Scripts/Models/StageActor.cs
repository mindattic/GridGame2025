using Assets.Scripts.Models;
using System;
using UnityEngine;
using static Game.Instances.Actor.ActorLayer;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;
using UnityEngine.TextCore.Text;

[Serializable]
public class StageActor
{
    public string Character;
    public int Level = 1;
    [NonSerialized] public Team Team;
    [NonSerialized] public int SpawnTurn;
    [NonSerialized] public Vector2Int? Location;
    [NonSerialized] public ActorStats Stats;

    public StageActor() { }

    //Copy constructor
    public StageActor(StageActor other)
    {
        Character = other.Character;
        Team = other.Team;
        Level = other.Level;
        SpawnTurn = other.SpawnTurn;
        Location = other.Location;
        AssignStats();
    }

    public StageActor(string character, Team team, int level, Vector2Int? location = null)
    {
        Character = character;
        Team = team;
        Level = level;
        SpawnTurn = 0;
        Location = location.HasValue ? location.Value : Random.UnoccupiedLocation;
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
