using System;
using UnityEngine;

[Serializable]
public class StageActor
{
    public Character Character;
    public Team Team;
    public int SpawnTurn;
    public Vector2Int? Location;  // Assuming Location is nullable; Vector2Int is a struct, so it's copied by value.
   
    public StageActor() { }

    public StageActor(StageActor other)
    {
        Character = other.Character;
        Team = other.Team;
        SpawnTurn = other.SpawnTurn;
        Location = other.Location.HasValue ? other.Location : Random.UnoccupiedLocation;
   
    }

    public StageActor(Character character, Team team)
    {
        Character = character;
        Team = team;
        SpawnTurn = 0;
        Location = Random.UnoccupiedLocation;
    }

}



/*
 * using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class StageActor
    {
        public StageActor() { }


        public StageActor(StageActor other)
        {
            Character = other.Character;
            Team = other.Team;
            SpawnTurn = other.SpawnTurn;
            Location = other.Location.HasValue ? other.Location : Random.UnoccupiedLocation;
        }

        public StageActor(Character character, Team team)
        {
            Character = character;
            Team = team;
            SpawnTurn = 0;
            Location = Random.UnoccupiedLocation;
        }

        public Character Character;
        public Team Team;
        public int SpawnTurn;
        public Vector2Int? Location;
    }
}
*/