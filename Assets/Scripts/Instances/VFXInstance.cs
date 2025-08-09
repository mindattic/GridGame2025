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
    /// Fire-and-forget spawn of a VFX at a world position. Optionally runs a trigger routine afterward.
    /// </summary>
    public void Spawn(VFXAsset vfx, Vector3 position, IEnumerator trigger = null)
    {
        StartCoroutine(SpawnTrigger(vfx, position, trigger));
    }

    /// <summary>
    /// Yieldable spawn of a VFX at a world position. Plays optional trigger routine, then despawns.
    /// </summary>
    public IEnumerator SpawnTrigger(VFXAsset vfx, Vector3 worldPosition, IEnumerator trigger = null)
    {
        // 1) Place in world space
        transform.position = worldPosition + vfx.RelativeOffset;

        // 2) Apply rotation and scale
        transform.eulerAngles = vfx.AngularRotation;
        transform.localScale = g.TileScale.MultiplyBy(vfx.RelativeScale);

        // Configure looping on all particle systems in this hierarchy
        SetLooping(vfx.IsLoop);

        // Optional start delay
        if (vfx.Delay != 0f)
            yield return new WaitForSeconds(vfx.Delay);

        // Optional trigger routine
        if (trigger != null)
            yield return this.YieldRoutine(trigger);

        // Optional lifetime duration
        if (vfx.Duration != 0f)
            yield return new WaitForSeconds(vfx.Duration);

        // Despawn this instance
        Despawn(name);
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
