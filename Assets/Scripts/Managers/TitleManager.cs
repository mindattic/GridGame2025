using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    //Fields
    private Fade fade;
    private Button[] buttons;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Title.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        buttons = GameObject.Find(ComponentHelper.Title.MainMenu).GetComponentsInChildren<Button>();
        MenuHelper.Align(buttons);
    }

    private void Start()
    {    
        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnNewGameButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnLoadGameButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnSettingsButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Settings)));
    }

    public void OnCreditsButtonClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Credits)));
    }

    public void OnQuitButtonClicked()
    {     
        StartCoroutine(fade.FadeOut(Quit()));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        DisableButtons();
        SceneManager.LoadScene(sceneName);
        yield break;
    }

    private IEnumerator Quit()
    {
        DisableButtons();
        Application.Quit();
        yield break;
    }

    private void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }
}
