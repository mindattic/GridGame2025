using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class RosterSlideInstance : MonoBehaviour
{
    [Tooltip("Unique key used to register this slide in the carousel.")]
    public string Key;

    [Tooltip("Manually override width of this slide.")]
    public float Width = 1000f;

    [Tooltip("Manually override height of this slide.")]
    public float Height = 1000f;

    public RectTransform rectTransform;
    public UnityEngine.UI.Image image;
    public Button button;

    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
    }

    public void Initialize(Sprite sprite, System.Action onClick = null)
    {
        image.sprite = sprite;
        if (onClick == null || !image.sprite.texture.isReadable) 
            return;

        image.alphaHitTestMinimumThreshold = 0.1f;
        button.enabled = true;
        button.onClick.AddListener(() => onClick.Invoke());
    }

}
