using UnityEngine;

[ExecuteAlways]
public class ScalableButton : MonoBehaviour
{
    [Header("Edges")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("Corners")]
    public RectTransform topLeft;
    public RectTransform topRight;
    public RectTransform bottomLeft;
    public RectTransform bottomRight;

    [Header("Core")]
    public RectTransform background;
    public RectTransform label;

    [Header("Sizes")]
    public float cornerSize = 16f;
    public float edgeThickness = 16f;

#if UNITY_EDITOR
    void Update()
    {
        // Only apply layout continuously in the editor (not during play mode)
        if (!Application.isPlaying)
            ApplyLayout();
    }
#endif

    void Start() => ApplyLayout();
    void OnValidate() => ApplyLayout();

    void ApplyLayout()
    {
        RectTransform root = GetComponent<RectTransform>();
        if (root.rect.width == 0 || root.rect.height == 0)
            return; // prevent divide by zero before layout is ready

        float minAnchor = cornerSize / root.rect.width;
        float minAnchorY = cornerSize / root.rect.height;

        // Edges with adjusted anchors
        if (top)
        {
            top.anchorMin = new Vector2(minAnchor, 1);
            top.anchorMax = new Vector2(1 - minAnchor, 1);
            top.pivot = new Vector2(0.5f, 1);
            top.anchoredPosition = Vector2.zero;
            top.sizeDelta = new Vector2(0, edgeThickness);
        }

        if (bottom)
        {
            bottom.anchorMin = new Vector2(minAnchor, 0);
            bottom.anchorMax = new Vector2(1 - minAnchor, 0);
            bottom.pivot = new Vector2(0.5f, 0);
            bottom.anchoredPosition = Vector2.zero;
            bottom.sizeDelta = new Vector2(0, edgeThickness);
        }

        if (left)
        {
            left.anchorMin = new Vector2(0, minAnchorY);
            left.anchorMax = new Vector2(0, 1 - minAnchorY);
            left.pivot = new Vector2(0, 0.5f);
            left.anchoredPosition = Vector2.zero;
            left.sizeDelta = new Vector2(edgeThickness, 0);
        }

        if (right)
        {
            right.anchorMin = new Vector2(1, minAnchorY);
            right.anchorMax = new Vector2(1, 1 - minAnchorY);
            right.pivot = new Vector2(1, 0.5f);
            right.anchoredPosition = Vector2.zero;
            right.sizeDelta = new Vector2(edgeThickness, 0);
        }

        // Corners
        if (topLeft)
        {
            topLeft.anchorMin = new Vector2(0, 1);
            topLeft.anchorMax = new Vector2(0, 1);
            topLeft.pivot = new Vector2(0, 1);
            topLeft.anchoredPosition = Vector2.zero;
            topLeft.sizeDelta = new Vector2(cornerSize, cornerSize);
        }

        if (topRight)
        {
            topRight.anchorMin = new Vector2(1, 1);
            topRight.anchorMax = new Vector2(1, 1);
            topRight.pivot = new Vector2(1, 1);
            topRight.anchoredPosition = Vector2.zero;
            topRight.sizeDelta = new Vector2(cornerSize, cornerSize);
        }

        if (bottomLeft)
        {
            bottomLeft.anchorMin = new Vector2(0, 0);
            bottomLeft.anchorMax = new Vector2(0, 0);
            bottomLeft.pivot = new Vector2(0, 0);
            bottomLeft.anchoredPosition = Vector2.zero;
            bottomLeft.sizeDelta = new Vector2(cornerSize, cornerSize);
        }

        if (bottomRight)
        {
            bottomRight.anchorMin = new Vector2(1, 0);
            bottomRight.anchorMax = new Vector2(1, 0);
            bottomRight.pivot = new Vector2(1, 0);
            bottomRight.anchoredPosition = Vector2.zero;
            bottomRight.sizeDelta = new Vector2(cornerSize, cornerSize);
        }

        // Background (fills inside border)
        if (background)
        {
            background.anchorMin = new Vector2(minAnchor, minAnchorY);
            background.anchorMax = new Vector2(1 - minAnchor, 1 - minAnchorY);
            background.pivot = new Vector2(0.5f, 0.5f);
            background.anchoredPosition = Vector2.zero;
            background.sizeDelta = Vector2.zero;
        }

        // Label (inside padding)
        if (label)
        {
            label.anchorMin = new Vector2(minAnchor, minAnchorY);
            label.anchorMax = new Vector2(1 - minAnchor, 1 - minAnchorY);
            label.pivot = new Vector2(0.5f, 0.5f);
            label.anchoredPosition = Vector2.zero;
            label.sizeDelta = Vector2.zero;
        }
    }
}
