using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;
using scene = Assets.Helpers.SceneHelper;

public class OverworldManager : MonoBehaviour, IBeginDragHandler
{
    private RectTransform scrollView;
    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private RectTransform map;
    private PlayerStageMover hero;

    // Off-screen arrow (optional; will be auto-found)
    [SerializeField] private RectTransform offscreenArrow; // assign in Inspector or found by path
    private Image offscreenArrowImage;

    // Indicator settings
    [SerializeField] private float indicatorPadding = 24f; // distance from viewport edge
    [SerializeField] private float arrowFadeSpeed = 8f;    // alpha units/sec (0..1)
    private float arrowTargetAlpha;                        // 0 when visible, 1 when off-screen

    [SerializeField] private Label centerModeLabel;

    private Coroutine centeringRoutine;
    private bool hasCenteredInitially;
    private bool followHero = false; // default: free scroll (do not follow)

    // Tap vs Drag detection
    private bool pointerDownOnMap;
    private Vector2 pointerDownPos;
    private float pointerDownTime;
    private const float tapMaxTime = 0.30f;
    private const float tapMaxSqrDistance = 12f * 12f;

    private void Awake()
    {
        // Gather references safely
        scrollView = GameObject.Find(GameObjectHelper.Overworld.ScrollView)?.GetComponent<RectTransform>();
        scrollRect = GameObject.Find(GameObjectHelper.Overworld.ScrollView)?.GetComponent<ScrollRect>();
        viewport = GameObject.Find(GameObjectHelper.Overworld.Viewport)?.GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.Overworld.Content)?.GetComponent<RectTransform>();
        map = GameObject.Find(GameObjectHelper.Overworld.Map)?.GetComponent<RectTransform>();
        hero = GameObject.Find(GameObjectHelper.Overworld.Hero)?.GetComponent<PlayerStageMover>();

        if (offscreenArrow == null)
            offscreenArrow = GameObject.Find(GameObjectHelper.Overworld.OffscreenArrow)?.GetComponent<RectTransform>();

        // Fallbacks for naming mismatch (OffScreenArrow vs OffscreenArrow)
        if (offscreenArrow == null && viewport != null)
        {
            var t = viewport.Find("OffScreenArrow") ?? viewport.Find("OffscreenArrow");
            if (t != null) offscreenArrow = t as RectTransform;
        }

        // Resolve Image even if only RectTransform was found
        if (offscreenArrowImage == null)
        {
            if (offscreenArrow != null)
                offscreenArrowImage = offscreenArrow.GetComponent<Image>();
            else
                offscreenArrowImage = GameObject.Find(GameObjectHelper.Overworld.OffscreenArrow)?.GetComponent<Image>();
        }

        // Wire ScrollRect if available
        if (scrollRect != null && viewport != null && content != null)
        {
            scrollRect.viewport = viewport;
            scrollRect.content = content;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;
            scrollRect.horizontal = true;
            scrollRect.vertical = true;
            if (viewport.GetComponent<RectMask2D>() == null)
                viewport.gameObject.AddComponent<RectMask2D>();
        }
        else
        {
            if (scrollRect == null) Debug.LogWarning("OverworldManager: ScrollRect not found.");
            if (viewport == null) Debug.LogWarning("OverworldManager: Viewport not found.");
            if (content == null) Debug.LogWarning("OverworldManager: Content not found.");
        }

        // Layout constraints
        if (content != null)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 1f);
        }
        if (map != null)
        {
            map.anchorMin = new Vector2(0f, 1f);
            map.anchorMax = new Vector2(0f, 1f);
            map.pivot = new Vector2(0f, 1f);
            map.anchoredPosition = Vector2.zero;

            if (content != null)
            {
                Vector2 mapSize = map.rect.size;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapSize.x);
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapSize.y);
            }
        }
        else
        {
            Debug.LogWarning("OverworldManager: Map not found.");
        }

        if (hero != null)
        {
            hero.AllowClickToMove = true;
            hero.OnHeroMoved += HandleHeroMoved;
        }
        else
        {
            Debug.LogWarning("OverworldManager: Hero not found.");
        }

        // Configure the indicator (center anchors, always active, start invisible)
        if (offscreenArrow != null)
        {
            offscreenArrow.anchorMin = new Vector2(0.5f, 0.5f);
            offscreenArrow.anchorMax = new Vector2(0.5f, 0.5f);
            offscreenArrow.pivot = new Vector2(0.5f, 0.5f);
            offscreenArrow.gameObject.SetActive(true); // never disable
            offscreenArrow.SetAsLastSibling();         // draw above Content
        }
        else
        {
            Debug.LogWarning("OverworldManager: OffscreenArrow not found under Viewport. Assign it in the Inspector or name it 'OffscreenArrow'.");
        }

        if (offscreenArrowImage != null)
        {
            // Ensure it doesn't block clicks and start transparent
            offscreenArrowImage.raycastTarget = false;
            SetArrowAlpha(0f);
        }
        else
        {
            Debug.LogWarning("OverworldManager: OffscreenArrow Image component not found.");
        }

        // Defer size/bounds setup
        StartCoroutine(ConfigureBoundsRoutine());

    }

    private void OnDestroy()
    {
        if (hero != null) hero.OnHeroMoved -= HandleHeroMoved;
    }

    private void Start()
    {
        scene.FadeIn();
    }

    private void Update()
    {
        if (map == null) return;

        // Touch
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                if (map != null)
                    pointerDownOnMap = RectTransformUtility.RectangleContainsScreenPoint(map, t.position, null);
                pointerDownPos = t.position; pointerDownTime = Time.unscaledTime;
            }
            else if (t.phase == TouchPhase.Ended && pointerDownOnMap && IsTap(t.position)) HandleTap(t.position);
            return;
        }
        // Mouse
        if (Input.GetMouseButtonDown(0))
        {
            var pos = (Vector2)Input.mousePosition;
            if (map != null)
                pointerDownOnMap = RectTransformUtility.RectangleContainsScreenPoint(map, pos, null);
            pointerDownPos = pos; pointerDownTime = Time.unscaledTime;
        }
        if (Input.GetMouseButtonUp(0))
        {
            var pos = (Vector2)Input.mousePosition;
            if (pointerDownOnMap && IsTap(pos)) HandleTap(pos);
        }
    }

    // Run after layout/movement so the indicator updates even when touch logic early-returns
    private void LateUpdate()
    {
        UpdateOffscreenArrow();
    }

    private bool IsTap(Vector2 releasePos)
    {
        if (Time.unscaledTime - pointerDownTime > tapMaxTime) return false;
        return (releasePos - pointerDownPos).sqrMagnitude <= tapMaxSqrDistance;
    }

    private void HandleTap(Vector2 screenPos)
    {
        if (hero == null || hero.rect == null || content == null) return;

        Vector2 local = UnitConversionHelper.Screen.ToCanvas(content, screenPos);
        hero.SetDestinationLocal(local);
    }

    private void OnRectTransformDimensionsChange() { ConfigureScrollBounds(); }

    private IEnumerator ConfigureBoundsRoutine()
    {
        yield return null; // wait one frame so map.rect is valid
        ConfigureScrollBounds();
        hasCenteredInitially = true;
    }

    private void ConfigureScrollBounds()
    {
        if (scrollRect == null || viewport == null || content == null || map == null) return;

        Vector2 mapSize = map.rect.size;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapSize.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapSize.y);
    }

    public void OnBackButtonClicked() { scene.Change.ToPreviousScene(); }

    // One-shot center helper (does not toggle follow; still allows scrolling after)
    public void OnCenterOnHeroClicked()
    {
        if (hero == null || hero.rect == null) return;
        CenterOnPosition(hero.rect.anchoredPosition, 8f, 0.01f);
    }

    private Vector2 GetMapPosition(Vector2 position)
    {
        if (viewport == null || content == null) return new Vector2(0.5f, 0.5f);

        Vector2 vp = viewport.rect.size, cs = content.rect.size;
        float x = position.x;
        float y = -position.y;
        float maxLeft = Mathf.Max(0f, cs.x - vp.x), maxTop = Mathf.Max(0f, cs.y - vp.y);
        float desiredLeft = Mathf.Clamp(x - vp.x * 0.5f, 0f, maxLeft);
        float desiredTop = Mathf.Clamp(y - vp.y * 0.5f, 0f, maxTop);
        float nx = maxLeft <= 0.001f ? 0.5f : desiredLeft / maxLeft;
        float ny = maxTop <= 0.001f ? 0.5f : 1f - (desiredTop / maxTop);
        return new Vector2(nx, ny);
    }

    public void CenterOnPosition(Vector2 targetLocalPosition, float speed, float snapThreshold)
    {
        CancelCentering();
        centeringRoutine = StartCoroutine(SmoothCenteringRoutine(targetLocalPosition, speed, snapThreshold));
    }

    private IEnumerator SmoothCenteringRoutine(Vector2 targetLocalPosition, float speed, float snapThreshold)
    {
        Vector2 targetPosition = GetMapPosition(targetLocalPosition);
        while (scrollRect != null && Vector2.Distance(scrollRect.normalizedPosition, targetPosition) > snapThreshold)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetPosition, Time.deltaTime * speed);
            yield return Wait.None();
        }
        if (scrollRect != null) scrollRect.normalizedPosition = targetPosition;
        centeringRoutine = null;
    }

    private void CancelCentering() { if (centeringRoutine != null) { StopCoroutine(centeringRoutine); centeringRoutine = null; } }

    public void OnBeginDrag(PointerEventData eventData) { CancelCentering(); }

    private void HandleHeroMoved(Vector2 heroLocalPos)
    {
        if (!followHero || scrollRect == null) return;
        scrollRect.normalizedPosition = GetMapPosition(heroLocalPos);
    }

    // Shows an arrow on the viewport edge pointing toward the hero when off-screen; fades in/out instead of SetActive
    private void UpdateOffscreenArrow()
    {
        if (offscreenArrow == null || offscreenArrowImage == null || viewport == null || hero == null || hero.rect == null)
            return;

        bool heroVisible = PlayerStageMover.IsTargetVisible(hero.rect, viewport);

        // Drive fade target
        arrowTargetAlpha = heroVisible ? 0f : 1f;

        // Fade alpha
        float current = offscreenArrowImage.color.a;
        float next = Mathf.MoveTowards(current, arrowTargetAlpha, arrowFadeSpeed * Time.unscaledDeltaTime);
        if (!Mathf.Approximately(current, next))
            SetArrowAlpha(next);

        // Only update placement when hero is off-screen (or while fading in)
        if (!heroVisible || next > 0.001f)
        {
            // Hero center in viewport local space
            Vector3 heroWorldCenter = hero.rect.TransformPoint(hero.rect.rect.center);
            Vector2 heroLocal = viewport.InverseTransformPoint(heroWorldCenter);

            // Viewport rect in local space
            Vector3[] vc = new Vector3[4];
            viewport.GetLocalCorners(vc);
            Vector2 min = vc[0];           // bottom-left
            Vector2 max = vc[2];           // top-right
            Vector2 center = (min + max) * 0.5f;
            Vector2 extents = (max - min) * 0.5f;

            // Direction from center to hero (local)
            Vector2 dir = heroLocal - center;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;

            // Shrink extents by padding and scale dir to the edge
            float pad = Mathf.Max(0f, indicatorPadding);
            Vector2 ex = new Vector2(Mathf.Max(0.001f, extents.x - pad), Mathf.Max(0.001f, extents.y - pad));
            float sx = Mathf.Abs(dir.x) > 0.0001f ? ex.x / Mathf.Abs(dir.x) : float.PositiveInfinity;
            float sy = Mathf.Abs(dir.y) > 0.0001f ? ex.y / Mathf.Abs(dir.y) : float.PositiveInfinity;
            float s = Mathf.Min(sx, sy);

            // Place relative to the viewport center (indicator anchored to center)
            offscreenArrow.anchoredPosition = dir * s;

            // Rotate to point at hero (assumes arrow graphic points "right")
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            offscreenArrow.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void SetArrowAlpha(float a)
    {
        if (offscreenArrowImage == null) return;
        Color c = offscreenArrowImage.color;
        c.a = Mathf.Clamp01(a);
        offscreenArrowImage.color = c;
    }
}