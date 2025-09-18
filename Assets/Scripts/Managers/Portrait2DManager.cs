using Assets.Helper;
using Assets.Scripts.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Manages 2D portrait instances using UI Images instead of SpriteRenderers.
/// </summary>
public class Portrait2DManager : MonoBehaviour
{
    // Active portrait instances
    private List<Portrait2DInstance> portraits = new List<Portrait2DInstance>();

    // Prefab containing a Canvas+RectTransform+Image+PortraitInstance
    private GameObject portraitPrefab;

    // For actor lookups
    public ActorInstance actor;

    // Default sorting order for new portraitsContainer
    public int sortingOrder;

    private void Awake()
    {
        // Load the UI-based portrait go
        portraitPrefab = PrefabLibrary.Prefabs["Portrait2DPrefab"];
    }

    /// <summary>
    /// Slide a portrait in for the given actor from the given direction.
    /// </summary>
    public void SlideIn(ActorInstance actor, Direction direction)
    {
        StartCoroutine(SlideInRoutine(actor, direction));
    }

    /// <summary>
    /// Instantiates and slides the UI portrait into view.
    /// </summary>
    private IEnumerator SlideInRoutine(ActorInstance actor, Direction direction)
    {
        var go = Instantiate(portraitPrefab, Vector3.zero, Quaternion.identity);
        var instance = go.GetComponent<Portrait2DInstance>();
        instance.actor = actor;
        instance.direction = direction;
        instance.name = $"Portrait_{Guid.NewGuid():N}";
        instance.parent = g.PortraitsContainer;
        instance.sprite = ActorLibrary.Actors[actor.characterName].Portrait;
        instance.scale = new Vector3(1f, 1f, 1f);
        instance.image.color = new Color(1f, 1f, 1f, 1);

        portraits.Add(instance);
        yield return instance.SlideInRoutine();
    }

    /// <summary>
    /// Spawns a pair of portraitsContainer sliding in from opposite sides.
    /// </summary>
    public IEnumerator SpawnPairRoutine(ActorPair actorPair)
    {
        yield return Wait.For(Intermission.Before.Player.Attack);
        g.AudioManager.Play("Click");

        var (d1, d2) = GetDirection(actorPair);

        // Run both SlideInRoutine coroutines in parallel
        yield return CoroutineHelper.WaitForAll(this,
            SlideInRoutine(actorPair.actor1, d1),
            SlideInRoutine(actorPair.actor2, d2)
        );

        yield return Wait.For(Intermission.Before.Portrait.SlideIn);
    }

    /// <summary>
    /// Determines slide directions for a pair based on axis.
    /// </summary>
    private (Direction, Direction) GetDirection(ActorPair pair)
    {
        var first = pair.axis == Axis.Vertical ? Direction.North : Direction.West;
        var second = pair.axis == Axis.Vertical ? Direction.South : Direction.East;

        return (pair.actor1 == pair.startActor ? first : second,
                pair.actor2 == pair.startActor ? first : second);
    }

    /// <summary>
    /// Removes and destroys a portrait instance.
    /// </summary>
    public void Despawn(Portrait2DInstance portrait)
    {
        if (portrait != null && portraits.Contains(portrait))
        {
            portraits.Remove(portrait);
            Destroy(portrait.gameObject);
        }
    }
}
