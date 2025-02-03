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
    private Coroutine countdownCoroutine;

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

    ///// <summary>
    ///// Resets the timer bar by sliding it into view and resetting the countdown.
    ///// </summary>
    //public void TriggerInitialize()
    //{
    //    // Stop any running coroutines (e.g. a SlideOut or a countdown)
    //    StopAllCoroutines();
    //    countdownCoroutine = null;
    //    // Start sliding the timer bar into its initial position.
    //    StartCoroutine(Initialize());
    //}

    ///// <summary>
    ///// Coroutine that slides the timer bar to its initial position and resets its state.
    ///// </summary>
    //IEnumerator Initialize()
    //{
    //    // Reset bar scale and timer.
    //    bar.transform.localScale = new Vector3(1f, scale.y, scale.z);
    //    timeRemaining = maxDuration;
    //    isRunning = false;

    //    // Slide the bar into the on?screen position.
    //    while (Vector3.Distance(transform.position, initialPosition) > snapDistance)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, initialPosition, slideSpeed * Time.deltaTime);
    //        yield return Wait.OneTick();
    //    }
    //    transform.position = initialPosition;
    //}

    /// <summary>
    /// Coroutine that counts down the timer bar.
    /// When time expires, calls selectedPlayerManager.Unselect().
    /// </summary>
    private IEnumerator CountdownCoroutine()
    {
        isRunning = true;
        while (timeRemaining > 0)
        {
            // If debug mode has the timer infinite, simply wait.
            if (debugManager.isTimerInfinite)
            {
                yield return null;
                continue;
            }

            timeRemaining -= Time.deltaTime;
            // Update the bar's horizontal scale to reflect the remaining time.
            float newXScale = scale.x * (timeRemaining / maxDuration);
            bar.transform.localScale = new Vector3(newXScale, scale.y, scale.z);
            yield return null;
        }
        isRunning = false;
        // When the timer expires, force unselect (only once).
        selectedPlayerManager.Unselect();
    }

    /// <summary>
    /// Starts the countdown.
    /// </summary>
    public void Play()
    {
        // If a countdown is already running, stop it.
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// Pauses the countdown.
    /// </summary>
    public void Pause()
    {
        isRunning = false;
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
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

    /// <summary>
    /// Slides the timer bar off-screen.
    /// </summary>
    //public void Hide()
    //{
    //    // Stop any countdown while hiding.
    //    Pause();
    //    StartCoroutine(SlideOut());
    //}

    /// <summary>
    /// Coroutine that slides the timer bar to the offscreen position.
    /// </summary>
    //IEnumerator SlideOut()
    //{
    //    while (Vector3.Distance(transform.position, offscreenPosition) > snapDistance)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, offscreenPosition, slideSpeed * Time.deltaTime);
    //        yield return Wait.OneTick();
    //    }
    //    transform.position = offscreenPosition;
    //}

    /// <summary>
    /// Optionally, you can use this method to simply enable the bar.
    /// </summary>
    //public void Show()
    //{
    //    bar.enabled = true;
    //}
}
