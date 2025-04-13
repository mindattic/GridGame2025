using Assets.Scripts.Repositories;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    //Quick Reference Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected StageManager stageManager => GameManager.instance.stageManager;


    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    public bool IsPaused => Time.timeScale == 0f;

    //Fields
    private GameObject pauseButton;
    private Image pauseButtonImage;
    private Sprite pause;
    private Sprite paused;
    private GameObject pauseMenu;
    private FadeInstance fade;

    void Awake()
    {
        pauseButton = GameObject.Find(ComponentHelper.Game.PauseButton);
        pauseButtonImage = pauseButton.GetComponent<Image>();
        pauseMenu = GameObject.Find(ComponentHelper.Game.PauseMenu).gameObject;
        fade = GameObject.Find(ComponentHelper.TitleScreen.Fade).GetComponent<FadeInstance>();
    }

    private void Start()
    {
        pause = resourceManager.Sprite("Pause").Value;
        paused = resourceManager.Sprite("Paused").Value;
        pauseButtonImage.sprite = pause;
        canvasOverlay.Hide();
        pauseMenu.SetActive(false);
    }

    private void DisableButtons()
    {
        pauseMenu.GetComponentInChildren<Button>().interactable = false;
    }

    public void Toggle()
    {
        if (IsPaused)
            OnResumeButtonClicked();
        else
            OnPauseButtonClicked();
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pauseButtonImage.sprite = paused;
        canvasOverlay.Show();
        pauseMenu.SetActive(true);

    }

    private void Resume()
    {
        Time.timeScale = 1f;
        pauseButtonImage.sprite = pause;
        canvasOverlay.Hide();
        pauseMenu.SetActive(false);
    }

    public void OnPauseButtonClicked()
    {
        Pause();
    }

    public void OnResumeButtonClicked()
    {
        Resume();
    }

    public void OnSaveGameButtonClicked()
    {
        ProfileRepo.instance.Save(overwrite: true);
        Resume();
    }

    public void OnRestartStageButtonClicked()
    {
        stageManager.RestartStage();
        Resume();
    }

    public void OnPartyManagerButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.instance.Save(overwrite: true);
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.PartyManager)));
    }

    public void OnSpawnEnemyButtonClicked()
    {
        debugManager.SpawnRandomEnemy();
    }

    public void OnStageSelectButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.instance.Save(overwrite: true);
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.StageSelect)));   
    }

    public void OnSettingsButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.instance.Save(overwrite: true);
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.Settings)));
    }

    public void OnTitleScreenButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.instance.Save(overwrite: true);
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.TitleScreen)));
    }

}
