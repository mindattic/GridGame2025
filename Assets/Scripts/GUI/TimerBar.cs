using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    //Quick Reference Properties
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected float tileSize => GameManager.instance.tileSize;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected float snapDistance => GameManager.instance.snapDistance;

    // Timer settings
    private const float maxDuration = 6f;
    private float timeRemaining = maxDuration;

    // UI elements (assumed to be Images now)
    private Image back;
    private Image bar;
    private Image front;

    private float maxWidth;

    // Reference to the active countdown coroutine
    private Coroutine countdown;

    private void Awake()
    {
        // Assumes the children are ordered: 0 = back, 1 = bar, 2 = front.
        back = transform.GetChild("Back").GetComponent<Image>();
        bar = transform.GetChild("Bar").GetComponent<Image>();
        front = transform.GetChild("Front").GetComponent<Image>();

        maxWidth = back.rectTransform.rect.width;
    }

  

    private IEnumerator Countdown()
    {
        while (timeRemaining > 0)
        {
            // If debug mode has the timer infinite, simply wait.
            if (debugManager.isTimerInfinite)
            {
                yield return Wait.UntilNextFrame();
                continue;
            }

            timeRemaining -= Time.deltaTime;
            Set();
            yield return Wait.UntilNextFrame();
        }

        // When the timer expires, force drop (only once).
        selectedPlayerManager.Drop();
    }


    private void Set()
    {
        Vector2 newSize = bar.rectTransform.sizeDelta;
        newSize.x = maxWidth * (timeRemaining / maxDuration);
        bar.rectTransform.sizeDelta = newSize;
    }

    /// <summary>
    /// Starts the countdown.
    /// </summary>
    public void Play()
    {
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
           
        countdown = StartCoroutine(Countdown());
    }

    /// <summary>
    /// Pauses the countdown.
    /// </summary>
    public void Pause()
    {
        if (countdown != null)
        {
            StopCoroutine(countdown);
        }
    }

    public void Refill()
    {
        back.color = ColorHelper.Solid.White;
        bar.color = ColorHelper.Solid.White;
        front.color = ColorHelper.Solid.White;

        timeRemaining = maxDuration;
        Set();
    }

    public void Lock()
    {
        back.color = ColorHelper.Translucent.Red;
        bar.color = ColorHelper.Translucent.Red;
        front.color = ColorHelper.Translucent.Red;
    }
}
