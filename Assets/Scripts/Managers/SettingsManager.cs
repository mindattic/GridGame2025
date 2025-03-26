using Assets.Scripts.Repositories;
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
    private Label title;
    private RectTransform scrollView;
    private RectTransform content;
    private VerticalLayoutGroup verticalLayoutGroup;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float spacing;
    private FadeInstance fade;


    private RectTransform actorPanMultiplier;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.Settings.Canvas2D).GetComponent<RectTransform>();
        title = GameObject.Find(ComponentHelper.StageSelect.Title).GetComponent<Label>();
        scrollView = GameObject.Find(ComponentHelper.Settings.ScrollView).GetComponent<RectTransform>();
        content = GameObject.Find(ComponentHelper.Settings.Content).GetComponent<RectTransform>();
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        fade = GameObject.Find(ComponentHelper.Settings.Fade).GetComponent<FadeInstance>();
        actorPanMultiplier = GameObject.Find(ComponentHelper.Settings.ActorPanMultiplier).GetComponent<RectTransform>();

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;

        title.fontSize = buttonHeight / 2;
        scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);

        spacing = 0.01f * screenHeight;
        verticalLayoutGroup.spacing = spacing;

        actorPanMultiplier.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        IEnumerator showConfirm()
        {
            ConfirmationDialog.Show(canvas2D, "Save changes?", onSubmit: (value) =>
            {
                if (value)
                {
                    Debug.Log("User said: " + value);

                    StartCoroutine(fade.Hide(SceneRepo.instance.LoadPreviousScene()));
                }
            });

            yield return null;
        }

        StartCoroutine(fade.Show(showConfirm()));
    }



    public void UpdateActorPanMultiplier()
    {
        var slider = actorPanMultiplier.GetComponent<Slider>();
        ProfileRepo.instance.CurrentProfile.Settings.ActorPanMultiplier = slider.value;
    }
}
