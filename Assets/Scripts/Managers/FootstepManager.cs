using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class FootstepManager : MonoBehaviour
{
    //Fields
    private GameObject FootstepPrefab;
    ActorInstance actor;
    Vector3 previousPosition;
    bool isRightFoot = false;
    float threshold;

    public void Awake()
    {
        FootstepPrefab = PrefabRepo.Prefabs["FootstepPrefab"];
    }


    void Start()
    {
        threshold = g.TileSize / 4;
    }

    public void Play(ActorInstance actor)
    {
        if (!actor.isActive || !actor.isAlive)
            return;

        this.actor = actor;
        previousPosition = this.actor.position;
        StartCoroutine(CheckSpawn());
    }

    public void Stop()
    {
        actor = null;
        isRightFoot = false;
    }

    private IEnumerator CheckSpawn()
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


    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.Footstep).ToList().ForEach(x => Destroy(x));
    }


}
