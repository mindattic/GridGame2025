using System.Collections;
using UnityEngine;

public class BoardOverlay : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    float alpha = 0;
    float minAlpha = Opacity.Transparent;
    float maxAlpha = Opacity.Percent70;
    Color color = ColorHelper.Translucent.DarkBlack;
    float increment = Increment.TwoPercent;

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = SortingOrder.BoardOverlay;
        spriteRenderer.enabled = false;
    }

    public void TriggerFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        //Before:
        alpha = minAlpha;
        color.a = alpha;
        spriteRenderer.color = color;
        spriteRenderer.enabled = true;

        //During:
        while (alpha < maxAlpha)
        {
            alpha += increment;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            color.a = alpha;
            spriteRenderer.color = color;
            yield return Wait.OneTick();
        }

        //After:
        alpha = maxAlpha;
        color.a = alpha;
        spriteRenderer.color = color;
    }


    public void TriggerFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        //Before:
        alpha = maxAlpha;
        color.a = alpha;
        spriteRenderer.color = color;

        //During:
        while (alpha > minAlpha)
        {
            alpha -= increment;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            color.a = alpha;
            spriteRenderer.color = color;
            yield return Wait.OneTick();
        }

        //After:
        alpha = minAlpha;
        color.a = alpha;
        spriteRenderer.color = color;
        spriteRenderer.enabled = false;
    }

}
