using Assets.Scripts.Store;
using Game.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class ProfileSelectManager : MonoBehaviour
{
    //Fields
    [SerializeField] public GameObject buttonPrefab;
    private RectTransform canvas2D;
    private Label header;
    private RectTransform scrollView;
    private Transform content;
    private VerticalLayoutGroup verticalLayoutGroup;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float spacing;
    private Fade fade;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.StageSelect.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        header = GameObject.Find(ComponentHelper.StageSelect.Header).GetComponent<Label>() ?? throw new UnityException("Label is null");
        scrollView = GameObject.Find(ComponentHelper.StageSelect.ScrollView).GetComponent<RectTransform>() ?? throw new UnityException("ScrollView is null");
        content = GameObject.Find(ComponentHelper.StageSelect.Content).GetComponent<Transform>() ?? throw new UnityException("Content is null");    
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>() ?? throw new UnityException("VerticalLayoutGroup is null");
        fade = GameObject.Find(ComponentHelper.StageSelect.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;
    
        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;

        header.fontSize = buttonHeight / 2;
        scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);

        spacing = 0.01f * screenHeight;
        verticalLayoutGroup.spacing = spacing;
    }

    private void Start()
    {
        Reload(); 
        StartCoroutine(fade.FadeIn());
    }

    private void Reload()
    {
        //Clear existing content
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        AddCreateNewProfileButton();

        //Add each profile as a button
        foreach (var entry in ProfileStore.instance.profiles)
        {
            AddProfileSelectButton(entry.Value);
        }
    }

    public void AddCreateNewProfileButton()
    {
        // Instantiate the prefab as a child of the content
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = "CreateNewProfile";

        // Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        // Set the button's click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnCreateNewProfileButtonClicked());

        // Set the button text
        Label label = instance.GetComponentInChildren<Label>();
        label.text = "Create New Profile";
    }


    public void AddProfileSelectButton(Profile profile)
    {
        // Instantiate the prefab as a child of the content
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Profile_{profile.Name}";

        // Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        // Set the button's click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnProfileButtonClicked(profile.Name));

        // Set the button text
        Label label = instance.GetComponentInChildren<Label>();
        label.text = profile.Name;
    }


    private void OnProfileButtonClicked(string name)
    {
        ProfileStore.instance.Select(name);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Title)));
    }
    private void OnCreateNewProfileButtonClicked()
    {
        ProfileStore.instance.Create();
        ProfileStore.instance.Load();
        Reload();
    }


    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Title)));
    }



}
