using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles fade-in and fade-out of the attached Image component.
/// </summary>
[RequireComponent(typeof(Image))]
public class CanvasOverlay : MonoBehaviour
{
    private Image overlayImage;
    private Coroutine fadeCoroutine;

    [SerializeField] private float fadeDuration = 1.0f; // Duration of fade effect
    [SerializeField] private float minAlpha = Opacity.Transparent; // Fully transparent
    [SerializeField] private float maxAlpha = Opacity.Percent70; // Maximum opacity

    private void Awake()
    {
        overlayImage = GetComponent<Image>();
        if (overlayImage == null)
        {
            Debug.LogError("CanvasOverlay requires an Image component.");
            enabled = false;
        }
    }

    /// <summary>
    /// Instantly hides the overlay (fully transparent).
    /// </summary>
    public void Hide()
    {
        SetAlpha(0f);
    }

    /// <summary>
    /// Instantly shows the overlay (fully visible).
    /// </summary>
    public void Show()
    {
        SetAlpha(maxAlpha);
    }

    /// <summary>
    /// Fades the overlay in (opaque).
    /// </summary>
    public void FadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(maxAlpha));
    }

    /// <summary>
    /// Fades the overlay out (transparent).
    /// </summary>
    public void FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(minAlpha));
    }

    /// <summary>
    /// Handles the fade transition over time.
    /// </summary>
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = overlayImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    /// <summary>
    /// Sets the alpha of the overlay instantly.
    /// </summary>
    private void SetAlpha(float alpha)
    {
        overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
    }
}
