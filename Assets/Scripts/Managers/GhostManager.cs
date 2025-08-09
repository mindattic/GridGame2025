using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class GhostManager : MonoBehaviour
{


    //Fields
    private GameObject ghostPrefab;
    ActorInstance actor;
    float threshold;
    Vector3 previousPosition;

    public void Awake()
    {
        ghostPrefab = PrefabRepo.Prefabs["GhostPrefab"];
    }


    void Start() {
        threshold = g.TileSize / 12;
    }


    public void Play(ActorInstance actor)
    {
        this.actor = actor;
        previousPosition = this.actor.position;
        StartCoroutine(CheckSpawn());
    }

    public void Stop()
    {
        actor = null;
    }


    private IEnumerator CheckSpawn()
    {
        while (actor.isActive && actor.isAlive)
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

    private void Spawn()
    {
        var prefab = Instantiate(ghostPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<GhostInstance>();
        //trailInstance.settings = actor.settings;
        instance.name = $"Ghost_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        instance.Spawn(actor);
    }

    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.Ghost).ToList().ForEach(x => Destroy(x));
    }

}
