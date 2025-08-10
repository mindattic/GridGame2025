using Assets.Helper;
using UnityEngine;
using f = Assets.Helpers.FadeOverlayHelper;
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

    private void Awake()
    {
        //Verify that game is ready to run
        if (!ProfileRepo.HasProfiles())
            return;

        profileButtonLabel = GameObject.Find(GameObjectHelper.TitleScreen.ProfileButtonLabel).GetComponent<RectTransform>();
        profileButtonLabel.GetComponent<Label>().text = ProfileRepo.CurrentProfile.Key;
    }

    private void Start()
    {
        f.Overlay.FadeIn();
    }

    public void OnContinueButtonClicked()
    {
        ProfileRepo.CurrentProfile.CurrentSave = ProfileRepo.CurrentProfile.LatestSave;
        f.Overlay.FadeOut(SceneHelper.LoadGame());
    }

    public void OnLoadGameButtonClicked()
    {
        f.Overlay.FadeOut(SceneHelper.LoadSaveFileSelect());
    }

    public void OnNewGameButtonClicked()
    {
        f.Overlay.FadeOut(SceneHelper.LoadProfileCreate());
    }

    public void OnSettingsButtonClicked()
    {
        f.Overlay.FadeOut(SceneHelper.LoadSettings());
    }

    public void OnCreditsButtonClicked()
    {
        f.Overlay.FadeOut(SceneHelper.LoadCredits());
    }

    public void OnChangeProfileButtonClicked()
    {
        f.Overlay.FadeOut(SceneHelper.LoadProfileSelect());
    }
}
