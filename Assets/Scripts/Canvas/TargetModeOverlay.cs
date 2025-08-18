// --- File: Assets/Scripts/Canvas/TargetModeOverlay.cs ---
using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Full-screen overlay for targeting modes.
/// Always stays active; visibility is controlled by Image alpha only.
/// </summary>
public class TargetModeOverlay : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Components and state
    // ---------------------------------------------------------------------

    private Image image;                 // Background image that we fade
    private Coroutine fadeCoroutine;     // Active fade routine if any

    // Fade parameters
    [SerializeField] private float minAlpha = 0f;          // Fully transparent
    [SerializeField] private float maxAlpha = 0.3333f;     // Visible overlay alpha
    [SerializeField] private float duration = 0.15f;       // Fade time (unscaled)

    // If a mode arrives while this component is disabled, store and apply on enable
    private bool hasPendingMode;         // Tracks if we queued a mode
    private InputMode pendingMode;       // The queued mode value

    // ---------------------------------------------------------------------
    // Lifecycle
    // ---------------------------------------------------------------------

    private void Awake()
    {
        // Cache the Image component
        image = GetComponent<Image>();

        // Ensure deterministic starting visuals
        if (image != null)
        {
            var c = image.color;
            c.a = 0f;                    // Start fully transparent
            image.color = c;
            image.enabled = true;        // Keep Image enabled; GO is never disabled by this script
        }
    }

    //private void OnEnable()
    //{
    //    // Subscribe to input mode changes if the input manager exists
    //    if (g.InputManager != null)
    //        g.InputManager.OnInputModeChanged += HandleModeChanged;

    //    // Apply any queued mode instantly
    //    if (hasPendingMode)
    //    {
    //        ApplyInstant(pendingMode);
    //        hasPendingMode = false;
    //    }
    //    else if (g.InputManager != null)
    //    {
    //        // Sync instantly to current mode on enable to avoid popping
    //        ApplyInstant(g.InputManager.InputMode);
    //    }
    //}

    //private void OnDisable()
    //{
    //    // Unsubscribe safely
    //    if (g.InputManager != null)
    //        g.InputManager.OnInputModeChanged -= HandleModeChanged;

    //    // Stop any running fade coroutine
    //    StopFade();
    //}

    public void Initialize()
    {
        // Subscribe if not already
        if (g.InputManager != null)
            g.InputManager.OnInputModeChanged += HandleModeChanged;

        // Snap to current state immediately
        if (hasPendingMode)
        {
            ApplyInstant(pendingMode);
            hasPendingMode = false;
        }
        else if (g.InputManager != null)
        {
            ApplyInstant(g.InputManager.InputMode);
        }
    }

    // ---------------------------------------------------------------------
    // Event handling
    // ---------------------------------------------------------------------

    private void HandleModeChanged(InputMode mode)
    {
        // If the component is disabled, defer until OnEnable
        if (!isActiveAndEnabled)
        {
            pendingMode = mode;
            hasPendingMode = true;
            return;
        }

        // Decide target visibility based on mode
        bool targetVisible = ShouldBeVisible(mode);

        // Drive fade purely by alpha (no SetActive toggles)
        StopFade();
        float from = GetAlpha();
        float to = targetVisible ? maxAlpha : minAlpha;
        fadeCoroutine = StartCoroutine(FadeRoutine(from, to, duration));
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    // Return whether the overlay should be visible for the given mode
    private static bool ShouldBeVisible(InputMode mode)
    {
        // Adjust as your game evolves
        return mode == InputMode.AbilityTarget;
    }

    // Get current alpha safely
    private float GetAlpha()
    {
        return image != null ? image.color.a : 0f;
    }

    // Stop an in-flight fade routine
    private void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    // Apply the target state instantly (no coroutine)
    private void ApplyInstant(InputMode mode)
    {
        bool visible = ShouldBeVisible(mode);

        if (image != null)
        {
            var c = image.color;
            c.a = visible ? maxAlpha : minAlpha;   // Only alpha changes
            image.color = c;
            image.enabled = true;                  // Keep the Image enabled
        }
    }

    // ---------------------------------------------------------------------
    // Animation
    // ---------------------------------------------------------------------

    /// <summary>
    /// Fade from current alpha to target alpha over the given duration.
    /// Uses unscaled time so it works while the game is paused.
    /// </summary>
    private IEnumerator FadeRoutine(float from, float to, float seconds)
    {
        if (image == null)
            yield break;

        // Ensure the Image is enabled while animating
        image.enabled = true;

        float elapsed = 0f;
        var color = image.color;
        color.a = from;
        image.color = color;

        // Immediate path for zero duration
        if (seconds <= 0f)
        {
            color.a = to;
            image.color = color;
            fadeCoroutine = null;
            yield break;
        }

        // Animate alpha
        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / seconds);
            color.a = Mathf.Lerp(from, to, t);
            image.color = color;
            yield return Wait.None();
        }

        // Snap final alpha
        color.a = to;
        image.color = color;

        // Note: We do NOT disable the GameObject here.
        // The overlay remains active at all times.

        fadeCoroutine = null;
    }
}
