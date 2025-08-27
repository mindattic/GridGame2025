using Assets.Scripts.Models;
using Assets.Scripts.Sequences;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Spawns and controls projectiles in a grid friendly way using coroutines.
/// A single looping TrailEffect travels from start to target using a MotionStyle,
/// then a single VFX plays on impact, then an optional routine is yielded.
/// </summary>
public class ProjectileManager : MonoBehaviour
{
    /// <summary>
    /// Queue a healing projectile sequence. Wiggle motion and heal VFX on impact.
    /// </summary>
    public void EnqueueHeal(Vector3 startPosition, ActorInstance target)
    {
        if (target == null) return;

        var heal = new ProjectileSettings
        {
            friendlyName = "Heal",
            startPosition = startPosition,
            target = target,

            trailKey = "GreenSparkle",
            vfxKey = "BuffLife",
            routine = target.HealRoutine(10),

            motionStyle = MotionStyle.Wiggle,
            travelSeconds = 10.9f,
            wiggleAmplitudeTiles = 3.35f,
            wiggleHz = 3.5f,
            arriveRadiusTiles = 0.1f
        };

        g.SequenceManager.Add(new FireProjectileSequence(heal));
    }

    /// <summary>
    /// Queue a fireball projectile sequence. Lobbed arc and explosion VFX on impact.
    /// </summary>
    public void EnqueueFireball(Vector3 startPosition, ActorInstance target)
    {
        if (target == null) return;

        var fireball = new ProjectileSettings
        {
            friendlyName = "Fireball",
            startPosition = startPosition,
            target = target,

            trailKey = "Fireball",
            vfxKey = "PuffyExplosion",
            routine = target.FireDamageRoutine(10),

            motionStyle = MotionStyle.LobbedArc,
            travelSeconds = 0.8f,
            lobbedHeightTiles = 0.9f,
            arriveRadiusTiles = 0.1f
        };

        g.SequenceManager.Add(new FireProjectileSequence(fireball));
    }


    /// <summary>
    /// Queue a healing projectile that homes in using a tightening spiral and plays heal VFX on impact.
    /// </summary>
    public void EnqueueHomingSpiral(Vector3 startPosition, ActorInstance target)
    {
        if (target == null) return;

        var heal = new ProjectileSettings
        {
            friendlyName = "Heal",
            startPosition = startPosition,
            target = target,

            // Visuals
            trailKey = "GoldSparkle",
            vfxKey = "BuffLife",
            routine = target.HealRoutine(10),

            // Motion
            motionStyle = MotionStyle.HomingSpiral,
            travelSeconds = 1.0f,
            arriveRadiusTiles = 0.1f,

            // Spiral specific
            spiralTurns = 3,
            spiralStartRadiusTiles = 0.9f,

            faceDirection = false
        };

        g.SequenceManager.Add(new FireProjectileSequence(heal));
    }



    /// <summary>
    /// Backward compatible despawn by instance name. Delegates to TrailManager and VfxManager.
    /// </summary>
    public void Despawn(string instanceName)
    {
        if (string.IsNullOrEmpty(instanceName)) return;

        if (g.VfxManager != null)
            g.VfxManager.Despawn(instanceName);
    }

    /// <summary>
    /// Fire and forget spawn.
    /// </summary>
    public void Spawn(ProjectileSettings s)
    {
        StartCoroutine(SpawnRoutine(s));
    }

    /// <summary>
    /// Creates the node, parents to board, attaches one trail, travels, plays impact, yields routine, cleans up.
    /// </summary>
    public IEnumerator SpawnRoutine(ProjectileSettings s)
    {
        if (s == null) yield break;

        // Resolve destination
        Transform targetTf = s.target != null ? s.target.transform : s.followTarget;
        Vector3 end = targetTf != null ? targetTf.position : s.staticTargetPosition;

        Vector3 start = s.startPosition;

        // Short circuit if already there
        if ((end - start).sqrMagnitude < 1e-8f)
        {
            SpawnImpact(s.vfxKey, start);
            if (s.routine != null)
                yield return s.routine;
            yield break;
        }

        // Node
        var root = GameObject.Find("Effects");
        var nodeGo = new GameObject("ProjectileNode");
        nodeGo.transform.position = start;
        nodeGo.transform.SetParent(root.transform, true); // no parent, no inherited scale


        var node = new ProjectileNode(nodeGo.transform, s);

        // One trail
        AttachTrail(node, s.trailKey);

        // Travel
        yield return StartCoroutine(node.TravelRoutine());

        // Impact
        Vector3 finalPos = node.position;
        SpawnImpact(s.vfxKey, finalPos);

        // Post impact
        if (s.routine != null)
            yield return s.routine;

        // Cleanup
        node.Cleanup();
    }

    /// <summary>
    /// Spawns a single trail and parents it to the node so it follows.
    /// </summary>
    private void AttachTrail(ProjectileNode node, string trailKey)
    {
        if (string.IsNullOrEmpty(trailKey)) return;


        var trailAsset = VfxLibrary.Get(trailKey);
        node.AttachTrail(trailAsset);
    }

    /// <summary>
    /// Spawns an impact VFX via VfxManager and VfxLibrary at a world position.
    /// </summary>
    private void SpawnImpact(string vfxKey, Vector3 position)
    {
        if (string.IsNullOrEmpty(vfxKey) || g.VfxManager == null) return;

        var vfx = VfxLibrary.Get(vfxKey);
        if (vfx == null || vfx.Prefab == null)
        {
            Debug.LogError($"ProjectileManager: VFX `{vfxKey}` not found or prefab is null.");
            return;
        }

        g.VfxManager.Spawn(vfx, position);
    }
}
