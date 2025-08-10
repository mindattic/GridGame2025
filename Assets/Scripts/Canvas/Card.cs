using Assets.Helper;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;
using Label = TMPro.TextMeshProUGUI;

// The Card class manages the UI card display that shows details about a focused actor.
// It handles initialization, assignment of actors (such as portrait, name, and stats),
// and provides an action to slide the portrait into view.
public class Card : MonoBehaviour
{
    RectTransform card;
    RectTransform backdrop;
    RectTransform portrait;
    RectTransform title;
    RectTransform details;
    Vector3 offscreenPosition;       // Starting offscreen position for the portrait.
    Vector3 destination;             // Final destination position for the portrait during slide-in action. 
    AnimationCurve slideInCurve;     // Easing curve for slide-in action.
    float slideDuration;            // Duration of the slide-in action in seconds.
    float portraitSize;


    private void Awake()
    {
        card = GameObject.Find(GameObjectHelper.Game.Card.Root).GetComponent<RectTransform>();
        backdrop = GameObject.Find(GameObjectHelper.Game.Card.Backdrop).GetComponent<RectTransform>();
        portrait = GameObject.Find(GameObjectHelper.Game.Card.Portrait).GetComponent<RectTransform>();
        title = GameObject.Find(GameObjectHelper.Game.Card.Title).GetComponent<RectTransform>();
        details = GameObject.Find(GameObjectHelper.Game.Card.Details).GetComponent<RectTransform>();

        portraitSize = c.CanvasRect.rect.width;
        slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        slideDuration = 0.5f;

        Clear();
    }

    private void Start()
    {
        //Show the portrait's size
        portrait.sizeDelta = new Vector2(portraitSize, portraitSize);

        // Define the offscreen starting position, just outside the screen width.
        offscreenPosition = new Vector3(portraitSize, 0);

        // Destination is calculated so that the portrait centers within its rect.
        destination = new Vector3(-portraitSize / 4, 0);
    }

    //Populates the card with actors from the currently focused actor.
    public void Assign()
    {
        // If no actor is focused, exit without making changes.
        if (!g.Actors.HasFocusedActor)
            return;

        var actorData = ActorRepo.Get(g.Actors.FocusedActor.characterName);

        // Enable the backdrop and portrait images.
        backdrop.gameObject.SetActive(true);
        portrait.gameObject.SetActive(true);


        portrait.GetComponent<Image>().sprite = actorData.Portrait;
        title.GetComponent<Label>().text = g.Actors.FocusedActor.characterName;

        // Format the actor's stats for display:
        var HP = $"{g.Actors.FocusedActor.stats.HP,2}/{g.Actors.FocusedActor.stats.MaxHP,-3}";
        var STR = $"{g.Actors.FocusedActor.stats.Strength,4}";
        var VIT = $"{g.Actors.FocusedActor.stats.Vitality,4}";
        var AGI = $"{g.Actors.FocusedActor.stats.Agility,4}";
        var STA = $"{g.Actors.FocusedActor.stats.Stamina,4}";
        var INT = $"{g.Actors.FocusedActor.stats.Intelligence,4}";
        var WIS = $"{g.Actors.FocusedActor.stats.Wisdom,4}";
        var LCK = $"{g.Actors.FocusedActor.stats.Luck,4}";

        // Data a formatted stats table string.
        var stats =
            $"HP       STR  VIT  AGI  STA  INT  WIS  LCK{Environment.NewLine}" +
            $"{HP}   {STR}{VIT}{AGI}{STA}{INT}{WIS}{LCK}{Environment.NewLine}";

        // Show the details textarea combining the stats table with extra details from DataManager.
        details.GetComponent<Label>().text = stats + actorData.Details.Card;

        // Begin the slide-in action for the portrait.
        SlideIn();
    }


    private void SlideIn()
    {
        StartCoroutine(SlideInRoutine());
    }

    // Show smoothly animates the portrait image from an offscreen position to its destination.
    private IEnumerator SlideInRoutine()
    {
        float elapsedTime = 0f;
        portrait.anchoredPosition = offscreenPosition;

        // Animate over the duration specified by slideDuration.
        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / slideDuration);
            // Evaluate the easing curve to get a smooth transition.
            float curveValue = slideInCurve.Evaluate(progress);

            // Lerp (linearly interpolate) the portrait's position from offscreen to destination.
            portrait.anchoredPosition = Vector3.Lerp(offscreenPosition, destination, curveValue);

            yield return Wait.OneTick(); // Wait for the next frame.
        }

        // Ensure the portrait is exactly at the destination position after the action.
        portrait.anchoredPosition = destination;
    }

    // Hide resets the card UI to a hidden state, clearing all displayed g.Actors.All.
    public void Clear()
    {
        // Disable visual components of the card.
        backdrop.gameObject.SetActive(false);
        portrait.gameObject.SetActive(false);
        title.GetComponent<Label>().text = "";
        details.GetComponent<Label>().text = "";

        // Reset the portrait's position to the offscreen starting position.
        portrait.anchoredPosition = offscreenPosition;
    }


    public Vector3 PortraitWorldPosition()
    {
        return ScreenHelper.Convert.CanvasToWorldPosition(portrait.transform);
    }


    /// <summary>
    /// ProcessRoutine a quick bounce animation on the portrait: up, then down.
    /// </summary>
    public void BouncePortrait(float percentOfScreenHeight = 0.03f, float bounceDuration = 0.3333f)
    {
        float bounceDistance = Screen.height * percentOfScreenHeight;
        StartCoroutine(BouncePortraitRoutune(bounceDistance, bounceDuration));
    }

    /// <summary>
    /// StartCoroutine to animate the portrait bouncing up then back down.
    /// </summary>
    private IEnumerator BouncePortraitRoutune(float bounceDistance, float bounceDuration)
    {
        // Store original anchored position
        Vector2 originalPos = portrait.anchoredPosition;
        Vector2 upPos = originalPos + Vector2.up * bounceDistance;
        float halfDuration = bounceDuration / 2f;
        float elapsed = 0f;

        // Seek up
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            portrait.anchoredPosition = Vector2.Lerp(originalPos, upPos, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }
        portrait.anchoredPosition = upPos;

        // Seek down
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            portrait.anchoredPosition = Vector2.Lerp(upPos, originalPos, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }
        portrait.anchoredPosition = originalPos;
    }




}
