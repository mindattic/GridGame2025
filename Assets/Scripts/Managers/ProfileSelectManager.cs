using Assets.Scripts.Models;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ProfileSelectManager : MonoBehaviour
{

    //Fields
    [SerializeField] public GameObject buttonPrefab;
    private RectTransform canvas2D;
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
        content = GameObject.Find(ComponentHelper.StageSelect.Content).GetComponent<Transform>() ?? throw new UnityException("Content is null");
        fade = GameObject.Find(ComponentHelper.StageSelect.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>() ?? throw new UnityException("VerticalLayoutGroup is null");

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;

        spacing = 0.01f * screenHeight;
        verticalLayoutGroup.spacing = spacing;
    }

    private void Start()
    {
        foreach (var profile in ProfileManager.profiles)
        {
            AddButton(profile.Key);
        }
        StartCoroutine(fade.FadeIn());
    }

    public void AddButton(string profileKey)
    {
        // Instantiate the prefab as a child of the content
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Profile_{profileKey}";

        // Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        // Set the button's click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnProfileButtonClicked(profileKey));

        // Set the button text (using TextMeshProUGUI)
        TextMeshProUGUI label = instance.GetComponentInChildren<TextMeshProUGUI>();
        label.text = profileKey;
    }

    private void OnProfileButtonClicked(string profileName)
    {
        ProfileManager.currentProfile = ProfileManager.profiles[profileName];
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Title)));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        yield break;
    }

}
