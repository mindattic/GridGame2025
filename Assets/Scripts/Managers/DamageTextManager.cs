using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

public class DamageTextManager : MonoBehaviour
{
   

    //Fields
    private GameObject DamageTextPrefab;

    public void Awake()
    {
        DamageTextPrefab = PrefabRepo.Prefabs["DamageTextPrefab"];
    }

    public void Spawn(string text, Vector3 position, TextMotionStyle style = TextMotionStyle.Oscillate)
    {
        var prefab = Instantiate(DamageTextPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<DamageTextInstance>();
        instance.name = $"DamageText_{Guid.NewGuid():N}";
        instance.parent = g.Canvas3D.transform;
        instance.Spawn(text, position, style);
    }

    public void Clear()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag(Tag.DamageText).ToList();
        gameObjects.ForEach(x => Destroy(x));
    }

}
