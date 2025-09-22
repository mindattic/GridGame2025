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
    /// </summary>
    public IEnumerator SpawnPairRoutine(ActorPair actorPair)
    {
        yield return Wait.For(Intermission.Before.Player.Attack);
        g.AudioManager.Play("Click");

        var (d1, d2) = GetDirection(actorPair);

        // Compute lanes based on rules
        float? laneX = null;
        float? laneY = null;
        if (actorPair.axis == Axis.Vertical)
        {
            laneX = ComputeVerticalLaneX(actorPair);
        }
        else // Horizontal
        {
            laneY = ComputeHorizontalLaneY(actorPair);
        }

        // Run both SlideInRoutine coroutines in parallel with lanes
        yield return CoroutineHelper.WaitForAll(this,
            SlideInRoutine(actorPair.actor1, d1, laneX, laneY),
            SlideInRoutine(actorPair.actor2, d2, laneX, laneY)
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

    // ------------------------------------------------------------------
    // Positioning rules
    // ------------------------------------------------------------------

    // Vertical attacks: choose a side lane based on column: cols 1-3 => right, cols 4-6+ => left
    private float ComputeVerticalLaneX(ActorPair pair)
    {
        var container = g.PortraitsContainer;
        if (container == null) return 0f;

        float halfW = container.rect.width * 0.5f;
        int colA = Mathf.Max(1, pair.actor1.location.x);
        int colB = Mathf.Max(1, pair.actor2.location.x);
        float avgCol = (colA + colB) * 0.5f;

        // Right side positive X, left side negative X (relative to center)
        bool useRight = avgCol <= 3f; // columns 1-3 => right side; otherwise left

        // Keep inside safe margin so UI is visible
        float lane = halfW * 0.6f;
        return useRight ? +lane : -lane;
    }

    // Horizontal attacks: choose a top/bottom lane so it doesn't overlap the row containing the actors
    private float ComputeHorizontalLaneY(ActorPair pair)
    {
        var container = g.PortraitsContainer;
        if (container == null) return 0f;

        float halfH = container.rect.height * 0.5f;

        // World midpoint of the two actors
        Vector3 worldMid = (pair.actor1.Position + pair.actor2.Position) * 0.5f;

        // Convert to this container's local canvas space
        Vector2 local = Assets.Helpers.UnitConversionHelper.World.ToCanvas(container, worldMid);

        // If the row is in upper half (y >= 0), use a bottom lane; else use a top lane
        bool rowIsUpperHalf = local.y >= 0f;
        float lane = halfH * 0.6f;
        return rowIsUpperHalf ? -lane : +lane;
    }
}
