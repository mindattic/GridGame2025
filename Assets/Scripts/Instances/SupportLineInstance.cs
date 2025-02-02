using System;
using System.Collections;
using UnityEngine;

public class SupportLineInstance : MonoBehaviour
{
    //External properties
    protected float tileSize => GameManager.instance.tileSize;
    protected BoardInstance board => GameManager.instance.board;
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    //Fields
    public float alpha = 0;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float minAlpha = Opacity.Transparent;
    private float maxAlpha = Opacity.Percent50;
    private Color color = ColorHelper.RGBA(48, 161, 49, 0);
    private LineRenderer lineRenderer;

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.sortingOrder = SortingOrder.SupportLine;
    }

    //Method which is automatically called before the first frame update  
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = tileSize / 2;
        lineRenderer.endWidth = tileSize / 2;
    }

    public void Spawn(CombatPair pair)
    {
        parent = board.transform;
        name = $"SupportLine_{Guid.NewGuid():N}";

        startPosition = pair.startActor.position;
        endPosition = pair.endActor.position;

        //lineRenderer.sortingOrder = SortingOrder.SupportLine;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        alpha = Opacity.Transparent;
        color = new Color(color.r, color.g, color.b, alpha);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        while (alpha < maxAlpha)
        {
            alpha += Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            color = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.startColor = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.endColor = new Color(color.r, color.g, color.b, alpha);

            yield return Wait.OneTick();
        }
    }

    public void TriggerDespawn()
    {
        StartCoroutine(Despawn());
    }

    public IEnumerator Despawn()
    {
        while (alpha > minAlpha)
        {
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            color = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            yield return Wait.OneTick();
        }
    }



    public void Destroy()
    {
        Destroy(this.gameObject);
    }



}
