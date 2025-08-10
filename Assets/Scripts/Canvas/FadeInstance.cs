using Assets.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInstance : MonoBehaviour
{
    private Image image;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = SpriteRepo.Sprites["Black16x16"];
        image.color = new Color(0, 0, 0, 1);
    }

    public IEnumerator FadeInRoutine(IEnumerator coroutine = null)
    {
        //Before: BounceRoutine fully black
        image.color = new Color(0, 0, 0, 1);
        float elapsedTime = 0f;

        //During: FadeInstance from black (1) to transparent (0)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return Wait.None();
        }

        //After: Ensure fully transparent
        image.color = new Color(0, 0, 0, 0);

        //Run additional coroutine if provided
        if (coroutine != null)
            yield return coroutine;
    }

    public IEnumerator FadeOutRoutine(IEnumerator coroutine = null)
    {
        //Before: BounceRoutine fully transparent
        image.color = new Color(0, 0, 0, 0);
        float elapsedTime = 0f;

        //During: FadeInstance from transparent (0) to black (1)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return Wait.None();
        }

        //After: Ensure fully black
        image.color = new Color(0, 0, 0, 1);

        //Run additional coroutine if provided
        if (coroutine != null)
            yield return coroutine;
    }

    public IEnumerator ShowRoutine(IEnumerator coroutine = null)
    {
        image.color = new Color(0, 0, 0, 0);

        if (coroutine != null)
            yield return coroutine;
    }

    public IEnumerator HideRoutine(IEnumerator coroutine = null)
    {
        image.color = new Color(0, 0, 0, 1);

        //Run additional coroutine if provided
        if (coroutine != null)
            yield return coroutine;
    }
}
