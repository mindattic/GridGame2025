using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Draws a curved support line (arc) between two ActorInstances,
/// handles fade in/out, and updates positions each physics step
/// </summary>
public class SupportLineInstance : MonoBehaviour
{
      /// <summary>
    /// Quick reference to the parent transform;
    /// setting this moves the GameObject under the board in the hierarchy
    /// </summary>
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    /// <summary>
    /// Current alpha transparency of the line
    /// </summary>
    public float alpha = 0f;

    /// <summary>
    /// Duration for fade in and fade out
    /// </summary>
    [SerializeField]
    private float fadeDuration = 0.1f;

    /// <summary>
    /// Minimum alpha value (fully transparent)
    /// </summary>
    private float minAlpha = Opacity.Transparent;

    /// <summary>
    /// Maximum alpha value (semi-transparent)
    /// </summary>
    private float maxAlpha = Opacity.Percent50;

    /// <summary>
    /// First actor endpoint for the arc
    /// </summary>
    public ActorInstance supporter;

    /// <summary>
    /// Second actor endpoint for the arc
    /// </summary>
    public ActorInstance attacker;

    /// <summary>
    /// Base color of the line (green) with adjustable alpha
    /// </summary>
    private UnityEngine.Color color = ColorHelper.RGBA(48, 161, 49, 0);

    /// <summary>
    /// Unity LineRenderer component used to draw the arc
    /// </summary>
    private LineRenderer lineRenderer;

    public bool isStatic = false;


    public SortingGroup sortingGroup
    {
        get => this.GetComponent<SortingGroup>();
    }






    /// <summary>
    /// Cache component and configure initial renderer properties
    /// </summary>
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // UpdateFill width of the line relative to tile size
        lineRenderer.startWidth = g.TileSize * 0.25f;
        lineRenderer.endWidth = g.TileSize * 0.25f;

        // Ensure alignment faces camera
        lineRenderer.alignment = LineAlignment.View;

        lineRenderer.positionCount = 2;
    }

    public void SetSorting(string sortingLayer, int sortingOrder = 0)
    {
        sortingGroup.sortingLayerID = SortingLayer.NameToID(sortingLayer);
        sortingGroup.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Initializes the support line between two actors and starts fade-in
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

        // Begin fade-in effect
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Updates line alpha over time from transparent to maxAlpha
    /// </summary>
    private IEnumerator FadeIn()
    {
        float startAlpha = minAlpha;
        float targetAlpha = maxAlpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            UpdateLineAlpha(alpha);
            yield return Wait.None();
        }

        alpha = maxAlpha;
        UpdateLineAlpha(alpha);
    }

    /// <summary>
    /// Triggers fade-out and eventual destruction of the support line
    /// </summary>
    public void TriggerDespawn()
    {
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Updates line alpha over time from maxAlpha to transparent, then destroys
    /// </summary>
    public IEnumerator FadeOut()
    {
        if (isStatic)
            yield break;

        float startAlpha = maxAlpha;
        float targetAlpha = minAlpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            UpdateLineAlpha(alpha);
            yield return Wait.None();
        }

        alpha = minAlpha;
        UpdateLineAlpha(alpha);

        // Inform manager to clean up references
        g.SupportLineManager.Destroy(supporter, attacker);
    }

    /// <summary>
    /// Applies new alpha to both ends of the LineRenderer's color
    /// </summary>
    private void UpdateLineAlpha(float a)
    {
        color.a = a;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    /// <summary>
    /// Destroys this GameObject when requested
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
