using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays "Wave X/Y" textarea with an animation that rotates it into view,
/// holds for 3 seconds, and then rotates it out.
/// </summary>
public class WaveAnnouncement : MonoBehaviour
{
    public TextMeshProUGUI label; // UI CreditsLabel component
    public float rotationSpeed = 200f; // Speed of rotation

    public void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // Ensure the initial rotation is -90 degrees
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        gameObject.SetActive(false); // Hide until needed
    }

    /// <summary>
    /// Displays the wave announcement with an animated entrance and exit.
    /// </summary>
    public void Show(int currentWave, int totalWaves)
    {
        gameObject.SetActive(true);
        label.text = $"Wave {currentWave}/{totalWaves}";

        // Start animation sequence
        StartCoroutine(AnimateWaveText());
    }

    /// <summary>
    /// Animates the wave textarea: rotates in, holds, and rotates out.
    /// </summary>
    private IEnumerator AnimateWaveText()
    {
        yield return RotateTo(0f); // Rotate into view
        yield return new WaitForSeconds(3f); // Hold for 3 seconds
        yield return RotateTo(-90f); // Rotate out of view

        gameObject.SetActive(false); // Hide after animation
    }

    /// <summary>
    /// Smoothly rotates the textarea to the given X angle.
    /// </summary>
    private IEnumerator RotateTo(float targetX)
    {
        Quaternion targetRotation = Quaternion.Euler(targetX, 0, 0);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation; // Ensure exact final rotation
    }
}
