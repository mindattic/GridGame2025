using System.Collections;
using UnityEngine;

/// <summary>
/// Handles fade-in and fade-out of the BoardOverlay using a SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BoardOverlay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Coroutine fadeCoroutine;

    [SerializeField] private float fadeDuration = 0.25f; // Duration of fade effect
    [SerializeField] private float minAlpha = Opacity.Transparent; // Fully transparent
    [SerializeField] private float maxAlpha = Opacity.Translucent.Alpha196; // Maximum opacity
    [SerializeField] private Color overlayColor = ColorHelper.Translucent.DarkBlack; // Default color

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sortingOrder = SortingOrder.BoardOverlay;
        spriteRenderer.enabled = false;
        SetAlpha(minAlpha);
    }

    /// <summary>
    /// Instantly hides the overlay (fully transparent).
    /// </summary>
    public void Hide()
    {
        SetAlpha(minAlpha);
        spriteRenderer.enabled = false;
    }

    /// <summary>
    /// Instantly shows the overlay (fully visible).
    /// </summary>
    public void Show()
    {
        SetAlpha(maxAlpha);
        spriteRenderer.enabled = true;
    }

    public IEnumerator FadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        yield return fadeCoroutine = StartCoroutine(Fade(maxAlpha, true));
    }

    public IEnumerator FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        yield return fadeCoroutine = StartCoroutine(Fade(minAlpha, false));
    }


    /// <summary>
    /// Handles the fade transition over time.
    /// </summary>
    private IEnumerator Fade(float targetAlpha, bool enableOnStart)
    {
        if (enableOnStart)
            spriteRenderer.enabled = true;

        float startAlpha = spriteRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);

        if (!enableOnStart)
            spriteRenderer.enabled = false;
    }

    /// <summary>
    /// Sets the alpha of the overlay instantly.
    /// </summary>
    private void SetAlpha(float alpha)
    {
        overlayColor.a = alpha;
        spriteRenderer.color = overlayColor;
    }
}
