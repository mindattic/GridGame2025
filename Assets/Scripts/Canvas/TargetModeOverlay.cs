using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Utilities; // added
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Full-screen overlay for targeting modes.
/// Always stays active; visibility is controlled by Image + Label alpha only.
/// </summary>
public class TargetModeOverlay : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Components and state
    // ---------------------------------------------------------------------

    private Image image;                 // Background image that we fade
    private TextMeshProUGUI label;       // Child label we also fade
    private Coroutine runningCoroutine;  // Active fade routine if any

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

        // Cache child "Label" TextMeshProUGUI (safe lookup)
        var labelTransform = transform.Find("Label");
        if (labelTransform != null)
            label = labelTransform.GetComponent<TextMeshProUGUI>();

        // Ensure deterministic starting visuals
        if (image != null)
        {
            var c = image.color;
            c.a = 0f;                    // Start fully transparent
            image.color = c;
            image.enabled = true;        // Keep Image enabled
        }

        if (label != null)
        {
            var lc = label.color;
            lc.a = 0f;
            label.color = lc;
        }

        GameReady.Begin(this);
    }

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
        if (!isActiveAndEnabled)
        {
            pendingMode = mode;
            hasPendingMode = true;
            return;
        }

        bool targetVisible = ShouldBeVisible(mode);

        StopFade();
        float from = GetAlpha();
        float to = targetVisible ? maxAlpha : minAlpha;
        runningCoroutine = StartCoroutine(FadeRoutine(from, to, duration));
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    private static bool ShouldBeVisible(InputMode mode)
    {
        return mode == InputMode.AbilityTarget;
    }

    private float GetAlpha()
    {
        return image != null ? image.color.a : 0f;
    }

    private void StopFade()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private void ApplyInstant(InputMode mode)
    {
        bool visible = ShouldBeVisible(mode);
        float targetAlpha = visible ? maxAlpha : minAlpha;

        if (image != null)
        {
            var c = image.color;
            c.a = targetAlpha;
            image.color = c;
            image.enabled = true;
        }

        if (label != null)
        {
            var lc = label.color;
            lc.a = targetAlpha;
            label.color = lc;
        }
    }

    // ---------------------------------------------------------------------
    // Animation
    // ---------------------------------------------------------------------

    private IEnumerator FadeRoutine(float from, float to, float seconds)
    {
        if (image == null && label == null)
            yield break;

        if (image != null)
            image.enabled = true;

        float elapsed = 0f;

        if (seconds <= 0f)
        {
            SetAlpha(to);
            runningCoroutine = null;
            yield break;
        }

        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / seconds);
            float alpha = Mathf.Lerp(from, to, t);
            SetAlpha(alpha);
            yield return Wait.None();
        }

        SetAlpha(to);
        runningCoroutine = null;
    }

    // Apply alpha to both background and label
    private void SetAlpha(float a)
    {
        if (image != null)
        {
            var c = image.color;
            c.a = a;
            image.color = c;
        }

        if (label != null)
        {
            var lc = label.color;
            lc.a = a;
            label.color = lc;
        }
    }
}
