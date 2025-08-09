using Assets.Helper;
using Game.Behaviors.Actor;
using System;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class GhostManager : MonoBehaviour
{
    // Fields
    private GameObject ghostPrefab;
    ActorInstance actor;
    float threshold;
    Vector3 previousPosition;

    public void Awake()
    {
        ghostPrefab = PrefabRepo.Prefabs["GhostPrefab"];
    }

    private void Start()
    {
        threshold = g.TileSize / 12;
    }

    /// <summary>
    /// Starts spawning ghost trail effects for the given actor.
    /// </summary>
    public void Play(ActorInstance actor)
    {
        this.actor = actor;
        previousPosition = this.actor.position;
        StartCoroutine(CheckSpawn());
    }

    /// <summary>
    /// Stops ghost trail spawning.
    /// </summary>
    public void Stop()
    {
        actor = null;
    }

    /// <summary>
    /// Checks the actor's movement to determine when to spawn ghost effects.
    /// </summary>
    private IEnumerator CheckSpawn()
    {
        while (actor != null && actor.isActive && actor.isAlive)
        {
            var distance = Vector3.Distance(actor.position, previousPosition);
            if (distance >= threshold)
            {
                previousPosition = actor.position;
                Spawn();
            }

            yield return Wait.None();
        }
    }

    /// <summary>
    /// Spawns a ghost trail instance at the actor's current position.
    /// </summary>
    private void Spawn()
    {
        var prefab = Instantiate(ghostPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<GhostInstance>();
        instance.name = $"Ghost_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        instance.Spawn(actor);
    }

    /// <summary>
    /// Clears all ghost objects from the scene without using tags.
    /// </summary>
    public void Clear()
    {
        var instances = GameObject.FindObjectsByType<GhostInstance>(FindObjectsSortMode.None);
        foreach (var instance in instances)
        {
            Destroy(instance.gameObject);
        }
    }
}
