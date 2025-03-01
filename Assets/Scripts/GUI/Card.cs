using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviors
{
    public class Card : MonoBehaviour
    {
       //Quick Reference Properties
        protected float cardPortraitSize => GameManager.instance.cardPortraitSize;
        protected DataManager dataManager => GameManager.instance.dataManager;
        protected ResourceManager resourceManager => GameManager.instance.resourceManager;
        protected List<ActorInstance> actors => GameManager.instance.actors;



        //Fields
        RectTransform rectTransform;
        GameObject backdrop;
        GameObject portrait;
        GameObject title;
        GameObject details;
        Image backdropImage;
        Image portraitImage;
        TextMeshProUGUI titleText;
        TextMeshProUGUI detailsText;
        Vector3 destination;
        Vector3 offscreenPosition;
        AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float slideDuration = 0.5f;

        //Method which is used for initialization tasks that need to occur before the game starts 
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            //Game Objects
            backdrop = GameObject.Find(Constants.CardBackdrop);
            backdropImage = backdrop.GetComponent<Image>();

            portrait = GameObject.Find(Constants.CardPortrait);
            portraitImage = portrait.GetComponent<Image>();

            title = GameObject.Find(Constants.CardTitle);
            titleText = title.GetComponent<TextMeshProUGUI>();

            details = GameObject.Find(Constants.CardDetails);
            detailsText = details.GetComponent<TextMeshProUGUI>();

            Reset();
        }

        //Method which is automatically called before the first frame update  
        private void Start()
        {
            //RectTransform rect;

            //rect = backdrop.GetComponent<RectTransform>();
            //rect.anchorMin = new Vector2(0.5f, 0.5f);
            //rect.anchorMax = new Vector2(0.5f, 0.5f);
            //rect.pivot = new Vector2(0.5f, 0.5f);
            //rect.sizeDelta = new Vector2(Screen.width, Screen.width * Constants.percent33);

            //rect = portrait.GetComponent<RectTransform>();
            //rect.anchorMin = new Vector2(0.5f, 0.5f);
            //rect.anchorMax = new Vector2(0.5f, 0.5f);
            //rect.pivot = new Vector2(0.5f, 0.5f);
            //rect.sizeDelta = new Vector2(Screen.width, Screen.width * Constants.percent33);


            portraitImage.rectTransform.sizeDelta = new Vector2(cardPortraitSize, cardPortraitSize);

            var position = portraitImage.rectTransform.localPosition;
            float width = portraitImage.rectTransform.rect.width;
            float height = portraitImage.rectTransform.rect.height;
            destination = new Vector3(position.x + width / 2, position.y + height / 2, position.z);

            offscreenPosition = new Vector3(Screen.width + width, destination.y, destination.z);
            portraitImage.rectTransform.localPosition = offscreenPosition;
        }

        public void Assign(ActorInstance actor)
        {
            backdropImage.enabled = true;
            portraitImage.sprite = resourceManager.Portrait(actor.character.ToString()).Value.ToSprite();
            portraitImage.enabled = true;

            titleText.text = actor.name.Split("_")[0];

            var hp = $"{actor.stats.HP,2}/{actor.stats.MaxHP,-3}"; //HP/MaxHP with dynamic padding
            var str = $"{actor.stats.Strength,4}";                //Right-align Stats to 4 characters
            var vit = $"{actor.stats.Vitality,4}";
            var agi = $"{actor.stats.Agility,4}";
            var spd = $"{actor.stats.Speed,4}";
            var lck = $"{actor.stats.Luck,4}";

            //Create the Stats table
            var stats
                = $"HP       STR  VIT  AGI  SPD  LCK{Environment.NewLine}"
                + $"{hp}   {str}{vit}{agi}{spd}{lck}{Environment.NewLine}";

            detailsText.text = stats + dataManager.GetDetails(actor.character).Card;

            TriggerSlideIn();
        }

        private void TriggerSlideIn()
        {
            StartCoroutine(SlideIn());
        }

        private IEnumerator SlideIn()
        {
            float elapsedTime = 0f;

            while (elapsedTime < slideDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / slideDuration);
                float curveValue = slideInCurve.Evaluate(progress);

                //Interpolate between offscreen and initial position
                portraitImage.rectTransform.localPosition = Vector3.Lerp(offscreenPosition, destination, curveValue);

                yield return Wait.OneTick();
            }

            //Ensure the portrait reaches the final position
            portraitImage.rectTransform.localPosition = destination;
        }

        public void Reset()
        {
            backdropImage.enabled = false;
            portraitImage.enabled = false;
            titleText.text = "";
            detailsText.text = "";

            //TriggerDespawn all selection boxes from actors
            actors.ForEach(x => x.render.SetSelectionBoxEnabled(false));

            //Initialize portrait position
            portraitImage.rectTransform.localPosition = offscreenPosition;
        }
    }
}
