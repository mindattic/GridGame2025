using Assets.Helper;
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
        StartCoroutine(FadeInRoutine());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadTitleScreen()));
    }

    private IEnumerator FadeInRoutine()
    {
        yield return fade.FadeInRoutine();
        yield return new WaitForSeconds(waitDuration);
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadTitleScreen()));
    }


}
