using Assets.Helper;
using Assets.Scripts.Repositories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class StageSelectManager : MonoBehaviour
{
    //Fields
    private GameObject buttonPrefab;
    private Label header;
    private RectTransform scrollView;
    private Transform content;
    private VerticalLayoutGroup verticalLayoutGroup;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float spacing;
    private FadeInstance fade;

    private void Awake()
    {

        buttonPrefab = PrefabRepo.Prefabs["ScreenWidthButtonPrefab"];
        content = GameObject.Find(GameObjectHelper.StageSelect.Content).GetComponent<Transform>();

        fade = GameObject.Find(GameObjectHelper.StageSelect.Fade).GetComponent<FadeInstance>();

        //startX = canvas.rect.width;
        //startY = canvas.rect.height;

        //buttonWidth = 0.9f * startX;
        //buttonHeight = startY / 16f;

        //header.fontSize = buttonHeight / 2;
        //scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);

        //spacing = 0.01f * startY;
        //verticalLayoutGroup.spacing = spacing;

        foreach (var stage in StageRepo.Stages)
        {
            AddButton(stage.Value.Name);
        }
    }

    private void Start()
    {
        StartCoroutine(fade.FadeInRoutine());
    }

    public void AddButton(string stageName)
    {
        //Instantiate the prefab as a child of the content
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Button_{stageName}";
       
        //Show the button size
        //RectTransform buttonRect = instance.GetComponent<RectTransform>();
        //buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        //Show the button's click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnStageSelectButtonClicked(stageName));

        //Show the button textarea
        Label label = instance.GetComponentInChildren<Label>();
        label.text = stageName;
    }

    private void OnStageSelectButtonClicked(string stageName)
    {
        ProfileRepo.CurrentProfile.LatestSave.Stage.CurrentStage = stageName;
        StartCoroutine(fade.FadeOutRoutine(SceneRepo.LoadScene(SceneHelper.Game)));
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneRepo.LoadPreviousScene()));
    }

}
