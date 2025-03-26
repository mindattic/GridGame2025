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

    float screenWidth;
    float screenHeight;
    float buttonWidth;
    float buttonHeight;
    float fontSize;
    float rowSpacing;

    private void Awake()
    {
        //Verify that game is ready to run
        if (!ProfileRepo.instance.HasProfiles)
        {
            SceneManager.LoadScene(SceneHelper.ProfileCreate);
            return;
        }

        canvas2D = GameObject.Find(ComponentHelper.TitleScreen.Canvas2D).GetComponent<RectTransform>();
        //panel = GameObject.Find(ComponentHelper.TitleScreen.Panel).GetComponent<RectTransform>();
        //continueButton = GameObject.Find(ComponentHelper.TitleScreen.ContinueButton).GetComponent<RectTransform>();
        //loadGameButton = GameObject.Find(ComponentHelper.TitleScreen.LoadGameButton).GetComponent<RectTransform>();
        //settingsButton = GameObject.Find(ComponentHelper.TitleScreen.SettingsButton).GetComponent<RectTransform>();
        //creditsButton = GameObject.Find(ComponentHelper.TitleScreen.CreditsButton).GetComponent<RectTransform>();
        //profileButton = GameObject.Find(ComponentHelper.TitleScreen.ProfileButton).GetComponent<RectTransform>();
        profileButtonLabel = GameObject.Find(ComponentHelper.TitleScreen.ProfileButtonLabel).GetComponent<RectTransform>();
        fade = GameObject.Find(ComponentHelper.TitleScreen.Fade).GetComponent<FadeInstance>();

        //screenWidth = canvas2D.rect.width;
        //screenHeight = canvas2D.rect.height;
        //buttonWidth = 0.9f * screenWidth;
        //buttonHeight = screenHeight / 16f;
        //fontSize = buttonHeight / 2;
        //rowSpacing = screenHeight * 0.01f;

        //panel.sizeDelta = new Vector2(screenWidth, screenHeight);
        //panel.anchoredPosition = new Vector2(0, 0);

        //continueButton.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        //continueButton.anchoredPosition = new Vector2(0, buttonHeight * 2 + rowSpacing * 2);
        //continueButton.GetComponentInChildren<Label>().fontSize = fontSize;

        //loadGameButton.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        //loadGameButton.anchoredPosition = new Vector2(0, buttonHeight + rowSpacing);
        //loadGameButton.GetComponentInChildren<Label>().fontSize = fontSize;

        //settingsButton.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        //settingsButton.anchoredPosition = new Vector2(0, 0);
        //settingsButton.GetComponentInChildren<Label>().fontSize = fontSize;

        //creditsButton.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        //creditsButton.anchoredPosition = new Vector2(0, -buttonHeight - rowSpacing);
        //creditsButton.GetComponentInChildren<Label>().fontSize = fontSize;

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
