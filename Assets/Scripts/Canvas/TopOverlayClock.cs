using UnityEngine;
using TMPro;

/// <summary>
/// Displays a live clock pinned to the upper-left corner of the TopOverlay.
/// Ignores timescale, updates once per second, and never blocks raycasts.
/// </summary>
[DisallowMultipleComponent]
public sealed class TopOverlayClock : MonoBehaviour
{
    [Header("Clock Settings")]
    [SerializeField] private string timeFormat = "h:mm tt";
    [SerializeField] private int fontSize = 24;
    [SerializeField] private Color fontColor = Color.white;
    [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Left;

    private TMP_Text clockText;
    private RectTransform clockRect;
    private float nextClockTickTime;

    private void OnEnable()
    {
        EnsureClockObject();
        LayoutClock();
        UpdateClock(force: true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            LayoutClock();
            UpdateClock(force: true);
            return;
        }
#endif
        if (Application.isPlaying)
            TickClock();
    }

    /// <summary>
    /// Ensures the TMP text object exists and is configured.
    /// </summary>
    private void EnsureClockObject()
    {
        if (clockText != null) return;

        Transform child = transform.Find("ClockText");
        if (child != null)
            clockText = child.GetComponent<TMP_Text>();

        if (clockText == null)
        {
            var go = new GameObject("ClockText", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(transform, false);
            clockText = go.GetComponent<TMP_Text>();
        }

        clockRect = clockText.GetComponent<RectTransform>();

        clockText.raycastTarget = false;
        clockText.enableWordWrapping = false;
        clockText.fontSize = fontSize;
        clockText.color = fontColor;
        clockText.alignment = alignment;
        clockText.text = string.Empty;
    }

    /// <summary>
    /// Positions and sizes the clock in the upper-left corner of the overlay.
    /// </summary>
    private void LayoutClock()
    {
        if (clockRect == null) return;

        clockRect.anchorMin = new Vector2(0f, 1f);
        clockRect.anchorMax = new Vector2(0f, 1f);
        clockRect.pivot = new Vector2(0f, 1f);
        clockRect.sizeDelta = new Vector2(240f, fontSize + 6f);
    }

    /// <summary>
    /// Updates the clock text every second.
    /// </summary>
    private void TickClock()
    {
        if (Time.unscaledTime >= nextClockTickTime)
        {
            UpdateClock(force: false);
            nextClockTickTime = Mathf.Floor(Time.unscaledTime) + 1f;
        }
    }

    /// <summary>
    /// Formats and applies the current system time to the text.
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
