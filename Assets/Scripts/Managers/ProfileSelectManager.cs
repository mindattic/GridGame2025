using Assets.Helper;
using Game.Models.Profile;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using scene = Assets.Helpers.SceneHelper;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;
using Assets.Helpers;

public class ProfileSelectManager : MonoBehaviour
{
    private GameObject buttonPrefab;
    private Label header;
    private RectTransform scrollView;
    private RectTransform content;
    private VerticalLayoutGroup verticalLayoutGroup;

    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float fontSize;
    private float rowSpacing;

    private void Awake()
    {

        buttonPrefab = PrefabLibrary.Prefabs["ScreenWidthButtonPrefab"];
        header = GameObject.Find(GameObjectHelper.StageSelect.Title).GetComponent<Label>();
        scrollView = GameObject.Find(GameObjectHelper.StageSelect.ScrollView).GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.StageSelect.Content).GetComponent<RectTransform>();
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();

        screenWidth = c.CanvasRect.rect.width;
        screenHeight = c.CanvasRect.rect.height;
        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;
        fontSize = buttonHeight / 2;
        rowSpacing = 0.01f * screenHeight;

        header.fontSize = fontSize;
        scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);

        verticalLayoutGroup.spacing = rowSpacing;
    }

    private void Start()
    {
        Reload();
        scene.FadeIn();
    }

    private void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    private void Reload()
    {
        //Hide existing content
        Clear();

        AddCreateNewProfileButton();

        if (!ProfileHelper.HasProfiles())
            return;

        //Add each profile as a button
        foreach (var item in ProfileHelper.Profiles.Values)
        {
            AddProfileSelectButton(item);
        }
    }

    public void AddCreateNewProfileButton()
    {
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = "CreateNewProfile";

        //RectTransform buttonRect = instance.GetComponent<RectTransform>();
        //buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnCreateNewProfileButtonClicked());

        Label label = instance.GetComponentInChildren<Label>();
        label.text = "Create New Profile";
    }

    public void AddProfileSelectButton(Profile item)
    {
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Profile_{item.Key}";

        //RectTransform buttonRect = instance.GetComponent<RectTransform>();
        // buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnProfileButtonClicked(item.Key));

        Label label = instance.GetComponentInChildren<Label>();
        label.text = item.Key;
    }

    private void OnProfileButtonClicked(string key)
    {
        ProfileHelper.SelectProfile(key);
        scene.Change.ToTitleScreen();
    }

    private void OnCreateNewProfileButtonClicked()
    {
        scene.Change.ToProfileCreate();
    }

    public void OnBackButtonClicked()
    {
        scene.Change.ToPreviousScene();
    }
}
