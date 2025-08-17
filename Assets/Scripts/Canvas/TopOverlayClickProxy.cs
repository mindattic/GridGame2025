using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Stretches over the unsafe top inset and forwards any tap in that area to a target PauseButton.
/// Keeps the overlay visible (or transparent) and raycastable so the whole notch strip is clickable.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class TopOverlayClickProxy : MonoBehaviour, IPointerClickHandler
{
    [Header("Targets")]
    [SerializeField] private RectTransform topOverlay;
    [SerializeField] private Button pauseButton;

    [Header("Appearance")]
    [SerializeField] private Color overlayColor = Color.black;
    [SerializeField] private bool overlayVisible = true;

    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;
    private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    private void OnEnable()
    {
        if (topOverlay == null) topOverlay = GetComponent<RectTransform>();
        Apply(force: true);
        EnsureRaycastableImage();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Apply(force: true);
            return;
        }
#endif
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenSize.x ||
            Screen.height != lastScreenSize.y ||
            Screen.orientation != lastOrientation)
        {
            Apply(force: false);
        }
    }

    /// <summary>
    /// Sizes the overlay to the top inset above the safe area.
    /// </summary>
    private void Apply(bool force)
    {
        if (topOverlay == null) return;

        Rect safe = Screen.safeArea;
        Vector2Int size = new Vector2Int(Screen.width, Screen.height);
        ScreenOrientation orient = Screen.orientation;

        if (!force && safe == lastSafeArea && size == lastScreenSize && orient == lastOrientation)
            return;

        lastSafeArea = safe;
        lastScreenSize = size;
        lastOrientation = orient;

        topOverlay.anchorMin = new Vector2(0f, 1f);
        topOverlay.anchorMax = new Vector2(1f, 1f);
        topOverlay.pivot = new Vector2(0.5f, 1f);

        if (size.x <= 0 || size.y <= 0) return;

        float topInsetPixels = Mathf.Max(0f, size.y - safe.yMax);

        topOverlay.offsetMin = new Vector2(0f, -topInsetPixels);
        topOverlay.offsetMax = new Vector2(0f, 0f);
    }

    /// <summary>
    /// Makes sure there is an Image to receive raycasts and sets its visibility.
    /// </summary>
    private void EnsureRaycastableImage()
    {
        var img = topOverlay != null ? topOverlay.GetComponent<Image>() : null;
        if (img == null && topOverlay != null)
        {
            img = topOverlay.gameObject.AddComponent<Image>();
            img.sprite = null;
            img.type = Image.Type.Simple;
        }

        if (img != null)
        {
            img.raycastTarget = true;
            img.color = overlayVisible ? overlayColor : new Color(0f, 0f, 0f, 0f);
        }

        var cg = topOverlay != null ? topOverlay.GetComponent<CanvasGroup>() : null;
        if (cg != null) cg.blocksRaycasts = true;
    }

    /// <summary>
    /// Forwards any tap in the top overlay to the PauseButton.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pauseButton != null && pauseButton.interactable)
            pauseButton.onClick.Invoke();
    }
}
