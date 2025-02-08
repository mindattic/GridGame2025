using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] public Image fade;
    float delay = 1f;
    float fadeDuration = 1f;


    void Start()
    {
        fade.gameObject.SetActive(true);
        fade.color = new Color(0, 0, 0, 0);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        //Wait for delay
        yield return new WaitForSeconds(delay);

        //Start fading to black
        yield return StartCoroutine(FadeToBlack());

        // Load the next scene
        SceneManager.LoadScene("Game");
    }

    IEnumerator FadeToBlack()
    {
        //Before:
        float elapsedTime = 0f;

        //During:
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fade.color = new Color(0, 0, 0, alpha);
            yield return Wait.UntilNextFrame();
        }

        //After:
        fade.color = new Color(0, 0, 0, 1);
    }
}
