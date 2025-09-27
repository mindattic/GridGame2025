using System;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

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
        Apex = other.Apex;
        Duration = other.Duration;
        IsLoop = other.IsLoop;
    }


    //Name of this VFX asset
    public string Name;

    //Prefab to spawn
    public GameObject Prefab;

    //Offset from target to spawn at relative to target's center
    public Vector3 RelativeOffset;

    //Rotation to apply to the prefab when spawning
    public Vector3 AngularRotation;

    //Scale to apply to the prefab when spawning relative to original prefab scale
    public Vector3 RelativeScale;

    // Whether this VFX should loop until manually despawned
    public bool IsLoop;

    // Time since spawn + Delay at which the apex occurs
    public float Apex = 1f;

    //Time in ms since + Deplay before vfx despawns
    public float Duration;
}
