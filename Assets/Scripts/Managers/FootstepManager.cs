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
        FootstepPrefab = PrefabLibrary.Prefabs["FootstepPrefab"];
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
        if (!actor.IsActive || !actor.IsAlive)
            return;

        this.actor = actor;
        previousPosition = this.actor.Position;
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
        while (actor != null && actor.IsActive && actor.IsAlive)
        {
            var distance = Vector3.Distance(actor.Position, previousPosition);
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
        instance.sprite = SpriteLibrary.Sprites["FootstepManager"];
        instance.name = $"Footstep_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        instance.Spawn(actor.Position, RotationHelper.ByDirection(actor.Position, previousPosition), isRightFoot);
        previousPosition = actor.Position;
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
