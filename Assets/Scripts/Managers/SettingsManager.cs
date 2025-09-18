using Assets.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;
using Label = TMPro.TextMeshProUGUI;
using TMPro;
using Game.Models.Profile;
using Assets.Helpers;
using Assets.Scripts.Libraries;

public class SettingsManager : MonoBehaviour
{
    [Header("Prefabs")] public GameObject sliderPrefab;
    public GameObject togglePrefab;
    public GameObject dropdownPrefab;

    [Header("Layout References")] public RectTransform contentRoot;

    private void Awake()
    {
        if (!Assets.Helpers.ProfileHelper.HasProfiles()) return;


        sliderPrefab = PrefabLibrary.Prefabs.GetValueOrDefault("SettingSlider");
        togglePrefab = PrefabLibrary.Prefabs.GetValueOrDefault("SettingToggle");
        dropdownPrefab = PrefabLibrary.Prefabs.GetValueOrDefault("SettingDropdown");
        contentRoot = GameObject.Find(GameObjectHelper.Settings.Content).GetComponent<RectTransform>();

        BuildSettingsUI();

    }

    private void Start()
    {
        scene.FadeIn();
    }

    private void BuildSettingsUI()
    {
        if (contentRoot == null)
        {
            Debug.LogError("SettingsManager: contentRoot not assigned");
            return;
        }
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        var settings = Assets.Helpers.ProfileHelper.CurrentProfile.Settings;
        var type = typeof(ProfileSettings);
        var members = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var f in members)
        {
            var displayAttr = f.GetCustomAttribute<SettingDisplayNameAttribute>();
            string displayName = displayAttr?.Name ?? f.Name;
            Type ft = f.FieldType;

            if (ft == typeof(float) || ft == typeof(int))
            {
                var range = f.GetCustomAttribute<SettingRangeAttribute>();
                float min = range?.Min ?? 0f;
                float max = range?.Max ?? (ft == typeof(int) ? 10f : 1f);
                float increment = range?.Increment > 0f ? range.Increment : 0f; // 0 means continuous
                CreateSlider(displayName, Convert.ToSingle(f.GetValue(settings)), min, max, increment, (val) =>
                {
                    if (ft == typeof(int)) f.SetValue(settings, Mathf.RoundToInt(val));
                    else f.SetValue(settings, val);
                    Assets.Helpers.ProfileHelper.SaveSettings();
                }, ft == typeof(int));
            }
            else if (ft == typeof(bool))
            {
                CreateToggle(displayName, (bool)f.GetValue(settings), (val) =>
                {
                    f.SetValue(settings, val);
                    Assets.Helpers.ProfileHelper.SaveSettings();
                });
            }
            else if (ft.IsEnum)
            {
                CreateDropdown(displayName, ft, f.GetValue(settings), (val) =>
                {
                    f.SetValue(settings, val);
                    Assets.Helpers.ProfileHelper.SaveSettings();
                });
            }
            else
            {
                Debug.LogWarning($"Unsupported setting type: {f.Name} {ft.Name}");
            }
        }
    }

    private void CreateSlider(string label, float current, float min, float max, float increment, Action<float> onChanged, bool asInt)
    {
        if (sliderPrefab == null) { Debug.LogError("Slider prefab not set"); return; }
        var go = Instantiate(sliderPrefab, contentRoot);
        var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0) texts[0].text = label;
        var slider = go.GetComponentInChildren<Slider>();
        var valueLabel = texts.Length > 1 ? texts[1] : null;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = Mathf.Clamp(current, min, max);
        void updateValue(float v)
        {
            float snapped = v;
            if (increment > 0f)
            {
                snapped = Mathf.Round((v - min) / increment) * increment + min;
                snapped = Mathf.Clamp(snapped, min, max);
                if (Mathf.Abs(snapped - v) > 0.0001f)
                {
                    slider.SetValueWithoutNotify(snapped);
                }
            }
            valueLabel?.SetText(asInt ? Mathf.RoundToInt(snapped).ToString() : snapped.ToString(increment >= 0.1f ? "0.0" : "0.00"));
            onChanged(snapped);
        }
        updateValue(slider.value);
        slider.onValueChanged.AddListener(v => updateValue(v));
    }

    private void CreateToggle(string label, bool current, Action<bool> onChanged)
    {
        if (togglePrefab == null) { Debug.LogError("Toggle prefab not set"); return; }
        var go = Instantiate(togglePrefab, contentRoot);
        var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0) texts[0].text = label;
        var toggle = go.GetComponentInChildren<Toggle>();
        toggle.isOn = current;
        toggle.onValueChanged.AddListener(v => onChanged(v));
    }

    private void CreateDropdown(string label, Type enumType, object current, Action<object> onChanged)
    {
        if (dropdownPrefab == null) { Debug.LogError("Dropdown prefab not set"); return; }
        var go = Instantiate(dropdownPrefab, contentRoot);
        var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0) texts[0].text = label;
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

    public void OnBackButtonClicked() => scene.Fade.ToPreviousScene();
}
