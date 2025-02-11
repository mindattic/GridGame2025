using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

public class TitleScreen : MonoBehaviour
{
    //Fields
    private Fade fade;
    private Button[] buttons;

    private void Awake()
    {
        fade = GameObject.Find(Constants.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        buttons = GameObject.Find("Panel").GetComponentsInChildren<Button>();
    }

    void Start()
    {  
        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(Scene.Game)));
    }

    public void OnNewGameClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(Scene.Game)));
    }

    public void OnLoadGameClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(Scene.Game)));
    }

    public void OnOptionsClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(Scene.OptionsScreen)));
    }

    public void OnQuitClicked()
    {     
        StartCoroutine(fade.FadeOut(Quit()));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        DisableButtons();
        SceneManager.LoadScene(sceneName);
        yield return Wait.UntilNextFrame();
    }

    private IEnumerator Quit()
    {
        DisableButtons();
        Application.Quit();
        yield return Wait.UntilNextFrame();
    }

    private void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }
}
