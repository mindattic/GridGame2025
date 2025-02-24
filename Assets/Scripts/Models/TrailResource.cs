using UnityEngine;

public class TrailResource
{
    public TrailResource() { }
    public TrailResource(TrailResource other)
    {
        Name = other.Name;
        RelativeOffset = other.RelativeOffset;
        AngularRotation = other.AngularRotation;
        RelativeScale = other.RelativeScale;
        Delay = other.Delay;
        Duration = other.Duration;
        IsLoop = other.IsLoop;
    }


    public string Name;
    public GameObject Prefab;
    public Vector3 RelativeOffset;
    public Vector3 AngularRotation;
    public Vector3 RelativeScale;
    public float Delay = 0f;
    public float Duration = 2f;
    public bool IsLoop = true;
}
