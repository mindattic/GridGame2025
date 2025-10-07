using System.Collections;
using UnityEngine;
using TMPro;
using Assets.Helper;
using System.Collections.Generic;

/// <summary>
/// Displays "Wave X/Y" text with a rotate-in, hold, rotate-out animation.
/// Supports multiple TextMeshPro - Text (UI) children under the WaveAnnouncement root.
/// </summary>
public class WaveAnnouncement : MonoBehaviour
{
    // Controls how quickly the banner rotates toward its target angle.
    public float rotationFocus = 200f;

    // Track the currently running animation to prevent overlapping.
    private Coroutine animationRoutine;

    GameObject root;
    TextMeshProUGUI back;
    TextMeshProUGUI front;

    // ------------------------------------------------------------------------
    // Unity lifecycle
    // ------------------------------------------------------------------------

    private void Awake()
    {
        // Resolve labels using GameObjectHelper strongly-typed paths.
        root = GameObjectHelper.Game.WaveAnnouncement.Root;
        back = GameObjectHelper.Game.WaveAnnouncement.Back;
        front = GameObjectHelper.Game.WaveAnnouncement.Front;
    }

    private void Start()
    {
        // Ensure the initial rotation is -90 degrees so it is hidden off-axis.
        transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        // Keep object active; hide by alpha.
        SetLabelAlpha(0);
    }

    // ------------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------------

    /// <summary>
    /// Shows "Wave current/total" with an entrance, a short hold, and an exit.
    /// </summary>
    public void Show(int currentWave, int totalWaves)
    {
        SetText($"Wave {currentWave}/{totalWaves}");
        SetLabelAlpha(255);
        RestartAnimation();
    }

    /// <summary>
    /// Shows Wave current/ for Endless mode.
    /// </summary>
    public void ShowEndless(int currentWave)
    {
        SetText($"Wave {currentWave}/\u221E");
        SetLabelAlpha(255);
        RestartAnimation();
    }

    // ------------------------------------------------------------------------
    // Animation
    // ------------------------------------------------------------------------

    private void RestartAnimation()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(AnimateWaveTextRoutine());
    }

    /// <summary>
    /// Rotate in, wait, then rotate out and hide (by alpha).
    /// </summary>
    private IEnumerator AnimateWaveTextRoutine()
    {
        // Rotate into view
        yield return RotateToRoutine(0f);

        // Hold on screen briefly
        yield return new WaitForSeconds(3f);

        // Rotate out of view
        yield return RotateToRoutine(-90f);

        // Hide by alpha after leaving
        SetLabelAlpha(0);
        animationRoutine = null;
    }

    /// <summary>
    /// Smoothly rotates the transform to the given X angle.
    /// </summary>
    private IEnumerator RotateToRoutine(float targetX)
    {
        Quaternion target = Quaternion.Euler(targetX, 0f, 0f);

        while (Quaternion.Angle(transform.localRotation, target) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                target,
                rotationFocus * Time.deltaTime
            );
            yield return Wait.None();
        }

        // Snap exactly to the target to avoid tiny residual angles.
        transform.localRotation = target;
    }

    // ------------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------------

    private void SetText(string value)
    {
        if (back != null) back.text = value;
        if (front != null) front.text = value;
    }

    private void SetLabelAlpha(byte a)
    {
        if (back != null)
        {
            var c = (Color32)back.color;
            c.a = a;
            back.color = c;
        }

        if (front != null)
        {
            var c = (Color32)front.color;
            c.a = a;
            front.color = c;
        }
    }
}
