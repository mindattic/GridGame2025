using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    //Fields
    private Fade fade;
    private float waitDuration = 3f;

    private void Awake()
    {
        fade = GameObject.Find("Fade").GetComponent<Fade>();
    }

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
        yield return fade.FadeIn();
        yield return new WaitForSeconds(waitDuration);
        yield return LoadScene();
    }

    private IEnumerator LoadScene()
    {
        yield return fade.FadeOut();
        SceneManager.LoadScene(Scene.TitleScreen);
    }
}
