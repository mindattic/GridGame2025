using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //Quick Reference Properties
    protected List<TileInstance> tiles => GameManager.instance.tiles;
    protected TileMap tileMap => GameManager.instance.tileMap;


    public void Awake()
    {
        
    }

    public void Start()
    {
        
    }


    public void Reset()
    {
        foreach (var tile in tiles)
        {
            tile.spriteRenderer.color = ColorHelper.Translucent.White;
        }
    }


    public void OnLocationChanged(Vector2Int previousLocation, Vector2Int newLocation)
    {
        tileMap.GetTile(previousLocation).color = ColorHelper.Translucent.White;
        tileMap.GetTile(newLocation).color = ColorHelper.Translucent.Yellow;
    }


}
