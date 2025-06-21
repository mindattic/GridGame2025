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
    protected EventManager eventManager => GameManager.instance.eventManager;

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

    public void EnqueueHeal(ActorInstance source, ActorInstance target, bool castBeforeAttack = true)
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
            trigger = new Trigger(target.Heal(10), isAsync: false)
        };

        var e = new FireProjectileEvent(heal);

        if (castBeforeAttack)
            eventManager.Insert(e);
        else
            eventManager.Add(e);
    }


    public void EnqueueFireball(ActorInstance source, ActorInstance target, bool castBeforeAttack = true)
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
            trigger = new Trigger(target.FireDamage(10), isAsync: false)
        };

        var action = new FireProjectileEvent(fireball);

        if (castBeforeAttack)
            eventManager.Insert(action);
        else
            eventManager.Add(action);
    }

}