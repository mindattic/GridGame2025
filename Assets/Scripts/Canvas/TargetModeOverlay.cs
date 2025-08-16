// --- File: Assets/Scripts/Canvas/TargetModeOverlay.cs ---
using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Simple full screen overlay used while in targeting modes.
/// Hardened so it never starts a coroutine while inactive,
/// and it applies state instantly when disabled.
/// </summary>
public class TargetModeOverlay : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Components and state
    // ---------------------------------------------------------------------

    private Image image;                 // Background image to overlay
    private Coroutine fadeCoroutine;     // Handle to the active overlay

    // FadeRoutine parameters
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 0.3333f;
    [SerializeField] private float duration = 0.15f;

    // If a mode arrives while disabled, store it and apply on enable
    private bool hasPendingMode;
    private InputMode pendingMode;

    // ---------------------------------------------------------------------
    // Lifecycle
    // ---------------------------------------------------------------------

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            // Ensure a deterministic starting state
            var c = image.color;
            c.a = 0f;
            image.color = c;
            image.enabled = true;
        }
    }



    private void OnDisable()
    {
        // Unsubscribe first
        if (g.InputManager != null)
            g.InputManager.OnInputModeChanged -= HandleModeChanged;

        // DespawnRoutine any running animation owned by this component
        StopFade();
    }

    // ---------------------------------------------------------------------
    // Public compatibility initializer
    // ---------------------------------------------------------------------
    // Kept for backward compatibility if some bootstrapper still calls it.
    // No subscription here to avoid double registering. It only snaps
    // the initial visual state once.
    public void Initialize()
    {
        // Subscribe to input mode changes
        g.InputManager.OnInputModeChanged += HandleModeChanged;

        // If a mode was requested while disabled, apply instantly
        if (hasPendingMode)
        {
            ApplyInstant(pendingMode);
            hasPendingMode = false;
        }
        else
        {
            // Otherwise, sync to current mode instantly to avoid a pop
            ApplyInstant(g.InputManager.InputMode);
        }
    }

    // ---------------------------------------------------------------------
    // Event handling
    // ---------------------------------------------------------------------

    private void HandleModeChanged(InputMode mode)
    {
        // If we are not active, record and bail. OnEnable will apply instantly.
        if (!isActiveAndEnabled)
        {
            pendingMode = mode;
            hasPendingMode = true;
            return;
        }

        // Active. Pick animated or instant path depending on current visibility.
        bool targetVisible = ShouldBeVisible(mode);

        // Ensure we are active before animating in
        if (targetVisible && !gameObject.activeSelf)
            gameObject.SetActive(true);

        // Drive overlay
        StopFade();
        float from = GetAlpha();
        float to = targetVisible ? maxAlpha : minAlpha;
        fadeCoroutine = StartCoroutine(FadeRoutine(from, to, duration));
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    // Returns whether overlay should be visible for a given mode
    private static bool ShouldBeVisible(InputMode mode)
    {
        // Adjust these modes as your game evolves
        return mode == InputMode.AbilityTarget;
    }

    // Reads current alpha safely
    private float GetAlpha()
    {
        return image != null ? image.color.a : 0f;
    }

    // Stops an existing overlay if any
    private void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    // Applies the state instantly without coroutines
    private void ApplyInstant(InputMode mode)
    {
        bool visible = ShouldBeVisible(mode);

        if (image != null)
        {
            var c = image.color;
            c.a = visible ? maxAlpha : minAlpha;
            image.color = c;
            image.enabled = true;
        }

        // If you want the GO disabled when invisible, toggle here
        gameObject.SetActive(visible);
    }

    // ---------------------------------------------------------------------
    // Animation
    // ---------------------------------------------------------------------

    /// <summary>
    /// Fades overlay alpha from current to target over duration seconds.
    /// Uses unscaled time so it works while game is paused.
    /// </summary>
    private IEnumerator FadeRoutine(float from, float to, float seconds)
    {
        if (image == null)
            yield break;

        // Ensure image is enabled while we animate
        image.enabled = true;

        float elapsed = 0f;
        var color = image.color;
        color.a = from;
        image.color = color;

        // Early exit if duration is tiny
        if (seconds <= 0f)
        {
            color.a = to;
            image.color = color;
        }
        else
        {
            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / seconds);
                color.a = Mathf.Lerp(from, to, t);
                image.color = color;
                yield return Wait.None();
            }

            color.a = to;
            image.color = color;
        }

        // Optionally disable GO after fading out
        if (Mathf.Approximately(to, minAlpha))
            gameObject.SetActive(false);

        fadeCoroutine = null;
    }
}
