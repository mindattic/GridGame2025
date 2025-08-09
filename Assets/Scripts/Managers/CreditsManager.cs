using Assets.Scripts.Repositories;
using UnityEngine;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;
using g = Assets.Helpers.GameHelper;

public class CreditsManager : MonoBehaviour
{
    //Fields
    private RectTransform title;
    private RectTransform scrollView;
    private RectTransform content;
    private RectTransform textarea;
    private FadeInstance fade;

    private void Awake()
    {
        //title = GameObject.Find(GameObjectHelper.Credits.Title).GetComponent<RectTransform>();
        //scrollView = GameObject.Find(GameObjectHelper.Credits.ScrollView).GetComponent<RectTransform>();
        //content = GameObject.Find(GameObjectHelper.Credits.Content).GetComponent<RectTransform>();
        textarea = GameObject.Find(GameObjectHelper.Credits.Textarea).GetComponent<RectTransform>();
        fade = GameObject.Find(GameObjectHelper.Credits.Fade).GetComponent<FadeInstance>();

        //var startX = canvas.rect.width;
        //var startY = canvas.rect.height;
        //var buttonWidth = 0.9f * startX;
        //var buttonHeight = startY / 16f;
        //var fontSize = buttonHeight / 2;
        //var rowSpacing = 0.01f * startY;

        //title.GetComponent<Label>().fontSize = fontSize;
        //scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);
        //content.GetComponent<VerticalLayoutGroup>().spacing = rowSpacing;

        const string NL = "\r\n";
        string text
            = $"{NL}{NL}"
            + $"<size=80%>Game Design & Development</size>{NL}"
            + $"<size=150%>Ryan DeBraal</size>{NL}{NL}"
            + $"<size=80%>Typography</size>{NL}"
            + $"<size=150%>Brian Willson</size> <size=50%>(Attic)</size>{NL}"
            + $"<size=150%>Jonas Hecksher</size> <size=50%>(SpawnPair)</size>{NL}{NL}"
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
            + g.CanvasRect.rect.height * 0.5f;

        textarea.sizeDelta = new Vector2(g.CanvasRect.rect.width, textareaHeight);
    }
    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadPreviousScene()));
    }


}
