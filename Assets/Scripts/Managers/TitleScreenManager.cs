using Assets.Helper;
using Assets.Scripts.Repositories;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        StartCoroutine(fade.FadeIn());
    }
    public void OnContinueButtonClicked()
    {
        ProfileRepo.CurrentProfile.CurrentSave = ProfileRepo.CurrentProfile.LatestSave;
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.Game)));
    }

    public void OnLoadGameButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.SaveFileSelect)));
    }

    public void OnNewGameButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.ProfileCreate)));
    }

    public void OnSettingsButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.Settings)));
    }

    public void OnCreditsButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.Credits)));
    }

    public void OnChangeProfileButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.ProfileSelect)));
    }
}
