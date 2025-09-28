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
    /// Optional fixed lanes can be supplied to constrain cross-axis positioning.
    /// </summary>
    private IEnumerator SlideInRoutine(ActorInstance actor, Direction direction, float? fixedX = null, float? fixedY = null)
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

        // Apply optional fixed lanes
        instance.fixedX = fixedX;
        instance.fixedY = fixedY;

        portraits.Add(instance);
        yield return instance.SlideInRoutine();
    }

    /// <summary>
    /// Spawns a pair of portraitsContainer sliding in from opposite sides.
    /// Vertical attacks: lanes on left/right. Horizontal attacks: lanes on top/bottom.
    /// </summary>
    public IEnumerator SpawnPairRoutine(ActorPair actorPair)
    {
        yield return Wait.For(Intermission.Before.Player.Attack);
        g.AudioManager.Play("Click");

        var (d1, d2) = GetDirection(actorPair);

        // Compute lanes based on axis and assign per-actor lanes
        if (actorPair.axis == Axis.Vertical)
        {
            var (leftX, rightX) = ComputeVerticalLaneXs(actorPair);

            // Decide which actor goes left/right based on board column (x)
            bool a1Left = actorPair.actor1.location.x <= actorPair.actor2.location.x;
            float a1X = a1Left ? leftX : rightX;
            float a2X = a1Left ? rightX : leftX;

            // Run both SlideInRoutine coroutines in parallel with lanes
            yield return CoroutineHelper.WaitForAll(this,
                SlideInRoutine(actorPair.actor1, d1, fixedX: a1X, fixedY: null),
                SlideInRoutine(actorPair.actor2, d2, fixedX: a2X, fixedY: null)
            );
        }
        else // Horizontal
        {
            var (bottomY, topY) = ComputeHorizontalLaneYs(actorPair);

            // Decide which actor goes bottom/top based on board row (y)
            bool a1Bottom = actorPair.actor1.location.y <= actorPair.actor2.location.y;
            float a1Y = a1Bottom ? bottomY : topY;
            float a2Y = a1Bottom ? topY : bottomY;

            // Run both SlideInRoutine coroutines in parallel with lanes
            yield return CoroutineHelper.WaitForAll(this,
                SlideInRoutine(actorPair.actor1, d1, fixedX: null, fixedY: a1Y),
                SlideInRoutine(actorPair.actor2, d2, fixedX: null, fixedY: a2Y)
            );
        }

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

    // ------------------------------------------------------------------
    // Positioning rules
    // ------------------------------------------------------------------

    // Vertical attacks: choose 25% and 75% lanes across the canvas width (canvas-local X offsets)
    private (float leftX, float rightX) ComputeVerticalLaneXs(ActorPair pair)
    {
        var parentRect = g.PortraitsContainer as RectTransform;
        float width = parentRect != null ? parentRect.rect.width : 1920f; // Fallback width

        // 25% and 75% of the full width => centered coordinates are -0.25w and +0.25w
        float lane = width * 0.25f;
        return (-lane, lane);
    }

    // Horizontal attacks: choose 25% and 75% lanes across the canvas height (canvas-local Y offsets)
    private (float bottomY, float topY) ComputeHorizontalLaneYs(ActorPair pair)
    {
        var parentRect = g.PortraitsContainer as RectTransform;
        float height = parentRect != null ? parentRect.rect.height : 1080f; // Fallback height

        // 25% and 75% of the full height => centered coordinates are -0.25h and +0.25h
        float lane = height * 0.25f;
        return (-lane, lane);
    }
}
