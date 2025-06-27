using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PortraitManager : MonoBehaviour
{
    // Quick Reference Properties
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardInstance board => GameManager.instance.board;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;

    private List<PortraitInstance> portraits = new List<PortraitInstance>();

    // Fields
    private GameObject portraitPrefab;
    public ActorInstance actor;
    public int sortingOrder;

    public void Awake()
    {
        portraitPrefab = PrefabRepo.Prefabs["PortraitPrefab"];
    }

    public void TriggerSlideIn(ActorInstance actor, Direction direction)
    {
        StartCoroutine(SlideIn(actor, direction));
    }

    public void TriggerPopInOut(ActorInstance actor)
    {
        StartCoroutine(PopInOut(actor));
    }

    public IEnumerator SlideIn(ActorInstance actor, Direction direction)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.actor = actor;
        instance.direction = direction;
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;
        instance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.startTime = Time.time;

        portraits.Add(instance);
        yield return instance.SlideIn();
    }

    public IEnumerator PopInOut(ActorInstance actor, float scale = 0.1666f)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.actor = actor;
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        sortingManager.OnPortraitPopIn(instance);
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;
        instance.transform.localScale = new Vector3(scale, scale, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Transparent);
        instance.startTime = Time.time;

        portraits.Add(instance);
        yield return instance.PopInOut();
    }

    public IEnumerator PopIn(ActorInstance actor, float scale = 0.1666f)
    {
        // Remove and destroy any existing portrait for this actor
        var existing = portraits.FirstOrDefault(x => x != null && x.actor == actor);
        if (existing != null)
        {
            Destroy(existing.gameObject);
            portraits.Remove(existing);
        }

        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        sortingManager.OnPortraitPopIn(instance);
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;
        instance.transform.localScale = new Vector3(scale, scale, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Transparent);
        instance.actor = actor;
        instance.startTime = Time.time;

        portraits.Add(instance);
        yield return instance.PopIn();
    }

    public IEnumerator PopOut(ActorInstance actor)
    {
        var instance = portraits.FirstOrDefault(x => x != null && x.actor == actor);
        if (instance != null)
        {
            yield return instance.PopOut();
            //portraits.Remove(instance);
            //Destroy(instance.gameObject);
        }
        else
        {
            yield break;
        }
    }

    public void Dissolve(ActorInstance actor)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.actor = actor;
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;
        instance.transform.localScale = new Vector3(0.25f, 0.25f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.position = actor.position;
        instance.startPosition = actor.position;

        portraits.Add(instance);
        StartCoroutine(instance.Dissolve());
    }

    public IEnumerator SpawnPair(ActorPair actorPair)
    {
        yield return Wait.For(Intermission.Before.Player.Attack);
        audioManager.Play("Click");

        var (direction1, direction2) = GetDirection(actorPair);

        // Start both slide animations concurrently and wait for both to finish.
        yield return CoroutineHelper.WaitForAll(this,
            SlideIn(actorPair.actor1, direction1),
            SlideIn(actorPair.actor2, direction2)
        );

        yield return Wait.For(Intermission.Before.Portrait.SlideIn);
    }

    private (Direction, Direction) GetDirection(ActorPair actorPair)
    {
        var first = actorPair.axis == Axis.Vertical ? Direction.North : Direction.West;
        var second = actorPair.axis == Axis.Vertical ? Direction.South : Direction.East;

        return (actorPair.actor1 == actorPair.startActor ? first : second,
                actorPair.actor2 == actorPair.startActor ? first : second);
    }

    public void Despawn(PortraitInstance portrait)
    {
        if (portrait != null && portraits.Contains(portrait))
        {
            portraits.Remove(portrait);
            Destroy(portrait.gameObject);
        }
    }









}

