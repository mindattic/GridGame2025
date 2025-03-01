using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasOverlay : MonoBehaviour
{
    //Constants
    const float duration = 0.25f;
    const float rotateMultiplier = 1.5f;

    //Fields
    private RectTransform rect;
    private Image backgroundImage;
    private TextMeshProUGUI label;
    private float backgroundMinAlpha;
    private float backgroundMaxAlpha;
    private Color backgroundColor;
    private float labelMinAlpha;
    private float labelMaxAlpha;
    private Color labelColor;

    void Awake()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("CanvasOverlay must be a child of a Canvas.");
            return;
        }

        //Ensure the CanvasOverlay fills the entire screen
        rect = GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        //Ensure the Background Sprite also fills the entire screen
        RectTransform backgroundImageRect = transform.Find("Background").GetComponent<RectTransform>();
        backgroundImageRect.anchorMin = Vector2.zero;
        backgroundImageRect.anchorMax = Vector2.one;
        backgroundImageRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundImageRect.offsetMin = Vector2.zero;
        backgroundImageRect.offsetMax = Vector2.zero;

        //Initialize Background
        backgroundMinAlpha = Opacity.Transparent;
        backgroundMaxAlpha = Opacity.Percent70;
        backgroundImage = transform.Find("Background").GetComponent<Image>();
        backgroundColor = new Color(0f, 0f, 0f, backgroundMinAlpha);
        backgroundImage.color = backgroundColor;

        //Initialize Text
        label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 0.75f); //Place label 75% up the screen
        labelRect.anchorMax = new Vector2(0.5f, 0.75f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.alignment = TextAlignmentOptions.Center;
        labelMinAlpha = Opacity.Transparent;
        labelMaxAlpha = Opacity.Opaque;
        labelColor = new Color(1f, 1f, 1f, labelMinAlpha);
        label.color = labelColor;
        label.fontSize = 48; //Ensure visibility

    }

    public void Show(string text)
    {
        backgroundColor.a = backgroundMaxAlpha;
        backgroundImage.color = backgroundColor;

        labelColor.a = labelMaxAlpha;
        label.color = labelColor;
        label.text = text;
        label.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void Reset()
    {
        StopCoroutine(FadeIn());
        StopCoroutine(FadeOut());

        backgroundColor.a = backgroundMinAlpha;
        backgroundImage.color = backgroundColor;

        labelColor.a = labelMinAlpha;
        label.color = labelColor;
        label.text = "";
        label.transform.eulerAngles = new Vector3(-90, 0, 0);
    }


    public void TriggerFadeIn(string text = "")
    {
        StartCoroutine(FadeIn(text));
    }

    private IEnumerator FadeIn(string text = "")
    {
        float elapsedTime = 0f;

        //Set initial values
        float startAngle = -90f;
        float endAngle = 0f;

        if (!string.IsNullOrWhiteSpace(text))
        {
            label.text = text;
            label.transform.eulerAngles = new Vector3(startAngle, 0, 0);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            //Update background alpha
            var alpha = Mathf.Lerp(backgroundMinAlpha, backgroundMaxAlpha, t);
            backgroundColor.a = alpha;
            backgroundImage.color = backgroundColor;

            //Update label rotation (rotate faster)
            float angle = Mathf.Lerp(startAngle, endAngle, t * rotateMultiplier);
            label.transform.eulerAngles = new Vector3(angle, 0, 0);

            yield return Wait.OneTick();
        }

        //Ensure final values
        backgroundColor.a = backgroundMaxAlpha;
        backgroundImage.color = backgroundColor;
        label.transform.eulerAngles = Vector3.zero;
    }

    public void TriggerFadeOut(float delay = 0)
    {
        StartCoroutine(FadeOut(delay));
    }

    private IEnumerator FadeOut(float delay = 0)
    {
        if (delay > 0)
            yield return Wait.For(delay);

        float elapsedTime = 0f;

        //Set initial values
        float alpha = backgroundMaxAlpha;
        float startAngle = 0f;
        float endAngle = -90f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            //Update background alpha
            alpha = Mathf.Lerp(backgroundMaxAlpha, backgroundMinAlpha, t);
            backgroundColor.a = alpha;
            backgroundImage.color = backgroundColor;

            //Update label rotation (rotate faster)
            float angle = Mathf.Lerp(startAngle, endAngle, t * rotateMultiplier);
            label.transform.eulerAngles = new Vector3(angle, 0, 0);

            yield return Wait.UntilNextFrame();
        }

        //Ensure final values
        backgroundColor.a = backgroundMinAlpha;
        backgroundImage.color = backgroundColor;
        label.transform.eulerAngles = new Vector3(endAngle, 0, 0);
    }

    public void UpdateText(string text, Color color = default)
    {
        label.text = text;
        label.color = color == default ? ColorHelper.Solid.White : color;
    }
}
