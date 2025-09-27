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
    /// - Non-looping (Duration <= 0): waits for particle completion (with timeout) then auto-despawns
    /// Looping with Duration <= 0: persists until manually despawned.
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

        // Optional chained routine
        if (routine != null)
            yield return StartCoroutine(routine);

        bool shouldDespawn = false;

        // Duration in seconds: finite lifetime -> wait and despawn
        if (vfx.Duration > 0f)
        {
            yield return new WaitForSeconds(vfx.Duration);
            shouldDespawn = true;
        }
        else if (!vfx.IsLoop)
        {
            // Non-looping with no explicit duration: wait until particles finish (with a safety timeout)
            var particleSystems = new List<ParticleSystem>();
            GetRecursively(ref particleSystems, transform);

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

            // Wait a frame to let PlayOnAwake systems start
            yield return null;

            while (timeout > 0f && anyAlive())
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            shouldDespawn = true;
        }

        // If this was already destroyed by a parent, stop quietly
        if (this == null || gameObject == null)
            yield break;

        if (shouldDespawn)
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
