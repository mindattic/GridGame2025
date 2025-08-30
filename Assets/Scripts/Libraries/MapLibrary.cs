using Assets.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public string Name;
    public Sprite Terrain;
    public Sprite Surface;
    public Sprite Canopy;
}



// Simple map library to resolve map images and metadata by name.
// Now stores MapData (Name, Terrain, Surface, Canopy).
// Keeps a legacy Get(name) that returns a Sprite for existing callers.
// Expand as needed (spawn points, bounds, etc.).
public static class MapLibrary
{
    private static Dictionary<string, MapData> maps;
    private static bool loaded;


    public static Dictionary<string, MapData> Maps
    {
        get
        {
            if (!loaded) Load();
            return maps;
        }
    }

    public static MapData Get(string name)
    {
        if (!Maps.TryGetValue(name, out var data)) return null;
        return data;
    }

    public static MapData Get(Map map)
    {
        var name = map.ToString();
        if (!Maps.TryGetValue(name, out var data)) return null;
        return data;
    }

    private static void Load()
    {
        if (loaded) return;

        maps = new Dictionary<string, MapData>();

        // Load Test map
        MapData mapData = Create(Map.Test);
        maps[mapData.Name] = mapData;

        // Load GreenValley map
        mapData = Create(Map.GreenValley);
        maps[mapData.Name] = mapData;

        loaded = true;
    }

    private static MapData Create(Map map)
    {
        string name = map.ToString();
        var terrain = AssetHelper.LoadAsset<Sprite>($"Maps/{name}/Terrain");
        var surface = AssetHelper.LoadAsset<Sprite>($"Maps/{name}/Surface");
        var canopy = AssetHelper.LoadAsset<Sprite>($"Maps/{name}/Canopy");

        return new MapData
        {
            Name = name,
            Terrain = terrain,
            Surface = surface,
            Canopy = canopy
        };
    }


}
