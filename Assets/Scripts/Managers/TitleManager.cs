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
        DisableButtons();
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnNewGameButtonClicked()
    {
        DisableButtons();
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnLoadGameButtonClicked()
    {
        DisableButtons();
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.ProfileSelect)));
    }

    public void OnSettingsButtonClicked()
    {
        DisableButtons();
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Settings)));
    }

    public void OnCreditsButtonClicked()
    {
        DisableButtons();
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Credits)));
    }

    public void OnQuitButtonClicked()
    {
        DisableButtons();
        StartCoroutine(fade.FadeOut(Quit()));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        yield break;
    }

    private IEnumerator Quit()
    {
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
