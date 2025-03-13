using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Scripts.Store;

public class TitleManager : MonoBehaviour
{
    //Fields
    private Fade fade;
    private Button[] buttons;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Title.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        buttons = GameObject.Find(ComponentHelper.Title.MainMenu).GetComponentsInChildren<Button>();
        MenuHelper.Initialize(buttons);
    }

    private void Start()
    {    
        StartCoroutine(fade.FadeIn());
    }

    public void OnContinueButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Game)));
    }

    public void OnNewGameButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        ProfileStore.instance.Create();
        ProfileStore.instance.Load();
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Game)));
    }

    public void OnLoadGameButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.ProfileSelect)));
    }

    public void OnSettingsButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Settings)));
    }

    public void OnCreditsButtonClicked()
    {
        MenuHelper.DisableButtons(buttons);
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Credits)));
    }

    private void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }
}
