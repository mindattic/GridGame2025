using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using f = Assets.Helpers.FadeOverlayHelper;
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
    private RectTransform actorPanMultiplier;

    private void Awake()
    {
        actorPanMultiplier = GameObject.Find(GameObjectHelper.Settings.ActorPanMultiplier).GetComponent<RectTransform>();

    }
    private void Start()
    {
        f.Overlay.FadeIn();
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

                    f.Overlay.FadeOut(SceneHelper.LoadPreviousScene());
                }
            });

            yield return Wait.None();
        }

        f.Overlay.Show(showConfirmRoutine());
    }



    public void UpdateActorPanMultiplier()
    {
        var slider = actorPanMultiplier.GetComponentInChildren<Slider>();
        ProfileRepo.CurrentProfile.Settings.ActorPanMultiplier = slider.value;
    }
}
