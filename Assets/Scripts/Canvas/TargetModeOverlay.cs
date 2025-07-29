using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameManagerHelper;

public class TargetModeOverlay : MonoBehaviour
{
    // Components
    private Image image;
    private Coroutine fadeCoroutine;

    private float minAlpha = 0f;
    private float maxAlpha = 0.3333f;
    private float duration = 0.15f;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Initialize()
    {
        // Subscribe to input mode changes
        g.InputManager.OnInputModeChanged += HandleModeChanged;
        // Initial state
        HandleModeChanged(g.InputManager.inputMode);
    }

    private void OnDestroy()
    {
        // Unsubscribe
        if (g.InputManager != null)
            g.InputManager.OnInputModeChanged -= HandleModeChanged;
    }

    private void HandleModeChanged(InputMode mode)
    {
        // Stop any currently running fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        switch (mode)
        {
            case InputMode.HeroTurn:
                fadeCoroutine = StartCoroutine(Fade(maxAlpha, minAlpha, duration)); // fade out quickly
                break;

            case InputMode.AbilityTarget:
                gameObject.SetActive(true); // Ensure enabled so fade can run
                fadeCoroutine = StartCoroutine(Fade(minAlpha, maxAlpha, duration)); // fade in quickly
                break;
        }
    }

    // Fades the overlay alpha from 'from' to 'to' over 'duration' seconds.
    private IEnumerator Fade(float from, float to, float duration)
    {
        // Make sure image is enabled
        image.enabled = true;
        Color color = image.color;
        color.a = from;
        image.color = color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(from, to, t);
            image.color = color;
            yield return null;
        }
        color.a = to;
        image.color = color;

        // Optionally disable GameObject after fading out
        if (Mathf.Approximately(to, minAlpha))
            gameObject.SetActive(false);
    }
}
