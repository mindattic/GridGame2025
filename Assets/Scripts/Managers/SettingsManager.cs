using Assets.Scripts.Store;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class SettingsManager : MonoBehaviour
{
    //Fields
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
        canvas2D = GameObject.Find(ComponentHelper.Settings.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        header = GameObject.Find(ComponentHelper.StageSelect.Header).GetComponent<Label>() ?? throw new UnityException("Label is null");
        scrollView = GameObject.Find(ComponentHelper.Settings.ScrollView).GetComponent<RectTransform>() ?? throw new UnityException("ScrollView is null");
        content = GameObject.Find(ComponentHelper.Settings.Content).GetComponent<Transform>() ?? throw new UnityException("Content is null");
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>() ?? throw new UnityException("VerticalLayoutGroup is null");
        fade = GameObject.Find(ComponentHelper.Settings.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;

        header.fontSize = buttonHeight / 2;
        scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);

        spacing = 0.01f * screenHeight;
        verticalLayoutGroup.spacing = spacing;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneHub.LoadPreviousScene()));
    }
}
