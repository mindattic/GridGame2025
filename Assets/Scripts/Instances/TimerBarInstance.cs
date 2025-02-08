using System.Collections;
using UnityEngine;

public class TimerBarInstance : MonoBehaviour
{
    // External properties
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected float tileSize => GameManager.instance.tileSize;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected float snapDistance => GameManager.instance.snapDistance;

    // Timer settings
    private const float maxDuration = 6f;
    private float timeRemaining = maxDuration;
    private bool isRunning = false;

    // UI elements (assumed to be SpriteRenderers)
    private SpriteRenderer back;
    private SpriteRenderer bar;
    private SpriteRenderer front;

    // Cached values for sliding and scaling
    private Vector3 scale = Vector3.one;
    private Vector3 initialPosition;
    private Vector3 offscreenPosition;
    private float slideSpeed;
    private float width;

    // Reference to the active countdown coroutine
    private Coroutine countdown;

    private void Awake()
    {
        // Assumes the children are ordered: 0 = back, 1 = bar, 2 = front.
        back = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bar = transform.GetChild(1).GetComponent<SpriteRenderer>();
        front = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Initialize the bar's scale.
        bar.transform.localScale = scale;
        // Calculate slide speed based on tileSize.
        slideSpeed = tileSize * 0.25f;
        // Use the front's bounds (plus an offset) to compute the offscreen width.
        width = front.bounds.size.x + 2.4f;
        // Save the on-screen (initial) position.
        initialPosition = transform.position;
        // Compute the offscreen position by subtracting the width from the initial position.
        offscreenPosition = initialPosition.SubtractX(width);
    }

    private IEnumerator Countdown()
    {
        isRunning = true;
        while (timeRemaining > 0)
        {
            // If debug mode has the timer infinite, simply wait.
            if (debugManager.isTimerInfinite)
            {
                yield return Wait.UntilNextFrame();
                continue;
            }

            timeRemaining -= Time.deltaTime;
            // Update the bar's horizontal scale to reflect the remaining time.
            float newXScale = scale.x * (timeRemaining / maxDuration);
            bar.transform.localScale = new Vector3(newXScale, scale.y, scale.z);
            yield return Wait.UntilNextFrame();
        }
        isRunning = false;

        // When the timer expires, force drop (only once).
        selectedPlayerManager.Drop();
    }

    /// <summary>
    /// Starts the countdown.
    /// </summary>
    public void Play()
    {
        if (countdown != null)
            StopCoroutine(countdown);

        countdown = StartCoroutine(Countdown());
    }

    /// <summary>
    /// Pauses the countdown.
    /// </summary>
    public void Pause()
    {
        isRunning = false;
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
    }

    public void Refill()
    {
        isRunning = false;
        back.color = ColorHelper.Solid.White;
        bar.color = ColorHelper.Solid.White;
        front.color = ColorHelper.Solid.White;

        timeRemaining = maxDuration;
        float newXScale = scale.x;
        bar.transform.localScale = new Vector3(newXScale, scale.y, scale.z);
    }

    public void Lock()
    {
        back.color = ColorHelper.Translucent.Red;
        bar.color = ColorHelper.Translucent.Red;
        front.color = ColorHelper.Translucent.Red;
    }

}
