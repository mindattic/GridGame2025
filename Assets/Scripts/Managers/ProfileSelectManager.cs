using Assets.Scripts.Repositories;
using Game.Models.Profile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class ProfileSelectManager : MonoBehaviour
{
    private GameObject buttonPrefab;
    private RectTransform canvas2D;
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

        buttonPrefab = PrefabRepo.instance.Prefabs["ScreenWidthButtonPrefab"];

        canvas2D = GameObject.Find(ComponentHelper.StageSelect.Canvas2D).GetComponent<RectTransform>();
        header = GameObject.Find(ComponentHelper.StageSelect.Title).GetComponent<Label>();
        scrollView = GameObject.Find(ComponentHelper.StageSelect.ScrollView).GetComponent<RectTransform>();
        content = GameObject.Find(ComponentHelper.StageSelect.Content).GetComponent<RectTransform>();
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        fade = GameObject.Find(ComponentHelper.StageSelect.Fade).GetComponent<FadeInstance>();

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;
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
        //Validate a current profile exists
        if (!ProfileRepo.instance.HasProfiles)
        {
            Debug.LogError("No profiles found.");
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    private void Reload()
    {
        //Clear existing content
        Clear();

        AddCreateNewProfileButton();

        //Add each profile as a button
        foreach (var item in ProfileRepo.instance.profiles.Values)
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
        ProfileRepo.instance.SelectProfile(key);
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.TitleScreen)));
    }

    private void OnCreateNewProfileButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.ProfileCreate)));
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadPreviousScene()));
    }
}
