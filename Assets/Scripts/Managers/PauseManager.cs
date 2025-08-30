using Assets.Helper;
using Assets.Helpers;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;
using scene = Assets.Helpers.SceneHelper;

public class PauseManager : MonoBehaviour
{
    // True when the game is paused by time scale.
    public bool IsPaused => Time.timeScale == 0f;

    // Visible pause icon image found under the pause button.
    private Image pauseIconImage;

    // Sprites for pause and resume states.
    private Sprite pauseIcon;
    private Sprite resumeIcon;

    // Root of the pause menu UI.
    private GameObject pauseMenu;

    /// <summary>
    /// Locate the PauseButton object, resolve its visible icon Image, and cache the pause menu root.
    /// </summary>
    private void Awake()
    {
        // Find the pause button object by helper key.
        var pauseButtonGO = GameObject.Find(GameObjectHelper.Game.PauseButton);
        if (pauseButtonGO == null)
        {
            Debug.LogError($"Pause button not found: {GameObjectHelper.Game.PauseButton}");
        }
        else
        {
            // Prefer a child Image as the visible icon (root may be a transparent hit area).
            var images = pauseButtonGO.GetComponentsInChildren<Image>(true);

            // Scan children for the first Image that is not on the root.
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null && images[i].transform != pauseButtonGO.transform)
                {
                    pauseIconImage = images[i];
                    break;
                }
            }

            // Fallback to the root Image if no child icon is present.
            if (pauseIconImage == null)
            {
                pauseIconImage = pauseButtonGO.GetComponent<Image>();
                if (pauseIconImage == null)
                {
                    Debug.LogError("Pause button is missing an Image component for the icon.");
                }
            }
        }

        // Cache the pause menu root.
        pauseMenu = GameObject.Find(GameObjectHelper.Game.PauseMenu);
        if (pauseMenu == null)
        {
            Debug.LogError($"Pause menu not found: {GameObjectHelper.Game.PauseMenu}");
        }
    }

    /// <summary>
    /// Load sprites, initialize UI, and hide overlays and menu.
    /// </summary>
    private void Start()
    {
        // Validate sprite library and fetch required sprites.
        if (SpriteLibrary.Sprites == null ||
            !SpriteLibrary.Sprites.ContainsKey("Pause") ||
            !SpriteLibrary.Sprites.ContainsKey("Paused"))
        {
            Debug.LogError("SpriteLibrary missing required 'Pause' or 'Paused' sprites.");
        }
        else
        {
            pauseIcon = SpriteLibrary.Sprites["Pause"];
            resumeIcon = SpriteLibrary.Sprites["Paused"];
        }

        // Initialize the visible icon if available.
        if (pauseIconImage != null && pauseIcon != null)
        {
            pauseIconImage.sprite = pauseIcon;
            pauseIconImage.preserveAspect = true;
            pauseIconImage.color = Color.white;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    /// <summary>
    /// Toggle between paused and unpaused states.
    /// </summary>
    public void Toggle()
    {
        // Use current state to decide action.
        if (IsPaused) 
            OnResumeButtonClicked();
        else 
            OnPauseButtonClicked();
    }

    /// <summary>
    /// Apply paused state, swap icon to the resume sprite, and show overlay/menu.
    /// </summary>
    private void Pause()
    {
        // Stop time to pause gameplay.
        Time.timeScale = 0f;

        // Update icon to show the "resume" state.
        if (pauseIconImage != null && resumeIcon != null)
        {
            pauseIconImage.sprite = resumeIcon;
            pauseIconImage.preserveAspect = true;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Clear paused state, swap icon to the pause sprite, and hide overlay/menu.
    /// </summary>
    private void Resume()
    {
        // Resume time to unpause gameplay.
        Time.timeScale = 1f;

        // Update icon to show the "pause" state.
        if (pauseIconImage != null && pauseIcon != null)
        {
            pauseIconImage.sprite = pauseIcon;
            pauseIconImage.preserveAspect = true;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }


    /// <summary>
    /// Clear paused state, swap icon to the pause sprite, and hide overlay/menu.
    /// </summary>
    private void Runaway()
    {
        // Ensure time is running before scene change.
        Time.timeScale = 1f;

        // Navigate.
        //TODO: Cause party to drop coins like FF IV...
        scene.Change.ToOverworld();
    }


    /// <summary>
    /// UI button hook to pause.
    /// </summary>
    public void OnPauseButtonClicked()
    {
        // Enter paused state.
        Pause();
    }

    /// <summary>
    /// UI button hook to resume.
    /// </summary>
    public void OnResumeButtonClicked()
    {
        // Exit paused state.
        Resume();
    }

    public void OnRunAwayClicked()
    {
        Runaway();
    }

    /// <summary>
    /// Save the profile, then resume the game.
    /// </summary>
    public void OnQuickSaveGameButtonClicked()
    {
        // Save progress.
        ProfileHelper.Save(overwrite: true);

        // Return to gameplay.
        Resume();
    }

    /// <summary>
    /// Save the profile, then resume the game.
    /// </summary>
    public void OnCreateSaveGameButtonClicked()
    {
        ProfileHelper.Save(overwrite: false);

        // Return to gameplay.
        Resume();
    }

    /// <summary>
    /// Restart the current stage, then resume the game.
    /// </summary>
    public void OnRestartStageButtonClicked()
    {
        // Restart stage through game helper.
        try { g.StageManager.RestartStage(); } catch { Debug.LogError("Stage restart failed."); }

        // Ensure gameplay resumes.
        Resume();
    }

    /// <summary>
    /// Go to Party Manager scene and unpause time.
    /// </summary>
    public void OnPartyManagerButtonClicked()
    {
        // Ensure time is running before scene change.
        Time.timeScale = 1f;

        // Navigate.
        scene.Change.ToPartyManager();
    }

    /// <summary>
    /// Go to Stage Select and unpause time.
    /// </summary>
    public void OnStageSelectButtonClicked()
    {
        // Ensure time is running before scene change.
        Time.timeScale = 1f;

        // Navigate.
        scene.Change.ToStageSelect();
    }

    /// <summary>
    /// Go to Settings and unpause time.
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        // Ensure time is running before scene change.
        Time.timeScale = 1f;

        // Navigate.
        scene.Change.ToSettings();
    }

    /// <summary>
    /// Go to Title Screen and unpause time.
    /// </summary>
    public void OnTitleScreenButtonClicked()
    {
        // Ensure time is running before scene change.
        Time.timeScale = 1f;

        // Navigate.
        scene.Change.ToTitleScreen();
    }
}
