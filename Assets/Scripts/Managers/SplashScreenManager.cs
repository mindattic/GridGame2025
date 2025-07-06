using Assets.Scripts.Repositories;
using System.Collections;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour
{
    //Fields
    private FadeInstance fade;
    private float waitDuration = 30;

    private void Awake()
    {
        fade = GameObject.Find(GameObjectHelper.SplashScreen.Fade).GetComponent<FadeInstance>();
    }

    void Start()
    {
        StartCoroutine(Startup());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.TitleScreen)));
    }

    private IEnumerator Startup()
    {
        yield return fade.FadeIn();
        yield return new WaitForSeconds(waitDuration);
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.TitleScreen)));
    }


}
