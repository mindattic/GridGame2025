using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;

public class SettingsManager : MonoBehaviour
{
    [Header("Prefabs")] public GameObject sliderPrefab;
    public GameObject togglePrefab;
    public GameObject dropdownPrefab;

    [Header("Layout References")] public RectTransform contentRoot;

    public static readonly List<SliderSetting> Sliders = new List<SliderSetting>
        {
            new SliderSetting(
                "Actor Pan Multiplier",
                "Determines the activity of the actor panning effect",
                0f, 1f, 0.01f,
                s => s.ActorPanMultiplier,
                (s, v) => s.ActorPanMultiplier = v),

            new SliderSetting(
                "Game Speed",
                "Determines the speed of the game.",
                0.25f, 3f, 0.05f,
                s => s.GameSpeed,
                (s, v) => s.GameSpeed = v),

            new SliderSetting(
                "Drag Sensitivity",
                "Controls the sensitivity of drag actions.",
                0.01f, 0.10f, 0.01f,
                s => s.DragSensitivity,
                (s, v) => s.DragSensitivity = v),

            new SliderSetting(
                "Coin Count Multiplier",
                "Coin spawn multiplier",
                0f, 5f, 0.05f,
                s => s.CoinCountMultiplier,
                (s, v) => s.CoinCountMultiplier = v),
        };

    public static readonly List<ToggleSetting> Toggles = new List<ToggleSetting>
        {
            new ToggleSetting(
                "Apply Movement Tilt",
                "Determines whether movement tilt effects are applied.",
                s => s.ApplyMovementTilt,
                (s, v) => s.ApplyMovementTilt = v),

            new ToggleSetting(
                "Reload Thumbnail Settings",
                "If enabled, thumbnails will be reloaded based on current settings.",
                s => s.ReloadThumbnailSettings,
                (s, v) => s.ReloadThumbnailSettings = v),
        };

    public static readonly List<DropdownSetting> Dropdowns = new List<DropdownSetting>
        {
            new DropdownSetting("Texture Resolution",
                "Sets the texture resolution quality.",
                typeof(TextureResolution),
                s => (object)s.TextureResolution,
                (s, o) => s.TextureResolution = (TextureResolution)o),
        };


    private void Awake()
    {
        if (!ProfileHelper.HasProfiles()) return;


        sliderPrefab = PrefabLibrary.Prefabs.GetValueOrDefault("SettingSlider");
        togglePrefab = PrefabLibrary.Prefabs.GetValueOrDefault("SettingToggle");
        dropdownPrefab = PrefabLibrary.Prefabs.GetValueOrDefault("SettingDropdown");
        contentRoot = GameObject.Find(GameObjectHelper.Settings.Content).GetComponent<RectTransform>();

        ReloadUI();
    }

    private void Start()
    {
        scene.FadeIn();
    }

    private void ReloadUI()
    {
        if (contentRoot == null)
        {
            Debug.LogError("SettingsManager: contentRoot not assigned");
            return;
        }
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        var settings = ProfileHelper.CurrentProfile.Settings;

        // Build sliders
        foreach (var x in Sliders)
        {
            CreateSlider(x.FriendlyName, x.TooltipText, x.Getter(settings), x.Min, x.Max, x.Increment, v =>
            {
                // Update in-memory only; no save on change
                x.Setter(settings, v);
            }, x.AsInt);
        }

        // Build toggles
        foreach (var x in Toggles)
        {
            CreateToggle(x.FriendlyName, x.TooltipText, x.Getter(settings), v =>
            {
                // Update in-memory only; no save on change
                x.Setter(settings, v);
            });
        }

        // Build dropdowns
        foreach (var x in Dropdowns)
        {
            CreateDropdown(x.FriendlyName, x.TooltipText, x.EnumType, x.Getter(settings), val =>
            {
                // Update in-memory only; no save on change
                x.Setter(settings, val);
            });
        }
    }

    private IEnumerator SetTextNextFrame(TextMeshProUGUI label, string text)
    {
        yield return null; // wait one frame to avoid layout changes mid-pointer event
        if (label != null)
        {
            label.SetText(text);
        }
    }

    private void CreateSlider(string label, string tooltipText, float current, float min, float max, float increment, Action<float> onChanged, bool asInt)
    {
        if (sliderPrefab == null) { Debug.LogError("Slider prefab not set"); return; }
        var go = Instantiate(sliderPrefab, contentRoot);
        // Name: PrefabName + Pascal(setting)
        var prefabName = sliderPrefab != null ? sliderPrefab.name : "SettingSlider";

        go.name = $"{prefabName}_{label.ToPascalCase()}";

        //var tt = new TooltipSettings()
        //{
        //    message = tooltipText,
        //    target = go,
        //};
        //Tooltip.Show(tt);

        var texts = go.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (texts != null && texts.Length > 0) texts[0].text = label;
        foreach (var t in texts) if (t != null) t.raycastTarget = false;

        var slider = go.GetComponentInChildren<Slider>(true);
        var valueLabel = texts != null && texts.Length > 1 ? texts[1] : null;
        if (valueLabel != null)
        {
            var le = valueLabel.GetComponent<LayoutElement>();
            if (le == null) le = valueLabel.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
        }

        if (slider == null)
        {
            Debug.LogError($"No Slider component found under '{go.name}'");
            return;
        }

        // Ensure consistent behavior irrespective of prefab defaults
        slider.SetDirection(Slider.Direction.LeftToRight, true);
        slider.direction = Slider.Direction.LeftToRight;
        var nav = new Navigation { mode = Navigation.Mode.None };
        slider.navigation = nav;
        slider.transition = Selectable.Transition.None;
        slider.interactable = true;

        // Ensure correct numeric mode
        slider.wholeNumbers = asInt;

        slider.minValue = min;
        slider.maxValue = max;

        var srt = slider.GetComponent<RectTransform>();
        if (srt != null && srt.rect.width <= 2f)
        {
            Debug.LogWarning($"[Settings] Slider '{go.name}' has very small width ({srt.rect.width}). Clicks may map to extremes. Check prefab/layout.");
        }

        float Snap(float v)
        {
            var clamped = Mathf.Clamp(v, min, max);
            if (asInt) return Mathf.Clamp(Mathf.Round(clamped), min, max);
            if (increment > 0f)
            {
                var steps = Mathf.Round((clamped - min) / increment);
                var snapped = min + steps * increment;
                return Mathf.Clamp(snapped, min, max);
            }
            return clamped;
        }

        string Format(float v)
        {
            return asInt ? Mathf.RoundToInt(v).ToString() : v.ToString(increment >= 0.1f ? "0.0" : "0.00");
        }

        // Initialize without firing onChanged
        var initial = Snap(current);
        slider.SetValueWithoutNotify(initial);
        if (valueLabel != null)
        {
            valueLabel.text = Format(initial);
            //StartCoroutine(SetTextNextFrame(valueLabel, Format(initial)));
        }

        // Event handlers for setting value from pointer position explicitly
        void SetFromPointer(PointerEventData e)
        {
            if (e == null) return;
            var rt = slider.fillRect != null ? slider.fillRect : slider.GetComponent<RectTransform>();
            if (rt == null) return;
            Vector2 local;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out local))
                return;
            var rect = rt.rect;
            float t = Mathf.InverseLerp(rect.xMin, rect.xMax, local.x);
            t = Mathf.Clamp01(t);
            if (slider.direction == Slider.Direction.RightToLeft)
                t = 1f - t;
            var target = min + t * (max - min);
            var snapped = Snap(target);
            if (Mathf.Abs(slider.value - snapped) > 0.0001f)
                slider.value = snapped;
            if (valueLabel != null)
            {
                //StartCoroutine(SetTextNextFrame(valueLabel, Format(snapped)));
                valueLabel.text = Format(snapped);
            }

            e.Use();
        }

        var trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = slider.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = trigger.triggers ?? new List<EventTrigger.Entry>();

        void AddTrigger(EventTriggerType type, Action<BaseEventData> act)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(data => act(data));
            trigger.triggers.Add(entry);
        }

        AddTrigger(EventTriggerType.PointerDown, d => SetFromPointer(d as PointerEventData));
        AddTrigger(EventTriggerType.Drag, d => SetFromPointer(d as PointerEventData));

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(v =>
        {
            var snapped = Snap(v);
            if (Mathf.Abs(snapped - v) > 0.0001f)
                slider.SetValueWithoutNotify(snapped);
            if (valueLabel != null)
            {
                //StartCoroutine(SetTextNextFrame(valueLabel, Format(snapped)));
                valueLabel.text = Format(snapped);
            }

            onChanged(snapped);
        });
    }

    private void CreateToggle(string label, string tooltipText, bool current, Action<bool> onChanged)
    {
        if (togglePrefab == null) { Debug.LogError("Toggle prefab not set"); return; }
        var go = Instantiate(togglePrefab, contentRoot);
        var prefabName = togglePrefab != null ? togglePrefab.name : "SettingToggle";
        go.name = $"{prefabName}_{label.ToPascalCase()}";

        //var tt = new TooltipSettings()
        //{
        //    message = tooltipText,
        //    target = go,
        //};
        //Tooltip.Show(tt);

        var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0) texts[0].text = label;
        foreach (var t in texts) if (t != null) t.raycastTarget = false;

        var toggle = go.GetComponentInChildren<Toggle>();
        toggle.isOn = current;
        toggle.onValueChanged.AddListener(v => onChanged(v));
    }

    private void CreateDropdown(string label, string tooltipText, Type enumType, object current, Action<object> onChanged)
    {
        if (dropdownPrefab == null) { Debug.LogError("Dropdown prefab not set"); return; }
        var go = Instantiate(dropdownPrefab, contentRoot);
        var prefabName = dropdownPrefab != null ? dropdownPrefab.name : "SettingDropdown";
        go.name = $"{prefabName}_{label.ToPascalCase()}";

        //var tt = new TooltipSettings()
        //{
        //    message = tooltipText,
        //    target = go,
        //};
        //Tooltip.Show(tt);

        var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0) texts[0].text = label;
        foreach (var t in texts) if (t != null) t.raycastTarget = false;

        var dropdown = go.GetComponentInChildren<TMP_Dropdown>();
        dropdown.ClearOptions();
        var names = Enum.GetNames(enumType);
        dropdown.AddOptions(new List<string>(names));
        int idx = Array.IndexOf(names, current.ToString());
        dropdown.value = idx >= 0 ? idx : 0;
        dropdown.onValueChanged.AddListener(i =>
        {
            var val = Enum.Parse(enumType, names[i]);
            onChanged(val);
        });
    }

    public void OnBackButtonClicked()
    {
        scene.Fade.ToPreviousScene();
    }

    public void OnSaveButtonClicked()
    {
        // Persist current in-memory settings in the active profile
        ProfileHelper.SaveSettings();
        Debug.Log("Settings saved.");
    }

    public void OnDefaultsButtonClick()
    {
        var profile = ProfileHelper.CurrentProfile;
        if (profile == null) return;
        profile.Settings = new ProfileSettings(ProfileHelper.DefaultSettings);
        ProfileHelper.SaveSettings(profile);
        ReloadUI();
        Debug.Log("Settings reset to defaults.");
    }
}

public class SliderSetting
{
    public string FriendlyName { get; }
    public string TooltipText { get; }
    public float Min { get; }
    public float Max { get; }
    public float Increment { get; }
    public bool AsInt { get; }
    public Func<ProfileSettings, float> Getter { get; }
    public Action<ProfileSettings, float> Setter { get; }
    public SliderSetting(string friendlyName, string tooltipText, float min, float max, float increment,
                         Func<ProfileSettings, float> getter, Action<ProfileSettings, float> setter,
                         bool asInt = false)
    {
        FriendlyName = friendlyName;
        TooltipText = tooltipText;
        Min = min;
        Max = max;
        Increment = increment;
        Getter = getter;
        Setter = setter;
        AsInt = asInt;
    }
}

public class ToggleSetting
{
    public string FriendlyName { get; }
    public string TooltipText { get; }
    public Func<ProfileSettings, bool> Getter { get; }
    public Action<ProfileSettings, bool> Setter { get; }
    public ToggleSetting(string friendlyName, string tooltipText, Func<ProfileSettings, bool> getter, Action<ProfileSettings, bool> setter)
    {
        FriendlyName = friendlyName;
        TooltipText = tooltipText;
        Getter = getter;
        Setter = setter;
    }
}

public class DropdownSetting
{
    public string FriendlyName { get; }
    public string TooltipText { get; }
    public Type EnumType { get; }
    public Func<ProfileSettings, object> Getter { get; }
    public Action<ProfileSettings, object> Setter { get; }
    public DropdownSetting(string friendlyName, string tooltipText, Type enumType, Func<ProfileSettings, object> getter, Action<ProfileSettings, object> setter)
    {
        FriendlyName = friendlyName;
        TooltipText = tooltipText;
        EnumType = enumType;
        Getter = getter;
        Setter = setter;
    }
}
