using Assets.Scripts.Repositories;
using Game.Models.Profile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;
using Assets.Helper;

public class ProfileSelectManager : MonoBehaviour
{
    private GameObject buttonPrefab;
    private Label header;
    private RectTransform scrollView;
    private RectTransform content;
    private VerticalLayoutGroup verticalLayoutGroup;
    private FadeInstance fade;

    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float fontSize;
    private float rowSpacing;
   
    private void Awake()
    {

        buttonPrefab = PrefabRepo.Prefabs["ScreenWidthButtonPrefab"];
        header = GameObject.Find(GameObjectHelper.StageSelect.Title).GetComponent<Label>();
        scrollView = GameObject.Find(GameObjectHelper.StageSelect.ScrollView).GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.StageSelect.Content).GetComponent<RectTransform>();
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        fade = GameObject.Find(GameObjectHelper.StageSelect.Fade).GetComponent<FadeInstance>();

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
        StartCoroutine(fade.FadeIn());
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

        if (!ProfileRepo.HasProfiles())
            return;

        //Add each profile as a button
        foreach (var item in ProfileRepo.Profiles.Values)
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
        label.text = "Data New Profile";
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
        ProfileRepo.SelectProfile(key);
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.TitleScreen)));
    }

    private void OnCreateNewProfileButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.ProfileCreate)));
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadPreviousScene()));
    }
}
