using Assets.Scripts.Repositories;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;

public class TitleScreenManager : MonoBehaviour
{
    //Fields
    private RectTransform canvas2D;
    private RectTransform mainMenu;
    private RectTransform continueButton;
    private RectTransform loadGameButton;
    private Fade fade;

    private Label currentProfileLabel;

    private void Awake()
    {
        //Verify that game is ready to run
        if (!ProfileRepo.instance.HasProfiles)
        {
            SceneManager.LoadScene(SceneHelper.ProfileCreate);
            return;
        }

        canvas2D = GameObject.Find(ComponentHelper.Title.Canvas2D).GetComponent<RectTransform>();
        mainMenu = GameObject.Find(ComponentHelper.Title.MainMenu).GetComponent<RectTransform>();
        continueButton = GameObject.Find(ComponentHelper.Title.ContinueButton).GetComponent<RectTransform>();
        loadGameButton = GameObject.Find(ComponentHelper.Title.LoadGameButton).GetComponent<RectTransform>();
        fade = GameObject.Find(ComponentHelper.Title.Fade).GetComponent<Fade>();
        currentProfileLabel = GameObject.Find(ComponentHelper.Title.ChangeProfileButtonLabel).GetComponent<Label>();
        currentProfileLabel.text = ProfileRepo.instance.CurrentProfile.Key;
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
