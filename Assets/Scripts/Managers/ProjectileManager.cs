using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

public class ProjectileManager : MonoBehaviour
{
    //Fields
    private GameObject projectilePrefab;
    public Dictionary<string, ProjectileInstance> projectiles = new Dictionary<string, ProjectileInstance>();

    public void Awake()
    {
        projectilePrefab = PrefabRepo.Prefabs["ProjectilePrefab"];
    }

    // Spawns a ProjectileInstance configured by type.
    public IEnumerator Spawn(ProjectileSettings projectile)
    {
        // Instantiate the ProjectileInstance prefab at the source's position.
        var prefab = Instantiate(projectilePrefab, projectile.source.position, Quaternion.identity);
        var instance = prefab.GetComponent<ProjectileInstance>();
        instance.name = $"Projectile_{projectile.friendlyName}_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        projectiles.Add(instance.name, instance);
        yield return instance.Spawn(projectile);
    }

    public void Despawn(string name)
    {

        Destroy(projectiles[name].gameObject);
        projectiles.Remove(name);
    }

    public void EnqueueHeal(ActorInstance source, ActorInstance target)
    {
        var heal = new ProjectileSettings()
        {
            friendlyName = "Heal",
            source = source,
            target = target,
            path = ProjectilePath.BezierCurve,
            controlPoints = BezierCurveHelper.Gentle(source, target),
            trailKey = "GreenSparkle",
            vfxKey = "BuffLife",
            trigger = new TriggerEvent(target.Heal(10))
        };

        g.SequenceManager.Add(new PortraitPopInSequence(source));
        g.SequenceManager.Add(new FireProjectileSequence(heal));
        g.SequenceManager.Add(new PortraitPopOutSequence(source));

        //if (castBeforeAttack)
        //    g.SequenceManager.AddFirst(e);
        //else
        //    g.SequenceManager.Add(e);
    }


    public void EnqueueFireball(ActorInstance source, ActorInstance target)
    {
        var fireball = new ProjectileSettings()
        {
            friendlyName = "Fireball",
            source = source,
            target = target,
            path = ProjectilePath.BezierCurve,
            controlPoints = BezierCurveHelper.Overshooting(source, target),
            trailKey = "Fireball",
            vfxKey = "PuffyExplosion",
            trigger = new TriggerEvent(target.FireDamage(10))
        };

        g.SequenceManager.Add(new PortraitPopInSequence(source));
        g.SequenceManager.Add(new FireProjectileSequence(fireball));
        g.SequenceManager.Add(new PortraitPopOutSequence(source));

        //if (castBeforeAttack)
        //    g.SequenceManager.AddFirst(action);
        //else
        //    g.SequenceManager.Add(action);
    }

}