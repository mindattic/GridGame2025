using UnityEngine;

public class Destination
{

    public Destination() { }

    public Vector2Int? location;
    public Vector3? position;
    public Direction direction;


    //public bool IsValid => boardLocation != null && boardPosition != null && direction != Direction.None;

    public void Clear()
    {
        location = null;
        position = null;
        direction = Direction.None;
    }

}
