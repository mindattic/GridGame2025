using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXInstance : MonoBehaviour
{

   //Quick Reference Properties
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected Vector3 tileScale => GameManager.instance.tileScale;


    //Properties
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }

    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }

    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }




    public IEnumerator Spawn(VFXResource vfx, Vector3 position, Trigger trigger = default)
    {
        if (trigger == default)
            trigger = new Trigger();

        //Translate, rotate, and relativeScale relative to tile dimensions (determined by device)
        //var offset = Geometry.Tile.Relative.Translation(vfx.RelativeOffset);
        //var scale = Geometry.Tile.Relative.Scale(vfx.RelativeScale);
        //var rotation = Geometry.Rotation(vfx.AngularRotation);

        //this.position = position + vfx.RelativeOffset;
        this.position = position;
        this.scale = tileScale.MultiplyBy(vfx.RelativeScale);

        SetLooping(vfx.IsLoop);

        //Wait until waitDuration is over
        if (vfx.Delay != 0f)
            yield return new WaitForSeconds(vfx.Delay);

        //Trigger coroutine (if applicable)
        trigger.SetContext(this);
        yield return trigger.StartCoroutine();

        //Wait until VFX duration completes
        if (vfx.Duration != 0f)
            yield return Wait.For(vfx.Duration);

        //Destroy VFX
        Despawn(name);
    }

    private void SetLooping(bool isLoop)
    {
        var particleSystems = new List<ParticleSystem>();
        GetRecursively(ref particleSystems, transform);

        //Set the looping flag for each ParticleSystem
        foreach (var system in particleSystems)
        {
            var main = system.main;
            main.loop = isLoop;
        }
    }

    private void GetRecursively(ref List<ParticleSystem> particleSystems, Transform transform)
    {
        //SpawnActor particle system from root transform
        particleSystems.Add(transform.GetComponent<ParticleSystem>());

        //Recursively retrieve child particle systems from children transforms
        foreach (Transform child in transform)
        {
            GetRecursively(ref particleSystems, child);
        }
    }

    private void Despawn(string name)
    {
        vfxManager.Despawn(name);
    }

}
