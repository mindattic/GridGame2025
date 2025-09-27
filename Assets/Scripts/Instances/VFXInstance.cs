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
    /// Fire-and-forget spawn of a VFX at a world position. Optionally runs a routine afterward.
    /// </summary>
    public void Spawn(VFXAsset vfx, Vector3 position, IEnumerator routine = null)
    {
        StartCoroutine(SpawnRoutine(vfx, position, routine));
    }

    /// <summary>
    /// Yield until this VFX reaches its Apex moment (includes Delay).
    /// Uses Unity seconds.
    /// </summary>
    public IEnumerator WaitUntilTrigger(VFXAsset vfx)
    {
        float wait = Mathf.Max(0f, (vfx?.Apex ?? 1f));
        if (wait <= 0f)
            yield break;

        float t = 0f;
        while (t < wait)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Yieldable spawn of a VFX at a world position.
    /// Respects:
    /// - Delay: waits before playing
    /// - Duration: finite -> waits then auto-despawns
    /// - Non-looping with no Duration: waits for particle completion (with timeout)
    /// - Looping with Duration <= 0: persists until manually despawned.
    /// </summary>
    public IEnumerator SpawnRoutine(VFXAsset vfx, Vector3 position, IEnumerator routine = null)
    {
        if (vfx == null) yield break;

        // 1) Place
        transform.position = position + vfx.RelativeOffset;

        // 2) Apply rotation
        transform.eulerAngles = vfx.AngularRotation;

        // 3) Apply scale in world space relative to tile size, independent of parent scale
        ApplyWorldScale(g.TileScale, vfx.RelativeScale);

        // Configure looping and scaling behavior for particle systems
        SetLooping(vfx.IsLoop);

        // Cache the name now, before any possible destroy by parent
        string instanceName = name;

        // Optional chained routine
        if (routine != null)
            yield return StartCoroutine(routine);

        bool shouldAutoDespawn = false;

        // Lifetime handling
        if (vfx.IsLoop)
        {
            // Looping: if a finite duration is provided, respect it, else persist
            if (vfx.Duration > 0f)
            {
                yield return new WaitForSeconds(vfx.Duration);
                shouldAutoDespawn = true;
            }
            else
            {
                // Infinite loop: do not auto-despawn
                shouldAutoDespawn = false;
            }
        }
        else
        {
            // Non-looping: if a finite duration is provided, respect it; otherwise wait for particles to finish
            if (vfx.Duration > 0f)
            {
                yield return new WaitForSeconds(vfx.Duration);
                shouldAutoDespawn = true;
            }
            else
            {
                var particleSystems = new List<ParticleSystem>();
                GetRecursively(ref particleSystems, transform);

                // Wait a frame to let PlayOnAwake systems start
                yield return null;

                float timeout = 5f; // safety cap
                bool anyAlive()
                {
                    foreach (var ps in particleSystems)
                    {
                        if (ps != null && ps.IsAlive(true))
                            return true;
                    }
                    return false;
                }

                while (timeout > 0f && anyAlive())
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }

                shouldAutoDespawn = true;
            }
        }

        // If this was already destroyed by a parent, stop quietly
        if (this == null || gameObject == null)
            yield break;

        if (shouldAutoDespawn)
            Despawn(instanceName);
    }

    /// <summary>
    /// Compute a world-space scale from tile and relative scales and apply as localScale compensating for parent lossyScale.
    /// </summary>
    private void ApplyWorldScale(Vector3 tileScale, Vector3 relativeScale)
    {
        Vector3 desiredWorld = new Vector3(
            Mathf.Max(1e-4f, tileScale.x * relativeScale.x),
            Mathf.Max(1e-4f, tileScale.y * relativeScale.y),
            Mathf.Max(1e-4f, (relativeScale.z == 0f ? 1f : tileScale.z * relativeScale.z))
        );

        Vector3 parentLossy = Vector3.one;
        if (transform.parent != null)
            parentLossy = transform.parent.lossyScale;

        // Avoid division by zero
        float ix = parentLossy.x != 0f ? 1f / parentLossy.x : 1f;
        float iy = parentLossy.y != 0f ? 1f / parentLossy.y : 1f;
        float iz = parentLossy.z != 0f ? 1f / parentLossy.z : 1f;

        transform.localScale = new Vector3(desiredWorld.x * ix, desiredWorld.y * iy, desiredWorld.z * iz);
    }

    /// <summary>
    /// Sets the loop flag on all ParticleSystem components in the transform hierarchy
    /// and enforces Hierarchy scaling so transform scale affects particle size.
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
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
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
