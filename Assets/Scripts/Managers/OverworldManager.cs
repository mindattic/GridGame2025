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
        joystickRect = virtualJoystick != null ? virtualJoystick.GetComponent<RectTransform>() : null;

        // Input mode button + icon
        var btnGo = GameObject.Find(GameObjectHelper.Overworld.Canvas.InputModeButton);
        if (btnGo != null)
        {
            inputModeButton = btnGo.GetComponent<Button>();
            if (inputModeButton != null)
                inputModeButton.onClick.AddListener(CycleInputMode);
        }
        var imgGo = GameObject.Find(GameObjectHelper.Overworld.Canvas.InputModeImage);
        inputModeImage = imgGo.GetComponent<Image>();
        var labelGo = GameObject.Find(GameObjectHelper.Overworld.Canvas.InputModeLabel);
        inputModeLabel = labelGo.GetComponent<Label>();

        // Find Map root
        var mapGo = GameObject.Find("Map");
        mapRoot = mapGo != null ? mapGo.transform : null;

        // Load map data from profile
        var overworld = ProfileHelper.CurrentProfile.CurrentSave.Overworld;
        var data = MapLibrary.Get(overworld.MapName);

        // Ensure world-space layers exist as SpriteRenderers (preserve scene scale)
        terrainSR = EnsureWorldLayerSR(RelativeOrGlobal(mapRoot, "Terrain"), data.Terrain, sortingOrder: 0);
        surfaceSR = EnsureWorldLayerSR(RelativeOrGlobal(mapRoot, "Surface"), data.Surface, sortingOrder: 1);
        canopySR  = EnsureWorldLayerSR(RelativeOrGlobal(mapRoot, "Canopy"),  data.Canopy,  sortingOrder: 10);

        // Ensure the terrain has a collision provider component
        MapTerrain collisionProvider = null;
        if (terrainSR != null)
        {
            collisionProvider = terrainSR.GetComponent<MapTerrain>();
            if (collisionProvider == null)
                collisionProvider = terrainSR.gameObject.AddComponent<MapTerrain>();
            collisionProvider.ForceRefresh();
        }

        // Find hero under Map/Hero or by helper as fallback
        hero = (RelativeOrGlobal(mapRoot, "Hero")?.GetComponent<OverworldHero>())
              ?? GameObject.Find(GameObjectHelper.Overworld.Map.Hero)?.GetComponent<OverworldHero>();

        // Wire hero
        if (hero != null)
        {
            hero.AllowClickToMove = true; // enable click handling
            hero.OnHeroMoved += HandleHeroMoved;

            // Bind world mode
            hero.BindWorld(terrainSR, cam);
            if (collisionProvider != null)
                hero.BindCollisionProvider(collisionProvider);

            // Position hero from save (world units)
            hero.transform.position = new Vector3(overworld.HeroX, overworld.HeroY, hero.transform.position.z);
            hero.SetFacing(overworld.HeroDirection);
        }

        // Initialize UI state
        UpdateInputModeUI();

        scene.FadeIn();
    }

    private void OnDestroy()
    {
        if (hero != null) hero.OnHeroMoved -= HandleHeroMoved;
        if (inputModeButton != null) inputModeButton.onClick.RemoveListener(CycleInputMode);
    }

    // Called by InputMode button
    public void CycleInputMode()
    {
        if (hero == null) return;

        hero.InputMode = (OverworldHeroInputMode)(((int)hero.InputMode + 1) % 3);
        ApplyInputModeEffects();
    }

    // Apply visuals and clear inputs when switching mode
    private void ApplyInputModeEffects()
    {
        UpdateInputModeUI();

        // Reset joystick output when not in joystick mode
        if (hero.InputMode != OverworldHeroInputMode.VirtualJoystick)
        {
            if (virtualJoystick != null) virtualJoystick.ResetOutput();
            hero.SetAnalogInput(Vector2.zero);
        }

        // Ensure no latched directional state when not in directional mode
        if (hero.InputMode != OverworldHeroInputMode.DirectionalPress)
        {
            hero.EndDirectional();
        }
    }

    // Set the input-mode icon and show/hide the joystick canvas object
    private void UpdateInputModeUI()
    {
        // Icon
        if (inputModeImage != null)
        {
            string key = "Joystick00";
            switch (hero != null ? hero.InputMode : OverworldHeroInputMode.VirtualJoystick)
            {
                case OverworldHeroInputMode.VirtualJoystick: key = "Joystick00"; break;
                case OverworldHeroInputMode.ClickToMove:      key = "Joystick01"; break;
                case OverworldHeroInputMode.DirectionalPress: key = "Joystick02"; break;
            }

            inputModeImage.sprite = SpriteLibrary.GUI[key];
            inputModeLabel.text = key switch
            {
                "Joystick00" => "Joystick",
                "Joystick01" => "Click",
                "Joystick02" => "Directional",
                _ => "Unknown"
            };
        }

        // Joystick visibility
        bool showStick = hero != null && hero.InputMode == OverworldHeroInputMode.VirtualJoystick;
        if (virtualJoystick != null)
        {
            var go = virtualJoystick.gameObject;
            if (go.activeSelf != showStick)
                go.SetActive(showStick);
        }
    }

    private void Update()
    {
        if (terrainSR == null) return;

        bool isDirectional = hero != null && hero.InputMode == OverworldHeroInputMode.DirectionalPress;

        // Touch (hold-to-move in directional mode)
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                pointerDownAllowed = !IsOverJoystick(t.position);
                pointerDownPos = t.position;
                pointerDownTime = Time.unscaledTime;

                if (pointerDownAllowed && isDirectional && hero != null)
                    hero.BeginDirectionalFromScreen(t.position, null);
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                if (pointerDownAllowed && isDirectional && hero != null)
                    hero.UpdateDirectionalFromScreen(t.position, null);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                if (isDirectional && hero != null)
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

            if (pointerDownAllowed && isDirectional && hero != null)
                hero.BeginDirectionalFromScreen(pos, null);
        }
        if (Input.GetMouseButton(0))
        {
            if (pointerDownAllowed && isDirectional && hero != null)
            {
                Vector2 pos = (Vector2)Input.mousePosition;
                hero.UpdateDirectionalFromScreen(pos, null);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition;
            if (isDirectional && hero != null)
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
        // Feed analog input to the hero every frame (hero ignores it unless in VirtualJoystick mode)
        Vector2 stick = virtualJoystick != null ? virtualJoystick.Direction : Vector2.zero;

        if (hero != null)
            hero.SetAnalogInput(stick);

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

        // Always center camera on hero
        if (cam != null && hero != null)
        {
            var hp = hero.transform.position;
            cam.transform.position = new Vector3(hp.x, hp.y, cam.transform.position.z);
        }
    }

    private bool IsTap(Vector2 releasePos)
    {
        if (Time.unscaledTime - pointerDownTime > tapMaxTime) return false;
        return (releasePos - pointerDownPos).sqrMagnitude <= tapMaxSqrDistance;
    }

    private bool IsOverJoystick(Vector2 screenPos)
    {
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
}