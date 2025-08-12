using Assets.Helper;
using UnityEngine;
using scene = Assets.Helpers.SceneHelper;
using Label = TMPro.TextMeshProUGUI;
using Assets.Helpers;

public class TitleScreenManager : MonoBehaviour
{
    //Fields
    private RectTransform canvas;
    private RectTransform panel;
    private RectTransform continueButton;
    private RectTransform loadGameButton;
    private RectTransform settingsButton;
    private RectTransform creditsButton;
    private RectTransform profileButton;
    private RectTransform profileButtonLabel;

    private void Awake()
    {
        //Verify that game is ready to run
        if (!ProfileHelper.HasProfiles())
            return;

        profileButtonLabel = GameObject.Find(GameObjectHelper.TitleScreen.ProfileButtonLabel).GetComponent<RectTransform>();
        profileButtonLabel.GetComponent<Label>().text = ProfileHelper.CurrentProfile.Key;
    }

    private void Start()
    {
        scene.FadeIn();
    }

    public void OnContinueButtonClicked()
    {
        ProfileHelper.CurrentProfile.CurrentSave = ProfileHelper.CurrentProfile.LatestSave;
        scene.Change.ToGame();
    }

    public void OnLoadGameButtonClicked()
    {
        scene.Change.ToSaveFileSelect();
    }

    public void OnNewGameButtonClicked()
    {
        scene.Change.ToProfileCreate();
    }

    public void OnSettingsButtonClicked()
    {
        scene.Change.ToSettings();
    }

    public void OnCreditsButtonClicked()
    {
        scene.Change.ToCredits();
    }

    public void OnChangeProfileButtonClicked()
    {
        scene.Change.ToProfileSelect();
    }
}
