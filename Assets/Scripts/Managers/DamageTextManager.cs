using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    //Quick Reference Properties
    protected Canvas canvas3D => GameManager.instance.canvas3D;


    //Fields
    private GameObject DamageTextPrefab;

    public void Awake()
    {
        DamageTextPrefab = PrefabRepo.instance.Prefabs["DamageTextPrefab"];
    }

    public void Spawn(string text, Vector3 position, DamageTextStyle style = DamageTextStyle.Oscillate)
    {
        var prefab = Instantiate(DamageTextPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<DamageTextInstance>();
        instance.name = $"DamageText_{Guid.NewGuid():N}";
        instance.parent = canvas3D.transform;
        instance.Spawn(text, position, style);
    }

    public void Clear()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag(Tag.DamageText).ToList();
        gameObjects.ForEach(x => Destroy(x));
    }

}
