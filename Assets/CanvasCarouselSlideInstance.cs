using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class CanvasCarouselSlideInstance : MonoBehaviour
{
    [Tooltip("Unique key used to register this slide in the carousel.")]
    public string Key;

    [Tooltip("Manually override width of this slide.")]
    public float Width = 1000f;

    [Tooltip("Manually override height of this slide.")]
    public float Height = 1000f;

    public RectTransform Rect => GetComponent<RectTransform>();

    private void Awake()
    {
        // Apply manual dimensions to RectTransform
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
    }
}
