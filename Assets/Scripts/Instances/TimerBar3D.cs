using UnityEngine;
using System.Collections;
using g = Assets.Helpers.GameHelper;
using Assets.Helper;

public class TimerBar3D : MonoBehaviour
{
    private SpriteRenderer backRenderer;
    private SpriteRenderer fillRenderer;
    private SpriteRenderer frontRenderer;

    private const float maxDuration = 6f;
    private float timeRemaining = maxDuration;

    // record the original scale and position of the fill
    private Vector3 fillOriginalScale;
    private Vector3 fillOriginalPosition;

    // sprite width in world-units when at full scale
    private float fillSpriteWidth;

    private Coroutine countdown;

    private void Awake()
    {
        backRenderer = transform.Find("Back").GetComponent<SpriteRenderer>();
        fillRenderer = transform.Find("Bar").GetComponent<SpriteRenderer>();
        frontRenderer = transform.Find("Front").GetComponent<SpriteRenderer>();

        // cache original local scale & position
        fillOriginalScale = fillRenderer.transform.localScale;
        fillOriginalPosition
            = fillRenderer.transform.localPosition;

        // compute fullsize width in world units
        fillSpriteWidth
            = fillRenderer.sprite.bounds.size.x
            * fillOriginalScale.x;
    }

    private IEnumerator CountdownRoutine()
    {
        while (timeRemaining > 0f)
        {
            if (g.DebugManager.isTimerInfinite)
            {
                yield return Wait.None();
                continue;
            }

            timeRemaining -= Time.deltaTime;
            UpdateFill();
            yield return Wait.None();
        }

        g.SelectedHeroManager.Drop();
    }

    private void UpdateFill()
    {
        // clamp ratio 01
        float ratio = Mathf.Clamp01(timeRemaining / maxDuration);

        // 1) scale the fill
        Vector3 s = fillOriginalScale;
        s.x *= ratio;
        fillRenderer.transform.localScale = s;

        // 2) shift it so the left edge stays in place
        float newWidth = fillSpriteWidth * ratio;
        // when newWidth < fillSpriteWidth, (fillSpriteWidth - newWidth) > 0
        // multiply by -0.5 to Move the center left by half the shrink-amount
        float offset = (fillSpriteWidth - newWidth) * -0.5f;
        fillRenderer.transform.localPosition
            = fillOriginalPosition
            + Vector3.right * offset;
    }

    public void Play()
    {
        if (countdown != null)
            StopCoroutine(countdown);

        timeRemaining = maxDuration;
        UpdateFill();
        countdown = StartCoroutine(CountdownRoutine());
    }

    public void Pause()
    {
        if (countdown != null)
            StopCoroutine(countdown);
    }

    public void Refill()
    {
        backRenderer.color = ColorHelper.Solid.White;
        fillRenderer.color = ColorHelper.Solid.White;
        frontRenderer.color = ColorHelper.Solid.White;

        timeRemaining = maxDuration;
        UpdateFill();
    }

    public void Lock()
    {
        backRenderer.color = ColorHelper.Translucent.Red;
        fillRenderer.color = ColorHelper.Translucent.Red;
        frontRenderer.color = ColorHelper.Translucent.Red;
    }
}
