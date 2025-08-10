using Assets.Helper;
using Game.Behaviors.Actor;
using System;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class FootstepManager : MonoBehaviour
{
    // Fields
    private GameObject FootstepPrefab;
    ActorInstance actor;
    Vector3 previousPosition;
    bool isRightFoot = false;
    float threshold;

    public void Awake()
    {
        FootstepPrefab = PrefabRepo.Prefabs["FootstepPrefab"];
    }

    private void Start()
    {
        threshold = g.TileSize / 4;
    }

    /// <summary>
    /// Starts playing footstep effects for a given actor.
    /// </summary>
    public void Play(ActorInstance actor)
    {
        if (!actor.isActive || !actor.isAlive)
            return;

        this.actor = actor;
        previousPosition = this.actor.position;
        StartCoroutine(CheckSpawnRoutine());
    }

    /// <summary>
    /// Stops playing footstep effects.
    /// </summary>
    public void Stop()
    {
        actor = null;
        isRightFoot = false;
    }

    /// <summary>
    /// Checks the distance traveled by the actor to decide when to spawn footsteps.
    /// </summary>
    private IEnumerator CheckSpawnRoutine()
    {
        while (actor != null && actor.isActive && actor.isAlive)
        {
            var distance = Vector3.Distance(actor.position, previousPosition);
            if (distance >= threshold)
            {
                Spawn();
            }

            yield return Wait.None();
        }
    }

    /// <summary>
    /// Spawns a single footstep instance at the actor's position.
    /// </summary>
    private void Spawn()
    {
        GameObject prefab = Instantiate(FootstepPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<FootstepInstance>();
        instance.sprite = SpriteRepo.Sprites["FootstepManager"];
        instance.name = $"Footstep_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        instance.Spawn(actor.position, RotationHelper.ByDirection(actor.position, previousPosition), isRightFoot);
        previousPosition = actor.position;
        isRightFoot = !isRightFoot;
    }

    /// <summary>
    /// Clears all footstep objects from the scene without using tags.
    /// </summary>
    public void Clear()
    {
        var instances = GameObject.FindObjectsByType<FootstepInstance>(FindObjectsSortMode.None);
        foreach (var instance in instances)
        {
            Destroy(instance.gameObject);
        }
    }
}
