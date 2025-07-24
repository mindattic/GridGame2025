using System;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

[Serializable]
public class VFXAsset
{
    public VFXAsset() { }

    public VFXAsset(VFXAsset other)
    {
        Name = other.Name;
        Prefab = other.Prefab;
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
    public float Delay;
    public float Duration;
    public bool IsLoop;
   
}
