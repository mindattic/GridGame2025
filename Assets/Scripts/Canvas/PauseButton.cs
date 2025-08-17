using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps a PauseButton hot zone pinned to the top-right of TopOverlay and
/// sizes it using the top safe-area inset. Icon remains centered and non-raycastable.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class PauseHotzone : MonoBehaviour
{
    [SerializeField] private RectTransform pauseButton;   // parent button
    [SerializeField] private RectTransform icon;          // child visible icon
    [SerializeField] private float width = 160f;          // hot zone width
    [SerializeField] private float minHeight = 48f;       // minimum hot zone height
    [SerializeField] private Vector2 margin = new Vector2(8f, 8f);

    private Rect lastSafe;
    private Vector2Int lastSize;

    private void OnEnable() { Apply(true); }
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) { Apply(true); return; }
#endif
        if (Screen.safeArea != lastSafe || lastSize.x != Screen.width || lastSize.y != Screen.height)
            Apply(false);
    }

    private void Apply(bool force)
    {
        if (pauseButton == null) return;

        lastSafe = Screen.safeArea;
        lastSize = new Vector2Int(Screen.width, Screen.height);

        // Anchor PauseButton to top-right
        pauseButton.anchorMin = new Vector2(1f, 1f);
        pauseButton.anchorMax = new Vector2(1f, 1f);
        pauseButton.pivot = new Vector2(1f, 1f);
        pauseButton.anchoredPosition = new Vector2(-margin.x, -margin.y);

        // Height equals top inset, with a floor
        float inset = Mathf.Max(0f, Screen.height - Screen.safeArea.yMax);
        float h = Mathf.Max(minHeight, inset);

        pauseButton.sizeDelta = new Vector2(width, h);

        // Make the button graphic invisible but raycastable
        var btnImg = pauseButton.GetComponent<Image>();
        if (btnImg != null)
        {
            btnImg.sprite = null;
            btnImg.color = new Color(0f, 0f, 0f, 0f);
            btnImg.raycastTarget = true;
        }

        // Center the icon and ensure it does not block clicks
        if (icon != null)
        {
            icon.anchorMin = icon.anchorMax = icon.pivot = new Vector2(0.5f, 0.5f);
            var iconImg = icon.GetComponent<Image>();
            if (iconImg != null) iconImg.raycastTarget = false;
        }
    }
}
