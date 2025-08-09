using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class CombatTextManager : MonoBehaviour
{
    // Fields
    private GameObject CombatTextPrefab;

    public void Awake()
    {
        CombatTextPrefab = PrefabRepo.Prefabs["CombatTextPrefab"];
    }

    /// <summary>
    /// Spawns a floating text using a profile key (e.g., "Damage", "Healing", etc.)
    /// </summary>
    public void Spawn(string text, Vector3 position, string styleKey = "Damage")
    {
        var textStyle = TextStyleRepo.Get(styleKey);
        if (textStyle == null)
        {
            Debug.LogError($"Text style '{styleKey}' not found. Falling back to default profile.");
            textStyle = TextStyleRepo.Get("Damage"); // fallback
            if (textStyle == null) return;
        }

        var prefab = Instantiate(CombatTextPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<CombatTextInstance>();
        instance.name = $"DamageText_{Guid.NewGuid():N}";
        instance.parent = g.Canvas3D.transform;
        instance.Spawn(text, position, textStyle);
    }

    /// <summary>
    /// Clears all combat text objects from the scene without using tags.
    /// </summary>
    public void Clear()
    {
        var instances = GameObject.FindObjectsByType<CombatTextInstance>(FindObjectsSortMode.None);
        foreach (var instance in instances)
        {
            Destroy(instance.gameObject);
        }
    }
}
