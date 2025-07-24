using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

public class VFXInstance : MonoBehaviour
{
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


    public void SpawnAsync(VFXAsset vfx, Vector3 position, TriggerEvent trigger = null)
    {
        StartCoroutine(Spawn(vfx, position, trigger));
    }

    public IEnumerator Spawn(VFXAsset vfx, Vector3 position, TriggerEvent trigger = null)
    {
        this.position = position;
        transform.localPosition = vfx.RelativeOffset;
        transform.localEulerAngles = vfx.AngularRotation;
        transform.localScale = g.TileScale.MultiplyBy(vfx.RelativeScale);

        SetLooping(vfx.IsLoop);

        // Optionally wait for a delay before starting.
        if (vfx.Delay != 0f)
            yield return new WaitForSeconds(vfx.Delay);

        // Run TriggerEvent (if applicable)
        if (trigger != null)
            yield return trigger.Execute(this);

        // Wait until the VfxManager duration completes.
        if (vfx.Duration != 0f)
            yield return new WaitForSeconds(vfx.Duration);

        // Destroy the VfxManager after completion.
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
        g.VfxManager.Despawn(name);
    }

}
