using Assets.Helper;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;
using g = Assets.Helpers.GameHelper;
using Assets.Helpers;

public class PauseManager : MonoBehaviour
{
    public bool IsPaused => Time.timeScale == 0f;

    //Fields
    private GameObject pauseButton;
    private Image pauseButtonImage;
    private Sprite pause;
    private Sprite paused;
    private GameObject pauseMenu;
    void Awake()
    {
        pauseButton = GameObject.Find(GameObjectHelper.Game.PauseButton);
        pauseButtonImage = GameObject.Find(GameObjectHelper.Game.PauseButton).GetComponent<Image>();
        pauseMenu = GameObject.Find(GameObjectHelper.Game.PauseMenu);
    }

    private void Start()
    {
        pause = SpriteLibrary.Sprites["Pause"];
        paused = SpriteLibrary.Sprites["Paused"];
        pauseButtonImage.sprite = pause;
        g.PauseOverlay.Hide();
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
        g.PauseOverlay.Show();
        pauseMenu.SetActive(true);

    }

    private void Resume()
    {
        Time.timeScale = 1f;
        pauseButtonImage.sprite = pause;
        g.PauseOverlay.Hide();
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
        ProfileHelper.Save(overwrite: true);
        Resume();
    }

    public void OnRestartStageButtonClicked()
    {
        g.StageManager.RestartStage();
        Resume();
    }

    public void OnPartyManagerButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToPartyManager();
    }

    public void OnSpawnEnemyButtonClicked()
    {
        g.DebugManager.SpawnRandomEnemy();
    }

    public void OnStageSelectButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToStageSelect();
    }

    public void OnSettingsButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToSettings();
    }

    public void OnTitleScreenButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToTitleScreen();
    }

}
