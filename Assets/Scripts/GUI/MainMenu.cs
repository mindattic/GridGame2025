using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class MainMenu : MonoBehaviour
{
    //Fields
    private Fade fade;
    //[SerializeField] private Button continueButton;
    //[SerializeField] private Button newGameButton;
    //[SerializeField] private Button loadGameButton;
    //[SerializeField] private Button optionsButton;
    //[SerializeField] private Button quiteButton;

    private void Awake()
    {
        fade = GameObject.Find("Fade").GetComponent<Fade>();
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
        ToggleButtons(interactable: false);
        IEnumerator loadScene()
        {
            SceneManager.LoadScene(sceneName);
            yield return Wait.UntilNextFrame();
        }
        yield return StartCoroutine(fade.FadeOut(loadScene()));
    }

    private IEnumerator Quit()
    {
        ToggleButtons(interactable: false);
        IEnumerator quit()
        {
            Application.Quit();
            yield return Wait.UntilNextFrame();
        }
        yield return StartCoroutine(fade.FadeOut(quit()));
    }

    private void ToggleButtons(bool interactable)
    {
        //foreach (Button button in buttons)
        //{
        //    button.interactable = interactable;
        //}
    }
}
