using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    #region Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    public bool IsPaused => Time.timeScale == 0f;
    #endregion

    private Image buttonImage;
    private Sprite pause;
    private Sprite paused;


    void Awake()
    {
        GameObject pauseButton = GameObject.Find("PauseButton");
        buttonImage = pauseButton.GetComponent<Image>();  
    }

    private void Start()
    {
        pause = resourceManager.Sprite("Pause").Value;
        paused = resourceManager.Sprite("Paused").Value;
        buttonImage.sprite = pause;
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
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        buttonImage.sprite = pause;
        canvasOverlay.Reset();
        Time.timeScale = 1f;
    }
}
