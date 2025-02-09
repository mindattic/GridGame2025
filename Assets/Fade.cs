using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] public Image image;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        image.gameObject.SetActive(true);
        image.color = new Color(0, 0, 0, 1);
    }

    public IEnumerator FadeIn(IEnumerator coroutine = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        image.color = new Color(0, 0, 0, 0);

        if (coroutine != null)
            yield return coroutine;
    }

    public IEnumerator FadeOut(IEnumerator coroutine = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        image.color = new Color(0, 0, 0, 1);

        if (coroutine != null)
            yield return coroutine;
    }
}
