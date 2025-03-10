using Assets.Scripts.Store;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    //Fields
    private Fade fade;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Settings.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneHub.LoadPreviousScene()));
    }
}
