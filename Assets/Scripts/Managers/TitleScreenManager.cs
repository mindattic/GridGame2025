using Assets.Helper;
using UnityEngine;
using Label = TMPro.TextMeshProUGUI;

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
    private FadeInstance fade;

    private void Awake()
    {
        //Verify that game is ready to run
        if (!ProfileRepo.HasProfiles())
            return;

        canvas = GameObject.Find(GameObjectHelper.TitleScreen.Canvas).GetComponent<RectTransform>();

        profileButtonLabel = GameObject.Find(GameObjectHelper.TitleScreen.ProfileButtonLabel).GetComponent<RectTransform>();
        fade = GameObject.Find(GameObjectHelper.TitleScreen.Fade).GetComponent<FadeInstance>();

        profileButtonLabel.GetComponent<Label>().text = ProfileRepo.CurrentProfile.Key;
    }

    private void Start()
    {
        StartCoroutine(fade.FadeInRoutine());
    }
    public void OnContinueButtonClicked()
    {
        ProfileRepo.CurrentProfile.CurrentSave = ProfileRepo.CurrentProfile.LatestSave;
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadGame()));
    }

    public void OnLoadGameButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadSaveFileSelect()));
    }

    public void OnNewGameButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadProfileCreate()));
    }

    public void OnSettingsButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadSettings()));
    }

    public void OnCreditsButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadCredits()));
    }

    public void OnChangeProfileButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadProfileSelect()));
    }
}
