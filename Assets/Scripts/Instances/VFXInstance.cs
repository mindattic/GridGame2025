using Assets.Scripts.Events;
using Assets.Scripts.Models;
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

    public IEnumerator Spawn(VisualEffectAsset vfx, Vector3 position, TriggerEvent trigger = null)
    {
        if (trigger == null)
            trigger = new TriggerEvent(null);

        // Setup the position and scale based on the VFX resource.
        this.position = position;
        this.scale = tileScale.MultiplyBy(vfx.RelativeScale);
        SetLooping(vfx.IsLoop);

        // Optionally wait for a delay before starting.
        if (vfx.Delay != 0f)
            yield return new WaitForSeconds(vfx.Delay);

        // Run TriggerEvent (if applicable)
        yield return trigger.Execute(this);

        // Wait until the VFX duration completes.
        if (vfx.Duration != 0f)
            yield return new WaitForSeconds(vfx.Duration);

        // Destroy the VFX after completion.
        Despawn(name);
    }


    private void SetLooping(bool isLoop)
    {
        var particleSystems = new List<ParticleSystem>();
        GetRecursively(ref particleSystems, transform);

        //SelectProfile the looping flag for each ParticleSystem
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
