using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;
using Label = TMPro.TextMeshProUGUI;
using Assets.Helpers;

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
    private RectTransform actorPanMultiplier;

    private void Awake()
    {
        actorPanMultiplier = GameObject.Find(GameObjectHelper.Settings.ActorPanMultiplier).GetComponent<RectTransform>();

    }
    private void Start()
    {
        scene.FadeIn();
    }

    public void OnBackButtonClicked()
    {
        //IEnumerator showConfirmRoutine()
        //{
        //    ConfirmationDialog.Show("Save changes?", onSubmit: (value) =>
        //    {
        //        if (value)
        //        {
        //            Debug.Log("User said: " + value);

        //            scene.Change.ToPreviousScene();
        //        }
        //    });

        //    yield return Wait.None();
        //}

        scene.Change.ToPreviousScene();
    }



    public void UpdateActorPanMultiplier()
    {
        var slider = actorPanMultiplier.GetComponentInChildren<Slider>();
        ProfileHelper.CurrentProfile.Settings.ActorPanMultiplier = slider.value;
    }
}
