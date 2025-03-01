using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    //Quick Reference Properties
    protected float tileSize => GameManager.instance.tileSize;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected BoardInstance board => GameManager.instance.board;

    //Fields
    [SerializeField] public GameObject FootstepPrefab;
    ActorInstance actor;
    Vector3 previousPosition;
    bool isRightFoot = false;
    float threshold;

    //Method which is automatically called before the first frame update  
    void Start()
    {
        threshold = tileSize / 4;
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

            yield return Wait.UntilNextFrame();
        }
    }

    private void Spawn()
    {
        GameObject prefab = Instantiate(FootstepPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<FootstepInstance>();
        instance.sprite = resourceManager.Sprite("Footstep").Value;
        instance.name = $"Footstep_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.Spawn(actor.position, RotationHelper.ByDirection(actor.position, previousPosition), isRightFoot);
        previousPosition = actor.position;
        isRightFoot = !isRightFoot;
    }


    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.Footstep).ToList().ForEach(x => Destroy(x));
    }


}
