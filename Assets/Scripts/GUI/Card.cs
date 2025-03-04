using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviors
{
    // The Card class manages the UI card display that shows details about a focused actor.
    // It handles initialization, assignment of data (such as portrait, name, and stats),
    // and provides an animation to slide the portrait into view.
    public class Card : MonoBehaviour
    {
        // Quick Reference Properties:
        // These properties provide shortcuts to access global settings and data from GameManager.
        protected float cardPortraitSize => GameManager.instance.cardPortraitSize; // Desired size for the card's portrait.
        protected DataManager dataManager => GameManager.instance.dataManager;       // Data manager for retrieving additional character details.
        protected ResourceManager resourceManager => GameManager.instance.resourceManager; // Resource manager to load assets like sprites.
        protected List<ActorInstance> actors => GameManager.instance.actors;         // List of all actor instances in the game.
        protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator; // UI element to highlight the focused actor.
        protected bool hasFocusedActor => GameManager.instance.hasFocusedActor;      // Check if an actor is currently focused.
        protected ActorInstance focusedActor => GameManager.instance.focusedActor;   // The currently focused actor.

        // Fields for UI elements and animation settings.
        RectTransform rectTransform;     // The RectTransform of this card, used for layout and positioning.
        GameObject backdrop;             // The backdrop GameObject of the card.
        GameObject portrait;             // The portrait GameObject showing the actor's image.
        GameObject title;                // The title GameObject displaying the actor's name.
        GameObject details;              // The details GameObject displaying actor stats and information.
        Image backdropImage;             // The Image component for the backdrop.
        Image portraitImage;             // The Image component for the portrait.
        TextMeshProUGUI titleText;       // Text component for the title.
        TextMeshProUGUI detailsText;     // Text component for detailed stats and description.
        Vector3 destination;             // Final destination position for the portrait during slide-in animation.
        Vector3 offscreenPosition;       // Starting offscreen position for the portrait.
        AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Easing curve for slide-in animation.
        float slideDuration = 0.5f;      // Duration of the slide-in animation in seconds.

        // Awake is called when the script instance is loaded.
        // This method initializes references to UI elements by finding them in the scene.
        private void Awake()
        {
            // Get the RectTransform component of the card.
            rectTransform = GetComponent<RectTransform>();

            // Find and assign UI GameObjects using predefined constant names.
            backdrop = GameObject.Find(Constants.CardBackdrop);
            backdropImage = backdrop.GetComponent<Image>();

            portrait = GameObject.Find(Constants.CardPortrait);
            portraitImage = portrait.GetComponent<Image>();

            title = GameObject.Find(Constants.CardTitle);
            titleText = title.GetComponent<TextMeshProUGUI>();

            details = GameObject.Find(Constants.CardDetails);
            detailsText = details.GetComponent<TextMeshProUGUI>();

            // Start with a cleared (hidden) card.
            Clear();
        }

        // Start is called before the first frame update.
        // It sets up the initial positions and sizes for UI elements.
        private void Start()
        {
            // The following commented code shows an example of configuring anchors and size,
            // but is not used in the current implementation.
            /*
            RectTransform rect;
            rect = backdrop.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(Screen.width, Screen.width * Constants.percent33);

            rect = portrait.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(Screen.width, Screen.width * Constants.percent33);
            */

            // Set the portrait's size according to the global cardPortraitSize.
            portraitImage.rectTransform.sizeDelta = new Vector2(cardPortraitSize, cardPortraitSize);

            // Calculate the destination position for the portrait's slide-in animation.
            var position = portraitImage.rectTransform.localPosition;
            float width = portraitImage.rectTransform.rect.width;
            float height = portraitImage.rectTransform.rect.height;
            // Destination is calculated so that the portrait centers within its rect.
            destination = new Vector3(position.x + width / 2, position.y + height / 2, position.z);

            // Define the offscreen starting position, just outside the screen width.
            offscreenPosition = new Vector3(Screen.width + width, destination.y, destination.z);
            // Initially position the portrait offscreen.
            portraitImage.rectTransform.localPosition = offscreenPosition;
        }

        // Assign populates the card with data from the currently focused actor.
        public void Assign()
        {
            // If no actor is focused, exit without making changes.
            if (!hasFocusedActor)
                return;

            // Enable the backdrop and portrait images.
            backdropImage.enabled = true;
            portraitImage.sprite = resourceManager.Portrait(focusedActor.character.ToString()).Value.ToSprite();
            portraitImage.enabled = true;

            // Extract the actor's name (splitting at underscore to simplify if needed).
            titleText.text = focusedActor.name.Split("_")[0];

            // Format the actor's stats for display:
            var hp = $"{focusedActor.stats.HP,2}/{focusedActor.stats.MaxHP,-3}"; // HP/MaxHP with proper spacing.
            var str = $"{focusedActor.stats.Strength,4}";                // Right-align Strength in 4 characters.
            var vit = $"{focusedActor.stats.Vitality,4}";                // Right-align Vitality.
            var agi = $"{focusedActor.stats.Agility,4}";                 // Right-align Agility.
            var spd = $"{focusedActor.stats.Speed,4}";                   // Right-align Speed.
            var lck = $"{focusedActor.stats.Luck,4}";                    // Right-align Luck.

            // Create a formatted stats table string.
            var stats =
                $"HP       STR  VIT  AGI  SPD  LCK{Environment.NewLine}" +
                $"{hp}   {str}{vit}{agi}{spd}{lck}{Environment.NewLine}";

            // Set the details text combining the stats table with extra details from DataManager.
            detailsText.text = stats + dataManager.GetDetails(focusedActor.character).Card;

            // Begin the slide-in animation for the portrait.
            TriggerSlideIn();
        }

        // TriggerSlideIn starts the coroutine to animate the portrait sliding in from offscreen.
        private void TriggerSlideIn()
        {
            StartCoroutine(SlideIn());
        }

        // SlideIn smoothly animates the portrait image from an offscreen position to its destination.
        private IEnumerator SlideIn()
        {
            float elapsedTime = 0f;

            // Animate over the duration specified by slideDuration.
            while (elapsedTime < slideDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / slideDuration);
                // Evaluate the easing curve to get a smooth transition.
                float curveValue = slideInCurve.Evaluate(progress);

                // Lerp (linearly interpolate) the portrait's position from offscreen to destination.
                portraitImage.rectTransform.localPosition = Vector3.Lerp(offscreenPosition, destination, curveValue);

                yield return Wait.OneTick(); // Wait for the next frame.
            }

            // Ensure the portrait is exactly at the destination position after the animation.
            portraitImage.rectTransform.localPosition = destination;
        }

        // Clear resets the card UI to a hidden state, clearing all displayed data.
        public void Clear()
        {
            // Disable visual components of the card.
            backdropImage.enabled = false;
            portraitImage.enabled = false;
            titleText.text = "";
            detailsText.text = "";

            // Optionally, disable selection boxes on all actors or update the focus indicator.
            // actors.ForEach(x => x.render.SetSelectionBoxEnabled(false));
            // focusIndicator.Assign();

            // Reset the portrait's position to the offscreen starting position.
            portraitImage.rectTransform.localPosition = offscreenPosition;
        }
    }
}
