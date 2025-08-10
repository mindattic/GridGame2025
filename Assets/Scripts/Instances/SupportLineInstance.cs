using Assets.Helper;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Draws a support line between two ActorInstances,
/// handles fade in/out, and initializes renderer settings.
/// </summary>
public class SupportLineInstance : MonoBehaviour
{
    /// <summary>
    /// Quick reference to the parent transform.
    /// Setting this moves the GameObject under the board in the hierarchy.
    /// </summary>
    public Transform parent
    {
        get => transform.parent;
        set => transform.SetParent(value, true);
    }

    /// <summary>
    /// Current alpha transparency of the line.
    /// </summary>
    public float alpha = 0f;

    /// <summary>
    /// Duration for fade in and fade out.
    /// </summary>
    [SerializeField]
    private float fadeDuration = 0.1f;

    /// <summary>
    /// Minimum alpha value (fully transparent).
    /// </summary>
    private float minAlpha = Opacity.Transparent;

    /// <summary>
    /// Maximum alpha value (semi transparent).
    /// </summary>
    private float maxAlpha = Opacity.Percent50;

    /// <summary>
    /// First actor endpoint for the line.
    /// </summary>
    public ActorInstance supporter;

    /// <summary>
    /// Second actor endpoint for the line.
    /// </summary>
    public ActorInstance attacker;

    /// <summary>
    /// Base color of the line (green) with adjustable alpha.
    /// </summary>
    private Color color = ColorHelper.RGBA(48, 161, 49, 0);

    /// <summary>
    /// LineRenderer used to draw the line.
    /// </summary>
    private LineRenderer lineRenderer;

    /// <summary>
    /// If true, FadeOutTrigger is skipped.
    /// </summary>
    public bool isStatic = false;

    /// <summary>
    /// SortingGroup accessor.
    /// </summary>
    public SortingGroup sortingGroup
    {
        get => GetComponent<SortingGroup>();
    }

    /// <summary>
    /// Cache component and configure initial renderer properties.
    /// </summary>
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Set line width relative to tile size
        lineRenderer.startWidth = g.TileSize * 0.25f;
        lineRenderer.endWidth = g.TileSize * 0.25f;

        // Ensure alignment faces camera
        lineRenderer.alignment = LineAlignment.View;

        lineRenderer.positionCount = 2;
    }

    /// <summary>
    /// Configure sorting layer and order.
    /// </summary>
    public void SetSorting(string sortingLayer, int sortingOrder = 0)
    {
        sortingGroup.sortingLayerID = SortingLayer.NameToID(sortingLayer);
        sortingGroup.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Initializes the support line between two actors and starts fade in.
    /// </summary>
    public void Spawn(ActorInstance supporter, ActorInstance attacker)
    {
        this.supporter = supporter;
        this.attacker = attacker;

        // Parent under the board for organization
        parent = g.Board.transform;

        // Unique name for debugging
        name = $"SupportLine_{Guid.NewGuid():N}";

        lineRenderer.SetPosition(0, supporter.position);
        lineRenderer.SetPosition(1, attacker.position);

        g.SortingManager.OnSupportLineSpawn(this);

        // Begin fade in effect
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fades from transparent to maxAlpha.
    /// </summary>
    private IEnumerator FadeIn()
    {
        yield return FadeTrigger(minAlpha, maxAlpha, null);
    }

    /// <summary>
    /// Triggers fade out and eventual destruction of the support line.
    /// </summary>
    public void Despawn()
    {
        StartCoroutine(FadeOutTrigger());
    }

    /// <summary>
    /// Fades from maxAlpha to transparent, then informs the manager to destroy.
    /// </summary>
    public IEnumerator FadeOutTrigger()
    {
        if (isStatic)
            yield break;

        yield return FadeTrigger(maxAlpha, minAlpha, () =>
        {
            g.SupportLineManager.Destroy(supporter, attacker);
        });
    }

    /// <summary>
    /// Applies new alpha to both ends of the LineRenderer color.
    /// </summary>
    private void UpdateLineAlpha(float a)
    {
        color.a = a;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    /// <summary>
    /// Destroys this GameObject when requested.
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Consolidated fade routine used by FadeIn and FadeOutTrigger.
    /// Interpolates alpha from startAlpha to targetAlpha over fadeDuration.
    /// Calls onComplete after finishing if provided.
    /// </summary>
    private IEnumerator FadeTrigger(float startAlpha, float targetAlpha, Action onComplete)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            UpdateLineAlpha(alpha);
            yield return Wait.None();
        }

        alpha = targetAlpha;
        UpdateLineAlpha(alpha);

        onComplete?.Invoke();
    }
}
