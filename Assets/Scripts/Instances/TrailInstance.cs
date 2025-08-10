using Assets.Helper;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class TrailInstance : MonoBehaviour
{
    // Convenience transform accessors
    public Transform parent
    {
        get => transform.parent;
        set => transform.SetParent(value, true);
    }

    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Quaternion rotation
    {
        get => transform.rotation;
        set => transform.rotation = value;
    }

    public Vector3 scale
    {
        get => transform.localScale;
        set => transform.localScale = value;
    }

    /// <summary>
    /// Spawns a trail effect at a world position, applies relative offsets, optional delay and duration,
    /// optionally runs a routine, then despawns.
    /// </summary>
    public IEnumerator SpawnRoutine(TrailEffectAsset trail, Vector3 worldPosition, IEnumerator routine = null)
    {
        position = worldPosition;
        transform.localPosition = trail.RelativeOffset;
        transform.localEulerAngles = trail.AngularRotation;
        transform.localScale = g.TileScale.MultiplyBy(trail.RelativeScale);

        SetLooping(trail.IsLoop);

        if (trail.Delay != 0f)
            yield return new WaitForSeconds(trail.Delay);

        if (routine != null)
            yield return StartCoroutine(routine);

        if (trail.Duration != 0f)
            yield return Wait.For(trail.Duration);

        Despawn(name);
    }

    /// <summary>
    /// Sets the loop flag on all ParticleSystem components in this hierarchy.
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
    /// Collects ParticleSystem components starting at the given transform and all children.
    /// </summary>
    private void GetRecursively(ref List<ParticleSystem> particleSystems, Transform t)
    {
        var ps = t.GetComponent<ParticleSystem>();
        if (ps != null)
            particleSystems.Add(ps);

        foreach (Transform child in t)
            GetRecursively(ref particleSystems, child);
    }

    /// <summary>
    /// Requests despawn for this trail by name.
    /// </summary>
    private void Despawn(string instanceName)
    {
        g.TrailManager.Despawn(instanceName);
    }
}
