using Assets.Scripts.Models;
using Assets.Scripts.Repositories;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Intermission.Before;
using static UnityEngine.Rendering.DebugUI.Table;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class PartyManager : MonoBehaviour
{
    private GameObject slidePrefab;

    private RectTransform title;
    private RectTransform rosterPanel;
    private float spacing = 0f;
    private float deceleration = 1250;
    private float maxFocus = 3000f;
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
    private Label addRemovePartyMemberLabel;
    private Label partyMemberCountLabel;

    //private StatsDisplay statsDisplay;
    private Dictionary<RectTransform, Coroutine> barAnimations = new();
    private RectTransform levelRow;
    private RectTransform hpRow;
    private RectTransform strRow;
    private RectTransform vitRow;
    private RectTransform agiRow;
    private RectTransform spdRow;
    private RectTransform lckRow;
    private float centeredX;


    private FadeInstance fade;


    private bool IsInParty(string character)
    {
        return ProfileRepo.CurrentProfile.CurrentSave.Party.Members.Any(x => x.Character == character);
    }

    //Properties
    private int partyMemberCount => ProfileRepo.CurrentProfile.CurrentSave.Party.Members.Count;


    private void Awake()
    {
        //Validate a current profile exists
        if (!ProfileRepo.HasCurrentProfile)
        {
            Debug.LogError("No current profile selected.");
            SceneManager.LoadScene(SceneHelper.ProfileCreate);         
            return;
        }

        //Validate a current save exists
        if (!ProfileRepo.HasCurrentSave)
        {
            Debug.LogError("No current save selected.");
            SceneManager.LoadScene(SceneHelper.SaveFileSelect);
            return;
        }

        slidePrefab = PrefabRepo.Prefabs["RosterSlidePrefab"];

        title = GameObject.Find(GameObjectHelper.PartyManager.Title).GetComponent<RectTransform>();
        rosterPanel = GameObject.Find(GameObjectHelper.PartyManager.RosterPanel).GetComponent<RectTransform>();
        addRemovePartyMemberButton = GameObject.Find(GameObjectHelper.PartyManager.AddRemovePartyMemberButton).GetComponent<RectTransform>();
        addRemovePartyMemberLabel = GameObject.Find(GameObjectHelper.PartyManager.AddRemovePartyMemberButtonLabel).GetComponent<Label>();
        partyMemberCountLabel = GameObject.Find(GameObjectHelper.PartyManager.PartyMemberCountLabel).GetComponent<Label>();
        //statsDisplay = GameObject.Find(GameObjectHelper.PartyManager.StatsDisplay).GetComponent<StatsDisplay>();


        var statsDisplay = GameObject.Find(GameObjectHelper.PartyManager.StatsDisplay).GetComponent<RectTransform>();
        var panel = statsDisplay.transform.GetChild("Panel").GetComponent<RectTransform>();
        levelRow = panel.transform.GetChild("LVL").GetComponent<RectTransform>();
        hpRow = panel.transform.GetChild("HP").GetComponent<RectTransform>();
        strRow = panel.transform.GetChild("STR").GetComponent<RectTransform>();
        vitRow = panel.transform.GetChild("VIT").GetComponent<RectTransform>();
        agiRow = panel.transform.GetChild("AGI").GetComponent<RectTransform>();
        spdRow = panel.transform.GetChild("SPD").GetComponent<RectTransform>();
        lckRow = panel.transform.GetChild("LCK").GetComponent<RectTransform>();

        Debug.Log(GameObject.Find("Fade")); // Should log null
        Debug.Log(GameObject.Find("PartyManager/Canvas2D/Fade")); // Should log the object


        fade = GameObject.Find("Fade").GetComponent<FadeInstance>();

        float parentWidth = statsDisplay.rect.width;
        float barBackWidth = levelRow.rect.width;
        centeredX = (parentWidth - barBackWidth) / 2;

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

    private void LoadRosterSlides()
    {
        var rosterMembers = ProfileRepo.CurrentProfile.CurrentSave.Roster.Members;
        foreach (var member in rosterMembers)
        {
            // Instantiate the slide prefab and retrieve the RosterSlideInstance script
            GameObject slide = Instantiate(slidePrefab, rosterPanel);
            var instance = slide.GetComponent<RosterSlideInstance>();

            // Assign the slide name
            slide.name = $"RosterSlide_{member.Character}";

            // Load the sprite asynchronously
            string address = $"Actor-Portraits/{member.Character}";
            var sprite = AssetHelper.LoadAsset<Sprite>(address);

            // Load the instance with all required variables
            instance.Initialize(
                key: member.Character,
                sprite: sprite,
                width: 512f,
                height: 512f,
                onClick: () => CenterOn(instance),
                isInParty: IsInParty(member.Character)
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
                velocity = Mathf.Clamp(deltaX / Time.deltaTime, -maxFocus, maxFocus);
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
        title.GetComponent<Label>().text = slide.Key;

        // Update the button text and functionality
        UpdateAddRemoveButton(slide.Key);

        // Update the stats display
        UpdateStatsDisplay(slide.Key);
    }

    private void UpdateStatsDisplay(string character)
    {
        var rosterMember = ProfileRepo.CurrentProfile.CurrentSave.Roster.Members.Where(x => x.Character == character).First();
        Load(rosterMember.Character, rosterMember.Level);
    }

    private void UpdatePartyMemberLabel(bool isInParty)
    {
        addRemovePartyMemberLabel.text = isInParty ? "Remove from Party" : "Add to Party";
    }

    private void UpdatePartyMemberCountLabel()
    {
        partyMemberCountLabel.text = $"{partyMemberCount}/{Constants.MaxPartyMemberCount}";
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

        ProfileRepo.AddToParty(characterName);
        UpdateAddRemoveButton(characterName); // Refresh button state
    }




    private void RemoveFromParty(string characterName)
    {
        ProfileRepo.RemoveFromParty(characterName);
        UpdateAddRemoveButton(characterName); // Refresh button state
    }

    public void AddItem(RosterSlideInstance slide)
    {
        if (!slides.ContainsKey(slide.Key))
        {
            slides.Add(slide.Key, slide);
        }
    }

    public void Load(string character, int level)
    {
        var actorData = ActorRepo.Actors[character];
        var stats = actorData.GetStats(level);

        // Update each stat row
        UpdateStatRow(levelRow, "LVL", stats.Level); // Assuming max level is 100
        UpdateStatRow(hpRow, "HP", stats.HP, stats.MaxHP);
        UpdateStatRow(strRow, "STR", stats.Strength); // Assuming max stat value is 100
        UpdateStatRow(vitRow, "VIT", stats.Vitality);
        UpdateStatRow(agiRow, "AGI", stats.Agility);
        UpdateStatRow(spdRow, "SPD", stats.Intelligence);
        UpdateStatRow(lckRow, "LCK", stats.Luck);
    }



    private void UpdateStatRow(RectTransform row, string label, float value, float maxValue = 99)
    {
        var labelComponent = row.Find("Label").GetComponent<Label>();
        labelComponent.text = label;

        var backImage = row.Find("Bar/Back").GetComponent<Image>();
        var fillImage = row.Find("Bar/Fill").GetComponent<Image>();
        var valueComponent = row.Find("Value").GetComponent<Label>();
        valueComponent.text = $"{value}";

        float targetWidth = backImage.rectTransform.rect.width * (value / maxValue);

        // Cancel any existing animate on this row
        if (barAnimations.TryGetValue(row, out Coroutine running))
            StopCoroutine(running);

        // Start new animate
        barAnimations[row] = StartCoroutine(AnimateBarFill(row, fillImage.rectTransform, targetWidth));

    }

    private IEnumerator AnimateBarFill(RectTransform row, RectTransform bar, float targetWidth)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        float startWidth = bar.sizeDelta.x;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newWidth = Mathf.Lerp(startWidth, targetWidth, t);
            bar.sizeDelta = new Vector2(newWidth, bar.sizeDelta.y);
            yield return null;
        }

        bar.sizeDelta = new Vector2(targetWidth, bar.sizeDelta.y);

        //Remove from dictionary
        barAnimations.Remove(row);
    }




    public void OnBackButtonClicked()
    {
        //Execute(fade.FadeOut(SceneRepo.LoadPreviousScene()));
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.Game)));
    }


    public void OnEquipmentTooltipAnchorClicked()
    {

        var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In tellus augue, iaculis quis neque at, malesuada consectetur leo. Vestibulum id nulla lobortis, ullamcorper nibh at, vestibulum justo. Etiam sed ipsum eget odio consequat vulputate. Phasellus consequat neque quam, ac pulvinar nulla consectetur quis. Nulla facilisi. Nulla semper, justo eget volutpat tempor, elit mauris convallis mi, nec pellentesque neque dui eu metus. Mauris non facilisis sapien, lacinia consequat nibh.";
        var tt = new TooltipSettings()
        {
            message = message,
            target = GameObject.Find("TestTooltip").GetComponent<RectTransform>(),
            placement = TooltipPlacement.Top,
            useFade = true,
            useTypewriter = true,
            autoDestroy = false,
            autoDestroyDelay = 2.5f,     
            textAlignment = TooltipTextAlignment.TopLeft
        };
        Tooltip.Show(tt);
    }


}
