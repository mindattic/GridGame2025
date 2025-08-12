using Assets.Helper;
using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class ProjectileManager : MonoBehaviour
{
    //Fields
    private GameObject projectilePrefab;
    public Dictionary<string, ProjectileInstance> projectiles = new Dictionary<string, ProjectileInstance>();

    public void Awake()
    {
        projectilePrefab = PrefabLibrary.Prefabs["ProjectilePrefab"];
    }

    // Spawns a ProjectileInstance configured by type.
    public IEnumerator SpawnRoutine(ProjectileSettings projectile)
    {
        // Instantiate the ProjectileInstance go at the startPosition's position.
        var go = Instantiate(projectilePrefab, projectile.startPosition, Quaternion.identity);
        var instance = go.GetComponent<ProjectileInstance>();
        instance.name = $"Projectile_{projectile.friendlyName}_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        projectiles.Add(instance.name, instance);
        yield return instance.SpawnRoutine(projectile);
    }

    public void Despawn(string name)
    {
        if (projectiles.TryGetValue(name, out var instance))
        {
            Destroy(instance.gameObject);
            projectiles.Remove(name);
        }
    }

    public void EnqueueHeal(Vector3 startPosition, ActorInstance target)
    {
        var heal = new ProjectileSettings()
        {
            friendlyName = "Heal",
            startPosition = startPosition,
            target = target,
            path = ProjectilePath.BezierCurve,
            controlPoints = BezierCurveHelper.Gentle(startPosition, target),
            trailKey = "GreenSparkle",
            vfxKey = "BuffLife",
            routine = target.HealRoutine(10)
        };

        //g.SequenceManager.Add(new PortraitPopInSequence(startPosition));
        g.SequenceManager.Add(new FireProjectileSequence(heal));
        //g.SequenceManager.Add(new PortraitPopOutSequence(startPosition));

        //if (castBeforeAttack)
        //    g.SequenceManager.AddFirst(e);
        //else
        //    g.SequenceManager.Add(e);
    }


    public void EnqueueFireball(Vector3 startPosition, ActorInstance target)
    {
        var fireball = new ProjectileSettings()
        {
            friendlyName = "Fireball",
            startPosition = startPosition,
            target = target,
            path = ProjectilePath.BezierCurve,
            controlPoints = BezierCurveHelper.Overshooting(startPosition, target),
            trailKey = "Fireball",
            vfxKey = "PuffyExplosion",
            routine = target.FireDamageRoutine(10)
        };

        //g.SequenceManager.Add(new PortraitPopInSequence(startPosition));
        g.SequenceManager.Add(new FireProjectileSequence(fireball));
        //g.SequenceManager.Add(new PortraitPopOutSequence(startPosition));

        //if (castBeforeAttack)
        //    g.SequenceManager.AddFirst(Animation);
        //else
        //    g.SequenceManager.Add(Animation);
    }

}