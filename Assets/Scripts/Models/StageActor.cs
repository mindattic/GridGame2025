using Assets.Scripts.Models;
using System;
using UnityEngine;
using static Game.Instances.Actor.ActorLayer;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;
using UnityEngine.TextCore.Text;
using Assets.Scripts.Libraries;

[Serializable]
public class StageActor
{
    public string characterName;
    public int Level = 1;
    [NonSerialized] public Team Team;
    [NonSerialized] public int SpawnTurn;
    [NonSerialized] public Vector2Int? Location;
    [NonSerialized] public ActorStats Stats;

    public StageActor() { }

    //Copy constructor
    public StageActor(StageActor other)
    {
        characterName = other.characterName;
        Team = other.Team;
        Level = other.Level;
        SpawnTurn = other.SpawnTurn;
        Location = other.Location;
        AssignStats();
    }

    public StageActor(string character, Team team, int level, Vector2Int? location = null)
    {
        characterName = character;
        Team = team;
        Level = level;
        SpawnTurn = 0;
        Location = location.HasValue ? location.Value : RNG.UnoccupiedLocation;
        AssignStats();
    }

    public void AssignStats()
    {
        if (ActorLibrary.Actors.ContainsKey(characterName))
        {
            var actor = ActorLibrary.Actors[characterName];
            Stats = actor.GetStats(Level);
        }
        else
        {
            Debug.LogError($"StageActor failed to assign Stats for characterName: {characterName}");
        }
    }
}
