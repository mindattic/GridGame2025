using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    //Quick Reference Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected ProfileManager profileManager => GameObject.Find(Constants.ProfileManager).GetComponent<ProfileManager>();
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    public bool IsPaused => Time.timeScale == 0f;

    //Fields
    private GameObject pauseButton;
    private Image pauseButtonImage;
    private Sprite pause;
    private Sprite paused;
    private GameObject pauseMenu;
    private Button[] buttons;
    private Fade fade;

    void Awake()
    {
        pauseButton = GameObject.Find(ComponentHelper.Game.PauseButton) ?? throw new UnityException("PauseButton is null");
        pauseButtonImage = pauseButton.GetComponent<Image>() ?? throw new UnityException("PauseButtonImage is null");
        pauseMenu = GameObject.Find(ComponentHelper.Game.PauseMenu).gameObject ?? throw new UnityException("ComponentHelper is null");
        fade = GameObject.Find(ComponentHelper.Title.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        buttons = GameObject.Find(ComponentHelper.Game.PauseMenu).GetComponentsInChildren<Button>() ?? throw new UnityException("PauseMenu buttons are null");
        MenuHelper.Initialize(buttons);
    }

    private void Start()
    {
        pause = resourceManager.Sprite("Pause").Value;
        paused = resourceManager.Sprite("Paused").Value;
        pauseButtonImage.sprite = pause;
        canvasOverlay.Reset();
        pauseMenu.SetActive(false);
    }

    public void Toggle()
    {
        if (IsPaused)
            OnResumeButtonClicked();
        else
            OnPauseButtonClicked();
    }

    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
        pauseButtonImage.sprite = pause;
        canvasOverlay.Reset();
        pauseMenu.SetActive(false);
      
    }

    public void OnPauseButtonClicked()
    {
        Time.timeScale = 0f;
        pauseButtonImage.sprite = paused;
        canvasOverlay.Show("Paused");
        pauseMenu.SetActive(true);     
    }

    public void OnSettingsButtonClicked()
    {
        Time.timeScale = 1f;
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Settings)));
    }

    public void OnStageSelectButtonClicked()
    {
        Time.timeScale = 1f;
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.StageSelect)));   
    }

    public void OnQuitButtonClicked()
    {
        Time.timeScale = 1f;
        profileManager.Save();
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Title)));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        yield break;
    }

    //private void DisableButtons()
    //{
    //    foreach (var button in buttons)
    //    {
    //        button.interactable = false;
    //    }
    //}

}
