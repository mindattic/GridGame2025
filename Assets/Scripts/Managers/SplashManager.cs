using Assets.Scripts.Store;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    //Fields
    private Fade fade;
    private float waitDuration = 30;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Splash.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
    }

    void Start()
    {
        StartCoroutine(Startup());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(fade.FadeOut(SceneHub.LoadScene(SceneHelper.Title)));
    }

    private IEnumerator Startup()
    {
        yield return fade.FadeIn();
        yield return new WaitForSeconds(waitDuration);
        StartCoroutine(fade.FadeOut(SceneHub.LoadScene(SceneHelper.Title)));
    }

}
