using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;

public class SettingsManager : MonoBehaviour
{
    //Fields
    private RectTransform canvas;
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
        canvas = GameObject.Find(GameObjectHelper.Settings.Canvas).GetComponent<RectTransform>();
        fade = GameObject.Find(GameObjectHelper.Settings.Fade).GetComponent<FadeInstance>();
        actorPanMultiplier = GameObject.Find(GameObjectHelper.Settings.ActorPanMultiplier).GetComponent<RectTransform>();

    }
    private void Start()
    {
        StartCoroutine(fade.FadeInRoutine());
    }

    public void OnBackButtonClicked()
    {
        IEnumerator showConfirmRoutine()
        {
            ConfirmationDialog.Show("Save changes?", onSubmit: (value) =>
            {
                if (value)
                {
                    Debug.Log("User said: " + value);

                    StartCoroutine(fade.HideRoutine(SceneHelper.LoadPreviousScene()));
                }
            });

            yield return Wait.None();
        }

        StartCoroutine(fade.ShowRoutine(showConfirmRoutine()));
    }



    public void UpdateActorPanMultiplier()
    {
        var slider = actorPanMultiplier.GetComponentInChildren<Slider>();
        ProfileRepo.CurrentProfile.Settings.ActorPanMultiplier = slider.value;
    }
}
