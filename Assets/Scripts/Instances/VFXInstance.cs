using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class VFXInstance : MonoBehaviour
{
    // Transforms for convenience.
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

    /// <summary>
    /// SpawnRoutine of a VFX at a world position. Optionally runs a routine routine afterward.
    /// </summary>
    public void Spawn(VFXAsset vfx, Vector3 position, IEnumerator routine = null)
    {
        StartCoroutine(SpawnRoutine(vfx, position, routine));
    }

    /// <summary>
    /// Yieldable spawn of a VFX at a world position. Plays optional routine routine, then despawns.
    /// </summary>
    public IEnumerator SpawnRoutine(VFXAsset vfx, Vector3 position, IEnumerator routine = null)
    {
        // 1) Place
        transform.position = position + vfx.RelativeOffset;

        // 2) Apply
        transform.eulerAngles = vfx.AngularRotation;
        transform.localScale = g.TileScale.MultiplyBy(vfx.RelativeScale);

        // Configure looping
        SetLooping(vfx.IsLoop);

        // Cache the name now, before any possible destroy by parent
        string instanceName = name;

        // Optional start delay
        if (vfx.Delay != 0f)
            yield return new WaitForSeconds(vfx.Delay);

        // Optional routine
        if (routine != null)
            yield return StartCoroutine(routine);

        // Optional lifetime
        if (vfx.Duration != 0f)
            yield return new WaitForSeconds(vfx.Duration);

        // If this was already destroyed by a parent, stop quietly
        if (this == null || gameObject == null)
            yield break;

        // Only self-despawn when there is a finite lifetime
        if (vfx.Duration > 0f)
            Despawn(instanceName);
    }


    /// <summary>
    /// Sets the loop flag on all ParticleSystem components in the transform hierarchy.
    /// </summary>
    private void SetLooping(bool isLoop)
    {
        var particleSystems = new List<ParticleSystem>();
        GetRecursively(ref particleSystems, transform);

        foreach (var system in particleSystems)
        {
            if (system == null)
                continue;

            var main = system.main;
            main.loop = isLoop;
        }
    }

    /// <summary>
    /// Collects ParticleSystem components from this transform and all children.
    /// </summary>
    private void GetRecursively(ref List<ParticleSystem> particleSystems, Transform transform)
    {
        var ps = transform.GetComponent<ParticleSystem>();
        if (ps != null)
            particleSystems.Add(ps);

        foreach (Transform child in transform)
            GetRecursively(ref particleSystems, child);
    }

    /// <summary>
    /// Requests despawn from the VFX manager.
    /// </summary>
    private void Despawn(string name)
    {
        g.VfxManager.Despawn(name);
    }
}
