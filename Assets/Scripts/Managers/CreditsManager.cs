using Assets.Scripts.Repositories;
using UnityEngine;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;

public class CreditsManager : MonoBehaviour
{
    //Fields
    private RectTransform canvas2D;
    private RectTransform headerBackdrop;
    private RectTransform header;
    private RectTransform scrollView;
    private RectTransform content;
    private RectTransform textarea;
    private Fade fade;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.Credits.Canvas2D).GetComponent<RectTransform>();
        headerBackdrop = GameObject.Find(ComponentHelper.Credits.Header).GetComponent<RectTransform>();
        header = GameObject.Find(ComponentHelper.Credits.Header).GetComponent<RectTransform>();
        scrollView = GameObject.Find(ComponentHelper.Credits.ScrollView).GetComponent<RectTransform>();
        content = GameObject.Find(ComponentHelper.Credits.Content).GetComponent<RectTransform>();
        textarea = GameObject.Find(ComponentHelper.Credits.Textarea).GetComponent<RectTransform>();
        fade = GameObject.Find(ComponentHelper.Credits.Fade).GetComponent<Fade>();

        var screenWidth = canvas2D.rect.width;
        var screenHeight = canvas2D.rect.height;
        var buttonWidth = 0.9f * screenWidth;
        var buttonHeight = screenHeight / 16f;
        var fontSize = buttonHeight / 2;
        var rowSpacing = 0.01f * screenHeight;

        header.GetComponent<Label>().fontSize = fontSize;
        scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);
        content.GetComponent<VerticalLayoutGroup>().spacing = rowSpacing;

        const string NL = "\r\n";
        string text
            = $"{NL}{NL}"
            + $"<size=80%>Game Design & Development</size>{NL}"
            + $"<size=150%>Ryan DeBraal</size>{NL}{NL}"
            + $"<size=80%>Typography</size>{NL}"
            + $"<size=150%>Brian Willson</size> <size=50%>(Attic)</size>{NL}"
            + $"<size=150%>Jonas Hecksher</size> <size=50%>(Play)</size>{NL}{NL}"
            + $"<size=80%>Visual Effects</size>{NL}"
            + $"<size=150%>Eric Wang</size>{NL}"
            + $"{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}"
            + $"{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}"
            + $"{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}"
            + $"{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}"
            + $"{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}{NL}"
            + $"Thanks for playing!";
        var label = textarea.GetComponent<Label>();
        label.text = text;
        label.ForceMeshUpdate();

        var textareaHeight
            = label.textInfo.lineCount
            * label.textInfo.lineInfo[0].lineHeight
            + (screenHeight * 0.25f);

        textarea.sizeDelta = new Vector2(screenWidth, textareaHeight);

        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadPreviousScene()));
    }


}
