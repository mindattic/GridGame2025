using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    //Quick Reference Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected ProfileManager profileManager => GameManager.instance.profileManager;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    public bool IsPaused => Time.timeScale == 0f;

    //Fields

    private Image buttonImage;
    private Sprite pause;
    private Sprite paused;
    private GameObject pauseMenu;

    void Awake()
    {
        GameObject pauseButton = GameObject.Find("PauseButton");
        buttonImage = pauseButton.GetComponent<Image>();
        pauseMenu = GameObject.Find("PauseMenu").gameObject;
    }

    private void Start()
    {
        pause = resourceManager.Sprite("Pause").Value;
        paused = resourceManager.Sprite("Paused").Value;
        buttonImage.sprite = pause;
        canvasOverlay.Reset();
        pauseMenu.SetActive(false);
    }

    public void Toggle()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        buttonImage.sprite = paused;
        canvasOverlay.Show("Paused");
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        buttonImage.sprite = pause;
        canvasOverlay.Reset();
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void GotoOptionsScreen()
    {
        SceneManager.LoadScene(Scene.OptionsScreen);
    }

    public void GotoLevelSelect()
    {
        SceneManager.LoadScene(Scene.LevelSelect);
    }

    public void GotoTitleScreen()
    {
        SceneManager.LoadScene(Scene.TitleScreen);
    }

    public void Save()
    {
        profileManager.Save();
    }

}
