using Assets.Scripts.Models;
using Assets.Scripts.Repositories;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Intermission.Before;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class PartyManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slidePrefab;

    private Label title;
    private RectTransform rosterPanel;
    private float spacing = 0f;
    private float deceleration = 1250;
    private float maxSpeed = 3000f;
    private float scrollSpeed = 10f;
    private float dragThreshold = 15f;
    private float wrapThresholdMultiplier = 1.5f;

    private Dictionary<string, RosterSlideInstance> slides = new Dictionary<string, RosterSlideInstance>();

    private Vector2 touchStart;
    private bool dragging = false;
    private float velocity = 0f;
    private float targetOffset = 0f;
    private bool scrollingToCenter = false;
    private bool clickAllowed = true;


    private RectTransform addRemovePartyMemberButton;
    private RectTransform addRemovePartyMemberLabel;
    private RectTransform addRemovePartyMemberCount;

  
    private FadeInstance fade;


    private bool IsInParty(string characterName)
    {
        return ProfileRepo.instance.CurrentProfile.CurrentSave.Party.HeroActors.Any(hero => hero.Character == characterName);
    }

    //Properties
    private int partyMemberCount => ProfileRepo.instance.CurrentProfile.CurrentSave.Party.HeroActors.Count;


    private void Awake()
    {
        //Validate a current profile exists
        if (!ProfileRepo.instance.HasCurrentProfile || !ProfileRepo.instance.HasCurrentSave)
        {
            Debug.LogError("No current profile selected.");
            return;
        }

        //Validate a current save exists
        if (!ProfileRepo.instance.HasCurrentSave)
        {
            Debug.LogError("No current save selected.");
            return;
        }

        title = GameObject.Find(ComponentHelper.PartyManager.Title).GetComponent<Label>();
        rosterPanel = GameObject.Find(ComponentHelper.PartyManager.RosterPanel).GetComponent<RectTransform>();
        addRemovePartyMemberButton = GameObject.Find(ComponentHelper.PartyManager.AddRemovePartyMemberButton).GetComponent<RectTransform>();
        addRemovePartyMemberLabel = GameObject.Find(ComponentHelper.PartyManager.AddRemovePartyMemberButtonLabel).GetComponent<RectTransform>();
        addRemovePartyMemberCount = GameObject.Find(ComponentHelper.PartyManager.AddRemovePartyMemberButtonCount).GetComponent<RectTransform>();
        fade = GameObject.Find(ComponentHelper.PartyManager.Fade).GetComponent<FadeInstance>();

        UpdatePartyMemberCountLabel();
        LoadRosterSlides();

    }

    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    private void Update()
    {
        HandleTouch();

        if (!dragging && Mathf.Abs(velocity) > 0.1f)
        {
            float delta = velocity * Time.deltaTime;
            MoveSlides(delta);
            float decel = deceleration * Time.deltaTime;
            velocity = velocity > 0 ? Mathf.Max(0, velocity - decel) : Mathf.Min(0, velocity + decel);
        }

        if (scrollingToCenter)
        {
            float move = Mathf.Lerp(0, targetOffset, 10f * Time.deltaTime);
            MoveSlides(move);
            targetOffset -= move;

            if (Mathf.Abs(targetOffset) < 0.5f)
            {
                scrollingToCenter = false;
                targetOffset = 0f;
            }
        }

        WrapSlides();
    }

    private async void LoadRosterSlides()
    {
        string[] sprites = { "Barbarian", "Cleric", "GreenNinja", "Paladin", "Pugilist", "RedNinja", "Ronin", "Sellsword", "Thief", "Vampire" };
        for (int i = 0; i < sprites.Length; i++)
        {
            // Instantiate the slide prefab and retrieve the RosterSlideInstance script
            GameObject slide = Instantiate(slidePrefab, rosterPanel);
            var instance = slide.GetComponent<RosterSlideInstance>();

            // Set the slide name
            slide.name = $"RosterSlide_{sprites[i]}";

            // Load the sprite asynchronously
            string address = $"Actor-Portraits/{sprites[i]}";
            var sprite = await AssetHelper.LoadSpriteAsync(address);

            // Initialize the instance with all required variables
            instance.Initialize(
                key: sprites[i],
                sprite: sprite,
                width: 512f,
                height: 512f,
                onClick: () => CenterOn(instance),
                isInParty: IsInParty(sprites[i])
            );

            // Add the instance to the roster
            AddItem(instance);
        }

        RepositionSlides();
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rosterPanel, touch.position, null, out Vector2 localPoint)) return;
            if (!rosterPanel.rect.Contains(localPoint)) return;

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                dragging = true;
                velocity = 0f;
                clickAllowed = true;
            }
            else if (touch.phase == TouchPhase.Moved && dragging)
            {
                Vector2 current = touch.position;
                float deltaX = current.x - touchStart.x;

                if (Mathf.Abs(deltaX) > dragThreshold)
                    clickAllowed = false;

                MoveSlides(deltaX);
                velocity = Mathf.Clamp(deltaX / Time.deltaTime, -maxSpeed, maxSpeed);
                touchStart = current;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                dragging = false;
            }
        }
    }

    private void MoveSlides(float deltaX)
    {
        foreach (var item in slides.Values)
        {
            Vector3 pos = item.rectTransform.anchoredPosition;
            pos.x += deltaX;
            item.rectTransform.anchoredPosition = pos;
        }
    }

    private void WrapSlides()
    {
        var itemList = slides.Values.ToList();

        for (int i = 0; i < itemList.Count; i++)
        {
            var item = itemList[i];
            float width = item.Width;
            float totalWidth = width + spacing;
            Vector3 pos = item.rectTransform.anchoredPosition;

            if (pos.x < -totalWidth * wrapThresholdMultiplier)
            {
                float rightMostX = GetRightmostX();
                pos.x = rightMostX + totalWidth;
                item.rectTransform.anchoredPosition = pos;
            }
            else if (pos.x > totalWidth * (itemList.Count - wrapThresholdMultiplier))
            {
                float leftMostX = GetLeftmostX();
                pos.x = leftMostX - totalWidth;
                item.rectTransform.anchoredPosition = pos;
            }
        }
    }

    private float GetLeftmostX()
    {
        float min = float.MaxValue;
        foreach (var item in slides.Values)
            min = Mathf.Min(min, item.rectTransform.anchoredPosition.x);
        return min;
    }

    private float GetRightmostX()
    {
        float max = float.MinValue;
        foreach (var item in slides.Values)
            max = Mathf.Max(max, item.rectTransform.anchoredPosition.x);
        return max;
    }

    private void RepositionSlides()
    {
        float x = 0;
        foreach (var slide in slides.Values)
        {
            float width = slide.Width;
            slide.rectTransform.anchoredPosition = new Vector2(x, 0);
            x += width + spacing;
        }
    }

    public void CenterOn(RosterSlideInstance slide)
    {
        if (!clickAllowed) return;

        float offset = slide.rectTransform.anchoredPosition.x;
        if (slide.rectTransform.parent != rosterPanel)
            offset += slide.rectTransform.parent.GetComponent<RectTransform>().anchoredPosition.x;

        targetOffset = -offset;
        scrollingToCenter = true;

        // Update the title
        title.text = slide.Key;

        // Update the button text and functionality
        UpdateAddRemoveButton(slide.Key);
    }


    private void UpdatePartyMemberLabel(bool isInParty)
    {
        addRemovePartyMemberLabel.GetComponent<Label>().text = isInParty ? "Remove from Party" : "Add to Party";
    }

    private void UpdatePartyMemberCountLabel()
    {
        addRemovePartyMemberCount.GetComponent<Label>().text = $"{partyMemberCount}/{Constants.MaxPartyMemberCount}";
    }

    private void UpdateSlideCheckmark(string characterName, bool isInParty)
    {
        // Update the checkmark for the slide
        if (slides.TryGetValue(characterName, out var slide))
        {
            slide.SetCheckmark(isInParty);
        }
    }

    private void UpdateAddRemoveButton(string characterName)
    {
        bool isInParty = IsInParty(characterName);
        UpdatePartyMemberLabel(isInParty);
        UpdatePartyMemberCountLabel();
        UpdateSlideCheckmark(characterName, isInParty);

        // Update the button functionality
        var button = addRemovePartyMemberButton.GetComponent<Button>();
        button.onClick.RemoveAllListeners(); // Clear previous listeners
        if (isInParty)
        {
            button.onClick.AddListener(() => RemoveFromParty(characterName));
        }
        else
        {
            button.onClick.AddListener(() => AddToParty(characterName));
        }
    }

    private void AddToParty(string characterName)
    {
        if (partyMemberCount >= Constants.MaxPartyMemberCount)
        {
            Debug.LogWarning($"Cannot add more than {Constants.MaxPartyMemberCount} members to the party.");
            return;
        }

        ProfileRepo.instance.AddToParty(characterName);
        UpdateAddRemoveButton(characterName); // Refresh button state
    }




    private void RemoveFromParty(string characterName)
    {
        ProfileRepo.instance.RemoveFromParty(characterName);
        UpdateAddRemoveButton(characterName); // Refresh button state
    }

    public void AddItem(RosterSlideInstance slide)
    {
        if (!slides.ContainsKey(slide.Key))
        {
            slides.Add(slide.Key, slide);
        }
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadPreviousScene()));
    }
}
