using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    //Quick Reference Properties for accessing game systems
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected float tileSize => GameManager.instance.tileSize;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected float snapDistance => GameManager.instance.snapThreshold;

    //Timer settings
    private const float maxDuration = 6f;
    private float timeRemaining = maxDuration;

    //UI elements for displaying the timer bar
    private Image back;
    private Image bar;
    private Image front;

    //Maximum width of the timer bar
    private float maxWidth;

    //Reference to the active countdown coroutine
    private Coroutine countdown;

    private void Awake()
    {
        //Assumes the children are ordered: 0 = back, 1 = bar, 2 = front
        back = transform.GetChild("Back").GetComponent<Image>();
        bar = transform.GetChild("Bar").GetComponent<Image>();
        front = transform.GetChild("Front").GetComponent<Image>();

        //Set maxWidth based on the background image width
        maxWidth = back.rectTransform.rect.width;
    }

    /// <summary>
    ///Handles the countdown logic, decrementing time and triggering events when time runs out.
    /// </summary>
    private IEnumerator Countdown()
    {
        while (timeRemaining > 0)
        {
            //If debug mode has the timer infinite, simply wait
            if (debugManager.isTimerInfinite)
            {
                yield return Wait.UntilNextFrame();
                continue;
            }

            //Decrement time and update the timer UI
            timeRemaining -= Time.deltaTime;
            Set();
            yield return Wait.UntilNextFrame();
        }

        //When the timer expires, force drop the selected player
        selectedPlayerManager.Drop();
    }

    /// <summary>
    ///Updates the timer bar size based on the remaining time.
    /// </summary>
    private void Set()
    {
        //Calculate new width proportional to the remaining time
        Vector2 newSize = bar.rectTransform.sizeDelta;
        newSize.x = maxWidth * (timeRemaining / maxDuration);
        bar.rectTransform.sizeDelta = newSize;
    }

    /// <summary>
    ///Starts the countdown.
    /// </summary>
    public void Play()
    {
        //Stop any existing countdown before starting a new one
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }

        //Start a new countdown coroutine
        countdown = StartCoroutine(Countdown());
    }

    /// <summary>
    ///Pauses the countdown.
    /// </summary>
    public void Pause()
    {
        //Stop the countdown if it is running
        if (countdown != null)
        {
            StopCoroutine(countdown);
        }
    }

    /// <summary>
    ///Refills the timer bar and resets the timer.
    /// </summary>
    public void Refill()
    {
        //Reset colors to white
        back.color = ColorHelper.Solid.White;
        bar.color = ColorHelper.Solid.White;
        front.color = ColorHelper.Solid.White;

        //Reset the timer and update the bar
        timeRemaining = maxDuration;
        Set();
    }

    /// <summary>
    ///Locks the timer bar visually by changing its color to red.
    /// </summary>
    public void Lock()
    {
        //Set colors to translucent red to indicate locked state
        back.color = ColorHelper.Translucent.Red;
        bar.color = ColorHelper.Translucent.Red;
        front.color = ColorHelper.Translucent.Red;
    }
}
