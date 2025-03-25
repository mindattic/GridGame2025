using Assets.Scripts.Repositories;
using System.Collections;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour
{
    //Fields
    private Fade fade;
    private float waitDuration = 30;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Splash.Fade).GetComponent<Fade>();
    }

    void Start()
    {
        StartCoroutine(Startup());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.TitleScreen)));
    }

    private IEnumerator Startup()
    {
        yield return fade.FadeIn();
        yield return new WaitForSeconds(waitDuration);
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.TitleScreen)));
    }


}
