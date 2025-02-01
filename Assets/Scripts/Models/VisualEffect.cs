using UnityEngine;

public class VisualEffect
{
    public string Name;
    public GameObject Prefab;
    public Vector3 RelativeOffset;
    public Vector3 AngularRotation;
    public Vector3 RelativeScale;
    public float Delay = 0f;
    public float Duration = 2f;
    public bool IsLoop;
}
