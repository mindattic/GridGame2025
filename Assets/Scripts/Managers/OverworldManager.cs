using Assets.Helper;
using Assets.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;

public class OverworldManager : MonoBehaviour, IBeginDragHandler
{
    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private RectTransform map;
    private RawImage mapImage;

    private OverworldHero hero;

    private RectTransform offscreenArrow;
    private Image offscreenArrowImage;

    private VirtualJoystick virtualJoystick;
    private RectTransform joystickRect;

    [SerializeField] private bool hasRandomEncounters = true;
    [SerializeField] private float indicatorPadding = 24f; // distance from viewport edge
    [SerializeField] private float arrowFadeSpeed = 8f;    // alpha units/sec (0..1)
    private float arrowTargetAlpha;                        // 0 when visible, 1 when off-screen

    private Coroutine centeringRoutine;
    private float centeringSpeed = 8f;
    private bool followHero;        // used when joystick is active
    private bool lastAnalogActive;  // edge detection

    // Random encounter
    private float encounterTimer;                       // accumulates only while moving
    private const float encounterIntervalSeconds = 3f;  // trigger threshold
    private bool movedThisFrame;                        // set by HandleHeroMoved each frame
    private bool isLoadingEncounter;                    // prevent double loads

    // Tap vs Drag detection
    private bool pointerDownAllowed; // true if not over joystick
    private Vector2 pointerDownPos;
    private float pointerDownTime;
    private const float tapMaxTime = 0.30f;
    private const float tapMaxSqrDistance = 12f * 12f;

    private const float centeringSnapThreshold = 0.01f; // when to snap to target
   
    private void Awake()
    {
        if (!ProfileHelper.HasProfiles())
            return;

        //Validate a current profile exists
        if (!ProfileHelper.HasCurrentProfile)
        {
            Debug.LogError("No current profile selected.");
            scene.Change.ToProfileCreate();
            return;
        }

        //Validate a current save exists
        if (!ProfileHelper.HasCurrentSave)
        {
            Debug.LogError("No current save selected.");
            scene.Change.ToSaveFileSelect();
            return;
        }

        GameObject go;

        // Gather references safely
        go = GameObject.Find(GameObjectHelper.Overworld.ScrollView);
        scrollRect = go.GetComponent<ScrollRect>();
        viewport = GameObject.Find(GameObjectHelper.Overworld.Viewport)?.GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.Overworld.Content)?.GetComponent<RectTransform>();

        go = GameObject.Find(GameObjectHelper.Overworld.Map);
        map = go.GetComponent<RectTransform>();
        mapImage = go.GetComponent<RawImage>();

        go = GameObject.Find(GameObjectHelper.Overworld.OffscreenArrow);
        offscreenArrow = go.GetComponent<RectTransform>();
        offscreenArrowImage = go.GetComponent<Image>();

        hero = GameObject.Find(GameObjectHelper.Overworld.Hero)?.GetComponent<OverworldHero>();
        virtualJoystick = GameObject.Find(GameObjectHelper.Overworld.VirtualJoystick)?.GetComponent<VirtualJoystick>();
        joystickRect = virtualJoystick != null ? virtualJoystick.GetComponent<RectTransform>() : null;

        // Wire ScrollRect if available
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.horizontal = true;
        scrollRect.vertical = true;

        // Layout constraints
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(0f, 1f);
        content.pivot = new Vector2(0f, 1f);

        // Map is top-left anchored
        map.anchorMin = new Vector2(0f, 1f);
        map.anchorMax = new Vector2(0f, 1f);
        map.pivot = new Vector2(0f, 1f);
        map.anchoredPosition = Vector2.zero;

        // Load map sprite from profile and size map/content to match
        var overworld = ProfileHelper.CurrentProfile.CurrentSave.Overworld;
        var sprite = MapLibrary.Get(overworld.MapName); // Sprite expected
        var t = sprite.texture;
        var r = sprite.rect;
        mapImage.texture = t;
        mapImage.uvRect = new Rect(
            r.x / t.width, r.y / t.height,
            r.width / t.width, r.height / t.height
        );
        map.sizeDelta = r.size;

        // Size content to match map
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, map.sizeDelta.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, map.sizeDelta.y);

        // Wire hero
        if (hero != null)
        {
            hero.AllowClickToMove = true; // enable click handling
            hero.OnHeroMoved += HandleHeroMoved;
            hero.rect.anchoredPosition = new Vector2(overworld.HeroX, overworld.HeroY);
            hero.SetFacing(overworld.HeroDirection);
        }

        // Off-screen arrow config
        SetOffscreenArrowAlpha(0f);

        ConfigureScrollBounds();

        // Snap viewport to hero location immediately (no tween)
        if (hero != null)
            SnapCentering(hero.rect.anchoredPosition);
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

        bool isDirectional = hero != null && hero.TouchMoveMode == OverworldHeroTouchMode.DirectionalClick;

        // Touch (hold-to-move in directional mode)
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                pointerDownAllowed = !IsOverJoystick(t.position);
                pointerDownPos = t.position;
                pointerDownTime = Time.unscaledTime;

                if (pointerDownAllowed && isDirectional)
                    hero.BeginDirectionalFromScreen(t.position, content);
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                if (pointerDownAllowed && isDirectional)
                    hero.UpdateDirectionalFromScreen(t.position, content);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                if (isDirectional)
                {
                    hero.EndDirectional();
                }
                else if (pointerDownAllowed && IsTap(t.position) && !IsOverJoystick(t.position))
                {
                    HandleTap(t.position);
                }
            }
            return;
        }

        // Mouse (hold-to-move in directional mode)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            pointerDownAllowed = !IsOverJoystick(pos);
            pointerDownPos = pos;
            pointerDownTime = Time.unscaledTime;

            if (pointerDownAllowed && isDirectional)
                hero.BeginDirectionalFromScreen(pos, content);
        }
        if (Input.GetMouseButton(0))
        {
            if (pointerDownAllowed && isDirectional)
            {
                Vector2 pos = (Vector2)Input.mousePosition;
                hero.UpdateDirectionalFromScreen(pos, content);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            if (isDirectional)
            {
                hero.EndDirectional();
            }
            else if (pointerDownAllowed && IsTap(pos) && !IsOverJoystick(pos))
            {
                HandleTap(pos);
            }
        }
    }

    private void LateUpdate()
    {
        // If directional mode, always follow hero and disable dragging
        bool directionalMode = hero != null && hero.TouchMoveMode == OverworldHeroTouchMode.DirectionalClick;

        if (directionalMode)
        {
            if (scrollRect != null)
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = false;
            }
        }
        else
        {
            if (scrollRect != null)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = true;
            }
        }

        // Feed analog input to the hero every frame
        Vector2 stick = virtualJoystick != null ? virtualJoystick.Direction : Vector2.zero;
        bool analogActive = stick.sqrMagnitude > 1e-6f;

        if (hero != null)
            hero.SetAnalogInput(stick);

        // On press outside deadzone -> center and start following
        if (!directionalMode && analogActive && !lastAnalogActive && hero != null && hero.rect != null)
        {
            SmoothCentering(hero.rect.anchoredPosition);
            followHero = true;
        }
        // On release -> stop following and stop any centering tween
        if (!directionalMode && !analogActive && lastAnalogActive)
        {
            followHero = false;
            CancelCentering();
        }
        lastAnalogActive = analogActive;

        // Random encounter timer
        if (movedThisFrame)
        {
            encounterTimer += Time.deltaTime;
            if (encounterTimer >= encounterIntervalSeconds)
            {
                encounterTimer = 0f;
                TriggerRandomEncounter();
            }
        }
        else
        {
            encounterTimer = 0f;
        }
        movedThisFrame = false;

        // Hard-follow centering when in directional mode
        if (directionalMode && hero != null && hero.rect != null && scrollRect != null)
        {
            scrollRect.normalizedPosition = GetMapPosition(hero.rect.anchoredPosition);
        }

        UpdateOffscreenArrow();
    }

    private bool IsTap(Vector2 releasePos)
    {
        if (Time.unscaledTime - pointerDownTime > tapMaxTime) return false;
        return (releasePos - pointerDownPos).sqrMagnitude <= tapMaxSqrDistance;
    }

    private bool IsOverJoystick(Vector2 screenPos)
    {
        return joystickRect != null && RectTransformUtility.RectangleContainsScreenPoint(joystickRect, screenPos, null);
    }

    private void HandleTap(Vector2 screenPos)
    {
        if (hero == null || hero.rect == null || content == null) return;
        hero.HandleClickScreen(screenPos, content); // delegate to hero; mode decides behavior
    }

    private void OnRectTransformDimensionsChange()
    {
        ConfigureScrollBounds();
    }

    private IEnumerator ConfigureBoundsRoutine()
    {
        yield return null; // wait one frame so map.rect is valid
        ConfigureScrollBounds();
    }

    private void ConfigureScrollBounds()
    {
        if (scrollRect == null || viewport == null || content == null || map == null) return;

        Vector2 mapSize = map.rect.size;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapSize.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapSize.y);
    }

    public void OnBackButtonClicked()
    {
        scene.Change.ToPreviousScene();
    }

    public void OnCenterOnHeroClicked()
    {
        if (hero == null || hero.rect == null) return;
        SmoothCentering(hero.rect.anchoredPosition);
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

    public void SmoothCentering(Vector2 position)
    {
        CancelCentering();
        centeringRoutine = StartCoroutine(SmoothCenteringRoutine(position));
    }

    private IEnumerator SmoothCenteringRoutine(Vector2 targetLocalPosition)
    {
        Vector2 targetPosition = GetMapPosition(targetLocalPosition);
        while (scrollRect != null && Vector2.Distance(scrollRect.normalizedPosition, targetPosition) > centeringSnapThreshold)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetPosition, Time.deltaTime * centeringSpeed);
            yield return null;
        }
        if (scrollRect != null) scrollRect.normalizedPosition = targetPosition;
        centeringRoutine = null;
    }

    // Instantly center the viewport on the given local position (no tween)
    public void SnapCentering(Vector2 position)
    {
        CancelCentering();
        if (scrollRect == null) return;

        Vector2 targetPosition = GetMapPosition(position);
        scrollRect.normalizedPosition = targetPosition;
    }

    // Cancel any ongoing centering coroutine
    private void CancelCentering()
    {
        if (centeringRoutine != null)
        {
            StopCoroutine(centeringRoutine);
            centeringRoutine = null;
        }
    }

    // IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        CancelCentering();
    }

    // Follow hero while moving and flag movement for encounter timer
    private void HandleHeroMoved(Vector2 heroLocalPos)
    {
        movedThisFrame = true;

        if (!followHero || scrollRect == null) return;
        scrollRect.normalizedPosition = GetMapPosition(heroLocalPos);
    }

    // Update the offscreen arrow position/alpha to point toward the hero
    private void UpdateOffscreenArrow()
    {
        if (offscreenArrow == null || offscreenArrowImage == null || viewport == null || hero == null || hero.rect == null)
            return;

        bool heroVisible = OverworldHero.IsTargetVisible(hero.rect, viewport);

        arrowTargetAlpha = heroVisible ? 0f : 1f;
        float current = offscreenArrowImage.color.a;
        float next = Mathf.MoveTowards(current, arrowTargetAlpha, arrowFadeSpeed * Time.unscaledDeltaTime);
        if (!Mathf.Approximately(current, next))
            SetOffscreenArrowAlpha(next);

        if (!heroVisible || next > 0.001f)
        {
            // Hero position in viewport local space
            Vector3 heroWorldCenter = hero.rect.TransformPoint(hero.rect.rect.center);
            Vector2 heroLocal = viewport.InverseTransformPoint(heroWorldCenter);

            // Viewport rect in local space
            Vector3[] vc = new Vector3[4];
            viewport.GetLocalCorners(vc);
            Vector2 min = vc[0];
            Vector2 max = vc[2];
            Vector2 center = (min + max) * 0.5f;
            Vector2 extents = (max - min) * 0.5f;

            Vector2 dir = heroLocal - center;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;

            float pad = Mathf.Max(0f, indicatorPadding);
            Vector2 ex = new Vector2(Mathf.Max(0.001f, extents.x - pad), Mathf.Max(0.001f, extents.y - pad));
            float sx = Mathf.Abs(dir.x) > 0.0001f ? ex.x / Mathf.Abs(dir.x) : float.PositiveInfinity;
            float sy = Mathf.Abs(dir.y) > 0.0001f ? ex.y / Mathf.Abs(dir.y) : float.PositiveInfinity;
            float s = Mathf.Min(sx, sy);

            offscreenArrow.anchoredPosition = dir * s;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // arrow graphic points right
            offscreenArrow.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    // Helper to set arrow alpha
    private void SetOffscreenArrowAlpha(float a)
    {
        Color c = offscreenArrowImage.color;
        c.a = Mathf.Clamp01(a);
        offscreenArrowImage.color = c;
    }

    // Trigger scene change to a random stage after sustained movement
    private void TriggerRandomEncounter()
    {
        if (isLoadingEncounter) return;
        if (StageLibrary.Stages == null || StageLibrary.Stages.Count == 0) return;

        string mapName = ProfileHelper.Overworld.MapName;

        // Persist overworld location and facing
        if (hero != null && hero.rect != null)
        {
            ProfileHelper.CurrentProfile.LatestSave.Overworld.MapName = mapName;
            ProfileHelper.CurrentProfile.LatestSave.Overworld.HeroX = hero.rect.anchoredPosition.x;
            ProfileHelper.CurrentProfile.LatestSave.Overworld.HeroY = hero.rect.anchoredPosition.y;
            ProfileHelper.CurrentProfile.LatestSave.Overworld.HeroDirection = hero.CurrentFacingName ?? "Idle";
            ProfileHelper.SaveOverworldPosition(hero.rect.anchoredPosition, mapName, hero.CurrentFacingName ?? "Idle");
        }

        // Get a random stage for this map from RNG
        string stageName = RNG.Stage(mapName);
        ProfileHelper.CurrentProfile.LatestSave.Stage.CurrentStage = stageName;

        isLoadingEncounter = true;
        scene.Change.ToGame();
    }
}