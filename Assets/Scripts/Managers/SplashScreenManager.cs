using Assets.Helper;
using System.Collections;
using UnityEngine;
using f = Assets.Helpers.FadeOverlayHelper;

public class SplashScreenManager : MonoBehaviour
{
    //Fields
    private float waitDuration = 30;

    private void Awake()
    {
    }

    void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            f.Overlay.FadeOut(SceneHelper.LoadTitleScreen());
    }

    private IEnumerator FadeInRoutine()
    {
        f.Overlay.FadeIn();
        yield return new WaitForSeconds(waitDuration);
        f.Overlay.FadeOut(SceneHelper.LoadTitleScreen());
    }
}
