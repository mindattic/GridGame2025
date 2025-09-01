using Assets.Helper;
using Assets.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.StandaloneInputModule;
using scene = Assets.Helpers.SceneHelper;
using Label = TMPro.TextMeshProUGUI;

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

    private bool hasRandomEncounters = false;

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

    // Camera mode and panning
    private OverworldCameraMode cameraMode = OverworldCameraMode.FollowHero;
    private bool isPanning;
    private Vector2 panStartScreen;
    private Vector3 panStartCameraTarget;
    private Vector3 cameraTarget;
    private float panLerpSpeed = 10f; // higher = snappier

    private Camera cam;
    private Transform mapRoot; // parent for map components

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
        var collisionProvider = terrainSR.GetComponent<MapTerrain>();
        collisionProvider.ForceRefresh();

        // Find hero under Map/Hero or by helper as fallback
        hero = GameObject.Find(GameObjectHelper.Overworld.Map.Hero).GetComponent<OverworldHero>();
        hero.AllowClickToMove = true; // enable click handling
        hero.OnHeroMoved += HandleHeroMoved;
        hero.transform.position = new Vector3(overworld.HeroX, overworld.HeroY, hero.transform.position.z);
        hero.SetFacing(overworld.HeroDirection);
        hero.SetFollowSpeedRampDistance(8f);
        hero.BindWorld(terrainSR, cam);
        hero.BindCollisionProvider(collisionProvider);

        // Wire offscreen indicator target now that we have hero
        offscreenArrow = GameObject.Find(GameObjectHelper.Overworld.Canvas.OffscreenArrow).GetComponent<OffscreenArrowIndicator>();
        offscreenArrow.WorldCamera = Camera.main;
        //offscreenArrow.Target = (cameraMode == OverworldCameraMode.FreeCamera && hero != null) ? hero.transform : null; // null -> fade out

     
        // Initialize UI state
        UpdateInputModeUI();
        UpdateCameraModeUI();

        scene.FadeIn();
    }

    private void OnDestroy()
    {
        if (hero != null) hero.OnHeroMoved -= HandleHeroMoved;
        if (inputModeButton != null) inputModeButton.onClick.RemoveListener(CycleInputMode);
        if (cameraModeButton != null) cameraModeButton.onClick.RemoveListener(CycleCameraMode);
    }

    // Called by InputMode button
    public void CycleInputMode()
    {
        if (hero == null) return;

        hero.InputMode = hero.InputMode switch
        {
            OverworldHeroInputMode.FollowCursor => OverworldHeroInputMode.ClickToMove,
            OverworldHeroInputMode.ClickToMove => OverworldHeroInputMode.VirtualJoystick,
            _ => OverworldHeroInputMode.FollowCursor,
        };

        // Reset joystick output when not in joystick mode
        if (hero.InputMode != OverworldHeroInputMode.VirtualJoystick)
        {
            if (virtualJoystick != null) virtualJoystick.ResetOutput();
            hero.SetAnalogInput(Vector2.zero);
        }

        UpdateInputModeUI();
        hero.FullStop();
    }

    // Set the input-mode icon and show/hide the joystick canvas object
    private void UpdateInputModeUI()
    {
        //Map input mode to button sprite and label
        var mapping = hero.InputMode switch
        {
            OverworldHeroInputMode.FollowCursor => ("Joystick00", "Follow"),
            OverworldHeroInputMode.ClickToMove => ("Joystick01", "Click"),
            OverworldHeroInputMode.VirtualJoystick => ("Joystick02", "Joystick"),
            _ => ("Joystick00", "Follow"),
        };

        // Set button sprite and label
        if (inputModeImage != null) inputModeImage.sprite = SpriteLibrary.GUI[mapping.Item1];
        if (inputModeLabel != null) inputModeLabel.text = mapping.Item2;

        //Toggle joystick visibility
        if (virtualJoystick != null)
            virtualJoystick.gameObject.SetActive(hero.UsingJoystick);
    }

    public void CycleCameraMode()
    {
        cameraMode = cameraMode == OverworldCameraMode.FollowHero ? OverworldCameraMode.FreeCamera : OverworldCameraMode.FollowHero;
        if (cameraMode == OverworldCameraMode.FollowHero && hero != null)
        {
            cameraTarget = hero.transform.position;
            isPanning = false;
            hero.AllowClickToMove = true; // re-enable click move
        }
        else if (cameraMode == OverworldCameraMode.FreeCamera)
        {
            // Stop hero and block inputs while in free camera
            if (hero != null)
            {
                hero.FullStop();
                hero.SetAnalogInput(Vector2.zero);
                hero.AllowClickToMove = false; // cancel click-to-move and block new ones
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

        bool isDirectional = hero != null && hero.InputMode == OverworldHeroInputMode.FollowCursor;

        // Touch (hold-to-move in directional mode) or pan camera in FreeCamera
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                pointerDownAllowed = !IsOverJoystick(t.position);
                pointerDownPos = t.position;
                pointerDownTime = Time.unscaledTime;

                // Start camera pan when in FreeCamera
                if (pointerDownAllowed && cameraMode == OverworldCameraMode.FreeCamera)
                {
                    isPanning = true;
                    panStartScreen = t.position;
                    panStartCameraTarget = cameraTarget;
                }

                // Begin directional only when not in FreeCamera
                if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && isDirectional && hero != null)
                    hero.BeginDirectionalFromScreen(t.position, null);
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                if (isPanning && cameraMode == OverworldCameraMode.FreeCamera)
                {
                    UpdatePanTarget(t.position);
                }
                else if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && isDirectional && hero != null)
                    hero.UpdateDirectionalFromScreen(t.position, null);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                if (isPanning)
                {
                    isPanning = false;
                }
                else if (cameraMode != OverworldCameraMode.FreeCamera && isDirectional && hero != null)
                {
                    hero.FullStop();
                }
                else if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && IsTap(t.position) && !IsOverJoystick(t.position))
                {
                    HandleTap(t.position);
                }
            }
            return;
        }

        // Mouse (hold-to-move in directional mode) OR pan camera with right/left drag in FreeCamera
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            pointerDownAllowed = !IsOverJoystick(pos);
            pointerDownPos = pos;
            pointerDownTime = Time.unscaledTime;

            if (pointerDownAllowed && cameraMode == OverworldCameraMode.FreeCamera)
            {
                isPanning = true;
                panStartScreen = pos;
                panStartCameraTarget = cameraTarget;
            }

            if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && isDirectional && hero != null)
                hero.BeginDirectionalFromScreen(pos, null);
        }
        if (Input.GetMouseButton(0))
        {
            if (isPanning && cameraMode == OverworldCameraMode.FreeCamera)
            {
                Vector2 pos = (Vector2)Input.mousePosition;
                UpdatePanTarget(pos);
            }
            else if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && isDirectional && hero != null)
            {
                Vector2 pos = (Vector2)Input.mousePosition;
                hero.UpdateDirectionalFromScreen(pos, null);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            if (isPanning)
            {
                isPanning = false;
            }
            else if (cameraMode != OverworldCameraMode.FreeCamera && isDirectional && hero != null)
            {
                hero.FullStop();
            }
            else if (pointerDownAllowed && cameraMode != OverworldCameraMode.FreeCamera && IsTap(pos) && !IsOverJoystick(pos))
            {
                HandleTap(pos);
            }
        }

        // Also allow right mouse to pan in FreeCamera (editor convenience)
        if (cameraMode == OverworldCameraMode.FreeCamera)
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

        if (hero != null)
        {
            if (cameraMode == OverworldCameraMode.FreeCamera)
            {
                hero.SetAnalogInput(Vector2.zero);
            }
            else
            {
                hero.SetAnalogInput(stick);
                hero.AllowClickToMove = true; // ensure re-enabled while following
            }
        }

        // Update camera position
        if (cam != null)
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
            offscreenArrow.Target = (cameraMode == OverworldCameraMode.FreeCamera && hero != null) ? hero.transform : null; // null when not in FreeCamera -> fade out
        }
    }

    private bool IsTap(Vector2 releasePos)
    {
        if (Time.unscaledTime - pointerDownTime > tapMaxTime) return false;
        return (releasePos - pointerDownPos).sqrMagnitude <= tapMaxSqrDistance;
    }

    private bool IsOverJoystick(Vector2 screenPos)
    {
        // In FreeCamera, always allow panning regardless of joystick area
        if (cameraMode == OverworldCameraMode.FreeCamera) return false;

        // Do not block taps when the joystick is hidden or missing
        if (joystickRect == null || !joystickRect.gameObject.activeInHierarchy) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(joystickRect, screenPos, null);
    }

    private void HandleTap(Vector2 screenPos)
    {
        if (hero == null) return;
        hero.HandleClickScreen(screenPos, null); // world mode path inside hero
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
        if (go.name == "Terrain")
        {
            var cp = go.GetComponent<MapTerrain>();
            if (cp == null) cp = go.AddComponent<MapTerrain>();
            cp.ForceRefresh();
        }

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