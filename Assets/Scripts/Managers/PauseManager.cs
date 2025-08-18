using Assets.Helper;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;
using g = Assets.Helpers.GameHelper;
using Assets.Helpers;

public class PauseManager : MonoBehaviour
{
    public bool IsPaused => Time.timeScale == 0f;

    private PauseButton pauseButton;
    private Image pauseIconImage;
    private Sprite pauseIcon;
    private Sprite resumeIcon;
    private GameObject pauseMenu;

    /// <summary>
    /// Grabs references to the PauseButton, its visible icon Image, and the PauseMenu root.
    /// The icon Image is resolved from children so the correct visible sprite is updated.
    /// </summary>
    private void Awake()
    {
        var pauseButtonGO = GameObject.Find(GameObjectHelper.Game.PauseButton);
        if (pauseButtonGO != null)
        {
            pauseButton = pauseButtonGO.GetComponent<PauseButton>();

            // Prefer a child Image as the visible icon, since the root Image is a transparent hit area
            var images = pauseButtonGO.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null && images[i].transform != pauseButtonGO.transform)
                {
                    pauseIconImage = images[i];
                    break;
                }
            }

            // Fallback to the root Image if no child icon is present
            if (pauseIconImage == null) pauseIconImage = pauseButtonGO.GetComponent<Image>();
        }

        pauseMenu = GameObject.Find(GameObjectHelper.Game.PauseMenu);
    }

    /// <summary>
    /// Loads sprites and initializes UI state. Ensures the icon is visible and preserves aspect.
    /// </summary>
    private void Start()
    {
        pauseIcon = SpriteLibrary.Sprites["Pause"];
        resumeIcon = SpriteLibrary.Sprites["Paused"];

        if (pauseIconImage != null)
        {
            pauseIconImage.sprite = pauseIcon;
            pauseIconImage.preserveAspect = true;
            pauseIconImage.color = Color.white;
        }

        g.PauseOverlay.Hide();

        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    /// <summary>
    /// Disables interaction on the first Button found under the pause menu.
    /// </summary>
    private void DisableButtons()
    {
        if (pauseMenu == null) return;
        var firstButton = pauseMenu.GetComponentInChildren<Button>();
        if (firstButton != null) firstButton.interactable = false;
    }

    /// <summary>
    /// Toggles between paused and unpaused states.
    /// </summary>
    public void Toggle()
    {
        if (IsPaused) OnResumeButtonClicked();
        else OnPauseButtonClicked();
    }

    /// <summary>
    /// Applies paused state, swaps the icon to the resume sprite, and shows the overlay/menu.
    /// </summary>
    private void Pause()
    {
        Time.timeScale = 0f;

        if (pauseIconImage != null)
        {
            pauseIconImage.sprite = resumeIcon;
            pauseIconImage.preserveAspect = true;
        }

        g.PauseOverlay.Show();
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Clears paused state, swaps the icon to the pause sprite, and hides the overlay/menu.
    /// </summary>
    private void Resume()
    {
        Time.timeScale = 1f;

        if (pauseIconImage != null)
        {
            pauseIconImage.sprite = pauseIcon;
            pauseIconImage.preserveAspect = true;
        }

        g.PauseOverlay.Hide();
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    /// <summary>
    /// UI button hook to pause.
    /// </summary>
    public void OnPauseButtonClicked()
    {
        Pause();
    }

    /// <summary>
    /// UI button hook to resume.
    /// </summary>
    public void OnResumeButtonClicked()
    {
        Resume();
    }

    /// <summary>
    /// Saves the profile and resumes the game.
    /// </summary>
    public void OnSaveGameButtonClicked()
    {
        ProfileHelper.Save(overwrite: true);
        Resume();
    }

    /// <summary>
    /// Restarts the stage and resumes the game.
    /// </summary>
    public void OnRestartStageButtonClicked()
    {
        g.StageManager.RestartStage();
        Resume();
    }

    /// <summary>
    /// Navigates to Party Manager scene and unpauses time.
    /// </summary>
    public void OnPartyManagerButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToPartyManager();
    }

    /// <summary>
    /// Spawns a random enemy through the debug manager.
    /// </summary>
    public void OnSpawnEnemyButtonClicked()
    {
        g.DebugManager.SpawnRandomEnemy();
    }

    /// <summary>
    /// Navigates to Stage Select and unpauses time.
    /// </summary>
    public void OnStageSelectButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToStageSelect();
    }

    /// <summary>
    /// Navigates to Settings and unpauses time.
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToSettings();
    }

    /// <summary>
    /// Navigates to Title Screen and unpauses time.
    /// </summary>
    public void OnTitleScreenButtonClicked()
    {
        Time.timeScale = 1f;
        ProfileHelper.Save(overwrite: true);
        scene.Change.ToTitleScreen();
    }
}
