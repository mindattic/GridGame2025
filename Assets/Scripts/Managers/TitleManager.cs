using Assets.Scripts.Store;
using UnityEngine;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;

public class TitleManager : MonoBehaviour
{
    //Fields
    private Fade fade;
    private Button[] buttons;

    private Label currentProfileLabel;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Title.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        buttons = GameObject.Find(ComponentHelper.Title.MainMenu).GetComponentsInChildren<Button>();
        MenuHelper.Initialize(buttons);
        currentProfileLabel = GameObject.Find(ComponentHelper.Title.ChangeProfileButtonLabel).GetComponent<Label>();
        currentProfileLabel.text = ProfileStore.instance.CurrentProfile.Key;
    }

    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);

        ProfileStore.instance.CurrentProfile.CurrentSave = ProfileStore.instance.CurrentProfile.LatestSave;
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Game)));
    }

    public void OnNewGameButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        ProfileStore.instance.CreateProfile();
        ProfileStore.instance.Reload();
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Overworld)));
    }

    public void OnLoadGameButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.SaveFileSelect)));
    }

    public void OnSettingsButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Settings)));
    }

    public void OnCreditsButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Credits)));
    }

    public void OnChangeProfileButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.ProfileSelect)));
    }
}
