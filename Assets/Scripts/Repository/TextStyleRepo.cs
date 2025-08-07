using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;



public static class TextStyleRepo
{
    private static Dictionary<string, TextStyle> textStyles;
    private static bool isLoaded = false;

    public static Dictionary<string, TextStyle> TextStyles
    {
        get
        {
            if (!isLoaded)
                Load();
            return textStyles;
        }
    }

    private static void Load()
    {
        if (isLoaded) return;

        textStyles = new Dictionary<string, TextStyle>
        {
            { "Damage", new TextStyle("Damage", FontRepo.Get("Damage"), 32, ColorHelper.Solid.White, TextMotion.Bounce) },
            { "Heal", new TextStyle("Heal", FontRepo.Get("Heal"), 32, ColorHelper.Solid.Green, TextMotion.Float) },
        };
    }

    /// <summary>
    /// Retrieves a single music track asynchronously by key.
    /// </summary>
    public static TextStyle Get(string key)
    {
        if (TextStyles.TryGetValue(key, out var entry))
            return entry;

        Debug.LogError($"Floating Text '{entry}' not found in TextStyleRepo.");
        return null;
    }
}
