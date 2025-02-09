using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] public Image image;
    private float fadeDuration = 0.5f;

    public IEnumerator FadeIn(IEnumerator coroutine = null)
    {
        //Before: Start fully black
        image.color = new Color(0, 0, 0, 1);
        float elapsedTime = 0f;

        //During: Fade from black (1) to transparent (0)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return Wait.UntilNextFrame();
        }

        //After: Ensure fully transparent
        image.color = new Color(0, 0, 0, 0);

        //Triggered: Run additional coroutine if provided
        if (coroutine != null)
            yield return coroutine;
    }

    public IEnumerator FadeOut(IEnumerator coroutine = null)
    {
        //Before: Start fully transparent
        image.color = new Color(0, 0, 0, 0);
        float elapsedTime = 0f;

        //During: Fade from transparent (0) to black (1)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return Wait.UntilNextFrame();
        }

        //After: Ensure fully black
        image.color = new Color(0, 0, 0, 1);

        //Triggered: Run additional coroutine if provided
        if (coroutine != null)
            yield return coroutine;
    }
}
