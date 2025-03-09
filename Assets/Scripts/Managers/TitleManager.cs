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
        MenuHelper.SetPosition(buttons);
    }

    private void Start()
    {
        
        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnNewGameClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnLoadGameClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Game)));
    }

    public void OnSettingsClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(SceneHelper.Settings)));
    }

    public void OnQuitClicked()
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
