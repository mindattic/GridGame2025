using Assets.Scripts.Models;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
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
        foreach (var stage in DataManager.Stages)
        {
            AddButton(stage.Value.Name);
        }
        StartCoroutine(fade.FadeIn());
    }

    public void AddButton(string stageName)
    {
        //Instantiate the prefab as a child of the content
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Button_{stageName}";

        //Set the button size
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        //Set the button's click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnStageSelectButtonClicked(stageName));

        //Set the button text (using TextMeshProUGUI)
        TextMeshProUGUI label = instance.GetComponentInChildren<TextMeshProUGUI>();
        label.text = stageName;
    }

    private void OnStageSelectButtonClicked(string stageName)
    {
        var profileManager = GameObject.Find(Constants.ProfileManager).GetComponent<ProfileManager>();
        profileManager.currentProfile.Stage.CurrentStageName = stageName;
        SceneManager.LoadScene(SceneHelper.Game);
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        yield break;
    }

}
