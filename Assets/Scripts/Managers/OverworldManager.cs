using Assets.Helper;
using Assets.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;
using scene = Assets.Helpers.SceneHelper;

// OverworldManager orchestrates input and scene transitions for the world-space overworld.
// World rendering uses SpriteRenderers (scaled to 1,1,1) and the camera centers on the hero.
public class OverworldManager : MonoBehaviour
{
    // World layers
    private SpriteRenderer terrainSR;
    private SpriteRenderer surfaceSR;
    private SpriteRenderer canopySR;

    private OverworldHero hero;

    private VirtualJoystick virtualJoystick;
    private RectTransform joystickRect;

    // Input mode UI
    private Button inputModeButton;
    private Image inputModeImage;
    private Label inputModeLabel;

    // Camera mode UI
    private Button cameraModeButton;
    private Image cameraModeImage;
    private Label cameraModeLabel;

    // Offscreen arrow (now handled by its own component)
    private OffscreenArrowIndicator offscreenArrow;

    [SerializeField] private bool hasRandomEncounters = false;

    // Random encounter
    private float encounterTimer;                       // accumulates only while moving
    private const float encounterIntervalSeconds = 3f;  // trigger threshold
    private bool movedThisFrame;                        // set by HandleHeroMoved each frame
    private bool isLoadingEncounter;                    // prevent double loads

    // Tap vs Drag detection
    private bool pointerDownAllowed; // true if not over UI (except joystick)
    private Vector2 pointerDownPos;
    private float pointerDownTime;
    private const float tapMaxTime = 0.30f;
    private const float tapMaxSqrDistance = 12f * 12f;

    // Camera mode and panning
    private OverworldCameraMode cameraMode = OverworldCameraMode.FollowHero;
    private bool isPanning;
    private Vector2 panStartScreen;
    private Vector3 panStartCameraTarget;
    private Vector3 cameraTarget;
    private float panLerpSpeed = 10f; // higher = snappier

    private Camera cam;
    private Transform mapRoot; // parent for map components

    // Mode7 controller (optional)
    private Mode7CameraController mode7;
    private bool Mode7Active
    {
        get
        {
            if (mode7 == null)
            {
                var c = Camera.main;
                if (c != null) mode7 = c.GetComponent<Mode7CameraController>();
            }
            return mode7 != null && mode7.enabled && mode7.enableMode7;
        }
    }

    private void Awake()
    {
        cam = Camera.main;

        if (!ProfileHelper.HasProfiles())
            return;

        if (!ProfileHelper.HasCurrentProfile)
        {
            Debug.LogError("No current profile selected.");
            scene.Change.ToProfileCreate();
            return;
        }

        if (!ProfileHelper.HasCurrentSave)
        {
            Debug.LogError("No current save selected.");
            scene.Change.ToSaveFileSelect();
            return;
        }

        // Load UI joystick and hero
        virtualJoystick = GameObject.Find(GameObjectHelper.Overworld.Canvas.VirtualJoystick)?.GetComponent<VirtualJoystick>();
        if (virtualJoystick != null) joystickRect = virtualJoystick.GetComponent<RectTransform>();

        // Input mode button + icon
        inputModeButton = GameObject.Find(GameObjectHelper.Overworld.Canvas.InputModeButton)?.GetComponent<Button>();
        inputModeImage = GameObject.Find(GameObjectHelper.Overworld.Canvas.InputModeImage)?.GetComponent<Image>();
        inputModeLabel = GameObject.Find(GameObjectHelper.Overworld.Canvas.InputModeLabel)?.GetComponent<Label>();

        // Camera mode button + icon (optional wiring from scene)
        cameraModeButton = GameObject.Find(GameObjectHelper.Overworld.Canvas.CameraModeButton)?.GetComponent<Button>();
        cameraModeImage = GameObject.Find(GameObjectHelper.Overworld.Canvas.CameraModeImage)?.GetComponent<Image>();
        cameraModeLabel = GameObject.Find(GameObjectHelper.Overworld.Canvas.CameraModeLabel)?.GetComponent<Label>();

        // Offscreen arrow indicator



        // Find Map root
        mapRoot = GameObject.Find("Map").transform;

        // Load map data from profile
        var overworld = ProfileHelper.CurrentProfile.CurrentSave.Overworld;
        var data = MapLibrary.Get(overworld.MapName);

        // Ensure world-space layers exist as SpriteRenderers (preserve scene scale)
        terrainSR = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain).GetComponent<SpriteRenderer>();
        surfaceSR = GameObject.Find(GameObjectHelper.Overworld.Map.Surface).GetComponent<SpriteRenderer>();
        canopySR = GameObject.Find(GameObjectHelper.Overworld.Map.Canopy).GetComponent<SpriteRenderer>();

        // Ensure the terrain has a collision provider component
        //var collisionProvider = terrainSR.GetComponent<MapTerrain>();
        //collisionProvider.ForceRefresh();

        // Find hero under Map/Hero or by helper as fallback
        hero = GameObject.Find(GameObjectHelper.Overworld.Map.Hero).GetComponent<OverworldHero>();
        hero.OnHeroMoved += HandleHeroMoved;
        hero.transform.position = new Vector3(overworld.HeroX, overworld.HeroY, hero.transform.position.z);
        hero.SetFacing(overworld.HeroDirection);
        hero.BindWorld(terrainSR, cam);
        //hero.BindCollisionProvider(collisionProvider);

        // Wire offscreen indicator target now that we have hero
        offscreenArrow = GameObject.Find(GameObjectHelper.Overworld.Canvas.OffscreenArrow).GetComponent<OffscreenArrowIndicator>();
        offscreenArrow.WorldCamera = Camera.main;
        //offscreenArrow.Target = (cameraMode == OverworldCameraMode.FreeCamera && hero != null) ? hero.transform : null; // null -> fade out


        // Initialize UI state
        UpdateCameraModeUI();

        scene.FadeIn();
    }

    private void OnDestroy()
    {
        if (hero != null) hero.OnHeroMoved -= HandleHeroMoved;
        if (cameraModeButton != null) cameraModeButton.onClick.RemoveListener(CycleCameraMode);
    }

    public void CycleCameraMode()
    {
        // When Mode7 camera drives the pose, keep FollowHero
        if (Mode7Active)
        {
            cameraMode = OverworldCameraMode.FollowHero;
            UpdateCameraModeUI();
            return;
        }

        cameraMode = cameraMode == OverworldCameraMode.FollowHero ? OverworldCameraMode.FreeCamera : OverworldCameraMode.FollowHero;
        if (cameraMode == OverworldCameraMode.FollowHero && hero != null)
        {
            cameraTarget = hero.transform.position;
            isPanning = false;
        }
        else if (cameraMode == OverworldCameraMode.FreeCamera)
        {
            // Stop hero and block inputs while in free camera
            if (hero != null)
            {
                hero.FullStop();

            }
            // Start free camera target from current camera position
            cameraTarget = cam != null ? cam.transform.position : cameraTarget;
        }
        UpdateCameraModeUI();
    }



    private void UpdateCameraModeUI()
    {
        if (cameraModeImage == null && cameraModeLabel == null) return;
        // Map camera mode to sprite+label (reusing existing sprites)
        var mapping = cameraMode switch
        {
            OverworldCameraMode.FollowHero => ("Camera00", "Follow"),
            OverworldCameraMode.FreeCamera => ("Camera001", "Free"),
            _ => ("Camera00", "Follow"),
        };
        if (cameraModeImage != null && SpriteLibrary.GUI.ContainsKey(mapping.Item1))
            cameraModeImage.sprite = SpriteLibrary.GUI[mapping.Item1];
        if (cameraModeLabel != null) cameraModeLabel.text = mapping.Item2;
    }

    private void Update()
    {
        if (terrainSR == null) return;

        // Touch (hold-to-move in directional mode) or pan camera in FreeCamera
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            bool overUiNow = IsOverUI(t.position) || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId));

            if (t.phase == TouchPhase.Began)
            {
                pointerDownAllowed = !overUiNow;
                pointerDownPos = t.position;
                pointerDownTime = Time.unscaledTime;

                if (pointerDownAllowed && cameraMode == OverworldCameraMode.FreeCamera && !Mode7Active)
                {
                    isPanning = true;
                    panStartScreen = t.position;
                    panStartCameraTarget = cameraTarget;
                }

                if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && hero != null)
                    hero.BeginDirectionalFromScreen(t.position, null);
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                if (isPanning && cameraMode == OverworldCameraMode.FreeCamera && !Mode7Active)
                {
                    UpdatePanTarget(t.position);
                }
                else if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && hero != null)
                    hero.UpdateDirectionalFromScreen(t.position, null);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                if (isPanning)
                {
                    isPanning = false;
                }
                else if (cameraMode != OverworldCameraMode.FreeCamera && hero != null)
                {
                    hero.FullStop();
                }

            }
            return;
        }

        // Mouse (hold-to-move in directional mode) OR pan camera with right/left drag in FreeCamera
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            bool overUiNow = IsOverUI(pos) || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject());
            pointerDownAllowed = !overUiNow;
            pointerDownPos = pos;
            pointerDownTime = Time.unscaledTime;

            if (pointerDownAllowed && cameraMode == OverworldCameraMode.FreeCamera && !Mode7Active)
            {
                isPanning = true;
                panStartScreen = pos;
                panStartCameraTarget = cameraTarget;
            }

            if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && hero != null)
                hero.BeginDirectionalFromScreen(pos, null);
        }
        if (Input.GetMouseButton(0))
        {
            if (isPanning && cameraMode == OverworldCameraMode.FreeCamera && !Mode7Active)
            {
                Vector2 pos = (Vector2)Input.mousePosition;
                UpdatePanTarget(pos);
            }
            else if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && hero != null)
            {
                Vector2 pos = (Vector2)Input.mousePosition;
                hero.UpdateDirectionalFromScreen(pos, null);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            bool overUiNow = IsOverUI(pos) || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject());
            if (isPanning)
            {
                isPanning = false;
            }
            else if (cameraMode != OverworldCameraMode.FreeCamera && hero != null)
            {
                hero.FullStop();
            }

        }

        // Also allow right mouse to pan in FreeCamera (editor convenience)
        if (cameraMode == OverworldCameraMode.FreeCamera && !Mode7Active)
        {
            if (Input.GetMouseButtonDown(1))
            {
                panStartScreen = (Vector2)Input.mousePosition;
                panStartCameraTarget = cameraTarget;
                isPanning = true;
            }
            if (Input.GetMouseButton(1) && isPanning)
            {
                UpdatePanTarget((Vector2)Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(1))
            {
                isPanning = false;
            }
        }
    }

    private void LateUpdate()
    {
        // Feed analog input to the hero every frame (hero ignores it unless in VirtualJoystick mode)
        Vector2 stick = virtualJoystick != null ? virtualJoystick.Direction : Vector2.zero;

        // If currently over non-joystick UI, suppress joystick feed entirely
        bool blockByUI = false;
        if (EventSystem.current != null)
        {
            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                blockByUI = IsOverUI(t.position) || EventSystem.current.IsPointerOverGameObject(t.fingerId);
            }
            else
            {
                Vector2 mp = Input.mousePosition;
                blockByUI = IsOverUI(mp) || EventSystem.current.IsPointerOverGameObject();
            }
        }

        if (blockByUI) stick = Vector2.zero;

        // Update camera position unless Mode7 is driving it
        if (cam != null && !Mode7Active)
        {
            if (cameraMode == OverworldCameraMode.FollowHero && hero != null)
            {
                cameraTarget = hero.transform.position;
            }

            // Clamp camera target to map bounds
            cameraTarget = ClampCameraTarget(cameraTarget);

            // Smoothly move camera
            var cur = cam.transform.position;
            var target = new Vector3(cameraTarget.x, cameraTarget.y, cur.z);
            cam.transform.position = Vector3.Lerp(cur, target, Mathf.Clamp01(Time.deltaTime * panLerpSpeed));
        }

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

        // Offscreen arrow indicator wiring/toggle (fade-based)
        if (offscreenArrow != null)
        {
            offscreenArrow.WorldCamera = cam;
            offscreenArrow.Target = (cameraMode == OverworldCameraMode.FreeCamera && hero != null && !Mode7Active) ? hero.transform : null; // null when not in FreeCamera -> fade out
        }
    }

    private bool IsTap(Vector2 releasePos)
    {
        if (Time.unscaledTime - pointerDownTime > tapMaxTime) return false;
        return (releasePos - pointerDownPos).sqrMagnitude <= tapMaxSqrDistance;
    }

    private bool IsOverUI(Vector2 screenPos)
    {
        // Treat any UI under the pointer as blocking, except the virtual joystick hierarchy.
        if (EventSystem.current == null) return false;
        var ped = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        if (results == null || results.Count == 0) return false;

        foreach (var r in results)
        {
            if (joystickRect != null)
            {
                var tr = r.gameObject.transform as RectTransform;
                if (tr == joystickRect || tr != null && tr.IsChildOf(joystickRect))
                {
                    // Over joystick -> not blocking
                    continue;
                }
            }
            return true; // some UI hit that's not the joystick
        }
        return false;
    }



    // Follow hero while moving and flag movement for encounter timer
    private void HandleHeroMoved(Vector2 heroPos)
    {
        movedThisFrame = true;
    }

    // Trigger scene change to a random stage after sustained movement
    private void TriggerRandomEncounter()
    {
        if (!hasRandomEncounters || isLoadingEncounter) return;
        if (StageLibrary.Stages == null || StageLibrary.Stages.Count == 0) return;

        string mapName = ProfileHelper.Overworld.MapName;

        // Persist overworld location and facing
        if (hero != null)
        {
            ProfileHelper.CurrentProfile.LatestSave.Overworld.MapName = mapName;
            ProfileHelper.CurrentProfile.LatestSave.Overworld.HeroX = hero.transform.position.x;
            ProfileHelper.CurrentProfile.LatestSave.Overworld.HeroY = hero.transform.position.y;
            ProfileHelper.CurrentProfile.LatestSave.Overworld.HeroDirection = hero.CurrentFacingName ?? "Idle";
            ProfileHelper.SaveOverworldPosition(new Vector2(hero.transform.position.x, hero.transform.position.y), mapName, hero.CurrentFacingName ?? "Idle");
        }

        // Get a random stage for this map from RNG
        string stageName = RNG.Stage(mapName);
        ProfileHelper.CurrentProfile.LatestSave.Stage.CurrentStage = stageName;

        isLoadingEncounter = true;
        scene.Change.ToGame();
    }

    // -------- helpers for layered map (world-space) --------

    private SpriteRenderer EnsureWorldLayerSR(Transform existingOrParent, Sprite s, int sortingOrder = 0)
    {
        GameObject go = null;
        SpriteRenderer sr = null;

        if (existingOrParent != null)
        {
            // If passed a Transform of the existing object, use it. Otherwise, create under parent.
            sr = existingOrParent.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                go = existingOrParent.gameObject;
            }
            else
            {
                // Create a new child with this transform as parent
                go = new GameObject("Layer");
                go.transform.SetParent(existingOrParent, false);
            }
        }

        if (go == null)
        {
            // Fallback: try to find by GameObjectHelper path, then by name
            go = GameObject.Find(GameObjectHelper.Overworld.Map.Terrain);
            if (go == null)
            {
                go = new GameObject("Layer");
                if (mapRoot != null) go.transform.SetParent(mapRoot, false);
            }
        }

        sr = go.GetComponent<SpriteRenderer>() ?? go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = sortingOrder;

        // Parent under Map root if not already
        if (mapRoot != null && go.transform.parent != mapRoot && (go.name == "Surface" || go.name == "Canopy" || go.name == "Terrain" || go.name == "Layer"))
        {
            go.transform.SetParent(mapRoot, false);
        }

        // Preserve authored scale: do not modify go.transform.localScale here.

        // If this is the terrain, ensure collision provider exists
        //if (go.name == "Terrain")
        //{
        //    var cp = go.GetComponent<MapTerrain>();
        //    if (cp == null) cp = go.AddComponent<MapTerrain>();
        //    cp.ForceRefresh();
        //}

        return sr;
    }

    private Transform RelativeOrGlobal(Transform parent, string childName)
    {
        Transform t = null;
        if (parent != null)
        {
            var child = parent.Find(childName);
            if (child != null) t = child;
        }
        if (t == null)
        {
            // Fallbacks
            var byPath = GameObject.Find("Map/" + childName);
            if (byPath != null) t = byPath.transform;
        }
        if (t == null)
        {
            var byName = GameObject.Find(childName);
            if (byName != null) t = byName.transform;
        }
        return t;
    }

    // --- Camera helpers ---
    private void UpdatePanTarget(Vector2 currentScreen)
    {
        if (cam == null) return;
        // Convert screen delta to world delta at camera plane
        Vector3 a = cam.ScreenToWorldPoint(new Vector3(panStartScreen.x, panStartScreen.y, 0f));
        Vector3 b = cam.ScreenToWorldPoint(new Vector3(currentScreen.x, currentScreen.y, 0f));
        Vector3 worldDelta = b - a;
        // Move camera opposite to finger drag
        cameraTarget = panStartCameraTarget - new Vector3(worldDelta.x, worldDelta.y, 0f);
    }

    private Vector3 ClampCameraTarget(Vector3 target)
    {
        if (terrainSR == null || cam == null) return target;
        Bounds b = terrainSR.bounds;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float minX = b.min.x + halfW;
        float maxX = b.max.x - halfW;
        float minY = b.min.y + halfH;
        float maxY = b.max.y - halfH;
        // If map smaller than view, just center clamp
        if (minX > maxX)
        {
            float cx = (b.min.x + b.max.x) * 0.5f;
            minX = maxX = cx;
        }
        if (minY > maxY)
        {
            float cy = (b.min.y + b.max.y) * 0.5f;
            minY = maxY = cy;
        }
        float x = Mathf.Clamp(target.x, minX, maxX);
        float y = Mathf.Clamp(target.y, minY, maxY);
        return new Vector3(x, y, target.z);
    }
}
