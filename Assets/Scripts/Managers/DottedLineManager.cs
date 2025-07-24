using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using g = Assets.Helpers.GameManagerHelper;

public class DottedLineManager : MonoBehaviour
{

    //Fields
    private GameObject DottedLinePrefab;

    public List<DottedLineInstance> dottedLines = new List<DottedLineInstance>();

 
    void Awake()
    {
        DottedLinePrefab = PrefabRepo.Prefabs["DottedLinePrefab"];
    }


    private void ResetColors()
    {
        foreach(var dottedLine in dottedLines)
        {
            dottedLine.ResetColor();
        }
    }


    public void Spawn(DottedLineSegment segment, Vector2Int location)
    {
        GameObject prefab = Instantiate(DottedLinePrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<DottedLineInstance>();
        instance.name = $"DottedLine_{Guid.NewGuid():N}";
        instance.parent = g.Board.transform;
        instance.Spawn(segment, location);
        dottedLines.Add(instance);
    }

    public void Despawn(DottedLineInstance instance)
    {
        dottedLines.Remove(instance);
        Destroy(instance.gameObject);
    }

    public void Clear()
    {
        GameObject.FindGameObjectsWithTag(Tag.DottedLine).ToList().ForEach(x => Destroy(x));
    }

    //private void OnDestroy()
    //{
    //   if (onSelectedHeroLocationChanged != null)
    //       onSelectedHeroLocationChanged.RemoveListener(OnSelectedHeroLocationChanged);
    //}
}