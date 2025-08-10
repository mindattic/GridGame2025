using Assets.Helper;
using UnityEngine;
using UnityEngine.UI;
using f = Assets.Helpers.FadeOverlayHelper;
using g = Assets.Helpers.GameHelper;

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
        pauseButtonImage = pauseButton.GetComponent<Image>();
        pauseMenu = GameObject.Find(GameObjectHelper.Game.PauseMenu).gameObject;
    }

    private void Start()
    {
        pause = SpriteRepo.Sprites["Pause"];
        paused = SpriteRepo.Sprites["Paused"];
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
        ProfileRepo.Save(overwrite: true);
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
        ProfileRepo.Save(overwrite: true);
        f.Overlay.FadeOut(SceneHelper.LoadPartyManager());
    }

    public void OnSpawnEnemyButtonClicked()
    {
        g.DebugManager.SpawnRandomEnemy();
    }

    public void OnStageSelectButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.Save(overwrite: true);
        f.Overlay.FadeOut(SceneHelper.LoadStageSelect());
    }

    public void OnSettingsButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.Save(overwrite: true);
        f.Overlay.FadeOut(SceneHelper.LoadSettings());
    }

    public void OnTitleScreenButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileRepo.Save(overwrite: true);
        f.Overlay.FadeOut(SceneHelper.LoadTitleScreen());
    }

}
