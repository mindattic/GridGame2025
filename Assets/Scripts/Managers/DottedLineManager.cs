using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DottedLineManager : MonoBehaviour
{
    protected float tileSize => GameManager.instance.tileSize;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected BoardInstance board => GameManager.instance.board;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedHero;
    protected bool hasSelectedPlayer => GameManager.instance.hasSelectedPlayer;
  

    //Fields
    [SerializeField] public GameObject DottedLinePrefab;

    public List<DottedLineInstance> dottedLines = new List<DottedLineInstance>();

    //Method which is automatically called before the first frame update  
    void Start()
    {
  
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
        instance.parent = board.transform;
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
    //   if (onSelectedPlayerLocationChanged != null)
    //       onSelectedPlayerLocationChanged.RemoveListener(OnSelectedPlayerLocationChanged);
    //}
}