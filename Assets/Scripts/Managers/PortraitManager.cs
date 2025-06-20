using Assets.Scripts.Models;
using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Intermission.Before;

public class PortraitManager : MonoBehaviour
{
    //Quick Reference Properties
    
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardInstance board => GameManager.instance.board;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
    protected SortingManager sortingManager => GameManager.instance.sortingManager;



    //Fields
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

    public void TriggerPopIn(ActorInstance actor, Direction direction)
    {
        StartCoroutine(PopIn(actor, direction));
    }

    public IEnumerator SlideIn(ActorInstance actor, Direction direction)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.sortingOrder = sortingOrder--;
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;

        instance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.actor = actor;
        instance.direction = direction;
        instance.startTime = Time.time;

        yield return instance.SlideIn();
    }


    public IEnumerator PopIn(ActorInstance actor, Direction direction)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.sortingOrder = sortingOrder--;
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;

        instance.transform.localScale = new Vector3(0.1666f, 0.1666f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.actor = actor;
        instance.direction = direction;
        instance.startTime = Time.time;

        yield return instance.PopIn();
    }

    public void Dissolve(ActorInstance actor)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        //instance.sortingOrder = GameManager.instance.sortingManager.Max;
        instance.sprite = ActorRepo.Actors[actor.characterName].Portrait;

        instance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.position = actor.position;
        instance.startPosition = actor.position;
        instance.transform.localScale = new Vector3(0.25f, 0.25f, 1);

        StartCoroutine(instance.Dissolve());
    }

    public IEnumerator Play(ActorPair actorPair)
    {
        //sortingOrder = SortingOrder.Max;
        yield return Wait.For(Intermission.Before.Player.Attack);

        //audioManager.Play("Portrait");
        audioManager.Play("Click");

        var (direction1, direction2) = GetDirection(actorPair);

        // Start both slide animations concurrently and wait for both to finish.
        yield return CoroutineHelper.WaitForAll(this,
            SlideIn(actorPair.actor1, direction1),
            SlideIn(actorPair.actor2, direction2)
        );

        yield return Wait.For(Intermission.Before.Portrait.SlideIn);
        //sortingOrder = SortingOrder.Max;
    }



    private (Direction, Direction) GetDirection(ActorPair actorPair)
    {
        var first = actorPair.axis == Axis.Vertical ? Direction.North : Direction.West;
        var second = actorPair.axis == Axis.Vertical ? Direction.South : Direction.East;
       
        return (actorPair.actor1 == actorPair.startActor ? first : second,
                actorPair.actor2 == actorPair.startActor ? first : second);
    }




}
