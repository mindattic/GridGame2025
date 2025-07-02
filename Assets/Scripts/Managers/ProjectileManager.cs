using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    //Quick Reference
    protected BoardInstance board => GameManager.instance.board;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;

    //Fields

    private GameObject projectilePrefab;

    Dictionary<string, ProjectileInstance> projectiles = new Dictionary<string, ProjectileInstance>();



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
        instance.parent = board.transform;
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

        sequenceManager.Add(new PortraitPopInSequence(source));
        sequenceManager.Add(new FireProjectileSequence(heal));
        sequenceManager.Add(new PortraitPopOutSequence(source));

        //if (castBeforeAttack)
        //    sequenceManager.AddFirst(e);
        //else
        //    sequenceManager.Add(e);
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

        sequenceManager.Add(new PortraitPopInSequence(source));
        sequenceManager.Add(new FireProjectileSequence(fireball));
        sequenceManager.Add(new PortraitPopOutSequence(source));

        //if (castBeforeAttack)
        //    sequenceManager.AddFirst(action);
        //else
        //    sequenceManager.Add(action);
    }

}