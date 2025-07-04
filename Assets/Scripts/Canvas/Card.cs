using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;


// The Card class manages the UI card display that shows details about a focused actor.
// It handles initialization, assignment of actors (such as portrait, name, and stats),
// and provides an animate to slide the portrait into view.
public class Card : MonoBehaviour
{
    #region Game Properies
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator;
    protected bool hasFocusedActor => GameManager.instance.hasFocusedActor;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    #endregion

    private RectTransform canvas2D;
    RectTransform card;
    RectTransform backdrop;
    RectTransform portrait;
    RectTransform title;
    RectTransform details;
    Vector3 offscreenPosition;       // Starting offscreen position for the portrait.
    Vector3 destination;             // Final destination position for the portrait during slide-in animate. 
    AnimationCurve slideInCurve;     // Easing curve for slide-in animate.
    float slideDuration;            // Duration of the slide-in animate in seconds.
    float portraitSize;


    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.Game.Canvas2D).GetComponent<RectTransform>();
        card = GameObject.Find(ComponentHelper.Game.Card.Root).GetComponent<RectTransform>();
        backdrop = GameObject.Find(ComponentHelper.Game.Card.Backdrop).GetComponent<RectTransform>();
        portrait = GameObject.Find(ComponentHelper.Game.Card.Portrait).GetComponent<RectTransform>();
        title = GameObject.Find(ComponentHelper.Game.Card.Title).GetComponent<RectTransform>();
        details = GameObject.Find(ComponentHelper.Game.Card.Details).GetComponent<RectTransform>();

        portraitSize = canvas2D.rect.width;
        slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        slideDuration = 0.5f;

        Clear();
    }

    private void Start()
    {
        //Assign the portrait's size
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
        if (!hasFocusedActor)
            return;

        // Enable the backdrop and portrait images.
        backdrop.gameObject.SetActive(true);
        portrait.gameObject.SetActive(true);

        string address = $"Actor-Portraits/{focusedActor.characterName}";
        portrait.GetComponent<Image>().sprite = AssetHelper.LoadAsset<Sprite>(address);
        title.GetComponent<Label>().text = focusedActor.characterName;

        // Format the actor's stats for display:
        var HP = $"{focusedActor.stats.HP,2}/{focusedActor.stats.MaxHP,-3}";
        var STR = $"{focusedActor.stats.Strength,4}";
        var VIT = $"{focusedActor.stats.Vitality,4}";
        var AGI = $"{focusedActor.stats.Agility,4}";
        var INT = $"{focusedActor.stats.Intelligence,4}";
        var WIS = $"{focusedActor.stats.Wisdom,4}";

        var LCK = $"{focusedActor.stats.Luck,4}";

        // Create a formatted stats table string.
        var stats =
            $"HP       STR  VIT  AGI  INT  WIS  LCK{Environment.NewLine}" +
            $"{HP}   {STR}{VIT}{AGI}{INT}{WIS}{LCK}{Environment.NewLine}";

        // Assign the details textarea combining the stats table with extra details from DataManager.
        details.GetComponent<Label>().text = stats + ActorRepo.Actors[focusedActor.characterName].Details.Card;

        // Begin the slide-in animate for the portrait.
        TriggerSlideIn();
    }

    // TriggerSlideIn starts the coroutine to animate the portrait sliding in from offscreen.
    private void TriggerSlideIn()
    {
        StartCoroutine(SlideIn());
    }

    // Assign smoothly animates the portrait image from an offscreen position to its destination.
    private IEnumerator SlideIn()
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

        // Ensure the portrait is exactly at the destination position after the animate.
        portrait.anchoredPosition = destination;
    }

    // Clear resets the card UI to a hidden state, clearing all displayed actors.
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
}
