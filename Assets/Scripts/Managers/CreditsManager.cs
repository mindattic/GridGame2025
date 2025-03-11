using Assets.Scripts.Store;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ComponentHelper;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;


public class CreditsManager : MonoBehaviour
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
    private Label credits;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.StageSelect.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        header = GameObject.Find(ComponentHelper.StageSelect.Header).GetComponent<Label>() ?? throw new UnityException("Label is null");
        scrollView = GameObject.Find(ComponentHelper.StageSelect.ScrollView).GetComponent<RectTransform>() ?? throw new UnityException("ScrollView is null");
        content = GameObject.Find(ComponentHelper.StageSelect.Content).GetComponent<Transform>() ?? throw new UnityException("Content is null");
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>() ?? throw new UnityException("VerticalLayoutGroup is null");
        credits = GameObject.Find(ComponentHelper.Settings.Credits).GetComponent<Label>() ?? throw new UnityException("Credits is null");
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        const string NL = "\r\n";
        string text 
            = $"{NL}{NL}" 
            + $"<size=80%>Game Design & Development</size>{NL}" 
            + $"<size=150%>Ryan DeBraal</size>{NL}{NL}" 
            + $"<size=80%>Typography</size>{NL}" 
            + $"<size=150%>Brian Willson</size> <size=50%>(Attic)</size>{NL}" 
            + $"<size=150%>Jonas Hecksher</size> <size=50%>(Play)</size>{NL}{NL}" 
            + $"<size=80%>Visual Effects</size>{NL}" 
            + $"<size=150%>Eric Wang</size>{NL}" + 
            "";
        credits.text = text;

        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadPreviousScene()));
    }

  
}
