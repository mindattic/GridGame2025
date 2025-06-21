using System;
using System.Collections;
using UnityEngine;

public class SupportLineInstance : MonoBehaviour
{
    // Quick Reference Properties
    protected float tileSize => GameManager.instance.tileSize;
    protected BoardInstance board => GameManager.instance.board;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;

    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    // Fields
    public float alpha = 0f;

    [SerializeField] private float fadeDuration = 0.1f;
    private float minAlpha = Opacity.Transparent;
    private float maxAlpha = Opacity.Percent50;

    private ActorInstance actor1;
    private ActorInstance actor2;

    private Color color = ColorHelper.RGBA(48, 161, 49, 0);
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void Start()
    {
        lineRenderer.startWidth = tileSize / 4;
        lineRenderer.endWidth = tileSize / 4;
    }

    public void Spawn(ActorInstance actor1, ActorInstance actor2)
    {
        this.actor1 = actor1;
        this.actor2 = actor2;

        parent = board.transform;
        name = $"SupportLine_{Guid.NewGuid():N}";

        Vector3 offset = Vector3.down * (tileSize * 0.333f);
        lineRenderer.SetPosition(0, actor1.position + offset);
        lineRenderer.SetPosition(1, actor2.position + offset);

        StartCoroutine(FadeIn());
    }


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
            yield return null;
        }

        alpha = maxAlpha;
        UpdateLineAlpha(alpha);
    }

    public void TriggerDespawn()
    {
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        //Before:
        float startAlpha = maxAlpha;
        float targetAlpha = minAlpha;
        float elapsedTime = 0f;

        //During:
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            UpdateLineAlpha(alpha);
            yield return null;
        }

        //After:
        alpha = minAlpha;
        UpdateLineAlpha(alpha);
        supportLineManager.Destroy(actor1, actor2);
    }

    private void UpdateLineAlpha(float a)
    {
        color.a = a;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void UpdateSortingOrder()
    {
        // Placeholder for sorting logic if needed
    }


    private void FixedUpdate()
    {
        Vector3 offset = Vector3.down * (tileSize * 0.333f);
        lineRenderer.SetPosition(0, actor1.position + offset);
        lineRenderer.SetPosition(1, actor2.position + offset);
    }


    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
