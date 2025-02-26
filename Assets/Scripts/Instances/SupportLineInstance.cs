using System;
using System.Collections;
using UnityEngine;

public class SupportLineInstance : MonoBehaviour
{
    //External properties
    protected float tileSize => GameManager.instance.tileSize;
    protected BoardInstance board => GameManager.instance.board;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;

    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    //Fields
    public float alpha = 0;
    private float minAlpha = Opacity.Transparent;
    private float maxAlpha = Opacity.Percent50;
    private ActorPair actorPair;
    private Color color = ColorHelper.RGBA(48, 161, 49, 0);
    private LineRenderer lineRenderer;
    private int baseSortingOffset = -1; // Ensure it's always beneath actors

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    //Method which is automatically called before the first frame update  
    void Start()
    {
        lineRenderer.startWidth = tileSize / 2;
        lineRenderer.endWidth = tileSize / 2;
    }

    public void Spawn(ActorPair actorPair)
    {
        parent = board.transform;
        name = $"SupportLine_{Guid.NewGuid():N}";

        this.actorPair = actorPair;

        UpdateSortingOrder();
        lineRenderer.SetPosition(0, actorPair.startActor.position);
        lineRenderer.SetPosition(1, actorPair.endActor.position);

        // Subscribe to sorting order changes
        actorPair.actor1.OnSortingOrderChanged += UpdateSortingOrder;
        actorPair.actor2.OnSortingOrderChanged += UpdateSortingOrder;

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        //Before:
        alpha = Opacity.Transparent;
        color = new Color(color.r, color.g, color.b, alpha);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        //During:
        while (alpha < maxAlpha)
        {
            alpha += Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            color = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.startColor = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.endColor = new Color(color.r, color.g, color.b, alpha);

            yield return Wait.OneTick();
        }

        //After:
        alpha = maxAlpha;
        color = new Color(color.r, color.g, color.b, alpha);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void TriggerDespawn()
    {
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        //Before:
        alpha = maxAlpha;
        color = new Color(color.r, color.g, color.b, alpha);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        //During:
        while (alpha > minAlpha)
        {
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            color = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            yield return Wait.OneTick();
        }

        //After:
        alpha = minAlpha;
        color = new Color(color.r, color.g, color.b, alpha);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        Debug.Log("Fade-out complete, destroying support line");

        supportLineManager.Destroy(actorPair);
    }


    public void UpdateSortingOrder()
    {
        if (this == null || actorPair == null || lineRenderer == null) return;

        int lowestSortingOrder = Mathf.Min(actorPair.actor1.sortingOrder, actorPair.actor2.sortingOrder);
        lineRenderer.sortingOrder = lowestSortingOrder + baseSortingOffset;
    }



    public void Destroy()
    {
        if (actorPair != null)
        {
            actorPair.actor1.OnSortingOrderChanged -= UpdateSortingOrder;
            actorPair.actor2.OnSortingOrderChanged -= UpdateSortingOrder;
        }
        Destroy(this.gameObject);
    }



}
