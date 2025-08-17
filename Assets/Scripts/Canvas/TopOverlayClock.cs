using UnityEngine;
using TMPro;

/// <summary>
/// Updates a TextMeshProUGUI with the current system time and anchors it
/// to the upper-left corner of its parent (TopOverlay).
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class TopOverlayClock : MonoBehaviour
{
    [Header("Clock Settings")]
    [SerializeField] private string timeFormat = "h:mm tt";
    [SerializeField] private int fontSize = 24;
    [SerializeField] private Color fontColor = Color.white;
    [SerializeField] private Vector2 padding = new Vector2(80f, 32f);
    [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Left;

    private TMP_Text clockText;
    private RectTransform rect;
    private float nextClockTickTime;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        clockText = GetComponent<TMP_Text>();
        if (clockText == null)
            clockText = gameObject.AddComponent<TextMeshProUGUI>();

        // Configure text
        clockText.raycastTarget = false;
        clockText.enableWordWrapping = false;
        clockText.fontSize = fontSize;
        clockText.color = fontColor;
        clockText.alignment = alignment;

        AnchorToUpperLeft();
        UpdateClock(force: true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            AnchorToUpperLeft();
            UpdateClock(force: true);
            return;
        }
#endif
        if (Application.isPlaying && Time.unscaledTime >= nextClockTickTime)
        {
            UpdateClock(force: false);
            nextClockTickTime = Mathf.Floor(Time.unscaledTime) + 1f;
        }
    }

    /// <summary>
    /// Anchors and positions this RectTransform to the upper-left corner.
    /// </summary>
    private void AnchorToUpperLeft()
    {
        if (rect == null) return;

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);

        rect.anchoredPosition = new Vector2(padding.x, -padding.y);
        rect.sizeDelta = new Vector2(240f, fontSize + 6f);
    }

    /// <summary>
    /// Updates the displayed time string.
    /// </summary>
    private void UpdateClock(bool force)
    {
        if (clockText == null) return;

        string fmt = string.IsNullOrEmpty(timeFormat) ? "h:mm tt" : timeFormat;
        string now = System.DateTime.Now.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);

        if (force || !string.Equals(clockText.text, now))
            clockText.text = now;
    }
}
