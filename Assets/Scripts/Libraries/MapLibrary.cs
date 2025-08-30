using Assets.Helpers;
using System.Collections.Generic;
using UnityEngine;

// Simple map library to resolve map images and metadata by name.
// Expand as needed (spawn points, bounds, etc.).
public static class MapLibrary
{
    private static Dictionary<string, Sprite> maps;
    private static bool loaded;

    public static Dictionary<string, Sprite> Maps
    {
        get
        {
            if (!loaded) Load();
            return maps;
        }
    }

    public static Sprite Get(string name)
    {
        if (!Maps.TryGetValue(name, out var data)) return null;
        return data;
    }

    private static void Load()
    {
        if (loaded) return;
        maps = new Dictionary<string, Sprite>
        {
            { "GreenValley", AssetHelper.LoadAsset<Sprite>("Maps/GreenValley") }
        };
        loaded = true;
    }
}
