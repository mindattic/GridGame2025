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
        fade = GameObject.Find(ComponentHelper.Settings.Fade).GetComponent<FadeInstance>();
        actorPanMultiplier = GameObject.Find(ComponentHelper.Settings.ActorPanMultiplier).GetComponent<RectTransform>();

    }
    private void Start()
    {
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

                    StartCoroutine(fade.Hide(SceneRepo.LoadPreviousScene()));
                }
            });

            yield return null;
        }

        StartCoroutine(fade.Show(showConfirm()));
    }



    public void UpdateActorPanMultiplier()
    {
        var slider = actorPanMultiplier.GetComponentInChildren<Slider>();
        ProfileRepo.CurrentProfile.Settings.ActorPanMultiplier = slider.value;
    }
}
