using System.Collections;
using UnityEngine;
using TMPro;
using Assets.Helper;

/// <summary>
/// Displays "Wave X/Y" text with a rotate-in, hold, rotate-out animation.
/// </summary>
public class WaveAnnouncement : MonoBehaviour
{
    // Reference to the TMP label that shows the "Wave X/Y" text.
    // You can assign in the inspector. If left null, Awake will try to find it.
    public TextMeshProUGUI label;

    // Controls how quickly the banner rotates toward its target angle.
    public float rotationFocus = 200f;

    // ------------------------------------------------------------------------
    // Unity lifecycle
    // ------------------------------------------------------------------------

    private void Awake()
    {
        // If not wired in the inspector, try to find a TMP label in children
        // Include inactive children so prefabs with disabled text still resolve.
        if (label == null)
            label = GetComponentInChildren<TextMeshProUGUI>(true);

        // Log a clear error once if we still could not find it.
        if (label == null)
            Debug.LogError("[WaveAnnouncement] Missing TextMeshProUGUI child. Assign 'label' in the inspector or add a TMP component under this object.");
    }

    private void Start()
    {
        // Ensure the initial rotation is -90 degrees so it is hidden off-axis.
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        // Keep the object hidden until shown.
        gameObject.SetActive(false);
    }

    // ------------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------------

    /// <summary>
    /// Shows "Wave current/total" with an entrance, a short hold, and an exit.
    /// </summary>
    public void Show(int currentWave, int totalWaves)
    {
        // Do nothing if the label is missing to avoid null exceptions at runtime.
        if (label == null)
            return;

        gameObject.SetActive(true);
        label.text = $"Wave {currentWave}/{totalWaves}";

        StartCoroutine(AnimateWaveTextRoutine());
    }

    // ------------------------------------------------------------------------
    // Animation
    // ------------------------------------------------------------------------

    /// <summary>
    /// Rotate in, wait, then rotate out and hide.
    /// </summary>
    private IEnumerator AnimateWaveTextRoutine()
    {
        // Rotate into view
        yield return RotateToRoutine(0f);

        // Hold on screen briefly
        yield return new WaitForSeconds(3f);

        // Rotate out of view
        yield return RotateToRoutine(-90f);

        // Hide after leaving
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Smoothly rotates the transform to the given X angle.
    /// </summary>
    private IEnumerator RotateToRoutine(float targetX)
    {
        Quaternion target = Quaternion.Euler(targetX, 0f, 0f);

        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                target,
                rotationFocus * Time.deltaTime
            );
            yield return Wait.None();
        }

        // Snap exactly to the target to avoid tiny residual angles.
        transform.rotation = target;
    }
}
