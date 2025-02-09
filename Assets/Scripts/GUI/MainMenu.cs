using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public Image fade;

    private float fadeDuration = 0.5f;
    private Button[] buttons;

    void Start()
    {
        fade.gameObject.SetActive(true);
        fade.color = new Color(0, 0, 0, 1);
        buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        StartCoroutine(FadeIn());
    }

    public void OnContinueClicked()
    {
        StartCoroutine(LoadScene("Game"));
    }

    public void OnNewGameClicked()
    {
        StartCoroutine(LoadScene("Game"));
    }

    public void OnLoadGameClicked()
    {
        StartCoroutine(LoadScene("Game"));
    }

    public void OnOptionsClicked()
    {
        // StartCoroutine(LoadScene("Options"));
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
            yield return null;
        }
        yield return StartCoroutine(FadeOut(loadScene()));
        
    }

    private IEnumerator Quit()
    {
        DisableButtons();

        IEnumerator quit()
        {
            Application.Quit();
            yield return null;
        }
        yield return StartCoroutine(FadeOut(quit()));
       
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            fade.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fade.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator FadeOut(IEnumerator coroutine)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fade.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fade.color = new Color(0, 0, 0, 1);
        yield return coroutine;
    }

    private void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}
