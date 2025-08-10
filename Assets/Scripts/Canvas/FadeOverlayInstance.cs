using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOverlayInstance : MonoBehaviour
{
    private Image image;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = SpriteRepo.Sprites["Black16x16"];
        image.color = new Color(0, 0, 0, 1);
    }

    public void FadeIn(IEnumerator routine = null) => StartCoroutine(FadeInRoutine(routine));
    private IEnumerator FadeInRoutine(IEnumerator routine = null)
    {
        //Before: BounceRoutine fully black
        image.color = new Color(0, 0, 0, 1);
        float elapsedTime = 0f;

        //During: FadeOverlayInstance from black (1) to transparent (0)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return Wait.None();
        }

        //After: Ensure fully transparent
        image.color = new Color(0, 0, 0, 0);

        //Run additional routine if provided
        if (routine != null)
            yield return routine;
    }

    public void FadeOut(IEnumerator routine = null) => StartCoroutine(FadeOutRoutine(routine));
    private IEnumerator FadeOutRoutine(IEnumerator routine = null)
    {
        //Before: BounceRoutine fully transparent
        image.color = new Color(0, 0, 0, 0);
        float elapsedTime = 0f;

        //During: FadeOverlayInstance from transparent (0) to black (1)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return Wait.None();
        }

        //After: Ensure fully black
        image.color = new Color(0, 0, 0, 1);

        //Run additional routine if provided
        if (routine != null)
            yield return routine;
    }

    public void Show(IEnumerator routine = null) => StartCoroutine(ShowRoutine(routine));
    private IEnumerator ShowRoutine(IEnumerator routine = null)
    {
        image.color = new Color(0, 0, 0, 0);

        if (routine != null)
            yield return routine;
    }

    public void Hide(IEnumerator routine = null) => StartCoroutine(HideRoutine(routine));
    private IEnumerator HideRoutine(IEnumerator routine = null)
    {
        image.color = new Color(0, 0, 0, 1);

        //Run additional routine if provided
        if (routine != null)
            yield return routine;
    }
}
