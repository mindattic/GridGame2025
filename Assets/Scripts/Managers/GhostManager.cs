using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    #region Properties
    protected float tileSize => GameManager.instance.tileSize;
    protected BoardInstance board => GameManager.instance.board;
    #endregion

    //Fields
    [SerializeField] public GameObject ghostPrefab;
    ActorInstance actor;
    float threshold;
    Vector3 previousPosition;

    //Method which is automatically called before the first frame update  
    void Start() {
        threshold = tileSize / 12;
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

            yield return Wait.UntilNextFrame();
        }
    }

    private void Spawn()
    {
        var prefab = Instantiate(ghostPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<GhostInstance>();
        //instance.thumbnailSettings = actor.thumbnailSettings;
        instance.name = $"Ghost_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.Spawn(actor);
    }

    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.Ghost).ToList().ForEach(x => Destroy(x));
    }

}
