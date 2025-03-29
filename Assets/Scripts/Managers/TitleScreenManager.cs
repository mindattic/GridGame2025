using Assets.Scripts.Repositories;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;

public class TitleScreenManager : MonoBehaviour
{
    //Fields
    private RectTransform canvas2D;
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
        if (!ProfileRepo.instance.HasProfiles)
        {
            SceneManager.LoadScene(SceneHelper.ProfileCreate);
            return;
        }

        canvas2D = GameObject.Find(ComponentHelper.TitleScreen.Canvas2D).GetComponent<RectTransform>();

        profileButtonLabel = GameObject.Find(ComponentHelper.TitleScreen.ProfileButtonLabel).GetComponent<RectTransform>();
        fade = GameObject.Find(ComponentHelper.TitleScreen.Fade).GetComponent<FadeInstance>();

        profileButtonLabel.GetComponent<Label>().text = ProfileRepo.instance.CurrentProfile.Key;

        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueButtonClicked()
    {
        ProfileRepo.instance.CurrentProfile.CurrentSave = ProfileRepo.instance.CurrentProfile.LatestSave;
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.Game)));
    }

    public void OnLoadGameButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.SaveFileSelect)));
    }

    public void OnNewGameButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.ProfileCreate)));
    }

    public void OnSettingsButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.Settings)));
    }

    public void OnCreditsButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.Credits)));
    }

    public void OnChangeProfileButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.ProfileSelect)));
    }
}
