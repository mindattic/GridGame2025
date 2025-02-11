using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
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


    public void OnBackClicked()
    {
        StartCoroutine(fade.FadeOut(LoadScene(Scene.TitleScreen)));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        DisableButtons();
        SceneManager.LoadScene(sceneName);
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
