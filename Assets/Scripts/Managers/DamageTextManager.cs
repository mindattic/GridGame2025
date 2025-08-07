using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

public class DamageTextManager : MonoBehaviour
{
    // Fields
    private GameObject DamageTextPrefab;

    public void Awake()
    {
        DamageTextPrefab = PrefabRepo.Prefabs["DamageTextPrefab"];
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

        var prefab = Instantiate(DamageTextPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<DamageTextInstance>();
        instance.name = $"DamageText_{Guid.NewGuid():N}";
        instance.parent = g.Canvas3D.transform;
        instance.Spawn(text, position, textStyle);
    }

    public void Clear()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag(Tag.DamageText).ToList();
        gameObjects.ForEach(x => Destroy(x));
    }
}
