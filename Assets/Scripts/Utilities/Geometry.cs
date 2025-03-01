using Game.Behaviors;
using Game.Behaviors.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Geometry
{
    private static BoardInstance board => GameManager.instance.board;
    private static float tileSize => GameManager.instance.tileSize;
    private static Vector3 tileScale => GameManager.instance.tileScale;
    private static List<ActorInstance> actors => GameManager.instance.actors;
    private static List<TileInstance> tiles => GameManager.instance.tiles;

    //private static Dictionary<Vector2Int, Vector3> boardPositions = new Dictionary<Vector2Int, Vector3>();
    //private static Dictionary<Vector3, Vector2Int> boardLocations = new Dictionary<Vector3, Vector2Int>();


    public Geometry()
    {

        //Assign lookup dictionaries
        //tiles.ForEach(x => boardPositions.SpawnActor(x.boardLocation, x.boardPosition));
        //tiles.ForEach(x => boardLocations.SpawnActor(x.boardPosition, x.boardLocation));
    }


    public static Vector3 CalculatePositionByLocation(Vector2Int location)
    {
        //return boardPositions[boardLocation];
        float x = board.offset.x + (tileSize * location.x);
        float y = board.offset.y + -(tileSize * location.y);
        return new Vector3(x, y, 0);
    }

    public static Vector3 GetPositionByLocation(Vector2Int location)
    {
        return board.tileMap.GetPosition(location);
    }

    public static Vector2Int GetLocationByPosition(Vector3 position)
    {
        return board.tileMap.GetLocation(position);
    }
    //public static Vector2Int GetLocation(int col, int row)
    //{
    //   col = Math.Clamp(col, 1, board.columnCount);
    //   row = Math.Clamp(row, 1, board.rowCount);
    //   return new Vector2Int(col, row);
    //}

    //public static Vector2Int LocationFromPosition(Vector3 boardLocation)
    //{
    //   int x = Mathf.FloorToInt(boardLocation.x / tileSize - board.relativeOffset.x);
    //   int y = Mathf.FloorToInt(boardLocation.y / tileSize - board.relativeOffset.y);
    //   return new Vector2Int(x, y);
    //}

    public static TileInstance GetClosestTile(Vector3 position)
    {
        return tiles.OrderBy(x => Vector3.Distance(x.transform.position, position)).First();
    }

    public static TileInstance GetClosestTile(Vector2Int location)
    {
        return tiles.First(x => x.location == location);
        //return tiles.OrderBy(x => Vector2Int.Distance(x.boardLocation, boardLocation)).First();
    }


    //public static Vector3 ClosestTilePosition(Vector2Int boardLocation)
    //{
    //   return tiles.OrderBy(x => Vector2Int.Distance(x.boardLocation, boardLocation)).First().boardPosition;
    //}





    public static bool IsSameColumn(ActorInstance a, ActorInstance b) => a.location.x == b.location.x;
    public static bool IsSameRow(ActorInstance a, ActorInstance b) => a.location.y == b.location.y;
    public static bool IsNorthOf(ActorInstance a, ActorInstance b) => IsSameColumn(a, b) && a.location.y == b.location.y - 1;
    public static bool IsEastOf(ActorInstance a, ActorInstance b) => IsSameRow(a, b) && a.location.x == b.location.x + 1;
    public static bool IsSouthOf(ActorInstance a, ActorInstance b) => IsSameColumn(a, b) && a.location.y == b.location.y + 1;
    public static bool IsWestOf(ActorInstance a, ActorInstance b) => IsSameRow(a, b) && a.location.x == b.location.x - 1;
    public static bool IsNorthWestOf(ActorInstance a, ActorInstance b) => a.location.x == b.location.x - 1 && a.location.y == b.location.y - 1;
    public static bool IsNorthEastOf(ActorInstance a, ActorInstance b) => a.location.x == b.location.x + 1 && a.location.y == b.location.y - 1;
    public static bool IsSouthWestOf(ActorInstance a, ActorInstance b) => a.location.x == b.location.x - 1 && a.location.y == b.location.y + 1;
    public static bool IsSouthEastOf(ActorInstance a, ActorInstance b) => a.location.x == b.location.x + 1 && a.location.y == b.location.y + 1;
    public static bool IsAdjacentTo(ActorInstance a, ActorInstance b) => (IsSameColumn(a, b) || IsSameRow(a, b)) && Vector2Int.Distance(a.location, a.location).Equals(1);


    public static bool IsSameColumn(Vector2Int a, Vector2Int b) => a.x == b.x;
    public static bool IsSameRow(Vector2Int a, Vector2Int b) => a.y == b.y;
    public static bool IsNorthOf(Vector2Int a, Vector2Int b) => IsSameColumn(a, b) && a.y == b.y - 1;
    public static bool IsEastOf(Vector2Int a, Vector2Int b) => IsSameRow(a, b) && a.x == b.x + 1;
    public static bool IsSouthOf(Vector2Int a, Vector2Int b) => IsSameColumn(a, b) && a.y == b.y + 1;
    public static bool IsWestOf(Vector2Int a, Vector2Int b) => IsSameRow(a, b) && a.x == b.x - 1;
    public static bool IsNorthWestOf(Vector2Int a, Vector2Int b) => a.x == b.x - 1 && a.y == b.y - 1;
    public static bool IsNorthEastOf(Vector2Int a, Vector2Int b) => a.x == b.x + 1 && a.y == b.y - 1;
    public static bool IsSouthWestOf(Vector2Int a, Vector2Int b) => a.x == b.x - 1 && a.y == b.y + 1;
    public static bool IsSouthEastOf(Vector2Int a, Vector2Int b) => a.x == b.x + 1 && a.y == b.y + 1;
    public static bool IsAdjacentTo(Vector2Int a, Vector2Int b) => (IsSameColumn(a, b) || IsSameRow(a, b)) && Vector2Int.Distance(a, b).Equals(1);


    public static Direction AdjacentDirectionTo(ActorInstance a, ActorInstance b)
    {
        if (!IsAdjacentTo(a, b)) return Direction.None;
        if (IsNorthOf(a, b)) return Direction.South;
        if (IsEastOf(a, b)) return Direction.West;
        if (IsSouthOf(a, b)) return Direction.North;
        if (IsWestOf(a, b)) return Direction.East;

        return Direction.None;
    }


    public static Direction CalculateDirection(Vector2 a, Vector2 b)
    {
        Vector2 difference = b - a;

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            //Horizontal movement
            return difference.x > 0 ? Direction.East : Direction.West;
        }
        else
        {
            //Vertical movement
            return difference.y > 0 ? Direction.North : Direction.South;
        }
    }

    public static Vector2Int GetClosestAttackLocation(Vector2Int attackerLocation, Vector2Int defenderLocation)
    {
        //Determine if already adjacent to player...
        if (IsAdjacentTo(attackerLocation, defenderLocation))
            return attackerLocation;

        //Swap boardPosition with target
        return defenderLocation;



        /*


        //...Otherwise, Find closest unoccupied tile adjacent to player...
        var closestUnoccupiedAdjacentTile = GetClosestUnoccupiedAdjacentTileByLocation(boardPosition.boardLocation);
        if (closestUnoccupiedAdjacentTile != null)
            return closestUnoccupiedAdjacentTile.boardPosition;

        //...Otherwise, Find closest tile adjacent to player...
        var closestAdjacentTile = GetClosestAdjacentTileByLocation(boardPosition.boardLocation);
        if (closestAdjacentTile != null)
            return closestAdjacentTile.boardPosition;

        //...Otherwise, find closest unoccupied tile to player...
        var closestUnoccupiedTile = GetClosestUnoccupiedTileByLocation(boardPosition.boardLocation);
        if (closestUnoccupiedTile != null)
            return closestUnoccupiedTile.boardPosition;

        //...Otherwise, find closest tile to player
        var closestTile = GetClosestTile(boardPosition.boardLocation);
        if (closestTile != null)
            return closestTile.boardPosition;

        return attacker.boardPosition;
        */
    }

    public static TileInstance GetClosestUnoccupiedTileByLocation(Vector2Int other)
    {
        return tiles.FirstOrDefault(x => !x.IsOccupied && Vector2Int.Distance(x.location, other) == 1);
    }

    public static TileInstance GetClosestUnoccupiedAdjacentTileByLocation(Vector2Int other)
    {
        return tiles.FirstOrDefault(x => !x.IsOccupied && x.IsAdjacentTo(other));
    }

    public static TileInstance GetClosestAdjacentTileByLocation(Vector2Int other)
    {
        return tiles.FirstOrDefault(x => x.IsAdjacentTo(other));
    }

    public static Vector3 GetDirectionalPosition(Vector3 position, Direction direction, float amount)
    {
        return direction switch
        {
            Direction.North => new Vector3(position.x, position.y + amount, position.z),
            Direction.East => new Vector3(position.x + amount, position.y, position.z),
            Direction.South => new Vector3(position.x, position.y + -amount, position.z),
            Direction.West => new Vector3(position.x + -amount, position.y, position.z),
            _ => position,
        };
    }

    public static bool IsInCorner(Vector2Int location)
    {
        return location == board.tileMap.GetLocation(1, 1)  //A1
            || location == board.tileMap.GetLocation(1, 6)  //A6
            || location == board.tileMap.GetLocation(8, 1)  //H1
            || location == board.tileMap.GetLocation(8, 6); //H6
    }


    public static float GetPercentageBetween(Vector3 startPosition, Vector3 endPosition, Vector3 currentPosition)
    {
        //Calculate the vectors
        Vector3 AB = endPosition - startPosition;
        Vector3 AC = currentPosition - startPosition;

        //TriggerEnqueueAttacks for division by zero; Handle the case where startPosition and endPosition are the same point
        if (AB.magnitude == 0)
            return 0;

        //Calculate the percentage along the line segment
        float percentage = AC.magnitude / AB.magnitude;
        return percentage;
    }



    public static List<Vector2Int> GetLocationsBetween(Vector2Int start, Vector2Int end)
    {
        var result = new List<Vector2Int>();

        // If same row
        if (start.y == end.y)
        {
            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);

            // Instead of for(int x=...), use Enumerable.Range
            foreach (int x in Enumerable.Range(minX + 1, maxX - (minX + 1)))
            {
                result.Add(new Vector2Int(x, start.y));
            }
        }
        // If same col
        else if (start.x == end.x)
        {
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            foreach (int y in Enumerable.Range(minY + 1, maxY - (minY + 1)))
            {
                result.Add(new Vector2Int(start.x, y));
            }
        }

        return result;
    }



    ///<summary>
    ///Methods which calculate values relative to another unit
    ///(which is calculated based on currentPosition device aspect ratio, screen size, etc)
    ///</summary>
    public static class Tile
    {

        public static class Relative
        {
            public static Vector3 Translation(float x, float y, float z)
            {
                return new Vector3(
                    tileSize * (x / tileSize),
                    tileSize * (y / tileSize),
                    tileSize * (z / tileSize));
            }
            public static Vector3 Translation(Vector3 v) => Translation(v.x, v.y, v.z);

            public static Vector3 Scale(float x, float y, float z)
            {
                return new Vector3(
                    tileSize * (x),
                    tileSize * (y),
                    tileSize * (z));
            }
            public static Vector3 Scale(Vector3 v) => Scale(v.x, v.y, v.z);

        }

    }


    public static ActorInstance GetStartActor(ActorInstance actor1, ActorInstance actor2)
    {
        // Determine axis based on Y position
        Axis axis = Math.Abs(actor1.location.y - actor2.location.y) > Math.Abs(actor1.location.x - actor2.location.x)
            ? Axis.Vertical
            : Axis.Horizontal;

        return GetStartActor(actor1, actor2, axis);
    }

    public static ActorInstance GetStartActor(ActorInstance actor1, ActorInstance actor2, Axis axis)
    {
        return axis == Axis.Horizontal
            ? actor1.location.x <= actor2.location.x ? actor1 : actor2
            : actor1.location.y >= actor2.location.y ? actor1 : actor2;
    }


    public static Quaternion Rotation(float x, float y, float z)
    {
        return Quaternion.Euler(new Vector3(x, y, z));
    }
    public static Quaternion Rotation(Vector3 v) => Rotation(v.x, v.y, v.z);

}
