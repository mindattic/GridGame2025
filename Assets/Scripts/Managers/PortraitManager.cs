using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    //External properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardInstance board => GameManager.instance.board;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;


    //Fields
    [SerializeField] public GameObject portraitPrefab;
    public ActorInstance actor;
    public int sortingOrder;

    public void SlideIn(ActorInstance actor, Direction direction)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.sortingOrder = sortingOrder--;
        instance.sprite = resourceManager.Portrait(actor.character.ToString()).Value.ToSprite();
        instance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.actor = actor;
        instance.direction = direction;
        instance.startTime = Time.time;

        StartCoroutine(instance.SlideIn());
    }

    public void TriggerDissolve()
    {
        var actor = players.Shuffle().FirstOrDefault();
        Dissolve(actor);
    }

    public void Dissolve(ActorInstance actor)
    {
        var prefab = Instantiate(portraitPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<PortraitInstance>();
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.sortingOrder = SortingOrder.Max;
        instance.sprite = resourceManager.Portrait(actor.character.ToString()).Value.ToSprite();
        instance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        instance.spriteRenderer.color = new Color(1, 1, 1, Opacity.Percent90);
        instance.position = actor.position;
        instance.startPosition = actor.position;
        instance.transform.localScale = new Vector3(0.25f, 0.25f, 1);

        StartCoroutine(instance.Dissolve());
    }


    public IEnumerator Play(CombatPair pair)
    {
        sortingOrder = SortingOrder.Max;

        yield return Wait.For(Intermission.Before.Player.Attack);

        audioManager.Play("Portrait");

        var (direction1, direction2) = GetDirection(pair);
        SlideIn(pair.actor1, direction1);
        SlideIn(pair.actor2, direction2);

        yield return Wait.For(Intermission.Before.Portrait.SlideIn);

        sortingOrder = SortingOrder.Max;
    }


    private (Direction, Direction) GetDirection(CombatPair pair)
    {
        var first = pair.axis == Axis.Vertical ? Direction.North : Direction.West;
        var second = pair.axis == Axis.Vertical ? Direction.South : Direction.East;
       
        return (pair.actor1 == pair.startActor ? first : second,
                pair.actor2 == pair.startActor ? first : second);
    }




}
