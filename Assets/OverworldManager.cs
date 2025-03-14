using Assets.Scripts.Store;
using Game.Models;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class OverworldManager : MonoBehaviour
{
    //Fields
    [SerializeField] public GameObject stageIconPrefab;
    private Label header;
    private RectTransform canvas2D;
    private RectTransform scrollView;
    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private RectTransform map;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private Fade fade;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.Overworld.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        header = GameObject.Find(ComponentHelper.Overworld.Header).GetComponent<Label>() ?? throw new UnityException("Label is null");
        scrollView = GameObject.Find(ComponentHelper.Overworld.ScrollView).GetComponent<RectTransform>() ?? throw new UnityException("ScrollView is null");
        scrollRect = GameObject.Find(ComponentHelper.Overworld.ScrollView).GetComponent<ScrollRect>() ?? throw new UnityException("ScrollRect is null");
        viewport = GameObject.Find(ComponentHelper.Overworld.Viewport).GetComponent<RectTransform>() ?? throw new UnityException("Viewport is null");
        content = GameObject.Find(ComponentHelper.Overworld.Content).GetComponent<RectTransform>() ?? throw new UnityException("Content is null");
        map = GameObject.Find(ComponentHelper.Overworld.Map).GetComponent<RectTransform>() ?? throw new UnityException("Map is null");
        fade = GameObject.Find(ComponentHelper.Overworld.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        buttonWidth = 64;
        buttonHeight = 32;

        header.fontSize = screenHeight / 16f / 2;
        scrollView.sizeDelta = new Vector2(screenWidth, screenHeight);

        //scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);


    }

    private void Start()
    {
        AddStageIcon(new Vector2(1280, -2840), "Stage 1");
        CenterOnPosition(new Vector2(1280, -2840));

        StartCoroutine(fade.FadeIn());
    }


    private void AddStageIcon(Vector2 position, string stageName)
    {
        // Instantiate the prefab as a child of the content
        GameObject instance = Instantiate(stageIconPrefab, content);
        instance.name = $"StageIcon_{stageName}";

        // Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        // Set the button's click event
        Button button = instance.GetComponent<Button>();
        button.transform.localPosition = position;
        button.onClick.AddListener(() => OnStageSelectButtonClicked(stageName));

        // Set the button text
        Label label = instance.GetComponentInChildren<Label>();
        label.text = stageName;
    }


    private void OnStageSelectButtonClicked(string stageName)
    {
        ProfileStore.instance.selectedProfile.Stage.CurrentStageName = stageName;
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Game)));
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadPreviousScene()));
    }


    public void CenterOnPosition(Vector2 targetLocalPosition)
    {

        // Convert local position into normalized position
        Vector2 viewportSize = viewport.rect.size;
        Vector2 contentSize = content.rect.size;
      
        // Normalize position (0 = top/left, 1 = bottom/right)
        float normalizedX = Mathf.Clamp01((targetLocalPosition.x - viewportSize.x / 2) / (contentSize.x - viewportSize.x));
        float normalizedY = Mathf.Clamp01((targetLocalPosition.y - viewportSize.y / 2) / (contentSize.y - viewportSize.y));

        // Apply the new scroll position
        scrollRect.normalizedPosition = new Vector2(normalizedX, 1 - normalizedY);
    }


}
