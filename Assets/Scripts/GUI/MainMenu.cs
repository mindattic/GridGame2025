using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    //Fields
    private Fade fade;

    private void Awake()
    {
        fade = GameObject.Find(Constants.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
    }

    void Start()
    {  
        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueClicked()
    {
        StartCoroutine(LoadScene(Scene.Game));
    }

    public void OnNewGameClicked()
    {
        StartCoroutine(LoadScene(Scene.Game));
    }

    public void OnLoadGameClicked()
    {
        StartCoroutine(LoadScene(Scene.Game));
    }

    public void OnOptionsClicked()
    {
        StartCoroutine(LoadScene(Scene.Game));
    }

    public void OnQuitClicked()
    {
        StartCoroutine(Quit());
    }

    private IEnumerator LoadScene(string sceneName)
    {
        DisableButtons();
        IEnumerator loadScene()
        {
            SceneManager.LoadScene(sceneName);
            yield return Wait.UntilNextFrame();
        }
        yield return StartCoroutine(fade.FadeOut(loadScene()));
    }

    private IEnumerator Quit()
    {
        DisableButtons();
        IEnumerator quit()
        {
            Application.Quit();
            yield return Wait.UntilNextFrame();
        }
        yield return StartCoroutine(fade.FadeOut(quit()));
    }

    private void DisableButtons()
    {
        Button[] buttons = GameObject.Find("Panel").GetComponentsInChildren<Button>();

        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }
}
