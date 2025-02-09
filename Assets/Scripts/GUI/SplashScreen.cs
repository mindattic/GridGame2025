using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] public Image fade;

    float fadeDuration = 0.5f;
    float waitDuration = 3f;

    void Start()
    {
        StartCoroutine(Startup());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(LoadScene());
    }

    private IEnumerator Startup()
    {
        fade.gameObject.SetActive(true);
        fade.color = new Color(0, 0, 0, 1);
        yield return FadeIn();
        yield return new WaitForSeconds(waitDuration);
        yield return LoadScene();
    }

    private IEnumerator LoadScene()
    {
        yield return FadeOut();
        SceneManager.LoadScene("TitleScreen");
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

    private IEnumerator FadeOut()
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
    }

}
